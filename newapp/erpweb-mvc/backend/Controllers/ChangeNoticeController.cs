using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using backend.Models;
using backend.Properties;
using Utilities = company.Common.Utilities;
using backend.ApiServices;

namespace backend.Controllers
{
    public class ChangeNoticeController : BaseController
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

        public ChangeNoticeController(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL,
            ICategory1DAL category1DAL, IAccountService accountService) 
            : base(unitOfWork,loginHistoryDetailDAL,companyDAL,adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService )
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
            var model = new ChangeNoticeListModel {ChangeNotices = unitOfWork.ChangeNoticeRepository.Get().ToList()};
            return View(model);
        }

        public ActionResult Create()
        {
            return View("Edit",BuildModel(EditMode.New));
        }

        [HttpPost]
        public ActionResult Create(Change_notice n)
        {
            n.createdById = accountService.GetCurrentUser().userid;
            HandleFile(n);
            if (n.Allocations != null)
            {
                foreach (var a in n.Allocations)
                {
                    a.Product = null;
                }
            }
            unitOfWork.ChangeNoticeRepository.Insert(n);
            unitOfWork.Save();
            return Json(new Change_notice { id = n.id });
        }

        private void HandleFile(Change_notice changeNotice)
        {
            var sessionFiles = WebUtilities.GetTempFiles("tempFile_changenotice_");
            if (sessionFiles != null)
            {
                foreach (KeyValuePair<string, byte[]> kv in sessionFiles)
                {
                    //Write file
                    string filePath = Utilities.WriteFile(kv.Key, Server.MapPath(Settings.Default.ChangeNoticeRootFolder), kv.Value);
                    changeNotice.filename = Path.GetFileName(filePath);
                    break;  //only one file
                }
                WebUtilities.ClearTempFiles("tempFile_changenotice_");
            }
        }

        public ActionResult Edit(int id)
        {
            return View("Edit", BuildModel(EditMode.Edit, id));
        }

        [HttpPost]
        public ActionResult Edit(Change_notice n)
        {
            n.modifiedById = accountService.GetCurrentUser().userid;
            HandleFile(n);
            if (n.Allocations != null)
            {
                foreach (var a in n.Allocations)
                {
                    a.Product = null;
                }
            }
            unitOfWork.ChangeNoticeRepository.Update(n);
            unitOfWork.Save();
            n = unitOfWork.ChangeNoticeRepository.GetByID(n.id);
            if (n.Allocations != null)
            {
                foreach (var a in n.Allocations)
                {
                    a.Notice = null;
                }    
            }
            
            return Json(n);
        }

        private ChangeNoticeEditModel BuildModel(EditMode mode, int? id = null)
        {
            return new ChangeNoticeEditModel
            {
                Categories = category1DAL.GetAll().Select(c=>new LookupItem{id = c.category1_id,value = c.cat1_name}).ToList(),
                Clients = companyDAL.GetClientsFromProducts().Select(c=>new LookupItem{id = c.user_id,value = c.customer_code}).ToList(),
                Factories = companyDAL.GetFactories().Select(c=>new LookupItem{id = c.user_id,value = c.factory_code}).ToList(),
                Notice = mode == EditMode.New ? new Change_notice() : unitOfWork.ChangeNoticeRepository.GetByID(id.Value),
                EditMode = mode
            };
        }

        public ActionResult Files(string name)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            WebUtilities.ClearTempFiles("tempFile_changenotice_");
            return Json(WebUtilities.SaveTempFile(fileName, Request, 50, "tempFile_changenotice_"), "text/html");
        }

        public ActionResult Products(int? factory_id, int? client_id, int? category_id)
        {
            var products = unitOfWork.CustProductRepository.Get(p =>
                                (p.cprod_user == client_id || client_id == null) &&
                                (p.MastProduct.factory_id == factory_id || factory_id == null), includeProperties: "MastProduct").ToList();
            if (category_id != null)
                products = products.Where(p => p.MastProduct.category1 == category_id).ToList();
            return Json(products.Select(p => new {p.cprod_id, p.cprod_code1, p.cprod_name}));
        }

        public FileContentResult GetTempFile(string file)
        {
            var fileContents = WebUtilities.GetTempFile(file,"tempFile_changenotice_");
            if (fileContents != null)
                return File(fileContents, WebUtilities.ExtensionToContentType(Path.GetExtension(file).Replace(".", "")));
            return null;
        }

    }
}
