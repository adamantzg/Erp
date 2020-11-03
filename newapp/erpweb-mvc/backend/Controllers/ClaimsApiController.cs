using company.Common;
using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using backend.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.UI.DataVisualization.Charting;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class ClaimsApiController : ApiController
    {
        public ClaimsApiController(IUnitOfWork unitOfWork, IFeedbackTypeDAL feedbackTypeDAL, ICompanyDAL companyDAL, IReturnCategoryDAL returnCategoryDAL,
            IReturnsDAL returnsDAL, IFeedbackCategoryDAL feedbackCategoryDAL, IReturnsImportanceDAL returnsImportanceDAL,
            ICustproductsDAL custproductsDAL, IUserDAL userDAL, IEmailRecipientsDAL emailRecipientsDAL,
            IReturnsImagesDAL returnsImagesDAL, IRoleDAL roleDAL, IFeedbackSubscriptionsDAL feedbackSubscriptionsDAL,
            IOrderLineExportDal orderLineExportDal, IAccountService accountService, IClaimsService claimsService,
            IMailHelper mailHelper
            )
        {
            this.unitOfWork = unitOfWork;
            this.feedbackTypeDAL = feedbackTypeDAL;
            this.companyDAL = companyDAL;
            this.returnCategoryDAL = returnCategoryDAL;
            this.returnsDAL = returnsDAL;
            this.feedbackCategoryDAL = feedbackCategoryDAL;
            this.returnsImportanceDAL = returnsImportanceDAL;
            this.custproductsDAL = custproductsDAL;
            this.userDAL = userDAL;
            this.emailRecipientsDAL = emailRecipientsDAL;
            this.returnsImagesDAL = returnsImagesDAL;
            this.roleDAL = roleDAL;
            this.feedbackSubscriptionsDAL = feedbackSubscriptionsDAL;
            this.orderLineExportDal = orderLineExportDal;
            this.accountService = accountService;
            this.claimsService = claimsService;
            this.mailHelper = mailHelper;
        }

        [Route("api/claims/")]
        [HttpGet]
        public object GetClaims(int type, int? status1 = 1)
        {
            return unitOfWork.ReturnRepository.GetClaimsSimple(type, status1);
        }

        [Route("api/claims/getModelAll")]
        [HttpGet]
        public object GetAll()
        {
            var claims = unitOfWork.ReturnRepository.GetClaimsSimpleAll(accountService.GetCurrentUser(), Month21.Now.Value, false);
            return new {
                pending = claims.Where(c => c.openclosed != 1),
                completed = claims.Where(c => c.openclosed == 1),
                user_id = accountService.GetCurrentUser().userid,
                month = getMonthObject(Month21.Now),
                asproot = Properties.Settings.Default.aspsite_root,
                types = feedbackTypeDAL.GetAll().Where(t => t.type_id != Feedback_type.ItFeedback).Select(t => t.typename)
            };
        }



        [Route("api/claims/getCompleted")]
        [HttpGet]
        public object GetCompleted(int month21)
        {
            return unitOfWork.ReturnRepository.GetClaimsSimpleAll(accountService.GetCurrentUser(), month21, true);

        }

        [Route("api/claims/getByCriteria")]
        [HttpGet]
        public object GetByCriteria(int month21, int? qc_id = null, bool products = false)
        {
            return unitOfWork.ReturnRepository.GetClaimsSimpleAll(accountService.GetCurrentUser(), month21, true, qc_id, products);

        }

        [Route("api/claims/getByCriteria")]
        [HttpGet]
        public object GetByCriteria(DateTime monthStart, int? qc_id = null, bool products = false, bool currentUserOnly = true)
        {
            return unitOfWork.ReturnRepository.GetClaimsSimpleAll(currentUserOnly ? accountService.GetCurrentUser() : null, Month21.FromDate(monthStart).Value, true, qc_id, products);

        }

        [Route("api/claims/getMonth21")]
        [HttpGet]
        public int GetMonth21(DateTime date)
        {
            return new Month21(date).Value;
        }


        [Route("api/claims/getNextMonth")]
        [HttpGet]
        public object GetNextMonth(int month21)
        {

            return getMonthObject(new Month21(month21) + 1);
        }

        [Route("api/claims/getPreviousMonth")]
        [HttpGet]
        public object GetPreviousMonth(int month21)
        {
            return getMonthObject(new Month21(month21) - 1);
        }

        private object getMonthObject(Month21 m)
        {
            return new { month21 = m.Value, text = m.Date.ToString("MMMM yyyy") };
        }

        [Route("api/claims/getModel")]
        public object GetModel(int? id = null, int? type = null)
        {
            //refactor getting return_no in method
            var feedback_type = feedbackTypeDAL.GetById(type ?? 0);
            WebUtilities.ClearTempFiles();
            var return_no_parts = new[] { feedback_type?.typename, returnsDAL.GetNextFeedbackNum(type ?? 0).ToString("0000") };
            var user = accountService.GetCurrentUser();

            var format = $"{return_no_parts[0]}-{return_no_parts[1]}";
            if(type == Returns.ClaimType_QualityAssurance)
            {
                var date_current = DateTime.Now.ToString("yyyMMdd");

                int num = unitOfWork.ReturnRepository.Get(c => c.return_no.Contains("-QA") && c.return_no.Contains(date_current)).Count() + 1;

                format = $"XX-QA{num:00}X-{date_current}-XX";
                return_no_parts = new[] { "XX", $"QA{num:00}", "X", date_current, "XX" };
            }
            
            var dependencies = type == Returns.ClaimType_ITFeedback ? "Subscriptions.User,Creator,Category,Comments.Creator, Comments.Files, Images,Importance, IssueType" : "Subscriptions.User, Products, Comments.Creator, Comments.Files, Images";                       

            var r = id != null ? 
                type == Returns.ClaimType_ITFeedback  ?  returnsDAL.GetById(id.Value) : unitOfWork.ReturnRepository.Get(re=>re.returnsid == id, includeProperties: dependencies).FirstOrDefault()
                :
                new Returns
                {
                    return_no = format, //$"{feedback_type?.typename}-{ReturnsDAL.GetNextFeedbackNum(type ?? 0):0000}",
                    claim_type = type,
                    status1 = (int) FeedbackStatus.Incomplete,
                    issue_type_id = 1
                };

            feedback_authorization auth = null;
            if (type == Returns.ClaimType_ITFeedback)
            {
                var groupsIds = user.Groups.Select(g => (int?)g.id).ToList();
                auth = unitOfWork.FeedbackAuthorizationRepository.Get(fa => fa.feedback_type_id == type && fa.feedback_issue_type_id == r.issue_type_id
                    && groupsIds.Contains(fa.usergroup_id), includeProperties: "Levels").FirstOrDefault();
                if(id != null)
                    r.Comments.Add(new Returns_comments { comments_to = 1, return_id = r.returnsid, comments = r.client_comments2, comments_date = r.request_date, comments_from = r.request_userid, Creator = r.Creator });
            }

            return new {
                claim = id != null ? GetClaimUIObject(r) : r,
                categories = feedbackCategoryDAL.GetForType(type ?? 0), //source
                imagesRoot = Properties.Settings.Default.returns_fileroot,
                importances = returnsImportanceDAL.GetForType(type ?? 0),
                issueTypes = unitOfWork.FeedbackIssueTypeRepository.Get(),
                clients = companyDAL.GetClients().OrderBy(c => c.customer_code),
                factories = companyDAL.GetFactories().OrderBy(f => f.factory_code),
                returnCategories = returnCategoryDAL.GetAll(),
                return_no_parts = return_no_parts,
                canAuthorizeFeedback = CanAuthorizeFeedback(user, r, auth),
                canCloseFeedback = user.userid == r.request_userid || user.HasPermission(Permission.ITF_CloseFeedback) || CanAuthorizeFeedback(user, r, auth)
            };
        }

        [Route("api/claims/searchProductsByCriteria")]
        public object GetProducts(int? client_id = null, int? factory_id = null)
        {
            /*return Cust_productsDAL.GetCustProductsByCriteria2(prefixText).Where(p => !string.IsNullOrEmpty(p.cprod_code1.Trim()))
                .Select(p => new { p.cprod_id, cprod_code1 = !string.IsNullOrEmpty(p.Client.customer_code) ? $"{p.cprod_code1} ({p.Client.customer_code})" : p.cprod_code1, p.cprod_name, p.factory_ref });*/
            return custproductsDAL.GetByCriteria(discontinued: false, clientIds: client_id != null ? new List<int> { client_id.Value } : null, factoryIds: factory_id != null ? new List<int> { factory_id.Value } : null).
                Select(p => new { p.cprod_id, p.cprod_name, p.cprod_code1 });
        }

        [Route("api/claims/searchUsers")]
        public object GetUsers(string prefixText)
        {
            return userDAL.GetUsersByCriteria(prefixText).Select(u => new { u.userid, u.userwelcome, u.username, u.company_id }).ToList();
        }

       

        //Exists in CaApiController
        /*[Route("api/claims/getTempUrl")]
        [HttpGet]
        public HttpResponseMessage getTempUrl(string file_id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(file_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }*/
        [Route("api/claims/getDefaultSubscribers")]
        [HttpGet]
        public object DefaultRecipient(string claim_type)
        {
            var company_id = accountService.GetCurrentUser().company_id;
            var defaultUserIds = emailRecipientsDAL.GetByCriteria(accountService.GetCurrentUser().company_id, "feedback", claim_type).FirstOrDefault()?.param2?.Split(',')?.Select(s => Convert.ToInt32(s));
            if(defaultUserIds != null)
            {
                var defUsers = unitOfWork.UserRepository.Get(u => defaultUserIds.Any(i => u.userid.Equals(i))).Select(s => s.userwelcome);

                //var defUsers = unitOfWork.UserRepository.Get(u => defaultUserIds.Any(i => u.userid.Equals(i))).Select(s => new { user = new { s.userwelcome }, order = 1 });

                return defUsers;
            }
            return null;
        }

        [Route("api/claims/create")]
        [HttpPost]
        public object Create(Returns r)
        {
            r.request_date = DateTime.Now;
            var user = accountService.GetCurrentUser();

            //return_no logic
            var feedbackType = feedbackTypeDAL.GetById(r.claim_type ?? 0);
            var returns_no_parts = new[] { feedbackType?.typename, returnsDAL.GetNextFeedbackNum(r.claim_type ?? 0).ToString("0000")};
            var format = $"{returns_no_parts[0]}-{returns_no_parts[1]}";

            if(r.claim_type == Returns.ClaimType_QualityAssurance)
            {
                var date_current = DateTime.Now.ToString("yyyMMdd");
                int num = unitOfWork.ReturnRepository.Get(c => c.return_no.Contains("-QA") && c.return_no.Contains(date_current)).Count() + 1;

                format = $"XX-QA{num:00}X-{date_current}-XX";

                returns_no_parts = new[] {"XX", $"QA{num:00}", "X", date_current, "XX" };
            }

            r.return_no = format;

            r.request_userid = user?.userid;
            if (r.client_id == null)
                r.client_id = user?.company_id;
            r.usergroup_id = user?.Groups.FirstOrDefault(g => g.returns_default == true)?.id;
            
            var authorization = unitOfWork.FeedbackAuthorizationRepository.Get(fa => r.usergroup_id == fa.usergroup_id
                            && fa.feedback_type_id == r.claim_type
                            && r.issue_type_id == fa.feedback_issue_type_id, includeProperties: "Levels").FirstOrDefault();

            if (authorization != null && authorization.Levels.Count > 0)
            {
                var level = authorization.Levels.OrderBy(l => l.level).FirstOrDefault();
                r.authorization_level = level.id;

                var group = unitOfWork.UserGroupRepository.Get(g => g.id == level.authorization_usergroupid, includeProperties: "Users").FirstOrDefault();
                if (r.Subscriptions == null)
                    r.Subscriptions = new List<Feedback_subscriptions>();

                r.Subscriptions.AddRange( group.Users.Where(u=> !r.Subscriptions.Select(s=>s.subs_useruserid).Contains(u.userid)).Select(u=> new Feedback_subscriptions { subs_useruserid = u.userid}));                


                if(user.HasPermission(Permission.ITF_Authorize))
                {
                    r.status1 = (int)FeedbackStatus.Live;
                    // ? Authorize(r.returnsid);
                }

            }                
            else
                r.status1 = (int) FeedbackStatus.Live;

           // var feedback_type = Feedback_typeDAL.GetById(r.claim_type ?? 0);
            //r.return_no = $"{feedback_type?.typename}-{ReturnsDAL.GetNextFeedbackNum(r.claim_type ?? 0):0000}";
            HandleFiles(r);
            unitOfWork.ReturnRepository.Insert(r);

            ClaimsController.RegisterEvent(r, ReturnEventType.CorrectiveActionCreate);
            unitOfWork.Save();

            //unitOfWork.ReturnRepository.LoadReference(r, "Category");   //for e-mail
            //unitOfWork.ReturnRepository.LoadReference(r, "Creator");

            //Cust_products p;
            //var recipients = ClaimsController.CreateFeedbackSubscriptions(r, out p, unitOfWork: unitOfWork);

            //ClaimsController.SendEmails(r, recipients, p);

            return GetClaimUIObject(r);
        }

        [Route("api/claims/update")]
        [HttpPut]
        public object Update(Returns r)
        {
            HandleFiles(r);
            unitOfWork.ReturnRepository.Update(r);
            unitOfWork.Save();
            return GetClaimUIObject(r);
        }

        private void HandleFiles(Returns r)
        {
            var rootRelativeFolder = WebUtilities.GetImagesFolder(r.request_date, Properties.Settings.Default.returns_fileroot);

            //Handle images

            if (r.Images != null)
            {
                foreach (var im in r.Images)
                {
                    if (!string.IsNullOrEmpty(im.file_id))
                    {
                        var oFile = WebUtilities.GetTempFile(im.file_id);
                        /*if(oFile == null) {
                            oFile = WebUtilities.GetTempFile(im.insp_image, string.Format("tempMulti_{0}", im.comments));   //comments contains file id from uploader
                        }*/
                        if (oFile != null)
                        {
                            var filePath = company.Common.Utilities.WriteFile(im.return_image, WebUtilities.GetFolderFullPath(rootRelativeFolder, Properties.Settings.Default.returns_fileroot),
                                oFile);
                            im.return_image = WebUtilities.CombineUrls(rootRelativeFolder, Path.GetFileName(filePath));
                        }
                    }
                }
            }
        }


        private void HandleCommentFiles(Returns_comments c)
        {
            var rootRelativeFolder = WebUtilities.GetImagesFolder(c.comments_date, Properties.Settings.Default.returns_fileroot);

            //Handle images

            if (c.Files != null)
            {
                foreach (var im in c.Files)
                {
                    if (!string.IsNullOrEmpty(im.file_id))
                    {
                        var oFile = WebUtilities.GetTempFile(im.file_id);
                        /*if(oFile == null) {
                            oFile = WebUtilities.GetTempFile(im.insp_image, string.Format("tempMulti_{0}", im.comments));   //comments contains file id from uploader
                        }*/
                        if (oFile != null)
                        {
                            var filePath = company.Common.Utilities.WriteFile(im.image_name, WebUtilities.GetFolderFullPath(rootRelativeFolder, Properties.Settings.Default.returns_fileroot),
                                oFile);
                            im.image_name = WebUtilities.CombineUrls(rootRelativeFolder, Path.GetFileName(filePath));
                        }
                    }
                }
            }
        }

        [Route("api/claims/getRecheks")]
        [HttpGet]
        public object GetFeedbacksForRecheck()
        {
            var user = accountService.GetCurrentUser();
            var feedbacks = unitOfWork.ReturnRepository.GetClaimsSimple((int)erp.Model.Returns.ClaimType_CorrectiveAction, ((int)FeedbackStatus.Live))
                .Where(n => (n.request_userid == user.userid || n.assigned_qc == user.userid) && n.recheck_status == null)
                .Select(cs => new {
                    returnsid = cs.returnsid,
                    return_no = cs.return_no,
                    openclosed = cs.openclosed,
                    request_date = cs.request_date,
                    importance_id = cs.importance_id,
                    client_comments = cs.description,
                    Last_Commenter_Name = cs.lastUpdatedBy,
                    Category = new Feedback_category { name = cs.category },
                    HasComments = cs.commentCount > 0,
                    request_userid = cs.request_userid,
                    recheck_required = cs.recheck_required,
                    recheck_date = cs.recheck_date == null ? DateTime.Now : cs.recheck_date,
                    recheck_status = cs.recheck_status,
                    client_comments2 = cs.client_comments2
                }).ToList();

            return feedbacks;
        }

        [Route("api/claims/getRecheck")]
        [HttpGet]
        public object GetRecheck(int? returnsid)
        {
            if (returnsid == null)
                return null;

            var feedback = unitOfWork.ReturnRepository.Get(r => r.returnsid == returnsid)
                .Select(r => new
                {
                    returnsid = r.returnsid,
                    recheck_required = r.recheck_required,
                    recheck_date = r.recheck_date == null ? DateTime.Now : r.recheck_date,
                    recheck_status = r.recheck_status,
                    client_comments2 = r.client_comments2
                }).FirstOrDefault();

            return feedback;
        }

        [Route("api/claims/getResolved")]
        [HttpGet]
        public object GetFeedbacksForResolved(int m)
        {
            var from = DateTime.Now.AddMonths(m);
            var to = DateTime.Now.AddDays(1);
            
            var user = accountService.GetCurrentUser();

            var assignedQCS = unitOfWork.ReturnsUserUserRepository.Get(r => r.useruser_id == user.userid)
                .Select(r => r.return_id).ToList();
                
            var feedbacks = unitOfWork.ReturnRepository.GetClaimsSimple((int)erp.Model.Returns.ClaimType_CorrectiveAction, ((int)FeedbackStatus.Live))
                .Where(n => (
                    n.request_userid == user.userid || assignedQCS.Contains(n.returnsid) /*n.assigned_qc == user.userid*/)
                    && n.recheck_status != null
                    && (n.recheck_date != null && n.recheck_date >= from && n.recheck_date <= to)
                    //&& n.openclosed == 1 
                    //&& (n.closed_date != null && n.closed_date >= from && n.closed_date <= to)
                    )
                .Select(cs => new {
                    returnsid = cs.returnsid,
                    return_no = cs.return_no,
                    openclosed = cs.openclosed,
                    request_date = cs.request_date,
                    importance_id = cs.importance_id,
                    client_comments = cs.description,
                    Last_Commenter_Name = cs.lastUpdatedBy,
                    Category = new Feedback_category { name = cs.category },
                    HasComments = cs.commentCount > 0,
                    request_userid = cs.request_userid,
                    recheck_required = cs.recheck_required,
                    recheck_date = cs.recheck_date,
                    recheck_status = cs.recheck_status,
                    client_comments2 = cs.client_comments2,
                    closed_date = cs.closed_date
                }).ToList();

            return feedbacks;
        }
        [Route("api/claims/createcomment")]
        [HttpPost]
        public object CreateComment(Returns_comments c)
        {
            c.comments_date = DateTime.Now;
            c.comments_from = accountService.GetCurrentUser().userid;
            HandleCommentFiles(c);
            var r = unitOfWork.ReturnRepository.Get(re => re.returnsid == c.return_id, includeProperties: "Comments").FirstOrDefault();
            r.Comments.Add(c);
            unitOfWork.Save();
            SendCommentMail(c);
            return new
            {
                c.return_id,
                c.comments,
                c.comments_date,
                c.comments_from,
                c.comments_id,
                c.comments_to,
                c.comments_type,
                c.decision_flag,
                c.fc_response,
                creator = new { userwelcome = accountService.GetCurrentUser().userwelcome },
                Files = c.Files.Select(f => new
                {
                    f.image_id,
                    f.image_name,
                    f.return_comment_file_id,
                    f.return_comment_id,
                    url = WebUtilities.CombineUrls(Properties.Settings.Default.returns_fileroot, f.image_name)
                })
            };
        }

        [Route("api/claims/createRecheck")]
        [HttpPost]
        public object CreateRecheck(Recheck recheck)
        {
            var clientId = accountService.GetCurrentUser().userid;
            var Images = recheck.Images.RemoveAll(x => x.return_id == null);
            var model = recheck;

            HandleRecheckFiles(model);

            foreach (var item in model.Images)
            {
                item.return_id = model.returnsid;
                item.added_by = clientId;
                item.added_date = DateTime.Now;
            }

            returnsDAL.UpdateSimpleRecheck(model);

            var returns = unitOfWork.ReturnRepository.Get(r => r.returnsid == model.returnsid,includeProperties: "AssignedQCUsers,Products,Images,Subscriptions,Events").FirstOrDefault();

            ClaimsController.RegisterEvent(returns, ReturnEventType.Recheck);

            unitOfWork.Save();

            //var sendMail = new ClaimsController();
            //ClaimsController.SendRecheckUpdated(returns);

            return "ok";
        }

        private string HandleRecheckFiles(Recheck r)
        {
            var rootRelativeFolder = Properties.Settings.Default.returns_fileroot;

            //WebUtilities.GetImagesFolder(r.recheck_date, Properties.Settings.Default.returns_fileroot);

            //Handle images

            if (r.Images != null)
            {
                foreach (var im in r.Images)
                {
                    if (!string.IsNullOrEmpty(im.file_id))
                    {
                        var oFile = WebUtilities.GetTempFile(im.file_id);
                        /*if(oFile == null) {
                            oFile = WebUtilities.GetTempFile(im.insp_image, string.Format("tempMulti_{0}", im.comments));   //comments contains file id from uploader
                        }*/
                        if (oFile != null)
                        {
                            var filePath = company.Common.Utilities.WriteFile(im.return_image, WebUtilities.GetFolderFullPath(rootRelativeFolder, Properties.Settings.Default.returns_fileroot), oFile);
                            if (!string.IsNullOrEmpty(filePath))
                                im.return_image = Path.GetFileName(filePath);
                        }
                    }
                }
            }   

            WebUtilities.ClearTempFiles();
            return "OK";
        }

        [Route("api/claims/getFeedbackImages")]
        [HttpGet]
        public object GetCAFeedbackImages(int returnId, int imageType = 0)
        {
            WebUtilities.ClearTempFiles();

            var images = returnsImagesDAL.GetByReturn(returnId, file_category: imageType)
                .Select(s => new { s.added_by, s.added_date, s.file_id, s.file_category, return_image = $"{backend.Properties.Settings.Default.returns_fileroot}/{s.return_image}" }).ToList();

            return images;
        }

        [Route("api/claims/getStatisticsData")]
        [HttpGet]
        public object GetClaimsStatisticsData(DateTime? from = null, DateTime? to = null, int monthsForSale = 3)
        {
            if (from == null)
                from = company.Common.Utilities.GetMonthStart(DateTime.Now.AddMonths(-1));
            if (to == null)
                to = company.Common.Utilities.GetMonthEnd(DateTime.Now.AddMonths(-1));
            var claims = unitOfWork.ReturnRepository.Get(r => r.request_date >= from && r.request_date <= to && r.claim_type == 0 && r.decision_final == 1 && r.Product.Brand.eb_brand == 1,
                    includeProperties: "Product.Brand, Product.MastProduct.Factory");
            var cprod_ids = claims.Select(r => r.cprod_id).Distinct().ToList();
            //var factory_ids = claims.Select(r => r.Product?.MastProduct?.factory_id).Distinct().ToList();
            var brand_ids = claims.Select(r => r.Product?.brand_id).Distinct().ToList();
            var sales_from = from.Value.AddMonths(-1 * monthsForSale);
            var sales_to = from;
            var statuses = new[] { "X", "Y" };

            /*var salesClaims = unitOfWork.OrderLinesRepository.Get(l => cprod_ids.Contains(l.cprod_id) && l.Header.req_eta >= sales_from && l.Header.req_eta <= sales_to && !statuses.Contains(l.Header.status), 
                        includeProperties: "Header")
                            .GroupBy(l => l.cprod_id)
                            .ToDictionary(g => g.Key, g => g.Sum(l=>l.orderqty * l.unitprice));*/
            var sales = unitOfWork.ReturnRepository.GetOtherProductsSales(sales_from, sales_to, brand_ids: brand_ids);

            var exClaims = claims.Select(c => new {
                id = c.returnsid,
                c.cprod_id,
                c.Product?.cprod_code1,
                c.Product?.cprod_name,
                c.Product?.MastProduct?.Factory?.factory_code,
                c.credit_value,
                c.return_qty,
                brand = c.Product?.Brand?.brandname
                //sales = salesClaims.ContainsKey(c.cprod_id) ? salesClaims[c.cprod_id] : 0
            });


            return new
            {
                data = exClaims.GroupBy(c => c.brand)
                             .Select(gb => new ClaimsStatsRow
                             {
                                 brand = gb.Key,
                                 credit_value = gb.Sum(c => c.credit_value),
                                 sales = (sales.Where(so => so.brandname == gb.Key).Sum(so => so.value) ?? 0) / (monthsForSale * 1.0),
                                 otherValue = (sales.Where(s => s.brandname == gb.Key && !gb.Select(b => b.factory_code).Distinct().Contains(s.factory_code)).Sum(so => so.value) ?? 0) / (monthsForSale * 1.0),
                                 subData = gb.GroupBy(c => c.factory_code)
                                        .Select(gf => new ClaimsStatsRow
                                        {
                                            factory_code = gf.Key,
                                            credit_value = gf.Sum(c => c.credit_value),
                                            sales = (sales.Where(so => so.brandname == gb.Key && so.factory_code == gf.Key).Sum(so => so.value) ?? 0) / (monthsForSale * 1.0),
                                            subData = gf.GroupBy(c => new { c.cprod_id, c.cprod_code1, c.cprod_name })
                                                .Select(gp => new ClaimsStatsRow
                                                {
                                                    cprod_code1 = gp.Key.cprod_code1,
                                                    cprod_name = gp.Key.cprod_name,
                                                    credit_value = gp.Sum(c => c.credit_value),
                                                    return_qty = gp.Sum(c => c.return_qty),
                                                    sales = (sales.Where(so => so.brandname == gb.Key && so.factory_code == gf.Key && so.cprod_id == gp.Key.cprod_id).Sum(so => so.value) ?? 0) / (monthsForSale * 1.0)
                                                }).ToList(),
                                            otherValue = (sales.Where(s => s.brandname == gb.Key && s.factory_code == gf.Key && !gf.Select(b => b.cprod_id).Distinct().Contains(s.cprod_id)).Sum(so => so.value) ?? 0) / (monthsForSale * 1.0)
                                        }).ToList()
                             })
            };
        }

        private const string statsDefaultClients = "85,79,211,95,97,103";

        [Route("api/claims/getBrandsStatistics")]
        [HttpGet]
        public object getBrandStatistics(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            return getBrandStatisticsData(from, to, clients).Select(d => new
            {
                brandname = d.brand,
                total = d.TotalAcceptedValue
            });
        }

        [Route("api/claims/getReasonsStatistics")]
        [HttpGet]
        public object getReasonsStatistics(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            return getReasonStatisticsData(from, to, clients);
        }

        [Route("api/claims/getDecisionStatistics")]
        [HttpGet]
        public object getDecisionStatistics(DateTime from, DateTime to, int? brand_id = null, int? reason_id = null, string clients = statsDefaultClients)
        {
            return getDecisionStatisticsPivotTable(getDecisionStatisticsData(from, to, clients, brand_id, reason_id));
        }

        [Route("api/claims/getBrandByMonthStatistics")]
        [HttpGet]
        public object getBrandByMonthStatistics(DateTime from, DateTime to, string clients = statsDefaultClients, int? reason_id = null, int? decision_id = null)
        {
            return getBrandByMonthPivotTable(getBrandByMonthStatisticsData(from, to, clients, reason_id, decision_id));
        }

        [Route("api/claims/getReasonByMonthStatistics")]
        [HttpGet]
        public object getReasonByMonthStatistics(DateTime from, DateTime to, string clients = statsDefaultClients, int? brand_id = null, int? decision_id = null)
        {
            return getReasonByMonthPivotTable(getReasonByMonthStatisticsData(from, to, clients, brand_id, decision_id));
        }

        
        [Route("api/claims/getDistributorPercentageStats")]
        [HttpGet]
        public object getDistributorPercentageStats(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            return getDistributorPercentageStatsData(from, to, clients);
        }

        [Route("api/claims/getFactoryPercentageStats")]
        [HttpGet]
        public object getFactoryPercentageStats(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            return getFactoryPercentageStatsData(from, to, clients);
        }

        [Route("api/claims/getBrands")]
        [HttpGet]
        public object getBrands(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            return unitOfWork.ReturnRepository.Get(r => r.request_date >= from && r.request_date <= to && r.decision_final == 1 && r.status1 == 1 && clientIds.Contains(r.client_id), includeProperties: "Product.Brand")
                .Where(r => r.Product != null && r.Product.Brand != null)
                .GroupBy(r => new { r.Product.brand_id, r.Product.Brand.brandname })
                .Select(g => new
                {
                    g.Key.brand_id,
                    g.Key.brandname
                });
        }

        [Route("api/claims/getReasons")]
        [HttpGet]
        public object getReasons(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            var result = unitOfWork.ReturnRepository.Get(r => r.request_date >= from && r.request_date <= to && r.decision_final == 1 && r.status1 == 1 && clientIds.Contains(r.client_id), includeProperties: "Product.Brand")
                .Select(r => r.reason)
                .Distinct().ToList();

            return unitOfWork.ReturnCategoryRepository.Get(rc => result.Contains(rc.category_code)).ToList();
        }

        [Route("api/claims/getDecisions")]
        [HttpGet]
        public object getDecisions(DateTime from, DateTime to, string clients = statsDefaultClients)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            return unitOfWork.ReturnRepository.Get(r => r.request_date >= from && r.request_date <= to && r.decision_final > 0 && r.status1 == 1 && clientIds.Contains(r.client_id), includeProperties: "DecisionFinal")
                .GroupBy(r => new { r.DecisionFinal.code, r.DecisionFinal.description })
                .Select(g => new
                {
                    g.Key.code,
                    g.Key.description
                });
        }




        [Route("api/claims/getChart")]
        [HttpGet]
        public HttpResponseMessage getChart(int type, DateTime from, DateTime to, int width = 900, int height = 400, int? brand_id = null, int? reason_id = null, int? decision_id = null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(generateChart(type, from, to, width, height, brand_id, reason_id, decision_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        [Route("api/claims/deactivate")]
        [HttpPost]
        public void Deactivate(int id)
        {

            //unitOfWork.ReturnRepository.Delete(id);
            var claim = unitOfWork.ReturnRepository.GetByID(id);
            claim.status1 = (int)FeedbackStatus.Cancelled;
            unitOfWork.Save();
        }

                           

        
        [Route("api/claims/uploadFile")]
        [HttpPost]
        public object UploadImage()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };
        }
        /*[Route("api/it/getTempFileUrl")]
        [HttpGet]
        public object GetTempFileUrl(string file)
        {
            //var oFile = WebUtilities.GetTempFile(file);
            //if (oFile != null)
            //    return File(oFile, WebUtilities.ExtensionToContentType(Path.GetExtension(file).Replace(".", "")));
            var oFile = new HttpResponseMessage(HttpStatusCode.OK);
            oFile.Content = new ByteArrayContent(WebUtilities.GetTempFile(file));
            oFile.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return oFile;

           

        }*/

        
        [Route("api/claims/openclose")]
        public object OpenClose(int id, int value)
        {
            var claim = unitOfWork.ReturnRepository.GetByID(id);
            if(claim != null)
            {
                claim.openclosed = value;
                unitOfWork.Save();
                return true;
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("api/claims/authorize")]
        [HttpPost]
        public object Authorize(int id)
        {
            var r = unitOfWork.ReturnRepository.Get(re => re.returnsid == id, includeProperties: "Comments, Subscriptions").FirstOrDefault(); //ReturnsDAL.GetById(id);
            var currentUser = accountService.GetCurrentUser();
            var groupIds = currentUser?.Groups.Select(g => (int?)g.id).ToList();
            var authorization = unitOfWork.FeedbackAuthorizationRepository.Get(a => groupIds.Contains(a.usergroup_id) && a.feedback_type_id == r.claim_type && a.feedback_issue_type_id == r.issue_type_id, includeProperties: "Levels").FirstOrDefault();
            var status = FeedbackStatus.Live;
            var recipients = roleDAL.GetUsersInRole(Role.ITAdmin);
            var showButton = false;

            var authLevelCommentText = string.Empty;

            if (authorization != null)
            {
                var maxLevel = authorization.Levels.Max(l => l.level);
                var authLevel = authorization.Levels.FirstOrDefault(l => l.id == r.authorization_level);
                if (authLevel != null && authLevel.level < maxLevel)
                {
                    var nextLevel = authorization.Levels.FirstOrDefault(l => l.level == authLevel.level + 1);
                    if (nextLevel != null)
                    {
                        authLevelCommentText = $"(Level: {authLevel.level})";
                        status = FeedbackStatus.Incomplete;
                        var group = unitOfWork.UserGroupRepository.Get(g => g.id == nextLevel.authorization_usergroupid, includeProperties: "Users").FirstOrDefault();
                        recipients = group.Users;
                        r.authorization_level = nextLevel.id;
                        if (currentUser.Groups.Any(g => g.id == group.id))
                            //retain authorization button if same user can authorize on next level
                            showButton = true;
                    }
                }
            }

            r.status1 = (int?) status;
            //ReturnsDAL.UpdateStatus(id, status, r.authorization_level);
            r.Comments.Add(new Returns_comments
            {
                comments_from = currentUser.userid,
                comments = $"Authorization {authLevelCommentText}. Ip address: {HttpContext.Current.Request.UserHostAddress}",
                comments_date = DateTime.Now
            });
            unitOfWork.Save();

            foreach (var re in recipients)
            {
                if (r.Subscriptions.Count(s => s.subs_useruserid == re.userid) == 0)
                    feedbackSubscriptionsDAL.Create(new Feedback_subscriptions { subs_returnid = r.returnsid, subs_useruserid = re.userid });
            }
            
            return showButton;
        }

        [Route("api/claims/saveSubscription")]
        [HttpPost]
        public object SaveSubscription(Feedback_subscriptions sub)
        {
            var claim = unitOfWork.ReturnRepository.Get(r => r.returnsid == sub.subs_returnid, includeProperties: "Subscriptions").FirstOrDefault();
            if(claim != null)
            {
                claim.Subscriptions.Add(sub);
                unitOfWork.Save();
                return true;
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [Route("api/claims/removeSubscription")]
        [HttpPost]
        public object RemoveSubscription(Feedback_subscriptions sub)
        {
            var claim = unitOfWork.ReturnRepository.Get(r => r.returnsid == sub.subs_returnid, includeProperties: "Subscriptions").FirstOrDefault();
            if (claim != null)
            {
                var s = claim.Subscriptions.FirstOrDefault(fs => fs.subs_useruserid == sub.subs_useruserid);
                if (s != null)
                {
                    claim.Subscriptions.Remove(s);
                    unitOfWork.Save();
                }
                return true;
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        private bool CanAuthorizeFeedback(User user, Returns f, feedback_authorization auth)
        {
            if (auth == null)
                return user.HasPermission(Permission.ITF_Authorize);
            var level = auth.Levels.FirstOrDefault(l => l.id == f.authorization_level);
            if (level != null)
                return user.Groups.Any(g => g.id == level.authorization_usergroupid);
            return false;
        }

        private List<ReturnAggregateDataProduct> getBrandStatisticsData(DateTime from, DateTime to, string clients)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            return unitOfWork.ReturnRepository
                .GetQuery(r => r.request_date >= from && r.request_date <= to && r.decision_final == 1 && r.status1 == 1 && clientIds.Contains(r.client_id), includeProperties: "Product.Brand")
                .GroupBy(r => new { r.Product.brand_id, r.Product.Brand.brandname })
                .Select(g => new ReturnAggregateDataProduct
                {
                    brand = g.Key.brandname,
                    TotalAcceptedValue = g.Sum(r => r.return_qty * r.credit_value)
                }).ToList().Where(g => !string.IsNullOrEmpty(g.brand)).OrderByDescending(g => g.TotalAcceptedValue).ToList();
        }

        private List<ReturnAggregateDataProduct> getReasonStatisticsData(DateTime from, DateTime to, string clients)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            var result = unitOfWork.ReturnRepository
                .GetQuery(r => r.request_date >= from && r.request_date <= to && r.decision_final == 1 && r.status1 == 1 && clientIds.Contains(r.client_id), includeProperties: "Product.Brand")
                .GroupBy(r => r.reason)
                .Select(g => new ReturnAggregateDataProduct
                {
                    Reason = g.Key,
                    TotalAcceptedValue = g.Sum(r => r.return_qty * r.credit_value)
                }).ToList();
            var returnCategories = unitOfWork.ReturnCategoryRepository.Get().ToList();
            return result.GroupBy(r => returnCategories.FirstOrDefault(rc=>rc.category_code == r.Reason)?.category_name)
                .Select(g=> new ReturnAggregateDataProduct
                {
                    Reason = g.Key ?? "Unknown",
                    TotalAcceptedValue = g.Sum(r=>r.TotalAcceptedValue)
                }).OrderByDescending(g => g.TotalAcceptedValue).ToList();
        }

        private List<ReturnAggregateDataProduct> getDecisionStatisticsData(DateTime from, DateTime to, string clients, int? brand_id = null, int? reason_id = null)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            var reason = unitOfWork.ReturnCategoryRepository.Get(rc => rc.returncategory_id == reason_id).FirstOrDefault()?.category_code;
            return unitOfWork.ReturnRepository
                .GetQuery(r => r.request_date >= from && r.request_date <= to && r.decision_final > 0 && r.status1 == 1 && clientIds.Contains(r.client_id)
                                && (brand_id == null || r.Product.brand_id == brand_id) && (reason == null || r.reason == reason) && r.claim_type != 5,
                includeProperties: "DecisionFinal, Product")
                .Select(r => new { r.request_date.Value.Year, r.request_date.Value.Month, r.DecisionFinal.description, r.return_qty, r.credit_value })
                .GroupBy(r => new { r.Year, r.Month, r.description })
                .Select(g => new ReturnAggregateDataProduct
                {
                    Decision = g.Key.description,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAcceptedValue = g.Sum(r => r.return_qty * r.credit_value)
                }).ToList().Where(g => !string.IsNullOrEmpty(g.Decision)).OrderBy(g => g.Year).ThenBy(g=>g.Month).ToList();
        }

        private object getDecisionStatisticsPivotTable(List<ReturnAggregateDataProduct> rawData)
        {
            var columns = rawData.Select(d => d.Decision).Distinct().OrderBy(d => d).ToList();

            var rows = rawData.GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        formattedDate = new DateTime(g.Key.Year.Value, g.Key.Month.Value, 1).ToString("MMM-yy"),
                        columns = columns.Select(c => new { c, g.FirstOrDefault(r => r.Decision == c)?.TotalAcceptedValue }),
                        rowTotal = g.Sum(r => r.TotalAcceptedValue ?? 0)
                    }).ToList();
            var grandTotal = new
            {
                columns = columns.Select(c=> new { c, TotalAcceptedValue = rawData.Where(d => d.Decision == c).Sum(d => d.TotalAcceptedValue) }),
                rowTotal = rawData.Sum(d=>d.TotalAcceptedValue ?? 0)
            };
            return new { columns, rows, grandTotal };
        }

        private List<ReturnAggregateDataProduct> getBrandByMonthStatisticsData(DateTime from, DateTime to, string clients, int? reason_id = null, int? decision_id = null)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            var reason = unitOfWork.ReturnCategoryRepository.Get(rc => rc.returncategory_id == reason_id).FirstOrDefault()?.category_code;
            return unitOfWork.ReturnRepository
                .GetQuery(r => r.request_date >= from && r.request_date <= to && r.status1 == 1 && clientIds.Contains(r.client_id)
                        && (reason == null || r.reason == reason) && (decision_id == null || r.decision_final == decision_id)
                , includeProperties: "Product.Brand")
                .Select(r => new { r.request_date.Value.Year, r.request_date.Value.Month, r.Product.Brand.brandname, r.return_qty, r.credit_value })
                .GroupBy(r => new { r.Year, r.Month, r.brandname })
                .Select(g => new ReturnAggregateDataProduct
                {
                    brand = g.Key.brandname,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAcceptedValue = g.Sum(r => r.return_qty * r.credit_value)
                }).ToList().Where(g => !string.IsNullOrEmpty(g.brand)).OrderBy(g => g.Year).ThenBy(g => g.Month).ToList();
        }

        private object getBrandByMonthPivotTable(List<ReturnAggregateDataProduct> rawData)
        {
            var columns = rawData.Select(d => d.brand).Distinct().OrderBy(d => d).ToList();

            var rows = rawData.GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        formattedDate = new DateTime(g.Key.Year.Value, g.Key.Month.Value, 1).ToString("MMM-yy"),
                        columns = columns.Select(c => new { c, g.FirstOrDefault(r => r.brand == c)?.TotalAcceptedValue }),
                        rowTotal = g.Sum(r => r.TotalAcceptedValue ?? 0)
                    }).ToList();
            var grandTotal = new
            {
                columns = columns.Select(c => new { c, TotalAcceptedValue = rawData.Where(d => d.brand == c).Sum(d => d.TotalAcceptedValue) }),
                rowTotal = rawData.Sum(d => d.TotalAcceptedValue ?? 0)
            };
            return new { columns, rows, grandTotal };
        }

        private List<ReturnAggregateDataProduct> getReasonByMonthStatisticsData(DateTime from, DateTime to, string clients, int? brand_id = null, int? decision_id = null)
        {
            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clients);
            var result = unitOfWork.ReturnRepository
                .GetQuery(r => r.request_date >= from && r.request_date <= to && r.status1 == 1 && clientIds.Contains(r.client_id)
                            && (brand_id == null || r.Product.brand_id == brand_id) && (decision_id == null || r.decision_final == decision_id)
                , includeProperties: "Product")
                .Select(r => new { r.request_date.Value.Year, r.request_date.Value.Month, r.reason, r.return_qty, r.credit_value })
                .GroupBy(r => new { r.Year, r.Month, r.reason })
                .Select(g => new ReturnAggregateDataProduct
                {
                    Reason = g.Key.reason,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAcceptedValue = g.Sum(r => r.return_qty * r.credit_value)
                }).ToList().Where(g => !string.IsNullOrEmpty(g.Reason)).OrderBy(g => g.Year).ThenBy(g => g.Month).ToList();

            var returnCategories = unitOfWork.ReturnCategoryRepository.Get().ToList();
            return result.GroupBy(r => new { r.Year,r.Month, returnCategories.FirstOrDefault(rc => rc.category_code == r.Reason)?.category_name })
                .Select(g => new ReturnAggregateDataProduct
                {
                    Reason = g.Key.category_name ?? "Unknown",
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAcceptedValue = g.Sum(r => r.TotalAcceptedValue)
                }).OrderBy(g => g.Year).ThenBy(g => g.Month).ToList();
        }

        private object getReasonByMonthPivotTable(List<ReturnAggregateDataProduct> rawData)
        {
            var columns = rawData.Select(d => d.Reason).Distinct().OrderBy(d => d).ToList();

            var rows = rawData.GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        formattedDate = new DateTime(g.Key.Year.Value, g.Key.Month.Value, 1).ToString("MMM-yy"),
                        columns = columns.Select(c => new { c, g.FirstOrDefault(r => r.Reason == c)?.TotalAcceptedValue }),
                        rowTotal = g.Sum(r => r.TotalAcceptedValue ?? 0)
                    }).ToList();
            var grandTotal = new
            {
                columns = columns.Select(c => new { c, TotalAcceptedValue = rawData.Where(d => d.Reason == c).Sum(d => d.TotalAcceptedValue) }),
                rowTotal = rawData.Sum(d => d.TotalAcceptedValue ?? 0)
            };
            return new { columns, rows, grandTotal };
        }

        private List<PercentageStatsRow> getDistributorPercentageStatsData(DateTime from, DateTime to, string clients)
        {
            var distributors = companyDAL.GetDistributors();
            var sales = orderLineExportDal.GetCustomerSummaryForPeriod(from, to);
            var returnsSummary =
                            returnsDAL.GetTotalsPerClient(from, to)
                                      .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                                      .ToList();
            return distributors.Select(d => new PercentageStatsRow{
                code = d.customer_code,
                acceptedGbp = returnsSummary.FirstOrDefault(r => r.code == d.customer_code)?.TotalAccepted ?? 0,
                totalSalesGbp = sales.FirstOrDefault(s=>s.code == d.customer_code)?.total ?? 0
            }).Where(r=>r.acceptedGbp > 0 && r.totalSalesGbp > 0).ToList();

        }

        private List<PercentageStatsRow> getFactoryPercentageStatsData(DateTime from, DateTime to, string clients)
        {
            var factories = companyDAL.GetFactories();
            var sales = orderLineExportDal.GetFactorySummaryForPeriod(from, to);
            var returnsSummary =
                            returnsDAL.GetTotalsPerFactory(from, to)
                                      .Where(r => r.TotalAccepted > 0 )
                                      .ToList();
            return factories.Select(f => new PercentageStatsRow
            {
                code = f.factory_code,
                acceptedGbp = returnsSummary.FirstOrDefault(r => r.code == f.factory_code)?.TotalAccepted ?? 0,
                totalSalesGbp = sales.FirstOrDefault(s => s.code == f.factory_code)?.total ?? 0
            }).Where(r => r.acceptedGbp > 0 && r.totalSalesGbp > 0).ToList();

        }


        private byte[] generateChart(int type, DateTime from, DateTime to, int width, int height, int? brand_id = null, int? reason_id = null, int? decision_id = null)
        {
            Chart result  = null;
            switch(type)
            {
                case 0:
                    result = generateBrandChart(from, to,width, height);
                    break;
                case 1:
                    result = generateReasonChart(from, to, width, height);
                    break;
                case 2:
                    result = generateDecisionByMonthChart(from, to, width, height,brand_id:  brand_id, reason_id: reason_id);
                    break;
                case 3:
                    result = generateBrandByMonthChart(from, to, width, height, reason_id: reason_id, decision_id: decision_id);
                    break;
                case 4:
                    result = generateReasonByMonthChart(from, to, width, height,brand_id: brand_id, decision_id: decision_id);
                    break;
                case 5:
                    result = generateAcceptedSalesRatioChart(getDistributorPercentageStatsData(from, to,statsDefaultClients),"Distributor", width, height);
                    break;
                case 6:
                    result = generateAcceptedSalesRatioChart(getFactoryPercentageStatsData(from, to, statsDefaultClients),"Factory", width, height);
                    break;
            }
            if(result != null)
            {
                MemoryStream ms = new MemoryStream();
                result.SaveImage(ms, ChartImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.GetBuffer();
            }

            return null;
        }

        

        public const string summaryDefaultClients = statsDefaultClients;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFeedbackTypeDAL feedbackTypeDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IReturnCategoryDAL returnCategoryDAL;
        private readonly IReturnsDAL returnsDAL;
        private readonly IFeedbackCategoryDAL feedbackCategoryDAL;
        private readonly IReturnsImportanceDAL returnsImportanceDAL;
        private readonly ICustproductsDAL custproductsDAL;
        private readonly IUserDAL userDAL;
        private readonly IEmailRecipientsDAL emailRecipientsDAL;
        private readonly IReturnsImagesDAL returnsImagesDAL;
        private readonly IRoleDAL roleDAL;
        private readonly IFeedbackSubscriptionsDAL feedbackSubscriptionsDAL;
        private readonly IOrderLineExportDal orderLineExportDal;
        private readonly IAccountService accountService;
        private readonly IClaimsService claimsService;
        private readonly IMailHelper mailHelper;

        private Chart generateBrandChart(DateTime from, DateTime to, int width, int height, string clients = summaryDefaultClients)
        {
            var data = getBrandStatisticsData(from, to,clients);
            var chart = SetupColumnChart("Brand", "Total",width, height);

            foreach (var d in data)
            {
                chart.Series[0].Points.AddXY(d.brand, d.TotalAcceptedValue);
            }
            return chart;
            
        }

        private Chart generateReasonChart(DateTime from, DateTime to, int width, int height, string clients = summaryDefaultClients)
        {
            var data = getReasonStatisticsData(from, to, clients);
            var chart = SetupColumnChart("Reason", "Total", width, height);

            foreach (var d in data)
            {
                chart.Series[0].Points.AddXY(d.Reason, d.TotalAcceptedValue);
            }
            return chart;

        }

        private Chart generateDecisionByMonthChart(DateTime from, DateTime to, int width, int height, string clients = summaryDefaultClients, int? brand_id = null, int? reason_id = null)
        {
            var data = getDecisionStatisticsData(from, to, clients,brand_id, reason_id);
            var chart = SetupColumnChart("Month", "Total", width, height);
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "MMM-yy";
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                        
            chart.Series.Clear();

            var months = data.GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToList();
                        
            foreach(var g in data.GroupBy(d=>d.Decision))
            {
                var series = new Series
                {
                    ChartType = SeriesChartType.StackedColumn,
                    LegendText = g.Key
                };

                foreach (var m in months)
                {
                    series.Points.AddXY(new DateTime(m.Year.Value, m.Month.Value,1), g.FirstOrDefault(d=>d.Year == m.Year && d.Month == m.Month)?.TotalAcceptedValue);
                }
                chart.Series.Add(series);
            }

            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            chart.Legends.Add(legend);

            return chart;
        }

        private Chart generateBrandByMonthChart(DateTime from, DateTime to, int width, int height, string clients = summaryDefaultClients, int? reason_id = null, int? decision_id = null)
        {
            var data = getBrandByMonthStatisticsData(from, to, clients,reason_id, decision_id);
            var chart = SetupColumnChart("Month", "Total", width, height);
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "MMM-yy";
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;

            chart.Series.Clear();

            var months = data.GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToList();

            foreach (var g in data.GroupBy(d => d.brand))
            {
                var series = new Series
                {
                    ChartType = SeriesChartType.Line,
                    LegendText = g.Key
                };

                foreach (var m in months)
                {
                    series.Points.AddXY(new DateTime(m.Year.Value, m.Month.Value, 1), g.FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month)?.TotalAcceptedValue);
                }
                chart.Series.Add(series);
            }

            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            chart.Legends.Add(legend);

            return chart;
        }

        private Chart generateReasonByMonthChart(DateTime from, DateTime to, int width, int height, string clients = summaryDefaultClients, int? brand_id = null, int? decision_id = null)
        {
            var data = getReasonByMonthStatisticsData(from, to, clients,brand_id, decision_id);
            var chart = SetupColumnChart("Month", "Total", width, height);
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "MMM-yy";
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;

            chart.Series.Clear();

            var months = data.GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToList();

            foreach (var g in data.GroupBy(d => d.Reason))
            {
                var series = new Series
                {
                    ChartType = SeriesChartType.Line,
                    LegendText = g.Key
                };

                foreach (var m in months)
                {
                    series.Points.AddXY(new DateTime(m.Year.Value, m.Month.Value, 1), g.FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month)?.TotalAcceptedValue);
                }
                chart.Series.Add(series);
            }

            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            chart.Legends.Add(legend);

            return chart;
        }

        private Chart generateAcceptedSalesRatioChart(List<PercentageStatsRow> data, string title, int width, int height)
        {
            var chart = SetupColumnChart(title, "claim GBP as % of sales ", width, height);
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;

            foreach (var d in data)
            {
                chart.Series[0].Points.AddXY(d.code, d.percentage);
            }
            return chart;
        }

        private Chart SetupColumnChart(string xTitle, string yTitle,int width, int height)
        {
            var result = new Chart { Width = width, Height = height };
            var area = new ChartArea();
            area.AxisY.IsInterlaced = true;
            area.AxisY.InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE);

            area.AxisX.Title = xTitle;            
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = yTitle;

            result.ChartAreas.Add(area);

            var series = new Series
            {
                ChartType = SeriesChartType.Column                
            };
            result.Series.Add(series);
            return result;
        }

        private void SendCommentMail(Returns_comments c)
        {
            var r = unitOfWork.ReturnRepository.Get(re => re.returnsid == c.return_id, includeProperties: "Subscriptions.User, Creator").FirstOrDefault();
            var currUser = accountService.GetCurrentUser();
            if(r != null)
            {
                var subject = string.Format(App_GlobalResources.Resources.Feedback_comment_subject, r.return_no);

                var resBody =
                    App_GlobalResources.Resources.ResourceManager.GetString(string.Format("Feedback_comment_{0}_body", r.claim_type));
                resBody = string.Format(resBody, accountService.GetCurrentUser().userwelcome, c.comments, WebUtilities.GetSiteUrl(), r.returnsid);
                if(r.Subscriptions != null)
                {
                    var subscriberEmails = string.Join(",", r.Subscriptions.Where(s => s.subs_useruserid != accountService.GetCurrentUser().userid && s.User != null && s.User.user_email?.Length > 0).Select(s => s.User.user_email));
                    var emailRecipientsDb = emailRecipientsDAL.GetByCriteria(accountService.GetCurrentUser().company_id, "feedback", r.claim_type.ToString(),
                                 r.claim_type == Feedback_type.ItFeedback ? accountService.GetCurrentUser().Groups.FirstOrDefault(g => g.returns_default == true)?.id : null).FirstOrDefault();
                    var creatorEmails = r.Creator != null ? r.Creator.user_email : "";
                    string to = string.Empty, cc = string.Empty, bcc = string.Empty;
                    if (emailRecipientsDb != null)
                    {
                        to = claimsService.SubstituteMacros(emailRecipientsDb.to, subscriberEmails, creatorEmails);
                        cc = claimsService.SubstituteMacros(emailRecipientsDb.cc, subscriberEmails, creatorEmails);
                        bcc = claimsService.SubstituteMacros(emailRecipientsDb.bcc, subscriberEmails, creatorEmails);
                    }
                    else
                    {
                        to = subscriberEmails;
                        if (r.Creator != null && r.Creator.userid != accountService.GetCurrentUser().userid && !string.IsNullOrEmpty(r.Creator.user_email)
                            && (c.comments_to == 1 || (currUser.IsUserIT() && r.Creator.IsUserIT()) || (currUser.IsUserFC() && r.Creator.IsUserFC())))
                        {
                            to += "," + r.Creator.user_email;
                        }
                    }
                    if (!string.IsNullOrEmpty(to))
                    {
                        mailHelper.SendMail(Properties.Settings.Default.Feedback_From, to, subject, resBody,cc,bcc,
                        c.Files.Select(f => new System.Net.Mail.Attachment(Path.Combine(HttpContext.Current.Server.MapPath(Properties.Settings.Default.returns_fileroot), f.image_name))).ToArray(),
                        removeCurrentUser: true);
                    }
                }
                
            }
        }


        private object GetClaimUIObject(Returns r)
        {
            return new
            {
                r.returnsid,
                r.return_no,
                r.return_qty,
                r.cprod_id,
                r.request_date,
                r.cc_response_date,
                r.client_comments,
                r.client_comments2,
                r.request_userid,
                r.status1,
                r.claim_type,
                r.feedback_category_id,
                r.reason,
                r.client_id,
                r.issue_type_id,
                factory_id = GetFactoryId(r),
                r.openclosed,
                subscriptions = r.Subscriptions?.Select(s => new {
                    s.subs_id,
                    s.subs_useruserid,
                    s.subs_returnid,
                    user = new { s.User?.userid, s.User?.userwelcome }
                }),
                products = r.Products?.Select(p=> new { p.cprod_id, p.cprod_code1, p.cprod_name}),
                images = r.Images?.Select(im=> new { im.image_unique, im.return_image,im.return_id, url = WebUtilities.CombineUrls(Properties.Settings.Default.returns_fileroot, im.return_image) }),
                comments = r.Comments?.Select(c=> new {
                    c.comments_id,
                    c.comments,
                    c.comments_date,
                    c.comments_type,
                    c.return_id,
                    c.comments_to,
                    c.comments_from,
                    creator =  new { userwelcome = c.Creator?.userwelcome },
                    files = c.Files?.Select(f=> new { f.image_id, f.return_comment_id, f.image_name,f.return_comment_file_id, url = WebUtilities.CombineUrls(Properties.Settings.Default.returns_fileroot, f.image_name) })
                }),
                Category = r.Category != null ? new {r.Category.feedback_cat_id, r.Category.name} : null,
                IssueType = r.IssueType != null ? new {r.IssueType.id, r.IssueType.name} : null,
                Importance = r.Importance != null ? new {r.Importance.importance_id, r.Importance.importance_text} : null,
                Creator = r.Creator != null ? new {r.Creator.userid, r.Creator.username, r.Creator.userwelcome} : null
            };
        }
        
        private int? GetFactoryId(Returns r)
        {
            int? factory_id = 0;

            if (r.Products != null)
            {
                var prod = r.Products.FirstOrDefault();

                if (prod != null)
                {
                    var prod_info = unitOfWork.CustProductRepository.Get(cp => cp.cprod_id == prod.cprod_id, includeProperties: "MastProduct.Factory").FirstOrDefault();

                    r.factory = prod_info?.MastProduct?.Factory?.factory_code;
                    factory_id = prod_info?.MastProduct?.Factory?.user_id;
                }
            }

            return factory_id;

        }
    }

    public class PercentageStatsRow
    {
        public string code { get; set; }
        public double? acceptedGbp { get; set; }
        public double? totalSalesGbp { get; set; }
        public double? percentage
        {
            get
            {
                if (totalSalesGbp == 0)
                    return 0;
                return acceptedGbp / totalSalesGbp * 100;
            }
        }
    }
}