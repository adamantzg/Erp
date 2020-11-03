using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using asaq2.Model;
using asaq2.Model.DAL;
using asaq2back.App_GlobalResources;
using asaq2back.Models;
using System.IO;
using Utilities = asaq2.Common.Utilities;

namespace asaq2back.Controllers
{
    [Authorize]
    public class AfterSalesEnquiriesController : BaseController
    {
        //
        // GET: /AfterSales/

        public ActionResult Index(DateTime? month = null, string sortField = "", string sortDir = " ASC")
        {
            var sort = string.Empty;

            //Prevent injection
            if (sortDir.Trim().ToUpper() != "ASC" && sortDir.Trim().ToUpper() != "DESC")
                sortDir = " ASC";
            if (!string.IsNullOrEmpty(sortField))
                sort = sortField + sortDir;
            if (month == null)
                //if not set, put today
                month = DateTime.Today;
            List<After_sales_enquiry> enquiries = new List<After_sales_enquiry>();

            DateTime? closedFrom=null, closedTo=null;
            //1st day of month
            closedFrom =  new DateTime(month.Value.Year, month.Value.Month, 1);
            //last day of month
            closedTo = closedFrom.Value.AddMonths(1).AddSeconds(-1);

            bool completedOnly = (month.Value.Month != DateTime.Today.Month || month.Value.Year != DateTime.Today.Year);
            
            CompanyType currentUserRole;
            if(User.IsInRole(UserRole.Distributor.ToString()))
            {
                currentUserRole = CompanyType.Distributor;
                enquiries = After_sales_enquiryDAL.GetByDistributor(WebUtilities.GetCurrentUser().company_id, sort,completedOnly, closedFrom, closedTo);
            }
            else if(User.IsInRole(UserRole.MasterDistributor.ToString()))
            {
                currentUserRole = CompanyType.MasterDistributor;
                enquiries = After_sales_enquiryDAL.GetForMasterDistributor(sort, completedOnly, closedFrom, closedTo);
            }
            else
            {
                currentUserRole = CompanyType.Manufacturer;
                //TODO
                //Get for manufacturer
                enquiries = After_sales_enquiryDAL.GetForManufacturer(WebUtilities.GetCurrentUser(), sort, completedOnly, closedFrom, closedTo);
            }
          
            
            //Compute dynamic statuses
            ComputeStatuses(enquiries, currentUserRole);

            AfterSalesEnquiriesListModel model = new AfterSalesEnquiriesListModel();

            model.Enquiries = enquiries;//.OrderBy(e=>e.status_id).ToList();
            
            model.Statuses = (from e in enquiries group e by new { e.status_id, e.status_name } into g select new EnquiryStatus { status_id = g.Key.status_id, status_name = g.Key.status_name }).ToList();
            model.EnableAddNew = currentUserRole == CompanyType.Distributor;
            model.IsMasterDistributor = currentUserRole == CompanyType.MasterDistributor;
            model.Month = month.Value;
            ViewData["sortField"] = sortField;
            ViewData["sortDir"] = sortDir;
            return View(model);
        }

