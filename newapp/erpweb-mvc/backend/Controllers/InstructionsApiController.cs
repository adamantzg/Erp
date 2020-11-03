using erp.DAL.EF.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.Model;
using erp.Model.Dal.New;
using System.Web;
using System.IO;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class InstructionsApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICompanyDAL companyDAL;
        private readonly ILanguageDal languageDal;
        private readonly IAccountService accountService;

        public InstructionsApiController(IUnitOfWork unitOfWork, ICompanyDAL companyDAL, ILanguageDal languageDal,
            IAccountService accountService)
        {
            this.unitOfWork = unitOfWork;
            this.companyDAL = companyDAL;
            this.languageDal = languageDal;
            this.accountService = accountService;
        }

        [Route("api/instructions/getAll")]
        [HttpGet]
        public object GetAll()
        {
            return new
            {
                instructions = unitOfWork.InstructionsRepository.Get(includeProperties: "CreatedBy,Language,Products").Select(GetUIObject).ToList(),
                fileRootFolder = Properties.Settings.Default.InstructionExternaFolder
            };            
        }

        [Route("api/instructions/getModel")]
        [HttpGet]
        public object getModel(int? id = null)
        {
            WebUtilities.ClearTempFiles();
            return new
            {
                clients = companyDAL.GetClientsFromProducts().ToList(),
                factories = companyDAL.GetFactories().ToList(),
                fileRootFolder = Properties.Settings.Default.InstructionExternaFolder,
                languages = languageDal.GetAll(),
                instruction = id == null ? new instructions_new() : unitOfWork.InstructionsRepository.Get(i => i.id == id, includeProperties: "Products.CustProducts,Products.Factory").Select(GetUIObject).FirstOrDefault()
            };
        }

        [Route("api/instructions/searchProducts")]
        [HttpGet]
        public object Products(int? factory_id = null, int? client_id = null)
        {
            return unitOfWork.CustProductRepository.Get(p => (p.brand_userid == client_id || client_id == null)
            && (p.MastProduct.factory_id == factory_id || factory_id == null)
            && p.cprod_status == "N" && p.cprod_code1 != null, includeProperties: "BrandCompany,Mastproduct.Factory").
            Select(p => new
            {
                p.cprod_id,
                p.cprod_code1,
                p.cprod_name,
                p.cprod_mast,
                mastProduct = new {p.MastProduct?.mast_id, p.MastProduct?.factory_ref,p.MastProduct?.asaq_name, factory = new { p.MastProduct?.Factory?.factory_code} },
                BrandCompany = new { p.BrandCompany?.customer_code }
            });
        }

        [Route("api/instructions/create")]
        [HttpPost]
        public object Create(instructions_new i)
        {
            HandleFile(i);
            i.dateCreated = DateTime.Now;
            i.created_by = accountService.GetCurrentUser()?.userid;
            unitOfWork.InstructionsRepository.Insert(i);
            unitOfWork.Save();
            return GetUIObject(i);
        }

        [Route("api/instructions/update")]
        [HttpPost]
        public object Update(instructions_new i)
        {
            HandleFile(i);
            unitOfWork.InstructionsRepository.Update(i);
            unitOfWork.Save();
            return GetUIObject(i);
        }

        [Route("api/instructions/upload")]
        [HttpPost]
        public object UploadFile()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };
        }


        private void HandleFile(instructions_new i)
        {
            var temp_file = WebUtilities.GetTempFile(i.file_id);

            if (temp_file != null)
            {
                var filePath = company.Common.Utilities.WriteFile(i.filename, System.Web.Hosting.HostingEnvironment.MapPath(Properties.Settings.Default.InstructionExternaFolder), temp_file);
                if (!string.IsNullOrEmpty(filePath))
                    i.filename = Path.GetFileName(filePath);
            }
        }

        private object GetUIObject(instructions_new i)
        {
            var comparer = new CustProductCprodCodeDistinctComparer();
            return new
            {
                i.id,
                i.filename,
                i.created_by,
                //i.dateCreated,
                i.language_id,
                createdBy = i.CreatedBy != null ? new { i.CreatedBy.userwelcome } : null,
                language = i.Language != null ? new { i.Language.code, i.Language.name } : null,
                products = i.Products?.Select(p => new {
                    p.mast_id,
                    p.factory_ref,
                    p.asaq_name,
                    factory = p.Factory != null ? new {p.Factory.factory_code} : null,
                    custProducts = p.CustProducts?.Where(cp=>cp.cprod_status != "D").Distinct(comparer).Select(cp=> new {
                        cp.cprod_id,
                        cp.cprod_name,
                        cp.cprod_code1
                    })
                })
            };
        }
    }
}