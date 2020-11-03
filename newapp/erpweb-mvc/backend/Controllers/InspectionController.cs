using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ASPPDFLib;
using company.Common;
using erp.Model.Dal.New;

using backend.Models;
using erp.Model;
using backend.Properties;
using Ionic.Zip;
using System.Net.Mime;
using erp.DAL.EF.New;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data.Entity;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize(Roles = "Administrator, Inspector")]
    public class InspectionController : BaseController
    {
        
        private readonly ILoginHistoryDetailDAL loginHistoryDetailDAL;
        private readonly IAdminPagesDAL adminPagesDAL;
        private readonly IAdminPagesNewDAL adminPagesNewDAL;
        private readonly IClientPagesAllocatedDAL clientPagesAllocatedDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IInspectionsDAL inspectionsDAL;
        private readonly ILocationDAL locationDAL;
        private readonly IUserDAL userDAL;
        private readonly IInspectionsV2DAL inspectionsV2DAL;
        private readonly IAccountService accountService;
        private readonly IInspectionLinesTestedDal inspectionLinesTestedDal;
        private readonly IOrderLinesDAL orderLinesDAL;
        private readonly IAmendmentsDAL amendmentsDAL;
        private readonly ICustproductsDAL custproductsDAL;
        private readonly ICustProductDocTypeDal custProductDocTypeDal;
        private readonly IInspectionLinesAcceptedDal inspectionLinesAcceptedDal;
        private readonly IInspectionLinesRejectedDal inspectionLinesRejectedDal;
        private readonly IInspectionImagesDAL inspectionImagesDAL;
        private readonly IAqlDal aqlDal;
        private readonly IReturnCategoryDAL returnCategoryDAL;
        private readonly IMastProductsDal mastProductsDal;
        private readonly IProductTrackNumberFcDal productTrackNumberFc;
        private readonly IInspectionCriteriaDal inspectionCriteriaDal;

        public InspectionController(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, IAdminPagesDAL adminPagesDAL, 
            IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL, ICompanyDAL companyDAL, IInspectionsDAL inspectionsDAL,
            ILocationDAL locationDAL, IUserDAL userDAL, IInspectionsV2DAL inspectionsV2DAL, IAccountService accountService,
            IInspectionLinesTestedDal inspectionLinesTestedDal, IOrderLinesDAL orderLinesDAL, IAmendmentsDAL amendmentsDAL,
            ICustproductsDAL custproductsDAL, ICustProductDocTypeDal custProductDocTypeDal, IInspectionLinesAcceptedDal inspectionLinesAcceptedDal,
            IInspectionLinesRejectedDal inspectionLinesRejectedDal, IInspectionImagesDAL inspectionImagesDAL, IAqlDal aqlDal,
            IReturnCategoryDAL returnCategoryDAL, IMastProductsDal mastProductsDal, IProductTrackNumberFcDal productTrackNumberFc,
            IInspectionCriteriaDal inspectionCriteriaDal
            ) :
            base(unitOfWork,loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService)
        {
            
            this.loginHistoryDetailDAL = loginHistoryDetailDAL;
            this.adminPagesDAL = adminPagesDAL;
            this.adminPagesNewDAL = adminPagesNewDAL;
            this.clientPagesAllocatedDAL = clientPagesAllocatedDAL;
            this.companyDAL = companyDAL;
            this.inspectionsDAL = inspectionsDAL;
            this.locationDAL = locationDAL;
            this.userDAL = userDAL;
            this.inspectionsV2DAL = inspectionsV2DAL;
            this.accountService = accountService;
            this.inspectionLinesTestedDal = inspectionLinesTestedDal;
            this.orderLinesDAL = orderLinesDAL;
            this.amendmentsDAL = amendmentsDAL;
            this.custproductsDAL = custproductsDAL;
            this.custProductDocTypeDal = custProductDocTypeDal;
            this.inspectionLinesAcceptedDal = inspectionLinesAcceptedDal;
            this.inspectionLinesRejectedDal = inspectionLinesRejectedDal;
            this.inspectionImagesDAL = inspectionImagesDAL;
            this.aqlDal = aqlDal;
            this.returnCategoryDAL = returnCategoryDAL;
            this.mastProductsDal = mastProductsDal;
            this.productTrackNumberFc = productTrackNumberFc;
            this.inspectionCriteriaDal = inspectionCriteriaDal;
        }
        //
        // GET: /Inspection/

        public ActionResult Plan(DateTime? dateFrom, DateTime? dateTo, int? location_id )
        {
            ViewBag.breadcrumbs = new List<BreadCrumb> { new BreadCrumb { Text = "Inspection plan" } };
            if (dateFrom == null)
            {
                dateFrom = WebUtilities.GetFirstDayOfWeek(DateTime.Now.Date);
                dateTo = dateFrom.Value.AddDays(6);
            }
            if (location_id == null)
            {
                location_id = (accountService.GetCurrentUser()?.admin_type == 5 ? accountService.GetCurrentUser()?.consolidated_port : 1);
            }


            return View(BuildListModel(dateFrom,dateTo, location_id));
        }

        private InspectionsPlanListModel BuildListModel(DateTime? dateFrom, DateTime? dateTo, int? location_id)
        {
            var excludedInspectors =
                Properties.Settings.Default.Inspectors_Exclusions.Split(',').Select(s => int.Parse(s)).ToList();
            var model = new InspectionsPlanListModel
            {
                DateFrom = dateFrom.Value,
                DateTo = dateTo.Value,
                location_id = location_id.Value,
                Inspections = inspectionsDAL.GetByCriteria(dateFrom.Value, dateTo.Value.AddDays(1).AddSeconds(-1), location_id.Value).ToList(),
                Locations = locationDAL.GetAll().Where(l=>l.show_on_plan == true).ToList(),// WebUtilities.GetLocations(),
                Inspectors =
                    userDAL.GetInspectors(location_id.Value)
                            .Where(insp => !excludedInspectors.Contains(insp.userid))
                            .ToList(),
                Inspectors_Seeunallocated = Properties.Settings.Default.Inspectors_SeeUnallocated.Split(',').Select(s => int.Parse(s)).ToList(),
                ForPdf = false
            };
            var inspectionsV2 = GetV2ByCriteria(dateFrom.Value, dateTo.Value.AddDays(1).AddSeconds(-1), location_id.Value);
            List<Inspections> newInspections = new List<Inspections>();
            if (inspectionsV2 != null)
            {
                newInspections = inspectionsV2.Select(GetFromV2Record).ToList();
                foreach (var ins in newInspections)
                {
                    ins.insp_unique =
                        model.Inspections.FirstOrDefault(i => i.new_insp_id == ins.new_insp_id)?.insp_unique ?? 0;
                }
            }

            model.Inspections =
                    model.Inspections.Where(ins => ins.new_insp_id == null).Union(newInspections).ToList();

            return model;

        }

        private Inspections GetFromV2Record(Inspection_v2 i)
        {
            return new Inspections
            {
                new_insp_id = i.id,
                insp_unique = i.id,
                insp_start = i.startdate,
                insp_type = i.type == 1 ? "LO" : "FI",
                insp_days = i.duration,
                custpo = i.custpo,
                customer_code = i.Client?.customer_code,
                Factory = i.Factory,
                factory_code = i.Factory.factory_code,
                insp_comments = i.comments,
                qc_required = i.qc_required,
                Controllers =
                    i.Controllers.Select(
                        c => new Inspection_controller(c))
                        .ToList()
            };
        }

        public ActionResult List(DateTime? dateFrom, DateTime? dateTo, int? location_id)
        {
            var model = BuildListModel(dateFrom, dateTo, location_id);
            AddControllersFromLegacyFields(model.Inspections);
            return View(model);
        }

        private void AddControllersFromLegacyFields(List<Inspections> inspections)
        {
            PropertyInfo prop = null;
            foreach (var insp in inspections)
            {
                if (insp.Controllers.Count == 0)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        prop = insp.GetType().GetProperty(string.Format("insp_qc{0}", i));
                        var propVal = (int?)prop.GetValue(insp, null);
                        if (propVal != null && propVal != 0)
                        {
                            insp.Controllers.Add(new Inspection_controller
                            {
                                controller_id = propVal.Value,
                                startdate = insp.insp_start.Value,
                                duration =  insp.insp_days.Value
                            });
                        }
                    }

                }
            }
        }

        public PartialViewResult Edit(int id, int location_id, string returnTo, bool isV2 = false)
        {
            var excludedInspectors =
                Settings.Default.Inspectors_Exclusions.Split(',').Select(s => int.Parse(s)).ToList();
            var model = new InspectionPlanEditModel {Inspection = isV2 ? inspectionsV2DAL.GetById(id).IfNotNull(GetFromV2Record) : 
                inspectionsDAL.GetById(id), returnTo = returnTo, location_id = location_id};
            model.IsV2 = isV2;
            if (model.Inspection != null)
            {
                var factory = companyDAL.GetByFactoryCode(model.Inspection.factory_code);
                int? qc_location_id = location_id;
                if (factory != null && factory.consolidated_port_mix == 1)
                    qc_location_id = null;
                model.Inspectors =
                    userDAL.GetInspectors(qc_location_id).Where(insp => !excludedInspectors.Contains(insp.userid)).OrderBy(i => i.userwelcome).ToList();
                var newId = -1;
                if (model.Inspection.Controllers == null || model.Inspection.Controllers.Count == 0)
                {
                    if (model.Inspection.Controllers == null)
                        model.Inspection.Controllers = new List<Inspection_controller>();
                    //old system, controllers are written in qc fields
                    Inspections ins = model.Inspection;

                    if (!WebUtilities.IsEmpty(ins.insp_qc1))
                        ins.Controllers.Add(new Inspection_controller
                            {
                                id = newId--,
                                controller_id = ins.insp_qc1.Value/*,
                            Controller = UserDAL.GetById(ins.insp_qc1.Value),*/

                            });
                    if (!WebUtilities.IsEmpty(ins.insp_qc2))
                        ins.Controllers.Add(new Inspection_controller
                        {
                            id = newId--,
                            controller_id = ins.insp_qc2.Value/*,
                        Controller = UserDAL.GetById(ins.insp_qc2.Value)*/
                        });
                    if (!WebUtilities.IsEmpty(ins.insp_qc3))
                        ins.Controllers.Add(new Inspection_controller
                        {
                            id = newId--,
                            controller_id = ins.insp_qc3.Value/*,
                        Controller = UserDAL.GetById(ins.insp_qc3.Value)*/
                        });
                    if (!WebUtilities.IsEmpty(ins.insp_qc4))
                        ins.Controllers.Add(new Inspection_controller
                        {
                            id = newId--,
                            controller_id = ins.insp_qc4.Value/*,
                        Controller = UserDAL.GetById(ins.insp_qc4.Value)*/
                        });
                    if (!WebUtilities.IsEmpty(ins.insp_qc5))
                        ins.Controllers.Add(new Inspection_controller
                        {
                            id = newId--,
                            controller_id = ins.insp_qc5.Value/*,
                        Controller = UserDAL.GetById(ins.insp_qc5.Value)*/
                        });
                    if (!WebUtilities.IsEmpty(ins.insp_qc6))
                        ins.Controllers.Add(new Inspection_controller
                        {
                            id = newId--,
                            controller_id = ins.insp_qc6.Value/*,
                        Controller = UserDAL.GetById(ins.insp_qc6.Value)*/
                        });

                    foreach (var c in ins.Controllers)
                    {
                        if (ins.insp_start != null)
                            c.startdate = ins.insp_start.Value;
                        if (ins.insp_days != null)
                            c.duration = ins.insp_days.Value;
                    }

                }
                ViewBag.nextId = newId;
                ViewBag.mode = "edit";
                if (model.Inspection.insp_type == "X")
                {
                    return PartialView("PlanEditAdmin", model);
                }
                else
                    return PartialView("PlanEdit", model);
            }
            ViewBag.message = "No id";
            return PartialView("Message");
        }


        [HttpPost]
        public ActionResult Edit(int id, InspectionPlanEditModel m)
        {
            var amendments = new List<Amendments>();
            List<Inspection_controller> deleted;
            if (m.IsV2)
            {
                var insp2 = inspectionsV2DAL.GetById(id);//unitOfWork.InspectionV2Repository.GetById(id);
                var insp = inspectionsDAL.GetByNewInspId(id);
                if (insp2.startdate != m.Inspection.insp_start) {
                    amendments.Add(new Amendments
                    {
                        process = "modify inspection date",
                        old_data = insp2.startdate.ToString("d"),
                        new_data = m.Inspection.insp_start.ToString("d"),
                        userid = accountService.GetCurrentUser().userwelcome ,
                        tablea = "Inspections",
                        timedate = DateTime.Now,
                        ref1 = id.ToString()
                    });
                }
                insp2.startdate = m.Inspection.insp_start;
                if (insp2.duration != m.Inspection.insp_days) {
                    amendments.Add(new Amendments
                    {
                        process = "modify inspection days",
                        old_data = insp2.duration.ToString(),
                        new_data = m.Inspection.insp_days.ToString(),
                        userid = accountService.GetCurrentUser().userwelcome,
                        tablea = "Inspections",
                        timedate = DateTime.Now,
                        ref1 = id.ToString()
                    });
                }

                insp2.duration = m.Inspection.insp_days;
                insp2.comments_admin = m.Inspection.insp_comments_admin;

                HandleControllers(null,insp2,out deleted, ref amendments);
                unitOfWork.InspectionV2Repository.UpdateLoading(insp2);
                unitOfWork.Save();
                if(insp != null) {
                    insp.insp_start = insp2.startdate;
                    insp.insp_days = insp2.duration;
                    SyncControllers(insp, insp2, out deleted);
                    //HandleControllers(insp, null, out deleted, ref amendments);
                    WriteToLegacyFields(insp, deleted);
                    inspectionsDAL.Update(insp, deleted);
                }

            }
            else
            {
                Inspections insp = inspectionsDAL.GetById(id);

                if (insp.insp_start != m.Inspection.insp_start) {
                    amendments.Add(new Amendments
                    {
                        process = "modify inspection date",
                        old_data = insp.insp_start.ToString("d"),
                        new_data = m.Inspection.insp_start.ToString("d"),
                        userid = accountService.GetCurrentUser().userid.ToString(),
                        tablea = "Inspections",
                        timedate = DateTime.Now,
                        ref1 = id.ToString()
                    });
                }
                insp.insp_start = m.Inspection.insp_start;
                if (insp.insp_days != m.Inspection.insp_days) {
                    amendments.Add(new Amendments
                    {
                        process = "modify inspection days",
                        old_data = insp.insp_days.ToString(),
                        new_data = m.Inspection.insp_days.ToString(),
                        userid = accountService.GetCurrentUser().userwelcome,
                        tablea = "Inspections",
                        timedate = DateTime.Now,
                        ref1 = id.ToString()
                    });
                }

                insp.insp_days = m.Inspection.insp_days;
                insp.insp_comments_admin = m.Inspection.insp_comments_admin;
                if (insp.insp_type == "X")
                    insp.insp_comments = m.Inspection.insp_comments;


                HandleControllers(insp,null, out deleted, ref amendments);
                inspectionsDAL.Update(insp, deleted);


            }
            foreach (var amendment in amendments) {
                amendmentsDAL.Create(amendment);
            }


            return Redirect(m.returnTo);
        }

        private void SyncControllers(Inspections insp, Inspection_v2 insp2, out List<Inspection_controller> deleted)
        {
            //copy controllers from v2 to old
            foreach(var c in insp2.Controllers) {
                var oldCont = insp.Controllers.FirstOrDefault(oldC => oldC.controller_id == c.controller_id);
                if(oldCont == null)
                    insp.Controllers.Add(new Inspection_controller { controller_id = c.controller_id,inspection_id = insp.insp_unique });
                else {
                    oldCont.startdate = c.startdate;
                    oldCont.duration = c.duration;
                }
            }
            deleted = insp.Controllers.Where(oldC => insp2.Controllers.Count(c => c.controller_id == oldC.controller_id) == 0).ToList();
            //foreach (var c in toBeRemoved)
            //    insp.Controllers.Remove(c);
        }

        private List<IInspectionController> CollectControllers(int id)
        {
            List<IInspectionController> collectedControllers = new List<IInspectionController>();
            foreach (var key in Request.Form.Keys) {
                string k = key.ToString();
                if (!k.StartsWith("insp_controller_id_")) continue;

                //workoround for getting "-1,-1,-1.." in request for insp_controller_id_-1
                var requestValueTest = Request[k];
                var testValues = requestValueTest.Split(',');
                var firstTestValue = testValues[0];
                var insp_controller_id = int.Parse(firstTestValue);

                //var insp_controller_id = int.Parse(Request[k]);
                if (insp_controller_id != 0) {

                    //check values
                    var ddlControlCheck = Request["ddlController_" + insp_controller_id.ToString()];

                    //empty value
                    if (string.IsNullOrEmpty(ddlControlCheck)) continue;

                    var controller_id = int.Parse(Request["ddlController_" + insp_controller_id.ToString()]);
                    var start = DateTime.Parse(Request["startDate_" + insp_controller_id.ToString()]);
                    var days = int.Parse(Request["txtDays_" + insp_controller_id.ToString()]);

                    IInspectionController cont = new Inspection_controller
                    {
                        controller_id = controller_id,
                        startdate = start,
                        duration = days,
                        inspection_id = id
                    };

                    if (insp_controller_id > 0)
                        cont.id = insp_controller_id;
                    collectedControllers.Add(cont);
                }
            }
            return collectedControllers;
        }

        private void HandleControllers(Inspections inspection,Inspection_v2 insp2, out List<Inspection_controller> deletedControllers, ref List<Amendments> amendments)
        {
            deletedControllers = new List<Inspection_controller>();
            var collectedControllers = CollectControllers(inspection != null ? inspection.insp_unique : insp2.id);
            if (inspection != null && inspection.Controllers == null)
                inspection.Controllers = new List<Inspection_controller>();
            //Existing lines
            foreach (var cont in collectedControllers.Where(ic => ic.id > 0))
            {
                var found =  inspection != null
                    ? (IInspectionController) inspection.Controllers.FirstOrDefault(ic => ic.id == cont.id)
                    : insp2.Controllers.FirstOrDefault(ic => ic.id == cont.id);
                if (found != null)
                {
                    if (found.controller_id != cont.controller_id)
                    {
                        amendments.Add(new Amendments
                        {
                            process = "modify QC inspector",
                            old_data = ResolveUser(found.controller_id),
                            new_data = ResolveUser(cont.controller_id),
                            userid = accountService.GetCurrentUser().userwelcome,
                            tablea = "Inspections",
                            timedate = DateTime.Now,
                            ref1 = inspection != null ? inspection.insp_unique.ToString() : insp2.id.ToString()
                        });


                    }
                    found.controller_id = cont.controller_id;
                    if (found.startdate != cont.startdate)
                    {
                        amendments.Add(new Amendments
                        {
                            process = "modify QC start date",
                            old_data = found.startdate.ToString("d"),
                            new_data = cont.startdate.ToString("d"),
                            userid = accountService.GetCurrentUser().userwelcome,
                            tablea = "Inspections",
                            timedate = DateTime.Now,
                            ref1 = inspection != null ? inspection.insp_unique.ToString() : insp2.id.ToString()
                        });
                    }
                    found.startdate = cont.startdate;
                    if (found.duration != cont.duration)
                    {
                        amendments.Add(new Amendments
                        {
                            process = "modify QC days",
                            old_data = found.duration.ToString(),
                            new_data = cont.duration.ToString(),
                            userid = accountService.GetCurrentUser().userwelcome,
                            tablea = "Inspections",
                            timedate = DateTime.Now,
                            ref1 = inspection != null ? inspection.insp_unique.ToString() : insp2.id.ToString()
                        });
                    }
                    found.duration = cont.duration;
                }
            }

            var addedControllers = collectedControllers.Where(cl => cl.id <= 0).ToList();
            foreach (var cont in addedControllers)
            {
                amendments.Add(new Amendments
                {
                    process = "add QC inspector",
                    old_data = "none",
                    new_data = ResolveUser(cont.controller_id),
                    userid = accountService.GetCurrentUser().userwelcome,
                    tablea = "Inspections",
                    timedate = DateTime.Now,
                    ref1 = inspection != null ? inspection.insp_unique.ToString() : insp2.id.ToString()
                });
            }

            deletedControllers.AddRange(inspection != null ? inspection.Controllers.Where(l => collectedControllers.Count(cl => cl.id == l.id) == 0) :
                                insp2.Controllers.Where(l => collectedControllers.Count(cl => cl.id == l.id) == 0).Select(c=>new Inspection_controller(c)));
            foreach (var cont in deletedControllers) {
                amendments.Add(new Amendments
                {
                    process = "delete QC inspector",
                    new_data = "none",
                    old_data = ResolveUser(cont.controller_id),
                    userid = accountService.GetCurrentUser().userwelcome,
                    tablea = "Inspections",
                    timedate = DateTime.Now,
                    ref1 = inspection != null ? inspection.insp_unique.ToString() : insp2.id.ToString()
                });
            }


            if (inspection != null) {

                inspection.Controllers.AddRange(addedControllers.Select(c => new Inspection_controller(c)));
                inspection.Controllers.AddRange(collectedControllers.Where(cc => inspection.Controllers.All(c => c.id != cc.id)).Select(c => new Inspection_controller(c)));

                //Copy because out param cannot be used in lambdas

                WriteToLegacyFields(inspection, new List<Inspection_controller>(deletedControllers));
            }
            else
            {
                insp2.Controllers.AddRange(addedControllers.Select(c=>new Inspection_v2_controller(c)));
                foreach (var c in deletedControllers.Select(dc=>insp2.Controllers.FirstOrDefault(co=>co.id == dc.id)))
                {
                    insp2.Controllers.Remove(c);
                }
            }


        }

        private static void WriteToLegacyFields(Inspections inspection, List<Inspection_controller> deletedControllers)
        {
            //Write to legacy fields
            int counter = 1;
            //not deleted
            foreach (
                var controller_id in
                    inspection.Controllers.Where(
                        c => deletedControllers.Count(dc => dc.controller_id == c.controller_id) == 0)
                        .Select(c => c.controller_id)
                        .Distinct()) {
                if (counter <= 6) {
                    var prop = inspection.GetType().GetProperty(string.Format("insp_qc{0}", counter));
                    prop.SetValue(inspection, controller_id, null);
                }

                counter++;
            }
            //deleted - put null to legacy field if no records for this controller
            foreach (
                var controller_id in
                    deletedControllers.Where(
                        dc =>
                            inspection.Controllers.Count(
                                ic => ic.controller_id == dc.controller_id && ic.id != dc.id) == 0)
                        .Select(dc => dc.controller_id)) {
                for (int i = 1; i <= 6; i++) {
                    var prop = inspection.GetType().GetProperty(string.Format("insp_qc{0}", i));
                    var propVal = (int?)prop.GetValue(inspection, null);
                    if (propVal == controller_id)
                        prop.SetValue(inspection, null, null);
                }
            }
        }

        private string ResolveUser(int controllerId)
        {
            var result = string.Empty;
            if (controllerId > 0)
            {
                var user = userDAL.GetById(controllerId);
                if (user != null)
                    result = user.userwelcome;
            }
            return result;
        }

        public PartialViewResult Create(string insp_type, int controller_id, DateTime date, int location_id, string returnTo)
        {
            var model = new InspectionPlanEditModel
            {
                location_id = location_id,
                returnTo = returnTo,
                Inspection = new Inspections { insp_type = insp_type, insp_qc1 = controller_id, insp_start = date }
            };
            ViewBag.mode = "new";
            if (insp_type == "X")
            {
                model.Inspection.insp_id = App_GlobalResources.Resources.Inspection_Admin_Id;
                return PartialView("PlanEditAdmin", model);
            }
            else
            {
                return PartialView("PlanEdit", model);
            }
        }

        [HttpPost]
        public ActionResult Create(InspectionPlanEditModel m)
        {
            m.Inspection.insp_days = 1;
            inspectionsDAL.Create(m.Inspection);
            return Redirect(m.returnTo);
        }

        public ActionResult Pdf(DateTime? dateFrom, DateTime? dateTo, int? location_id)
        {

            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("RenderForPdf", new { dateFrom, dateTo, location_id }), "scale=0.78, leftmargin=22,rightmargin=22,media=1",
                "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.SaveToMemory();
            return File((byte[]) s, "application/pdf");
        }

        [AllowAnonymous]
        public ActionResult RenderForPdf(DateTime? dateFrom, DateTime? dateTo, int? location_id)
        {
            var model = BuildListModel(dateFrom, dateTo, location_id);
            model.ForPdf = true;
            AddControllersFromLegacyFields(model.Inspections);
            return View("Plan", model);
        }

        public ActionResult Export(DateTime dateFrom, DateTime dateTo, int location_id,bool screen = false )
        {
            var model = BuildListModel(dateFrom, dateTo, location_id);
            model.ForExport = true;
            AddControllersFromLegacyFields(model.Inspections);
            if (!screen)
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=export1.xls");
                Response.ContentType = "application/vnd.ms-excel";
            }
            return View("Export", model);
        }

        public ActionResult ExportForFactories()
        {
            var model = new InspectionExportModel {Factories = companyDAL.GetCompaniesForUser(accountService.GetCurrentUser().userid), From = DateTime.Today, To=DateTime.Today.AddDays(7)};
            return View("ExportCriteria", model);
        }

        [HttpPost]
        public ActionResult ExportForFactories(InspectionExportModel m)
        {
            var factories = string.Empty + Request["SelectedFactories"];
            if (!string.IsNullOrEmpty(factories))
            {
                var user = accountService.GetCurrentUser();
                //18.5.2016 - factory restriction removed
                m.Inspections = inspectionsDAL.GetForExport(m.From, m.To, factories.Split(','),
                                                             /*user.admin_type.In(1, 2, 4) ? (int?) user.userid : */null);
                var custpos = new List<string>();
                m.InspectionChangedProducts = new Dictionary<string, int>();
                foreach (var insp in m.Inspections)
                {
                    custpos.AddRange(insp.custpo.Split(',').Where(c => !string.IsNullOrEmpty(c) && !custpos.Contains(c)));
                    var key = string.Join(",", insp.CustPos) + "," + insp.factory_code;
                    if (!m.InspectionChangedProducts.ContainsKey(key))
                    {
                        m.InspectionChangedProducts[key] = inspectionsDAL.GetChangedProductCount(insp.CustPos,
                                                                                                 insp.factory_code);
                    }
                }
                m.Lines = orderLinesDAL.GetByCustPos(custpos.ToArray());
                m.ProductPreviousShipments = new Dictionary<int, int>();
                foreach (var line in m.Lines)
                {
                    if (!m.ProductPreviousShipments.ContainsKey(line.cprod_id.Value))
                    {
                        m.ProductPreviousShipments[line.cprod_id.Value] =
                            orderLinesDAL.GetNumOfPreviousShipments(DateTime.Today.AddDays(-1), line.cprod_id.Value);
                    }
                }
                m.LoadingInspections =
                    inspectionsDAL.GetLoadingInspections(
                        m.Inspections.Select(i => i.factory_code).Distinct().ToArray(),
                        m.Inspections.Select(i => i.customer_code).Distinct().ToArray(),
                        m.Inspections.Select(i => i.custpo).Distinct().ToArray());


                Response.AddHeader("Content-Disposition", "attachment;filename=inspection_export.xls");
                Response.ContentType = "application/vnd.ms-excel";



                return View(m);
            }
            else
            {
                ViewBag.message = "No factory selected";
                return View("Message");
            }

        }

        [HttpPost]
        public ActionResult Delete(int id, InspectionPlanEditModel m)
        {
            inspectionsDAL.Delete(id);
            return Redirect(m.returnTo);
        }

        public ActionResult ReportPDF(string id, string imagesFolder="")
        {
	        var iId = inspectionsDAL.GetIdFromIdString(id);
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            //var font = doc.Fonts["SimHei"];
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("ReportRender", new { id = iId,statsKey=Settings.Default.StatsKey,imagesFolder}), "CJK=true;scale=0.78, topmargin=10,bottommargin=10,leftmargin=12,rightmargin=12,media=1", "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf");
        }

        [AllowAnonymous]
        public ActionResult ReportRender(string id, string statsKey, string imagesFolder="")
        {
	        var iId = inspectionsDAL.GetIdFromIdString(id);
            imagesFolder = AdjustImagesFolder(iId, imagesFolder);
            if (statsKey == Properties.Settings.Default.StatsKey)
            {
                return View("Report", BuildInspectionReportModel(iId,imagesFolder));
            }
            return null;
        }

        public ActionResult Report(string id, string imagesFolder = "")
        {
	        var iId = inspectionsDAL.GetIdFromIdString(id);
            imagesFolder = AdjustImagesFolder(iId, imagesFolder);
            var model = BuildInspectionReportModel(iId,imagesFolder);

            return View(model);
        }

        

        private string AdjustImagesFolder(int id,string imagesFolder)
        {
            var insp = inspectionsDAL.GetById(id);
            if (insp.insp_start.Year() < 2013 && string.IsNullOrEmpty(imagesFolder))
                return Settings.Default.InspectionImagesPre2013Folder;
            if (insp.insp_start.Year() == 2013 && string.IsNullOrEmpty(imagesFolder))
                return Settings.Default.InspectionImages2013Folder;
            if (insp.insp_start.Year() == 2014 && string.IsNullOrEmpty(imagesFolder))
                return Settings.Default.InspectionImages2014Folder;
            if (insp.insp_start.Year() < 2016 && string.IsNullOrEmpty(imagesFolder))
                return $"qc_archive/{insp.insp_start.Year()}";
            if (insp.insp_start.Year() < 2017 && string.IsNullOrEmpty(imagesFolder))
                return $"qc_archive/{insp.insp_start.Year()}";
            return imagesFolder;
        }


        public ActionResult LoadingReportPDF(int id, string imagesFolder = "")
        {
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("LoadingReportRender", new { id = id, statsKey = Settings.Default.StatsKey,imagesFolder }), "scale=0.78, leftmargin=12,rightmargin=12,media=1,Timeout=300", "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf");
        }

        public ActionResult BulkDownload(int insp_id=0)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            ViewBag.user = ticket.Name;
            ViewBag.id = userDAL.GetByLogin(ticket.Name).userid;
            var model = new InspectionBulkModel
            {
                InspectionLines=inspectionLinesTestedDal.GetInspLines(insp_id),
                Insepctions=inspectionLinesTestedDal.GetByInspId(insp_id),
                DocTypes=custProductDocTypeDal.GetAll().Where(d=>d.id != Settings.Default.InspectionDownload_exludedDocType).ToList()
            };
            var contZero = model.InspectionLines.Any(c=>c.order_linenum == 0);
            if (contZero)
            {
                var _ins = inspectionLinesTestedDal.GetByFactoryRef(insp_id).GroupBy(p=>p.insp_client_ref);
                foreach (var item in _ins)
                {
                    model.Insepctions.Add(item.First());
                }
            }
            
            return View(model);
        }

        public ActionResult FilterBulkDownload(string factory_code = "JD", string customer_code = "BS")
        {
            var model = new FilteredBulkDownload
            {
               // Factories = CompanyDAL.GetFactories(),
                Factories=custproductsDAL.GetFactoriesForClientsDetails(),
                DocTypes = custProductDocTypeDal.GetAll().Where(d => d.id != Settings.Default.InspectionDownload_exludedDocType).ToList()//,
                //Insepctions = Inspection_lines_testedDAL.GetByInspId(factory_code: factory_code, customer_code: customer_code)
            };


            return View("FilterBulkDownload", model);

        }

        public string FilteredDownload(int factory_id , int customer_id )
        {
            //var model = new FilteredBulkDownload
            //{
            //    Factories = CompanyDAL.GetFactories(),
            //    DocTypes = Cust_product_doctypeDAL.GetAll().Where(d => d.id != Settings.Default.InspectionDownload_exludedDocType).ToList(),
            //    Insepctions = Inspection_lines_testedDAL.GetByInspId(factory_code: factory_code, customer_code: customer_code)
            //};
            //var model = Inspection_lines_testedDAL.GetByInspId(factory_code: factory_code, customer_code: customer_code);
            var model = inspectionLinesTestedDal.GetByProducts( factory_id,  customer_id);

            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var set = JsonConvert.SerializeObject(model, Formatting.None, settings);
            return set;

        }

        public string GetDistributors(int factory_id)
        {
            var model = custproductsDAL.GetDistributorsByFactory(factory_id).Select(s => new {factory_code=s.factory_code,customer_code=s.customer_code, cprod_user=s.user_id});

            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var set = JsonConvert.SerializeObject(model, Formatting.None, settings);
            return set;
        }
        
        public ActionResult Download(string prodIds, string docTypeIds, int inspId, int factory_id=0, int customer_id=0)
        {
            var prodctIds = prodIds.Split(',').Select(int.Parse).ToList();
            var documentTypeIds = docTypeIds.Split(',').Select(int.Parse).ToList();
            //var products= Cust_
            var inspectionLines = inspectionLinesTestedDal.GetInspLines(inspId).Any(c => c.order_linenum == 0);
            var productsAll = inspectionLinesTestedDal.GetByInspId(inspId).ToList();
            if (inspectionLines)
            {
                var _ins = inspectionLinesTestedDal.GetByFactoryRef(inspId).GroupBy(p => p.insp_client_ref);
                foreach (var item in _ins)
                {
                    productsAll.Add(item.First());
                }
            }
            var factoryProductsAll = inspectionLinesTestedDal.GetByProducts(factory_id,customer_id);

            var listSelectedProducts = new List<Inspections_documents>();
            System.Collections.Generic.Dictionary<int, LogDataModel> dictSelectedProducts = new Dictionary<int, LogDataModel>();
            var factoryProductsDownload = false;
            if (inspId > 0)
            {
                foreach (var item in productsAll)
                {
                    foreach (var it in prodctIds)
                    {
                        if (item.mast_id == it)
                        {
                            listSelectedProducts.Add(item);
                            //dictSelectedProducts.Add
                        }
                    }
                }
            }
            else
            {
                factoryProductsDownload=true;
                foreach (var item in factoryProductsAll)
                {
                    foreach (var it in prodctIds)
                    {
                        if (item.mast_id == it)
                        {
                            listSelectedProducts.Add(item);
                        }
                    }
                }
            }
           // return new DownloadZipResult(products, documentTypeIds, "TechnicalData.zip", Settings.Default.DownloadGroupType);


            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            var user = ticket.Name;
            var userId = userDAL.GetByLogin(user).userid;


            return new DownloadZipInspection(listSelectedProducts,documentTypeIds,string.Format("{0}.zip",productsAll.First().inspection_id),Settings.Default.DownloadGroupType,userId,factoryProductsDownload);
        }
        



        private InspectionReportModel BuildInspectionReportModel(int id, string imagesFolder)
        {
            var model = new InspectionReportModel
                {
                    Inspection = inspectionsDAL.GetById(id),
                    InspectionImages = inspectionImagesDAL.GetByInspection(id),/*database inspection images*/
                    InspectionLinesAccepted = inspectionLinesAcceptedDal.GetByInspection(id),/*database inspection_lines_accepted */
                    InspectionLinesRejected = inspectionLinesRejectedDal.GetByInspection(id),
                    InspectionLinesTested = inspectionLinesTestedDal.GetByInspection(id, true).Where(l=>l.OrderLine == null || l.OrderLine.orderqty > 0).ToList(),
                    ImagesFolder = imagesFolder,
					Ranges = aqlDal.GetRanges(),
					RangeLevelSamples = aqlDal.GetRangeLevelSamples(),
					Category1ReturncategoryLevels = aqlDal.GetCategory1ReturnCategoryLevels(),
					ReturnCategories = returnCategoryDAL.GetAll().Where(c=> new []{"A","P","M","D","F"}.Contains(c.category_code)).ToList()
                };
            if(model.Inspection != null)
            {
				model.Client = companyDAL.GetByCustomerCode(model.Inspection.customer_code);
            }
            if (model.Inspection != null)
            {
                var factoryClientSettingsClients = company.Common.Utilities.GetIdsFromString(Settings.Default.InspectionReport_ClientsForSupplierCodeReplacement);
                model.Factory = companyDAL.GetByFactoryCode(model.Inspection.factory_code);
                if (model.Client != null && factoryClientSettingsClients.Contains(model.Client.user_id)) {
                    var factoryClientSettings =
                        unitOfWork.FactoryClientSettingsRepository.Get(s => s.custid ==  model.Client.user_id).ToList();
                    if (factoryClientSettings != null && model.Factory != null) {
                        var setting = factoryClientSettings.FirstOrDefault(fcs => fcs.factoryid == model.Factory.user_id);
                        if (setting != null) {
                            var factoryCode = model.Inspection.factory_code;
                            model.Inspection.factory_code = setting.supplier_code;
                            model.Inspection.insp_id = model.Inspection.insp_id.Replace(factoryCode + "-",
                                setting.supplier_code + "-");
                        }
                    }
                }
            }
            model.Products = new List<Mast_products>();
            model.CustProducts = new List<Cust_products>();

            model.Inspectors=new List<User>();
            if(model.Inspection != null){
            if (model.Inspection.insp_qc1 != null && model.Inspection.insp_qc1 != 0) model.Inspectors.Add(userDAL.GetById((int)model.Inspection.insp_qc1));
            if (model.Inspection.insp_qc2 != null && model.Inspection.insp_qc2 != 0) model.Inspectors.Add(userDAL.GetById((int)model.Inspection.insp_qc2));
            if (model.Inspection.insp_qc3 != null && model.Inspection.insp_qc3 != 0) model.Inspectors.Add(userDAL.GetById((int)model.Inspection.insp_qc3));
            if (model.Inspection.insp_qc4 != null && model.Inspection.insp_qc4 != 0) model.Inspectors.Add(userDAL.GetById((int)model.Inspection.insp_qc4));
            if (model.Inspection.insp_qc5 != null && model.Inspection.insp_qc5 != 0) model.Inspectors.Add(userDAL.GetById((int)model.Inspection.insp_qc5));
            if (model.Inspection.insp_qc6 != null && model.Inspection.insp_qc6 != 0) model.Inspectors.Add(userDAL.GetById((int)model.Inspection.insp_qc6));
            }
            int j=0;
            foreach (var i in model.Inspectors)
            {
                model.InspectorName += j > 0 ? ", " : "";
                model.InspectorName += i.userwelcome;
                j++;
            }
            var ids = new SortedSet<int>();

	        //new logic for CA
	        var correctiveActions = unitOfWork.ReturnRepository
		        .Get(r => r.claim_type == Returns.ClaimType_CorrectiveAction && r.insp_id == id,
			        includeProperties: "Products").ToList();

            foreach (var line in model.InspectionLinesTested)
            {
                line.RejectedLines =
                    model.InspectionLinesRejected.Where(l => l.insp_line_id == line.insp_line_unique).ToList();
                for (int i = 0; i < line.RejectedLines.Count; i++)
                {
                    line.RejectedLines[i].LineTested = line;
                }

                line.AcceptedLines =
                    model.InspectionLinesAccepted.Where(l => l.insp_line_id == line.insp_line_unique).ToList();
                for (int i = 0; i < line.AcceptedLines.Count; i++)
                {
                    line.AcceptedLines[i].LineTested = line;
                }
                if (model.Inspection != null)
                {
                    var product = mastProductsDal.GetByRefAndCode(line.insp_factory_ref, model.Inspection.factory_code);

                    if (product != null && model.Products.Count(p=>p.mast_id == product.mast_id) == 0)
                        model.Products.Add(product);
                }
                if(line.insp_qty > 0)
                {
                    var custproducts = custproductsDAL.GetByCode1(line.insp_client_ref);
                    if (custproducts != null && custproducts.Count > 0 && model.CustProducts.Count(prod=>prod.cprod_id == custproducts[0].cprod_id) == 0)
                    {
                        model.CustProducts.Add(custproducts[0]);
                    }
                }
                

                //if (line.insp_id != null) model.Inspectors.Add(UserDAL.GetById((int)line.insp_qc));
                if (line.OrderLine != null)
                {
                    var orderid = line.OrderLine.orderid;
                    var prod = custproductsDAL.GetById(line.OrderLine.cprod_id ?? 0);
                    if (prod != null)
                    {

                        if(model.ProductTrackNumbers == null)
                            model.ProductTrackNumbers = new List<products_track_number_fc>();
                        var trackNumbers = productTrackNumberFc.GetByCriteria(orderid, prod.cprod_mast);
                        if(trackNumbers.Count > 0)
                            ids.Add(prod.cprod_id);
                        model.ProductTrackNumbers.AddRange(trackNumbers);
                    }
                }

	            line.CA = correctiveActions.FirstOrDefault(a =>
		            a.Products != null && a.Products.Any(p => p.cprod_id == line.OrderLine?.cprod_id));

            }
            if (model.ProductTrackNumbers != null && ids != null && ids.Count() > 0)
                model.ProductsForTracking =
                    custproductsDAL.GetForIds(ids.ToList());
            model.InspectionCriteria = new List<Inspection_criteria>();
            foreach (var cat1 in model.Products.GroupBy(p => p.category1))
            {
                model.InspectionCriteria.AddRange(inspectionCriteriaDal.GetForCategory1(cat1.Key.Value));
            }


            return model;
        }



       

        public List<Inspection_v2> GetV2ByCriteria(DateTime from, DateTime to, int? location_id)
        {
            
            return unitOfWork.InspectionV2Repository
                .Get(i => ((i.startdate >= from && i.startdate <= to)
                ||
                (DbFunctions.AddDays(i.startdate, i.duration.Value) >=
                from &&
                DbFunctions.AddDays(i.startdate, i.duration.Value) <= to)
                || i.Controllers.Any(c => c.startdate >= from && c.startdate <= to)
                || i.Controllers.Any(c =>
                (DbFunctions.AddDays(c.startdate, c.duration) >=
                from &&
                DbFunctions.AddDays(c.startdate, c.duration) <= to)
                ))
                && (i.Factory.consolidated_port2 == location_id || location_id == null || i.Factory.consolidated_port_mix == 1)
                && i.insp_status != InspectionV2Status.Cancelled, includeProperties: "Factory, Client, Controllers.Controller")
                .ToList();
            
        }
    }




    public class LogDataModel
    {
        public DateTime  Date{ get; set; }
        public int UserUserId { get; set; }
        public string FileName { get; set; }
        public int CprodId { get; set; }
        public int MastId { get; set; }


    }

    public class DownloadZipInspection : ActionResult
    {
        private List<Inspections_documents> SelectedProducts;
        private List<int> DocTypes;
        private string FileName;
        private int GroupType;
        private int User;
        private bool DownloadFromFactories;


        public DownloadZipInspection(List<Inspections_documents> listSelectedProducts, List<int> docTypes, string fileName, int groupType, int userId, bool factoryProductsDownload)
        {
            // TODO: Complete member initialization
            SelectedProducts = listSelectedProducts;
            DocTypes = docTypes;
            FileName = fileName;
            GroupType = groupType;
            User = userId;
            DownloadFromFactories = factoryProductsDownload;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var response = context.HttpContext.Response;
            response.ContentType = "application/gzip";

           List<string> files =new List<string>();
            /* log_mastid  log_file log_cprodid*/
            //System.Collections.Generic.Dictionary<int,string> LogDataFile=new Dictionary<int,string>();
            List<LogDataModel> logData=new List<LogDataModel>();

            using(var zip =new ZipFile())
            {
                IPdfManager pdf=new PdfManager();

                string entryPath=string.Format("{0}",FileName.Replace(".zip",""));
                string name = "";
                var entryPath2 = "";
                foreach (var prod in SelectedProducts)
	            {

                    if (DocTypes.Contains(Cust_product_doctype.BasicDrawing) &&
                        (!string.IsNullOrEmpty(prod.prod_image3)))
                    {
                        try
                        {
                            
                            var pdfFromUrl = string.Format("{0}/backend/factory_PR_4_tech_pdf1.asp?prod_id={1}", Settings.Default.aspsite_root, prod.mast_id);
                            var doc = pdf.CreateDocument();
                            doc.ImportFromUrl(pdfFromUrl,
                                              string.Format("scale=0.82, leftmargin=5,rightmargin=10, topmargin=10, media=1,landscape=true"));

                            entryPath2 = string.Format("\\{1}\\{0}_technical_basic_drawing.pdf",prod.insp_client_ref,DownloadFromFactories==true ?"drawings":FileName.Replace(".zip",""));

                            zip.AddEntry(entryPath2, (byte[])doc.SaveToMemory());
                            name = pdfFromUrl;//.Split('/').Last();
                            logData.Add(new LogDataModel { CprodId = prod.cprod_id, FileName = name.Replace("dwg", "pdf"), MastId = prod.mast_id });

                        }
                        catch (Exception)
                        {

                            System.Diagnostics.Debug.WriteLine("ERROR BASIC DRAWING");
                        }
                    }

                    if (DocTypes.Contains(Cust_product_doctype.DetailedDrawing)&&
                        (!string.IsNullOrEmpty(prod.prod_image3)))
                    {
                        try
                        {
                            
                            var pdfFromUrl = string.Format("{0}/backend/factory_PR_4_tech_pdf2.asp?prod_id={1}",Settings.Default.aspsite_root,prod.mast_id);
                            System.Diagnostics.Debug.WriteLine(pdfFromUrl);
                            var doc = pdf.CreateDocument();
                            doc.ImportFromUrl(pdfFromUrl,
                                              "scale=0.82, leftmargin=5,rightmargin=10, topmargin=10, media=1,landscape=true"
                                              );

                               entryPath2= string.Format("\\{1}\\{0}_technical_detailed_drawing.pdf",prod.insp_client_ref,DownloadFromFactories == true ? "drawings" : FileName.Replace(".zip",""));
                                zip.AddEntry(entryPath2, (byte[])doc.SaveToMemory());

                                name = pdfFromUrl;//.Split('/').Last();

                                logData.Add(new LogDataModel { CprodId = prod.cprod_id, FileName = name.Replace("dwg", "pdf"), MastId = prod.mast_id });
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR: "+ ex);

                            }
                    }
                    if(DocTypes.Contains(Cust_product_doctype.CAD) &&
                        (!string.IsNullOrEmpty(prod.prod_image3)))
                    {
                        if (DownloadFromFactories)
                        {
                            AddFileIfExists(prod.prod_image3,prod.cprod_dwg,zip,context.HttpContext,"drawings");
                        }
                        else
                        {
                            files.Add(context.HttpContext.Server.MapPath(prod.prod_image3));
                            name = prod.prod_image3;//.Split('/').Last();
                            logData.Add(new LogDataModel { CprodId = prod.cprod_id, FileName = name, MastId = prod.mast_id });
                        }
                    }

                    if (DocTypes.Contains(Cust_product_doctype.Instructions) &&
                        (!string.IsNullOrEmpty(prod.cprod_instructions)||!string.IsNullOrEmpty(prod.prod_instructions)))
                    {
                       // AddFileIfExists(prod.cprod_instructions, prod.prod_instructions, zip, context.HttpContext, GroupType == 1 ? "packaging" : prod.mast_id.ToString());
                        string imageWithPath = !string.IsNullOrEmpty(prod.cprod_instructions) ? prod.cprod_instructions : prod.prod_instructions;
                        if (DownloadFromFactories)
                        {
                            AddFileIfExists(prod.cprod_instructions, !string.IsNullOrEmpty(prod.cprod_instructions) ? prod.cprod_instructions : prod.prod_instructions, zip, context.HttpContext, GroupType == 1 ? "instructions" : "instructions");
                        }
                        else
                        {
                            files.Add(context.HttpContext.Server.MapPath(imageWithPath));
                        }
                        name = imageWithPath;//.Split('/').Last();
                        logData.Add(new LogDataModel { CprodId = prod.cprod_id, FileName = name, MastId = prod.mast_id });
                    }
                    if (DocTypes.Contains(Cust_product_doctype.Label) &&
                        ((!string.IsNullOrEmpty(prod.cprod_label) || !string.IsNullOrEmpty(prod.prod_image4))))
                    {
                        // AddFileIfExists(prod.cprod_instructions, prod.prod_instructions, zip, context.HttpContext, GroupType == 1 ? "packaging" : prod.mast_id.ToString());
                        var f = context.HttpContext.Server.MapPath(!string.IsNullOrEmpty(prod.cprod_label) ? prod.cprod_label :  prod.prod_image4);
                        if (DownloadFromFactories)
                        {
                           // AddFileIfExists(prod.prod_image4,prod.prod_image4, zip, context.HttpContext, "label");
                           
                            AddFileIfExists(!string.IsNullOrEmpty(prod.cprod_label)?prod.cprod_label: prod.prod_image4, "", zip, context.HttpContext, "labels");
                        }
                        else
                        {

                            if (System.IO.File.Exists(f))
                            {
                                files.Add(f);
                            }

                        }
                        name = prod.prod_image4;//.Split('/').Last();
                        logData.Add(new LogDataModel { CprodId = prod.cprod_id, FileName = name, MastId = prod.mast_id });
                    }
                    if (DocTypes.Contains(Cust_product_doctype.Packaging) && ((!string.IsNullOrEmpty(prod.cprod_packaging) || !string.IsNullOrEmpty(prod.prod_image5))))
                    {
                        var f = context.HttpContext.Server.MapPath(!string.IsNullOrEmpty(prod.cprod_packaging)?prod.cprod_packaging:prod.prod_image5);
                        if (DownloadFromFactories)
                        {
                            AddFileIfExists(!string.IsNullOrEmpty(prod.cprod_packaging) ? prod.cprod_packaging : prod.prod_image5, "", zip, context.HttpContext, "packaging");
                        }
                        else
                        {
                            if (System.IO.File.Exists(f))
                            {
                                files.Add(f);
                            }
                        }
                        name = prod.prod_image5;//.Split('/').Last();
                        logData.Add(new LogDataModel { CprodId = prod.cprod_id, FileName = name, MastId = prod.mast_id });
                    }
                    if (DocTypes.Contains(Cust_product_doctype.Photo))
                    {
                        var photo = context.HttpContext.Server.MapPath(prod.prod_image1);
                        if (!string.IsNullOrEmpty(prod.prod_image1))
                        {
                            if(System.IO.File.Exists(photo))
                            {
                                files.Add(photo);
                            }
                        }
                         //context.HttpContext.Server.MapPath(prod.prod_image1);
                        //if(File)
                    }
	            }

                CreateDownloadLog(logData);

                if(!DownloadFromFactories)
                    AddFilesExists(files,zip,context.HttpContext,entryPath);

                zip.Save(response.OutputStream);
                var cd = new ContentDisposition
                {
                    FileName = FileName,
                    Inline = false
                };
                response.AddHeader("Content-Disposition", cd.ToString());
            }

        }

        private void CreateDownloadLog(List<LogDataModel> logData)
        {


            var date = DateTime.Now;
            foreach(var log in logData)
            {
                log.UserUserId = User;
                log.Date = date;

            }
            var downloadLogs = new List<Download_logs>();
            foreach(var l in logData)
            {
                downloadLogs.Add(new Download_logs
                {
                    log_date = l.Date,
                    log_useruserid=l.UserUserId,
                    log_file=l.FileName,
                    log_crpodid=l.CprodId,
                    log_mastid=l.MastId
                });
            }
            //Download_logDAL.Create(downloadLogs);

        }

        private void AddFileIfExists(string file1, string file2, ZipFile zip, HttpContextBase httpContextBase, string folder="")
        {
            if (!string.IsNullOrEmpty(file1) ||!string.IsNullOrEmpty(file2))
            {
                try
                {
                    var filePath = httpContextBase.Server.MapPath(!string.IsNullOrEmpty(file1) ? file1 : file2);
                    if (System.IO.File.Exists(filePath))
                        zip.AddFile(filePath, folder);
                    //zip.AddFiles();
                }
                catch (Exception)
                {

                    //throw;
                }
            }
        }
        private void AddFilesExists(List<string> files,ZipFile zip,HttpContextBase httpContextBase,string folder="")
        {
            try
            {
                zip.AddFiles(files,folder);
            }
            catch (Exception)
            {


            }
        }
    }
}