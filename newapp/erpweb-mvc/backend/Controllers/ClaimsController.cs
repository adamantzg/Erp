using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using company.Common;
using erp.Model.Dal.New;
using backend.App_GlobalResources;
using backend.Models;
using erp.Model;
using System.Text;
using backend.Properties;
using System.Web.UI.DataVisualization.Charting;
using Elmah;
using Utilities = company.Common.Utilities;

//using System.Web.UI.DataVisualization.Charting;
using System.Diagnostics;
using ASPPDFLib;
using System.Web.Security;
using System.Web.SessionState;
using System.Threading;
using System.Threading.Tasks;
using erp.DAL.EF.New;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize(Roles = "Administrator, Inspector")]
    public class ClaimsController : BaseController
    {
        
        private readonly ILoginHistoryDetailDAL loginHistoryDetailDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IAdminPagesDAL adminPagesDAL;
        private readonly IAdminPagesNewDAL adminPagesNewDAL;
        private readonly IClientPagesAllocatedDAL clientPagesAllocatedDAL;
        private ICategory1DAL category1DAL;
        private readonly IOrderLinesDAL orderLinesDAL;
        private readonly ICustproductsDAL custproductsDAL;
        private readonly IOrderHeaderDAL orderHeaderDAL;
        private readonly IReturnsDAL returnsDAL;
        private readonly IReturnCategoryDAL returnCategoryDAL;
        private readonly IReturnResolutionDAL returnResolutionDAL;
        private readonly IEmailRecipientsDAL emailRecipientsDAL;
        private readonly IReturnsImportanceDAL returnsImportanceDAL;
        private readonly IFeedbackTypeDAL feedbackTypeDAL;
        private readonly IFeedbackCategoryDAL feedbackCategoryDAL;
        private readonly IAdminPermissionsDal adminPermissionsDal;
        private readonly IUserDAL userDAL;
        private readonly IOrderLineExportDal orderLineExportDal;
        private readonly IBrandsDAL brandsDAL;
        private readonly IBrandCategoriesDal brandCategoriesDal;
        private readonly IStandardResponseDAL standardResponseDAL;
        private readonly ISalesDataDal salesDataDal;
        private readonly IProductFaultsDAL productFaultsDAL;
        private readonly IInspectionsDAL inspectionsDAL;
        private readonly IReturnsImagesDAL returnsImagesDAL;
        private readonly IReturnsCommentsFilesDAL returnsCommentsFilesDAL;
        private readonly IReturnsCommentsDAL returnsCommentsDAL;
        private readonly IFeedbackSubscriptionsDAL feedbackSubscriptionsDAL;
        private readonly IRoleDAL roleDAL;
        private readonly IProductfaultReasonDescriptionDAL productfaultReasonDescriptionDAL;
        private readonly IProductInvestigationStatusDAL productInvestigationStatusDAL;
        private readonly IProductInvestigationsDAL productInvestigationsDAL;
        private readonly IAnalyticsDAL analyticsDAL;
        private readonly IClaimsInvestigationReportsDAL claimsInvestigationReportsDAL;
        private readonly IProductInvestigationImagesDAL productInvestigationImagesDAL;
        private readonly IClaimsInvestigationReportActionImageDAL claimsInvestigationReportActionImageDAL;
        private readonly IClaimsInvestigationReportsActionDAL claimsInvestigationReportsActionDAL;
        private readonly IAccountService accountService;
        private readonly IMailHelper mailHelper;

        public ClaimsController(IUnitOfWork unitOfWork, ILoginHistoryDetailDAL loginHistoryDetailDAL, ICompanyDAL companyDAL,
            IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL, IClientPagesAllocatedDAL clientPagesAllocatedDAL,
            ICategory1DAL category1DAL, IOrderLinesDAL orderLinesDAL, ICustproductsDAL custproductsDAL, IOrderHeaderDAL orderHeaderDAL,
            IReturnsDAL returnsDAL, IReturnCategoryDAL returnCategoryDAL, IReturnResolutionDAL returnResolutionDAL,
            IEmailRecipientsDAL emailRecipientsDAL, IReturnsImportanceDAL returnsImportanceDAL, IFeedbackTypeDAL feedbackTypeDAL,
            IFeedbackCategoryDAL feedbackCategoryDAL, IAdminPermissionsDal adminPermissionsDal, IUserDAL userDAL,
            IOrderLineExportDal orderLineExportDal, IBrandsDAL brandsDAL, IBrandCategoriesDal brandCategoriesDal,
            IStandardResponseDAL standardResponseDAL, ISalesDataDal salesDataDal, IProductFaultsDAL productFaultsDAL,
            IInspectionsDAL inspectionsDAL, IReturnsImagesDAL returnsImagesDAL, IReturnsCommentsFilesDAL returnsCommentsFilesDAL,
            IReturnsCommentsDAL returnsCommentsDAL, IFeedbackSubscriptionsDAL feedbackSubscriptionsDAL, IRoleDAL roleDAL,
            IProductfaultReasonDescriptionDAL productfaultReasonDescriptionDAL, IProductInvestigationStatusDAL productInvestigationStatusDAL,
            IProductInvestigationsDAL productInvestigationsDAL, IAnalyticsDAL analyticsDAL,
            IClaimsInvestigationReportsDAL claimsInvestigationReportsDAL, IProductInvestigationImagesDAL productInvestigationImagesDAL,
            IClaimsInvestigationReportActionImageDAL claimsInvestigationReportActionImageDAL,
            IClaimsInvestigationReportsActionDAL claimsInvestigationReportsActionDAL, IAccountService accountService,
            IMailHelper mailHelper)
            : base(unitOfWork, loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService
                 )
        {
            
            this.loginHistoryDetailDAL = loginHistoryDetailDAL;
            this.companyDAL = companyDAL;
            this.adminPagesDAL = adminPagesDAL;
            this.adminPagesNewDAL = adminPagesNewDAL;
            this.clientPagesAllocatedDAL = clientPagesAllocatedDAL;
            this.category1DAL = category1DAL;
            this.orderLinesDAL = orderLinesDAL;
            this.custproductsDAL = custproductsDAL;
            this.orderHeaderDAL = orderHeaderDAL;
            this.returnsDAL = returnsDAL;
            this.returnCategoryDAL = returnCategoryDAL;
            this.returnResolutionDAL = returnResolutionDAL;
            this.emailRecipientsDAL = emailRecipientsDAL;
            this.returnsImportanceDAL = returnsImportanceDAL;
            this.feedbackTypeDAL = feedbackTypeDAL;
            this.feedbackCategoryDAL = feedbackCategoryDAL;
            this.adminPermissionsDal = adminPermissionsDal;
            this.userDAL = userDAL;
            this.orderLineExportDal = orderLineExportDal;
            this.brandsDAL = brandsDAL;
            this.brandCategoriesDal = brandCategoriesDal;
            this.standardResponseDAL = standardResponseDAL;
            this.salesDataDal = salesDataDal;
            this.productFaultsDAL = productFaultsDAL;
            this.inspectionsDAL = inspectionsDAL;
            this.returnsImagesDAL = returnsImagesDAL;
            this.returnsCommentsFilesDAL = returnsCommentsFilesDAL;
            this.returnsCommentsDAL = returnsCommentsDAL;
            this.feedbackSubscriptionsDAL = feedbackSubscriptionsDAL;
            this.roleDAL = roleDAL;
            this.productfaultReasonDescriptionDAL = productfaultReasonDescriptionDAL;
            this.productInvestigationStatusDAL = productInvestigationStatusDAL;
            this.productInvestigationsDAL = productInvestigationsDAL;
            this.analyticsDAL = analyticsDAL;
            this.claimsInvestigationReportsDAL = claimsInvestigationReportsDAL;
            this.productInvestigationImagesDAL = productInvestigationImagesDAL;
            this.claimsInvestigationReportActionImageDAL = claimsInvestigationReportActionImageDAL;
            this.claimsInvestigationReportsActionDAL = claimsInvestigationReportsActionDAL;
            this.accountService = accountService;
            this.mailHelper = mailHelper;
        }
        //
        // GET: /Returns/

        public ActionResult Returns()
        {
            var model = new ReturnsModel { SaleMonth = DateTime.Today.AddMonths(-1), Return = new Returns() };
            WebUtilities.ClearTempFiles();
            return View(model);
        }

        public ActionResult GetProducts(string custpo, int? month, int? year, string prod_criteria)
        {
            if (!string.IsNullOrEmpty(custpo))
            {
                var lines = orderLinesDAL.GetByCustPo(custpo, accountService.GetCurrentUser().company_id);
                return Json(lines.Select(l => new { l.unitprice, l.Cust_Product.cprod_id, l.Cust_Product.cprod_code1, l.Cust_Product.cprod_name, consolidated_port = "" }).Distinct());
            }
            else
            {
                //search by month/text
                var lines = orderLinesDAL.GetByCriteria(month, year, prod_criteria, accountService.GetCurrentUser().company_id);
                return
                    Json(
                        lines.Select(
                            l =>
                            new
                            {
                                l.unitprice,
                                l.cprod_id,
                                l.Cust_Product.cprod_code1,
                                l.Cust_Product.cprod_name,
                                l.Cust_Product.consolidated_port
                            }));
            }
        }
        [HttpPost]
        public ActionResult EditReturnReferenceNo(Returns feedback)
        {
            var _return = unitOfWork.ReturnRepository.GetByID(feedback.returnsid);
            _return.return_no = feedback.return_no;
            //unitOfWork.ReturnRepository.Update(_return);
            // unitOfWork.ReturnRepository.Insert(_return);
            unitOfWork.Save();
            return Json("OK");
        }

        [HttpPost]
        public ActionResult Returns(ReturnsModel m)
        {
            WebUtilities.ClearTempFiles();
            if (m.Return.cprod_id != null)
            {
                m.Return = new Returns { cprod_id = m.Return.cprod_id };
                m.Return.Product = custproductsDAL.GetById(m.Return.cprod_id.Value);
                m.Return.Creator = accountService.GetCurrentUser();
                m.Return.request_date = DateTime.Today;
                m.Status = ReturnStatus.New;
                var date = m.dateKnown != null && m.dateKnown.Value ? m.SaleMonth : null;
                Company company = companyDAL.GetById(accountService.GetCurrentUser().company_id);
                string customer_code = string.Empty;
                if (!string.IsNullOrEmpty(m.custpo))
                {
                    Order_header header = orderHeaderDAL.GetByCustpo(m.custpo);
                    if (header != null)
                        m.Return.order_id = header.orderid;
                }
                if (company != null)
                {
                    customer_code = company.customer_code;
                    int numOfReturns = returnsDAL.GetNoOfReturns(company.user_id, DateTime.Today);
                    m.Return.return_no = string.Format("{0}-?-{1}-{2}", customer_code, DateTime.Today.ToString("yyMMdd"),
                                                       Utilities.OrdinalToLetters(numOfReturns + 1));
                }
                m.OrderMonthlyData = orderHeaderDAL.GetQtyByMonth(m.Return.Product.cprod_code1, accountService.GetCurrentUser().company_id,
                                                                   Utilities.GetMonthStart(
                                                                       DateTime.Today.AddMonths(-11)),
                                                                   Utilities.GetMonthEnd(DateTime.Today));
                m.ReturnsDataLastYear = returnsDAL.GetQtyInPeriod(m.Return.Product.cprod_code1,
                                                                  accountService.GetCurrentUser().company_id,
                                                                  DateTime.Today.AddYears(-1), DateTime.Today);
                m.ReturnsDataTotal = returnsDAL.GetQtyInPeriod(m.Return.Product.cprod_code1,
                                                               accountService.GetCurrentUser().company_id);
                m.TotalOrdered = orderHeaderDAL.GetTotalQty(m.Return.Product.cprod_code1,
                                                             accountService.GetCurrentUser().company_id);
                m.Lines =
                    orderLinesDAL.GetForProductAndCriteria(
                        date != null ? (int?)date.Value.Month : null,
                        date != null ? (int?)date.Value.Year : null,
                        m.Return.cprod_id.Value, accountService.GetCurrentUser().company_id);

                m.ReturnCategories = returnCategoryDAL.GetAll();
                m.ReturnResolutions = returnResolutionDAL.GetAll();
                ViewBag.mode = "new";
                return View("Edit", m);
            }
            else
            {
                ViewBag.message = "No product selected";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult Create(ReturnsModel m)
        {
            if (ModelState.IsValid)
            {
                ViewBag.mode = "read";
                m.Return.Images = GetFiles(m.Return.Images, null);
                PrepareAndCreateReturn(m.Return);

            }
            m.ReturnResolutions = returnResolutionDAL.GetAll();
            m.OrderMonthlyData = orderHeaderDAL.GetQtyByMonth(m.Return.Product.cprod_code1, accountService.GetCurrentUser().company_id,
                                                                   Utilities.GetMonthStart(
                                                                       DateTime.Today.AddMonths(-11)),
                                                                   Utilities.GetMonthEnd(DateTime.Today));
            m.ReturnsDataLastYear = returnsDAL.GetQtyInPeriod(m.Return.Product.cprod_code1,
                                                              accountService.GetCurrentUser().company_id,
                                                              DateTime.Today.AddYears(-1), DateTime.Today);
            m.ReturnsDataTotal = returnsDAL.GetQtyInPeriod(m.Return.Product.cprod_code1,
                                                           accountService.GetCurrentUser().company_id);
            m.TotalOrdered = orderHeaderDAL.GetTotalQty(m.Return.Product.cprod_code1,
                                                         accountService.GetCurrentUser().company_id);
            ViewBag.confirmMessage =
                "Your return has been submitted and the status will be updated within 5 working days";
            return View("Edit", m);
        }

        private void PrepareAndCreateReturn(Returns r, FeedbackStatus status = FeedbackStatus.Live, bool useEF = false, feedback_authorization authorization = null)
        {
            r.request_userid = accountService.GetCurrentUser().userid;
            r.request_user = accountService.GetCurrentUser().userwelcome;
            r.client_id = accountService.GetCurrentUser().company_id;
            r.request_date = DateTime.Now;
            r.Creator = accountService.GetCurrentUser();
            if (authorization == null)
                r.status1 = (int?)status;
            else
            {
                if (r.authorization_level != null)
                {
                    var authLevel = authorization.Levels.FirstOrDefault(l => l.id == r.authorization_level);
                    var maxLevel = authorization.Levels.Max(l => l.level);
                    if (authLevel != null)
                    {
                        if (authLevel.level == maxLevel && r.Creator.Groups.Count(g => g.id == authLevel.authorization_usergroupid) > 0)
                        {
                            r.status1 = (int?)FeedbackStatus.Live;
                        }
                    }
                }
            }
            r.return_no = r.return_no.Replace("?", r.reason);
            r.decision_final = 0;
            r.openclosed = 0;

            //var userId =r.ReturnsQCusers!= null ? r.ReturnsQCusers.First().useruser_id:0;
            var usersId = new List<int>();
            //if(r.ReturnsQCusers!=null && r.ReturnsQCusers.Count>0)
            //    usersId= r.ReturnsQCusers.Select(s=>s.userid).ToList();

            //r.ReturnsQCusers = null;

            if (r.cprod_id != null)
                r.Product = custproductsDAL.GetById(r.cprod_id.Value);
            if (r.Product != null)
            {
                r.spec_code1 = r.Product.cprod_code1;
                r.spec_name = r.Product.cprod_name;
            }
            if (r.order_id != null && r.order_id > 0)
            {
                var header = orderHeaderDAL.GetById(r.order_id.Value);
                if (header != null)
                    r.custpo = header.custpo;
            }
            if (useEF)
            {
                foreach (var s in r.Subscriptions)
                {
                    s.User = null;
                    s.Return = null;
                }
                r.Creator.Permissions = null;
                r.Subscriptions = r.Subscriptions.GroupBy(c => c.subs_useruserid).Select(gr => gr.First()).ToList();
                /*var qcUsers = unitOfWork.UserRepository.Get(u => usersId.Contains(u.userid)).ToList();
                r.ReturnsQCusers.Clear();
                r.ReturnsQCusers.AddRange(qcUsers);*/
                unitOfWork.ReturnRepository.Insert(r);
                /*unitOfWork.Save();
                if (usersId != null )
                {
                    var retQCusers = new List<Returns_qcusers>();
                    var returnsId = unitOfWork.ReturnRepository.Get().Last().returnsid;
                    foreach (var userId in usersId )
                    {
                        unitOfWork.ReturnsUserUserRepository.Insert(new Returns_qcusers { return_id = returnsId, useruser_id = userId });
                        unitOfWork.Save();
                    }
                }*/
                unitOfWork.Save();
            }
            else
            {
                returnsDAL.Create(r);
            }

        }

        [HttpPost]
        public ActionResult DeleteFeedback(int id)
        {
            unitOfWork.ReturnRepository.Delete(id);
            unitOfWork.Save();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeactivateFeedback(int id)
        {
            //unitOfWork.ReturnRepository.Delete(id);
            var claim = unitOfWork.ReturnRepository.GetByID(id);
            claim.status1 = (int)FeedbackStatus.Cancelled;
            unitOfWork.Save();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateProductFeedback(Returns r)
        {
            r.Images = GetFiles(r.Images, null);
            r.claim_type = erp.Model.Returns.ClaimType_Product;

            PrepareAndCreateReturn(r);

            Cust_products product;

            var fcUsers = CreateFeedbackSubscriptions(r, out product);

            SendEmails(r, fcUsers, product);

            return Json(r);
        }

        public static void RegisterEvent(Returns ret, ReturnEventType retType)
        {
            if (ret.Events != null)
            {
                var recheckEvent = ret.Events.Where(e => e.event_type == (int)retType).OrderByDescending(e => e.event_time).FirstOrDefault();
                if (recheckEvent != null && recheckEvent.event_time != null)
                {
                    if ((DateTime.Now - recheckEvent.event_time).Hours >= 1)
                    {
                        ret.Events.Add(
                            new returns_events
                            {
                                return_id = ret.returnsid,
                                event_time = DateTime.Now,
                                event_type = (int)retType,
                                mail_sent = false
                            }
                        );
                    }
                }
                else //for first event
                {
                    ret.Events.Add(
                        new returns_events
                        {
                            return_id = ret.returnsid,
                            event_time = DateTime.Now,
                            event_type = (int)retType,
                            mail_sent = false
                        }
                    );

                }
            }
        }


        public void SendEmails(Returns returns, List<User> recipients, Cust_products product)
        {
            var emailRecipientsDb = emailRecipientsDAL.GetByCriteria(accountService.GetCurrentUser()?.company_id ?? 0, "feedback", returns.claim_type.ToString(),
                returns.claim_type == Feedback_type.ItFeedback ? accountService.GetCurrentUser()?.Groups?.FirstOrDefault(g => g.returns_default == true)?.id : 0)?.FirstOrDefault();

            string to = string.Empty, cc = string.Empty, bcc = string.Empty;



            var subscriberEmails = (recipients != null && recipients.Count > 0) ? string.Join(",",
                                               recipients.Where(f => !string.IsNullOrEmpty(f.user_email))
                                                         .Select(f => f.user_email)) : string.Empty;
            if (emailRecipientsDb != null)
            {
                to = SubstituteMacros(emailRecipientsDb.to, subscriberEmails, string.Empty);
                cc = SubstituteMacros(emailRecipientsDb.cc, subscriberEmails, string.Empty);
                bcc = SubstituteMacros(emailRecipientsDb.bcc, subscriberEmails, string.Empty);
            }
            else
            {
                to = subscriberEmails;
            }

            var importances = returns.claim_type != null ? returnsImportanceDAL.GetForType(returns.claim_type.Value) : new List<Returns_importance>();

            var categorie = returns.feedback_category_id != null ? feedbackCategoryDAL.GetById(Convert.ToInt32(returns.feedback_category_id)) : null;

            var importance = importances.FirstOrDefault(i => i.importance_id == returns.importance_id);

            var resolution = GetResolutions().FirstOrDefault(r => r.Id == returns.resolution);

            var subject = string.Format(App_GlobalResources.Resources.Feedback_subject, returns.return_no,
                importance != null ? importance.importance_text : "", GetTypeText(returns.claim_type ?? 0));

            var factory = product != null && product.MastProduct != null && product.MastProduct.factory_id != null ? companyDAL.GetById(product.MastProduct.factory_id.Value) : null;

            var resBody = App_GlobalResources.Resources.ResourceManager.GetString(string.Format("Feedback_{0}_body", returns.claim_type));

            string body = string.Empty;

            var attachments = new List<Attachment>();

            if (resBody != null)
            {
                if (returns.claim_type == erp.Model.Returns.ClaimType_Product)
                {
                    body = string.Format(resBody,
                                         accountService.GetCurrentUser().Company.customer_code,
                                         factory != null ? factory.factory_code : "",
                                         product != null ? product.cprod_code1 : "",
                                         product != null ? product.cprod_name : "", returns.client_comments,
                                         returns.client_comments2, returns.client_comments3,
                                         resolution != null ? resolution.Title : "");

                }
                else if (returns.claim_type == erp.Model.Returns.ClaimType_ITFeedback)
                {
                    body = string.Format(resBody, returns.client_comments,
                                         returns.client_comments2, returns.Creator?.userwelcome, WebUtilities.GetSiteUrl(), returns.returnsid);
                }
                else if (returns.claim_type == erp.Model.Returns.ClaimType_CorrectiveAction)
                {
                    //aditional users from db config settings
                    var add_emails = Settings.Default.FeedbackCorrectiveActionsAdditionalEMails;
                    var add_emails_cc = Settings.Default.FeedbackCorrectiveActionsAdditionalEMailsCC;
                    var add_emails_bcc = Settings.Default.FeedbackCorrectiveActionsAdditionalEMailsBCC;

                    if (!string.IsNullOrEmpty(add_emails) && add_emails.Length > 0)
                        to = MailOrganiser(to, add_emails);

                    if (!string.IsNullOrEmpty(add_emails_cc) && add_emails_cc.Length > 0)
                        cc = MailOrganiser(cc, add_emails_cc);

                    if (!string.IsNullOrEmpty(add_emails_bcc) && add_emails_bcc.Length > 0)
                        bcc = MailOrganiser(bcc, add_emails_bcc);

                    var qcUserList = string.Empty;
                    var products = new StringBuilder();

                    if (returns.Product != null)
                    {
                        products.Append(returns.Product?.cprod_code1);
                        products.Append(" ");
                        products.Append(returns.Product?.cprod_name);
                    }

                    if (returns.Products != null && returns.Products.Count > 0)
                    {
                        if (products.Length > 0)
                            products.Append(", ");

                        var plist = string.Join(", ", returns.Products.Select(m => String.Format("{0} {1}", m.cprod_code1, m.cprod_name)));

                        products.Append(plist);
                    }

                    if (returns.ReturnsQCUsers != null && returns.ReturnsQCUsers.Count > 0)
                    {
                        {
                            var qcusers = returns.ReturnsQCUsers.Select(m => m.User.userid).ToList();
                            qcUserList = String.Join(", ", unitOfWork.UserRepository.Get(m => qcusers.Contains(m.userid) && !String.IsNullOrEmpty(m.user_email)).Select(m => m.userwelcome).ToList());
                        }
                    }

                    body = string.Format(resBody,
                                    returns.return_no,
                                    categorie != null ? categorie.name : "",
                                    products.ToString(),
                                    returns.custpo,
                                    returns.request_date.ToString("dd/MM/yyyy"),
                                    importance != null ? importance.importance_text : "",
                                    qcUserList,
                                    returns.client_comments,
                                    returns.client_comments2,
                                    returns.client_comments3,
                                    returns.rejection_date.ToString("dd/MM/yyyy"),
                                    returns.recheck_date.ToString("dd/MM/yyyy"),
                                    returns.recheck_required == 1 ? "YES" : "NO",
                                    returns.recheck_status != null ? (returns.recheck_status == 1 ? "YES" : "NO") : "N/A",
                                    returns.inspection_qty.ToString(),
                                    returns.sample_qty.ToString(),
                                    returns.rejection_qty.ToString(),
                                    WebUtilities.GetSiteUrl(),
                                    returns.returnsid
                                    );
                }
                else if (returns.claim_type == erp.Model.Returns.ClaimType_QualityAssurance)
                {
                    body = string.Format(resBody, returns.Category?.name, returns.client_comments2, returns.client_comments, string.Join(",", returns.Products?.Select(p => p.cprod_code1)), WebUtilities.GetSiteUrl(), returns.returnsid);
                }
            }

            if (returns.Images != null && returns.Images.Count > 0)
            {
                // Open image from file
                IPdfManager pdfManager = new PdfManager();
                var doc = pdfManager.CreateDocument();

                foreach (var i in returns.Images.Where(i => i.file_category == null))
                {
                    try
                    {
                        IPdfImage objImage = doc.OpenImage(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), i.return_image));

                        float fWidth = objImage.Width * 72.0f / objImage.ResolutionX;
                        float fHeight = objImage.Height * 72.0f / objImage.ResolutionY;
                        IPdfPage objPage = doc.Pages.Add(fWidth, fHeight, Missing.Value);
                        objPage.Canvas.DrawImage(objImage, "x=0, y=0");
                    }
                    //for trying to open pdf-s or something that is not jpeg
                    catch (System.Runtime.InteropServices.COMException exp)
                    {
                        int error = exp.ErrorCode;

                        if (error != -2146828179)
                            throw exp;
                    }
                }

                var pdfDocName = $"feedback-rejection-{returns.return_no}.pdf";
                var directory = "ca_attachments";

                Directory.CreateDirectory(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory));

                doc.Save(System.Web.Hosting.HostingEnvironment.MapPath(Path.Combine(Settings.Default.returns_fileroot, directory, pdfDocName)), true);

                doc.Close();

                doc = null;
                pdfManager = null;

                Attachment pdf = new Attachment(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory, pdfDocName));
                attachments.Add(pdf);

            }

            if (!string.IsNullOrEmpty(to.Trim()))
            {
                
                if (attachments.Count > 0)
                {
                    mailHelper.SendMail(Settings.Default.Feedback_From, to, subject, body, cc, bcc, attachments.ToArray());
                }
                else
                {
                    mailHelper.SendMail(Settings.Default.Feedback_From, to, subject, body, cc, bcc);
                }
            }

        }


        private static string MailOrganiser(string existing_emails, string new_emails)
        {
            if (!string.IsNullOrEmpty(new_emails))
            {
                var tmp_add_emails = new_emails.Split(',');

                foreach (var email in tmp_add_emails)
                {
                    if (!existing_emails.Contains(email.Trim()))
                        existing_emails = existing_emails.Length > 0 ? (existing_emails + " ," + email) : (existing_emails + email);
                }
            }

            return existing_emails;
        }

        public void SendRecheckUpdated(Returns returns)
        {
            Cust_products product;

            var subscribers = CreateFeedbackSubscriptions(returns, out product);

            var subscriberEmails = string.Join(",",
                                   subscribers.Where(f => !string.IsNullOrEmpty(f.user_email))
                                             .Select(f => f.user_email));

            string to = string.Empty, cc = string.Empty, bcc = string.Empty;

            to = subscriberEmails;

            var importances = returns.claim_type != null ? returnsImportanceDAL.GetForType(returns.claim_type.Value) : new List<Returns_importance>();

            var categorie = returns.feedback_category_id != null ? feedbackCategoryDAL.GetById(Convert.ToInt32(returns.feedback_category_id)) : null;

            var importance = importances.FirstOrDefault(i => i.importance_id == returns.importance_id);

            var resolution = GetResolutions().FirstOrDefault(r => r.Id == returns.resolution);

            var recheckStatus = "";
            if (returns.recheck_status != null)
                recheckStatus = returns.recheck_status == 1 ? "OK" : "NO";
            else
                recheckStatus = "N/A";

            var subject = string.Format(App_GlobalResources.Resources.Feedback_Subject_Rejection, returns.return_no,
                importance != null ? importance.importance_text : "", GetTypeText(returns.claim_type ?? 0), recheckStatus);

            //var factory = product != null && product.MastProduct != null && product.MastProduct.factory_id != null ? CompanyDAL.GetById(product.MastProduct.factory_id.Value) : null;
            var resBody = App_GlobalResources.Resources.ResourceManager.GetString(string.Format("Feedback_recheck_body"));

            string body = string.Empty;

            var attachments = new List<Attachment>();

            if (resBody != null)
            {
                //aditional users from db config settings
                var add_emails = Settings.Default.FeedbackCorrectiveActionsAdditionalEMails;
                var add_emails_cc = Settings.Default.FeedbackCorrectiveActionsAdditionalEMailsCC;
                var add_emails_bcc = Settings.Default.FeedbackCorrectiveActionsAdditionalEMailsBCC;

                if (!string.IsNullOrEmpty(add_emails) && add_emails.Length > 0)
                    to = MailOrganiser(to, add_emails);

                if (!string.IsNullOrEmpty(add_emails_cc) && add_emails_cc.Length > 0)
                    cc = MailOrganiser(cc, add_emails_cc);

                if (!string.IsNullOrEmpty(add_emails_bcc) && add_emails_bcc.Length > 0)
                    bcc = MailOrganiser(bcc, add_emails_bcc);


                var qcUserList = string.Empty;
                var products = new StringBuilder();

                if (returns.Product != null)
                {
                    products.Append(returns.Product?.cprod_code1);
                    products.Append(" ");
                    products.Append(returns.Product?.cprod_name);
                }

                if (returns.Products != null)
                {
                    if (products.Length > 0)
                        products.Append(", ");

                    var plist = string.Join(", ", returns.Products.Select(m => String.Format("{0} {1}", m.cprod_code1, m.cprod_name)));

                    products.Append(plist);
                }

                if (returns.Images != null && returns.Images.Count > 0 && (returns.Images.Where(i => i.file_category == 2 || i.file_category == 1).FirstOrDefault() != null))
                {
                    // Open image from file
                    IPdfManager pdfManager = new PdfManager();
                    var doc = pdfManager.CreateDocument();

                    foreach (var i in returns.Images.Where(i => i.file_category == 2 || i.file_category == 1))
                    {
                        try
                        {
                            IPdfImage objImage = doc.OpenImage(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), i.return_image));

                            float fWidth = objImage.Width * 72.0f / objImage.ResolutionX;
                            float fHeight = objImage.Height * 72.0f / objImage.ResolutionY;
                            IPdfPage objPage = doc.Pages.Add(fWidth, fHeight, Missing.Value);
                            objPage.Canvas.DrawImage(objImage, "x=0, y=0");
                        }
                        //for trying to open pdf-s or something that is not jpeg
                        catch (System.Runtime.InteropServices.COMException exp)
                        {
                            int error = exp.ErrorCode;

                            if (error != -2146828179)
                                throw exp;
                        }
                    }

                    var pdfDocName = $"feedback-rejection-{returns.return_no}.pdf";
                    var directory = "mobile_attachments";

                    Directory.CreateDirectory(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory));

                    doc.Save(System.Web.Hosting.HostingEnvironment.MapPath(Path.Combine(Settings.Default.returns_fileroot, directory, pdfDocName)), true);

                    doc.Close();

                    doc = null;
                    pdfManager = null;

                    Attachment pdf = new Attachment(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory, pdfDocName));
                    attachments.Add(pdf);
                }

                body = string.Format(resBody,
                                returns.return_no,
                                  categorie != null ? categorie.name : "",
                                products.ToString(),
                                returns.custpo,
                                returns.request_date.ToString("dd/MM/yyyy"),
                                importance != null ? importance.importance_text : "",
                                //qcUserList,
                                returns.client_comments,
                                returns.client_comments2,
                                returns.client_comments3,
                                returns.rejection_date.ToString("dd/MM/yyyy"),
                                returns.recheck_date.ToString("dd/MM/yyyy"),
                                returns.recheck_required == 1 ? "YES" : "NO",
                                returns.recheck_status != null ? (returns.recheck_status == 1 ? "YES" : "NO") : "N/A",
                                returns.inspection_qty.ToString(),
                                returns.sample_qty.ToString(),
                                returns.rejection_qty.ToString(),
                                WebUtilities.GetSiteUrl(),
                                returns.returnsid,
                                returns.request_user
                                );
            }
            if (!string.IsNullOrEmpty(to.Trim()))
            {
                
                if (attachments.Count > 0)
                {
                    mailHelper.SendMail(Settings.Default.Feedback_From, to, subject, body, cc, bcc, attachments.ToArray());
                }
                else
                {
                    mailHelper.SendMail(Settings.Default.Feedback_From, to, subject, body, cc, bcc);
                }
            }
        }

        private string GetTypeText(int claim_type)
        {
            return feedbackTypeDAL.GetById(claim_type)?.typename;
        }


        public ActionResult GetFCS(int cprod_id)
        {
            var fcs = new List<User>();

            Cust_products product = custproductsDAL.GetById(cprod_id);

            if (product != null && product.MastProduct != null && product.MastProduct.factory_id != null)
            {
                fcs = adminPermissionsDal.GetByCompany(product.MastProduct.factory_id.Value).Where(a => a.processing == 0).Select(f => f.User).ToList();
            }

            return Json(fcs);

        }

        private List<User> CreateFeedbackSubscriptions(Returns returns, out Cust_products product, 
            feedback_authorization authorization = null, IUnitOfWork unitOfWork = null)
        {
            var fcs = new List<User>();
            var result = new List<User>();

           
            product = null;

            if (returns.cprod_id != null)
            {
                product = custproductsDAL.GetById(returns.cprod_id.Value);
            }

            if (returns.claim_type != erp.Model.Returns.ClaimType_CorrectiveAction)
            {
                if (product != null && product.MastProduct != null && product.MastProduct.factory_id != null)
                {
                    fcs = adminPermissionsDal.GetByCompany(product.MastProduct.factory_id.Value).Where(a => a.feedbacks == 1).Select(f => f.User).ToList();
                }
            }

            // Add factory controllers if CA
            if (returns.claim_type == erp.Model.Returns.ClaimType_CorrectiveAction)
            {
                if (returns.Product == null)
                {
                    Cust_products caproduct;

                    if (returns.Products != null && returns.Products.Count > 0)
                    {
                        caproduct = returns.Products.FirstOrDefault();
                        caproduct = custproductsDAL.GetById(caproduct.cprod_id); // unitOfWork.CustProductRepository.Get(m => m.cprod_id == caproduct.cprod_id, includeProperties: "MastProduct").FirstOrDefault();
                        product = caproduct;
                    }
                }

                if (product != null && product.MastProduct != null && product.MastProduct.factory_id != null)
                {
                    var factory_id = product.MastProduct.factory_id.Value;

                    fcs = adminPermissionsDal.GetByCompany(factory_id).Where(a => a.processing == 0).Select(f => f.User).ToList();

                    var factory = unitOfWork.CompanyRepository.Get(m => m.user_id == factory_id).FirstOrDefault();

                    if (factory != null)
                        SubscribeSupervisor(factory.consolidated_port2, returns);

                    if (returns.Subscriptions != null)
                    {
                        foreach (var user in fcs)
                        {
                            var sub = new Feedback_subscriptions { subs_returnid = returns.returnsid, subs_useruserid = user.userid };
                            returns.Subscriptions.Add(sub);
                        }
                    }
                }
            }

            var settings = Settings.Default.Properties.Cast<System.Configuration.SettingsProperty>().FirstOrDefault(p => p.Name == string.Format("Feedback_subscribers_{0}", returns.claim_type));

            var subscribers = new List<int>();

            if (returns.claim_type != erp.Model.Returns.ClaimType_CorrectiveAction && settings != null)
            {
                subscribers = settings != null ? settings.DefaultValue.ToString().Split(',').Select(int.Parse).ToList() : new List<int>();
            }
                        

            subscribers.AddRange(fcs.Select(f => f.userid));

            if (returns.Subscriptions != null && returns.Subscriptions.Count > 0)
                subscribers.AddRange(returns.Subscriptions.Where(s => s.subs_useruserid != null).Select(s => s.subs_useruserid.Value));

            //adding creator to subscribers list
            if (returns.request_userid == null)
                returns.request_userid = accountService.GetCurrentUser().userid;

            if (returns.request_userid != null && accountService.GetCurrentUser() != null && returns.request_userid != accountService.GetCurrentUser().userid)
                subscribers.Add((Convert.ToInt32(returns.request_userid)));

            if (authorization != null)
            {
                var authLevel = authorization.Levels.FirstOrDefault(l => l.id == returns.authorization_level);

                if (authLevel != null && authLevel.authorization_usergroupid != null)
                {
                    
                    var group = unitOfWork.UserGroupRepository.Get(g => g.id == authLevel.authorization_usergroupid, includeProperties: "Users").FirstOrDefault();
                    if (group != null)
                        subscribers.AddRange(group.Users.Select(u => u.userid));
                }
            }

            foreach (var subscriber in subscribers)
            {
                var sub = new Feedback_subscriptions { subs_returnid = returns.returnsid, subs_useruserid = subscriber };

                if (returns.Subscriptions == null || returns.Subscriptions.Count(s => s.subs_useruserid == subscriber && s.subs_id > 0) <= 0)
                {
                    if (returns.Subscriptions == null)
                    {
                        returns.Subscriptions = new List<Feedback_subscriptions>();
                    }
                }



                var user = userDAL.GetById(subscriber);

                if (user != null && !string.IsNullOrEmpty(user.user_email))
                    result.Add(user);
            }

            //remove duplicates
            result = result.GroupBy(u => u.userid).Select(gr => gr.First()).ToList();

            return result;
        }

        //TODO: add code for more than one supervisor
        private static void SubscribeSupervisor(int? consolidatedPort, Returns ca)
        {
            if (consolidatedPort != null)
            {
                int user_id = -1;

                try
                {
                    switch (consolidatedPort)
                    {
                        case 1:
                            user_id = Settings.Default.QCSupervisorLocation1;
                            break;
                        case 2:
                            user_id = Settings.Default.QCSupervisorLocation2;
                            break;
                        case 3:
                            user_id = Settings.Default.QCSupervisorLocation3;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception) { }

                if (user_id != -1)
                {
                    var sub = new Feedback_subscriptions { subs_returnid = ca.returnsid, subs_useruserid = user_id };
                    ca.Subscriptions.Add(sub);
                }
            }
        }

        public ActionResult UploadImage()
        {
            var request = HttpContext.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return Json(new { success = true });
        }


        public HttpResponseMessage getTempUrl(string file_id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(file_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }


        public ActionResult Files(string name)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize), "text/html");
        }

        public ActionResult FilesReturns(string name)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize), "text/html");
        }

        public ActionResult ReceivePastedImage(string fileName, string image, bool comment = false)
        {
            //image contains header separated by ,
            var parts = image.Split(new[] { ',' }, 2);
            if (parts.Length > 1)
            {
                byte[] fileBytes = Convert.FromBase64String(parts[1]);
                WebUtilities.SaveTempFile(fileName, fileBytes, comment ? "returncomment_temp_" : "tempFile_");
            }
            return Json("OK");
        }

        public FileContentResult GetTempFile(string file_id)
        {
            var oFile = WebUtilities.GetTempFile(file_id);
            if (oFile != null)
                return File(oFile, WebUtilities.ExtensionToContentType(Path.GetExtension(file_id).Replace(".", "")));
            return null;
        }

        public ActionResult DeleteFile(string name)
        {
            //Dealer_imagesDAL.Delete(id);
            //string filePath = Path.Combine(Server.MapPath(Properties.Settings.Default.dealerImagesFolder),name);
            //if(System.IO.File.Exists(filePath))
            //    System.IO.File.Delete(filePath);
            //if(id<0)
            WebUtilities.DeleteTempFile(name);
            return Json("OK");
        }

        private List<Returns_images> GetFiles(List<Returns_images> images, List<int> deletedFilesIds)
        {
            List<string> written_files = new List<string>();
            if (images == null)
                images = new List<Returns_images>();

            if (deletedFilesIds != null && deletedFilesIds.Count > 0)
            {
                foreach (var item in deletedFilesIds)
                {
                    var image = images.FirstOrDefault(f => f.image_unique == item);
                    if (image != null)
                    {
                        images.Remove(image);
                        System.IO.File.Delete(Path.Combine(Server.MapPath(Settings.Default.returns_fileroot), image.return_image));
                    }
                }
            }

            try
            {
                var sessionFiles = WebUtilities.GetTempFiles();
                if (sessionFiles != null)
                {
                    foreach (KeyValuePair<string, byte[]> kv in sessionFiles)
                    {
                        //Write file
                        string filePath = Utilities.GetFilePath(kv.Key, Server.MapPath(Settings.Default.returns_fileroot));
                        var sw = new StreamWriter(filePath);
                        var ms = new MemoryStream(kv.Value);
                        ms.WriteTo(sw.BaseStream);
                        sw.Close();
                        images.Add(new Returns_images { return_image = Path.GetFileName(filePath) });
                        written_files.Add(filePath);
                    }
                }
            }
            catch (Exception)
            {
                //if some files written to folder and error occurred, delete them
                foreach (var item in written_files)
                {
                    System.IO.File.Delete(item);
                }
                throw;
            }

            return images;
        }

        [AllowAnonymous]
        public ActionResult Stats(DateTime? dateFrom, DateTime? dateTo, string statsKey)
        {
            if (statsKey == Properties.Settings.Default.StatsKey)
            {
                var endDate = (dateTo == null ? DateTime.Today : dateTo.Value);
                var startDate = (dateFrom == null ? endDate.AddYears(-1) : dateFrom.Value);
                var distributors = companyDAL.GetDistributors();
                var model = new ReturnsStatsModel
                {
                    Distributors = distributors,
                    EndDate = endDate,
                    StartDate = startDate,
                    Sales = orderLineExportDal.GetCustomerSummaryForPeriod(startDate, endDate),
                    ReturnsSummary =
                            returnsDAL.GetTotalsPerClient(startDate, endDate)
                                      .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                                      .ToList()
                };
                return View(model);
            }
            else
            {
                ViewBag.message = "Invalid stats key";
                return View("Message");
            }
        }

        [AllowAnonymous]
        public ActionResult PieChart(string param)
        {
            //values are in one param (separated by |) because asppdf can't stand & in url (transforms it to &amp; and fails to load image)
            var parts = param.Split('|').Select(s => string.IsNullOrEmpty(s) ? (double?)null : double.Parse(s)).ToList();
            var chart = new Chart { Width = 400, Height = 400 };
            chart.ChartAreas.Add(new ChartArea());

            var series = new Series();
            series.ChartType = SeriesChartType.Pie;
            //series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Black";
            var legend = new Legend();

            chart.Legends.Add(new Legend());
            chart.Series.Add(series);
            series.Points.Add(new DataPoint { LegendText = "Accepted", YValues = new double[] { parts[0].Value } });
            series.Points.Add(new DataPoint { LegendText = "Declined", YValues = new double[] { parts[1].Value } });
            if (parts.Count > 2)
                series.Points.Add(new DataPoint { LegendText = "Replacement parts", YValues = new double[] { parts[2].Value } });

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "image/jpg");
        }

        public ActionResult Analytics(int? brand_id = null)
        {
            var model = new ReturnAnalyticsModel { Brands = brandsDAL.GetAll(), brand_id = brand_id };
            if (brand_id != null)
                model.Categories = brandCategoriesDal.GetBrandCategories(model.Brands.FirstOrDefault(b => b.brand_id == brand_id).user_id.Value, filterByWebSeq: false).Select(c => new CheckBoxItem { Code = c.brand_cat_id, IsChecked = false, Label = c.brand_cat_desc }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Analytics(ReturnAnalyticsModel m)
        {
            m.Brands = brandsDAL.GetAll();
            if (m.brand_id != null)
                m.Categories = brandCategoriesDal.GetBrandCategories(m.Brands.FirstOrDefault(b => b.brand_id == m.brand_id).user_id.Value, filterByWebSeq: false).Select(c => new CheckBoxItem { Code = c.brand_cat_id, IsChecked = false, Label = c.brand_cat_desc }).ToList();
            return View(m);
        }

        [HttpPost]
        public ActionResult Products(string categories)
        {
            var catArray = categories.Split(',').Select(int.Parse).ToList();
            return Json(custproductsDAL.GetForCatIds(catArray));
        }

        [HttpPost]
        public ActionResult AnalyticsReport()
        {
            //collect products
            List<int> prod_ids = new List<int>();
            foreach (var key in Request.Form.Keys)
            {
                string k = key.ToString();
                if (!k.StartsWith("chk_")) continue;
                prod_ids.Add(int.Parse(k.Replace("chk_", "")));
            }
            var rows = returnsDAL.GetAnalytics(prod_ids);
            Response.AddHeader("Content-Disposition", "attachment;filename=ReturnsAnalytics.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return View(rows);
        }

        public ActionResult ProductFeedbackEdit(int? return_id)
        {
            WebUtilities.ClearTempFiles("returncomment_temp_");
            if (return_id == null)
                goto Error;
            var feedback = returnsDAL.GetById(return_id.Value);
            if (feedback != null)
            {
                if (feedback.cprod_id != null)
                    feedback.Product = custproductsDAL.GetById(feedback.cprod_id.Value);
                if (feedback.order_id != null && string.IsNullOrEmpty(feedback.custpo))
                {
                    var order = orderHeaderDAL.GetById(feedback.order_id.Value);
                    feedback.custpo = order != null ? order.custpo : "";
                }
                var user = accountService.GetCurrentUser();
                var model = new ProductFeedbackModel
                {
                    Feedback = feedback,
                    StandardResponses = standardResponseDAL.GetAll(),
                    CanEditExternalComments = true,
                    CanViewExternalComments = true,
                    CanEditInternalComments = user.admin_type.In(1, 2, 3, 4, 8),
                    CanViewInternalComments = user.admin_type.In(1, 2, 3, 4, 8),
                    Importances = returnsImportanceDAL.GetForType(erp.Model.Returns.ClaimType_Product),
                    TicketStatuses = new List<TicketStatus>(new[]{new TicketStatus{Id = 0,Text = "Open"},
                            new TicketStatus{Id=0.5,Text = "awaiting factory"}, new TicketStatus{Id=0.6,Text = "awaiting client"},new TicketStatus{Id=1,Text = "closed"} })
                };
                if (feedback.request_date != null && feedback.cprod_id != null)
                {
                    var from = Utilities.GetMonthStart(feedback.request_date.Value.AddMonths(-11));
                    var to = Utilities.GetMonthEnd(feedback.request_date.Value);
                    var startDate = feedback.request_date.Value;
                    model.OrderMonthlyData = orderHeaderDAL.GetQtyByMonth(feedback.Product.cprod_code1, feedback.Product.cprod_user ?? 0,
                                                                   Utilities.GetMonthStart(
                                                                       feedback.request_date.Value.AddMonths(-11)),
                                                                    Utilities.GetMonthEnd(feedback.request_date.Value));


                    model.SalesData = salesDataDal.GetForPeriod(feedback.Product.cprod_id,
                                                                 Utilities.GetMonth21FromDate(
                                                                     feedback.request_date.Value.AddMonths(-11)),
                                                                 Utilities.GetMonth21FromDate(
                                                                     feedback.request_date.Value));
                    model.Faults = productFaultsDAL.GetProductFaults(feedback.cprod_id.Value, from, to);

                    FillSubTotals(startDate, model.Faults, model.SalesData, model);


                    //CreatePGChart(model,model.Feedback.returnsid);
                    if (!string.IsNullOrEmpty(model.Feedback.custpo))
                    {
                        int orderid;
                        //custpo is in fact orderid
                        var ok = int.TryParse(model.Feedback.custpo, out orderid);
                        if (ok)
                        {
                            var order = orderHeaderDAL.GetById(orderid);
                            if (order != null)
                            {
                                model.Inspections = inspectionsDAL.GetForCustPo(order.custpo);
                                model.CustPo = order.custpo;
                            }
                            // ForFeedback(model.Feedback);
                        }
                    }
                    var lines = orderLinesDAL.GetByExportCriteria(cprod_ids: new[] { model.Feedback.cprod_id ?? 0 }, etd_from: DateTime.Today.AddDays(1));
                    model.UpcomingOrders = lines.GroupBy(l => l.orderid).Select(g => g.First().Header).ToList();
                    return View(model);
                }
            }
            Error:
            ViewBag.message = "No record";
            return View("Message");
        }

        public ActionResult UpdateProductFeedback(Returns r, List<int> filesDeleted = null)
        {
            if (filesDeleted != null)
            {
                foreach (var item in filesDeleted)
                {
                    returnsImagesDAL.Delete(item, null);

                }
            }

            returnsDAL.Update(r);
            return Json("OK");
        }

        [HttpPost]
        public ActionResult UpdateCA(Returns r, List<int> filesDeleted = null)
        {
            if (filesDeleted != null)
            {
                foreach (var imageId in filesDeleted)
                {
                    returnsImagesDAL.Delete(imageId, null);
                }
            }

            if (r.Images != null)
            {
                CollectFiles(r);                
            }
            returnsDAL.Update(r);            
            return Json(r);
        }

        /// <summary>
        ///  SAVE UPLOADED FILES TO FOLDER ON SERVER
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        private List<Returns_images> SaveImages(List<Returns_images> images)
        {
            List<string> written_files = new List<string>();

            try
            {
                var sessionFiles = WebUtilities.GetTempFiles();

                if (sessionFiles != null)
                {
                    foreach (KeyValuePair<string, byte[]> kv in sessionFiles)
                    {
                        //Write file
                        string filePath = Utilities.GetFilePath(kv.Key, Server.MapPath(Settings.Default.returns_fileroot));
                        var sw = new StreamWriter(filePath);
                        var ms = new MemoryStream(kv.Value);
                        ms.WriteTo(sw.BaseStream);
                        sw.Close();
                        // images.Add(new Returns_images { return_image = Path.GetFileName(filePath) });
                        written_files.Add(filePath);

                        var returnImagePathName = Path.GetFileName(filePath);

                        if (!string.IsNullOrEmpty(returnImagePathName))
                        {
                            var item = images.FirstOrDefault(i => i.return_image == kv.Key);
                            if (item != null)
                            {
                                item.return_image = returnImagePathName;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //if some files written to folder and error occurred, delete them
                foreach (var item in written_files)
                {
                    System.IO.File.Delete(item);
                }
                throw;
            }

            return images;
        }

        [HttpGet]
        public ActionResult GetCAFeedbackImages(int returnId)
        {
            WebUtilities.ClearTempFiles();
            var images = returnsImagesDAL.GetByReturn(returnId);

            return Json(images, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRejection(Returns feedback, List<int> filesDeleted = null)
        {
            var copiedUploadedImages = new List<Returns_images>();

            if (filesDeleted != null)
            {
                foreach (var item in filesDeleted)
                {
                    returnsImagesDAL.Delete(item, null);
                }
            }

            if (feedback.Images != null)
            {
                copiedUploadedImages = feedback.Images.Where(c => c.image_unique == 0).ToList();

                CollectFiles(feedback);

                //feedback.Images = SaveImages(feedback.Images.Where(c => c.image_unique == 0).ToList());
            }

            //var dba_state = unitOfWork.ReturnRepository.Get(r => r.returnsid == feedback.returnsid, includeProperties: "AssignedQCUsers,Products,Product,Events,Subscriptions").FirstOrDefault();

            var dba_state = unitOfWork.ReturnRepository.Get(r => r.returnsid == feedback.returnsid, includeProperties: "AssignedQCUsers,Events").FirstOrDefault();

            //WARNING!!!    
            //since there is many to many table with only type 0 or 1 repository is gettting all the types
            //so I'm using quick hack to empty only type 1 from list and then re-insert back type 0 and new or existing type 1 
            if (dba_state.AssignedQCUsers != null && dba_state.AssignedQCUsers.Count > 0)
            {

                var tmp = dba_state.AssignedQCUsers.Where(q => q.type == 0).ToList();

                dba_state.AssignedQCUsers.Clear();

                if (tmp != null && tmp.Count > 0)
                    dba_state.AssignedQCUsers.AddRange(tmp);

            }

            //just insert all in collection, don't bother with delete and update since this is many to many table
            if (feedback.AssignedQCUsers != null)
            {
                foreach (var q in feedback.AssignedQCUsers)
                {
                    dba_state.AssignedQCUsers.Add(new Returns_qcusers
                    {
                        return_id = feedback.returnsid,
                        type = 1,
                        useruser_id = q.useruser_id
                    });

                }
            }

            dba_state.returnsid = feedback.returnsid;

            dba_state.rejection_date = feedback.rejection_date;
            dba_state.inspection_qty = feedback.inspection_qty;
            dba_state.sample_qty = feedback.sample_qty;

            dba_state.rejection_qty = feedback.rejection_qty;
            dba_state.recheck_required = feedback.recheck_required;
            dba_state.recheck_date = feedback.recheck_date;
            dba_state.recheck_status = feedback.recheck_status;
            dba_state.client_comments2 = feedback.client_comments2;
            dba_state.client_comments3 = feedback.client_comments3;
            dba_state.assigned_qc = feedback.assigned_qc;

            var imageType = new Returns_images();
            var user = accountService.GetCurrentUser();

            if (feedback != null && feedback.Images != null)
            {
                foreach (var item in feedback.Images.Where(c => c.image_unique == 0 && (c.file_category == 2 || c.file_category == 1)))
                {
                    if (item != null)
                    {
                        item.return_id = feedback.returnsid;
                        item.user_type = 0;
                        item.added_by = user.userid;
                        item.added_date = DateTime.Now;

                        var citem = copiedUploadedImages.FirstOrDefault(n => n.return_image == item.return_image);

                        if (citem != null && citem.file_category != null)
                            item.file_category = citem.file_category;

                        returnsImagesDAL.Create(item, null);
                    }
                }
            }

            RegisterEvent(dba_state, ReturnEventType.Recheck);
            
            unitOfWork.Save();

            return Json("OK");
        }

        private void FillSubTotals(DateTime startDate, List<Product_faults> faults, List<Sales_data> salesData, ISubTotals subTotals)
        {

            subTotals.SalesSubTotals = new SubTotal[12];
            subTotals.FaultXpg2SubTotals = new SubTotal[12];
            subTotals.FaultSubTotals = new SubTotal[12];
            for (var i = -11; i < 1; i++)
            {
                var i1 = i;
                subTotals.FaultSubTotals[11 + i] = new SubTotal
                {
                    month21 =
                        Utilities.GetMonth21FromDate(startDate.AddMonths(i)),
                    subtotal =
                        faults.Where(
                            f =>
                            f.fault_date >=
                            Utilities.GetMonthStart(startDate.AddMonths(i1)) &&
                            f.fault_date <=
                            Utilities.GetMonthEnd(startDate.AddMonths(i1)))
                             .Sum(f => f.fault_qty)
                };

                subTotals.FaultXpg2SubTotals[11 + i] = new SubTotal
                {
                    month21 =
                        Utilities.GetMonth21FromDate(startDate.AddMonths(i)),
                    subtotal =
                        faults.Where(
                            f => f.fault_reason == "XPG2" &&
                                 f.fault_date >=
                                 Utilities.GetMonthStart(startDate.AddMonths(i1)) &&
                                 f.fault_date <=
                                 Utilities.GetMonthEnd(startDate.AddMonths(i1)))
                             .Sum(f => f.fault_qty)
                };

                subTotals.SalesSubTotals[11 + i] = new SubTotal
                {
                    month21 = Utilities.GetMonth21FromDate(startDate.AddMonths(i))
                };
                var sales =
                    salesData.FirstOrDefault(
                        om =>
                        om.month21 ==
                        Utilities.GetMonth21FromDate(startDate.AddMonths(i)));
                if (sales != null)
                    subTotals.SalesSubTotals[11 + i].subtotal = sales.sales_qty;

            }
        }

        public ActionResult GetPGChart(int? return_id)
        {
            if (return_id == null)
                return null;
            var feedback = returnsDAL.GetById(return_id.Value);
            if (feedback.cprod_id != null)
                feedback.Product = custproductsDAL.GetById(feedback.cprod_id.Value);
            if (feedback.request_date != null && feedback.cprod_id != null)
            {
                var from = Utilities.GetMonthStart(feedback.request_date.Value.AddMonths(-11));
                var to = Utilities.GetMonthEnd(feedback.request_date.Value);
                var startDate = feedback.request_date.Value;
                var model = new ProductFeedbackModel { Feedback = feedback };
                model.SalesData = salesDataDal.GetForPeriod(feedback.Product.cprod_id, Utilities.GetMonth21FromDate(feedback.request_date.Value.AddMonths(-11)),
                                                             Utilities.GetMonth21FromDate(feedback.request_date.Value));
                model.Faults = productFaultsDAL.GetProductFaults(feedback.cprod_id.Value, from, to);
                FillSubTotals(startDate, model.Faults, model.SalesData, model);
                var chart = CreatePGChart(model, model.Feedback.returnsid);

                var ms = new MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "image/jpg");
            }
            return null;
        }

        private Chart CreatePGChart(ISubTotals model, int id, string chartName = "Chart_Pg")
        {
            var chart = new Chart { Width = Settings.Default.Feedback_pg_chart_width, Height = Settings.Default.Feedback_pg_chart_height };

            FormatChart(chart, "Month", "Faults");

            //var currdata = AnalyticsDAL.GetSalesByMonth(WebUtilities.GetMonthFromNow(-12),
            //                                            WebUtilities.GetMonthFromNow(2));
            //var previousData = AnalyticsDAL.GetSalesByMonth(WebUtilities.GetMonthFromNow(-24),
            //                                                WebUtilities.GetMonthFromNow(-13));

            foreach (var part in model.FaultSubTotals.OrderBy(f => f.month21))
            {
                if (part != null)
                {
                    var salesData = model.SalesSubTotals.FirstOrDefault(s => s.month21 == part.month21);
                    chart.Series[0].Points.AddXY(Utilities.GetDateFromMonth21(part.month21), salesData != null && salesData.subtotal > 0 ? part.subtotal / salesData.subtotal : 0);
                    //chart.Series[0].Points.Last().IsValueShownAsLabel = true;
                    chart.Series[0].Points.Last().LabelFormat = "P1";
                }
            }


            foreach (var part in model.FaultXpg2SubTotals.OrderBy(d => d.month21))
            {
                if (part != null)
                {
                    var salesData = model.SalesSubTotals.FirstOrDefault(s => s.month21 == part.month21);
                    //add year to actual date to show both series under same X axis
                    chart.Series[1].Points.AddXY(Utilities.GetDateFromMonth21(part.month21), salesData != null && salesData.subtotal > 0 ? part.subtotal / salesData.subtotal : 0);
                    //chart.Series[1].Points.Last().IsValueShownAsLabel = true;
                    chart.Series[1].Points.Last().LabelFormat = "P1";
                }
            }


            //return File(StreamChart(chart), "image/jpg");
            //SaveChartImage(chartName, chart,id.ToString());
            return chart;
        }

        private void FormatChart(Chart chart, string xTitle, string yTitle, bool customize = true)
        {
           
            var area = new ChartArea { AxisY = { IsInterlaced = true, InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE) } };

            area.AxisX.Title = xTitle;
            area.AxisX.IntervalType = DateTimeIntervalType.Months;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = yTitle;
            area.AxisY.LabelStyle.Format = "P1";


            area.AxisX.LabelStyle.Format = "MMM yy";

            chart.ChartAreas.Add(area);


            var series = new Series
            {
                Color = Color.Orange,
                MarkerColor = Color.White,
                MarkerBorderColor = Color.Orange,
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                LegendText = "PG faults",//string.Format("{0}/{1}", DateTime.Today.Year - 2, DateTime.Today.Year - 1),
                XValueType = ChartValueType.Date
            };
            chart.Series.Add(series);

            series = new Series
            {
                Color = Color.LightSkyBlue,
                MarkerBorderColor = Color.LightSkyBlue,
                BorderWidth = 3,
                MarkerColor = Color.White,
                ChartType = SeriesChartType.Line,
                LegendText = "XPG2 PG faults",//string.Format("{0}/{1}", DateTime.Today.Year - 1, DateTime.Today.Year),
                XValueType = ChartValueType.Date
            };

            chart.Series.Add(series);

            var legend = new Legend { Docking = Docking.Bottom, Alignment = StringAlignment.Center };
            chart.Legends.Add(legend);
        }

        private void SaveChartImage(string name, Chart chart, string key)
        {
            chart.SaveImage(Path.Combine(Server.MapPath(Properties.Settings.Default.returns_fileroot), string.Format("{0}_{1}.jpg",/*DateTime.Today.ToString("yyyyMMdd")*/key, name)), ChartImageFormat.Jpeg);
        }

        public ActionResult Image(string param)
        {
            var parts = param.Split('#');
            if (parts.Length > 1)
            {
                var key = parts[1];
                var name = parts[0];
                if (key == Settings.Default.StatsKey)
                {
                    return
                        File(
                            Path.Combine(Server.MapPath(Properties.Settings.Default.returns_fileroot),
                                         name + ".jpg"),
                            "image/jpg");
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public ActionResult ProductFeedback()
        {
            WebUtilities.ClearTempFiles();
            var company = companyDAL.GetById(accountService.GetCurrentUser().company_id);
            var model = new ProductFeedbackModel { 
                Brands = brandsDAL.GetAll(), 
                Importances = returnsImportanceDAL.GetForType(erp.Model.Returns.ClaimType_Product), Resolutions = GetResolutions() };
            model.Company = company;
            int numOfReturns = returnsDAL.GetNoOfReturns(company.user_id, DateTime.Today, erp.Model.Returns.ClaimType_Product);
            model.Feedback = new Returns
            {
                return_no = string.Format("FB-{0}-?-{1}-{2}", company.customer_code, DateTime.Today.ToString("yyMMdd"),
                                              Utilities.OrdinalToLetters(numOfReturns + 1)),
                Images = new List<Returns_images>()
            };
            model.Months = new List<LookupItem>();
            for (var i = -6; i <= 0; i++)
            {
                model.Months.Add(new LookupItem { id = Utilities.GetMonthFromDate(DateTime.Today.AddMonths(i)), value = DateTimeFormatInfo.CurrentInfo.MonthNames[DateTime.Today.AddMonths(i).Month - 1] });
            }

            return View(model);
        }

        public ActionResult GetOrders(int cprod_id)
        {
            var lines = orderLinesDAL.GetForProductAndCriteria(null, null, cprod_id,
                                                                accountService.GetCurrentUser().company_id);
            return Json(lines.OrderByDescending(l => l.Header.orderdate).Select(l => new { l.Header.orderid, l.Header.custpo }));
        }


        public ActionResult DeleteTempFile(string name)
        {
            WebUtilities.DeleteTempFile(name);
            return Json("OK");
        }

        public ActionResult DeleteImageFile(int? image_unique, int? return_id)
        {
            if (image_unique != null)
                returnsImagesDAL.Delete(Convert.ToInt32(image_unique), null);

            return Json("OK");
        }

        public ActionResult DeleteCommentFile(int? return_comment_file_id, int? return_comment_id, int? image_id)
        {
            if (return_comment_file_id != null)
                returnsCommentsFilesDAL.Delete(Convert.ToInt32(return_comment_file_id));

            return Json("OK");
        }

        public ActionResult DeleteCommentTempFile(string name)
        {
            WebUtilities.DeleteTempFile(name, "returncomment_temp_");
            return Json("OK");
        }

        public ActionResult CommentFiles(string name)
        {
            string fileName = WebUtilities.GetFileName(name, Request);
            return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize, "returncomment_temp_"), "text/html");
        }

        public ActionResult GetCommentTempFile(string file)
        {
            var oFile = WebUtilities.GetTempFile(file, "returncomment_temp_");
            if (oFile != null)
                return File(oFile, WebUtilities.ExtensionToContentType(Path.GetExtension(file).Replace(".", "")));
            return null;
        }

        private List<Returns_comments_files> GetCommentFiles()
        {
            var files = new List<Returns_comments_files>();
            var sessionFiles = WebUtilities.GetTempFiles("returncomment_temp_");
            if (sessionFiles != null)
            {
                foreach (KeyValuePair<string, byte[]> kv in sessionFiles)
                {
                    string filePath = Utilities.WriteFile(kv.Key, Server.MapPath(Settings.Default.returns_fileroot), kv.Value);
                    files.Add(new Returns_comments_files { image_name = Path.GetFileName(filePath) });
                }
            }

            return files;
        }

        [HttpPost]
        public ActionResult CreateComment(Returns_comments comment, bool? closeTicket = false)
        {
            comment.comments_date = DateTime.Now;
            comment.comments_from = accountService.GetCurrentUser().userid;
            comment.Creator = accountService.GetCurrentUser();

            comment.Files = GetCommentFiles();
            //foreach (var file in files)
            //{
            //    var foundFile = comment.Files != null ? comment.Files.FirstOrDefault(f => f.image_name == file.image_name) : null;
            //    if (foundFile == null && comment.Files != null)
            //        //Maybe filename was appended with _1,_2
            //        foundFile = comment.Files.FirstOrDefault(f => file.image_name.StartsWith(f.image_name));
            //    if (foundFile != null)
            //        foundFile.image_name = file.image_name;
            //}

            returnsCommentsDAL.Create(comment);
            WebUtilities.ClearTempFiles("returncomment_temp_");
            var currUser = accountService.GetCurrentUser();
            var feedback = comment.return_id != null ? returnsDAL.GetById(comment.return_id.Value) : null;
            if (feedback != null)
            {
                feedback.Product = custproductsDAL.GetById(feedback.cprod_id ?? 0);
                var client = feedback.client_id != null ? companyDAL.GetById(feedback.client_id.Value) : null;
                var factory = feedback.Product != null && feedback.Product.MastProduct != null &&
                              feedback.Product.MastProduct.factory_id != null
                                  ? companyDAL.GetById(feedback.Product.MastProduct.factory_id.Value)
                                  : null;
                var creator = feedback.request_userid != null ? userDAL.GetById(feedback.request_userid.Value) : null;
                var subscribers = feedbackSubscriptionsDAL.GetForReturn(feedback.returnsid);

                var subscribersToSkip = new List<Feedback_subscriptions>();
                if (feedback.claim_type == erp.Model.Returns.ClaimType_ITFeedback && comment.comments_to == 0)
                {
                    //Filter subscribers for internal comments

                    foreach (var sub in subscribers)
                    {
                        sub.User.Roles = roleDAL.GetRolesForUser(sub.User.userid);
                        if ((IsUserIT(currUser) && IsUserFC(sub.User)) || (IsUserFC(currUser) && IsUserIT(sub.User)))
                            subscribersToSkip.Add(sub);
                    }
                    foreach (var sub in subscribersToSkip)
                    {
                        subscribers.Remove(sub);
                    }
                }

                //Temporary exception - product feedback creators' comment doesn't generate emails 13.6.2014
                if (feedback.claim_type == erp.Model.Returns.ClaimType_Product && !IsUserIT(currUser) && !IsUserFC(currUser))
                    subscribers.Clear();

                //send email
                var subject = string.Format(App_GlobalResources.Resources.Feedback_comment_subject, feedback.return_no);

                var resBody =
                    App_GlobalResources.Resources.ResourceManager.GetString(string.Format("Feedback_comment_{0}_body", feedback.claim_type));
                string body = string.Empty;
                if (resBody != null)
                {
                    if (feedback.claim_type == erp.Model.Returns.ClaimType_Product)
                    {
                        body = string.Format(resBody,
                                         client != null ? client.customer_code : "",
                                         factory != null ? factory.factory_code : "",
                                         feedback.Product != null ? feedback.Product.cprod_code1 : "",
                                         feedback.Product != null ? feedback.Product.cprod_name : "",
                                         currUser.company_id == 1 ? "FE Office" : currUser.userwelcome, comment.comments);
                    }
                    else if (feedback.claim_type == erp.Model.Returns.ClaimType_ITFeedback || feedback.claim_type == erp.Model.Returns.ClaimType_CorrectiveAction)
                    {
                        body = string.Format(resBody, /*curUser.company_id == 1 ? "FE Office" : */ currUser.userwelcome,
                                             comment.comments, WebUtilities.GetSiteUrl(), comment.return_id);
                    }

                }
                                
                //under subscribers
                var subscriberEmails = string.Join(",", subscribers.Where(s => !string.IsNullOrEmpty(s.User.user_email)
                        && (feedback.claim_type == erp.Model.Returns.ClaimType_ITFeedback || comment.comments_to == 1 || s.User.company_id == 1))
                                                  .Select(s => s.User.user_email).Distinct().ToList());
                
                var emailRecipientsDb = emailRecipientsDAL.GetByCriteria(accountService.GetCurrentUser().company_id, "feedback", feedback.claim_type.ToString(), comment.comments_to == 1 ? "1" : "2").FirstOrDefault();


                var creatorEmails = creator != null ? creator.user_email : "";
                string to = string.Empty, cc = string.Empty, bcc = string.Empty;
                if (emailRecipientsDb != null)
                {
                    to = SubstituteMacros(emailRecipientsDb.to, subscriberEmails, creatorEmails);
                    cc = SubstituteMacros(emailRecipientsDb.cc, subscriberEmails, creatorEmails);
                    bcc = SubstituteMacros(emailRecipientsDb.bcc, subscriberEmails, creatorEmails);
                }
                else
                {
                    to = subscriberEmails;
                    if (creator != null && creator.userid != accountService.GetCurrentUser().userid && !string.IsNullOrEmpty(creator.user_email)
                        && (comment.comments_to == 1 || (IsUserIT(currUser) && IsUserIT(creator)) || (IsUserFC(currUser) && IsUserFC(creator))))
                    {
                        to += "," + creator.user_email;
                    }
                }

                if (closeTicket == true)
                {
                    feedback.openclosed = 1;
                }
                else
                {
                    feedback.openclosed = 0; //open it in case it was closed
                }
                returnsDAL.Update(feedback);

                if (!string.IsNullOrEmpty(to))
                    mailHelper.SendMail(Properties.Settings.Default.Feedback_From, to, subject, body, cc, bcc,
                        comment.Files.Select(f => new System.Net.Mail.Attachment(Path.Combine(Server.MapPath(Settings.Default.returns_fileroot), f.image_name))).ToArray());


            }
            return Json(comment);
        }

        public string SubstituteMacros(string recipients, string subscriberEmails, string creatorEmails)
        {
            var result = new List<string>();
            var currUserEmail = accountService.GetCurrentUser()?.user_email;
            var parts = recipients.Split(',');
            foreach (var part in parts)
            {
                if (part == "{creator}")
                {
                    if (!string.IsNullOrEmpty(creatorEmails))
                    {
                        var mails = creatorEmails.Split(',');
                        result.AddRange(mails);
                    }

                }
                else if (part == "{subscribers}")
                {
                    if (!string.IsNullOrEmpty(subscriberEmails))
                        result.AddRange(subscriberEmails.Split(','));
                }
                else
                {
                    result.Add(part);
                }
            }
            return string.Join(",", result.Distinct());
        }



        public ActionResult GetFile(string filename)
        {
            return File(Path.Combine(Server.MapPath(Settings.Default.returns_fileroot), filename),
                        WebUtilities.GetMIMEType(filename), filename);
        }

        //private List<Lookup> GetImportances()
        //{
        //    //return new []{new Lookup{Id=3, Title = "High (2 business days)" }, new Lookup{ Id=2, Title = "Medium (4 business days)" }, new Lookup{ Id=3, Title = "Low (10 business days)" }}.ToList();
        //}

        private static List<Lookup> GetResolutions()
        {
            return new[] {new Lookup{ Id= 1, Title = "Yes - customer has been refunded" },new Lookup { Id = 2, Title = "Yes - customer has been given a replacement and the issue is resolved" },
                            new Lookup{ Id= 3, Title = " No - this issue is still pending a resolution" }}.ToList();
        }

        public ActionResult ProductFaults(int month21, int cprod_id, string type = "")
        {
            var model = new ProductFaultModel
            {
                Faults = productFaultsDAL.GetProductFaults(cprod_id,
                                                                Utilities.GetMonthStart(
                                                                    Utilities.GetDateFromMonth21(month21)),
                                                                Utilities.GetMonthEnd(
                                                                    Utilities.GetDateFromMonth21(month21))),
                FaultDescriptions = productfaultReasonDescriptionDAL.GetAll()
            };
            if (!string.IsNullOrEmpty(type))
                model.Faults = model.Faults.Where(f => f.fault_reason == type).ToList();
            return PartialView(model);
        }

        public ActionResult ITFeedbacks(bool showAllCompleted = false)
        {
            var groupsId = CurrentUser.Groups?.Select(g => (int?)g.id).ToList();

            var authorizations = unitOfWork.FeedbackAuthorizationRepository.Get(fa => groupsId.Contains(fa.usergroup_id) && fa.feedback_type_id == erp.Model.Returns.ClaimType_ITFeedback, includeProperties: "Levels").ToList();

            
            //reduced dataset on only properties needed            
            var feedbacks_simple = returnsDAL.GetForClaimTypeSimple(erp.Model.Returns.ClaimType_ITFeedback, groupsId: groupsId).
                Where(f => f.status1 == (int)FeedbackStatus.Live || (f.status1 != (int)FeedbackStatus.Cancelled && CheckFeedbackVisibilityForUser(f, CurrentUser, authorizations))).ToList();

            
            //take only user ids from feedbacks
            var tempFeedbackLastCommenterIds = feedbacks_simple.Where(l => l.Last_Commenter_Id != null).Select(f => (int)f.Last_Commenter_Id).Distinct().ToList();
            //load users
            var feedbackUsers = unitOfWork.UserRepository.Get(u => tempFeedbackLastCommenterIds.Contains(u.userid)).ToList();

            //feedbacks with depended objects
            var tempFeedbackIds = feedbacks_simple.Select(r => r.returnsid).ToList();
            var feedbackObjects = unitOfWork.ReturnRepository.Get(r => tempFeedbackIds.Contains(r.returnsid), includeProperties: "Importance,Category,IssueType").ToList();

            foreach (var f in feedbackObjects)
            {
                var fsimple = feedbacks_simple.FirstOrDefault(fs => fs.returnsid == f.returnsid);

                f.Last_Commenter_Id = fsimple?.Last_Commenter_Id;
                f.HasComments = fsimple?.HasComments;

                var lastCommenter = feedbackUsers.FirstOrDefault(u => u.userid == f.Last_Commenter_Id);

                if (lastCommenter != null)
                {
                    f.Last_Commenter_Name = lastCommenter.userwelcome;
                }
            }

            return
                View(new ITFeedbackListModel
                {
                    Feedbacks = feedbackObjects,
                    ShowAllCompleted = showAllCompleted,
                    ListLimit = Settings.Default.ITFeedback_ListLimit,
                    FeedbackAuthorizationLevels = authorizations.SelectMany(a => a.Levels).ToList(),
                    CurrentUser = CurrentUser,
                    ShowIssueType = true,
                    CreateAction = "ItFeedback#/create",
                    EditAction = "ItFeedback#/edit"
                });
        }

        private bool CheckFeedbackVisibilityForUser(Returns f, User currentUser, List<feedback_authorization> authorizations)
        {
            if (f.request_userid == CurrentUser.userid)
                return true;
            var auth = authorizations?.FirstOrDefault(a => a.feedback_type_id == f.claim_type && a.feedback_issue_type_id == f.issue_type_id);
            if (authorizations == null || auth == null)
                return CurrentUser.HasPermission(Permission.ITF_Authorize);

            var authLevels = auth.Levels.Where(l => l.id <= f.authorization_level).ToList();
            if (authLevels != null)
            {
                return currentUser.Groups.Any(g => authLevels.Select(l => l.authorization_usergroupid).Contains(g.id));
            }
            return false;
        }

        [HttpPost]
        public ActionResult ITFeedbacks(ITFeedbackListModel m)
        {
            m.Feedbacks = returnsDAL.Search(erp.Model.Returns.ClaimType_ITFeedback, m.SearchTerm);
            var importances = returnsImportanceDAL.GetAll();
            var feedback_categories = feedbackCategoryDAL.GetForType(erp.Model.Returns.ClaimType_ITFeedback);
            foreach (var f in m.Feedbacks)
            {
                if (f.importance_id > 0)
                {
                    f.Importance = importances.FirstOrDefault(i => i.importance_id == f.importance_id);
                }
                if (f.feedback_category_id > 0)
                {
                    f.Category = feedback_categories.FirstOrDefault(c => c.feedback_cat_id == f.feedback_category_id);
                }
            }
            return
                View(m);
        }


        public ActionResult ItFeedback()
        {
            return View("EditITFeedbackNew");
        }

        public ActionResult GetReturnNo()
        {
            var nextFeedbackNum = returnsDAL.GetNextFeedbackNum(erp.Model.Returns.ClaimType_CorrectiveAction);
            string returnNo = $"CA-{nextFeedbackNum:0000}";
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var set = JsonConvert.SerializeObject(returnNo, Formatting.None, settings);
            return Json(set);
        }

        public ActionResult CorrectiveActions(bool showAllCompleted = false, bool export = false, DateTime? from = null, DateTime? to = null)
        {
            
            var feedbacks = unitOfWork.ReturnRepository.GetClaimsSimple((int)erp.Model.Returns.ClaimType_CorrectiveAction, (int)FeedbackStatus.Live).
                Select(cs => new Returns
                {
                    returnsid = cs.returnsid,
                    return_no = cs.return_no,
                    openclosed = cs.openclosed,
                    request_date = cs.request_date,
                    importance_id = cs.importance_id,
                    Importance = new Returns_importance { importance_text = cs.importance },
                    client_comments = cs.description,
                    Creator = new User { userid = cs.request_userid ?? 0, userwelcome = cs.creator },
                    Last_Commenter_Name = cs.lastUpdatedBy,
                    Category = new Feedback_category { name = cs.category },
                    Product = new Cust_products { MastProduct = new Mast_products { Factory = new Company { factory_code = cs.factory } } },
                    HasComments = cs.commentCount > 0
                }).OrderBy(r => r.recheck_date).ToList();

            //apply export time filter
            if (export)
            {
                if (from != null)
                    feedbacks = feedbacks.Where(f => (f.request_date >= from && f.request_date <= (to == null ? DateTime.Now.AddMonths(1) : to))).ToList();
            }

            var returnsids = feedbacks.Select(f => f.returnsid).ToList();
            var qcsubs = unitOfWork.ReturnRepository.Get(r => returnsids.Contains(r.returnsid) && r.ReturnsQCUsers.Count > 0, includeProperties: "ReturnsQCUsers.User").ToList();

            var qcsubsfullnames = qcsubs.Select(q => new
            {
                returnsid = q.returnsid,
                qcnames = String.Join(", ", q.ReturnsQCUsers.Select(u => u.User.userwelcome))
            }).ToDictionary(r => r.returnsid, r => r.qcnames);

            if (export)
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=Create CA list.xls");
                Response.ContentType = "application/vnd.ms-excel";
            }
            return
                View("ITFeedbacks", new ITFeedbackListModel
                {
                    Feedbacks = feedbacks,
                    CreateAction = "CreateCA",
                    EditAction = "EditCA",
                    ShowAllCompleted = showAllCompleted,
                    Export = export,
                    ShowExport = true,
                    ExportReturnsQCSubscribers = qcsubsfullnames
                });
        }

        public ActionResult CAExport(DateTime? from, DateTime? to, bool showAllCompleted = false)
        {

            //put defaults on time constraint

            Response.AddHeader("Content-Disposition", "attachment;filename=Create CA list.xls");
            Response.ContentType = "application/vnd.ms-excel";

            return null;
        }

        public ActionResult CorrectiveActionsPdf()
        {
            return null;
        }

        [Route("m-ca")]
        [Route("claims/CreateCaSimple")]
        public ActionResult CreateCaSimple()
        {
            ViewBag.User = accountService.GetCurrentUser().userwelcome;
            return View();
        }

        public ActionResult CreateCA()
        {
            WebUtilities.ClearTempFiles();            
            var returnNo = string.Format("CA-{0:0000}", returnsDAL.GetNextFeedbackNum(erp.Model.Returns.ClaimType_CorrectiveAction));

            var model = new ITFeedbackModel
            {
                Feedback = new Returns
                {
                    claim_type = erp.Model.Returns.ClaimType_CorrectiveAction,
                    return_no = returnNo,
                    Images = new List<Returns_images>(),
                    Subscriptions = new List<Feedback_subscriptions>(),
                    recheck_required = 1,
                    feedback_category_id = 6
                },
                EditMode = EditMode.New,
                Categories = feedbackCategoryDAL.GetForType(erp.Model.Returns.ClaimType_CorrectiveAction),
                Importances = returnsImportanceDAL.GetForType(erp.Model.Returns.ClaimType_CorrectiveAction)
            };

            var user = accountService.GetCurrentUser();

            if (user.IsInRole(Role.FCOfficeUser))
            {
                var adminUsers = roleDAL.GetUsersInRole(Role.FCOfficeAdmin);
                model.Feedback.Subscriptions =
                    adminUsers.Select(u => new Feedback_subscriptions { subs_useruserid = u.userid, User = u }).ToList();
            }

            else if (user.IsInRole(Role.FCOfficeAdmin))
            {
                var CAAdminUsers = roleDAL.GetUsersInRole(Role.CAAdmin);
                model.Feedback.Subscriptions =
                    CAAdminUsers.Select(u => new Feedback_subscriptions { subs_useruserid = u.userid, User = u }).ToList();
            }

            //var users = unitOfWork.ReturnRepository.Get(includeProperties: "ReturnsQCusers");
            return View("EditCAnew", model);
        }

        private void CollectFiles(Returns r)
        {
            foreach (var i in r.Images.Where(i => !string.IsNullOrEmpty(i.return_image)))
            {
                var temp_file = WebUtilities.GetTempFile(i.file_id);

                if (temp_file != null)
                {
                    var filePath = Utilities.WriteFile(i.return_image, System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), temp_file);

                    if (!string.IsNullOrEmpty(filePath))
                        i.return_image = Path.GetFileName(filePath);
                }
            }
        }

        [HttpPost]
        public ActionResult CreateCA(Returns r)
        {
            Cust_products product;

            //r.Images = GetFiles(r.Images, null);

            if (r.Images != null)
                CollectFiles(r);

            r.claim_type = erp.Model.Returns.ClaimType_CorrectiveAction;

            //CA Factory controllers are added in r.Subscribers collection with this method!
            var recipients = CreateFeedbackSubscriptions(r, out product);

            RegisterEvent(r, ReturnEventType.CorrectiveActionCreate);

            //All Corrective Actions will be Live as requested.
            PrepareAndCreateReturn(r, FeedbackStatus.Live, useEF: true);

            /*
            if (!Settings.Default.Disabled_ForTestingOnStaging)
            {   
                SendEmails(r, recipients, product);
            }
            */

            return Json("OK");
        }

        public ActionResult EditCA(int? id = null, bool pdf = false)
        {
            WebUtilities.ClearTempFiles();
            WebUtilities.ClearTempFiles("returncomment_temp_");

            var user = accountService.GetCurrentUser();

            if (id != null)
            {
                var f = returnsDAL.GetById(id.Value);

                if (f.claim_type == erp.Model.Returns.ClaimType_ITFeedback)
                    return RedirectToAction("ItFeedback", "Claims", new { id = id.Value });

                var model = new ITFeedbackModel
                {
                    Feedback = f,
                    // Feedback = unitOfWork.ReturnRepository.GetByID(id.Value),
                    EditMode = EditMode.Edit,
                    StandardResponses = standardResponseDAL.GetAll(),
                    CanEditExternalComments = true,
                    CanViewExternalComments = true,
                    CanEditInternalComments = true,// user.admin_type.In(1, 2, 3, 4, 8),
                    CanViewInternalComments = true, //user.admin_type.In(1, 2, 3, 4, 8),
                };

                var rqc = unitOfWork.ReturnsUserUserRepository.Get(r => r.return_id == id && r.type == 0, includeProperties: "User,Return").ToList();

                if (rqc != null)
                    model.Feedback.ReturnsQCUsers = rqc;

                var aqc = unitOfWork.ReturnsUserUserRepository.Get(r => r.return_id == id && r.type == 1, includeProperties: "User,Return").ToList();

                if (aqc != null)
                    model.Feedback.AssignedQCUsers = aqc;

                if (model.Feedback.cprod_id != null)
                    model.Feedback.Product = custproductsDAL.GetById(model.Feedback.cprod_id.Value);
                else
                    model.Feedback.Products = unitOfWork.ReturnRepository.Get(c => c.returnsid == model.Feedback.returnsid, includeProperties: "Products").First().Products;

                if (model.Feedback.Comments != null && model.Feedback.Comments.Count > 0)
                    FilterInternalComments(model.Feedback.Comments);


                if (model.Feedback.importance_id != null)
                    model.Feedback.Importance = returnsImportanceDAL.GetById(model.Feedback.importance_id.Value);

                //model.Feedback.Comments.Add(new Returns_comments { comments_to = 1, return_id = model.Feedback.returnsid, comments = model.Feedback.client_comments2, comments_date = model.Feedback.request_date, comments_from = model.Feedback.request_userid, Creator = model.Feedback.Creator });
                if (pdf)
                    return View("CorrectiveActionsPdf", model);

                return View("EditCAnew", model);
            }

            ViewBag.message = "No id";

            return View("Message");
        }

        public ITFeedbackModel ModelPdfCa(int? id)
        {
            var f = returnsDAL.GetById(id.Value);

            var model = new ITFeedbackModel() {
                Feedback = f
            };
            var rqc = unitOfWork.ReturnsUserUserRepository.Get(r => r.return_id == id && r.type == 0, includeProperties: "User,Return").ToList();
            if (rqc != null)
                model.Feedback.ReturnsQCUsers = rqc;

            var aqc = unitOfWork.ReturnsUserUserRepository.Get(r => r.return_id == id && r.type == 1, includeProperties: "User,Return").ToList();

            if (aqc != null)
                model.Feedback.AssignedQCUsers = aqc;

            if (model.Feedback.cprod_id != null)
                model.Feedback.Product = custproductsDAL.GetById(model.Feedback.cprod_id.Value);
            else
                model.Feedback.Products = unitOfWork.ReturnRepository.Get(c => c.returnsid == model.Feedback.returnsid, includeProperties: "Products").First().Products;

            if (model.Feedback.Comments != null && model.Feedback.Comments.Count > 0)
                FilterInternalComments(model.Feedback.Comments);


            if (model.Feedback.importance_id != null)
                model.Feedback.Importance = returnsImportanceDAL.GetById(model.Feedback.importance_id.Value);
            return model;
        }

        public ActionResult CAPdf(int id)
        {
            var model = ModelPdfCa(id);
            var products = $"{model.Feedback.Product?.cprod_code1} {model.Feedback.Product?.cprod_name}";
            if (model.Feedback.Products != null)
            {
                foreach (var item in model.Feedback.Products)
                {
                    products += $"\n{item.cprod_code1} {item.cprod_name}";
                }
            }

            IPdfManager pdfManager = new PdfManager();
            var doc = pdfManager.CreateDocument();

            /**Učitaj sa server map**/
            IPdfFont font = doc.Fonts.LoadFromFile(@"C:\Users\David\Downloads\msyh2.ttf");


            var objPage = doc.Pages.Add();

            /** table **/
            var param = pdfManager.CreateParam(@"rows=16, cols=2, width=750, CellPadding =5, height=20;Border=0; CellBorder=0.25;");
            var objParam = pdfManager.CreateParam("x=20, y=750");
            var cellParam = pdfManager.CreateParam("expand=true; alignment=left");
            var cellFirstCol = pdfManager.CreateParam("aligment=left; width= 100; expand=true");
            IPdfTable table = doc.CreateTable(param);
            table.Font = font;
            table.Rows[1].Cells[1].Width = 150;

            table.Rows[1].Cells[1].AddText("Reference", cellFirstCol, Missing.Value);
            table.Rows[1].Cells[2].AddText($" {model.Feedback.return_no}", cellParam, Missing.Value);

            table.Rows[2].Cells[1].AddText("Category", cellFirstCol, Missing.Value);
            table.Rows[2].Cells[2].AddText(model.Feedback.Category != null ? model.Feedback.Category.name : "", cellParam, Missing.Value);

            table.Rows[3].Cells[1].AddText("Product", cellFirstCol, Missing.Value);
            table.Rows[3].Cells[2].AddText($" {products}", cellParam, Missing.Value);

            table.Rows[4].Cells[1].AddText("PO#", cellFirstCol, Missing.Value);
            table.Rows[4].Cells[2].AddText($" {model.Feedback.custpo}", cellParam, Missing.Value);


            table.Rows[5].Cells[1].AddText("Reason", cellFirstCol, Missing.Value);
            table.Rows[5].Cells[2].AddText($" {model.Feedback.client_comments}", cellParam, Missing.Value);

            table.Rows[6].Cells[1].AddText("Resolution-Prior to shipment the product", cellFirstCol, Missing.Value);
            table.Rows[6].Cells[2].AddText($" {model.Feedback.client_comments2}", cellParam, Missing.Value);

            table.Rows[7].Cells[1].AddText("Permanent Action - How to prevent reoccurring", cellFirstCol, Missing.Value);
            table.Rows[7].Cells[2].AddText($" { model.Feedback.client_comments3}", cellParam, Missing.Value);

            table.Rows[8].Cells[1].ColSpan = 2;
            var filepath = Server.MapPath($"{ Settings.Default.returns_fileroot}/IMG_20180716_092403.jpg");
            var fileExist = System.IO.File.Exists(Server.MapPath($"{ Settings.Default.returns_fileroot}/IMG_20180716_092403.jpg"));
            table.Rows[8].Cells[1].AddText("Uploaded files ", cellFirstCol, Missing.Value);
            //table.Rows[8].Cells[2].Text($" -- {fileExist} . {filepath}", cellParam, Missing.Value);


            //List<IPdfImage> images = new List<IPdfImage>();
            List<SetImageModel> images = new List<SetImageModel>();
            foreach (var item in model.Feedback.Images)
            {
                //images.Add(doc.OpenImage(Server.MapPath($"{ Settings.Default.returns_fileroot}/{item.return_image}")));
                images.Add(SetImage(item.return_image, doc));

            }


            int marginBottom = 0, marginLeft = 5;
            
            var sumW = 0.0;
            

            table.Rows[9].Height = (float)images.Select(n => n.ObjectImage.Height * n.ScaleX).Max();
            table.Rows[9].Cells[1].ColSpan = 2;
            //table.Rows[9].Cells[1].Canvas.DrawImage(image, "x=165, y=5, scaley = 0.2, scalex=0.2");

            for (int i = 0; i < images.Count; i++)
            {

                table.Rows[9].Cells[1].Canvas.DrawImage(images[i].ObjectImage, $"x= {sumW}, y={marginBottom}, scaley={images[i].ScaleY}, scalex={images[i].ScaleY}");

                sumW += (images[i].ObjectImage.Width * images[i].ScaleX) + marginLeft;
            }
            table.Rows[10].Cells[1].AddText("Rejection date", cellFirstCol, Missing.Value);
            table.Rows[10].Cells[2].AddText($" {model.Feedback.rejection_date.ToString("yyyy-MM-dd")}", cellParam, Missing.Value);

            table.Rows[11].Cells[1].AddText("Recheck date", cellFirstCol, Missing.Value);
            table.Rows[11].Cells[2].AddText($" {model.Feedback.recheck_date.ToString("yyyy-MM-dd")}", cellParam, Missing.Value);

            table.Rows[12].Cells[1].AddText("Recheck required", cellFirstCol, Missing.Value);
            var recheckRequired = model.Feedback.recheck_required != null && model.Feedback.recheck_required > 0 ? "yes" : "no";
            table.Rows[12].Cells[2].AddText($" {recheckRequired}", cellParam, Missing.Value);

            table.Rows[13].Cells[1].AddText("Inspection qty", cellFirstCol, Missing.Value);
            table.Rows[13].Cells[2].AddText($" {model.Feedback.inspection_qty}", cellParam, Missing.Value);

            table.Rows[14].Cells[1].AddText("Sample qty", cellFirstCol, Missing.Value);
            table.Rows[14].Cells[2].AddText($" {model.Feedback.sample_qty}", cellParam, Missing.Value);

            table.Rows[15].Cells[1].AddText("Rejection qty", cellFirstCol, Missing.Value);
            table.Rows[15].Cells[2].AddText($" {model.Feedback.rejection_qty} ", cellParam, Missing.Value);




            objPage.Canvas.DrawTable(table, objParam);

            var fileName = string.Format("CA.pdf");
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf", fileName);
        }

        private SetImageModel SetImage(string return_image, IPdfDocument doc)
        {
            var model = new SetImageModel();
            var path = Server.MapPath($"{Settings.Default.returns_fileroot}/{return_image}");
            var exist = System.IO.File.Exists(path);
            var isPdf = Path.GetExtension(return_image) == ".pdf";
            if (exist && !isPdf)
            {
                model.ObjectImage = doc.OpenImage(path);
                model.ScaleX = 0.2;
                model.ScaleY = 0.2;
                return model;
            }
            else if (isPdf)
            {
                model.ObjectImage = doc.OpenImage(Server.MapPath($"/Images/pdf-black.png"));
                model.ScaleX = 0.9;
                model.ScaleY = 0.9;
                return model;

            }
            else
            {
                model.ObjectImage = doc.OpenImage(Server.MapPath(Settings.Default.No_image));
                model.ScaleX = 0.5;
                model.ScaleY = 0.5; 
                return model;
            }

        }
        private class SetImageModel
        {
            public IPdfImage ObjectImage { get; set; }
            public double ScaleX { get; set; }
            public double ScaleY { get; set; }
        }

        [AllowAnonymous]
        public ActionResult CAReport(Guid statsKey, DateTime dateCA, string customerIdsIn, string customerIdsOut)
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                var model = new CAReportModel();

                DateTime reportDate = dateCA.AddDays(-7);

                var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(reportDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                model.WeekNumber = weekNumber;

                //model.InspectionItem = returnsDAL.GetCAReportInspectionItem(reportDate, customerIdsIn, customerIdsOut);

                //model.CAItems = returnsDAL.GetCAReportCAItems(reportDate, customerIdsIn, customerIdsOut);

                List<CAReportCAItem> newList = new List<CAReportCAItem>();

                //to show CA products comma separated in one column
                foreach (var v in model.CAItems)
                {
                    IEnumerable<string> prods = model.CAItems.Where(i => i.Reference == v.Reference).Select(i => i.Products);

                    string joint_products = string.Join(",", prods);

                    v.Products = joint_products;

                    if (!newList.Exists(n => n.Reference == v.Reference))
                        newList.Add(v);
                }

                model.CAItemsCount = model.CAItems.GroupBy(l => l.Reference).Count();

                //get ridoff multiple CA records with same reference (because products)
                model.CAItems = newList.OrderBy(x => x.Reference).ToList();

                return View("CAReport", model);
            }
            else
            {
                ViewBag.message = "Invalid key";
                return View("Message");
            }
        }

        public ActionResult Qa()
        {
            return View();
        }


        /// <summary>
        /// get orders from selected qc-s
        /// </summary>
        public ActionResult GetClientsOrders(List<int?> qcsIds)
        {
            if (qcsIds == null || qcsIds.Count == 0)
                return null;

            //List<int?> QcsIds = selectedQcs.Select(i => (int?)i.userid).ToList();

            
            var userId = accountService.GetCurrentUser().userid;
            var from = DateTime.Now.AddMonths(-2);
            var to = DateTime.Now.AddDays(1);
            //var inspections = unitOfWork.InspectionList2Repository
            //    .Get(i => i.insp_start >= from && i.insp_start <= to
            //        && (qcsIds.Contains(i.insp_qc1) || qcsIds.Contains(i.insp_qc2) 
            //                                || qcsIds.Contains(i.insp_qc3) || qcsIds.Contains(i.insp_qc4) 
            //                                || qcsIds.Contains(i.insp_qc5))
            //        && (i.insp_status == 0 && i.acceptance_fc != 2 && (i.v2_status != 2 || i.v2_status == null))
            //        && !string.IsNullOrEmpty(i.custpo)
            //        && i.insp_batch_inspection == 0
            //        && i.insp_type == "FI"
            //        ).OrderBy(c => c.insp_start).ToList();
            var inspections = inspectionsDAL.GetForQcs(from, to, qcsIds);
            return Json(inspections.Select(s => new { s.insp_id, s.insp_unique, s.custpo, s.customer_code, s.factory_code, s.new_insp_id, s.orderid }));
        }

        public ActionResult GetProductsNew(int? inspId, int? newInspId)
        {
            if (inspId != null)
            {
                var insp = unitOfWork
                    .InspectionRepository
                    .Get(i => i.insp_unique == inspId, includeProperties: "LinesTested").FirstOrDefault();

                var cprod_codes = insp.LinesTested.Select(l => l.insp_client_ref).ToList();

                var products = unitOfWork.CustProductRepository.Get(p =>
                    p.cprod_status != "D"
                    /* &&  p.BrandCompany.customer_code == insp.customer_code*/
                    && cprod_codes.Contains(p.cprod_code1), includeProperties: "BrandCompany, MastProduct.Factory").ToList();

                return Json(products.Select(n => new Cust_products
                {
                    cprod_id = n.cprod_id,
                    cprod_code1 = n.MastProduct?.Factory?.factory_code + " " + n.cprod_code1
                }).ToList());

                /*
                return Json(insp.LinesTested.Select(n => new
                {
                    id = n.insp_line_unique,
                    cprod_code = n.insp_client_ref,
                    desc = n.insp_client_desc,
                    insp_qty = n.insp_qty,
                    order_linenum = n.order_linenum,
                    cprod_id = n.order_linenum > 0 ? null :
                        (products.FirstOrDefault(p => p.cprod_code1 == n.insp_client_ref && p.BrandCompany.customer_code == insp.customer_code) ?? products.FirstOrDefault(p => p.cprod_code1 == n.insp_client_ref))?.cprod_id,
                    factory_ref = n.insp_factory_ref
                }).ToList());
                */
            }

            return Json(unitOfWork.InspectionV2Repository.Get(i => i.id == newInspId, includeProperties: "Lines.Product")
                .SelectMany(s => s.Lines.Select(l => new Cust_products { cprod_id = (int)l?.cprod_id.Value, cprod_code1 = l.insp_custproduct_code })));

            /* call from casimple 
            return Json(unitOfWork.InspectionV2Repository.Get(i => i.id == newInspId, includeProperties: "Lines.Product")
                .SelectMany(s => s.Lines.Select(n => new { id = n.insp_id, cprod_code = n.insp_custproduct_code, desc = "", insp_qty = n.inspected_qty, order_linenum = n.orderlines_id,factory_ref = n.Product?.factory_ref })));
            */
        }

        public ActionResult QaExport(int type = 8, int? status1 = 1)
        {
            var model = new List<ClaimSimple>();
            model = unitOfWork.ReturnRepository.GetClaimsSimple(type, status1).ToList();
            Response.AddHeader("Content-Disposition", "attachment;filename=QA_list.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return View(model);
        }

        public ActionResult All()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            return View();
        }

        public ActionResult GetCustProducts(string prefixText)
        {
            return Json(custproductsDAL.GetCustProductsByCriteria2(prefixText).Where(p => !string.IsNullOrEmpty(p.cprod_code1.Trim()))
                .Select(p => new { p.cprod_id, cprod_code1 = !string.IsNullOrEmpty(p.Client.customer_code) ? $"{p.cprod_code1} ({p.Client.customer_code})" : p.cprod_code1, p.cprod_name, p.factory_ref }));
        }

        public ActionResult GetOrdersByProduct(string prefixText, int cprod_id)
        {
            var lines = orderLinesDAL.GetForProductAndCriteria(null, null, cprod_id).Where(l => l.Header.custpo.Contains(prefixText)).ToList();
            return Json(lines.OrderByDescending(l => l.Header.orderdate).Distinct(new OrderLineByHeaderComparer()).Select(l => new { l.Header.orderid, l.Header.custpo }));
        }
        public ActionResult GetOrdersByProducts(string prefixText, string cprod_ids)
        {
            var arrNum = cprod_ids.Split(',');
            arrNum = arrNum.Where(a => a != "").ToArray();

            var Cprod_ids = cprod_ids.Split(',').Where(s => s != "").Select(s => Convert.ToInt32(s)).ToArray();
            var lines = new List<Order_lines>();
            foreach (var cprod_id in Cprod_ids)
            {
                lines.AddRange(orderLinesDAL.GetForProductAndCriteria(null, null, cprod_id).Where(l => l.Header.custpo.Contains(prefixText)).ToList());
            }
            return Json(lines.OrderByDescending(l => l.Header.orderdate).Distinct(new OrderLineByHeaderComparer()).Select(l => new { l.Header.orderid, l.Header.custpo }));
        }

        public void FilterInternalComments(List<Returns_comments> comments)
        {

            var currUser = accountService.GetCurrentUser();

            var commentUsers = new List<User>();
            var commentsToRemove = new List<Returns_comments>();
            foreach (var c in comments)
            {
                if (c.comments_to == 0)
                {
                    //internal
                    var user = commentUsers.FirstOrDefault(u => u.userid == c.Creator.userid);
                    if (user == null)
                    {
                        c.Creator.Roles = roleDAL.GetRolesForUser(c.Creator.userid);
                        commentUsers.Add(c.Creator);
                        user = c.Creator;
                    }
                    if ((IsUserFC(user) && IsUserIT(currUser)) || (IsUserIT(user) && IsUserFC(currUser)))
                        commentsToRemove.Add(c);
                }
            }
            foreach (var c in commentsToRemove)
            {
                comments.Remove(c);
            }
        }

        private bool IsUserIT(User user)
        {
            return user.IsUserIT();
        }

        private bool IsUserFC(User user)
        {
            return user.IsUserFC();
        }


        public ActionResult CreateItFeedback()
        {
            WebUtilities.ClearTempFiles();
            
            var returnNo = string.Format("IT-{0:0000}", returnsDAL.GetNextITFeedbackNum());
            var model = new ITFeedbackModel
            {
                Feedback = new Returns
                {
                    claim_type = erp.Model.Returns.ClaimType_ITFeedback,
                    return_no = returnNo,
                    Images = new List<Returns_images>(),
                    Subscriptions = new List<Feedback_subscriptions>()
                },
                EditMode = EditMode.New,
                Categories = feedbackCategoryDAL.GetForType(erp.Model.Returns.ClaimType_ITFeedback),
                Importances = returnsImportanceDAL.GetForType(erp.Model.Returns.ClaimType_ITFeedback),
                IssueTypes = unitOfWork.FeedbackIssueTypeRepository.Get(t => t.feedback_type_id == erp.Model.Returns.ClaimType_ITFeedback).ToList()
            };
            var user = CurrentUser;
            if (user.IsInRole(Role.FCOfficeUser))
            {
                var adminUsers = roleDAL.GetUsersInRole(Role.FCOfficeAdmin);
                model.Feedback.Subscriptions =
                    adminUsers.Select(u => new Feedback_subscriptions { subs_useruserid = u.userid, User = u }).ToList();
            }
            else if (user.IsInRole(Role.FCOfficeAdmin))
            {
                var ITAdminUsers = roleDAL.GetUsersInRole(Role.ITAdmin);
                model.Feedback.Subscriptions =
                    ITAdminUsers.Select(u => new Feedback_subscriptions { subs_useruserid = u.userid, User = u }).ToList();
            }
            return View("EditITFeedback", model);
        }

        [HttpPost]
        public ActionResult CreateItFeedback(Returns r)
        {
            var user = CurrentUser;
            r.usergroup_id = user.Groups.FirstOrDefault(g => g.returns_default == true)?.id;

            var authorization = unitOfWork.FeedbackAuthorizationRepository.Get(fa => r.usergroup_id == fa.usergroup_id
                            && fa.feedback_type_id == erp.Model.Returns.ClaimType_ITFeedback
                            && r.issue_type_id == fa.feedback_issue_type_id, includeProperties: "Levels").FirstOrDefault();

            if (authorization != null && authorization.Levels.Count > 0)
                r.authorization_level = authorization.Levels.OrderBy(l => l.level).FirstOrDefault().id;
#if DEBUG
            if (r.usergroup_id == null)
                throw new ArgumentException("no usergroup_id for returns record.");
#endif
            r.Images = GetFiles(r.Images, null);
            r.return_no = string.Format("IT-{0:0000}", returnsDAL.GetNextITFeedbackNum());
            r.claim_type = erp.Model.Returns.ClaimType_ITFeedback;
            PrepareAndCreateReturn(r, user.HasPermission(Permission.ITF_Authorize) ? FeedbackStatus.Live : FeedbackStatus.Incomplete, authorization: authorization);
            Cust_products product;
            var recipients = CreateFeedbackSubscriptions(r, out product, authorization, unitOfWork);
            SendEmails(r, recipients, product);
            return Json(r);
        }

        public ActionResult OpenCloseFeedback(int id, int openClose)
        {
            returnsDAL.UpdateOpenClose(id, openClose);
            var r = returnsDAL.GetById(id).openclosed;
            return Json(r);
        }

        public ActionResult Authorize(int id)
        {
            var r = returnsDAL.GetById(id);
            var groupIds = CurrentUser.Groups.Select(g => (int?)g.id).ToList();
            var authorization = unitOfWork.FeedbackAuthorizationRepository.Get(a => groupIds.Contains(a.usergroup_id) && a.feedback_type_id == r.claim_type && a.feedback_issue_type_id == r.issue_type_id, includeProperties: "Levels").FirstOrDefault();
            var status = FeedbackStatus.Live;
            var recipients = roleDAL.GetUsersInRole(Role.ITAdmin);
            var showButton = false;

            if (authorization != null)
            {
                var maxLevel = authorization.Levels.Max(l => l.level);
                var authLevel = authorization.Levels.FirstOrDefault(l => l.id == r.authorization_level);
                if (authLevel != null && authLevel.level < maxLevel)
                {
                    var nextLevel = authorization.Levels.FirstOrDefault(l => l.level == authLevel.level + 1);
                    if (nextLevel != null)
                    {
                        status = FeedbackStatus.Incomplete;
                        var group = unitOfWork.UserGroupRepository.Get(g => g.id == nextLevel.authorization_usergroupid, includeProperties: "Users").FirstOrDefault();
                        recipients = group.Users;
                        r.authorization_level = nextLevel.id;
                        if (CurrentUser.Groups.Any(g => g.id == group.id))
                            //retain authorization button if same user can authorize on next level
                            showButton = true;
                    }
                }
            }

            returnsDAL.UpdateStatus(id, status, r.authorization_level);

            foreach (var re in recipients)
            {
                if (r.Subscriptions.Count(s => s.subs_useruserid == re.userid) == 0)
                    feedbackSubscriptionsDAL.Create(new Feedback_subscriptions { subs_returnid = r.returnsid, subs_useruserid = re.userid });
            }
            SendEmails(r, recipients, null);
            return Json(new { showButton });
        }

        public ActionResult ListPGFaults(string clientList)
        {

            var from = Utilities.GetMonthStart(DateTime.Today.AddMonths(-6));
            var to = Utilities.GetMonthEnd(DateTime.Today.AddMonths(-1));
            var clients = clientList.Split(',').Select(int.Parse).ToList();

            var model = new ListPGFaultsModel
            {
                ListType = 1,
                Ratio = 3,
                Factories = companyDAL.GetFactories(),
                Sort = 1,    //Factories
                ShowPopupLink = true,
                ClientList = clientList
            };
            model.Rows = CreatePGList(clients, from, to, model.Ratio ?? 0, model.ListType);
            return View(model);
        }

        [HttpPost]
        public ActionResult ListPGFaults(ListPGFaultsModel m)
        {
            var from = Utilities.GetMonthStart(DateTime.Today.AddMonths(-6));
            var to = Utilities.GetMonthEnd(DateTime.Today.AddMonths(-1));
            m.ShowPopupLink = true;
            m.Factories = companyDAL.GetFactories();
            var clients = m.ClientList.Split(',').Select(int.Parse).ToList();
            m.Rows = CreatePGList(clients, from, to, m.Ratio ?? 0, m.ListType);

            return View(m);
        }

        private List<PGListRow> CreatePGList(List<int> clients, DateTime from, DateTime to, int ratio, int listType)
        {
            var faults = productFaultsDAL.GetProductFaultsForCompanies(clients, from, to);
            var salesData = salesDataDal.GetForCompanyAndPeriod(clients,
                                                                 Utilities.GetMonth21FromDate(from),
                                                                Utilities.GetMonth21FromDate(to));
            var result = new List<PGListRow>();
            foreach (var prod in faults.GroupBy(f => f.fault_cprod))
            {
                var totalFaults = listType == 1 ? prod.Sum(p => p.fault_qty) : prod.Where(p => p.fault_reason == "XPG2").Sum(p => p.fault_qty);
                var sales = salesData.Where(s => s.cprod_id == prod.Key).Sum(s => s.sales_qty);

                if (sales > 0 && totalFaults * 1.0 / sales > (ratio * 1.0 / 100))
                {
                    result.Add(new PGListRow { Sales = sales ?? 0, PgData = totalFaults ?? 0, Product = prod.First().Product, Ratio = totalFaults.Value * 1.0 / sales.Value });
                }

            }
            return result;

        }

        public ActionResult ExportPgList(string clients, int type, int? ratio, int sort)
        {
            var from = Utilities.GetMonthStart(DateTime.Today.AddMonths(-6));
            var to = Utilities.GetMonthEnd(DateTime.Today.AddMonths(-1));
            var clientIdList = clients.Split(',').Select(int.Parse).ToList();

            var model = new ListPGFaultsModel
            {
                ListType = type,
                Ratio = ratio,
                Factories = companyDAL.GetFactories(),
                Sort = sort,
                ShowPopupLink = false
            };
            model.Rows = CreatePGList(clientIdList, from, to, model.Ratio ?? 0, model.ListType);
            Response.AddHeader("Content-Disposition", "attachment;filename=PG alarm list.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return PartialView("_PGTable", model);
        }

        public ActionResult PgProductDetail(int id, bool orders = false)
        {
            var startDate = DateTime.Today;
            var from = Utilities.GetMonthStart(startDate.AddMonths(-11));
            var to = Utilities.GetMonthEnd(startDate);
            var product = custproductsDAL.GetById(id);
            if (product != null)
            {
                var model = new SalesPGFaultsModel
                {
                    SalesData =
                            salesDataDal.GetForPeriod(id, Utilities.GetMonth21FromDate(startDate.AddMonths(-11)),
                                                       Utilities.GetMonth21FromDate(startDate)),
                    Faults = productFaultsDAL.GetProductFaults(id, from, to),
                    cprod_id = id,
                    ClientSide = false,
                    ShowOrders = orders,
                    OrderMonthlyData =
                            orders
                                ? orderHeaderDAL.GetQtyByMonth(product.cprod_code1,
                                                                product.cprod_user ?? 0,
                                                                Utilities.GetMonthStart(
                                                                    DateTime.Today.AddMonths(-11)),
                                                                Utilities.GetMonthEnd(DateTime.Today)) : null
                };

                FillSubTotals(startDate, model.Faults, model.SalesData, model);

                Session[string.Format("PGModel_{0}", id)] = model;

                //CreatePGChart(model, id, "Chart_Pg_cprod");
                return View(model);
            }
            ViewBag.message = "Invalid product id";
            return View("Message");
        }

        public ActionResult GetPgProductDetailChart(int id)
        {
            var model = Session[string.Format("PGModel_{0}", id)] as ISubTotals;
            if (model != null)
            {
                var chart = CreatePGChart(model, 0);

                var ms = new MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "image/jpg");
            }
            return null;
        }
        public ActionResult UpdateSubscription(int? user_id = null, int? returnsid = null)
        {
            var subUser = new Feedback_subscriptions()
            {
                subs_returnid = returnsid,
                subs_useruserid = user_id
            };
            if (user_id != 0)
            {
                if (user_id != null)
                    feedbackSubscriptionsDAL.Create(subUser);
                if (returnsid != null)
                {
                    var result = feedbackSubscriptionsDAL.GetForReturn((int)returnsid);
                    return Json(result);
                }
            }
            return Json("Empty parametar");

        }
        public ActionResult SaveSubscription(Feedback_subscriptions s)
        {
            feedbackSubscriptionsDAL.Create(s);
            s.User = userDAL.GetById(s.subs_useruserid ?? 0);
            var r = returnsDAL.GetById(s.subs_returnid ?? 0);
            if (s.User != null && Settings.Default.Disabled_ForTestingOnStaging)
                SendEmails(r, new List<User> { s.User }, null);
            return Json("OK");
        }
        public ActionResult DeleteSubscription(int? subs_id = null)
        {
            if (subs_id != null)
                feedbackSubscriptionsDAL.Delete((int)subs_id);
            else
                return Json("NO");
            return Json("OK");
        }


        //public ActionResult ClaimsCall
        //[AllowAnonymous]


        //[AllowAnonymous]
        public ActionResult ClaimsInvestigations(CountryFilter countryFilter = CountryFilter.UKOnly, ReportType type = ReportType.Brands, string includedClients = "", string excludedClients = "184", int? brand_id = null, bool pdf = true, bool retunFromClaimsDetail = false)
        {
            //var countryList=new List<string> {"GB","IE"};
            Stopwatch sw = new Stopwatch();
            if (retunFromClaimsDetail && TempData["claimsInvestigation"] != null)
            {
                var model = TempData["claimsInvestigation"] as ClaimsInvestigationsModel;

                return View(model);
            }
            else
            {

                var from = DateTime.Today;
                var topReturnedTo = WebUtilities.LastDayOfPreviousWeek(from);
                var topReturnedFrom = topReturnedTo;
                var useBrands = type == ReportType.Brands;
                var incClients = !string.IsNullOrEmpty(includedClients) ? includedClients.Split(',').Select(int.Parse).ToArray() : null;
                var exClients = !string.IsNullOrEmpty(excludedClients) ? excludedClients.Split(',').Select(int.Parse).ToArray() : null;


                Task data2 = Task.Factory.StartNew(() => returnsDAL.GetTopNTotalsPerProduct(
                    Utilities.GetMonthStart(topReturnedFrom.AddMonths(-5)),
                    topReturnedTo,
                    incClients: incClients,
                    exClients: exClients,
                    groupByBrands: false,
                    top: 9999,
                    useETA: true,
                    extendToEndOfMonthForUnits: true,
                    filterCprodStatus: true).Where(c => c.UnitsShipped > 100).ToList());

                Task data3 = Task.Factory.StartNew(() => returnsDAL.GetTopNTotalsPerProduct(
                    from.AddMonths(-12),
                    from.AddMonths(-6).Date,
                    incClients: incClients,
                    exClients: exClients,
                    groupByBrands: false,
                    top: 9999,
                    filterCprodStatus: true).Where(c => (c.cprod_user == 42 || c.cprod_user == 78 || c.cprod_user == 80 || c.cprod_user == 259 || c.cprod_user == 260 || c.cprod_user == 309)).ToList());

                Task data4 = Task.Factory.StartNew(() => returnsDAL.GetTopNTotalsPerProduct(
                    Utilities.GetMonthStart(topReturnedFrom.AddMonths(-11)),
                    topReturnedTo,
                    incClients: incClients,
                    exClients: exClients,
                    groupByBrands: false,
                    top: 9999,//AnalyticsModel.ReturnsTopRecords,
                    useETA: true,
                    extendToEndOfMonthForUnits: true,
                    filterCprodStatus: true)
                    .Where(c => c.UnitsShipped > 200).ToList());
                Task data5 = Task.Factory.StartNew(() => returnsDAL.GetTopNTotalsPerProduct());
                Task.WaitAll(data2, data3, data4, data5);
                var gData2 = data2 as Task<List<ReturnAggregateDataProduct>>;
                var gData3 = data3 as Task<List<ReturnAggregateDataProduct>>;
                var gData4 = data4 as Task<List<ReturnAggregateDataProduct>>;
                var gData5 = data5 as Task<List<ReturnAggregateDataProduct>>;

                var model = new ClaimsInvestigationsModel
                {

                    //From = new_from,
                    //To = new_to,
                    //CurrentProductSalesData = gData1.Result,
                    Top10ReturnedProducts6m = gData2.Result,
                    Top15ReturnedProduct6ago = gData3.Result,
                    Top10ReturnedProducts12m = gData4.Result,
                    TopReturnedProducts = gData5.Result,

                    ProductInvestigations = new List<Product_investigations>(),
                    Statuses = productInvestigationStatusDAL.GetAll(),
                    ShowPdfButton = pdf


                };


                foreach (var returnedProd in model.Top10ReturnedProducts6m.GroupBy(t => t.cprod_code1))
                {
                    if (model.ProductInvestigations.Count(p => p.cprod_id == returnedProd.First().cprod_id) == 0)
                    {
                        model.ProductInvestigations.AddRange(productInvestigationsDAL.GetClaimInvestigationForProduct(returnedProd.First().cprod_id));
                    }
                }

                foreach (var returnedProd in model.Top10ReturnedProducts12m.GroupBy(t => t.cprod_code1))
                {

                    if (model.ProductInvestigations.Count(p => p.cprod_id == returnedProd.First().cprod_id) == 0)
                    {
                        model.ProductInvestigations.AddRange(productInvestigationsDAL.GetClaimInvestigationForProduct(returnedProd.First().cprod_id));

                    }
                }



                //foreach (var returnedProd in model.TopReturnedProducts.GroupBy(t => t.cprod_code1))
                //{
                //    var units = model.CurrentProductSalesData.Where(s => s.cprod_code == returnedProd.First().cprod_code1).Sum(s => s.numOfUnits);
                //    foreach (var r in returnedProd)
                //    {
                //        r.UnitsShipped = units;
                //    }
                //    if (model.ProductInvestigations.Count(p => p.cprod_id == returnedProd.First().cprod_id) == 0)
                //    {
                //        model.ProductInvestigations.AddRange(Product_investigationsDAL.GetClaimInvestigationForProduct(returnedProd.First().cprod_id));
                //    }
                //}

                TempData["claimsInvestigation"] = model;
                System.Web.HttpRuntime.Cache["pdfInvest"] = model;

                sw.Stop();
                ViewBag.Time = sw.Elapsed;
                //Session["pdfInvest"] = model;
                return View(model);
            }
        }

        public ActionResult ClaimsParam(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {



                char[] delimiter = new char[] { '_', '-' };
                var parts = param.Split(delimiter);

                var param1 = new List<double>();
                var param2 = new List<string>();
                for (int i = 0; i < parts.Length; i += 2)
                {
                    param1.Add(double.Parse(parts[i]));
                }
                for (int i = 1; i < parts.Length; i += 2)
                    param2.Add(parts[i]);



                /* dobijem veličinu niza pošto imam 2 parametra*/

                var chart = GetDefineChart(param1, param2);

                MemoryStream ms = new MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms, "image/jpg");
            }
            return null;
        }


        public Chart GetDefineChart(List<double> param1, List<string> param2)
        {
            var chart = new Chart { Width = 300, Height = 300 };

            //string param = "5|1";
            //var parts = param.Split('|').Select(s => string.IsNullOrEmpty(s) ? (double?)null : double.Parse(s)).ToList();
            var series = new Series();
            series.ChartType = SeriesChartType.Pie;
            series["PieLineColor"] = "Black";
            var legend = new Legend();

            var area = new ChartArea();
            // area.BackColor = Color.LightGray;
            area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);
            Title title = new Title
            {
                //Text = string.Format("{0}", param3),
                Font = new Font("Arial", 10, FontStyle.Regular),
                ShadowColor = Color.FromArgb(32, 0, 0, 0),
                ShadowOffset = 3
            };
            chart.Titles.Add(title);
            chart.Legends.Add(new Legend());
            chart.Series.Add(series);

            for (int i = 0; i < param1.Count(); i++)
            {
                series.Points.Add(new DataPoint
                {
                    LegendText = param2[i].ToString(),
                    YValues = new double[] { param1[i] }
                });
            }


            return chart;
        }

        [AllowAnonymous]
        public ActionResult ClaimsChart(string param1, string param2 = null, string param3 = null)
        {
            // /**/
            if (!string.IsNullOrEmpty(param1))
            {
                var parts = param1.Split('|').Select(s => string.IsNullOrEmpty(s) ? (double?)null : double.Parse(s)).ToList();

                if (param3 == null)
                {
                    param2 = System.Web.HttpRuntime.Cache["pdfCustCode"] as string;
                    if (string.IsNullOrEmpty(param2))
                    {
                        param2 = System.Web.HttpRuntime.Cache["pdfCustCode2"] as string;
                    }
                }
                var partsName = param2.Split('|').Select(s => string.IsNullOrEmpty(s) ? "" : s).ToList();

                var chart = new Chart { Width = 300, Height = 300 };

                //string param = "5|1";
                //var parts = param.Split('|').Select(s => string.IsNullOrEmpty(s) ? (double?)null : double.Parse(s)).ToList();
                var series = new Series();
                series.ChartType = SeriesChartType.Pie;
                series["PieLineColor"] = "Black";
                var legend = new Legend();

                var area = new ChartArea();
                // area.BackColor = Color.LightGray;
                area.Area3DStyle.Enable3D = true;
                chart.ChartAreas.Add(area);
                Title title = new Title
                {
                    Text = string.Format("{0}", param3),
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    ShadowColor = Color.FromArgb(32, 0, 0, 0),
                    ShadowOffset = 3
                };
                chart.Titles.Add(title);
                chart.Legends.Add(new Legend());
                chart.Series.Add(series);
                var total = parts.Sum(s => s.Value);
                for (int i = 0; i < parts.Count(); i++)
                {
                    series.Points.Add(new DataPoint
                    {
                        LegendText = partsName[i].ToString(),
                        YValues = new double[] { parts[i].Value }
                    });
                }

                System.Web.HttpRuntime.Cache["pdfCustCode"] = "";
                //series.Points.Add(new DataPoint {
                //    LegendText = partsName[0],
                //    YValues = new double[] { parts[0].Value }
                //});
                //series.Points.Add(new DataPoint {
                //    LegendText =partsName[1],
                //    YValues = new double[] { parts[1].Value }
                //});


                //chart.Series[0].Points.DataBindXY(xValues, yValues);


                MemoryStream ms = new MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms, "image/jpg");

            }
            else return null;
        }

        //public ActionResult TechSpecPdf(string cprodCode, int id)
        [AllowAnonymous]
        public ActionResult ClaimInvestigationPdf()
        {
            IPdfManager pdfManager = new PdfManager();
            var doc = pdfManager.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("ClaimsInvestigationsPdf"), "scale=0.78, leftmargin=22,rightmargin=22,media=1", "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var fileName = string.Format("Claims.pdf");
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf", fileName);
        }

        [AllowAnonymous]
        public ActionResult ClaimsInvestigationsPdf()
        {
            System.Web.HttpRuntime.Cache.Remove("pdfInvest");
            if (System.Web.HttpRuntime.Cache["pdfInvest"] == null)
                return RedirectToAction("ClaimsInvestigations");
            var model = System.Web.HttpRuntime.Cache["pdfInvest"] as ClaimsInvestigationsModel;

            model.ShowPdfButton = false;
            return View("ClaimsInvestigations", model);
        }
        public ActionResult TechSpecPdf()
        {
            IPdfManager pdfManager = new PdfManager();
            var doc = pdfManager.CreateDocument();



            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("ClaimDetailsPdfNew"), "scale=0.74, leftmargin=22,rightmargin=22,media=1", "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            //var fileName = string.Format("TechSpec.pdf");

            var s = doc.SaveToMemory();
            //return File((byte[])s, "application/pdf", fileName);
            return File((byte[])s, "application/pdf");
            // return View("ClaimDetailsPdf");
        }
        [AllowAnonymous]
        public ActionResult ClaimDetailsPdfNew()
        {
            //var model= {ClaimDetails}.ControllerContext.HttpContext.Session["ClaimsInvestigationsModel"]as ClaimsInvestigationsModel;
            // var model = Session["ClaimsInvestigationsModel"] as ClaimsInvestigationsModel;
            var model = System.Web.HttpRuntime.Cache["pdfData"] as ClaimsInvestigationsModel;
            // return View("ClaimDetailsPdf", model);
            model.ShowPdfButton = false;
            return View("ClaimDetails", model);
        }
        //[OutputCache(Duration=20,VaryByParam="none")]
        public ActionResult ClaimDetails(int cprodId, string cprodCode = "", CountryFilter countryFilter = CountryFilter.UKOnly, ReportType type = ReportType.Brands, string includedClients = "", string excludedClients = "", int? brand_id = null, bool useEta = true, bool pdf = false)
        {
            if (cprodCode != "")
            {
                @ViewBag.CprodCode = cprodCode;
                Session["cprodCode"] = cprodCode;
            }
            if (cprodCode == "")
            {
                cprodCode = Session["cprodCode"] as string;
                if (cprodCode == null)
                    cprodCode = Session["inspectionCprodCode"] as string;
            }
            ViewBag.CprodId = cprodId;
            var from = DateTime.Today;

            var useBrands = type == ReportType.Brands;
            var incClients = !string.IsNullOrEmpty(includedClients) ? includedClients.Split(',').Select(int.Parse).ToArray() : null;
            var exClients = !string.IsNullOrEmpty(excludedClients) ? excludedClients.Split(',').Select(int.Parse).ToArray() : null;

            List<int> cprodIdList = new List<int>() { cprodId };
            List<string> cprodCodeList = new List<string>() { cprodCode };

            // var time = Stopwatch.StartNew();

            var model = new ClaimsInvestigationsModel
            {
                CprodId = cprodId,
                Product = custproductsDAL.GetById(cprodId),

                CurrentProductSalesData = analyticsDAL.GetProductSales(
                    fromDate: Utilities.GetMonthStart(DateTime.Today.AddMonths(-11)),
                    toDate: Utilities.GetMonthEnd(DateTime.Today.AddMonths(0)),
                    countryFilter: countryFilter,
                    brands: true,
                    monthBreakDown: true,
                    incClients: incClients,
                    exClients: exClients,
                    brand_id: brand_id,
                    useETA: true).Where(c => c.cprod_code == cprodCode).ToList(),
                CustomerSales = orderLineExportDal.GetCustomerSummaryForPeriod(
                    from: Utilities.GetMonthStart(DateTime.Today.AddMonths(-11)),
                    to: Utilities.GetMonthEnd(DateTime.Today.AddMonths(0)),
                    brand_user_id: null,
                    cprod_code: cprodCode
                 ),
                CustomerSales6 = orderLineExportDal.GetCustomerSummaryForPeriod(
                   from: Utilities.GetMonthStart(DateTime.Today.AddMonths(-5)),
                   to: Utilities.GetMonthEnd(DateTime.Today.AddMonths(0)),
                   brand_user_id: null,
                   cprod_code: cprodCode
                ),
                CustomerSales3 = orderLineExportDal.GetCustomerSummaryForPeriod(
                   from: Utilities.GetMonthStart(DateTime.Today.AddMonths(-2)),
                   to: Utilities.GetMonthEnd(DateTime.Today.AddMonths(0)),
                   brand_user_id: null,
                   cprod_code: cprodCode),
                //ClaimForProduct12m = ReturnsDAL.GetInPeriod(from.AddMonths(-12), from.AddMonths(0)).Where(c => c.cprod_id == cprodId).ToList(),
                ClaimForProduct12m = new List<Returns>(),
                Clients = companyDAL.GetClients(),
                Statuses = productInvestigationStatusDAL.GetAll(),//ClaimStatusGet(),
                                                                    //ClaimForProductAllSixMonth = ReturnsDAL.GetInPeriod(from.AddMonths(-6)).Where(c => c.cprod_id == cprodId).ToList(),
                                                                    //ClaimForProductAllThreeMonth = ReturnsDAL.GetInPeriod(from.AddMonths(-3)).Where(c => c.cprod_id == cprodId).ToList(),
                ClaimForProductAllSixMonth = new List<erp.Model.Returns>(),
                ClaimForProductAllThreeMonth = new List<erp.Model.Returns>(),
                ShowPdfButton = !pdf,
                PrintModel = pdf,
                Reports = claimsInvestigationReportsDAL.GetForProduct(cprodId),

                //Za graph da prikazuje plavu okomitu crtu koja pokazuje datum prvog izvješća
                Investigation = claimsInvestigationReportsDAL.GetForProduct(cprodId)
            };
            var cprodMast = model.Product.cprod_mast;

            model.ClaimForProduct12m = returnsDAL.GetInPeriod(from.AddMonths(-11), from.AddMonths(0)).Where(c => c.Product.cprod_mast == cprodMast).ToList();
            model.ClaimForProductAllSixMonth = returnsDAL.GetInPeriod(from.AddMonths(-5)).Where(c => c.Product.cprod_mast == cprodMast).ToList();
            model.ClaimForProductAllThreeMonth = returnsDAL.GetInPeriod(from.AddMonths(-2)).Where(c => c.Product.cprod_mast == cprodMast).ToList();


            foreach (var c in model.ClaimForProduct12m)
            {
                c.Images = returnsImagesDAL.GetByReturn(c.returnsid);
            }

            var arr = new int[12];
            model.ArrSalesData = GetSalesData(model.CurrentProductSalesData);
            //model.Arr

            ViewBag.ArrayResault = null;
            //time.Stop();
            //ViewBag.Time = time.Elapsed.ToString(@"m\:s\.ff");



            //System.Web.HttpContext.Current.Cache["pdfData"] = model;
            //System.Web.HttpRuntime.Cache["pdfData"] = model;
            System.Web.HttpRuntime.Cache.Insert("pdfData", model, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));
            // Session["ClaimsInvestigationsModel"] = model;
            //if(pdf){
            //    return View("ClaimDetailsPdf",model);
            //}
            // else{
            return View("ClaimDetails", model);
            //}
        }


        public ActionResult AllClaimsExl(int cprod_id = 0)
        {
            //collect products
            //List<int> prod_ids = new List<int>();
            //foreach (var key in Request.Form.Keys)
            //{
            //    string k = key.ToString();
            //    if (!k.StartsWith("chk_")) continue;
            //    prod_ids.Add(int.Parse(k.Replace("chk_", "")));
            //}
            //var rows = ReturnsDAL.GetAnalytics(prod_ids);
            var model = new ClaimsAllModel
            {
                Claims = returnsDAL.GetAllForProduct(cprod_id),
                Clients = companyDAL.GetClients(),
                PrintMode = true
            };

            Response.AddHeader("Content-Disposition", "attachment;filename=Claims.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return View("ClaimsAll", model);
        }


        //[AllowAnonymous]
        public ActionResult AllClaimsForProduct(int cprod_id, bool printMode = false)
        {
            var model = new ClaimsAllModel
            {
                Claims = returnsDAL.GetAllForProduct(cprod_id),
                // Claims = ReturnsDAL.GetInPeriod(),
                Clients = companyDAL.GetClients(),
                PrintMode = printMode
            };

            foreach (var c in model.Claims)
            {
                c.Images = returnsImagesDAL.GetByReturn(c.returnsid);
            }



            return View("ClaimsAll", model);
        }

        
        [AllowAnonymous]
        public ActionResult InspectionFi(int id)
        {
            //var m = new InspectionForProduct
            //{
            //    Inspection=
            //};
            return RedirectToAction("ReportPdf", "Inspection", new { id = id });

        }
        [AllowAnonymous]
        public ActionResult InspectionLi(int id)
        {
            return RedirectToAction("LoadingReportPdf", "Inspection", new { id = id });
        }

        // [AllowAnonymous]

        //[HttpPost]
        //public JsonResult ClaimDetails(HttpPostedFileBase file)
        //{
        //    var service = new ImageService();
        //    byte[] data = new byte[file.ContentLength];
        //    file.InputStream.Read(data, 0, file.ContentLength);
        //    var image = service.CreateImage(file.FileName,file.ContentType,data);
        //    JsonResult res = Json(new { image.Id });
        //    res.ContentType = "text/html";
        //    return res;

        //}

        //[OutputCache(Duration=0)]
        //public ActionResult ById(string id)
        //{
        //    var service = new ImageService();
        //    var image= service.GetImage(id);
        //    return new FileStreamResult(new MemoryStream(image.Data),image.ContentType);
        //}




        [AllowAnonymous]
        private int[] GetSalesData(List<ProductSales> listSalesData)
        {

            var result = new int[12];
            if (listSalesData != null && listSalesData.Count() > 0)
            {
                //probati i>result.count()*-1 (tako da dobijem -12)
                //  for (int i = 11; i >= 0; i--)
                for (int i = 0; i <= 11; i++)
                {
                    var month = DateTime.Today.AddMonths(i * -1).ToShortDateString().Substring(3, 2);
                    var year = DateTime.Today.AddMonths(i * -1).Year.ToString().Substring(2, 2);
                    var yearMonth = year + "" + month;
                    var flag = true;

                    foreach (var p in listSalesData.Where(c => c.Month22.ToString() == yearMonth))
                    {
                        result[i] = p.Month22.ToString() == yearMonth ? p.numOfUnits : 0;
                        flag = false;
                    }
                    if (flag)
                    {
                        result[i] = 0;
                    }
                }
            }
            return result;
        }


        [AllowAnonymous]
        public PartialViewResult StatusDetails(int cprodId)
        {
            var model = new ClaimsInvestigationsModel
            {
                ProductInvestigations = productInvestigationsDAL.GetClaimInvestigationForProduct(cprodId),
                ProductInvestImages = productInvestigationImagesDAL.GetForProduct(cprodId),
                ImageInvestigations = new List<Product_investigation_images>(),
                Statuses = productInvestigationStatusDAL.GetAll(),
                //CurrentProductSalesData=

            };

            foreach (var inv in model.ProductInvestigations)
            {
                foreach (var img in model.ProductInvestImages.Where(c => c.investigation_id == inv.id))
                {
                    model.ImageInvestigations.Add(new Product_investigation_images()
                    {
                        investigation_id = img.investigation_id,
                        image_name = img.image_name
                    });
                }
            }

            return PartialView("_StatusHistory", model);
        }
        // [AllowAnonymous]
        [AllowAnonymous]

        public List<StatusClaim> ClaimStatusGet()
        {
            List<StatusClaim> statusClaim = new List<StatusClaim>(){
                new StatusClaim{ Id=0,CurrentStatus="No Action"},
                new StatusClaim{ Id=1, CurrentStatus="Under investigation"},
                new StatusClaim{ Id=2, CurrentStatus="Being monitored"}
            };

            return statusClaim;
        }


        public ActionResult InvestigationImage(int id)
        {

            var images = productInvestigationImagesDAL.GetForInvestigation(id);

            var model = new Product_investigations();
            model = productInvestigationsDAL.GetById(id);

            ViewBag.Images = images;


            return View(model);

        }

        public ActionResult ImageUpdate(int cprodId = 0, int invId = 0)
        {
            var model = new ClaimsInvestigationsModel
            {
                InvestigationId = invId,
                ListProductImages = productInvestigationImagesDAL.GetForProduct(cprodId),
                ListInvestigationImages = productInvestigationImagesDAL.GetForInvestigation(invId)
            };
            return PartialView("_Images", model);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImageUpdate(HttpPostedFileBase file, int cprodId)
        {
            var model = new ClaimsInvestigationsModel
            {
                ProductImages = new Product_investigation_images()
            };

            if (file.ContentLength > 0)
            {
                var filenName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/updates"), filenName);

                model.ProductImages.image_name = filenName;
                model.ProductImages.cprod_id = cprodId;
                model.ProductImages.investigation_id = 130;

                file.SaveAs(path);
                productInvestigationImagesDAL.Create(model.ProductImages);
            }
            return RedirectToAction("ImageUpdate", new { cprodId = cprodId });
        }

        [HttpPost]
        [AllowAnonymous]
        public ContentResult UploadFiles(int cprod_id = 0, int id = 0)
        {
            var r = new ClaimsInvestigationsModel
            {
                ListProductImages = new List<Product_investigation_images>(),
                ProductImages = new Product_investigation_images()
            };

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                string savedFileName = Path.Combine(Server.MapPath("~/Images/updates"), Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName); // Save the file

                r.ListProductImages.Add(new Product_investigation_images()
                {
                    image_name = hpf.FileName,
                    cprod_id = cprod_id,
                    investigation_id = id
                });

                productInvestigationImagesDAL.Create(r.ListProductImages.First());

            }
            //Returns json
            return Content("{\"name\":\"" + r.ListProductImages[0].image_name + "\"}", "application/json");
        }

        [HttpPost]
        public ActionResult DeleteFile(int id = 0, int cprodId = 0, string name = "")
        {

            productInvestigationImagesDAL.Delete(id);
            string filePath = Path.Combine(Server.MapPath("/images/updates/"), name);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            //return RedirectToAction("ImageUpdate", new { cprodId=cprodId});
            return Content("{\"name\":\"" + name + "\"}", "application/json");
        }

        

        public ActionResult DetailedReportAll(string typeReport = "")
        {
            if (typeReport == "ago")
            {
                ViewBag.TypeReport = "ago";
                return View("DetailedReportsAll");
            }
            else
            {
                ViewBag.TypeReport = "";
                return View("DetailedReportsAll");
            }
        }
        /// <summary>
        /// Claim investigation, detaljniji prikaz za svaku stavku sa prve stranice može imati par vrsta prikaza
        /// </summary>
        /// <param name="param"></param>
        /// <param name="typeView">0- standardni , 1- partialView</param>
        /// <param name="typeReport"></param>
        /// <returns></returns>
        public ActionResult DetailedReport(int param = 0, int typeView = 0, string typeReport = "")
        {

            /*Dohvati top 15 proizvoda koji se pokazuju u tablici zatim u View pošalji pojedinačni detail*/
            var countryFilter = CountryFilter.UKOnly;
            var type = ReportType.Brands;
            var includedClients = "";
            var excludedClients = "";
            int? brand_id = null;

            var from = DateTime.Today;
            var useBrands = type == ReportType.Brands;
            var incClients = !string.IsNullOrEmpty(includedClients) ? includedClients.Split(',').Select(int.Parse).ToArray() : null;
            var exClients = !string.IsNullOrEmpty(excludedClients) ? excludedClients.Split(',').Select(int.Parse).ToArray() : null;
            //var useEta = true;
            var new_from = from.AddDays(-6).Date;
            var new_to = from;
            var topTo = WebUtilities.LastDayOfPreviousWeek(from);

            //  var from = DateTime.Today;


            var topReturnedTo = WebUtilities.LastDayOfPreviousWeek(from);
            var topReturnedFrom = topReturnedTo;
            var idstop15 = new List<ListFotGraphs>();
            if (typeReport == "")
            {
                idstop15 = Session["inv_top15Ids"] as List<ListFotGraphs>;
                ViewBag.Products = "";
            }
            else
            {
                idstop15 = Session["inv_top15IdsAgo"] as List<ListFotGraphs>;
                ViewBag.Products = "ago";

            }
            if (idstop15 == null)
            {
                return RedirectToAction("ClaimsInvestigations");
            }
            var model = new DetailedReport
            {
                //Top15ReturnedProducts6m = ReturnsDAL.GetTopNTotalsPerProduct(from.AddMonths(-6), topTo, incClients: incClients, exClients: exClients, groupByBrands: false, top: AnalyticsModel.ReturnsTopRecords),

                Param = param
            };

            model.CurrentProductSalesData = analyticsDAL.GetProductSales(
           fromDate: Utilities.GetMonthStart(DateTime.Today.AddMonths(-11)),
           toDate: Utilities.GetMonthEnd(DateTime.Today.AddMonths(0)),
           countryFilter: countryFilter,
           brands: true,
           monthBreakDown: true,
           incClients: incClients,
           exClients: exClients,
           brand_id: brand_id,
           useETA: true).Where(c => c.cprod_code == idstop15[param].CprodCode /*cprodCode*/).ToList();

            if (param <= 13 && param >= 0) { ViewBag.ShowLink = "inline"; }
            else { ViewBag.ShowLink = "none"; }
            model.ClaimForProduct12m = returnsDAL.GetInPeriod(from.AddMonths(-12), from.AddMonths(0)).Where(c => c.cprod_id == idstop15[param].CprodId).ToList();/* Dohvaćam proizvode redak 2 u tablici*/

            model.ClaimForProductAllSixMonth = returnsDAL.GetInPeriod(from.AddMonths(-6)).Where(c => c.cprod_id == idstop15[param].CprodId).ToList();

            model.Investigation = claimsInvestigationReportsDAL.GetForProduct(cprod_id: idstop15[param].CprodId, reports: true);
            //if(model.Investigation.Select(c=>c.date_created).Count()>0)
            //ViewBag.DateFirstRepoert= (int)model.Investigation.Last().date_created.Value.Month ;

            var arr = new int[12];
            model.ArrSalesData = GetSalesData(model.CurrentProductSalesData);

            model.ProductInvestigations = productInvestigationsDAL.GetClaimInvestigationForProduct(idstop15[param].CprodId/*model.Top15ReturnedProducts6m[param].cprod_id*/);

            //Vraća View koji ia Paging od 1 do 15 sa grafom i tablicom...
            if (typeView == 0)
            {

                //Ako nema prodatih proizvoda napravi jedan prazan objekt sa osnovnim podacima koji su potrebni za prkaz u View inače null exeption
                if (model.CurrentProductSalesData.Count() < 1)
                    model.CurrentProductSalesData = GetEmpty(model.CurrentProductSalesData, idstop15[param].CprodCode, idstop15[param].CprodId);

                return View("DetailedReport", model);
            }
            else
            {
                if (model.CurrentProductSalesData.Count() < 1)
                    model.CurrentProductSalesData = GetEmpty(model.CurrentProductSalesData, idstop15[param].CprodCode, idstop15[param].CprodId);


                return PartialView("_DetailedReport", model);                     //PartialView("Empty") ;
            }

        }
        /// <summary>
        /// Definiraj prazan Array objekt - koji je sa jednim elemntom - tako da definiram niz kojise ne definira jer nije prodan niti jedan proizvod
        /// </summary>
        /// <returns>Vraća niz sa jednim elementom, koji ima default postavke, </returns>
        public List<ProductSales> GetEmpty(List<ProductSales> one, string cprodCode, int cprodId)
        {
            //nema prodanih proizvoda pa ostane nedifiniran zato definiram jedan Array koji ima jedan osnovni element
            one.Add(
                        new ProductSales
                        {
                            Amount = 0.0,
                            brand_code = "",
                            cprod_code = cprodCode,
                            cprod_name = custproductsDAL.GetById(cprodId).cprod_name,
                            numOfUnits = 0,
                            Req_eta = DateTime.Now
                        }

              );
            return one;
        }

        public ActionResult DetailedReportAllAgo()
        {
            return View("DetailedReportsAllAgo");
        }

        

        [AllowAnonymous]
        public ActionResult DetailReportChart(string param1, string param2 = "", int param3 = 830, int param4 = 0)
        {

            var parts = param1.Split('|').Select(s => string.IsNullOrEmpty(s) ? (double?)null : double.Parse(s)).ToList();

            var partsDate = param2.Split('|').Select(d => string.IsNullOrEmpty(d) ? (DateTime?)null : Convert.ToDateTime(d)).ToList();

            var firstDate = DateTime.Today.AddMonths(-12);

            var paramForMap = new List<ParamForMap>();


            for (int i = 0; i < 12; i++)
            {
                paramForMap.Add(new ParamForMap { Date = partsDate[i], Value = parts[i] == null ? 0 : parts[i] });
            }



            var chart = new Chart { Width = param3, Height = 200 };
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

            Title title = new Title
            {
                //Text = cprodCode + " - sell",
                Text = string.Format("{0}", "Six month rolling return"),
                Font = new Font("Arial", 14, FontStyle.Regular),
                ShadowColor = Color.FromArgb(32, 0, 0, 0),
                ShadowOffset = 3
            };
            //chart.Titles.Add(title);

            var series = new Series
            {
                ChartType = SeriesChartType.Spline,
                BorderWidth = 3,
                ShadowOffset = 3,

            };
            chart.Series.Add(series);

            //DateTime month = (DateTime)TempData["inv_date"];
            //int i=0;
            //if(month != null)
            //    i =(int)month.Month;
            //else
            //     i = 12;
            DataPoint anchorPoint = null;

            /*foreach (var prod in parts)*/
            foreach (var prod in paramForMap)
            {

                chart.Series[0].Points.AddXY(prod.Date.Value, prod.Value == 100 ? 1 : prod.Value);

                chart.Series[0].Points.Last().LabelFormat = "{0.00} %";

                chart.Series[0].MarkerStyle = MarkerStyle.Circle;
                chart.Series[0].MarkerSize = 7;
                chart.Series[0].MarkerBorderColor = Color.Black;
                chart.Series[0].MarkerBorderWidth = 1;

                if (prod.Date.Value.Month == param4)
                {
                    anchorPoint = chart.Series[0].Points.Last();
                }
                chart.Series[0].MarkerColor = Color.FromArgb(167, 168, 163);
                chart.Series[0].XValueType = ChartValueType.Date;
                //i++;
            }
            TempData["inv-show"] = "none";
            //ViewBag.Show="none";
            chart.ChartAreas.Add(CreateChartArea("Six month roll return"));
            if (anchorPoint != null)
            {
                AddAnnotation(chart, anchorPoint);
                // ViewBag.Show = "inline";
                TempData["inv-show"] = "inline";
            }




            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "image/jpg");
        }

        private void AddAnnotation(Chart chart, DataPoint point = null)
        {
            var line = new VerticalLineAnnotation
            {
                LineDashStyle = ChartDashStyle.Dot,
                AxisX = chart.ChartAreas[0].AxisX,
                AxisY = chart.ChartAreas[0].AxisY,
                ClipToChartArea = chart.ChartAreas[0].Name,
                IsInfinitive = true,
                LineWidth = 4,
                LineColor = Color.FromArgb(255, 102, 102),
                Name = "This is name"
            };

            line.SetAnchor(point);

            chart.Annotations.Add(line);
        }

        [AllowAnonymous]
        private ChartArea CreateChartArea(string cprodCode)
        {

            ChartArea chartArea = new ChartArea();
            chartArea.Name = "Ovo je za proizvod " + "-" + " sales";

            //chartArea.AxisX.Interval = 1;
            chartArea.AxisX.IsMarginVisible = false;
            //chartArea.AxisX.IntervalOffsetType = DateTimeIntervalType.Months;
            //chartArea.AxisX.IntervalType = DateTimeIntervalType.Months;
            //chartArea.AxisX.LabelStyle.Format = "MMMY";
            chartArea.AxisX2.Title = cprodCode + " sell";
            chartArea.AxisX2.TitleFont = new Font("Arial", 14, FontStyle.Regular);
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisX.MinorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.Format = "MMM yy";
            //chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(167, 168, 163);
            //chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            //chartArea.AxisX.MajorTickMark.Interval = 1;
            //chartArea.AxisX.MajorTickMark.IntervalType = DateTimeIntervalType.Months;

            //chartArea.AxisX.MajorTickMark.LineColor = Color.FromArgb(167, 168, 163);
            //chartArea.AxisY.MajorTickMark.LineDashStyle = ChartDashStyle.Solid;
            // chartArea.AxisY.MajorTickMark.LineColor = Color.FromArgb(65, 140, 240);

            chartArea.AxisY.LabelStyle.Format = "P0";

            /*lijeva strana grafa*/
            chartArea.AxisY.LineColor = Color.FromArgb(65, 140, 240);
            chartArea.AxisY.LineDashStyle = ChartDashStyle.Solid;
            chartArea.AxisY.LineWidth = 4;
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(65, 140, 240);
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chartArea.AxisY.MajorTickMark.LineDashStyle = ChartDashStyle.Solid;
            chartArea.AxisY.MajorTickMark.LineColor = Color.FromArgb(65, 140, 240);
            chartArea.AxisY.MajorTickMark.LineWidth = 2;



            ////chartArea.AxisY2.
            ///*desna strana grafa*/
            //chartArea.AxisY2.LineColor = Color.FromArgb(230, 174, 84);
            //chartArea.AxisY2.LineWidth = 4;
            //chartArea.AxisY2.MajorGrid.LineColor = Color.FromArgb(230, 174, 84);
            //chartArea.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            ////chartArea.AxisY2.MajorTickMark.Interval = 5;
            //chartArea.AxisY2.MajorTickMark.LineColor = Color.FromArgb(230, 174, 84);
            //chartArea.AxisY2.MajorTickMark.LineWidth = 2;
            //chartArea.AxisX2.IsMarginVisible = false;


            //chartArea.AxisY.Title = "unit sales";
            //chartArea.AxisY.TitleFont = new Font("Arial", 14, FontStyle.Regular);

            //chartArea.AxisY2.Title = "accepted claims";
            //chartArea.AxisY2.TitleFont = new Font("Arial", 14, FontStyle.Regular);



            return chartArea;
        }



        public ActionResult DetailUpdate(int currStatus = 0, string textfield = "", int idCprod = 0, string cprodCode = "")
        {

            var m = new ClaimsInvestigationsModel
            {
                StatusDetail = new Product_investigations()
            };
            m.StatusDetail.cprod_id = idCprod;
            m.StatusDetail.date = DateTime.Now;
            m.StatusDetail.monitored_by = accountService.GetCurrentUser().username.ToString();
            m.StatusDetail.status = (int)currStatus;
            m.StatusDetail.comments = HttpUtility.HtmlDecode(textfield);
            /*Update */
            if (ModelState.IsValid)
            {
                productInvestigationsDAL.Create(m.StatusDetail);
            }
            return RedirectToAction("StatusDetails", new { cprodId = idCprod });
        }

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public ActionResult DynamicJavaScript();
        [AllowAnonymous]
        public ActionResult EditDetail(int invId)
        {
            var images = productInvestigationImagesDAL.GetForInvestigation(invId);
            Session["invId"] = invId;
            ViewBag.Images = images;
            var model = new Product_investigations();
            model = productInvestigationsDAL.GetById(invId);



            return View("_StatusHistoryEdit", model);
        }
        [AllowAnonymous]
        public ActionResult EditDetailInv(int invId)
        {
            var images = productInvestigationImagesDAL.GetForInvestigation(invId);
            //Session["invId"] = invId;
            ViewBag.Images = images;
            var model = new Product_investigations();
            model = productInvestigationsDAL.GetById(invId);

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public PartialViewResult EditDetail(Product_investigations model)
        {
            model.monitored_by = accountService.GetCurrentUser().username.ToString();
            model.comments = HttpUtility.HtmlDecode(model.comments);
            productInvestigationsDAL.Update(model);

            var m = new ClaimsInvestigationsModel
            {
                ProductInvestigations = productInvestigationsDAL.GetClaimInvestigationForProduct(model.cprod_id),
                ImageInvestigations = productInvestigationImagesDAL.GetForProduct(model.cprod_id)
            };
            return PartialView("_StatusHistory", m);
        }



        /**
		 * We are create empty report
		 */
        [AllowAnonymous]
        [HttpGet]
        public ActionResult CreateReport(string cprodCode, int cprod_id)
        {
            ViewBag.Cprod_id = cprod_id;


            var model = new CreateReport
            {
                CprodCode = cprodCode,
                Report = new Claims_investigation_reports(),
                Action = new Claims_investigation_reports_action(),
                Image = new Claims_investigation_report_action_images()
            };
            model.Edit = false;
            model.Report.unique_id = (int) claimsInvestigationReportsDAL.GetLastAddedReport().unique_id + 1;
            model.Report.cprod_id = cprod_id;
            model.Report.investigation = "";
            model.Report.extras = "";
            // model.date_modify = null;
            model.Report.date_created = DateTime.Today;
            model.Report.created_by = accountService.GetCurrentUser().username.ToString();


            claimsInvestigationReportsDAL.Create(model.Report);
            return View(model);
        }

        /*
		 * Update report with edit data
		 *
		 */
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateReport(CreateReport m)
        {
            var reportEdit = m.Edit;

            var mo = new ClaimInvestigatinReport
            {
                //Reports = new Claims_investigation_reports(),
                Report = claimsInvestigationReportsDAL.GetLastAddedReport()
            };
            mo.Report.unique_id = m.Report.unique_id;
            mo.Report.cprod_id = m.Report.cprod_id;
            mo.Report.investigation = HttpUtility.HtmlDecode(m.Report.investigation);
            mo.Report.extras = HttpUtility.HtmlDecode(m.Report.extras);

            mo.Report.modify_by = accountService.GetCurrentUser().username.ToString();
            mo.Report.date_modify = DateTime.Today;

            if (ModelState.IsValid)
            {
                claimsInvestigationReportsDAL.Create(mo.Report);
            }
            //Claims_investigation_reportsDAL.Update(mo.Report);


            if (!reportEdit)
                mo.Report = claimsInvestigationReportsDAL.GetLastAddedReport();
            else
                mo.Report = claimsInvestigationReportsDAL.GetById(m.Report.unique_id);

            var cprodCode = m.CprodCode;
            return RedirectToAction("ClaimDetails", new { cprodCode = cprodCode, cprodId = mo.Report.cprod_id });
        }


        /* EDITING REPORT */
        public ActionResult EditReport(string cprodCode, int report_id)
        {
            var model = new CreateReport
            {
                CprodCode = cprodCode,
                Edit = true,
                Report = claimsInvestigationReportsDAL.GetById(report_id),
                Action = new Claims_investigation_reports_action()
                //Actions =Claims_investigation_reports_actionDAL.GetActionsForReports(report_id)
            };

            return View("CreateReport", model);
        }
        [HttpGet]
        public ActionResult CreateAction(/*ClaimInvestigatinReport m*/int id)
        {

            var model = new ClaimInvestigatinReport
            {
                ReportAction = new Claims_investigation_reports_action()
            };
            model.ReportAction.report_id = id;
            model.ReportAction.comments = "";
            // model.ReportAction.report_id=m.Report.unique_id;
            // model.ReportAction.comments=m.ReportAction.comments;
            if (ModelState.IsValid)
            {
                claimsInvestigationReportsActionDAL.Create(model.ReportAction);
            }
            ViewBag.Id = id;
            //return RedirectToAction("ReportActionDetails", new { unique_id=model.ReportAction.report_id});
            //return "success";
            return Content("{\"name\":\"" + id + "\"}", "application/json");
            //return Content("OK");
        }

        public ActionResult GetActionForReport(int unique_id)
        {

            var model = new CreateReport
            {
                Report = claimsInvestigationReportsDAL.GetById(unique_id),
                Action = claimsInvestigationReportsActionDAL.GetLastAddedAction()
            };

            if (model.Action.report_id == unique_id)
            {
                return PartialView("_ActionForReport", model);
            }
            else if (model.Action.report_id != unique_id)
            {
                model.Action.report_id = model.Report.unique_id;
                return PartialView("_ActionForReport", model);
            }
            else
            {
                ViewBag.Error = "Out off scope ...";
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult UploadActionForReport(int unique_id, string comment, int id = 0)
        {
            var m = new CreateReport
            {
                Action = new Claims_investigation_reports_action()
            };
            if (id == 0)
                m.Action.id = claimsInvestigationReportsActionDAL.GetLastAddedAction().id;
            else
                m.Action.id = id;
            m.Action.report_id = unique_id; //o.Action.report_id;
            m.Action.comments = comment; // o.Action.comments;

            claimsInvestigationReportsActionDAL.Update(m.Action);
            return RedirectToAction("ReportActionDetails", new { unique_id = unique_id/*o.Action.report_id*/});
        }

        public ActionResult ReportActionDetails(int unique_id = 0, int action_id = 0)
        {
            
            var model = new ReportActionDetails
            {
                ReportActions = claimsInvestigationReportsActionDAL.GetActionsForReports(unique_id).ToList(),
                //ImageActions = new List<Claims_investigation_report_action_images>()
                ImageActions = claimsInvestigationReportActionImageDAL.GetAll()
            };
            

            return PartialView("_ReportActions", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ContentResult ReportActionUploadFilesZ(int actionId = 0)
        {
            var r = new ClaimsInvestigationsModel
            {
                ListProductImages = new List<Product_investigation_images>(),
                ProductImages = new Product_investigation_images()
            };

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                string savedFileName = Path.Combine(Server.MapPath("~/Images/updates"), Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName); // Save the file

                r.ListProductImages.Add(new Product_investigation_images()
                {
                    image_name = hpf.FileName,
                    //cprod_id = cprod_id,
                    //investigation_id = id
                });

                productInvestigationImagesDAL.Create(r.ListProductImages.First());

            }
            //Returns json
            return Content("{\"name\":\"" + r.ListProductImages[0].image_name + "\"}", "application/json");
        }

        public ActionResult ReportActionGetImages(int actionId)
        {

            var model = new CreateReport
            {
                Images = claimsInvestigationReportActionImageDAL.GetImagesForAction(actionId)
            };
            return PartialView("_ActionImages", model);

        }

        [HttpPost]
        [AllowAnonymous]
        public ContentResult ReportActionUploadFiles(int actionId = 0)
        {
            var r = new ReportActionDetails
            {
                ImageActions = new List<Claims_investigation_report_action_images>(),
                IdLastAction = claimsInvestigationReportsActionDAL.GetLastAddedAction()
                //ProductImages = new Product_investigation_images()
            };

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                string savedFileName = Path.Combine(Server.MapPath("~/Images/updates"), Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName); // Save the file

                r.ImageActions.Add(new Claims_investigation_report_action_images
                {
                    // id= je autonumber
                    // action_id= /*dohvati action_id*/
                    action_id = actionId,
                    name = hpf.FileName,

                });

                claimsInvestigationReportActionImageDAL.Create(r.ImageActions.First());

            }
            //Returns json
            return Content("{\"name\":\"" + r.ImageActions[0].name + "\"}", "application/json");
        }


        [HttpPost]
        public ActionResult ReportActionFileTitle(int id = 0, string title = "")
        {
            //var m = new ClaimInvestigationReportImage
            //{
            //    Image=new Claims_investigation_report_action_images()
            //};
            var m = new Claims_investigation_report_action_images();
            m.id = id;
            m.image_title = title;
            claimsInvestigationReportActionImageDAL.Update(m);

            var getTitle = claimsInvestigationReportActionImageDAL.GetAll();
            var newTitle = "";
            foreach (var t in getTitle.Where(i => i.id == id))
            {
                newTitle = t.image_title;
            }

            //return Json(string.Format("Ovo je novi naslov: {0}"),id.ToString());
            return Json(newTitle);
        }


        [HttpPost]
        public ActionResult DeleteFileReportActionImage(int id = 0, int action_id = 0, string name = "")
        {

            claimsInvestigationReportActionImageDAL.Delete(id);
            string filePath = Path.Combine(Server.MapPath("/images/updates/"), name);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            //return RedirectToAction("ImageUpdate", new { cprodId=cprodId});
            return Content("{\"name\":\"" + name + "\"}", "application/json");
        }

        [AllowAnonymous]
        public ActionResult GetClaimRawData(DateTime? dateFrom, DateTime? dateTo, int? brand_id = null, int? reason_id = null, int? decision_id = null, string clients = ClaimsApiController.summaryDefaultClients)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            var reasons = unitOfWork.ReturnCategoryRepository.Get().ToList();
            var reason = reasons.FirstOrDefault(rc => rc.returncategory_id == reason_id)?.category_code;
            ViewBag.Reasons = reasons;
            ViewBag.Data = unitOfWork.ReturnRepository
                .GetQuery(r => r.request_date >= dateFrom && r.request_date <= dateTo && r.status1 == 1 && clientIds.Contains(r.client_id) && r.claim_type != 5
                            && (brand_id == null || r.Product.brand_id == brand_id)
                            && (decision_id == null || r.decision_final == decision_id)
                            && (reason == null || r.reason == reason)
                , includeProperties: "Product.Brand,Client,DecisionFinal,Product.MastProduct.Factory")
                .ToList();
            Response.AddHeader("Content-Disposition", "attachment;filename=ClaimData.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return View();
        }

        [AllowAnonymous]
        public ActionResult ProcessCorrectiveActionMails(Guid statsKey)
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                Cust_products product;

                var corrrectiveactionsEvents = unitOfWork.ReturnsEventsRepository.Get(r => (r.event_type == (int)ReturnEventType.Recheck || r.event_type == (int)ReturnEventType.CorrectiveActionCreate) && r.mail_sent == false).ToList();

                var corrrectiveactionsEventsIds = corrrectiveactionsEvents.Select(r => r.return_id).ToList();

                var correctiveActions = unitOfWork.ReturnRepository.Get(r => corrrectiveactionsEventsIds.Contains(r.returnsid), includeProperties: "Products,AssignedQCUsers.User,ReturnsQCUsers.User,Product,Subscriptions,Images").ToList();

                foreach (var r in correctiveActions)
                {
                    var ev = corrrectiveactionsEvents.Where(e => e.return_id == r.returnsid).FirstOrDefault();

                    switch (ev.event_type)
                    {
                        case (int)ReturnEventType.Recheck:
                            SendRecheckUpdated(r);
                            break;
                        case (int)ReturnEventType.CorrectiveActionCreate:

                            var recipients = CreateFeedbackSubscriptions(r, out product,null,unitOfWork);

                            if (recipients != null)
                                SendEmails(r, recipients, r.Product);

                            break;
                        default:
                            break;
                    }

                    ev.mail_sent = true;

                    unitOfWork.Save();
                }

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NOT-OK", JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult ReturnsImagesFilesReport(Guid statsKey,int? claimType = null, DateTime? dateFrom = null)
        {
            List<Returns> problematicReturns = new List<erp.Model.Returns>();

            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                var correctiveActions = unitOfWork.ReturnRepository.Get(r => (r.claim_type == claimType || claimType == null) && (r.request_date > dateFrom || dateFrom == null ), includeProperties: "Images").ToList();

                var caWithImages = correctiveActions.Where(ca => ca.Images != null && ca.Images.Count > 0).ToList();

                foreach (var cor in correctiveActions)
                {
                    bool missingImage = false;

                    if (cor.Images != null && cor.Images.Count > 0)
                    {
                        foreach (var img in cor.Images)
                        {
                            if (!System.IO.File.Exists(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), img.return_image)))
                            {
                                missingImage = true;
                                break;
                            }
                        }
                    }

                    if (missingImage)
                        problematicReturns.Add(cor);
                }

                ViewBag.Data = problematicReturns;
                Response.AddHeader("Content-Disposition", "attachment;filename=CAWithImageProblem.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return View("ReturnsImagesFilesReport");

                //return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NOT-OK", JsonRequestBehavior.AllowGet);
            }
        }
    }
	class ParamForMap
	{
		public DateTime? Date { get; set; }
		public double? Value { get; set; }
	}
}
