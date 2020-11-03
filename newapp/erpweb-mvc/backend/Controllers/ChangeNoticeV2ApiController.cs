using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.DAL.EF.New;
using erp.Model;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
using Utilities = company.Common.Utilities;
using erp.Model.Dal.New;
using backend.Models;
using backend.Properties;
using backend.ApiServices;

namespace backend.Controllers
{
	[Authorize]
    public class ChangeNoticeV2ApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICategory1DAL category1DAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IOrderLinesDAL orderLinesDAL;
        private readonly IAccountService accountService;

        public ChangeNoticeV2ApiController(IUnitOfWork unitOfWork, ICategory1DAL category1DAL, ICompanyDAL companyDAL, 
            IOrderLinesDAL orderLinesDAL, IAccountService accountService )
        {
            this.unitOfWork = unitOfWork;
            this.category1DAL = category1DAL;
            this.companyDAL = companyDAL;
            this.orderLinesDAL = orderLinesDAL;
            this.accountService = accountService;
        }

        [Route("api/changenoticev2/GetChangeNoticeV2Model")]
        [HttpGet]
        public object GetChangeNoticeV2Model(string state, int? id = null)
        {
            var model = new ChangeNoticeV2Model
            {
                Categories = category1DAL.GetAll().ToList(),
                Clients = companyDAL.GetClientsFromProducts().ToList(),
                Factories = companyDAL.GetFactories().ToList(),
                ChangeNoticeCategories = unitOfWork.ReturnCategoryRepository.Get().ToList(),
                ChangeNoticeReasons = unitOfWork.ChangeNoticeReasonsRepository.Get().ToList(),
                Notice = state == "create" ? new Change_notice { Images = new List<change_notice_image>(), Allocations = new List<Change_notice_allocation>(), expectedReadyDate = DateTime.Today, Document = new change_notice_document()} : 
                            state == "edit" || state =="view" ? unitOfWork.ChangeNoticeRepository.Get(n => n.id == id, includeProperties: "Allocations.Product.BrandCompany,Allocations.Orders.Client,Images,Document").FirstOrDefault() : null,
                ImageRootFolder = Settings.Default.ChangeNoticeRootFolder,
                Statuses = getStatuses()
            };

            if (id != null)
            {
                var cprod_id = state == "create" ? id : model.Notice.Allocations.FirstOrDefault()?.cprod_id;

                if (cprod_id != null)
                {
                    model.Product = unitOfWork.CustProductRepository.Get(p => p.cprod_id == cprod_id, includeProperties: "BrandCompany,MastProduct").FirstOrDefault();
                    model.factory_id = model.Product?.MastProduct?.factory_id;
                    
                }
                if(state == "edit")
                {
                    model.Orders = loadClientsOrdersForProducts(model.Notice.Allocations.Select(a => a.cprod_id.Value).ToList(),
                        model.Notice.expectedReadyDate,
                        model.Notice.Allocations.SelectMany(a => a.Orders.Select(o => new { a.cprod_id, order = o })).Select(o => new ChangeNoticeOrder { orderid = o.order.orderid, userid1 = o.order.userid1, cprod_id = o.cprod_id }).ToList());
                }
                
            }
            
            return model;
        }

        [Route("api/changenoticev2/searchProducts")]
        [HttpGet]
        public object Products(int? factory_id = null, int? client_id = null, int? category1_id = null)
        {
            return unitOfWork.CustProductRepository.Get(p => (p.brand_userid == client_id || client_id == null)
            && (p.MastProduct.factory_id == factory_id || factory_id == null)
            && (p.MastProduct.category1 == category1_id || category1_id == null) && p.cprod_status == "N" && p.cprod_code1 != null, includeProperties: "BrandCompany,Mastproduct").
            Select(p => new
            {
                p.cprod_id,
                p.cprod_code1,
                p.cprod_name,
                BrandCompany = new { p.BrandCompany?.customer_code }
            });               
        }

