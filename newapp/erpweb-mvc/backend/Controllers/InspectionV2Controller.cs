using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using company.Common;
using erp.Model;

using backend.Models;
using ASPPDFLib;
using Settings = backend.Properties.Settings;
using Utilities = company.Common.Utilities;
using System.Data;
using ExcelDataReader;
using System.Text;
using erp.DAL.EF.New;
using erp.Model.Dal.New;

using backend.Properties;
using System.Net.Mail;
using backend.ApiServices;

//using MVCControlsToolkit.Core;

namespace backend.Controllers
{
    [Authorize(Roles = "Administrator, Inspector")]
    public class InspectionV2Controller : BaseController
    {
        
        
        private ICompanyDAL companyDal;
        private IInspectionsDAL inspectionsDal;
        private IInspectionsV2DAL inspectionsV2Dal;
        private IOrderHeaderDAL orderHeaderDal;
        private ICustproductsDAL custproductsDal;
        private IReturnsDAL returnsDal;
        private IOrderLinesDAL orderLinesDal;
        private IAmendmentsDAL amendmentsDal;
        private IInspectionLinesTestedDal inspectionLinesTestedDal;
        private ISettings settings;
        private readonly IAdminPermissionsDal adminPermissionsDal;
        private readonly IProductService productService;
        private readonly IContainerTypesDal containerTypesDal;
        private readonly IContainerDal containerDal;
        private readonly IInspectionsLoadingDal inspectionsLoadingDal;
        private readonly IAccountService accountService;
        private readonly IMailHelper mailHelper;
        public const string LoadingReportPdfOptions = "PageWidth=596,PageHeight=842,scale=0.68,leftmargin=22,rightmargin=22,bottommargin=15,media=1,timeout=300";

        public InspectionV2Controller(IUnitOfWork unitOfWork, ICompanyDAL companyDal, IInspectionsDAL inspectionsDal,
            IInspectionsV2DAL inspectionsV2Dal, IOrderHeaderDAL orderHeaderDal, ICustproductsDAL custproductsDal,
            IReturnsDAL returnsDal, IOrderLinesDAL orderLinesDal, IAmendmentsDAL amendmentsDal,
            IInspectionLinesTestedDal inspectionLinesTestedDal, ISettings settings, IAdminPermissionsDal adminPermissionsDal, 
            IProductService productService, IContainerTypesDal containerTypesDal, IContainerDal containerDal,
            IInspectionsLoadingDal inspectionsLoadingDal, IAccountService accountService, IMailHelper mailHelper, ILoginHistoryDetailDAL loginHistoryDetailDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL) 
            : base(unitOfWork, loginHistoryDetailDAL,companyDal, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL,accountService)
                
        {
            this.settings = settings;
            this.adminPermissionsDal = adminPermissionsDal;
            this.productService = productService;
            this.containerTypesDal = containerTypesDal;
            this.containerDal = containerDal;
            this.inspectionsLoadingDal = inspectionsLoadingDal;
            this.accountService = accountService;
            this.mailHelper = mailHelper;
            this.inspectionLinesTestedDal = inspectionLinesTestedDal;
            this.amendmentsDal = amendmentsDal;
            this.orderLinesDal = orderLinesDal;
            this.returnsDal = returnsDal;
            this.custproductsDal = custproductsDal;
            this.orderHeaderDal = orderHeaderDal;
            this.inspectionsV2Dal = inspectionsV2Dal;
            this.inspectionsDal = inspectionsDal;
            this.companyDal = companyDal;
            
        }

        public ActionResult Index()
        {
            var model = new InspectionListModel
            {
                Clients = companyDal.GetCompaniesForUser(accountService.GetCurrentUser().IfNotNull(u => u.userid), Company_User_Type.Client),
                Factories = companyDal.GetCompaniesForUser(accountService.GetCurrentUser().IfNotNull(u => u.userid)),
                StatusFilters = GetFiltersForUser(User)
            };
            return View(model);
        }

        public ActionResult UploadDrawing()
        {
            return View();
        }

        //
        // GET: /InspectionV2/

        public ActionResult Templates()
        {
            return View(unitOfWork.InspectionV2TemplateRepository.Get().ToList());
        }

        public ActionResult TemplateEdit(int id)
        {
            return View(BuildModel(id));
        }

        private InspectionTemplateEditModel BuildModel(int id)
        {
            return new InspectionTemplateEditModel
            {
                Clients = companyDal.GetClients().Select(c => new LookupItem { id = c.user_id, value = c.customer_code }).ToList(),
                Factories = companyDal.GetFactories().Select(c => new LookupItem { id = c.user_id, value = c.factory_code }).ToList(),
                Template = id > 0 ? unitOfWork.InspectionV2TemplateRepository.GetByID(id) : new Inspv2_template { Criteria = new List<Inspv2_criteria>(), Products = new List<Cust_products>() },
                Points = unitOfWork.InspectionV2PointRepository.Get().ToList(),
                Categories = unitOfWork.InspectionV2CriteriaCategoryRepository.Get().ToList()
            };
        }

        public ActionResult ProductCriteria()
        {
            return View(new InspectionProductCriteriaModel
            {
                Clients = companyDal.GetClients().Select(c => new LookupItem { id = c.user_id, value = c.customer_code }).ToList(),
                Factories = companyDal.GetFactories().Select(c => new LookupItem { id = c.user_id, value = c.factory_code }).ToList(),
                Templates = unitOfWork.InspectionV2TemplateRepository.Get().ToList(),
                Points = unitOfWork.InspectionV2PointRepository.Get().ToList(),
                Categories = unitOfWork.InspectionV2CriteriaCategoryRepository.Get().ToList()
            });
        }

        public ActionResult SearchProducts(int? factory_id, int? client_id, bool includeTemplates = false)
        {
            return
                Json(
                    unitOfWork.CustProductRepository.SearchProducts(factory_id, client_id, includeTemplates)
                    .Select(p => new
                    {
                        p.cprod_id,
                        p.cprod_code1,
                        p.cprod_name,
                        Inspv2Templates = p.Inspv2Templates != null ? p.Inspv2Templates.Select(t => new { t.id, t.name }) : null
                    }).OrderBy(p => p.cprod_code1));
        }



        public ActionResult Save(Inspv2_template template, int[] deletedCriteria, int[] deletedProducts)
        {
            if (template.id > 0)
                unitOfWork.InspectionV2TemplateRepository.Update(template);
            else
            {
                unitOfWork.InspectionV2TemplateRepository.Insert(template);
            }
            unitOfWork.Save();
            return Json("OK");
        }

        public ActionResult GetProductSelectionCriteria(IList<int> ids, int template_id)
        {
            return
                Json(
                    new
                    {
                        TemplateCriteria = unitOfWork.InspectionV2TemplateRepository.Get(t => t.id == template_id, includeProperties: "Criteria.Point")
                            .FirstOrDefault()?.Criteria?.
                            Select(c =>
                        new { c.id, c.importance, c.category_id, c.point_id, Point = new { c.point_id, c.Point.name }, c.requirements, c.requirements_cn, c.template_id, c.number, c.IsDeleted }),
                        CustomCriteria = unitOfWork.InspectionV2CustomCriteriaRepository.Get(c => ids.Contains(c.cprod_id))
                        .Select(c => new { c.id, c.importance, c.category_id, c.cprod_id, c.criteria_id, c.point_id, c.requirements, c.requirements_cn, c.number, c.IsDeleted })
                    });
        }

        public ActionResult GetTemplateCriteria(int template_id)
        {
            return Json(unitOfWork.InspectionV2TemplateRepository.Get(t => t.id == template_id, includeProperties: "Criteria.Point")
                .FirstOrDefault()?.Criteria?.ToList());
        }

        public ActionResult GetCustomCriteria(IList<int> ids)
        {
            return Json(unitOfWork.InspectionV2CustomCriteriaRepository.Get(c => ids.Contains(c.cprod_id)));
        }

        public ActionResult AssignTemplate(IList<int> ids, int template_id)
        {
            var template = unitOfWork.InspectionV2TemplateRepository
                .Get(t => t.id == template_id, includeProperties: "Products").FirstOrDefault();

            if (template != null)
            {
                var products = unitOfWork.CustProductRepository.Get(p => ids.Contains(p.cprod_id)).ToList();
                foreach (var p in products)
                {
                    if (template.Products.All(x => x.cprod_id != p.cprod_id))
                    {
                        template.Products.Add(p);
                    }
                }
                unitOfWork.Save();
                return Json("OK");
            }

            return Json("Error");
        }


        public ActionResult SaveProductCriteria(List<Inspv2_customcriteria> customCriteria, List<int> deletedCriteria, List<int> productIds)
        {
            var products = unitOfWork.CustProductRepository.Get(p => productIds.Contains(p.cprod_id)).ToList();
            foreach (var cprod_id in productIds)
            {
                if (customCriteria != null)
                {
                    foreach (var c in customCriteria)
                    {
                        if (c.id < 0)
                        {
                            c.cprod_id = cprod_id;
                            unitOfWork.InspectionV2CustomCriteriaRepository.Insert(c);
                        }
                        else
                        {
                            var crit =
                                unitOfWork.InspectionV2CustomCriteriaRepository.Get(
                                    cr => cr.cprod_id == cprod_id && cr.criteria_id == c.criteria_id).FirstOrDefault();
                            if (crit == null || c.IsDeleted)
                            {
                                c.cprod_id = cprod_id;
                                unitOfWork.InspectionV2CustomCriteriaRepository.Insert(c);
                            }
                            else
                            {
                                unitOfWork.InspectionV2CustomCriteriaRepository.Copy(c, crit);
                            }
                        }
                    }
                    unitOfWork.Save();
                }


            }
            return Json("OK");
        }


        public ActionResult LoadingReport2(string id, string imagesFolder = "")
        {
            var iId = inspectionsDal.GetIdFromIdString(id, true);
            return View(BuildLoadingInspectionV2Model(iId, imagesFolder));
        }

