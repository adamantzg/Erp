using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using backend.Models;
using backend.Properties;
using Utilities = company.Common.Utilities;
using ASPPDFLib;
using backend.ApiServices;

namespace backend.Controllers
{
    
    [Authorize]
    public class ChangeNoticeV2Controller : BaseController
    {
        
        private readonly ILoginHistoryDetailDAL loginHistoryDetailDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IAdminPagesDAL adminPagesDAL;
        private readonly IAdminPagesNewDAL adminPagesNewDAL;
        private readonly IClientPagesAllocatedDAL clientPagesAllocatedDAL;
        private readonly ICategory1DAL category1DAL;
        private readonly IAccountService accountService;

        //
        // GET: /ChangeNotice/

        public ChangeNoticeV2Controller(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL,
            ICategory1DAL category1DAL, IAccountService  accountService)
            : base(unitOfWork, loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService)
        {
            
            this.loginHistoryDetailDAL = loginHistoryDetailDAL;
            this.companyDAL = companyDAL;
            this.adminPagesDAL = adminPagesDAL;
            this.adminPagesNewDAL = adminPagesNewDAL;
            this.clientPagesAllocatedDAL = clientPagesAllocatedDAL;
            this.category1DAL = category1DAL;
            this.accountService = accountService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetTest()
        {
            return "TEST CNV2";
        }

        private ChangeNoticeV2Model CreateChangeNoticeV2Model()
        {
            var model = new ChangeNoticeV2Model
            {
                Categories = category1DAL.GetAll().ToList(),
                Clients = companyDAL.GetClientsFromProducts().ToList(),
                Factories = companyDAL.GetFactories().ToList(),
                ChangeNoticeCategories = unitOfWork.ReturnCategoryRepository.Get().ToList(),
                ChangeNoticeReasons = unitOfWork.ChangeNoticeReasonsRepository.Get().ToList()
            };

            return model;
        }

       public ActionResult ViewPdf(int id)
       {
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();

            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("RenderForPdf", new { id}), "scale=0.78, leftmargin=22,rightmargin=22,media=1");
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf");

       }

        [AllowAnonymous]
        public ActionResult RenderForPdf(string state="view",int id =157)
        {
            var model = new ChangeNoticeV2Model
            {
                Categories = category1DAL.GetAll().ToList(),
                Clients = companyDAL.GetClientsFromProducts().ToList(),
                Factories = companyDAL.GetFactories().ToList(),
                ChangeNoticeCategories = unitOfWork.ReturnCategoryRepository.Get().ToList(),
                ChangeNoticeReasons = unitOfWork.ChangeNoticeReasonsRepository.Get().ToList(),
                Notice = state == "create" ? new Change_notice { Images = new List<change_notice_image>(), Allocations = new List<Change_notice_allocation>(), expectedReadyDate = DateTime.Now, Document = new change_notice_document() } :
                            state == "edit" || state == "view" ? unitOfWork.ChangeNoticeRepository.Get(n => n.id == id, includeProperties: "Allocations.Product.BrandCompany,Allocations.Orders.Client,Images,Document").FirstOrDefault() : null,
                ImageRootFolder = Settings.Default.ChangeNoticeRootFolder
            };
            if (id > 0)
            {
                var cprod_id = state == "create" ? id : model.Notice.Allocations.FirstOrDefault()?.cprod_id;

                if (cprod_id != null)
                {
                    model.Product = unitOfWork.CustProductRepository.Get(p => p.cprod_id == cprod_id, includeProperties: "BrandCompany,MastProduct").FirstOrDefault();
                    model.factory_id = model.Product?.MastProduct?.factory_id;

                }
            }

            return View("PrintView",model);
        }

    }
}