        [Route("api/changenoticev2/loadClientsOrdersForProduct")]
        [HttpGet]
        public object loadClientsOrdersForProduct(int cprod_id, DateTime? etd = null)
        {
            if (etd == null)
                etd = DateTime.Today;
            return orderLinesDAL.GetByExportCriteria(new[] { cprod_id }, etd_from: etd).
                GroupBy(l => l.orderid).
                Select(g=> new { orderid= g.Key, g.First().Header?.custpo, g.First().Header?.po_req_etd, g.First().Header?.userid1, g.First().Header?.Client?.customer_code}).
                GroupBy(o=>o.userid1).
                Select(g => new
                {
                    userid1 = g.Key,
                    customer_code = g.First().customer_code,
                    OrderList = g.ToList().OrderBy(o=>o.po_req_etd)
                }).OrderBy(r=>r.customer_code);
        }

        [Route("api/changenoticev2/loadClientsOrdersForProducts")]
        [HttpGet]
        public object loadClientsOrdersForProducts(string cprod_ids, DateTime? etd = null, List<ChangeNoticeOrder> orders = null)
        {
            var cprodIdsList = Utilities.GetIdsFromString(cprod_ids);            
            return loadClientsOrdersForProducts(cprodIdsList, etd, orders);
        }

        private object loadClientsOrdersForProducts(List<int> cprodIdsList, DateTime? etd = null, List<ChangeNoticeOrder> orderList = null)
        {
            if (etd == null)
                etd = DateTime.Today;
            var lines = orderLinesDAL.GetByExportCriteria(cprodIdsList, etd_from: etd);
            if (orderList != null)
            {
                var filtered = orderList.Where(o => !lines.Select(li => li.orderid).Distinct().Contains(o.orderid)).ToList();
                if(filtered.Count > 0)
                    lines.AddRange(orderLinesDAL.GetByExportCriteria(cprod_ids: cprodIdsList, orderids: filtered.Select(o=>o.orderid).ToList() ));
            }
                

            return lines.GroupBy(l=>l.cprod_id).
                Select(gp=> new {cprod_id =  gp.Key,
                    orders = gp.ToList().
                            GroupBy(l => l.orderid).
                            Select(g => new { orderid = g.Key, g.First().Header?.custpo, g.First().Header?.po_req_etd, g.First().Header?.userid1, g.First().Header?.Client?.customer_code }).
                            GroupBy(o => o.userid1).
                            Select(g => new
                            {
                                userid1 = g.Key,
                                customer_code = g.First().customer_code,
                                OrderList = g.ToList().OrderBy(o => o.po_req_etd),
                                orderid = orderList.FirstOrDefault(o=>o.cprod_id == gp.Key && o.userid1 == g.Key)?.orderid
                            }).OrderBy(r => r.customer_code)
                }).ToDictionary(l=>l.cprod_id,l=>l.orders);
        }

        

        [Route("api/changenoticev2/create")]
        [HttpPost]
        public object Create(Change_notice n)
        {
            n.datecreated = DateTime.Now;
            n.createdById = accountService.GetCurrentUser().userid;
			n.status = Change_notice.status_pending;

            CollectFiles(n);

            unitOfWork.ChangeNoticeRepository.Insert(n);
            unitOfWork.Save();

            WebUtilities.ClearTempFiles("temp_");

            return GetNoticeUIObject(n);
        }

        [Route("api/changenoticev2/update")]
        [HttpPut]
        public object Update(Change_notice n)
        {
            n.dateModified = DateTime.Now;
            n.modifiedById = accountService.GetCurrentUser().userid;

            CollectFiles(n);

            unitOfWork.ChangeNoticeRepository.Update(n);
            unitOfWork.Save();

            WebUtilities.ClearTempFiles("temp_");

            return GetNoticeUIObject(n);
        }

        private void CollectFiles(Change_notice notice)
        {
            foreach (var i in notice.Images.Where(i => !string.IsNullOrEmpty(i.notice_image)))
            {
                var temp_file = WebUtilities.GetTempFile(i.comments);

                if (temp_file != null)
                {
                    var filePath = Utilities.WriteFile(i.notice_image, System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.ChangeNoticeRootFolder), temp_file);

                    if (!string.IsNullOrEmpty(filePath))
                        i.notice_image = Path.GetFileName(filePath);
                }
            }

