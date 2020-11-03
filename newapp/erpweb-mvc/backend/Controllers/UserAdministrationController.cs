using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize(Roles="Administrator")]
    public class UserAdministrationController : BaseController
    {
        private readonly IRoleDAL roleDAL;

        public UserAdministrationController(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL,
            ICompanyDAL companyDAL, IAccountService accountService, IRoleDAL roleDAL) : 
            base(unitOfWork,loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL,accountService )
        {
            this.roleDAL = roleDAL;
        }

        // GET: UserAdministration
        public ActionResult Index()
        {
            var model = BuildModel();
            ViewBag.breadcrumbs = new List<BreadCrumb> { new BreadCrumb { Text = "User and role administration" } };
            return View(model);
        }

        private UserAdministrationModel BuildModel()
        {
            var model = new UserAdministrationModel { Roles = roleDAL.GetAll() };
            var navigationItems = unitOfWork.NavigationItemRepository.Get().ToList();
            model.TreeNodes = BuildTree(navigationItems);
            return model;
        }

        private List<TreeNode> BuildTree(List<Navigation_item> items)
        {
            Dictionary<int, TreeNode> lookup = new Dictionary<int, TreeNode>();
            items.ForEach(x => lookup.Add(x.id, new TreeNode {id = x.id, text = x.text, Item = GetNavigationItemUIObject(x) }));
            foreach (var item in lookup.Values) {
                TreeNode proposedParent;
                if (item.Item.parent_id != null && lookup.TryGetValue(item.Item.parent_id.Value, out proposedParent)) {
                    item.Parent = proposedParent;
                    item.parent_id = proposedParent.id;
                    if (proposedParent.children == null)
                        proposedParent.children = new List<TreeNode>();
                    proposedParent.children.Add(item);
                }
            }
            foreach (var item in lookup.Values)
            {
                item.Parent = null;
            }
            return lookup.Values.Where(x => x.parent_id == null).ToList();
        }

        public ActionResult GetUsersInRole(int role_id)
        {
            return Json(roleDAL.GetUsersInRole(role_id));
        }

        
        public ActionResult GetNavigationItemPermissions(int id)
        {
            var items = unitOfWork.NavigationItemPermissionRepository.Get(i=>i.navigation_item_id == id, includeProperties: "Role, User")
                .Select(GetUIObject)
                .ToList();
            return Json(items);
        }

        public ActionResult AddPermissionForRole(int item_id, int role_id)
        {
            var item = new Navigation_item_permission { navigation_item_id = item_id, role_id = role_id };
            unitOfWork.NavigationItemPermissionRepository.Insert(item);
            unitOfWork.Save();
            item = unitOfWork.NavigationItemPermissionRepository.Get(pi => pi.id == item.id, includeProperties: "Role, User").FirstOrDefault();
            return Json(GetUIObject(item));
        }

        public ActionResult AddPermissionForUser(int item_id, int user_id, bool remove)
        {
            var item = new Navigation_item_permission { navigation_item_id = item_id, user_id = user_id, remove = remove };
            unitOfWork.NavigationItemPermissionRepository.Insert(item);
            unitOfWork.Save();
            item = unitOfWork.NavigationItemPermissionRepository.Get(pi => pi.id == item.id, includeProperties: "Role, User").FirstOrDefault();
            return Json(GetUIObject(item));
        }

        public ActionResult RemovePermission(int id)
        {
            unitOfWork.NavigationItemPermissionRepository.DeleteByIds(new[] { id });
            return Json("OK");
        }

        [HttpGet]
        public ActionResult GetNavigationItemsForRole(int role_id)
        {
            return
                Json(BuildTree(GetNavigationItems(unitOfWork.NavigationItemRepository.Get().ToList(),new List<int?>() {role_id})
                    .Select(GetNavigationItemUIObject).ToList()
                    ), JsonRequestBehavior.AllowGet);
        }

        private Navigation_item GetNavigationItemUIObject(Navigation_item ni)
        {
            return new Navigation_item
            {
                id = ni.id,
                image_url = ni.image_url,
                parent_id = ni.parent_id,
                text = ni.text,
                url = ni.url
            };
        }

        [HttpGet]
        public ActionResult GetNavigationItemsForUser(int user_id)
        {
            return
                Json(BuildTree(GetNavigationItems(unitOfWork.NavigationItemRepository.Get().ToList(),user_id:user_id)
                .Select(GetNavigationItemUIObject).ToList()
                ), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNavigationItems()
        {
            return Json(BuildTree(unitOfWork.NavigationItemRepository.Get().ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUserToRole(int user_id, int role_id)
        {            
            var user = unitOfWork.UserRepository.Get(u => u.userid == user_id).FirstOrDefault();
            var role = unitOfWork.RoleRepository.Get(r => r.id == role_id).FirstOrDefault();
            if (user != null && role != null)
            {
                if (user.Roles == null)
                    user.Roles = new List<Role>();
                user.Roles.Add(role);
                unitOfWork.Save();
            }
            return Json("OK");
        }

        public ActionResult RemoveUserFromRole(int user_id, int role_id)
        {
            unitOfWork.UserRepository.RemoveUserFromRole(user_id, role_id);
            return Json("OK");
        }

        public object GetUIObject(Navigation_item_permission pi)
        {
            return new
            {
                pi.role_id,
                pi.navigation_item_id,
                pi.user_id,
                pi.id,
                pi.remove,
                Role = pi.Role != null ? new
                {
                    pi.Role.id,
                    pi.Role.name
                } : null,
                User = pi.User != null ? new
                {
                    pi.User.username,
                    pi.User.userwelcome
                } : null
            };
        }

    }
}