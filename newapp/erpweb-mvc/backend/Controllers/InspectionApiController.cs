using erp.DAL.EF.New;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Utilities = company.Common.Utilities;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Net.Http.Headers;
using System.Linq.Expressions;
using backend.Models;
using System.Net.Mail;
using System.Text;
using backend.Properties;
using company.Common;
//using System.Web.Mvc;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class InspectionApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAccountService accountService;

        public InspectionApiController(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            this.unitOfWork = unitOfWork;
            this.accountService = accountService;
        }

        [Route("api/inspection/getFactories")]
        [HttpGet]
        public object GetFactories()
        {
            return unitOfWork.CompanyRepository.GetFactories().Select(f => new { f.user_id, f.factory_code }).OrderBy(f => f.factory_code).ToList();
        }

        [Route("api/inspection/getClients")]
        [HttpGet]
        public object GetClients()
        {
            return unitOfWork.CompanyRepository.GetClientsWithOrders().Select(f => new { f.user_id, f.customer_code }).OrderBy(c => c.customer_code).ToList();
        }

        [Route("api/inspection/getSubjects")]
        [HttpGet]
        public object GetSubjects()
        {
            return unitOfWork.InspectionSubjectRepository.Get().ToList();
        }

        [Route("api/inspection/getSIModel")]
        [HttpGet]
        public object GetSIModel(string state, int? id = null)
        {
            var imageTypes = new List<int> {
                    (int) Inspection_v2_image_type.Appearance, (int) Inspection_v2_image_type.Dimension,
                    (int) Inspection_v2_image_type.Function, (int) Inspection_v2_image_type.Material ,
                    (int) Inspection_v2_image_type.Packaging, (int) Inspection_v2_image_type.Summary };
            return new
            {
                factories = unitOfWork.CompanyRepository.GetFactories().Select(f => new { f.user_id, f.factory_code }).OrderBy(f => f.factory_code).ToList(),
                clients = unitOfWork.CompanyRepository.GetClientsWithOrders().Select(f => new { f.user_id, f.customer_code }).OrderBy(c => c.customer_code).ToList(),
                subjects = unitOfWork.InspectionSubjectRepository.Get().ToList(),
                imageTypes = unitOfWork.InspectionImageTypeRepository.Get(t => imageTypes.Contains(t.id)),
                role = getRole(),
                isApprover = accountService.GetCurrentUser()?.IsInRole(Role.FC_SI_Approver),
                inspectionStatus = new
                {
                    New = (int)InspectionV2Status.New,
                    ListReady = (int)InspectionV2Status.ListReady,
                    ReportSubmitted = (int)InspectionV2Status.ReportSubmitted,
                    Rejected = (int)InspectionV2Status.Rejected,
                    Approved = (int)InspectionV2Status.Accepted,
                    AwaitingApproval = (int)InspectionV2Status.AwaitingApproval
                },
                insp = state == "create" ? new Inspection_v2
                {
                    type = Inspection_v2_type.Sample,
                    Controllers = new List<Inspection_v2_controller>(),
                    Lines = new List<Inspection_v2_line>()
                } :
                       state == "edit" ? Get(id.Value) : null,
                imagesRoot = Properties.Settings.Default.InspectionImagesFolder
            };

        }

        [Route("api/inspection/get")]
        [HttpGet]
        public object Get(int id)
        {
            var isQc = User.IsInRole(UserRole.Inspector.ToString());
            return GetInspectionUIObject(unitOfWork.InspectionV2Repository.GetSiById(id, isQc));
        }

        [Route("api/inspection/getbycriteria")]
        [HttpGet]
        public object GetByCriteria(string types, DateTime? dateFrom = null, DateTime? dateTo = null, int? factory_id = null, int? client_id = null)
        {
            var typeIds = Utilities.GetIdsFromString(types);
            var user = accountService.GetCurrentUser();
            var userid = user.userid;
            var isApprover = user.IsInRole(Role.FC_SI_Approver);
            var isqc = User.IsInRole(UserRole.Inspector.ToString());

            var test = unitOfWork.InspectionV2Repository.Get(i => typeIds.Contains(i.type.Value)
                       && ((isApprover && i.insp_status != (int)InspectionV2Status.New)
                          || (!isApprover && !isqc)
                          || (isqc && (i.insp_status == InspectionV2Status.Accepted || i.insp_status == InspectionV2Status.ReportSubmitted)
                               && i.Controllers.Any(c => c.controller_id == userid)))
                       && (i.factory_id == factory_id || factory_id == null)
                       && (i.client_id == client_id || client_id == null)
                       && (i.dateCreated >= dateFrom || dateFrom == null)
                       && (i.dateCreated <= dateTo || dateTo == null), includeProperties: "Factory,Client,Subject,Controllers, InspectionType").
                Select(i => GetInspectionUIObject(i))
                .ToList();

            return test;
        }

        private object GetInspectionUIObject(Inspection_v2 i)
        {
            return new
            {
                i.id,
                i.factory_id,
                i.client_id,
                i.type,
                i.CreatedBy,
                i.dateCreated,
                i.si_subject_id,
                i.startdate,
                i.insp_status,
                i.ComputedCode,
                factory = i.Factory?.factory_code,
                client = i.Client?.customer_code,
                subject = i.Subject?.name,
                controllers = i.Controllers?.Select(c => new
                {
                    c.id,
                    controller = new
                    {
                        c.Controller?.userid,
                        c.Controller?.userwelcome
                    },
                    c.controller_id,
                    c.inspection_id
                }),
                lines = i.Lines?.Select(l => new
                {
                    l.id,
                    l.insp_id,
                    l.cprod_id,
                    l.insp_custproduct_code,
                    l.insp_mastproduct_code,
                    l.insp_custproduct_name,
                    l.qty,
                    l.si_requirement,
                    l.comments,
                    images = l.Images?.Select(im => new { im.id, im.insp_image, im.insp_line, im.type_id, url = WebUtilities.CombineUrls(Properties.Settings.Default.InspectionImagesFolder, im.insp_image) }),
                    sidetails = l.SiDetails?.Select(d => new { d.id, d.insp_line, d.type_id, d.requirement, d.comments }),
                    icons = GenerateIconLinks(l.Product),
                    combined_code = GetCombinedCode(l.Product?.cprod_code1, l.Product?.MastProduct?.factory_ref)
                })
            };
        }

        private string GetCombinedCode(string cprod_code1, string factory_ref)
        {
            if (string.IsNullOrEmpty(factory_ref))
                return cprod_code1;
            return $"{cprod_code1} ({factory_ref})";
        }

        private List<object> GenerateIconLinks(Cust_products p)
        {
            var result = new List<object>();
            if (p != null)
            {
                if (!string.IsNullOrEmpty(p.MastProduct?.prod_image3))
                    result.Add(new { type = "drawing", url = WebUtilities.CombineUrls(Properties.Settings.Default.aspsite_root, $"factory_PR_4_tech_pdf.asp?prod_id={p.cprod_mast}&cprod_code={p.MastProduct.factory_ref}") });
                else
                    result.Add(new { type = "drawing", url = GetImage(p.cprod_dwg, null) });
                if (!string.IsNullOrEmpty(p.MastProduct?.prod_image3))
                    result.Add(new { type = "detdrawing", url = WebUtilities.CombineUrls(Properties.Settings.Default.aspsite_root, $"factory_PR_4_tech2_pdf.asp?prod_id={p.cprod_mast}&cprod_code={p.MastProduct.factory_ref}") });
                else
                    result.Add(new { type = "detdrawing", url = GetImage(p.cprod_dwg, null) });
                result.Add(new { type = "instr", url = GetImage(p.MastProduct?.prod_instructions, p.cprod_instructions) });
                result.Add(new { type = "label", url = GetImage(p.MastProduct?.prod_image4, p.cprod_label) });
                result.Add(new { type = "pack", url = GetImage(p.MastProduct?.prod_image5, p.cprod_packaging) });
            }


            return result;
        }

        private static string GetImage(string cprod_image, string mastprod_image)
        {
            //throw new NotImplementedException();
            string imageName = !string.IsNullOrEmpty(cprod_image) ? cprod_image : !string.IsNullOrEmpty(mastprod_image) ? mastprod_image : string.Empty;
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = System.IO.File.Exists(HttpContext.Current.Server.MapPath(imageName)) ? imageName : String.Empty;
                return imageName;
            }
            return string.Empty;
        }

        [Route("api/inspection/getControllers")]
        [HttpGet]
        public object GetControllers(string prefixText = null)
        {
            var admin_type = erp.Model.User.adminType_Qc;
            return unitOfWork.UserRepository.Get(u => u.admin_type == admin_type && (prefixText == null || u.username.StartsWith(prefixText) || u.userwelcome.StartsWith(prefixText))).Select(u => new { u.userid, u.userwelcome }).ToList();
        }

        [Route("api/inspection/create")]
        [HttpPost]
        public object Create(Inspection_v2 insp)
        {
            insp.CreatedBy = accountService.GetCurrentUser().userid;
            unitOfWork.InspectionV2Repository.Insert(insp);
            unitOfWork.Save();
            return Get(insp.id);
        }

        [Route("api/inspection/update")]
        [HttpPost]
        public object Update(Inspection_v2 insp)
        {
            //if qc, handle files
            if (User.IsInRole(UserRole.Inspector.ToString()))
                CollectFiles(insp);

            unitOfWork.InspectionV2Repository.Update(insp);
            unitOfWork.Save();
            return Get(insp.id);
        }

        [Route("api/inspection/delete")]
        [HttpDelete]
        public void Delete(int id)
        {
            unitOfWork.InspectionV2Repository.Delete(id);
            unitOfWork.Save();
        }
        

        private void CollectFiles(Inspection_v2 insp)
        {
            var inspectionRootRelativeFolder = InspectionV2Controller.GetInspectionImagesFolder(insp, Properties.Settings.Default.InspectionImagesFolder);
            foreach (var i in insp.AllImages.Where(i => !string.IsNullOrEmpty(i.file_id)))
            {
                var filePath = Utilities.WriteFile(i.insp_image, InspectionV2Controller.GetInspectionFolderFullPath(insp), WebUtilities.GetTempFile(i.file_id));
                i.insp_image = WebUtilities.CombineUrls(inspectionRootRelativeFolder, Path.GetFileName(filePath));
            }

        }

        [Route("api/inspection/getProducts")]
        [HttpGet]
        public object GetProducts(string prefixText)
        {
            var products = unitOfWork.CustProductRepository.Get(p => p.cprod_status != "D" && (p.cprod_name.StartsWith(prefixText) || p.cprod_code1.StartsWith(prefixText)), includeProperties: "MastProduct.Factory").ToList();
            return Json(products.Select(p => new
            {
                p.cprod_id,
                p.cprod_code1,
                p.cprod_name,
                p.cprod_mast,
                p.MastProduct.factory_ref,
                p.MastProduct.factory_name,
                p.MastProduct.Factory.factory_code,
                icons = GenerateIconLinks(p),
                combined_code = GetCombinedCode(p.cprod_code1, p.MastProduct.factory_ref)
            }));
        }

        [Route("api/inspection/uploadproductlist")]
        [HttpPost]
        public object UploadProductList()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];

            return new { success = true };// lines = GetLinesFromFile(file) };
        }

        [Route("api/inspection/getRole")]
        [HttpGet]
        public string getRole()
        {
            if (User.IsInRole(UserRole.FactoryController.ToString()) || accountService.GetCurrentUser().IsInRole(Role.ITUser))
            {
                return "fc";
            }

            else if (User.IsInRole(UserRole.Inspector.ToString()))
                return "qc";
            return string.Empty;
        }

        [Route("api/inspection/uploadimage")]
        [HttpPost]
        public object UploadImage()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };
        }

        [Route("api/inspection/getTempUrl")]
        [HttpGet]
        public HttpResponseMessage getTempUrl(string file_id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(file_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        [Route("api/inspection/changeStatus")]
        [HttpPut]
        public object ChangeStatus(int id)
        {
            var insp = unitOfWork.InspectionV2Repository.GetByID(id);
            if (insp != null)
            {
                if (insp.insp_status == InspectionV2Status.New)
                    insp.insp_status = InspectionV2Status.AwaitingApproval;
                else if (insp.insp_status == InspectionV2Status.AwaitingApproval)
                    insp.insp_status = InspectionV2Status.Accepted;
                else if (insp.insp_status == InspectionV2Status.Accepted)
                    insp.insp_status = InspectionV2Status.ReportSubmitted;
                unitOfWork.Save();
                return (int)insp.insp_status;
            }
            return null;
        }

        [Route("api/inspection/getForKpi")]
        [HttpGet]
        public object getForKpi(int qc_id, DateTime? monthStart)
        {
            return unitOfWork.InspectionV2Repository.GetInspectionsForKpi(qc_id, monthStart != null ? (int?)Month21.FromDate(monthStart.Value).Value : null);
                
        }

        [Route("api/inspection/getKpiModel")]
        [HttpGet]
        public object getKpiModel()
        {
            var admin_type = erp.Model.User.adminType_Qc;
            return new
            {
                controllers = unitOfWork.UserRepository.Get(u => u.admin_type == admin_type && u.status_flag != 1).Select(u => new { u.userid, u.userwelcome }).ToList(),
                asproot = Settings.Default.aspsite_root
            };

        }

        private object GetLinesFromFile(HttpPostedFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            IExcelDataReader excelReader = extension == ".xls" ? ExcelReaderFactory.CreateBinaryReader(file.InputStream) : ExcelReaderFactory.CreateOpenXmlReader(file.InputStream);

            
            DataSet ds = excelReader.AsDataSet(new ExcelDataSetConfiguration() {
				ConfigureDataTable = (_) => new ExcelDataTableConfiguration() {
					UseHeaderRow = true
				}
			});

            var factory_refs = new List<string>();
            var cprod_codes = new List<string>();
            var dictQtys = new Dictionary<string, int?>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var factory_ref = string.Empty + row["asaq_ref"];
                var cprod_code = string.Empty + row["cprod_code1"];

                if (string.IsNullOrEmpty(cprod_code))
                {
                    factory_refs.Add(factory_ref);
                    dictQtys[factory_ref] = erp.Model.Utilities.FromDbValue<int>(row["qty"]);
                }
                else
                {
                    cprod_codes.Add(cprod_code);
                    dictQtys[cprod_code] = erp.Model.Utilities.FromDbValue<int>(row["qty"]);
                }

            }
            var mastProducts = unitOfWork.MastProductRepository.Get(p => factory_refs.Contains(p.factory_ref), includeProperties: "CustProducts").ToList();
            var custProducts = unitOfWork.CustProductRepository.Get(p => cprod_codes.Contains(p.cprod_code1), includeProperties: "MastProduct").ToList();
            return custProducts.Where(p => p.cprod_status != "D").Union(mastProducts.SelectMany(p => p.CustProducts.Where(cp => cp.cprod_status != "D"))).
                Select(p => new
                {
                    p.cprod_id,
                    insp_custproduct_code = p.cprod_code1,
                    insp_custproduct_name = p.cprod_name,
                    insp_mastproduct_code = p.MastProduct.factory_ref,
                    icons = GenerateIconLinks(p),
                    qty = dictQtys.ContainsKey(p.cprod_code1) ? dictQtys[p.cprod_code1] : dictQtys.ContainsKey(p.MastProduct.factory_ref) ? dictQtys[p.MastProduct.factory_ref] : 1
                }).ToList();
        }

        //Tvrtko 3.7.2017 - ca routes moved to CaApiController        

    }
}