            if(notice.Document != null)
            {
                var temp_doc_file = WebUtilities.GetTempFile(notice.Document.formatted_change_doc_id);

                if(temp_doc_file != null)
                {
                    var docName = Utilities.WriteFile(notice.Document.formatted_change_doc, System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.ChangeNoticeRootFolder), temp_doc_file);

                    if (!string.IsNullOrEmpty(docName))
                        notice.Document.formatted_change_doc = docName;
                }
            }
        }

        private object GetNoticeUIObject(Change_notice n)
        {
            return new
            {
                n.id,
                n.categoryId,
                n.createdById,
                n.datecreated,
                n.dateModified,
                n.description,
                n.expectedReadyDate,
                n.filename,
                n.reason_id,
                n.status,
                allocations = n.Allocations.Select(a => new
                {
                    a.id,
                    a.dateAllocated,
                    product = a.Product,
                    a.cprod_id,
                    a.notice_id,
                    orders = a.Orders.Select(o => new
                    {
                        o.orderid,
                        o.userid1
                    })
                }),
                Images = n.Images.Select(i =>new
                {
                    i.id,
                    i.notice_id,
                    i.notice_image,
                    i.order,
                    i.type_id,
                    i.comments,
                    ImageType = i.ImageType,
                    ChangeNotice = i.ChangeNotice
                }),
                n.Document
            };
        }

        [Route("api/changenoticev2/uploadimage")]
        [HttpPost]
        public object UploadImage()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };
        }
        
        [Route("api/changenoticev2/getTempUrl")]
        [HttpGet]
        public HttpResponseMessage getTempUrl(string file_id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(file_id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        [Route("api/changenoticev2/deleteTempFile")]
        [HttpPost]
        public object DeleteTempFile(string name)
        {
            WebUtilities.DeleteTempFile(name);
            return Json("OK");
        }

        [Route("api/changenoticev2/deleteChangeNoticeDocument")]
        [HttpPut]
        public object DeleteChangeNoticeFormattedDocument(Change_notice n)
        {
            if (n != null && n.Document != null)
            {
                WebUtilities.DeleteTempFile(n.Document.formatted_change_doc);

                string filePath = Path.Combine(Settings.Default.ChangeNoticeRootFolder, n.Document.formatted_change_doc);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                n.Document.formatted_change_doc = string.Empty;
                n.Document.formatted_change_doc_id = string.Empty;

                unitOfWork.ChangeNoticeRepository.Update(n);
                unitOfWork.Save();

            }

            return GetNoticeUIObject(n);
        }

        [Route("api/changenoticev2/getNotices")]
        [HttpGet]
        public object getNotices(int? factory_id = null, int? client_id = null, int? status = null, string product_code = null)
        {
            return unitOfWork.ChangeNoticeRepository.Get(
                n=>(n.status == status || status == null) && (factory_id == null || n.Allocations.Any(a=>a.Product.MastProduct.factory_id == factory_id))
                && (client_id == null || n.Allocations.Any(a => a.Product.brand_userid == client_id))
                && (product_code == null || n.Allocations.Any(a=>a.Product.cprod_code1.Contains(product_code) || a.Product.cprod_name.Contains(product_code) || a.Product.MastProduct.factory_ref.Contains(product_code)))
                , includeProperties: "Reason, Category, Allocations.Product.MastProduct").
                Select(n => new
                {
                    n.id,
                    n.description,
                    n.datecreated,
                    n.expectedReadyDate,
                    category = n.Category?.category_name,
                    reason = n.Reason?.description
                });
        }

        private List<LookupItem> getStatuses()
        {
            return new List<LookupItem> { new LookupItem { id = Change_notice.status_pending, value = "Pending" }, new LookupItem { id = Change_notice.status_resolved, value = "Resolved" } };
        }

        [Route("api/changenoticev2/getListModel")]
        [HttpGet]
        public object getListModel()
        {
            return new
            {
                Clients = companyDAL.GetClientsFromProducts().ToList(),
                Factories = companyDAL.GetFactories().ToList(),
                Statuses =  getStatuses(),
            };
        }

    }
}
