using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using asaq2.Model;
using System.IO;
using asaq2.Model.DAL;
using asaq2back.App_GlobalResources;
using Utilities = asaq2.Common.Utilities;

namespace asaq2back.Controllers
{
    [CustomAuthorization(LoginPage = "/ExternalUser/Login")]
    public class AfterSalesSiteVisitsController : Controller
    {
        //
        // GET: /AfterSalesSiteVisits/
        
        public ActionResult Index()
        {
            return View(new Models.AfterSalesSiteVisitsModel { Visits = GetVisits() });
        }

        private List<After_sales_enquiry_sitevisit> GetVisits()
        {
            List<After_sales_enquiry_sitevisit> sitevisits;
            int? contractor_id = GetContractorId();
            if (contractor_id != null)
                sitevisits = After_sales_enquiry_sitevisitDAL.GetByContractor(contractor_id.Value);
            else
            {
                User user = WebUtilities.GetCurrentUser();
                if(User.IsInRole(UserRole.MasterDistributor.ToString()))
                    sitevisits = After_sales_enquiry_sitevisitDAL.GetByCreator(user.userid);
                else
                    sitevisits = After_sales_enquiry_sitevisitDAL.GetByDistributor(user.userid);
            }
            return sitevisits.OrderBy(sv=>sv.datevisited).ToList();
        }

        public ActionResult Search(Models.AfterSalesSiteVisitsModel m)
        {
            After_sales_enquiry_sitevisit sv = After_sales_enquiry_sitevisitDAL.GetByReference(m.SearchTerm, GetContractorId());
            if (sv != null)
            {
                return RedirectToAction("Edit", new { id = sv.sitevisit_id });
            }
            else
            {
                ModelState.AddModelError("SearchTerm", "The site visit with given reference number cannot be found");
                m.Visits = GetVisits();
                return View("Index", m);
            }
        }

        private int? GetContractorId()
        {
            External_user user = WebUtilities.GetCurrentExternalUser();
            if (user != null)
                return user.user_id;
            else
                return null;
        }
        
        
        //
        // GET: /AfterSalesSiteVisits/Details/5

        public ActionResult Read(int id, string returnTo="")
        {
            var sv = After_sales_enquiry_sitevisitDAL.GetById(id);
            ViewBag.returnTo = returnTo;
            ViewBag.ShowSendButton = GetContractorId() == null && User.IsInRole(UserRole.MasterDistributor.ToString());
            return View(sv);
        }

        public ActionResult Report(int id)
        {
            string comment = string.Empty + Request["hidcomment"];
            var sv = After_sales_enquiry_sitevisitDAL.GetById(id);

            After_sales_enquiry_comment c = new After_sales_enquiry_comment();
            c.comment_text = Resources.AfterSales_SiteVisit_SubmitCommentPrefix + comment;
            c.enquiry_id = sv.enquiry_id;
            c.datecreated = DateTime.Now;
            c.respond_to = sv.Enquiry.distributor_id;
            c.respondto_role = CompanyDAL.GetCompanyType(c.respond_to);
            
            After_sales_enquiry_commentDAL.Create(c);

            User creator = UserDAL.GetById(sv.Enquiry.created_userid.Value);
            if (!string.IsNullOrEmpty(creator.user_email))
            {
                MailHelper.SendMail(Properties.Settings.Default.AfterSalesFromAccount, creator.user_email, Resources.AfterSales_SiteVisitNotifyDistributor_MailSubject,
                            string.Format(Resources.AfterSales_SiteVisitNotifyDistributor_MailBody, sv.Enquiry.cust_name, sv.Enquiry.cprod_name, comment, WebUtilities.GetSiteUrl() + Url.Action("Read", new { id = id })),
                            Properties.Settings.Default.AfterSalesCC, Properties.Settings.Default.AfterSalesBcc);
            }
            ViewBag.ShowSendButton = GetContractorId() == null && User.IsInRole(UserRole.MasterDistributor.ToString());
            return View("Read",sv);

        }

        //
        // GET: /AfterSalesSiteVisits/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AfterSalesSiteVisits/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /AfterSalesSiteVisits/Edit/5

        public ActionResult Edit(int id,string returnTo = "")
        {
            WebUtilities.ClearTempFiles();
            WebUtilities.ClearCommentTempFiles();
            ViewBag.returnTo = returnTo;
            var sv = After_sales_enquiry_sitevisitDAL.GetById(id);
            
            ViewData["mode"] = "edit";
            if (sv != null)
            {
                sv.Enquiry = After_sales_enquiryDAL.GetById(sv.enquiry_id, false);
                return View(sv);
            }
            else
            {
                ViewBag.Message = Resources.AfterSalesSiteVisit_Wrongid;
                return View("Message");
            }
        }

