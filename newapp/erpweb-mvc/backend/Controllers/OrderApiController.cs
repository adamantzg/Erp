using erp.Model;
using erp.DAL.EF.New;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Linq.Expressions;

namespace backend.Controllers
{
    [Authorize]
    public class OrderApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public OrderApiController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Route("api/order/getByCriteria")]
        [HttpGet]
        public object GetByCriteria(DateTime? from, DateTime? to)
        {
            var statuses = new[] { "X", "Y" };
            var containerTypes = new List<int?> { Container_types.Gp40, Container_types.Gp40b };
            return unitOfWork.OrderRepository.Get(o => o.req_eta >= from && o.req_eta <= to  
            && o.Client.distributor  > 0 
            && !statuses.Contains(o.status) && containerTypes.Contains(o.container_type), includeProperties: "Client").OrderBy(o=>o.req_eta)
                .Select(o => new
                {
                    o.orderid,
                    o.custpo,
                    o.req_eta,
                    o.userid1                    
                });
                
        }

        [Route("api/order/getOrdersByText")]
        [HttpGet]
        public object getOrdersByText(string text)
        {
            var statuses = new[] { "X", "Y" };
            var containerTypes = new List<int?> { Container_types.Gp40, Container_types.Gp40b };
            return unitOfWork.OrderRepository.Get(o => o.custpo.StartsWith(text) && o.Client.distributor > 0
            && !statuses.Contains(o.status) && containerTypes.Contains(o.container_type), includeProperties: "Client").OrderBy(o => o.custpo)
                .Select(o => new
                {
                    o.orderid,
                    o.custpo,
                    o.req_eta,
                    o.userid1
                });

        }

        

        

		[Route("api/order/getForExtraDataEntry")]
		[HttpGet]
		public object getForExtraDataEntry(int page=0, int pageSize=50, bool includeAlreadySet = false, int monthsInPast = 12, string custpo = null, bool brand = true, bool isUK = true)
		{
			var etaFrom = DateTime.Now.AddMonths(-1 * monthsInPast);
			var ukCodes = new[] { "UK", "GB", "IE" };
			var statuses = new[] { "X", "Y" };
			Expression<Func<Order_header, bool>> filter = o => !statuses.Contains(o.status) && o.stock_order != 1 && o.req_eta >= etaFrom && (custpo == null || custpo.Length == 0 || o.custpo.Contains(custpo))
						&& (brand ? o.Client.distributor > 0 : o.Client.distributor <= 0) && (isUK ? ukCodes.Contains(o.Client.user_country) : !ukCodes.Contains(o.Client.user_country)) &&
						(includeAlreadySet || o.sale_date == null);
            /*var test =
                new
                {
                    totalCount = unitOfWork.OrderRepository.GetQuery(filter, includeProperties: "Client").Count(),
                    data = unitOfWork.OrderRepository.Get(filter, take: pageSize, skip: page * pageSize, orderBy: or => or.OrderBy(ord => ord.req_eta), includeProperties: "Client").Select(o => new
                    {
                        o.custpo,
                        o.orderid,
                        o.req_eta,
                        o.BDi_VAT,
                        o.BDI_invoice,
                        o.BDi_import_fees,
                        o.Freight_value,
                        client = o.Client != null ? new { o.Client.user_id, o.Client.customer_code } : null,
                        o.sale_date,
                        o.freight_invoice_no,
                        o.bdi_import_fees_invoice_no
                    })
                };

            var single = test.data.Where(o => o.custpo == "SP - POR060743").ToList();*/

            return new
			{
				totalCount = unitOfWork.OrderRepository.GetQuery(filter, includeProperties: "Client").Count(),
				data = unitOfWork.OrderRepository.Get(filter, take: pageSize, skip: page * pageSize, orderBy: or => or.OrderBy(ord => ord.req_eta), includeProperties: "Client").Select(o => new
				{
					o.custpo,
					o.orderid,
					o.req_eta,
					o.bdi_vat,
					o.bdi_invoice,
					o.bdi_import_fees,
					o.freight_value,
					client = o.Client != null ? new { o.Client.user_id, o.Client.customer_code } : null,
					o.sale_date,
                    o.freight_invoice_no,
                    o.bdi_import_fees_invoice_no
				})
			};
			
		}

		[Route("api/order/updateSaleData")]
		[HttpPost]
		public object updateSaleData(Order_header o)
		{
			var order = unitOfWork.OrderRepository.GetByID(o.orderid);
			order.bdi_import_fees = o.bdi_import_fees;
			order.bdi_invoice = o.bdi_invoice;
			order.bdi_vat = o.bdi_vat;
			order.sale_date = o.sale_date?.Date;
			order.freight_value = o.freight_value;
            order.bdi_import_fees_invoice_no = o.bdi_import_fees_invoice_no;
            order.freight_invoice_no = o.freight_invoice_no;
			unitOfWork.Save();
			return true;

		}

	}
}