        public ActionResult LoadingReport2Edit(int? id = null, string imagesFolder = "")
        {
            if (HttpContext != null)
            {
                //In testing there is no HttpContext
                WebUtilities.ClearTempFiles("containertemp_");
                WebUtilities.ClearTempFiles("temp_");
            }
            if (id != null)
            {
                var model = BuildLoadingInspectionV2Model(id.Value, imagesFolder, forEdit: true);
                if (model.Inspection.insp_status == InspectionV2Status.New)
                    return View(StripModelForEdit(model));
                else
                {
                    return View("LoadingReport2", model);
                }
            }
            ViewBag.message = "No id";
            return View("Message");


        }

        private LoadingInspectionV2ReportModel StripModelForEdit(LoadingInspectionV2ReportModel model)
        {
            //TODO later if necessary (if js code too big)
            return model;
        }

        public ActionResult LoadingReport2Render(Guid statsKey, int id, string imagesFolder = "")
        {
            if (statsKey == new Guid(Properties.Settings.Default.StatsKey))
            {
                return View("LoadingReport2", BuildLoadingInspectionV2Model(id, imagesFolder, true));
            }
            return null;
        }

        public ActionResult LoadingReport2PDF(string id, string imagesFolder = "", string options = LoadingReportPdfOptions)
        {
            return File(GenerateLoadingReportPDF(id, imagesFolder, options), "application/pdf");
        }

        private byte[] GenerateLoadingReportPDF(string id, string imagesFolder = "", string options = LoadingReportPdfOptions)
        {
            var iId = inspectionsDal.GetIdFromIdString(id, true);
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("LoadingReport2Render", 
                new { id = iId, statsKey = Settings.Default.StatsKey, imagesFolder }), options, 
                "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.SaveToMemory();
            return s;
        }