        private void ComputeStatuses(List<After_sales_enquiry> enquiries, CompanyType currentUserRole)
        {
            foreach (var ase in enquiries)
            {
                string status = App_GlobalResources.Resources.AfterSales_Status_New; ;
                int status_id = EnquiryStatus.New; ;
                if (ase.dateclosed == null)
                {
                    After_sales_enquiry_comment lastComment = null;
                    After_sales_enquiry_sitevisit lastVisit = null;
                    if (ase.Comments != null)
                        lastComment = ase.Comments.LastOrDefault();
                    if (ase.SiteVisits != null)
                        lastVisit = ase.SiteVisits.LastOrDefault();
                    if (currentUserRole == CompanyType.Distributor)
                    {
                        if (IsStatusSiteVisit(lastComment, lastVisit))
                        {
                            //status is related to site visit
                            if (lastVisit.status_id == After_sales_enquiry_sitevisit_status.Pending)
                            {
                                status = Resources.AfterSales_Status_SiteVisitRequestedDist;
                                status_id = EnquiryStatus.SiteVisitRequested;
                            }
                            else
                            {
                                status = Resources.AfterSales_Status_SiteVisitSubmitted;
                                status_id = EnquiryStatus.SiteVisitSubmitted;
                            }
                        }
                        else
                        {
                            if (lastComment != null)
                            {
                                if (lastComment.creator_role == CompanyType.Distributor)
                                {
                                    status = Resources.AfterSales_Status_Awaiting;
                                    status_id = EnquiryStatus.AwaitingResponse;
                                }
                                else
                                {
                                    status = Resources.AfterSales_Status_Responded;
                                    status_id = EnquiryStatus.Responded;
                                }
                            }
                            else
                            {
                                status = Resources.AfterSales_Status_Awaiting;
                                status_id = EnquiryStatus.AwaitingResponse;
                            }
                        }
                    }
                    else if (currentUserRole == CompanyType.MasterDistributor)
                    {
                        if (IsStatusSiteVisit(lastComment, lastVisit))
                        {
                            //status is related to site visit
                            if (lastVisit.status_id == After_sales_enquiry_sitevisit_status.Pending)
                            {
                                status = Resources.AfterSales_Status_SiteVisitRequested;
                                status_id = EnquiryStatus.SiteVisitRequested;
                            }
                            else
                            {
                                status = Resources.AfterSales_Status_SiteVisitSubmitted;
                                status_id = EnquiryStatus.SiteVisitSubmitted;
                            }
                        }
                        else
                        {
                            if (lastComment != null)
                            {
                                //status is obtained from comment
                                if (lastComment.creator_role == CompanyType.MasterDistributor)
                                {
                                    if (lastComment.respondto_role == CompanyType.Distributor)
                                        status = Resources.AfterSales_Status_Awaiting_Master_Dist;
                                    else
                                        status = Resources.AfterSales_Status_Awaiting_Master_Manuf;
                                    status_id = EnquiryStatus.AwaitingResponse;
                                }
                                else
                                {
                                    if (lastComment.creator_role == CompanyType.Distributor)
                                        status = Resources.AfterSales_Status_Responded_Dist;
                                    else
                                        status = Resources.AfterSales_Status_Responded_Manuf;
                                    status_id = EnquiryStatus.Responded;
                                }
                            }
                        }

                    }
                    else
                    {
                        //manufacturer
                        if (lastComment != null)
                        {
                            if (lastComment.creator_role == CompanyType.Manufacturer)
                            {
                                status_id = EnquiryStatus.Responded;
                                status = Resources.AfterSales_Status_Responded;
                            }
                        }
                    }
                }
                else
                { 
                
                }
                ase.status_name = status;
                ase.status_id = status_id;
            }
        }

        public ActionResult Create()
        {
            ViewBag.Mode = "new";
            WebUtilities.ClearTempFiles();
            WebUtilities.ClearCommentTempFiles();
            AfterSalesEnquiryModel m = BuildModel();
            m.Enquiry.reference_number = GetNextReferenceNumber(string.Empty);
            return View("Edit", m);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AfterSalesEnquiryModel m)
        {
            if (ModelState.IsValid)
            {
                CollectValues(m);
                m.Enquiry.created_userid = WebUtilities.GetCurrentUser().userid;
                m.Enquiry.datecreated = DateTime.Now;
                if (m.Enquiry.dealer_id == null)
                {
                    DealerDAL.Create(m.Dealer);
                    m.Enquiry.dealer_id = m.Dealer.user_id;
                }
                m.Enquiry.reference_number = GetNextReferenceNumber(ClasificationDAL.GetAll().Where(c => c.clasification_id == m.Enquiry.clasification_id).Select(c => c.clasification_name.Substring(0,1).ToUpper()).FirstOrDefault());
                After_sales_enquiryDAL.Create(m.Enquiry);
                                
                WebUtilities.ClearTempFiles();

                Company distributor = CompanyDAL.GetById(WebUtilities.GetCurrentUser().company_id);
                User user = CompanyDAL.GetMasterDistributorAccount(WebUtilities.GetCurrentUser().company_id);
                Cust_products prod = Cust_productsDAL.GetById(m.Enquiry.cprod_id);

                if(user != null)
                {
                                        
                    MailHelper.SendMail(Properties.Settings.Default.AfterSalesFromAccount, user.user_email, string.Format(Resources.AfterSales_Subject, m.Enquiry.reference_number),
                                        string.Format(Resources.AfterSales_MailBody, distributor.user_name, m.Dealer.user_name, prod.cprod_code1 + " " + prod.cprod_name,
                                        m.Enquiry.response_type == After_sales_enquiry.response_type_sitevisit ? "yes" : "no",m.Enquiry.details, string.Format("{0}/AfterSalesEnquiries/Read/{1}",WebUtilities.GetSiteUrl(), m.Enquiry.enquiry_id.ToString())));
                }
                m.CurrentUser = WebUtilities.GetCurrentUser();
                return View("ConfirmCreate",m);
            }
            else
            {
                ViewBag.Mode = "new";
                var enquiryModel = BuildModel();
                enquiryModel.Enquiry = m.Enquiry;
                return View("Edit", enquiryModel);
            }
        }

