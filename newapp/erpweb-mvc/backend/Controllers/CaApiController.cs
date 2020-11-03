using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using backend.Properties;
using ASPPDFLib;
using System.Reflection;
using System.Web.Security;
using RazorEngine;
using Utilities = company.Common.Utilities;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class CaApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFeedbackCategoryDAL feedbackCategoryDAL;
        private readonly IInspectionsDAL inspectionsDAL;
        private readonly IReturnsImportanceDAL returnsImportanceDAL;
        private readonly IInspectionLinesTestedDal inspectionLinesTestedDal;
        private readonly ICustproductsDAL custproductsDAL;
        private readonly IOrderHeaderDAL orderHeaderDAL;
        private readonly IEmailRecipientsDAL emailRecipientsDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IReturnCategoryDAL returnCategoryDAL;
        private readonly IAdminPermissionsDal adminPermissionsDal;
        private readonly IUserDAL userDAL;
        private readonly IAccountService accountService;
        private readonly IMailHelper mailHelper;

        public CaApiController(IUnitOfWork unitOfWork, IFeedbackCategoryDAL feedbackCategoryDAL, IInspectionsDAL inspectionsDAL,
            IReturnsImportanceDAL returnsImportanceDAL, IInspectionLinesTestedDal inspectionLinesTestedDal, ICustproductsDAL custproductsDAL,
            IOrderHeaderDAL orderHeaderDAL, IEmailRecipientsDAL emailRecipientsDAL, ICompanyDAL companyDAL, IReturnCategoryDAL returnCategoryDAL,
            IAdminPermissionsDal adminPermissionsDal, IUserDAL userDAL, IAccountService accountService, IMailHelper mailHelper)
        {
            this.unitOfWork = unitOfWork;
            this.feedbackCategoryDAL = feedbackCategoryDAL;
            this.inspectionsDAL = inspectionsDAL;
            this.returnsImportanceDAL = returnsImportanceDAL;
            this.inspectionLinesTestedDal = inspectionLinesTestedDal;
            this.custproductsDAL = custproductsDAL;
            this.orderHeaderDAL = orderHeaderDAL;
            this.emailRecipientsDAL = emailRecipientsDAL;
            this.companyDAL = companyDAL;
            this.returnCategoryDAL = returnCategoryDAL;
            this.adminPermissionsDal = adminPermissionsDal;
            this.userDAL = userDAL;
            this.accountService = accountService;
            this.mailHelper = mailHelper;
        }

        /*API Inspection simple*/
        [Route("api/claims/test")]
        [HttpGet]
        public string Test()
        {
            return "Test ok: ";
        }
        [Route("api/claims/getReference")]
        [HttpGet]
        public string GetReference()
        {
            WebUtilities.ClearTempFiles();

            var date = DateTime.Now.ToString("yyyMMdd");
            var num = unitOfWork.ReturnRepository.Get(c => c.return_no.Contains("-CA") && c.return_no.Contains(date)).Count() + 1;
            /*
             * xx- facktory code
             * CAxx-x sequence -x return category type
             * xxxx - date
             */

            return $"XX-CA{num:00}X-{date}-XX";
        }

        [Route("api/claims/getRefSequence")]
        [HttpGet]
        public string GetReference (string factory)
        {
            var date = DateTime.Now.ToString("yyyMMdd");
            var patern = $"{factory}-CA";

            var counted= unitOfWork.ReturnRepository.Get(c => c.return_no.Contains(patern) && c.return_no.Contains(date)).Count()+1;
            return $"{counted:00}";
        }

        [Route("api/claims/getCategories")]
        [HttpGet]
        public object GetCategoryIssue()
        {
            return feedbackCategoryDAL.GetForType(erp.Model.Returns.ClaimType_CorrectiveAction);
        }

        [Route("api/claims/getClientOrders")]
        [HttpGet]
        public object GetClientOreder()
        {
            
            
            var userId = accountService.GetCurrentUser().userid;
            var from = DateTime.Now.AddMonths(-2);
            var to = DateTime.Now.AddDays(1);
            
            var inspections = inspectionsDAL.GetForQcs(from, to, new[] { (int?) userId});
            return inspections.Select(s => new { s.insp_id, s.insp_unique, s.custpo, s.customer_code, s.factory_code,s.new_insp_id });
        }
        [Route("api/claims/getProducts")]
        [HttpPost]
        public object GetProducts(int? inspId,int? newInspId )
        {
            if (inspId != null)
            {
                var insp = unitOfWork
                    .InspectionRepository
                    .Get(i => i.insp_unique == inspId, includeProperties: "LinesTested").FirstOrDefault();

                var cprod_codes = insp.LinesTested.Where(l => l.order_linenum == 0).Select(l => l.insp_client_ref).ToList();

                var products = unitOfWork.CustProductRepository.Get(p =>
                    p.cprod_status != "D"
                    /* &&  p.BrandCompany.customer_code == insp.customer_code*/ 
                    && cprod_codes.Contains(p.cprod_code1), includeProperties: "BrandCompany").ToList();

                return insp.LinesTested.Select(n => new
                {
                    id = n.insp_line_unique,
                    cprod_code = n.insp_client_ref,
                    desc = n.insp_client_desc,
                    insp_qty = n.insp_qty,
                    order_linenum = n.order_linenum,
                    cprod_id = n.order_linenum > 0 ? null : 
                        (products.FirstOrDefault(p => p.cprod_code1 == n.insp_client_ref && p.BrandCompany.customer_code == insp.customer_code) ?? products.FirstOrDefault(p => p.cprod_code1 == n.insp_client_ref))?.cprod_id
                    }).ToList();
            }       

            return unitOfWork.InspectionV2Repository.Get(i => i.id == newInspId, includeProperties: "Lines.Product")
                .SelectMany(s => s.Lines.Select(n => new { id = n.insp_id, cprod_code = n.insp_custproduct_code, desc = "", insp_qty=n.inspected_qty, order_linenum=n.orderlines_id }));
            
        }
        [Route("api/claims/getUser")]
        [HttpGet]
        public object GetUser()
        {
            //return 1234;
            var user = accountService.GetCurrentUser();
            return new { user = user.userid, name = user.userwelcome, client = user.company_id };

        }

        [Route("api/claims/getReasonTemplates")]
        [HttpGet]
        public object getReasonTemplates()
        {

            return unitOfWork.CaReasonsRepository.Get().ToList();

        }

        [Route("api/claims/getCprod")]
        [HttpGet]
        public int GetCprod(string cprodCode)
        {
            //return 1234;
            return unitOfWork.CustProductRepository.Get(c => c.cprod_code1 == cprodCode).First().cprod_id;

        }

        [Route("api/claims/importances")]
        [HttpGet]
        public object getImportances()
        {
            return returnsImportanceDAL.GetForType(erp.Model.Returns.ClaimType_CorrectiveAction);
        }

        [Route("api/claims/createCA")]
        [HttpPost]
        public object createCA(CASimpleApiModel model)
        {
            Returns r = model.CA;
            inspections_list2 insp = model.Inspection;
            Order_lines orderline;

            var recipients = new List<User>();
            Cust_products product;
            
            r.request_date = DateTime.Now;
            
            r.decision_final = 0;
            r.openclosed = 0;
            r.claim_type = Returns.ClaimType_CorrectiveAction;

            int insp_unique = insp.insp_unique;
            string custpo = string.Empty;

            int i = insp.custpo.IndexOf(',');

            if (i != -1 && i != 0)
            {
                custpo = insp.custpo.Substring(0, i);
            }
            else
            {
                custpo = insp.custpo;
            }

            var lines_tested = inspectionLinesTestedDal.GetInspLines(insp_unique).Where(l => l.insp_custpo == custpo).FirstOrDefault();

            if (lines_tested != null)
            {
                orderline = unitOfWork.OrderLinesRepository.Get(m => m.linenum == lines_tested.order_linenum).FirstOrDefault();

                if (orderline != null)
                    r.order_id = orderline.orderid;
            }

            if (r.cprod_id != null)
                r.Product = custproductsDAL.GetById(r.cprod_id.Value);
            if (r.Product != null)
            {
                r.spec_code1 = r.Product.cprod_code1;
                r.spec_name = r.Product.cprod_name;
            }
            if (model.CASimpleProducts.Count > 0)
            {
                model.CA.Products = new List<Cust_products>();
                //foreach (var item in model.CASimpleProducts)
                //{
                //tempProduct.Add(unitOfWork.CustProductRepository.Get(c=>c.cprod_code1 == item).FirstOrDefault());
                
                var ids = model.CASimpleProducts.Where(s=>s.OrderLineNum > 0).Select(s => s.OrderLineNum).ToList();
                //ids.AddRange( model.CASimpleProducts.Where(s => s.cprod_id != null).Select(s => (int)s.cprod_id).ToList());
                
                //var products= unitOfWork.OrderLinesRepository.Get(c => ids.Any(n => c.linenum.ToString().Contains(n.ToString()))).Select(n => new { cprod_id = n.cprod_id, cprod_code1 = n.spec_code });
                var products = unitOfWork.OrderLinesRepository.Get(c => ids.Any(n => c.linenum ==n)).Select(n => new Cust_products { cprod_id = n.cprod_id ?? 0, cprod_code1 = n.spec_code });

                model.CA.Products.AddRange(products);
                model.CA.Products.AddRange(model.CASimpleProducts.Where(p => p.cprod_id != null).Select(p => new Cust_products { cprod_id = p.cprod_id ?? 0, cprod_code1 = p.CprodCode }));
                //model.CA.Products.AddRange(model.CASimpleProducts.Where(p => p.cprod_id == null).Select(p => new Cust_products { cprod_id = p.cprod_id ?? 0, cprod_code1 = p.CprodCode }));
                
            }
            if (r.order_id != null && r.order_id > 0)
            {
                var header = orderHeaderDAL.GetById(r.order_id.Value);
                if (header != null)
                    r.custpo = header.custpo;   
            }

            //var list = GetFiles(null, null);
            //r.Images = list;

            CollectFiles(r);

            //CA creator is QC in case of mobile QCs so we are adding to QC
            if (r.request_userid != null)
            {
                var qc = unitOfWork.UserRepository.Get(u => u.userid == r.request_userid).FirstOrDefault();

                if (r.ReturnsQCUsers != null)
                    r.ReturnsQCUsers.Add(new Returns_qcusers { return_id = r.returnsid, useruser_id = qc.userid, type = 0 });
                else
                {
                    r.ReturnsQCUsers = new List<erp.Model.Returns_qcusers>();
                    r.ReturnsQCUsers.Add(new Returns_qcusers { return_id = r.returnsid, useruser_id = qc.userid, type = 0 });
                }
            }

            /** Default mail subscriber for created simple CA*/
            var subscribers = Properties.Settings.Default.FeedbackSubscribers_M_CA.Split(',').ToList().Select(s => Int32.Parse(s));

            r.Subscriptions = unitOfWork.UserRepository.Get(s => subscribers.Contains(s.userid)).Select(s => new Feedback_subscriptions { subs_useruserid = s.userid }).ToList();

            recipients = CreateFeedbackSubscriptions(r, out product);

            r.Subscriptions.AddRange(recipients.Select(s => new Feedback_subscriptions { subs_useruserid = s.userid }));
            r.Subscriptions = r.Subscriptions.GroupBy(c => c.subs_useruserid).Select(gr => gr.First()).ToList();

            unitOfWork.ReturnRepository.Insert(r);

            r.Events = new List<returns_events>();

            ClaimsController.RegisterEvent(r, ReturnEventType.CorrectiveActionCreate);

            unitOfWork.Save();

            //remove duplicates
            //recipients = recipients.GroupBy(c => c.userid).Select(gr => gr.First()).ToList();

            //SendEmails(r, recipients, r.Product);

            WebUtilities.ClearTempFiles();

            return "OK";
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

        [Route("api/claims/deleteTempFile")]
        [HttpPost]
        public string DeleteTempFile(string name)
        {
            WebUtilities.DeleteTempFile(name);
            return "OK";
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
                        //System.IO.File.Delete(Path.Combine(Server.MapPath(Settings.Default.returns_fileroot), image.return_image));
                        System.IO.File.Delete(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Properties.Settings.Default.returns_fileroot), image.return_image));

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
                        string filePath = company.Common.Utilities.GetFilePath(kv.Key, System.Web.Hosting.HostingEnvironment.MapPath(Properties.Settings.Default.returns_fileroot));
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
        
        public void SendEmails(Returns returns, List<User> recipients, Cust_products product)
        {
            var emailRecipientsDb = emailRecipientsDAL.GetByCriteria(accountService.GetCurrentUser().company_id, "feedback", returns.claim_type.ToString(), "0").FirstOrDefault();

            string to = string.Empty, cc = string.Empty, bcc = string.Empty;
            var subscriberEmails = string.Join(",",
                                               recipients.Where(f => !string.IsNullOrEmpty(f.user_email))
                                                         .Select(f => f.user_email));
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
                                         returns.client_comments2, returns.Creator.userwelcome, WebUtilities.GetSiteUrl(), returns.returnsid);
                }
                else if (returns.claim_type == erp.Model.Returns.ClaimType_CorrectiveAction)
                {

                    //aditional users from db config settings

                    var add_emails = Properties.Settings.Default.FeedbackCorrectiveActionsAdditionalEMails;
                    var add_emails_cc = Properties.Settings.Default.FeedbackCorrectiveActionsAdditionalEMailsCC;
                    var add_emails_bcc = Properties.Settings.Default.FeedbackCorrectiveActionsAdditionalEMailsBCC;

                    if (!string.IsNullOrEmpty(add_emails) && add_emails.Length > 0)
                        to = to + " ," + add_emails;

                    if (!string.IsNullOrEmpty(add_emails_cc) && add_emails_cc.Length > 0)
                        cc = cc.Length > 0 ? (cc + " ," + add_emails_cc) : (cc + add_emails_cc);

                    if (!string.IsNullOrEmpty(add_emails_bcc) && add_emails_bcc.Length > 0)
                        bcc = bcc.Length > 0 ? (bcc + " ," + add_emails_bcc) : (bcc + add_emails_bcc);

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

                    if (returns.ReturnsQCUsers != null)
                    {
                        
                        {
                            var qcusers = returns.ReturnsQCUsers.Select(m => m.User.userid).ToList();
                            qcUserList = String.Join(", ", unitOfWork.UserRepository.Get(m => qcusers.Contains(m.userid)).Select(m => m.userwelcome).ToList());
                        }
                    }
                    
                    if (returns.Images != null && returns.Images.Count > 0)
                    {
                       
                        // Open image from file
                        IPdfManager pdfManager = new PdfManager();
                        var doc = pdfManager.CreateDocument();

                        try
                        {
                            foreach (var i in returns.Images)
                            {
                                IPdfImage objImage = doc.OpenImage(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), i.return_image));
                                float fWidth = objImage.Width * 72.0f / objImage.ResolutionX;
                                float fHeight = objImage.Height * 72.0f / objImage.ResolutionY;
                                IPdfPage objPage = doc.Pages.Add(fWidth, fHeight, Missing.Value);
                                objPage.Canvas.DrawImage(objImage, "x=0, y=0");
                            }
                        }
                        //for trying to open pdf-s or something that is not jpeg
                        catch (System.Runtime.InteropServices.COMException exp)
                        {
                            int error = exp.ErrorCode;

                            if (error != -2146828179)
                                throw exp;
                        }


                        try
                        {
                            var pdfDocName = $"M-CA-Feedback-images-{returns.return_no}.pdf";
                            var directory = "mobile_attachments";
                            Directory.CreateDirectory(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory));
                            doc.Save(System.Web.Hosting.HostingEnvironment.MapPath(Path.Combine(Settings.Default.returns_fileroot, directory, pdfDocName)), true);
                            doc.Close();
                            Attachment pdf = new Attachment(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory, pdfDocName));
                            attachments.Add(pdf);
                        }
                        catch(System.Runtime.InteropServices.COMException) { }
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
            }


            if (attachments.Count > 0)
            {
                mailHelper.SendMail(Settings.Default.Feedback_From, to, subject, body, cc, bcc, attachments.ToArray());
               
            }
            else
            {
                mailHelper.SendMail(Properties.Settings.Default.Feedback_From, to, subject, body, cc, bcc);
            }
        }

        //[AllowAnonymous]
        //[Route("api/claims/pdf/param", Name = "PdfNoteView")]
        //[HttpGet]
        //public HttpResponseMessage SavePdfTest(string reference, string category,string products)
        //{
            
        //    dynamic model = new {
        //        Reference=reference,
        //        Category= category,
        //        Products= products
        //    };
        //    var response = new HttpResponseMessage(HttpStatusCode.OK);
        //    string viewPath = HttpContext.Current.Server.MapPath(@"~/Views/Claims/RenderCaPdf.cshtml");

        //    var template = File.ReadAllText(viewPath);
        //    string parsedView = Razor.Parse(template, model);
        //    response.Content = new StringContent(parsedView);


        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        //    return response;
        //}



        //[Route("api/claims/pdfNote")]
        //[HttpGet]
        //public string PdfNote(int id)
        //{
            
         
        //    var p = "V31 Diverter with ceramic lever, V38 Ceramic telephone handset kit (including telephone handset, hose and 1/2) ";

        //    var po = "SP - 137415 ";
           
        //    IPdfManager pdfManeger = new PdfManager();
        //    var doc = pdfManeger.CreateDocument();

        //    doc.ImportFromUrl(Url.Link
        //        ("PdfNoteView", new {
        //            reference = "VQ-CA01M-20170823-SM",
        //            category = "Inspection findings",
        //            products = "V31 Diverter with ceramic lever, V38 Ceramic telephone handset kit (including telephone handset, hose and 1/2) ",

        //        }), 
        //        "scale=0.6;hyperlinks=true;drawbackground=true");

        //    /*Save to local file*/
        //    var pdfDocName = $"m-CA-Feedback-images-{id}.pdf";
        //    var directory = "mobile_attachments";
        //    Directory.CreateDirectory(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), directory));
        //    doc.Save(System.Web.Hosting.HostingEnvironment.MapPath(Path.Combine(Settings.Default.returns_fileroot, directory, pdfDocName)), true);


        //    return "Test Pdf Note II";
        //}

        [Route("api/claims/getReturnCategories")]
        [HttpGet]
        public object GetReturnCategories()
        {
            return returnCategoryDAL.GetAll();
        }
        [Route("api/claims/getTempUrl")]
        [HttpGet]
        public HttpResponseMessage getClaimsTempUrl(string file_id,string name = null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(file_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            if (name != null)
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = name};
            return result;
        }
        /*IMAGES*/
        [Route("api/claims/filesReturns")]
        [HttpPost]
        public object FilesReturns()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };

            //string fileName = WebUtilities.GetFileName(name, Request);
            // return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize), "text/html");

        }

        /*
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
        */
        /*END IMAGES*/

        /* END MOBILE DEVICES*/

        public List<User> CreateFeedbackSubscriptions(Returns returns, out Cust_products product)
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

            if (returns.claim_type == erp.Model.Returns.ClaimType_CorrectiveAction)
            {
                if (returns.Product == null)
                {
                    Cust_products caproduct;

                    if (returns.Products != null && returns.Products.Count > 0)
                    {
                        caproduct = returns.Products.FirstOrDefault();
                        
                        caproduct = unitOfWork.CustProductRepository.Get(m => m.cprod_id == caproduct.cprod_id, includeProperties: "MastProduct").FirstOrDefault();
                        product = caproduct;
                    }
                }

                if (product != null && product.MastProduct != null && product.MastProduct.factory_id != null)
                {
                    var factory_id = product.MastProduct.factory_id.Value;

                    fcs = adminPermissionsDal.GetByCompany(factory_id).Where(a => a.processing == 0).Select(f => f.User).ToList();

                    var factory = unitOfWork.CompanyRepository.Get(m => m.user_id == factory_id).FirstOrDefault();

                    if (factory != null)
                        SubscribeSupervisor(factory.consolidated_port, returns);

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

            var settings = Properties.Settings.Default[string.Format("Feedback_subscribers_{0}", returns.claim_type)];
            var subscribers = new List<int>();

            if (returns.claim_type != erp.Model.Returns.ClaimType_CorrectiveAction)
                subscribers = settings != null ? settings.ToString().Split(',').Select(int.Parse).ToList() : new List<int>();

            subscribers.AddRange(fcs.Select(f => f.userid));

            if (returns.Subscriptions != null && returns.Subscriptions.Count > 0)
                subscribers.AddRange(returns.Subscriptions.Where(s => s.subs_useruserid != null).Select(s => s.subs_useruserid.Value));

            //adding creator to subscribers list
            if (returns.request_userid == null)
                returns.request_userid = accountService.GetCurrentUser().userid;

            if (returns.request_userid != null)
                subscribers.Add((Convert.ToInt32(returns.request_userid)));

            foreach (var subscriber in subscribers)
            {
                var sub = new Feedback_subscriptions { subs_returnid = returns.returnsid, subs_useruserid = subscriber };

                if (returns.Subscriptions == null || returns.Subscriptions.Count(s => s.subs_useruserid == subscriber && s.subs_id > 0) <= 0)
                {
                    if (returns.Subscriptions == null)
                    {
                        returns.Subscriptions = new List<Feedback_subscriptions>();
                    }
                    //returns.Subscriptions.Add(sub);

                }
                //Feedback_subscriptionsDAL.Create(sub);
                var user = userDAL.GetById(subscriber);

                if (user != null && !string.IsNullOrEmpty(user.user_email))
                    result.Add(user);
            }

            return result;
        }

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

        private string SubstituteMacros(string recipients, string subscriberEmails, string creatorEmails)
        {
            var result = new List<string>();
            var parts = recipients.Split(',');
            foreach (var part in parts)
            {
                if (part == "{creator}")
                {
                    if (!string.IsNullOrEmpty(creatorEmails))
                        result.AddRange(creatorEmails.Split(','));
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

        private List<Lookup> GetResolutions()
        {
            return new[] {new Lookup{ Id= 1, Title = "Yes - customer has been refunded" },new Lookup { Id = 2, Title = "Yes - customer has been given a replacement and the issue is resolved" },
                            new Lookup{ Id= 3, Title = " No - this issue is still pending a resolution" }}.ToList();
        }
        private string GetTypeText(int claim_type)
        {
            return claim_type == erp.Model.Returns.ClaimType_Product ? "product" :
                    claim_type == erp.Model.Returns.ClaimType_ITFeedback ? "IT" :
                    claim_type == erp.Model.Returns.ClaimType_CorrectiveAction ? "CA" : "";
        }
    }
}