        public ActionResult ContainerFiles(string name, int container_id, int imageOrder)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize, string.Format("containertemp_{0}_{1}", container_id, imageOrder)), "text/html");
            //Thread.Sleep(6000);
            //return Json(new {success = true}, "text/html");
        }

        public ActionResult InspectionList(int id, bool edit = false)
        {
            var model = new InspectionProductListModel
            {
                //Inspection = _inspectionV2Repository.GetById(id),
                Inspection = inspectionsV2Dal.GetById(id),
                InspectionV2Types = unitOfWork.InspectionV2TypeRepository.Get().ToList(),
                Factories = companyDal.GetFactories(),
                ChangeNoticesProducts = unitOfWork.ChangeNoticeProductTableRepository.Get().ToList(),  //Load all for simplicity,
                ShowEditLink = !User.IsInRole(UserRole.Inspector.ToString()),
                EditMode = edit && !User.IsInRole(UserRole.Inspector.ToString()),
                CombinedInspections = new List<Inspection_v2>()
            };
            model.AutoAddedLines = ExtractAutoAddedLines(model.Inspection.Lines);
            if (model.Inspection != null)
            {
                foreach (var g in model.Inspection.Lines.Where(l => l.OrderLine != null).GroupBy(l => l.OrderLine.orderid))
                {
                    if (g.Key != null)
                    {
                        var combinedOrders = orderHeaderDal.GetCombinedOrders(g.Key.Value);
                        if (combinedOrders?.Count > 0)
                        {
                            foreach (var co in combinedOrders)
                            {
                                var inspections = inspectionsV2Dal.GetOrderInspections(co.orderid);
                                if (inspections.Count == 0)
                                {
                                    //TODO: Create inspections if they don't exist
                                    var insp = new Inspection_v2
                                    {
                                        type = model.Inspection.type,
                                        startdate = model.Inspection.startdate,
                                        duration = model.Inspection.duration,
                                        factory_id = model.Inspection.factory_id,
                                        client_id = model.Inspection.client_id

                                    };
                                    CreateFromOrders(insp, new[] { co.orderid }, true);
                                    insp = inspectionsV2Dal.GetById(insp.id);
                                    inspections.Add(insp);
                                }
                                if (inspections.Count > 0)
                                    model.CombinedInspections.AddRange(inspections.Where(i => model.CombinedInspections.Count(ci => ci.id == i.id) == 0));
                            }
                        }

                    }
                }
                model.Inspection.InspectionType = model.InspectionV2Types.FirstOrDefault(t => t.id == model.Inspection.type);
                /*var factoryIds =
                    model.Inspection.Lines.Select(
                        l =>
                            l.OrderLine.IfNotNull(
                                ol => ol.Cust_Product.IfNotNull(p => p.MastProduct.IfNotNull(m => m.factory_id))))
                        .Distinct().ToList();*/
                var mastIds = model.Inspection.Lines.Select(l => l.OrderLine?.Cust_Product?.cprod_mast).Distinct().ToList();
                if (mastIds.Count > 0)
                {
                    model.Returns = returnsDal.GetForMastProducts(mastIds.Where(f => f != null).Select(f => f.Value).ToList(), true, DateTime.Today.AddYears(-1));
                }
                // _returnRepository.GetForFactories(factoryIds.Where(f=>f != null).Select(f=>f.Value).ToList(),true,DateTime.Today.AddYears(-1));
                return View(model);
            }
            else
            {
                ViewBag.message = "No inspection with given id";
                return View("Message");
            }

        }

        private void CreateFromOrders(Inspection_v2 insp, IList<int> orderids, bool includeCombinedOrders = false)
        {
            insp.dateCreated = DateTime.Now;

            if (insp.Lines == null)
                insp.Lines = new List<Inspection_v2_line>();
            //var lines = Order_linesDAL.GetByOrderIds(orderids);
            var lines = unitOfWork.OrderLinesRepository.Get(l => l.orderid != null && orderids.Contains(l.orderid.Value),
                includeProperties: "Header,Cust_Product.MastProduct").ToList();
            if (!includeCombinedOrders)
                lines = lines.Where(l => l.Header.combined_order <= 0).ToList();
            foreach (var g in lines.GroupBy(l => l.orderid))
            {
                insp.Lines.AddRange(g.Select(l =>
                    new Inspection_v2_line
                    {
                        insp_custproduct_code = l?.Cust_Product?.cprod_code1,
                        insp_custproduct_name = l?.Cust_Product?.cprod_name,
                        insp_mastproduct_code = l?.Cust_Product?.MastProduct?.factory_ref,
                        qty = null,
                        orderlines_id = l.linenum,
                        Loadings = insp.type == Inspection_v2_type.Loading
                            ? new List<Inspection_v2_loading>()
                            {
                                new Inspection_v2_loading {Container = Utilities.SafeGetElement(insp.Containers, 0)}
                            }
                            : null
                    }));
            }

            unitOfWork.InspectionV2Repository.Insert(insp);
            unitOfWork.Save();

        }

        public ActionResult GetProducts(string prefixText)
        {
            var products = custproductsDal.GetByPatternAndFactory(prefixText, null);
            return Json(products.Select(p => new {
                p.cprod_id,
                p.cprod_code1,
                p.cprod_name,
                p.cprod_mast,
                p.MastProduct.factory_ref,
                p.MastProduct.factory_name,
                p.MastProduct.Factory.factory_code,
                combined_code = productService.GetCombinedCode(p.cprod_code1, p.MastProduct.factory_ref)
            }));
        }

        public ActionResult BulkUpdateLines(Inspection_v2_line[] lines)
        {
            if (lines != null)
            {
                foreach (var g in lines.Where(l => l.insp_id != null).GroupBy(l => l.insp_id))
                {
                    var insp = unitOfWork.InspectionV2Repository.Get(ins => ins.id == g.Key, includeProperties: "Lines").FirstOrDefault();
                    foreach (var l in g)
                    {
                        if (l.id <= 0)
                            insp.Lines.Add(l);
                        else
                        {
                            var oldLine = insp.Lines.FirstOrDefault(ol => ol.id == l.id);
                            if (oldLine != null)
                            {
                                oldLine.qty = l.qty;
                            }
                        }
                    }
                    unitOfWork.Save();
                }
            }

            return Json("OK");
        }

        public ActionResult Update(Inspection_v2 insp)
        {
            var i = unitOfWork.InspectionV2Repository.GetByID(insp.id);
            if (i != null)
            {
                i.startdate = insp.startdate;
                unitOfWork.Save();
            }

            return Json("OK");
        }

        private LoadingInspectionV2ReportModel BuildLoadingInspectionV2Model(int id, string imagesFolder, bool forPdf = false, bool forEdit = false)
        {
            var model = new LoadingInspectionV2ReportModel
            {
                ImagesFolder = imagesFolder,
                Inspection = GetById(id),
                ForPdf = forPdf,
                Areas = unitOfWork.InspectionV2AreaRepository.Get().ToList(),
                AllLoadings = new List<Inspection_v2_loading>(),
                CombinedInspections = new List<Inspection_v2>(),
                CombinedOrders = new List<Order_header>(),
                AutoAddedLines = new List<Inspection_v2_line>()
            };

            model.AllLoadings.AddRange(model.Inspection.AllLoadings.Where(l => l.Line?.qty > 0 || l.Line?.OrderLine?.orderqty > 0));
            var inspection_Container_id = model.Inspection.AllLoadings.Count > 0 ? model.Inspection.AllLoadings[0].container_id : null;
            //Add inspections for combined orders
            foreach (var g in model.Inspection.Lines.Where(l => l.OrderLine != null).GroupBy(l => l.OrderLine.orderid))
            {
                if (g.Key != null)
                {
                    var combinedOrders = orderHeaderDal.GetCombinedOrders(g.Key.Value);
                    if (combinedOrders.Count > 0)
                    {
                        model.CombinedOrders.AddRange(combinedOrders);
                        foreach (var co in model.CombinedOrders)
                        {
                            var combinedOrderInspections = inspectionsV2Dal.GetOrderInspections(co.orderid, true).Where(i => i.id != id).ToList();
                            if (combinedOrderInspections.Count > 0)
                            {
                                model.CombinedInspections.AddRange(combinedOrderInspections);
                                foreach (var coi in combinedOrderInspections)
                                {
                                    foreach (var lo in coi.AllLoadings)
                                    {
                                        lo.container_id = inspection_Container_id;
                                        if ((lo.Line?.qty > 0 || lo.Line?.OrderLine?.orderqty > 0) && model.AllLoadings.Count(l => l.Line.orderlines_id == lo.Line.orderlines_id) == 0)
                                            model.AllLoadings.Add(lo);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            model.AutoAddedLines = ExtractAutoAddedLines(model.Inspection.Lines);

            if (forEdit)
            {
                var insp = model.Inspection;


                model.EditModel = new LoadingInspection2EditModel
                {
                    Containers =
                        model.Inspection.Containers?.Select(
                            c =>
                                new Inspection_v2_container
                                {
                                    id = c.id,
                                    insp_id = c.insp_id,
                                    container_no = c.container_no,
                                    container_size = c.container_size,
                                    seal_no = c.seal_no,
                                    Images =
                                        c.Images.Select(
                                            im =>
                                                new Inspection_v2_container_images
                                                {
                                                    id = im.id,
                                                    insp_image = im.insp_image,
                                                    order = im.order,
                                                    container_id = im.container_id
                                                }).ToList()
                                }).ToList(),
                    AllLoadings =
                        model.AllLoadings?.Where(l => l.Line?.qty > 0 || l.Line?.OrderLine?.orderqty > 0)?.Select(
                            l =>
                                new InspectionV2Loading
                                {
                                    id = l.id,
                                    insp_line = l.insp_line,
                                    container_id = l.container_id,
                                    cprod_code = !string.IsNullOrEmpty(l.Line?.insp_custproduct_code) ? l.Line?.insp_custproduct_code : l.Line?.OrderLine?.Cust_Product?.cprod_code1,
                                    factory_ref = !string.IsNullOrEmpty(l.Line?.insp_mastproduct_code) ? l.Line?.insp_mastproduct_code : l.Line?.OrderLine?.Cust_Product?.MastProduct?.factory_ref,
                                    qty = l.Line?.qty > 0 ? l.Line?.qty : l.Line?.OrderLine?.orderqty,
                                    custpo = !string.IsNullOrEmpty(l.Line?.custpo) ? l.Line?.custpo : l.Line?.OrderLine?.Header?.custpo,
                                    full_pallets = l.full_pallets,
                                    loose_load_qty = l.loose_load_qty,
                                    mixed_pallet_qty = l.mixed_pallet_qty,
                                    mixed_pallet_qty2 = l.mixed_pallet_qty2,
                                    mixed_pallet_qty3 = l.mixed_pallet_qty3,
                                    area_id = l.area_id,
                                    AreasText = l.Areas != null ? string.Join(",", l.Areas.Select(a => a.name)) : "",
                                    QtyMixedPallets = l?.QtyMixedPallets?.Select(q =>
                                            new Inspection_v2_loading_mixedpallet
                                            {
                                                id = q.id,
                                                loading_id = q.loading_id,
                                                pallet_id = q.pallet_id,
                                                qty = q.qty
                                            }).ToList(),
                                    qty_per_pallet = l.qty_per_pallet,
                                    Description = !string.IsNullOrEmpty(l?.Line?.OrderLine?.Cust_Product?.cprod_name) ? l?.Line?.OrderLine?.Cust_Product?.cprod_name : l?.Line?.insp_custproduct_name
                                }).ToList(),
                    AllImages =
                        model.Inspection.AllImages?.Select(
                            im =>
                                new Inspection_v2_image
                                {
                                    id = im.id,
                                    insp_image = im.insp_image,
                                    insp_line = im.insp_line,
                                    order = im.order,
                                    type_id = im.type_id
                                }
                            ).ToList(),
                    Inspection =
                        new Inspection_v2
                        {
                            id = insp.id,
                            startdate = insp.startdate,
                            client_id = insp.client_id,
                            duration = insp.duration,
                            comments_admin = insp.comments_admin,
                            code = insp.code,
                            comments = insp.comments,
                            custpo = insp.custpo,
                            factory_id = insp.factory_id,
                            qc_required = insp.qc_required,
                            type = insp.type,
                            drawingFile = insp.drawingFile,
                            dateCreated = insp.dateCreated,
                            insp_batch_inspection = insp.insp_batch_inspection,
                            insp_status = insp.insp_status,
                            si_subject_id = insp.si_subject_id,
                            show_all_images = insp.show_all_images,
                            MixedPallets = insp.MixedPallets?.Select(mp => new Inspection_v2_mixedpallet { id = mp.id, insp_id = mp.insp_id, name = mp.name, area_id = mp.area_id }).ToList(),
                            Controllers = insp.Controllers?.Select(co => new Inspection_v2_controller { controller_id = co.controller_id, duration = co.duration, id = co.id, startdate = co.startdate, inspection_id = co.inspection_id }).ToList(),
                            Lines =
                                model.Inspection.Lines?.Select(
                                    l =>
                                        new Inspection_v2_line
                                        {
                                            id = l.id,
                                            custpo = l.custpo,
                                            insp_custproduct_code = l.insp_custproduct_code,
                                            insp_mastproduct_code = l.insp_mastproduct_code,
                                            insp_custproduct_name = l.insp_custproduct_name,
                                            insp_id = l.insp_id,
                                            qty = l.qty,
                                            Images =
                                                l.Images?.Select(
                                                    im =>
                                                        new Inspection_v2_image
                                                        {
                                                            id = im.id,
                                                            insp_image = im.insp_image,
                                                            insp_line = im.insp_line,
                                                            order = im.order
                                                        }).ToList(),
                                            Loadings =
                                                l.Loadings?.Select(
                                                    lo =>
                                                        new Inspection_v2_loading
                                                        {
                                                            id = lo.id,
                                                            container_id = lo.container_id,
                                                            full_pallets = lo.full_pallets,
                                                            insp_line = lo.insp_line,
                                                            loose_load_qty = lo.loose_load_qty,
                                                            mixed_pallet_qty = lo.mixed_pallet_qty,
                                                            mixed_pallet_qty2 = lo.mixed_pallet_qty2,
                                                            mixed_pallet_qty3 = lo.mixed_pallet_qty3,
                                                            area_id = lo.area_id,
                                                            qty_per_pallet = lo.qty_per_pallet
                                                        }).ToList()
                                        }).ToList()
                        },
                    Areas = model.Areas,
                    AutoAddedLines = model.AutoAddedLines?.Select(l => new Inspection_v2_line
                    {
                        Product = new Cust_products
                        {
                            cprod_code1 = l?.Product?.cprod_code1,
                            cprod_name = l?.Product?.cprod_name
                        },
                        OrderLine = new Order_lines
                        {
                            Header = new Order_header
                            {
                                custpo = l.OrderLine?.Header?.custpo
                            }
                        },
                        qty = l.qty
                    }).ToList()
                };


            }
            return model;
        }

        private List<Inspection_v2_line> ExtractAutoAddedLines(List<Inspection_v2_line> lines)
        {
            if (settings.InspectionV2_AddAutoAddedProducts_DateFrom != null)
            {
                AddAutoAddedProducts(lines);
                var dictAddedLines = new Dictionary<int?, Inspection_v2_line>();
                foreach (var l in lines.Where(l => l?.OrderLine?.orderqty > 0))
                {
                    var qty = Convert.ToInt32(l?.OrderLine?.orderqty.Value);
                    var product = l.OrderLine?.Cust_Product;
                    if (product.AutoAddedProducts != null)
                    {
                        foreach (var ap in product.AutoAddedProducts)
                        {
                            if (l?.OrderLine?.Header?.orderdate >= ap.startdate)
                            {
                                Inspection_v2_line line = null;
                                if (!dictAddedLines.ContainsKey(ap.added_cprod_id))
                                {
                                    line = new Inspection_v2_line
                                    {
                                        Product = ap.AddedProduct,
                                        cprod_id = ap.added_cprod_id,
                                        qty = qty,
                                        OrderLine = l.OrderLine
                                    };
                                    dictAddedLines[ap.added_cprod_id] = line;
                                }
                                else
                                {
                                    line = dictAddedLines[ap.added_cprod_id];
                                    line.qty += qty;
                                }
                            }

                        }
                    }
                }
                return dictAddedLines.Values.ToList();
            }
            return null;
        }

        private Inspection_v2 GetById(int id, bool loadRejections = false)
        {
            //var insp = unitOfWork.InspectionV2Repository.Get(i => i.id == id,
            //	includeProperties: @"Lines.Loadings, Lines.OrderLine.Cust_Product.MastProduct,Lines.OrderLine.Header,
            //			Factory, Client, InspectionType, MixedPallets, Containers.Images, Controllers.Controller").FirstOrDefault();
            var insp = inspectionsV2Dal.GetById(id, true, true);

            //if (insp != null && insp.Lines != null)
            //{

            //	foreach (var l in insp.Lines)
            //	{
            //		//Can't load first time, SQL gets f.. up
            //		unitOfWork.InspectionV2LineRepository.LoadCollection(l, "Images");
            //		if(loadRejections)
            //			unitOfWork.InspectionV2LineRepository.LoadCollection(l, "Rejections");
            //		foreach(var lo in l.Loadings) {
            //			unitOfWork.InspectionV2LoadingRepository.LoadCollection(lo, "Areas");
            //			unitOfWork.InspectionV2LoadingRepository.LoadCollection(lo, "QtyMixedPallets");
            //		}					
            //	}
            //	//AddAutoAddedProducts(insp.Lines);
            //}
            return insp;

        }

        private void AddAutoAddedProducts(List<Inspection_v2_line> lines)
        {
            var cprod_ids = lines.Select(l => l.cprod_id ?? l?.OrderLine?.cprod_id).Where(x => x != null).ToList();
            var autoAddedProducts = unitOfWork.CustProductRepository.GetAutoAddedProducts(cprod_ids).
                GroupBy(x => x.trigger_cprod_id).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var l in lines)
            {
                var custProduct = l?.OrderLine?.Cust_Product;
                if (custProduct != null)
                {
                    if (autoAddedProducts.ContainsKey(custProduct.cprod_id))
                        custProduct.AutoAddedProducts = autoAddedProducts[custProduct.cprod_id];

                }
            }
        }

        public ActionResult Report(int id, string imagesFolder = "")
        {
            return View("FinalReport", BuildFinalInspectionV2Model(id, imagesFolder));
        }

        public ActionResult ReportEdit(int id, string imagesFolder = "")
        {
            ClearFinalImagesFromSession();
            var model = BuildFinalInspectionV2Model(id, imagesFolder, forEdit: true);
            return View("FinalReportEdit", model);

        }

        public static string AdjustInspectionImage(string image_name)
        {
            return WebUtilities.CombineUrls(Settings.Default.InspectionImagesFolder, image_name);
        }

        private FinalInspectionV2ReportModel BuildFinalInspectionV2Model(int id, string imagesFolder, bool forPdf = false, bool forEdit = false)
        {
            var model = new FinalInspectionV2ReportModel
            {
                ImagesFolder = imagesFolder,
                Inspection = GetById(id, true),
                ForPdf = forPdf
            };
            if (forEdit)
            {
                var insp = model.Inspection;
                model.EditModel = GetEditModel(insp);

            }
            return model;
        }

        private FinalInspection2EditModel GetEditModel(Inspection_v2 insp)
        {
            return new FinalInspection2EditModel
            {
                ImageTypes = unitOfWork.InspectionV2ImageTypeRepository.Get().ToList(),
                AllImages =
                    insp.AllImages.Select(
                        im =>
                            new Inspection_v2_Image_ex
                            {
                                id = im.id,
                                insp_image = im.insp_image,
                                insp_line = im.insp_line,
                                order = im.order,
                                type_id = im.type_id,
                                comments = im.comments
                            }
                        ).ToList(),
                Inspection =
                    new Inspection_v2
                    {
                        id = insp.id,
                        startdate = insp.startdate,
                        client_id = insp.client_id,
                        duration = insp.duration,
                        comments_admin = insp.comments_admin,
                        code = insp.code,
                        comments = insp.comments,
                        custpo = insp.custpo,
                        factory_id = insp.factory_id,
                        qc_required = insp.qc_required,
                        type = insp.type,
                        Controllers = insp.Controllers.IfNotNull(ctrls => ctrls.Select(co => new Inspection_v2_controller { controller_id = co.controller_id, duration = co.duration, id = co.id, startdate = co.startdate, inspection_id = co.inspection_id }).ToList()),
                        Lines =
                            insp.Lines.Select(
                                l =>
                                    new Inspection_v2_line
                                    {
                                        id = l.id,
                                        custpo = l.custpo,
                                        insp_custproduct_code = l.insp_custproduct_code,
                                        insp_mastproduct_code = l.insp_mastproduct_code,
                                        insp_custproduct_name = l.insp_custproduct_name,
                                        insp_id = l.insp_id,
                                        inspected_qty = l.inspected_qty,
                                        orderlines_id = l.orderlines_id,
                                        qty = l.qty,
                                        OrderLine = l.OrderLine != null ? new Order_lines { orderqty = l.OrderLine.orderqty, Header = new Order_header { custpo = l.OrderLine.Header.IfNotNull(h => h.custpo) } } : null,
                                        Images =
                                            l.Images.IfNotNull(imgs => imgs.Select(
                                                im =>
                                                    new Inspection_v2_image
                                                    {
                                                        id = im.id,
                                                        insp_image = im.insp_image,
                                                        insp_line = im.insp_line,
                                                        order = im.order,
                                                        type_id = im.type_id,
                                                        comments = im.comments
                                                    }).ToList()),
                                        Rejections = l.Rejections.IfNotNull(rej => rej.Select(
                                            r => new Inspection_v2_line_rejection
                                            {
                                                id = r.id,
                                                line_id = r.line_id,
                                                action = r.action,
                                                ca = r.ca,
                                                permanentaction = r.permanentaction,
                                                reason = r.reason,
                                                rejection = r.rejection,
                                                type = r.type,
                                                document = r.document,
                                                comments = r.comments
                                            }).ToList())

                                    }).ToList()
                    }
            };
        }

        public ActionResult GetTempFileC(string file, int container_id, int imageOrder)
        {
            var oFile = WebUtilities.GetTempFile(file, string.Format("containertemp_{0}_{1}", container_id, imageOrder));
            if (oFile != null)
                return File(oFile, WebUtilities.ExtensionToContentType(Path.GetExtension(file).Replace(".", "")), file);
            return null;
        }

        public ActionResult GetTempFile(string file, string id = null)
        {
            //var oFile = WebUtilities.GetTempFile(file, !string.IsNullOrEmpty(id) ? $"tempulti_{id}" :  string.Format("temp_{0}", imageOrder));
            var oFile = WebUtilities.GetTempFile(id, "temp_");
            if (oFile != null)
                return File(oFile, WebUtilities.ExtensionToContentType(Path.GetExtension(file).Replace(".", "")), file);
            return null;
        }

        public ActionResult Files(string name, int imageOrder)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize, string.Format("temp_{0}", imageOrder)), "text/html");
        }

        public ActionResult Images(string name, string id)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            return Json(WebUtilities.SaveTempFile(id, Request, Settings.Default.Enquiries_MaxFileSize, "temp_"), "text/html");
        }

        public ActionResult UpdateLoading(Inspection_v2 insp, List<Inspection_v2_loading> combinedLoadings)
        {
            HandleUploadedFiles(insp);

            //_inspectionV2Repository.UpdateLoading(insp);
            unitOfWork.InspectionV2Repository.UpdateLoading(insp);

            if (combinedLoadings != null)
                unitOfWork.InspectionV2Repository.UpdateCombinedLoadings(combinedLoadings);
            unitOfWork.Save();
            foreach (var image in insp.AllImages)
            {
                image.Line = null;
            }
            foreach (var cont in insp.Containers)
            {
                cont.Inspection = null;
                if (cont.Images != null)
                {
                    foreach (var im in cont.Images)
                    {
                        im.Container = null;
                    }
                }
            }
            if (insp.MixedPallets != null)
            {
                foreach (var mp in insp.MixedPallets)
                {
                    mp.Area = null;
                    mp.Inspection = null;
                }
            }

            if (insp.AllLoadings != null)
            {
                foreach (var l in insp.AllLoadings)
                {
                    if (l.QtyMixedPallets != null)
                    {
                        foreach (var q in l.QtyMixedPallets)
                        {
                            q.Pallet = null;
                            q.Loading = null;
                        }
                    }
                }
            }


            WebUtilities.ClearTempFiles("temp_");
            WebUtilities.ClearTempFiles("containertemp_");

            return Json(insp);
        }

        private void HandleUploadedFiles(Inspection_v2 insp)
        {
            var inspectionRootRelativeFolder = GetInspectionImagesFolder(insp, Settings.Default.InspectionImagesFolder);

            //Handle container images
            if (insp.Containers != null)
            {
                foreach (var c in insp.Containers)
                {
                    if (c.Images != null)
                    {
                        var imagesToRemove = new List<Inspection_v2_container_images>();
                        foreach (var im in c.Images)
                        {
                            if (!string.IsNullOrEmpty(im.insp_image))
                            {
                                var oFile = WebUtilities.GetTempFile(im.insp_image, string.Format("containertemp_{0}_{1}", c.id, im.order));
                                if (oFile != null)
                                {
                                    var filePath = Utilities.WriteFile(im.insp_image, GetInspectionFolderFullPath(inspectionRootRelativeFolder, Settings.Default.InspectionImagesFolder),
                                        oFile);
                                    im.insp_image = WebUtilities.CombineUrls(inspectionRootRelativeFolder, Path.GetFileName(filePath));
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(im.insp_image))
                                        //No temp file found and no previous image, remove it (maybe session timeout)
                                        imagesToRemove.Add(im);
                                }
                            }
                        }
                        foreach (var im in imagesToRemove)
                            c.Images.Remove(im);
                    }
                }
            }

            //Handle inspection images
            foreach (var l in insp.Lines)
            {
                if (l.Images != null)
                {
                    var imagesToRemove = new List<Inspection_v2_image>();
                    foreach (var im in l.Images)
                    {
                        if (!string.IsNullOrEmpty(im.comments))
                        {
                            var oFile = WebUtilities.GetTempFile(im.comments, "temp_");
                            /*if(oFile == null) {
                                oFile = WebUtilities.GetTempFile(im.insp_image, string.Format("tempMulti_{0}", im.comments));   //comments contains file id from uploader
                            }*/
                            if (oFile != null)
                            {
                                var filePath = Utilities.WriteFile(im.insp_image, GetInspectionFolderFullPath(inspectionRootRelativeFolder, Settings.Default.InspectionImagesFolder),
                                    oFile);
                                im.insp_image = WebUtilities.CombineUrls(inspectionRootRelativeFolder, Path.GetFileName(filePath));
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(im.insp_image))
                                    //No temp file found and no previous image, remove it (maybe session timeout)
                                    imagesToRemove.Add(im);
                            }
                        }
                    }
                    foreach (var im in imagesToRemove)
                        l.Images.Remove(im);
                }
            }

        }

        /// <summary>
        /// Returns folder relative to inspection root images folder
        /// </summary>
        /// <param name="insp"></param>
        /// <param name="inspectionImagesRootFolder"></param>
        /// <returns></returns>
        public static string GetInspectionImagesFolder(Inspection_v2 insp, string inspectionImagesRootFolder)
        {
            var inspRootrelativeFolder = insp.startdate.ToString("yyyy-MM");
            var fullFolderPath = GetInspectionFolderFullPath(inspRootrelativeFolder, inspectionImagesRootFolder);
            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);
            return inspRootrelativeFolder;
        }

        public static string GetInspectionFolderFullPath(string inspectionRootRelativeUrl, string inspectionImagesRootFolder)
        {
            return System.Web.HttpContext.Current.Server.MapPath(Path.Combine(inspectionImagesRootFolder, inspectionRootRelativeUrl));
        }

        public static string GetInspectionFolderFullPath(Inspection_v2 insp, string inspectionImagesRootFolder = null)
        {
            if (inspectionImagesRootFolder == null)
                inspectionImagesRootFolder = Settings.Default.InspectionImagesFolder;
            return System.Web.HttpContext.Current.Server.MapPath(Path.Combine(inspectionImagesRootFolder, GetInspectionImagesFolder(insp, inspectionImagesRootFolder)));
        }

        public ActionResult CreateLoadingFromOrder(int orderid, int porderid)
        { //for testing params, orderid=3875, porderid=802842
            //var m = Order_headerDAL.GetById(orderid);

            return View(new OrderLoadingModel
            {
                OrderHeader = orderHeaderDal.GetById(orderid),
                PorderId = porderid
            });
        }
        public ActionResult CreateFinalFromOrder(int id = 3875)
        {
            var model = new OrderLoadingModelFinal
            {
                OrderHeader = orderHeaderDal.GetById(id),
                InspectionTypes = unitOfWork.InspectionV2TypeRepository.Get().ToList(),
                OrderLines = orderLinesDal.GetByOrderId(id),
                InspectionLinesTable = new List<Inspection_v2_line>()
            };
            model.InspectionLinesTable = model.OrderLines.Select(l => new Inspection_v2_line
            {
                //cprod_code1=custp.cprod_code1,
                //factory_ref=custp.MastProduct.factory_ref,
                //cprod_name=custp.cprod_name,
                //order_qty= m.orderqty,
                //insp_qty = m.orderqty  /* preset input-box*/
                orderlines_id = l.linenum,
                OrderLine = l,
                inspected_qty = Convert.ToInt32(l.orderqty * 0.1),
                insp_mastproduct_code = l.Cust_Product.IfNotNull(c => c.MastProduct.IfNotNull(m => m.factory_ref)),
                insp_custproduct_code = l.Cust_Product.IfNotNull(c => c.cprod_code1),
                insp_custproduct_name = l.Cust_Product.IfNotNull(c => c.cprod_name),
                qty = Convert.ToInt32(l.orderqty),
                cprod_id = l.cprod_id

            }).ToList();


            return View(model);
        }

        public ActionResult CombinedLoadingList()
        {
            var model = new OrdersCombinedLoadingList()
            {
                Clients = unitOfWork.CompanyRepository.GetClientsWithOrders(true),
                Factories = companyDal.GetFactories(),
                Containers = containerTypesDal.GetAll()
            };

            return View(model);
        }



        public ActionResult test()
        {
            return View();
        }
        public ActionResult CreateInspection(InspectionObjForUpdate insp)
        {
            var orderids = new List<int>();
            foreach (var item in insp.orderids)
            {
                orderids.Add(item);
            }



            if (insp.client_id < 0)
            {
                //combined
                insp.client_id = unitOfWork.CompanyRepository.Get(c => c.combined_factory == -1 * insp.client_id).FirstOrDefault()?.user_id;
            }
            var m = new Inspection_v2()
            {

                startdate = insp.startdate,
                type = insp.type,
                custpo = insp.custpo,
                factory_id = insp.factory_id,
                code = insp.code,
                client_id = insp.client_id,
                factory_loading = insp.factory_loading,
                duration = insp.duration,
                comments = insp.comments,
                qc_required = insp.qc_required ?? 1,
                drawingFile = insp.drawingFile,
                Containers = new List<Inspection_v2_container>(),
                Lines = new List<Inspection_v2_line>()
            };

            if (!string.IsNullOrEmpty(m.drawingFile))
            {
                var file = WebUtilities.GetTempFile(insp.file_id, "temp_");
                if (file != null)
                {
                    var inspectionRootRelativeFolder = GetInspectionImagesFolder(m, Settings.Default.InspectionImagesFolder);
                    var filePath = Utilities.WriteFile(insp.drawingFile, GetInspectionFolderFullPath(inspectionRootRelativeFolder, Settings.Default.InspectionImagesFolder),
                            file);
                    m.drawingFile = WebUtilities.CombineUrls(inspectionRootRelativeFolder, Path.GetFileName(filePath));
                }

            }

            if (insp.Containers != null)
            {
                foreach (var item in insp.Containers)
                {
                    for (int i = 0; i < item.num; i++)
                    {
                        m.Containers.Add(new Inspection_v2_container { container_size = item.id });
                    }
                }
            }
            if (insp.orderids == null || insp.orderids.Count == 0)
            {
                //If no orderids, save as normal inspection
                foreach (var item in insp.Lines/*.Where(c=>c.qty>0)*/)
                {
                    item.OrderLine = null;
                    m.Lines.Add(item);
                }

                m.dateCreated = DateTime.Now;
                unitOfWork.InspectionV2Repository.Insert(m);
                unitOfWork.Save();

                return Json("OK");
            }

            //create loading inspection from order ids
            CreateFromOrders(m, orderids);
            if (insp.orderids.Count > 1)
            {
                //Log combined inspection
                var amend = new Amendments
                {
                    userid = accountService.GetCurrentUser().userwelcome,
                    ref1 = insp.client_id.ToString(),
                    ref2 = insp.factory_id.ToString(),
                    new_data = "Orders: " + string.Join(",", insp.orderids),
                    process = "create combined LI inspection",
                    tablea = "Inspection_v2",
                    timedate = DateTime.Now
                };
                amendmentsDal.Create(amend);
            }
            return Json("OK");
        }

        
        [HttpPost]
        public ActionResult GetInspections(int? factory_id, int? client_id, string custpo, DateTime? from, DateTime? to, erp.Model.InspectionStatus status)
        {
            List<int> factoryIds = null, clientIds = null;
            if (factory_id == null && User.IsInRole(UserRole.FactoryController.ToString()))
                factoryIds =
                    companyDal.GetCompaniesForUser(accountService.GetCurrentUser().IfNotNull(u => u.userid))
                        .Select(f => f.user_id)
                        .ToList();
            if (factory_id.HasValue)
                factoryIds = new List<int>() { factory_id.Value };
            if (client_id == null && User.IsInRole(UserRole.ClientController.ToString()))
                clientIds =
                    companyDal.GetCompaniesForUser(accountService.GetCurrentUser().IfNotNull(u => u.userid), Company_User_Type.Client)
                        .Select(f => f.user_id)
                        .ToList();
            if (client_id.HasValue)
                clientIds = new List<int>() { client_id.Value };
            //var inspections = _inspectionV2Repository.GetByCriteria(factoryIds, clientIds, custpo, from, to, ConvertUIStatusToDbStatuses(status), User.IsInRole(UserRole.Inspector.ToString()) ? (int?) accountService.GetCurrentUser().userid : null);
            var inspections = inspectionsV2Dal.GetByCriteria(factoryIds, clientIds, custpo, from, to, ConvertUIStatusToDbStatuses(status),
                User.IsInRole(UserRole.Inspector.ToString()) ? (int?)accountService.GetCurrentUser().userid : null);
            var result = new InspectionListResult();
            result.FiLiInspections = new List<InspectionListItem>();
            var usedInspections = new SortedSet<int>();
            foreach (var insp in inspections.Where(i => i.type == Inspection_v2_type.Final || i.type == Inspection_v2_type.Loading).OrderBy(i => i.Factory.IfNotNull(f => f.factory_code)).ThenBy(i => i.startdate))
            {
                if (!usedInspections.Contains(insp.id))
                {
                    usedInspections.Add(insp.id);
                    Inspection_v2 other = null;
                    var item = new InspectionListItem { status = ConvertDbStatusToUIStatus(insp.insp_status) };
                    other =
                        inspections.FirstOrDefault(
                            i =>
                                i.id != insp.id && i.type == (insp.type == Inspection_v2_type.Final ? Inspection_v2_type.Loading : Inspection_v2_type.Final) &&
                                i.Lines.Select(l => l.OrderLine.IfNotNull(ol => ol.Header.IfNotNull(h => h.custpo)))
                                    .Distinct()
                                    .Intersect(
                                        insp.Lines.Select(
                                            l => l.OrderLine.IfNotNull(ol => ol.Header.IfNotNull(h => h.custpo)))
                                            .Distinct()).Any());
                    if (other != null)
                        usedInspections.Add(other.id);
                    if (insp.type == Inspection_v2_type.Final)
                    {
                        item.FI = CreateDisplayObject(insp);
                        item.LI = CreateDisplayObject(other);
                    }
                    else if (insp.type == Inspection_v2_type.Loading)
                    {
                        item.LI = CreateDisplayObject(insp);
                        item.FI = CreateDisplayObject(other);
                    }
                    result.FiLiInspections.Add(item);
                }

            }
            result.OtherInspections =
                inspections.Where(i => i.type != Inspection_v2_type.Final && i.type != Inspection_v2_type.Loading)
                    .OrderBy(i => i.Factory.IfNotNull(f => f.factory_code)).Select(CreateDisplayObject)
                    .ToList();
            return Json(result);
        }



        private List<InspectionV2Status?> ConvertUIStatusToDbStatuses(InspectionStatus status)
        {
            var statuses = new List<InspectionV2Status?>();
            if (User.IsInRole(UserRole.FactoryController.ToString()) || User.IsInRole(UserRole.Administrator.ToString()) || User.IsInRole(UserRole.ClientController.ToString()))
            {
                if (status == InspectionStatus.Todo)
                {
                    statuses.Add(InspectionV2Status.New);
                    statuses.Add(InspectionV2Status.ReportSubmitted);

                }
                else if (status == InspectionStatus.AwaitingReport)
                    statuses.Add(InspectionV2Status.ListReady);
            }
            else if (User.IsInRole(UserRole.Inspector.ToString()))
            {
                if (status == InspectionStatus.Todo)
                    statuses.Add(InspectionV2Status.ListReady);
                else if (status == InspectionStatus.AwaitingReview)
                    statuses.Add(InspectionV2Status.ReportSubmitted);

            }
            if (status == InspectionStatus.Rejected)
                statuses.Add(InspectionV2Status.Rejected);
            if (status == InspectionStatus.Accepted)
                statuses.Add(InspectionV2Status.Accepted);
            if (status == InspectionStatus.Cancelled)
                statuses.Add(InspectionV2Status.Cancelled);
            return statuses;
        }

        private InspectionStatus ConvertDbStatusToUIStatus(InspectionV2Status? inspStatus)
        {
            if (User.IsInRole(UserRole.FactoryController.ToString()) || User.IsInRole(UserRole.Administrator.ToString()) || User.IsInRole(UserRole.ClientController.ToString()))
            {
                if (inspStatus == InspectionV2Status.New || inspStatus == InspectionV2Status.ReportSubmitted)
                    return InspectionStatus.Todo;
                if (inspStatus == InspectionV2Status.ListReady)
                    return InspectionStatus.AwaitingReport;
            }
            else if (User.IsInRole(UserRole.Inspector.ToString()))
            {
                if (inspStatus == InspectionV2Status.ListReady)
                    return InspectionStatus.Todo;
                if (inspStatus == InspectionV2Status.ReportSubmitted)
                    return InspectionStatus.AwaitingReview;
            }
            if (inspStatus == InspectionV2Status.Rejected)
                return InspectionStatus.Rejected;
            if (inspStatus == InspectionV2Status.Accepted)
                return InspectionStatus.Accepted;
            if (inspStatus == InspectionV2Status.Cancelled)
                return InspectionStatus.Cancelled;
            return InspectionStatus.Undefined;
        }

        private InspectionForDisplay CreateDisplayObject(Inspection_v2 i)
        {
            if (i == null)
                return null;
            return new InspectionForDisplay
            {
                id = i.id,
                customer_code = i.Client.IfNotNull(c => c.customer_code),
                factory_code = i.Factory.IfNotNull(f => f.factory_code),
                custpos = i.Lines.Select(l => l.OrderLine.IfNotNull(ol => ol.Header.IfNotNull(h => h.custpo))).Distinct().ToArray(),
                startdate = i.startdate,
                insp_status = i.insp_status,
                type = i.InspectionType.IfNotNull(t => t.name)
            };
        }



        public ActionResult GetFactoriesForClient(int user_id)
        {
            var userIds = GetUserIds(user_id);
            var result = unitOfWork.CompanyRepository.Get(c => c.POrders.Any(po => userIds.Contains(po.OrderHeader.userid1.Value))).ToList();
            return Json(result.Select(c => new { c.user_id, c.factory_code }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPoList(int user_id)
        {
            var startdate = DateTime.Now.AddMonths(-2);
            var userIds = GetUserIds(user_id);
            var model = unitOfWork.OrderRepository.Get(o => (o.status != "X" && o.status != "Y") && userIds.Contains(o.userid1.Value) && o.PorderHeaders.Max(po => po.po_req_etd) >= startdate).
                Select(o => new { o.orderid, o.userid1, o.custpo }).
                OrderBy(o => o.custpo).
                ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<int> GetUserIds(int user_id)
        {
            var userIds = new List<int> { user_id };
            if (user_id < 0)
            {
                //Combined client
                userIds = unitOfWork.CompanyRepository.Get(c => c.combined_factory == -1 * user_id).Select(c => c.user_id).ToList();
            }
            return userIds;
        }


        public ActionResult DeleteInspection(int id)
        {
            unitOfWork.InspectionV2Repository.DeleteByIds(new[] { id });
            return Json("OK");
        }

        private List<LookupItem> GetFiltersForUser(IPrincipal user)
        {
            var result = new List<LookupItem>();
            result.Add(new LookupItem { id = (int)InspectionStatus.Todo, value = "to do" });
            if (user.IsInRole(UserRole.FactoryController.ToString()) || user.IsInRole(UserRole.Administrator.ToString()))
            {
                result.Add(new LookupItem { id = (int)InspectionStatus.AwaitingReport, value = "awaiting report" });
            }
            else if (user.IsInRole(UserRole.Inspector.ToString()))
            {
                result.Add(new LookupItem { id = (int)InspectionStatus.AwaitingReview, value = "awaiting review" });
            }
            result.Add(new LookupItem { id = (int)InspectionStatus.Rejected, value = "rejected" });
            result.Add(new LookupItem { id = (int)InspectionStatus.Accepted, value = "accepted" });
            return result;
        }

        public ActionResult Import(DateTime? from = null, DateTime? to = null)
        {
            if (from == null)
                from = DateTime.Today.AddYears(-2);
            if (to == null)
                to = DateTime.Today;
            var clients = companyDal.GetClients();
            var inspections = inspectionsDal.GetByCriteria(from.Value, to.Value, null);
            var types = unitOfWork.InspectionV2TypeRepository.Get().ToList();
            Inspections currinsp = null;
            try
            {
                foreach (var oldinsp in inspections.Where(i => i.insp_type != "SI").OrderBy(i => i.insp_start))
                {
                    currinsp = oldinsp;
                    var insp = new Inspection_v2
                    {
                        client_id = clients.FirstOrDefault(c => c.customer_code == oldinsp.customer_code).IfNotNull(c => c.user_id),
                        factory_id = oldinsp.Factory.IfNotNull(f => f.user_id > 0 ? (int?)f.user_id : null),
                        comments = oldinsp.insp_comments,
                        code = oldinsp.insp_id,
                        comments_admin = oldinsp.insp_comments_admin,
                        startdate = oldinsp.insp_start,
                        duration = oldinsp.insp_days,
                        insp_status = oldinsp.insp_status == 0 ? InspectionV2Status.ListReady : oldinsp.acceptance_fc == 2
                                                               ? InspectionV2Status.Rejected : oldinsp.acceptance_fc == 1
                                                               ? InspectionV2Status.Accepted : oldinsp.insp_status == 1 && oldinsp.acceptance_fc == 0
                                                               ? InspectionV2Status.ReportSubmitted : InspectionV2Status.New,
                        insp_batch_inspection = oldinsp.insp_batch_inspection,
                        qc_required = oldinsp.qc_required,
                        type = oldinsp.insp_type == "LO" ? Inspection_v2_type.Loading : types.FirstOrDefault(t => t.name == oldinsp.insp_type).IfNotNull(t => t.id)
                    };
                    if (insp.type == 0)
                        insp.type = null;
                    var linesold = inspectionLinesTestedDal.GetByInspection(oldinsp.insp_unique);
                    var loadingsOld = inspectionsLoadingDal.GetForInspection(oldinsp.insp_unique);
                    insp.Lines = new List<Inspection_v2_line>();
                    List<Containers> containers = null;
                    var containerTypes = unitOfWork.ContainerTypeRepository.Get().ToList();
                    if (oldinsp.insp_type == "LO")
                    {
                        containers = containerDal.GetForInspection(oldinsp.insp_unique);
                        if (containers != null)
                        {
                            insp.Containers = new List<Inspection_v2_container>();
                            foreach (var c in containers)
                            {
                                int? type_id = null;
                                var type = containerTypes.FirstOrDefault(t => t.shortname == c.container_size);
                                if (type != null)
                                    type_id = type.container_type_id;
                                insp.Containers.Add(new Inspection_v2_container { container_comments = c.container_comments, container_no = c.container_no, container_size = type_id, container_space = c.container_space, container_count = c.container_count, seal_no = c.seal_no });

                            }
                        }
                    }

                    foreach (var l in linesold)
                    {
                        int? linenum = l.OrderLine.IfNotNull(ol => ol.linenum);
                        if (linenum > 0)
                        {
                            var line = new Inspection_v2_line
                            {
                                orderlines_id = linenum,
                                cprod_id = l.OrderLine.IfNotNull(ol => ol.cprod_id),
                                qty = Convert.ToInt32(l.OrderLine.IfNotNull(ol => ol.orderqty) ?? l.insp_qty)
                            };
                            if (oldinsp.insp_type == "LO")
                            {
                                var loadingsForLine = loadingsOld.Where(lo => lo.insp_line_unique == l.insp_line_unique).ToList();
                                if (loadingsForLine.Count > 0)
                                {
                                    line.Loadings = new List<Inspection_v2_loading>();
                                    foreach (var lo in loadingsForLine)
                                    {
                                        var oldcont = containers.FirstOrDefault(c => c.container_id == lo.container);
                                        if (oldcont != null)
                                        {
                                            var type = containerTypes.FirstOrDefault(t => t.shortname == oldcont.container_size);
                                            Inspection_v2_container newCont = null;
                                            if (type != null)
                                            {
                                                newCont =
                                                    insp.Containers.FirstOrDefault(
                                                        c => c.container_size == type.container_type_id && c.container_count == oldcont.container_count);

                                            }
                                            line.Loadings.Add(new Inspection_v2_loading
                                            {
                                                Container = newCont,
                                                full_pallets = lo.full_pallets,
                                                loose_load_qty = lo.loose_load_qty,
                                                mixed_pallet_qty = lo.mixed_pallet_qty,
                                                mixed_pallet_qty2 = lo.mixed_pallet_qty2,
                                                mixed_pallet_qty3 = lo.mixed_pallet_qty3
                                            });
                                        }

                                    }

                                }
                            }

                            insp.Lines.Add(line);
                        }

                    }
                    insp.Controllers = new List<Inspection_v2_controller>();
                    foreach (var c in oldinsp.Controllers)
                    {
                        insp.Controllers.Add(new Inspection_v2_controller { controller_id = c.controller_id, startdate = c.startdate, duration = c.duration });
                    }

                    unitOfWork.InspectionV2Repository.Insert(insp);
                    unitOfWork.Save();
                }
            }
            catch (Exception e)
            {

                ViewBag.message = "Import crashed for inspection: " + currinsp.insp_unique + "\n" + e;
                return View("Message");
            }

            ViewBag.message = "import finished";
            return View("Message");

        }

        public ActionResult FinalInspectionFiles(string name, int type, string id)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            object result;
            var bytes = WebUtilities.GetFileFromRequest(fileName, Request, Settings.Default.Enquiries_MaxFileSize,
                out result);
            var savedImages = GetFinalImagesFromSession();
            if (savedImages == null)
                savedImages = new List<Inspection_v2_Image_ex>();
            savedImages.Add(new Inspection_v2_Image_ex { insp_image = name, type_id = type, fileId = id, Data = bytes });
            Session["FinalInspImages"] = savedImages;
            return Json(result);
        }

        public ActionResult GetFinalTempFile(string id)
        {
            var savedImages = GetFinalImagesFromSession();
            var oFile = savedImages.FirstOrDefault(im => im.fileId == id);
            if (oFile != null)
                return File(oFile.Data, WebUtilities.ExtensionToContentType(Path.GetExtension(oFile.insp_image).IfNotNull(f => f.Substring(1))), oFile.insp_image);
            return null;
        }

        private List<Inspection_v2_Image_ex> GetFinalImagesFromSession()
        {
            return (List<Inspection_v2_Image_ex>)Session["FinalInspImages"];
        }

        private void ClearFinalImagesFromSession()
        {
            Session["FinalInspImages"] = null;
        }

        private void HandleFinalUploadedFiles(Inspection_v2 insp, List<Inspection_v2_Image_ex> images)
        {
            var savedImages = GetFinalImagesFromSession();
            var inspectionRootRelativeFolder = GetInspectionImagesFolder(insp, Settings.Default.InspectionImagesFolder);
            foreach (var im in images.Where(i => !string.IsNullOrEmpty(i.fileId)))
            {
                var saved = savedImages.FirstOrDefault(i => i.fileId == im.fileId);
                if (saved != null)
                {
                    var filePath = Utilities.WriteFile(im.insp_image, GetInspectionFolderFullPath(inspectionRootRelativeFolder, Settings.Default.InspectionImagesFolder),
                            saved.Data);
                    im.insp_image = WebUtilities.CombineUrls(inspectionRootRelativeFolder, Path.GetFileName(filePath));
                }
            }
        }

        public ActionResult UpdateFinal(Inspection_v2 insp, List<Inspection_v2_Image_ex> images)
        {
            HandleFinalUploadedFiles(insp, images);
            var allImages = insp.Lines.Where(l => l.Images != null).Aggregate(new List<Inspection_v2_image>(),
                (l1, l2) => l1.Union(l2.Images).ToList());
            //Check image line change
            foreach (var im in images)
            {
                var line = insp.Lines.FirstOrDefault(l => l.id == im.insp_line);
                if (im.id <= 0)
                {
                    if (line == null)
                        line = insp.Lines.FirstOrDefault();
                    if (line != null)
                    {
                        if (line.Images == null)
                            line.Images = new List<Inspection_v2_image>();
                        line.Images.Add(new Inspection_v2_image { insp_image = im.insp_image, type_id = im.type_id, order = im.order, rej_flag = im.rej_flag, comments = im.comments });
                    }

                }
                else
                {
                    var oldLine =
                        insp.Lines.FirstOrDefault(
                            l => l.id == allImages.FirstOrDefault(i => i.id == im.id).IfNotNull(i => i.insp_line));
                    if (oldLine != null)
                    {
                        if (oldLine != line)
                        {
                            if (oldLine.Images != null)
                            {
                                var img = oldLine.Images.FirstOrDefault(i => i.id == im.id);
                                if (img != null)
                                    oldLine.Images.Remove(img);
                            }
                            if (line != null)
                            {
                                if (line.Images == null)
                                    line.Images = new List<Inspection_v2_image>();
                                line.Images.Add(im);
                            }
                        }
                        else
                        {
                            var img = oldLine.Images.FirstOrDefault(i => i.id == im.id);
                            if (img != null)
                            {
                                img.comments = im.comments;
                            }
                        }
                    }
                }

            }

            var deletedImages = allImages.Where(im => images.Count(i => i.id == im.id) == 0).ToList();
            foreach (var d in deletedImages)
            {
                var line = insp.Lines.FirstOrDefault(l => l.id == d.insp_line);
                if (line != null && line.Images != null)
                    line.Images.Remove(d);
            }
            unitOfWork.InspectionV2Repository.UpdateFinal(insp);
            return Json(GetEditModel(insp));
        }

        public ActionResult SubmitReport(int id)
        {
            //_inspectionV2Repository.UpdateStatus(id, InspectionV2Status.ReportSubmitted);
            var insp = unitOfWork.InspectionV2Repository.GetByID(id);
            var userid = accountService.GetCurrentUser()?.userid;
            insp.insp_status = InspectionV2Status.ReportSubmitted;
            var comment = unitOfWork.QcCommentsRepository.Get(c => c.inspv2_id == id && c.insp_comments_from == userid
            && c.insp_comments == "Report Submitted").FirstOrDefault();
            if (comment == null)
                unitOfWork.QcCommentsRepository.Insert(
                    new Qc_comments
                    {
                        inspv2_id = id,
                        insp_comments = "Report Submitted",
                        insp_comments_from = userid,
                        insp_comments_date = DateTime.Now,
                        insp_comments_type = "QC1"
                    });
            unitOfWork.Save();
            SendLoadingReportMail(id);
            return Json("OK");
        }

        private void SendLoadingReportMail(int id)
        {
            var reportData = GenerateLoadingReportPDF(id.ToString());
            var mailSettings = Properties.Settings.Default.LoadingInspectionReportSubmitted_MailSettings;
            if (mailSettings != null)
            {
                //Get FC
                var insp = unitOfWork.InspectionV2Repository.Get(i => i.id == id, includeProperties: "InspectionType, Factory, Client").FirstOrDefault();
                var factory_id = insp?.factory_id;
                if (factory_id != null)
                {
                    var factoryControllers = adminPermissionsDal.GetByCompany(factory_id.Value).Where(ap => ap.processing == 1).ToList();
                    mailSettings.to = string.Join(",", factoryControllers.Where(fc => !string.IsNullOrEmpty(fc.User?.user_email)).Select(fc => fc.User.user_email).
                        Union(mailSettings.to.Split(',')).Distinct());
                }
                var ms = new MemoryStream(reportData);
                var attach = new Attachment(ms, $"{insp.ComputedCode}.pdf");
                mailHelper.SendMail(Settings.Default.FromAccount, mailSettings.to, mailSettings.subject + "-" + insp.ComputedCode, mailSettings.body, mailSettings.cc,
                    mailSettings.bcc, new[] { attach });
            }


        }

        [HttpPost]
        public ActionResult ModifyReport(LoadingInspectionV2ReportModel m)
        {
            var insp = unitOfWork.InspectionV2Repository.GetByID(m.Inspection.id);
            if (insp != null)
            {
                insp.insp_status = InspectionV2Status.New;
                unitOfWork.Save();
            }
            return RedirectToAction("LoadingReport2Edit", new { id = m.Inspection.id, imagesFolder = m.ImagesFolder });
        }

        public ActionResult TrackingNumberTemplate(int id)
        {
            var inspection = unitOfWork.InspectionV2Repository.Get(i => i.id == id, includeProperties: "Lines.OrderLine.Cust_Product.MastProduct").FirstOrDefault();
            if (inspection != null)
            {
                ViewBag.Data = inspection.Lines.Where(l => l.OrderLine?.Cust_Product?.MastProduct?.range1 == 1).Select(l => l.OrderLine?.Cust_Product?.MastProduct).ToList();
                Response.AddHeader("Content-Disposition", "attachment;filename=TrackingNumberTemplate.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return View();
            }
            ViewBag.message = "No inspection with that id";
            return View("Message");
        }

        public ActionResult TrackingNumbers(int id)
        {
            var oldInspId = inspectionsDal.GetByNewInspId(id)?.insp_unique;
            var model = new TrackingNumbersModel
            {
                ProductTrackNumbers = unitOfWork.ProductTrackNumberQcRepository.Get(n => n.insp_id == oldInspId).ToList(),
                Lines = unitOfWork.InspectionV2Repository.Get(i => i.id == id, includeProperties: "Lines.OrderLine.Cust_Product.MastProduct")
                        .FirstOrDefault()?.Lines.Where(l => l.OrderLine?.Cust_Product?.MastProduct?.range1 == 1 && (l.qty != 0 && l.OrderLine?.orderqty != 0))
                        .Select(l => new InspectionListFlatLine
                        {
                            mast_id = l.OrderLine?.Cust_Product?.cprod_mast,
                            factory_ref = l.OrderLine?.Cust_Product?.MastProduct?.factory_ref,
                            qty = l.qty ?? l.OrderLine?.orderqty
                        }).ToList()
            };
            return View(model);
        }

        public ActionResult TrackingNumbersEdit(int id)
        {
            return View();
        }

        public ActionResult GetTrackingNumbersDataJSON(int id)
        {
            var oldInspId = inspectionsDal.GetByNewInspId(id)?.insp_unique;
            return Json(new
            {
                trackingNumbers = unitOfWork.ProductTrackNumberQcRepository.Get(n => n.insp_id == oldInspId)
                .Select(n => new { n.insp_id, n.mastid, n.producttrack_id, n.track_number })
                .ToList(),
                lines = unitOfWork.InspectionV2Repository.Get(i => i.id == id, includeProperties: "Lines.OrderLine.Cust_Product.MastProduct")
                        .FirstOrDefault()?.Lines.Where(l => l.OrderLine?.Cust_Product?.MastProduct?.range1 == 1)
                        .Select(l => new {
                            mast_id = l.OrderLine?.Cust_Product?.cprod_mast,
                            factory_ref = l.OrderLine?.Cust_Product?.MastProduct?.factory_ref,
                            qty = l.qty ?? l.OrderLine?.orderqty
                        }).ToList(),
                oldInspId = oldInspId
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUploadedTrackingNumbersJSON()
        {
            return Json(Session["TrackingNumbers"]);
        }

        public ActionResult SaveTrackingNumbers(List<products_track_number_qc> list)
        {
            if (list != null && list.Count > 0)
            {
                if (list[0].producttrack_id <= 0)
                {
                    //new list
                    unitOfWork.ProductTrackNumberQcRepository.DeleteForInspection(list[0].insp_id ?? 0);
                    foreach (var t in list)
                    {
                        unitOfWork.ProductTrackNumberQcRepository.Insert(t);
                    }
                }
                else
                {
                    //update
                    var insp_id = list[0].insp_id;
                    var existing = unitOfWork.ProductTrackNumberQcRepository.Get(t => t.insp_id == insp_id).ToList();
                    foreach (var t in list)
                    {
                        var old = existing.FirstOrDefault(tn => tn.producttrack_id == t.producttrack_id);
                        if (old != null)
                        {
                            old.track_number = t.track_number;
                        }
                    }
                    var toDelete = existing.Where(t => list.Count(n => n.producttrack_id == t.producttrack_id) == 0).ToList();
                    foreach (var d in toDelete)
                        unitOfWork.ProductTrackNumberQcRepository.Delete(d);

                }
                unitOfWork.Save();
            }
            return Json(list);
        }


        public ActionResult TrackingNumberFile(string name)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            object r;
            byte[] fileBytes = WebUtilities.GetFileFromRequest(fileName, Request, Settings.Default.Enquiries_MaxFileSize, out r);
            var filePath = Utilities.WriteFile(fileName, Server.MapPath(Settings.Default.TrackingNumberFilesFolder), fileBytes);
            var extension = Path.GetExtension(filePath);

            FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);


            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = extension == ".xls" ? ExcelReaderFactory.CreateBinaryReader(stream) : ExcelReaderFactory.CreateOpenXmlReader(stream);
            //...
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //DataSet result = excelReader.AsDataSet();
            //...
            //4. DataSet - Create column names from first row
            //excelReader.IsFirstRowAsColumnNames = true;
            //DataSet ds = excelReader.AsDataSet();
            DataSet ds = excelReader.AsDataSet(
				new ExcelDataSetConfiguration()
				{
					ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
					{
						UseHeaderRow = true
					}
				}
				);
            var result = new List<products_track_number_qc>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(
                    new products_track_number_qc
                    {
                        mastid = erp.Model.Utilities.FromDbValue<int>(row["mast_id"]),
                        track_number = string.Empty + row["tracking_number"]
                    });
            }
            Session["TrackingNumbers"] = result;

            System.IO.File.Delete(filePath);
            return Json(new { success = true, trackingNumbers = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TrackingNumbersMail(int id)
        {
            Inspections insp = inspectionsDal.GetById(id);

            if (insp != null)
            {
                string qc_insp = String.Join(",", insp.Controllers.Select(c => c.Controller.user_initials));
                string report_number_insp = String.Format("{0}-{1}-{2}-{3}", insp.factory_code, insp.insp_type, insp.insp_start.ToString("yyMMdd"), insp.customer_code);

                var trackNUmbersList = unitOfWork.ProductTrackNumberQcRepository.Get(n => n.insp_id == id, includeProperties: "MastProduct")
                    .ToList();

                var model = new TrackingNumbersModelMail
                {
                    customer_code = insp.customer_code,
                    custpo = insp.custpo,
                    factory_code = insp.factory_code,
                    id = insp.insp_unique,
                    qc = qc_insp,
                    report_number = report_number_insp,
                    product_track_number_gc_list = trackNUmbersList

                };

                Company company = companyDal.GetByFactoryCode(insp.factory_code);
                var fcs = new List<User>();
                fcs = adminPermissionsDal.GetByCompany(company.user_id).Where(a => a.feedbacks == 1).Select(f => f.User).ToList();

                string companyusers = string.Empty;
                if (fcs.Count > 0)
                {
                    //only users with email listed
                    fcs = fcs.Where(n => !string.IsNullOrEmpty(n.user_email)).ToList();
                    companyusers = string.Join(",", fcs.Select(n => n.user_email).ToList());
                }

                var to = Properties.Settings.Default.TrackingNumbersMail__To;

                if (!string.IsNullOrEmpty(to))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(companyusers);
                    sb.Append(",");
                    sb.Append(to);

                    companyusers = sb.ToString();
                }

                var subject = Properties.Settings.Default.TrackingNumbersMail_Subject;
                var cc = Properties.Settings.Default.TrackingNumbersMail__Cc;
                var bcc = Properties.Settings.Default.TrackingNumbersMail__Bcc;

                ViewData.Model = model;

                var body = string.Empty;

                body = WebUtilities.RenderRazorViewToString(ControllerContext, "TrackingNumbersMail", ViewData, TempData);

                mailHelper.SendMail(Settings.Default.FromAccount, companyusers, subject, body, cc, bcc);


                return View("TrackingNumbersMail", model);
                //return Json("OK", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NOTOK", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SampleInspections()
        {
            WebUtilities.ClearTempFiles();
            return View();
        }

        public ActionResult SampleInspectionReportPdf(int id)
        {
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("SampleInspectionReportRender", 
                new { id = id, statsKey = Settings.Default.StatsKey }), "scale=0.78, leftmargin=12,rightmargin=12,media=1", 
                "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf");
        }

        public ActionResult SampleInspectionReportRender(int id, Guid statsKey)
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                var imageTypes = new List<int> {
                    Inspection_v2_image_type.Appearance, Inspection_v2_image_type.Dimension,
                    Inspection_v2_image_type.Function, Inspection_v2_image_type.Material ,
                    Inspection_v2_image_type.Packaging, Inspection_v2_image_type.Summary };
                ViewBag.ImageTypes = unitOfWork.InspectionV2ImageTypeRepository.Get(t => imageTypes.Contains(t.id)).ToList();

                var insp = unitOfWork.InspectionV2Repository.Get(i => i.id == id,
                    includeProperties: "Controllers.Controller, Lines.Product.MastProduct,Lines.Images,Factory,Client,Subject,InspectionType").FirstOrDefault();
                if (insp != null)
                {
                    //Separate load of SIDetails because more than one sub-collection  can't be handled by EF/mySQL driver
                    foreach (var l in insp.Lines)
                    {
                        unitOfWork.InspectionV2LineRepository.LoadCollection(l, "SiDetails");
                    }
                }

                ViewBag.Inspection = insp;
                return View("SampleInspectionReport");
            }
            return null;
        }

        public ActionResult CreateNR(int insp_id)
        {

            return View("EditNR");
            //ViewBag.message = "Invalid inspection id";
            //return View("Message");

        }

        public ActionResult EditNR(int id)
        {

            return View("EditNR");
            //ViewBag.message = "Invalid inspection id";
            //return View("Message");

        }

        public ActionResult EditNrReport(int id)
        {
            return View("EditNrReport");
        }

        public ActionResult NrPdfReport(int id, string options = "scale=0.75,leftmargin=22,rightmargin=22,media=1")
        {
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            //string test = WebUtilities.GetSiteUrl() + Url.Action("RenderNrReport", new { id });
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("RenderNrReport", new { id }), options, 
                "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf");
        }

        [AllowAnonymous]
        public ActionResult RenderNrReport(int id)
        {
            return View(BuildNrRenderReportModel(id));
        }

        public ActionResult NtReportList()
        {
            return View();
        }

        public ActionResult KpiReport()
        {
            return View();
        }

        private NRReportRenderModel BuildNrRenderReportModel(int id)
        {
            var nrHeader =
                unitOfWork.NrHeaderRepository.Get(h => h.id == id, includeProperties: "Lines,Images").FirstOrDefault();
            bool? isFromOldRecord;
            if (nrHeader != null && (nrHeader.insp_id != null || nrHeader.insp_v2_id != null))
            {
                isFromOldRecord = nrHeader.insp_id != null;
                var nrApiController = new NRApiController(unitOfWork, companyDal, inspectionsDal, inspectionsV2Dal, inspectionLinesTestedDal, 
                    productService, accountService);
                var insp = nrApiController.GetInspection((nrHeader.insp_v2_id ?? nrHeader.insp_id).Value, ref isFromOldRecord);

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
                return new NRReportRenderModel
                {
                    IsFromOldRecord = isFromOldRecord,
                    Inspection = insp,
                    NrHeader = nrHeader,
                    Orders = orders,
                    ImageTypes = unitOfWork.NrImageTypeRepository.Get(t => imageTypes.Contains(t.id)).ToList(),
                    ImagesRootUrl = Properties.Settings.Default.NrImagesRootUrl,
                    Title = nrHeader.nr_type_id == Nr_header.TypeNS ? App_GlobalResources.Resources.NsReportTitle : App_GlobalResources.Resources.NtReportTitle
                };

            }
            return null;
        }

        private static void ProcessNrLines(Nr_header nrHeader, Inspection_v2 insp)
        {
            if (nrHeader.Lines != null)
            {
                foreach (var line in nrHeader.Lines)
                {
                    if (insp != null && insp.Lines != null)
                    {
                        line.InspectionV2Line =
                            insp.Lines.FirstOrDefault(
                                l => l.id == (line.inspection_lines_v2_id ?? line.inspection_lines_tested_id));
                    }
                }
            }
        }

    }

    //[TestClass]
    //public class InspectionV2TestControllers
    //{
    //    [TestMethod]
    //    public void TestTrackingNumbersEmail()
    //    {
    //         var controler = new InspectionV2Controller();

    //        var result = controler.TrackingNumbersMail(69443) as JsonResult;

    //        string test = result.Data.ToString();

    //        Assert.AreEqual("OK", test);

    //    }
    //}
}