        [HttpPost]
        public ActionResult ConfirmCreate(AfterSalesEnquiryModel m)
        {
            if (ModelState.IsValid)
            {
                User user = UserDAL.GetById(m.CurrentUser.userid);
                user.mobilea = m.CurrentUser.mobilea;
                user.user_email = m.CurrentUser.user_email;
                UserDAL.Update(user);
                return RedirectToAction("Index");
            }
            else
                return View();
        }
        
        public ActionResult Read(int id)
        {
            AfterSalesEnquiryModel m = BuildModel();
            m.Enquiry = After_sales_enquiryDAL.GetById(id,true, WebUtilities.GetCurrentUser());
            ViewBag.returnTo = Url.Action("Read", new { id = id });
            if (m.Enquiry != null)
            {
                m.Dealer = DealerDAL.GetById(m.Enquiry.dealer_id.Value);
                return View(m);
            }
            else
            {
                ViewBag.Message = Resources.EnquiryWrongId;
                return View("Message");
            }
        }

        //
        // GET: /Issues/Edit/5

        public ActionResult Edit(int id)
        {

            WebUtilities.ClearTempFiles();
            WebUtilities.ClearCommentTempFiles();
            var ase = After_sales_enquiryDAL.GetById(id, true, WebUtilities.GetCurrentUser());
            var model = BuildModel(ase);
            model.UserRoles = UserDAL.GetUserRoles(WebUtilities.GetCurrentUser().username).ToList();
            model.CommentRecipients = GetCommentRecipients(ase);
            ViewData["mode"] = "edit";
            ViewBag.returnTo = Url.Action("Edit", new { id = id });
            if (ase != null)
            {
                model.Dealer = DealerDAL.GetById(ase.dealer_id.Value);
                return View(model);
            }
            else
            {
                ViewBag.Message = Resources.EnquiryWrongId ;
                return View("Message");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, AfterSalesEnquiryModel m)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    After_sales_enquiry ase = After_sales_enquiryDAL.GetById(id, true, WebUtilities.GetCurrentUser());
                    if (ase != null)
                    {
                        m.UserRoles = UserDAL.GetUserRoles(WebUtilities.GetCurrentUser().username).ToList();
                        if (m.UserRoles.Contains(UserRole.Distributor) || m.UserRoles.Contains(UserRole.MasterDistributor))
                        {
                            //Distributor or master distributor (CW) 
                            //copy old read-only values
                            m.Enquiry.datecreated = ase.datecreated;
                            m.Enquiry.created_userid = ase.created_userid;
                            m.Enquiry.enquiry_id = ase.enquiry_id;
                            m.Enquiry.modified_userid = WebUtilities.GetCurrentUser().userid;
                            m.Enquiry.datemodified = DateTime.Now;
                            m.Enquiry.Files = ase.Files;
                            m.Enquiry.Comments = ase.Comments;
                            m.Enquiry.dealer_name = ase.dealer_name;
                            m.Enquiry.cprod_name = ase.cprod_name;
                            m.Enquiry.status_id = ase.status_id;
                            m.Enquiry.SiteVisits = ase.SiteVisits;
                            //if (ase.contractor_id != m.Enquiry.contractor_id)
                            //{ 
                            //    //Send site visit request
                            //}
                            CollectValues(m);
                        }
                        else
                        { 
                            //Probably manufacturer - retain all old values except for new comment
                            m.Enquiry = ase;
                            CollectValues(m);
                        }

                        bool siteVisitCreated = m.Enquiry.SiteVisits.Count > 0 && m.Enquiry.SiteVisits.Last().sitevisit_id <= 0;

                        After_sales_enquiryDAL.Update(m.Enquiry);

                        //Send mail if site visit created
                        if(siteVisitCreated)
                        {
                            After_sales_enquiry_sitevisit sv = m.Enquiry.SiteVisits.Last();
                            External_user contractor = External_userDAL.GetById(sv.contractor_id);
    
                            MailHelper.SendMail(Properties.Settings.Default.AfterSalesFromAccount, contractor.email, Resources.AfterSales_SiteVisitRequest_MailSubject,
                                string.Format(Resources.AfterSales_SiteVisitRequest_MailBody,WebUtilities.GetCurrentUser().userwelcome,m.Enquiry.cprod_name,m.Enquiry.cust_name,ComposeCustomerAddress(m.Enquiry) , sv.requestmessage, string.Format("{0}/AfterSalesSiteVisits/Edit/{1}", WebUtilities.GetSiteUrl(), sv.sitevisit_id.ToString())), Properties.Settings.Default.AfterSalesCC,
                                Properties.Settings.Default.AfterSalesBcc);

                        }

                        WebUtilities.ClearTempFiles();
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Mode = "edit";
                    var enquiryModel = BuildModel();
                    enquiryModel.Enquiry = m.Enquiry;
                    return View("Edit", enquiryModel);
                }


            }
            catch
            {
                return View();
            }
        }

