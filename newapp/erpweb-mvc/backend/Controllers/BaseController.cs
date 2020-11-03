using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using backend.Models;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using erp.Model;
using company.Common;
using backend.Properties;
using System.Net.Http;
using backend.ApiServices;

namespace backend.Controllers
{
    [HandleError]
    public class BaseController : Controller
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ILoginHistoryDetailDAL loginHistoryDetailDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IAdminPagesDAL adminPagesDAL;
        private readonly IAdminPagesNewDAL adminPagesNewDAL;
        private readonly IClientPagesAllocatedDAL clientPagesAllocatedDAL;
        private readonly IAccountService accountService;

        //
        // GET: /Base/

        public BaseController()
        {
            
        }

        public BaseController(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL, IAdminPagesDAL adminPagesDAL,
            IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL, IAccountService accountService)
        {
            this.unitOfWork = unitOfWork;
            this.loginHistoryDetailDAL = loginHistoryDetailDAL;
            this.companyDAL = companyDAL;
            this.adminPagesDAL = adminPagesDAL;
            this.adminPagesNewDAL = adminPagesNewDAL;
            this.clientPagesAllocatedDAL = clientPagesAllocatedDAL;
            this.accountService = accountService;
            ViewBag.User = accountService.GetCurrentUser();
        }

		public User CurrentUser
        {
            get { return accountService.GetCurrentUser(); }
        }
				

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.MenuItems = GetMenuItemsForUser(filterContext);
            
            var session_id = Session?["session_id"];
            var user = accountService.GetCurrentUser();
            if (session_id == null && Request.IsAuthenticated && user != null)
            {
                accountService.LoginUser(user);
            }

            ViewBag.session_id = Session?["session_id"];
            ViewBag.CurrentUser = CurrentUser;
            var history_id = (int?) Session?["history_id"];
            ViewBag.datenow = Session?["datenow"];
            if (history_id != null /*&& Request.Path != "/Common/AspBridge"*/)
                loginHistoryDetailDAL.Create(new Login_history_detail
                {
                    history_id = (int?)history_id,
                    visit_page = $"/{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}/{filterContext.ActionDescriptor.ActionName}",
                    visit_URL = ("?" + string.Join("&", filterContext.ActionParameters.Select(p=>$"{p.Key}={p.Value}"))).Left(4000),
                    visit_time = DateTime.Now
                
                });

