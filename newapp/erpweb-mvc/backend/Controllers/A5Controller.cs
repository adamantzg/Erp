using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using erp.DAL.EF.New;
using erp.Model.Dal.New;
using backend.ApiServices;
using backend.Models;

namespace backend.Controllers
{
    public class A5Controller : BaseController
    {
        

        public A5Controller(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL,IClientPagesAllocatedDAL clientPagesAllocatedDAL, IAccountService accountService) 
            : base(unitOfWork, loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService)
        {
            
        }

        // GET: A5
		[Authorize]
        public ActionResult Index()
        {
            ViewBag.breadcrumbs = new List<BreadCrumb> { new BreadCrumb { Text = "[a5]" } };
            return View();
        }
    }
}