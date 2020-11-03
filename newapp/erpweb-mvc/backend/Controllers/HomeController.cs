using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using erp.DAL.EF.New;
using erp.Model.Dal.New;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL, IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL,
            IClientPagesAllocatedDAL clientPagesAllocatedDAL, IAccountService accountService, IUnitOfWork unitOfWork ) :
            base(unitOfWork,loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService)
        {

        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

		
        public ActionResult New()
        {
            return View("NewIndex");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}
