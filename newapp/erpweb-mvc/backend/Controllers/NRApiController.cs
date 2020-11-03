
using erp.Model;

using backend.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using erp.DAL.EF.New;
using erp.Model.Dal.New;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class NRApiController : ApiController
    {
        private IUnitOfWork unitOfWork;
        private ICompanyDAL companyDal;
        private IInspectionsDAL inspectionsDal;
        private IInspectionsV2DAL inspectionsV2Dal;
        private IInspectionLinesTestedDal inspectionLinesTestedDal;
        private readonly IProductService productService;
        private readonly IAccountService accountService;

        public NRApiController(IUnitOfWork unitOfWork, ICompanyDAL companyDal, IInspectionsDAL inspectionsDal, IInspectionsV2DAL inspectionsV2Dal,
            IInspectionLinesTestedDal inspectionLinesTestedDal, IProductService productService, IAccountService accountService)
        {
            this.inspectionLinesTestedDal = inspectionLinesTestedDal;
            this.productService = productService;
            this.accountService = accountService;
            this.inspectionsV2Dal = inspectionsV2Dal;
            this.inspectionsDal = inspectionsDal;
            this.companyDal = companyDal;
            this.unitOfWork = unitOfWork;
        }


        [Route("api/nr/getNRCreateModel")]
        [HttpGet]
        public object GetNRCreateModel(int insp_id)
        {
            bool? isFromOldRecord = null;
            var insp = GetInspection(insp_id, ref isFromOldRecord);
            if (insp != null)
            {
                var noOfNrsOnDay =
                        unitOfWork.NrHeaderRepository.Get(
                            h => DbFunctions.TruncateTime(h.NR_datecreated) == DateTime.Today && h.factory_id == insp.factory_id).Count();
                var model = new
                {
                    IsFromOldRecord = isFromOldRecord,
                    Inspection = GetInspectionUIObject(insp),
                    NrHeader = new Nr_header
                    {
                        insp_id = isFromOldRecord == true ? (int?)insp.id : null,
                        insp_v2_id = isFromOldRecord == false ? (int?)insp.id : null,
                        nr_type_id = 1, //NS
                        NR_document_no =
                            $"NS-{insp.Factory.factory_code}-{DateTime.Today.ToString("ddMMyyyy")}-{noOfNrsOnDay + 1}",
                        Lines = new List<Nr_lines>(),
                        factory_id = insp.factory_id
                    },
                    title = App_GlobalResources.Resources.NsReportTitle

                };

                return model;
            }

            return null;
        }

        [Route("api/nr/getNREditModel")]
        [HttpGet]
        public object GetNrEditModel(int id)
        {
            var nrHeader = unitOfWork.NrHeaderRepository.Get(h => h.id == id, includeProperties: "Lines,Images").FirstOrDefault();
            bool? isFromOldRecord;
            if (nrHeader != null && (nrHeader.insp_id != null || nrHeader.insp_v2_id != null))
            {

                isFromOldRecord = nrHeader.insp_id != null;
                var insp = GetInspection((nrHeader.insp_v2_id ?? nrHeader.insp_id).Value, ref isFromOldRecord);
                ProcessNrLines(nrHeader, insp);
                var model = new
                {
                    IsFromOldRecord = isFromOldRecord,
                    Inspection = GetInspectionUIObject(insp),
                    NrHeader = GetNrUIObject(nrHeader)
                };

                return model;
            }
            return null;

        }

        private static void ProcessNrLines(Nr_header nrHeader, Inspection_v2 insp)
        {
            if (nrHeader.Lines != null)
            {
                foreach (var line in nrHeader.Lines)
                {
                    line.Header = null;
                    if (insp != null && insp.Lines != null)
                    {
                        line.InspectionV2Line =
                            insp.Lines.FirstOrDefault(
                                l => l.id == (line.inspection_lines_v2_id ?? line.inspection_lines_tested_id));
                    }
                    if (line.Images != null)
                    {
                        foreach (var im in line.Images)
                        {
                            im.Line = null;
                        }
                    }
                }
            }
        }

        [Route("api/nr/getNrEditReportModel")]
        [HttpGet]
        public object GetNrEditReportModel(int id)
        {
            var model = BuildNrEditReportModel(id, unitOfWork);
            if (model != null)
                return model;
            return null;
        }

        public object BuildNrEditReportModel(int id, IUnitOfWork unitOfWork)
        {
            var nrHeader =
                unitOfWork.NrHeaderRepository.Get(h => h.id == id, includeProperties: "Lines,Images").FirstOrDefault();
            bool? isFromOldRecord;
            if (nrHeader != null && (nrHeader.insp_id != null || nrHeader.insp_v2_id != null))
            {
                isFromOldRecord = nrHeader.insp_id != null;
                var insp = GetInspection((nrHeader.insp_v2_id ?? nrHeader.insp_id).Value, ref isFromOldRecord);

                var orderIds = insp.Lines.Select(l => l.OrderLine?.orderid).Distinct().ToList();
                var orders = unitOfWork.OrderRepository.Get(o => orderIds.Contains(o.orderid),
                    includeProperties: "PorderHeaders").ToList();

                foreach (var o in orders)
                {
                    if (o.PorderHeaders != null)
                    {
                        o.po_req_etd = o.PorderHeaders.Max(po => po.po_req_etd);
                        o.PorderHeaders = null;
                    }
                }

                ProcessNrLines(nrHeader, insp);
                var imageTypes = nrHeader.nr_type_id == Nr_header.TypeNS ? new int[] { Nr_image_type.Label, Nr_image_type.Loading } : new int[] { Nr_image_type.Product, Nr_image_type.Loading };
                return new
                {
                    IsFromOldRecord = isFromOldRecord,
                    Inspection = GetInspectionUIObject(insp),
                    NrHeader = GetNrUIObject(nrHeader),
                    Orders = orders,
                    ImageTypes = unitOfWork.NrImageTypeRepository.Get(t => imageTypes.Contains(t.id)).ToList(),    //only types label and loading for NS, product and loading for NT
                    ImagesRootUrl = Properties.Settings.Default.NrImagesRootUrl,
                    headerTypes = new { Ns = Nr_header.TypeNS, Nt = Nr_header.TypeNT },
                    title = nrHeader.nr_type_id == Nr_header.TypeNS ? App_GlobalResources.Resources.NsReportTitle : App_GlobalResources.Resources.NtReportTitle
                };

            }
            return null;
        }

        [Route("api/nr/UpdateNr")]
        [HttpPost]
        [AllowAnonymous]
        public Nr_header UpdateNr(Nr_header nr, bool isOldRecord, string apiKey = null)
        {
            if (accountService.GetCurrentUser() == null && apiKey != Properties.Settings.Default.ApiKey)
                throw new HttpException(401, "Unauthorized");

            if (nr.Lines != null)
            {
                foreach (var l in nr.Lines)
                {
                    if (isOldRecord)
                    {
                        if (l.inspection_lines_tested_id == null && l.inspection_lines_v2_id != null)
                        {
                            l.inspection_lines_tested_id = l.inspection_lines_v2_id;
                            l.inspection_lines_v2_id = null;
                        }
                    }
                    l.InspectionV2Line = null;
                }
            }

            if (nr != null && nr.change_notice_id != null)
            {
                var cn = unitOfWork.ChangeNoticeRepository.Get(c => c.id == nr.change_notice_id).FirstOrDefault();

                if (cn != null)
                {
                    nr.NR_comment1 = cn.description;
                }
            }


            if (nr != null && nr.id <= 0)
            {
                if (nr.NR_document_no == null)
                    nr.NR_document_no = GenerateDocumentNo(nr);
                nr.NR_datecreated = DateTime.Now;
                unitOfWork.NrHeaderRepository.Insert(nr);
            }
            else
                unitOfWork.NrHeaderRepository.Update(nr);

            unitOfWork.Save();

            if (nr != null && nr.Lines != null)
            {
                foreach (var l in nr.Lines)
                {
                    l.Header = null;
                }
            }

            return nr;
        }

        private string GenerateDocumentNo(Nr_header nr)
        {
            var types = unitOfWork.NrTypeRepository.Get().ToList();
            var factory = companyDal.GetById(nr.factory_id ?? 0);
            if (nr.nr_type_id != null && factory != null)
            {
                var pattern = $"{types.FirstOrDefault(t => t.id == nr.nr_type_id)?.name}-{factory.factory_code}-{DateTime.Today.ToString("ddMMyyyy")}";
                var existing = unitOfWork.NrHeaderRepository.Get(h => h.NR_document_no.StartsWith(pattern)).OrderByDescending(h => h.NR_document_no).ToList();
                var suffix = 1;
                if (existing.Count > 0)
                {
                    var strSuffix = existing[0].NR_document_no.Split('-').Last();
                    if (int.TryParse(strSuffix, out suffix))
                        suffix++;
                }
                return pattern + "-" + suffix.ToString();

            }
            return string.Empty;
        }

        [HttpPost]
        [Route("api/nr/UpdateNrReport")]
        public Nr_header UpdateNrReport(Nr_header nr, bool submit = false)
        {
            HandleNrReportUploadedFiles(nr);
            if (submit)
            {
                nr.status = 1;
                nr.submitted_by = accountService.GetCurrentUser().userid;
                nr.submitted_date = DateTime.Now;
            }
            unitOfWork.NrHeaderRepository.Update(nr);
            unitOfWork.Save();

            if (nr.Lines != null)
            {
                foreach (var l in nr.Lines)
                {
                    l.Header = null;
                }
            }
            return nr;
        }

        private void HandleNrReportUploadedFiles(Nr_header nr)
        {
            var rootRelativeFolder = GetImagesFolder(nr.NR_datecreated, Properties.Settings.Default.NrImagesRootUrl);

            //Handle images

            if (nr.Images != null)
            {
                foreach (var im in nr.Images)
                {
                    if (!string.IsNullOrEmpty(im.file_id))
                    {
                        var oFile = WebUtilities.GetTempFile(im.file_id);
                        /*if(oFile == null) {
                            oFile = WebUtilities.GetTempFile(im.insp_image, string.Format("tempMulti_{0}", im.comments));   //comments contains file id from uploader
                        }*/
                        if (oFile != null)
                        {
                            var filePath = company.Common.Utilities.WriteFile(im.image_name, GetFolderFullPath(rootRelativeFolder, Properties.Settings.Default.NrImagesRootUrl),
                                oFile);
                            im.image_name = WebUtilities.CombineUrls(rootRelativeFolder, Path.GetFileName(filePath));
                        }
                    }
                }
            }
        }

        [Route("api/nr/uploadimage")]
        [HttpPost]
        public object UploadImage()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };
        }

        [Route("api/nr/getTempUrl")]
        [HttpGet]
        public HttpResponseMessage getTempUrl(string file_id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(file_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        [Route("api/nr/getNtList")]
        [HttpGet]
        [AllowAnonymous]
        public object GetNtList(DateTime? from = null, DateTime? etdReadyDate = null, string apiKey = null)
        {
            if (etdReadyDate == null)
                etdReadyDate = DateTime.Today;
            if (from == null)
                from = new DateTime(2017, 7, 1);

            if (accountService.GetCurrentUser() == null && apiKey != Properties.Settings.Default.ApiKey)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var inspectionRows = unitOfWork.NrHeaderRepository.GetInspectionsForNtReport_NoNrHeaders(from.Value, etdReadyDate.Value).Where(r => r.orderAllocated || r.po_req_etd >= DateTime.Today).ToList();
            var nrHeaders = GetNrHeadersFromInspectionRows(inspectionRows);
            inspectionRows.AddRange(unitOfWork.NrHeaderRepository.GetInspectionsForNtReport_NrHeaders(from.Value, etdReadyDate.Value).Where(r => r.po_req_etd >= DateTime.Today).ToList());
            nrHeaders.AddRange(GetNrHeadersFromInspectionRows(inspectionRows));
            return new
            {
                nrHeaders,
                inspectionRows = inspectionRows.Where(r => r.insp_line_id != null && nrHeaders.Count(nr => nr.insp_v2_id == r.insp_id) > 0).GroupBy(r => r.insp_id).Select(g => new {
                    g.First().insp_custpo,
                    g.First().order_custpo,
                    g.First().po_req_etd,
                    g.First().startdate,
                    g.First().customer_code,
                    g.First().factory_code
                }).ToList()
            };
        }

        private List<Nr_header> GetNrHeadersFromInspectionRows(List<NrReportInspectionRow> inspectionRows)
        {
            var nrHeaders = new List<Nr_header>();
            foreach (var gp in inspectionRows.GroupBy(r => r.cprod_id))
            {
                foreach (var gc in gp.GroupBy(r => r.userid1))
                {
                    var row = gc.FirstOrDefault(ro => ro.orderAllocated == true);
                    if (row == null)
                        row = gc.OrderBy(r => r.po_req_etd).FirstOrDefault();
                    if (row.insp_line_id != null)
                    {
                        var nr = nrHeaders.FirstOrDefault(n => n.insp_v2_id == row.insp_id);
                        if (nr == null)
                        {
                            nr = new Nr_header { insp_v2_id = row.insp_id, Lines = new List<Nr_lines>(), factory_id = row.factory_id, nr_type_id = Nr_header.TypeNT, change_notice_id = row.change_notice_id };
                            nrHeaders.Add(nr);
                        }
                        nr.Lines.Add(new Nr_lines { inspection_lines_v2_id = row.insp_line_id });
                    }
                }
            }
            return nrHeaders;
        }

        [Route("api/nr/getNsList")]
        [HttpGet]
        [AllowAnonymous]
        public object GetNsList(DateTime? from = null, string apiKey = null)
        {

            if (from == null)
                from = DateTime.Today.AddDays(-1);

            if (accountService.GetCurrentUser() == null && apiKey != Properties.Settings.Default.ApiKey)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var inspections = inspectionsDal.GetFinalInspectionsForNS(from);
            var factories = companyDal.GetFactories();

            var nrHeaders = inspections.Select(i => new Nr_header
            {
                insp_id = i.insp_unique,
                Lines = i.LinesTested.Select(lt => new Nr_lines { inspection_lines_tested_id = lt.insp_line_unique }).ToList(),
                factory_id = factories.FirstOrDefault(f => f.factory_code == i.factory_code)?.user_id,
                nr_type_id = Nr_header.TypeNS
            });

            return new
            {
                nrHeaders
            };
        }

        public static string GetImagesFolder(DateTime? date, string rootFolder)
        {
            var inspRootrelativeFolder = date.ToString("yyyy-MM");
            var fullFolderPath = GetFolderFullPath(inspRootrelativeFolder, rootFolder);
            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);
            return inspRootrelativeFolder;
        }

        public static string GetFolderFullPath(string rootRelativeUrl, string imagesRootFolder)
        {
            return System.Web.HttpContext.Current.Server.MapPath(Path.Combine(imagesRootFolder, rootRelativeUrl));
        }

        public Inspection_v2 GetInspection(int insp_id, ref bool? isFromOldRecord)
        {

            Inspection_v2 insp = null;
            if (isFromOldRecord != true)
            {
                insp = inspectionsV2Dal.GetById(insp_id, true);
                isFromOldRecord = insp == null;
            }

            if (isFromOldRecord == true)
            {
                var oldInsp = inspectionsDal.GetById(insp_id);
                oldInsp.LinesTested = inspectionLinesTestedDal.GetByInspection(insp_id, true, true);
                foreach (var l in oldInsp.LinesTested)
                {
                    l.Inspection = oldInsp;
                }
                insp = GetFromOldRecord(oldInsp);
            }

            var orderIds = insp?.Lines?.Where(l => l.OrderLine != null).Select(l => l.OrderLine?.orderid).Distinct().ToList();
            if (orderIds != null)
            {
                //Load combined orders
                var combinedIds = new List<int>();
                foreach (var id in orderIds)
                {
                    var order =
                        unitOfWork.OrderRepository.Get(o => o.orderid == id, includeProperties: "CombinedOrders")
                            .FirstOrDefault();
                    if (order != null && order.CombinedOrders != null)
                        combinedIds.AddRange(order.CombinedOrders.Select(co => co.orderid));
                }
                combinedIds = combinedIds.Distinct().ToList();
                if (insp.Lines != null && combinedIds.Count > 0)
                {
                    if (isFromOldRecord == false)
                        insp.Lines.AddRange(inspectionsV2Dal.LoadLines(orderids: combinedIds, id: insp.id, loadLoadings: true, insp: insp));
                    else
                        insp.Lines.AddRange(inspectionLinesTestedDal.GetLinesForOrders(combinedIds, insp_id).Select(GetLineFromOldRecord));
                }

                if (insp.Lines != null)
                {
                    var factories = companyDal.GetFactories();
                    foreach (var l in insp.Lines)
                    {
                        if (l.OrderLine != null && l.OrderLine.Cust_Product != null &&
                            l.OrderLine.Cust_Product.MastProduct != null)
                        {
                            l.OrderLine.Cust_Product.MastProduct.Factory =
                                factories.FirstOrDefault(f => f.user_id == l.OrderLine.Cust_Product.MastProduct.factory_id);
                        }
                    }
                }

            }
            return insp;
        }

        private Inspection_v2 GetFromOldRecord(Inspections i, bool processLines = true)
        {
            if (i != null)
            {
                var client = companyDal.GetByCustomerCode(i.customer_code);
                var factory = companyDal.GetByFactoryCode(i.factory_code);
                return new Inspection_v2
                {
                    id = i.insp_unique,
                    startdate = i.insp_start,
                    type = i.insp_type == "LO" ? 1 : i.insp_type == "FI" ? (int?)2 : null,
                    duration = i.insp_days,
                    custpo = i.custpo,
                    client_id = client?.user_id,
                    Client = client,
                    Factory = factory,
                    factory_id = factory?.user_id,
                    comments = i.insp_comments,
                    qc_required = i.qc_required,
                    InspectionType = new Inspection_v2_type { name = i.insp_type },
                    Lines = i.LinesTested != null && processLines ? i.LinesTested.Select(GetLineFromOldRecord).ToList() : null,
                    Controllers = i.Controllers?.Select(c => new Inspection_v2_controller { Controller = c.Controller, controller_id = c.controller_id }).ToList()

                };
            }
            return null;

        }

        private Inspection_v2_line GetLineFromOldRecord(Inspection_lines_tested lt)
        {
            return new Inspection_v2_line
            {
                id = lt.insp_line_unique,
                insp_id = lt.insp_id,
                orderlines_id = lt.order_linenum,
                OrderLine = lt.OrderLine,
                qty = Convert.ToInt32(lt.insp_qty),
                Inspection = GetFromOldRecord(lt.Inspection, false),
                Loadings = lt.Loadings?.Select(GetLoadingFromOldRecord).ToList()
            };
        }

        private static Inspection_v2_loading GetLoadingFromOldRecord(Inspections_loading oldLoading)
        {
            return new Inspection_v2_loading
            {
                id = oldLoading.loading_line_unique,
                container_id = oldLoading.container,
                Container = GetContainerFromOldRecord(oldLoading.Container)
            };
        }

        private static Inspection_v2_container GetContainerFromOldRecord(Containers container)
        {
            if (container != null)
                return new Inspection_v2_container
                {
                    id = container.container_id,
                    container_count = container.container_count,
                    container_comments = container.container_comments,
                    container_no = container.container_no,
                    container_space = container.container_space,
                    seal_no = container.seal_no
                };
            return null;
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
                factory = i.Factory,
                client = i.Client,
                controllers = i.Controllers?.Select(c => new
                {
                    c.id,
                    controller = new
                    {
                        c.Controller?.userid,
                        c.Controller?.userwelcome,
                        c.Controller?.user_initials
                    },
                    c.controller_id,
                    c.inspection_id
                }),
                lines = i.Lines?.Select(l => GetInspectionV2LineUIObject(l))
            };
        }

        private object GetInspectionV2LineUIObject(Inspection_v2_line l)
        {
            return new
            {
                l.id,
                l.insp_id,
                l.cprod_id,
                l.insp_custproduct_code,
                l.insp_mastproduct_code,
                l.insp_custproduct_name,
                l.qty,
                l.comments,
                inspection = new { l.Inspection?.type },
                orderLine = l.OrderLine != null ? new
                {
                    l.OrderLine?.orderqty,
                    l.OrderLine?.cprod_id,
                    custProduct = new
                    {
                        l.OrderLine?.Cust_Product?.cprod_code1,
                        l.OrderLine?.Cust_Product?.cprod_name,
                        mastProduct = new
                        {
                            l.OrderLine?.Cust_Product?.MastProduct?.factory_ref,
                            factory = new
                            {
                                l.OrderLine?.Cust_Product?.MastProduct?.Factory?.factory_code
                            },
                            l.OrderLine?.Cust_Product?.MastProduct?.category1
                        }
                    }
                } : null,
                images = l.Images?.Select(im => new
                {
                    im.id,
                    im.insp_image,
                    im.insp_line,
                    im.type_id,
                    url = WebUtilities.CombineUrls(Properties.Settings.Default.InspectionImagesFolder, im.insp_image)
                }),
                loadings = l.Loadings?.Select(lo => new
                {
                    lo.area_id,
                    lo.container_id,
                    lo.full_pallets,
                    lo.loose_load_qty,
                    lo.mixed_pallet_qty,
                    lo.mixed_pallet_qty2,
                    lo.mixed_pallet_qty3,
                    lo.qty_per_pallet,
                    lo.QtyMixedPallets,
                    Container = new
                    {
                        lo.Container?.container_no,
                        lo.Container?.container_size
                    }
                }
                ),
                combined_code = productService.GetCombinedCode(l.Product?.cprod_code1, l.Product?.MastProduct?.factory_ref)
            };
        }

        private object GetNrUIObject(Nr_header nr)
        {
            return new
            {
                nr.factory_id,
                nr.id,
                nr.insp_id,
                nr.no_of_cartons,
                insp_v2_id = nr.insp_v2_id,
                nr.NR_comment1,
                nr.NR_comment2,
                nr.NR_comment3,
                nr.NR_datecreated,
                nr.NR_document_no,
                nr.nr_type_id,
                nr.status,
                nr.submitted_by,
                nr.submitted_date,
                nr.change_notice_id,
                Lines = nr.Lines.Select(l => new
                {
                    l.id,
                    l.inspection_lines_tested_id,
                    l.NR_id,
                    l.NR_line_type,
                    l.NR_line_comments,
                    l.inspection_lines_v2_id/*,
                    Images = l.Images.Select(im => new
                    {
                        im.id,
                        im.image_name,
                        im.image_type,
                        im.NR_line_id,
                        im.sequence
                    })*/,
                    InspectionV2Line = GetInspectionV2LineUIObject(l.InspectionV2Line)
                }),
                Images = nr.Images.Select(
                    im => new
                    {
                        im.id,
                        im.image_name,
                        im.image_type,
                        im.NR_line_id,
                        im.nr_id,
                        im.sequence,
                        im.carton_no
                    }
                    )

            };
        }
    }

}