        private bool IsStatusSiteVisit(After_sales_enquiry_comment lastComment, After_sales_enquiry_sitevisit lastVisit)
        {
            //Status is related to site visit if lastComment came before sitevisit (create_userid is null only for auto comments generated for site visits)
            return lastVisit != null && (lastComment == null || lastComment.created_userid == null || (lastComment.datecreated < lastVisit.datecreated && (lastVisit.datecompleted == null || lastComment.datecreated < lastVisit.datecompleted)));
        }

        public static string ComposeCustomerAddress(After_sales_enquiry ase)
        {
            string[] parts = { ase.cust_address1, ase.cust_address2, ase.cust_address3, ase.cust_address4, ase.cust_address5, ase.cust_postcode };
            return string.Join(",", parts.Where(p => !string.IsNullOrEmpty(p)).ToArray());
        }

        public ActionResult Delete(int id)
        {
            After_sales_enquiryDAL.Delete(id);
            return RedirectToAction("Index");
        }

        private After_sales_enquiry_sitevisit CreateSiteVisit(After_sales_enquiry ase, int contractor_id, DateTime? datevisited, string contractor_message)
        {
            After_sales_enquiry_sitevisit sv = new After_sales_enquiry_sitevisit();
            sv.reference_number = string.Format("{0}-sv-{1}", ase.reference_number, ase.SiteVisits.Count + 1);
            sv.enquiry_id = ase.enquiry_id;
            sv.contractor_id = contractor_id;
            sv.datevisited = datevisited;
            sv.status_id = After_sales_enquiry_sitevisit_status.Pending;
            sv.requestmessage = contractor_message;
            sv.created_userid = WebUtilities.GetCurrentUser().userid;
            sv.datecreated = DateTime.Now;
            
            //Send auto email

            return sv;

        }

        private void CollectValues(AfterSalesEnquiryModel m)
        {
            m.Enquiry.details = HttpUtility.HtmlDecode(m.Enquiry.details);
            m.Enquiry.Files = GetFiles(m.Enquiry.Files, string.Empty + Request["hidDeletedFiles"]);
            if (m.Enquiry.dealer_id == null)
            { 
                //New dealer
                m.Dealer.user_type = WebUtilities.GetCurrentUser().company_id;
                
            }
            //new comment
            string newComment = string.Empty + Request["newcomment"];
            if (!string.IsNullOrEmpty(newComment))
            {
                After_sales_enquiry_comment c = new After_sales_enquiry_comment();
                c.enquiry_id = m.Enquiry.enquiry_id;
                c.comment_text = HttpUtility.HtmlDecode(newComment);
                c.created_userid = WebUtilities.GetCurrentUser().userid;
                c.datecreated = DateTime.Now;
                c.respond_to = int.Parse(string.Empty + Request["newcommentrecipient"]);
                c.respondto_role = CompanyDAL.GetCompanyType(c.respond_to);
                c.creator_role = CompanyDAL.GetCompanyType(WebUtilities.GetCurrentUser().company_id);
                
                //if (type == CompanyType.Distributor)
                //    m.Enquiry.status_id = After_sales_enquiry_status.AwaitingDistributorResponse;
                //else if (type == CompanyType.Manufacturer)
                //    m.Enquiry.status_id = After_sales_enquiry_status.AwaitingManufacturerResponse;

                c.Files = GetCommentFiles(c.Files);
                if (m.Enquiry.Comments == null)
                    m.Enquiry.Comments = new List<After_sales_enquiry_comment>();
                m.Enquiry.Comments.Add(c);

                //Send autoemail
                string to = GetRecipientAccount(m.Enquiry, c.respond_to, c.respondto_role);
                if (!string.IsNullOrEmpty(to))
                {

                    MailHelper.SendMail(Properties.Settings.Default.AfterSalesFromAccount, to, Resources.AfterSales_NewComment_MailSubject,
                            string.Format(Resources.AfterSales_NewComment_MailBody, m.Enquiry.dealer_name, m.Enquiry.cprod_name, c.comment_text, c.Files.Count,string.Format("{0}/AfterSalesEnquiries/Read/{1}",WebUtilities.GetSiteUrl(), m.Enquiry.enquiry_id.ToString()),m.Enquiry.reference_number,WebUtilities.GetCurrentUser().userwelcome), Properties.Settings.Default.AfterSalesCC,
                            Properties.Settings.Default.AfterSalesBcc);

                }
            }

            
            if (m.contractor_id != null)
            {
                if (m.Enquiry.SiteVisits == null)
                    m.Enquiry.SiteVisits = new List<After_sales_enquiry_sitevisit>();

                //DateTime? datevisited = Utilities.ParseDateFromString(string.Empty + Request["datevisited"]);
                
                After_sales_enquiry_sitevisit sv = CreateSiteVisit(m.Enquiry, m.contractor_id.Value,m.datevisited,m.siteVisitMessage);
                m.Enquiry.SiteVisits.Add(sv);
                //m.Enquiry.status_id = After_sales_enquiry_status.SiteVisitRequested;
                
            }
            
        }

