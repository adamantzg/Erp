using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using erp.DAL.EF.New;
using erp.Model;
using System.Web.Security;
using System.IO;
using erp.Model.Dal.New;
using company.Common;
using backend.Models;
using backend.Properties;
using Utilities = company.Common.Utilities;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class CommonController : BaseController
    {
        
        private readonly ILoginHistoryDetailDAL loginHistoryDetailDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IAdminPagesDAL adminPagesDAL;
        private readonly IAdminPagesNewDAL adminPagesNewDAL;
        private readonly IClientPagesAllocatedDAL clientPagesAllocatedDAL;
        private readonly ICategory1DAL category1DAL;
        private readonly IUserDAL userDAL;
        private readonly ICustproductsDAL custproductsDAL;
        private readonly IBrandsDAL brandsDAL;
        private readonly ILoginhistoryDAL loginhistoryDAL;
        private readonly IAccountService accountService;
        private readonly ICompanyDAL companyDAL1;

        public CommonController(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL,
            ICategory1DAL category1DAL, IUserDAL userDAL, ICustproductsDAL custproductsDAL, IBrandsDAL brandsDAL, 
            ILoginhistoryDAL loginhistoryDAL, IAccountService accountService)
            : base(unitOfWork,loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService)
        {
            
            this.loginHistoryDetailDAL = loginHistoryDetailDAL;
            this.companyDAL = companyDAL;
            this.adminPagesDAL = adminPagesDAL;
            this.adminPagesNewDAL = adminPagesNewDAL;
            this.clientPagesAllocatedDAL = clientPagesAllocatedDAL;
            this.category1DAL = category1DAL;
            this.userDAL = userDAL;
            this.custproductsDAL = custproductsDAL;
            this.brandsDAL = brandsDAL;
            this.loginhistoryDAL = loginhistoryDAL;
            this.accountService = accountService;
            companyDAL1 = companyDAL;
        }

        public ActionResult GetUsersLookupCompletionForAreaJSON(string prefixText, int count, int area_id)
        {
            var  userList = userDAL.GetUsersForArea(area_id);
            return Json((from u in userList where u.userwelcome.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase) select new LookupItemUI { id = u.userid, value = u.userwelcome, label = u.userwelcome }).Take(count).ToArray());
        }

        public ActionResult GetUsersByCriteria(string prefixText)
        {
            return Json(userDAL.GetUsersByCriteria(prefixText).Select(u => new { u.userid, u.userwelcome, u.username }).ToList());
        }
        public ActionResult GetUsersByCriteriaExpand(string prefixText)
        {
            return Json(userDAL.GetUsersByCriteria(prefixText).Select(u => new { u.userid, u.userwelcome, u.username, u.company_id }).ToList());

        }
        public ActionResult GetUsersQC(string prefixText)
        {
            return Json(userDAL.GetUsersByCriteria(prefixText).Where(u=>u.admin_type == erp.Model.User.adminType_Qc && u.status_flag == 0).Select(u => new { u.userid, u.userwelcome, u.username, u.admin_type, u.status_flag }).ToList());
        }


        public ActionResult GetProductsForCompanyCompletionJSON(string prefixText, int count, int company_id)
        {
            var productList = custproductsDAL.GetByCompany(company_id,prefixText);
            return Json((from p in productList orderby p.cprod_code1
                         select new LookupItemUI { id = p.cprod_id, value = p.cprod_code1 + " - " + p.cprod_name, label = p.cprod_code1 + " - " + p.cprod_name }).Take(count).ToArray());
        }

        public ActionResult GetProductsForDistributorCompletionJSON(string prefixText, int count, int distributor_id)
        {
            var productList = custproductsDAL.GetByDistributor(distributor_id);
            return Json((from p in productList
                         where (p.cprod_name.ToLower().Contains(prefixText.ToLower()) || p.cprod_code1.ToLower().Contains(prefixText.ToLower()) )
                         orderby p.cprod_code1
                         select new LookupItemUI { id = p.cprod_id, value = p.cprod_code1 + " - " + p.cprod_name, label = p.cprod_code1 + " - " + p.cprod_name }).Take(count).ToArray());
        }

        

        public ActionResult GetCustProductsForBrandCompletionJSON(string prefixText, int count, int brand_id)
        {
            var b = brandsDAL.GetById(brand_id);
            var productList = custproductsDAL.GetByCompany(b.user_id.Value, prefixText);
            //return Json((from p in productList
            //             orderby p.web_name
            //             select new LookupItemUI { id = p.web_unique, value = p.web_name + (!string.IsNullOrEmpty(p.option_name) ? " " + p.option_name : string.Empty), label = p.web_name + (!string.IsNullOrEmpty(p.option_name) ? " " + p.option_name : string.Empty) }).Take(count).ToArray());
            return Json(productList.Select(p => new { p.cprod_id, p.cprod_name, p.cprod_code1}).Take(count).ToArray());
        }

        

        public ActionResult GetProductsForCategoriesJSON(string selectedCats)
        {
            var products = custproductsDAL.GetForCatIds((from catid in selectedCats.Split(',') select Int32.Parse(catid)).ToList()).OrderBy(p => p.cprod_name);
            //Copy only necessary values to new list to minimize network traffic
            var optimized = new List<Cust_products>();
            foreach (var prod in products)
            {
                optimized.Add( new Cust_products
                {
                    cprod_id = prod.cprod_id,
                    cprod_name = prod.cprod_name,
                    cprod_retail = prod.cprod_retail,
                    cprod_image1 = prod.cprod_image1,
                    cprod_stock_code = prod.cprod_stock_code,
                    cprod_code1 = prod.cprod_code1,
                    moq = prod.moq,
                    MastProduct = new Mast_products { units_per_40nopallet_gp = prod.MastProduct.units_per_40nopallet_gp, price_dollar = prod.MastProduct.price_dollar,
                                price_pound = prod.MastProduct.price_pound, tariff_code = prod.MastProduct.tariff_code, prod_image1 = prod.MastProduct.prod_image1,
                                Factory = new Company {consolidated_port = prod.MastProduct.Factory.consolidated_port } }
                });
            }
            return Json(optimized);
        }

        public ActionResult GetCustProductsJSON(string prefixText)
        {
            return Json(custproductsDAL.GetCustProductsByCriteria(prefixText).Where(p => !string.IsNullOrEmpty(p.cprod_code1.Trim()))
                .Select(p => new { p.cprod_id, cprod_code1 = !string.IsNullOrEmpty(p.Client.customer_code) ? $"{p.cprod_code1} ({p.Client.customer_code})" : p.cprod_code1, p.cprod_name }));
        }


        public ActionResult GetProductsOrdered(string criteria)
        {
            var company_Id = accountService.GetCurrentUser().company_id;
            //handle Bathstore exception (200,37 are bathstore ids)
            var company_ids = company_Id == 200 || company_Id == 37
                                  ? new[] {200, 37}.ToList()
                                  : new[] {accountService.GetCurrentUser().company_id}.ToList();

            var products = custproductsDAL.GetProductsOrdered(company_ids,criteria).Select(p=>new {p.cprod_id,p.cprod_name,p.cprod_code1}).OrderBy(p => p.cprod_name);
            return Json(products,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClients(string prefixText)
        {
            return Json(companyDAL.GetClients(prefixText).OrderBy(c=>c.user_name).ToList());
        }

        public ActionResult GetClientFactories(string prefixText)
        {
            //This method is called from Invoice/Edit.cshtml
            IList<Company_User_Type> userTypes = new List<Company_User_Type>();
            userTypes.Add(Company_User_Type.Client);
            userTypes.Add(Company_User_Type.Base);

            return Json(companyDAL.GetClients(prefixText, userTypes).Union(companyDAL.GetFactories(prefixText)).OrderBy(c => c.user_name).ToList());
        }

        public ActionResult GetFactoriesForClient(int client_id)
        {
            var clientIds = new List<int>();
            if (client_id != -1)
                clientIds.Add(client_id);
            else {
                //Brands
                clientIds.AddRange(brandsDAL.GetAll().Where(b=>b.user_id != null).Select(b => b.user_id.Value));
            }
            return Json(companyDAL.GetFactoriesForClients(clientIds).Where(f=>!string.IsNullOrEmpty(f.factory_code)).OrderBy(f=>f.factory_code).Select(f=>new {f.user_id, f.user_name, f.factory_code }));
        }





        /// <summary>
        /// Asp bridge between asp pages and this app
        /// asp page should have invisible image whose source points to this method
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult AspBridge()
        {
            string session_id = String.Empty + Request["session_id"];
            string sdate = String.Empty + Request["datenow"];
            if (!String.IsNullOrEmpty(session_id) && !String.IsNullOrEmpty(sdate))
            {
                DateTime date;
                if (DateTime.TryParse(sdate, out date))
                {
                    Login_history login_hist = loginhistoryDAL.GetByCriteria(session_id, date);
                    if (login_hist != null)
                    {
                        FormsAuthentication.SetAuthCookie(login_hist.login_username, false);
                        Session["session_id"] = session_id;
                        Session["datenow"] = date.ToString("yyyy-MM-dd HH':'mm':'ss");;
                    }
                }
            }
            return File(new byte[] {}, "image/gif" );
        }

        [AllowAnonymous]
        public ActionResult Image(string url, string brand_code, string type, bool comp = false, bool category=false)
        {
            string filePath;
            if (!comp)
            {
                if (!category)
                {
                    //products
                    if (!String.IsNullOrEmpty(type))
                    {
                        if(type != "mobile")
                            url = Path.GetFileNameWithoutExtension(url) + "_" + type + Path.GetExtension(url);
                        else
                        {
                            url = Settings.Default.mobileAppImagesSubfolder + "/" + url;
                        }
                    }
                    filePath =
                        Server.MapPath(String.Format("{0}/web/{1}/{2}", Settings.Default.imagesRootFolder,
                                                     brand_code, url));
                }
                else
                {
                    //cats
                    if (!String.IsNullOrEmpty(type))
                        url = Path.GetFileNameWithoutExtension(url) + "_" + type + Path.GetExtension(url);
                    filePath =
                        Server.MapPath(String.Format("{0}/web/{1}/category/{2}", Settings.Default.imagesRootFolder,
                                                     brand_code, url));
                }
            }
            else
            {
                //components
                filePath = Server.MapPath(url);
            }
            if (System.IO.File.Exists(filePath))
                return File(filePath, WebUtilities.GetMIMEType(filePath));
            else
            {
                return null;
            }
        }

        public ActionResult ConvertWebPageToPdf()
        {
            return View(new ConvertToPdfModel { Options = "debug=true,scale=0.78, leftmargin=22,rightmargin=22,media=1" });
        }

        public static string GetReturnReasonFullName(string reason)
        {
            var values = Settings.Default.InspectionCategories.Split(',');
            return String.Empty + values.FirstOrDefault(v => v.StartsWith(reason));
        }
    }

    public class LookupItemUI : LookupItem
    {
        public string label { get; set; }
        public string code  { get; set; }
    }

    public class LookupItemUIEx
    {
        public string id { get; set; }
        public string value { get; set; }
        public string label { get; set; }
    }

}
