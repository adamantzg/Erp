using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using System.Net.Mail;
using backend.Properties;
using System.Text;
using ASPPDFLib;
using System.IO;
using System.Reflection;

namespace backend.ApiServices
{
	public class ClaimsService : IClaimsService
	{
		private readonly IEmailRecipientsDAL email_RecipientsDAL;
		private readonly IReturnsImportanceDAL returns_ImportanceDAL;
		private readonly IUnitOfWork unitOfWork;
		private readonly IFeedbackCategoryDAL feedback_CategoryDAL;
		private readonly IFeedbackTypeDAL feedback_TypeDAL;
		private readonly ICompanyDAL companyDAL;
        private readonly IAccountService accountService;
        private readonly IMailHelper mailHelper;

        public ClaimsService(IEmailRecipientsDAL email_RecipientsDAL, IReturnsImportanceDAL returns_ImportanceDAL,
			IUnitOfWork unitOfWork, IFeedbackCategoryDAL feedback_CategoryDAL, IFeedbackTypeDAL feedback_TypeDAL,
			ICompanyDAL companyDAL, IAccountService accountService, IMailHelper mailHelper)
		{
			this.email_RecipientsDAL = email_RecipientsDAL;
			this.returns_ImportanceDAL = returns_ImportanceDAL;
			this.unitOfWork = unitOfWork;
			this.feedback_CategoryDAL = feedback_CategoryDAL;
			this.feedback_TypeDAL = feedback_TypeDAL;
			this.companyDAL = companyDAL;
            this.accountService = accountService;
            this.mailHelper = mailHelper;
        }

		public void SendEmails(Returns returns, List<User> recipients, Cust_products product)
        {
            var emailRecipientsDb = email_RecipientsDAL.GetByCriteria(accountService.GetCurrentUser()?.company_id ?? 0, "feedback", returns.claim_type.ToString(),
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

            var importances = returns.claim_type != null ? returns_ImportanceDAL.GetForType(returns.claim_type.Value) : new List<Returns_importance>();

            var categorie = returns.feedback_category_id != null ? feedback_CategoryDAL.GetById(Convert.ToInt32(returns.feedback_category_id)) : null;

            var importance = importances.FirstOrDefault(i => i.importance_id == returns.importance_id);

            var resolution = GetResolutions().FirstOrDefault(r => r.Id == returns.resolution);

            var subject = string.Format(App_GlobalResources.Resources.Feedback_subject, returns.return_no,
                importance != null ? importance.importance_text : "", GetTypeText(returns.claim_type ?? 0));

            var factory = product != null && product.MastProduct != null && product.MastProduct.factory_id != null ? 
				companyDAL.GetById(product.MastProduct.factory_id.Value) : null;

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
                        var qcusers = returns.ReturnsQCUsers.Select(m => m.User.userid).ToList();
                        qcUserList = String.Join(", ", unitOfWork.UserRepository.Get(m => qcusers.Contains(m.userid) 
							&& !String.IsNullOrEmpty(m.user_email)).Select(m => m.userwelcome).ToList());                        
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

            var imagesExtensions = new List<string>() { "png", "PNG", "jpg", "JPG" };

            if (returns.Images != null && returns.Images.Count > 0 
                && (returns.Images.Where(i => imagesExtensions.Contains(i.return_image.Substring(i.return_image.Length - 3)))
                    .FirstOrDefault() != null))
            {
                // Open image from file
                IPdfManager pdfManager = new PdfManager();
                var doc = pdfManager.CreateDocument();

                foreach (var i in returns.Images.Where(i => i.file_category == null || imagesExtensions.Contains(i.return_image.Substring(i.return_image.Length -3))))
                {
                    try
                    {
                        IPdfImage objImage = doc.OpenImage(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_fileroot), 
							i.return_image));

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

		public List<Lookup> GetResolutions()
        {
            return new[] {new Lookup{ Id= 1, Title = "Yes - customer has been refunded" },new Lookup { Id = 2, Title = "Yes - customer has been given a replacement and the issue is resolved" },
                            new Lookup{ Id= 3, Title = " No - this issue is still pending a resolution" }}.ToList();
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

		public string GetTypeText(int claim_type)
        {
            return feedback_TypeDAL.GetById(claim_type)?.typename;            
        }

		public string MailOrganiser(string existing_emails, string new_emails)
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
	}
}