        private string GetRecipientAccount(After_sales_enquiry ase, int company_id, CompanyType type)
        {
            if (type == CompanyType.Distributor)
            {
                User user = UserDAL.GetById(ase.created_userid.Value);
                return user.user_email;
            }
            else if (type == CompanyType.MasterDistributor)
            {
                //List<User> commentThreadUsers = After_sales_enquiryDAL.GetCommentThreadUsers(ase.enquiry_id);
                //return string.Join(",", commentThreadUsers.Where(u => u.company_id == company_id).Select(u => u.user_email).ToArray());
                List<User> afterSalesNotificationRecipients = UserDAL.GetByCompany(company_id).Where(u => u.after_sales == true).ToList();
                return string.Join(",", afterSalesNotificationRecipients.Select(u => u.user_email).ToArray());
            }
            else
            { 
                //manufacturer
                return string.Join(",", Cust_productsDAL.GetFactoryControllerUsers(ase.cprod_id).Select(u => u.user_email).ToArray());
            }
        }

        private AfterSalesEnquiryModel BuildModel(After_sales_enquiry e = null)
        {
            return new AfterSalesEnquiryModel
            {
                Enquiry = (e == null ? new After_sales_enquiry() : e),
                ResponseTypes = GetResponseTypes(),
                Dealer = new Dealer(),
                ChargeTypes = GetChargeTypes(),
                Classifications = ClasificationDAL.GetAll(),
                distributor_id = (e != null ? e.distributor_id : WebUtilities.GetCurrentUser().company_id),
                Contractors = External_userDAL.GetAll()
            };
        }

        private string GetNextReferenceNumber(string classification)
        {
            int company_id = WebUtilities.GetCurrentUser().company_id;
            int nextNum = After_sales_enquiryDAL.GetNextReferenceNumber(company_id);
            Company company = CompanyDAL.GetById(company_id);
            if (company != null)
            {
                return string.Format("AS-{0}-{1}-{2}-{3}", company.customer_code, (!string.IsNullOrEmpty(classification) ? classification : "?"), DateTime.Today.ToString("yyMMdd"), Utilities.OrdinalToLetters(nextNum));
            }
            else
                return string.Empty;
            

        }