            base.OnActionExecuting(filterContext);
        }

        

        private void CheckCurrent(SiteMenuItem item, string requestPath)
        {
            if (requestPath.StartsWith(item.Url, StringComparison.InvariantCultureIgnoreCase))
                ViewBag.CurrentItem = item;
        }

        private List<SiteMenuItem> BuildChildItems(int page_id, List<Admin_pages_new> pagesNew)
        {
            var childPages = pagesNew.Where(p => p.parent_id == page_id);
            var children = childPages.Select(p => new SiteMenuItem { Url = MakeUrl(p), Text = p.page_title, DataItem = p, ImageUrl = p.icon}).ToList();
            foreach (var menuItem in children)
            {
                CheckCurrent(menuItem, Request.Path);
                menuItem.Children = BuildChildItems((menuItem.DataItem as IAdminpages).page_id, pagesNew);
            }
            return children;
        }

        private List<SiteMenuItem> GetMenuFromClientPages()
        {
            var pages = clientPagesAllocatedDAL.GetByPageAndUser(accountService.GetCurrentUser()?.userid ?? 0);
            var items = new List<SiteMenuItem>();
            //Build top level
            items.AddRange(pages.GroupBy(p => p.Page.sub_level).Select(g => new SiteMenuItem { Url = MakeUrl(g.First().Page), Text = g.First().Page.sub_level }));
            foreach (var item in items)
            { 
                item.Children =
                    pages.Where(p => p.Page.sub_level == item.Text)
                         .Select(p => new SiteMenuItem {Url = MakeUrl(p.Page), Text = p.Page.sub_sub_level, Parent = item})
                         .ToList();
                SiteMenuItem child;
                if ((child = item.Children.FirstOrDefault(c => Request.Path.ToString().StartsWith(c.Url))) != null)
                    ViewBag.CurrentItem = child;
            }
            return items;
        }

        private string MakeUrl(Client_page page)
        {
            string url = page.page_URL;
            if (url.Contains(".asp"))
            {
                url = Properties.Settings.Default.aspsite_root + (url.StartsWith("/") ? url :  "/2012_client_application/" + url);
            }
            if (!string.IsNullOrEmpty(page.parameter1))
                url += "?" + string.Format("{0}={1}", page.parameter1, page.parameter1_value);
            return url;
        }

        private string MakeUrl(IAdminpages page)
        {
            string url = page.page_URL;
            if (url.Contains(".asp"))
            {
                url = Properties.Settings.Default.aspsite_root + (url.StartsWith("/") ? url :  "/backend/" + url);
            }
            if (!string.IsNullOrEmpty(page.parameter1))
                url += "?" + string.Format("{0}={1}", page.parameter1, page.parameter1_value);
            return url;
        }

        protected virtual void SaveChartImage(string name, Chart chart, Guid chartKey, string folder = null)
        {
            if (folder == null)
                folder = Settings.Default.Analytics_ImagesFolder;
            var fullPath = Server.MapPath(folder);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            chart.SaveImage(
                Path.Combine(fullPath,
                    string.Format("{0}_{1}.jpg", /*DateTime.Today.ToString("yyyyMMdd")*/chartKey, name)),
                ChartImageFormat.Jpeg);
        }

        private List<Navigation_item> GetMenuItemsForUser(ActionExecutingContext context)
        {
            var navigationItems = unitOfWork.NavigationItemRepository.Get().ToList();
            var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = context.ActionDescriptor.ActionName;
            var url = $"/{controller}/{action}".ToLower();
            var url2 = url;
            if (context.ActionDescriptor.ActionName == "Index")
                url2 = ("/" + context.ActionDescriptor.ControllerDescriptor.ControllerName).ToLower();
            if (controller.ToLower() == "a5")
                url = context.HttpContext.Request.RawUrl;

            var user = accountService.GetCurrentUser();
            var user_id = user?.userid;
            if (user_id != null)
            {
                //var userRoles = UserDAL.GetDynamicUserRoles(WebUtilities.GetCurrentUser().username);
                var roleIds = user.Roles.Select(r => (int?) r.id).ToList();
                var result = GetNavigationItems(navigationItems, roleIds, user_id);

                var active = result.FirstOrDefault(r => r.url != null && (r.url.ToLower().StartsWith(url) || r.url.ToLower().StartsWith(url2)));
                if (active != null)
                    active.Active = true;
                return result;
            }
            return null;
        }

        public List<Navigation_item> GetNavigationItems(List<Navigation_item> navigationItems, List<int?> roleIds = null, int? user_id = null)
        {
            var permissions = unitOfWork.NavigationItemPermissionRepository.Get().ToList();
            List<Navigation_item_permission> effectivePermissions = new List<Navigation_item_permission>();
            if (roleIds != null)
            {
                effectivePermissions = permissions.Where(p => roleIds.Contains(p.role_id)).ToList();
            }
            if(user_id != null)
            {
                effectivePermissions.AddRange(permissions.Where(p => p.role_id == null &&  p.user_id == user_id && p.remove == false));
                var remove = permissions.Where(p => p.user_id == user_id && p.remove == true).ToList();
                effectivePermissions = effectivePermissions.Where(p => remove.Count(x => x.id == p.id) == 0).ToList();
            }                                     

            var items = navigationItems.Where(ni => effectivePermissions.FirstOrDefault(p => p.navigation_item_id == ni.id) != null).ToList();
            //check parents
            items.AddRange(navigationItems.Where(i => i.parent_id == null && items.Count(x => x.id == i.id) == 0 && items.Any(x => x.parent_id == i.id)));
            return items;
        }

        

    }
}