        //
        // POST: /AfterSalesSiteVisits/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, After_sales_enquiry_sitevisit m)
        {
            try
            {
                bool modelValid = ModelState.IsValid;
                //if (!modelValid)
                //{ 
                //    //Check for invalid date time format
                //    if(!ModelState.IsValidField("datevisited"))
                //    {
                //        m.datevisited = Utilities.ParseDateFromString(ModelState["datevisited"].Value.AttemptedValue);
                //        modelValid = true;
                //    }
                //}

                if (modelValid)
                {
                    After_sales_enquiry_sitevisit sv = After_sales_enquiry_sitevisitDAL.GetById(id);
                    if (sv != null)
                    {
                        //copy old read-only values
                        m.sitevisit_id = id;
                        m.datecreated = sv.datecreated;
                        m.reference_number = sv.reference_number;
                        m.created_userid = sv.created_userid;
                        m.requestmessage = sv.requestmessage;
                        m.enquiry_id = sv.enquiry_id;
                        m.contractor_id = sv.contractor_id;
                        //m.modified_userid = Utilities.GetCurrentExternalUser().user_id;
                        m.datemodified = DateTime.Now;
                        m.status_id = sv.status_id;
                        m.Files = sv.Files;
                        //if (ase.contractor_id != m.Enquiry.contractor_id)
                        //{ 
                        //    //Send site visit request
                        //}
                        CollectValues(m);
                        bool completed = sv.status_id != m.status_id && m.status_id == After_sales_enquiry_sitevisit_status.Complete;

                        After_sales_enquiry_sitevisitDAL.Update(m);
                        if(completed)
                        {
                            After_sales_enquiry ase = After_sales_enquiryDAL.GetById(m.enquiry_id,false);
                            ase.status_id = After_sales_enquiry_status.SiteVisitReportSubmitted;
                            After_sales_enquiryDAL.Update(ase);
                            if (sv.created_userid != null)
                            {
                                User creator = UserDAL.GetById(sv.created_userid.Value);
                                if (!string.IsNullOrEmpty(creator.user_email))
                                {
                                    External_user contractor = External_userDAL.GetById(sv.contractor_id);
                                    MailHelper.SendMail(Properties.Settings.Default.AfterSalesFromAccount, creator.user_email, Resources.AfterSales_SiteVisitSubmitted_MailSubject,
                                        string.Format(Resources.AfterSales_SiteVisitSubmitted_MailBody, contractor.fullname, ase.cprod_name, ase.cust_name,
                                        string.Format("{0}/AfterSalesSiteVisits/Read/{1}", WebUtilities.GetSiteUrl(), sv.sitevisit_id.ToString())), Properties.Settings.Default.AfterSalesCC, Properties.Settings.Default.AfterSalesBcc);
                                }
                            }
                        }
                        WebUtilities.ClearTempFiles();
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Mode = "edit";
                    return View("Edit", m);
                }

                
            }
            catch
            {
                return View(m);
            }
        }

        

        //
        // GET: /AfterSalesSiteVisits/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /AfterSalesSiteVisits/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Files(object qqfile, string remove)
        {
            ActionResult res = Json(null);
            string fileName = WebUtilities.GetFileName(qqfile, Request);
            if (!string.IsNullOrEmpty(fileName))
            {
                return Json(WebUtilities.SaveTempFile(fileName, Request, Properties.Settings.Default.SiteVisits_MaxFileSize), "text/html");
            }
            //else if (!string.IsNullOrEmpty(remove))
            //    RemoveFile(remove);
            return res;
        }

        private void CollectValues(After_sales_enquiry_sitevisit m)
        {
            if (Request["completed"].Contains("true"))
            {
                m.status_id = After_sales_enquiry_sitevisit_status.Complete;
                m.datecompleted = DateTime.Now;
            }
            m.Files = GetFiles(m.Files, string.Empty + Request["hidDeletedFiles"]);
        }

        private List<After_sales_enquiry_sitevisit_files> GetFiles(List<After_sales_enquiry_sitevisit_files> files, string deletedFiles)
        {
            List<string> written_files = new List<string>();
            if (files == null)
                files = new List<After_sales_enquiry_sitevisit_files>();

            if (!string.IsNullOrEmpty(deletedFiles))
            {
                List<int> deletedFilesIds = (from s in deletedFiles.Split(',') select int.Parse(s)).ToList();
                foreach (var item in deletedFilesIds)
                {
                    After_sales_enquiry_sitevisit_files file = files.Where(f => f.file_id == item).FirstOrDefault();
                    if (file != null)
                    {
                        files.Remove(file);
                        System.IO.File.Delete(Path.Combine(Server.MapPath(Properties.Settings.Default.Enquiries_FilesFolder), file.filename));
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
                        string filePath = Utilities.GetFilePath(kv.Key, Server.MapPath(Properties.Settings.Default.SiteVisits_FilesFolder));
                        StreamWriter sw = new StreamWriter(filePath);
                        MemoryStream ms = new MemoryStream(kv.Value);
                        ms.WriteTo(sw.BaseStream);
                        sw.Close();
                        files.Add(new After_sales_enquiry_sitevisit_files { filename = Path.GetFileName(filePath), created_userid = WebUtilities.GetCurrentExternalUser().user_id, datecreated = DateTime.Now });
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
    }
}