        private List<SelectListItem> GetResponseTypes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Value = After_sales_enquiry.response_type_technicalresponse.ToString(), Text = Resources.AfterSalesEnquiries_ResponseType_Tech });
            items.Add(new SelectListItem { Value = After_sales_enquiry.response_type_sitevisit.ToString(), Text = Resources.AfterSalesEnquiries_ResponseType_SiteVisit });

            return items;
        }

        private List<SelectListItem> GetChargeTypes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Value = After_sales_enquiry.charge_type_customer.ToString(), Text = Resources.AfterSalesEnquiries_ChargeType_Customer });
            items.Add(new SelectListItem { Value = After_sales_enquiry.charge_type_dealer.ToString(), Text = Resources.AfterSalesEnquiries_ChargeType_Dealer });
            items.Add(new SelectListItem { Value = After_sales_enquiry.charge_type_distributor.ToString(), Text = Resources.AfterSalesEnquiries_ChargeType_Distributor });

            return items;
        }

        private List<Company> GetCommentRecipients(After_sales_enquiry ase)
        {
            List<Company> result = new List<Company>();
            if (User.IsInRole(UserRole.Distributor.ToString()) || User.IsInRole(UserRole.Manufacturer.ToString()))
            {
                Company masterDist = CompanyDAL.GetById(CompanyDAL.GetMasterDistributorAccount(WebUtilities.GetCurrentUser().company_id).company_id);
                result.Add(masterDist);
            }
            else
            //master distributor
            {
                result.Add(new Company { user_id = ase.distributor_id, user_name = ase.distributor_name });
                result.Add(CompanyDAL.GetFactoryForProduct(ase.cprod_id));
                //Don't display factory name, just generic text Manufacturer
                result[1].user_name = Resources.AfterSalesEnquiriesComment_RespondTo_Manufacturer;
            }
            return result;
        }



        private List<After_sales_enquiry_files> GetFiles(List<After_sales_enquiry_files> files, string deletedFiles)
        {
            List<string> written_files = new List<string>();
            if (files == null)
                files = new List<After_sales_enquiry_files>();

            if (!string.IsNullOrEmpty(deletedFiles))
            {
                List<int> deletedFilesIds = (from s in deletedFiles.Split(',') select int.Parse(s)).ToList();
                foreach (var item in deletedFilesIds)
                {
                    After_sales_enquiry_files file = files.Where(f => f.file_id == item).FirstOrDefault();
                    if (file != null)
                    {
                        files.Remove(file);
                        System.IO.File.Delete(Path.Combine(Server.MapPath(Properties.Settings.Default.Enquiries_FilesFolder), file.file_name));
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
                        string filePath = Utilities.GetFilePath(kv.Key, Server.MapPath(Properties.Settings.Default.Enquiries_FilesFolder));
                        StreamWriter sw = new StreamWriter(filePath);
                        MemoryStream ms = new MemoryStream(kv.Value);
                        ms.WriteTo(sw.BaseStream);
                        sw.Close();
                        files.Add(new After_sales_enquiry_files { file_name = Path.GetFileName(filePath),created_userid = WebUtilities.GetCurrentUser().userid, datecreated = DateTime.Now });
                        written_files.Add(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                //if some files written to folder and error occurred, delete them
                foreach (var item in written_files)
                {
                    System.IO.File.Delete(item);
                }
                throw;
            }
            return files;
        }

        private List<After_sales_enquiry_comment_files> GetCommentFiles(List<After_sales_enquiry_comment_files> files)
        {
            List<string> written_files = new List<string>();
            if (files == null)
                files = new List<After_sales_enquiry_comment_files>();
            try
            {
                var sessionFiles = WebUtilities.GetCommentTempFiles();
                if (sessionFiles != null)
                {
                    foreach (KeyValuePair<string, MemoryStream> kv in sessionFiles)
                    {
                        //Write file
                        string filePath = Utilities.GetFilePath(kv.Key, Server.MapPath(Properties.Settings.Default.Enquiries_FilesFolder));
                        StreamWriter sw = new StreamWriter(filePath);
                        kv.Value.WriteTo(sw.BaseStream);
                        sw.Close();
                        files.Add(new After_sales_enquiry_comment_files { file_name = Path.GetFileName(filePath), created_userid = WebUtilities.GetCurrentUser().userid, datecreated = DateTime.Now });
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
            return files;
        }

        public ActionResult Files(object qqfile, string remove)
        {
            ActionResult res = Json(null);
            string fileName = WebUtilities.GetFileName(qqfile, Request);
            if (!string.IsNullOrEmpty(fileName))
            {
                return Json(WebUtilities.SaveTempFile(fileName, Request, Properties.Settings.Default.Enquiries_MaxFileSize),"text/html");
            }
            //else if (!string.IsNullOrEmpty(remove))
            //    RemoveFile(remove);
            return res;
        }

        public ActionResult CommentFiles(object qqfile, string remove)
        {
            ActionResult res = Json(null);
            string fileName = WebUtilities.GetFileName(qqfile, Request);
            if (!string.IsNullOrEmpty(fileName))
            {
                return Json(WebUtilities.SaveCommentFile(fileName, Request, Properties.Settings.Default.Enquiries_MaxFileSize),"text/html");
            }
            //else if (!string.IsNullOrEmpty(remove))
            //    RemoveCommentFile(remove);
            return res;
        }

    }
}
