using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public enum UsBackOrderStatus
    {
        NotInBrochure,
        OutOfStock,
        AwaitingETA,
        InStock
    }

    public class BackOrdersSummaryReportModel
    {
        public Dictionary<int, int> AnnualOrderCount { get; set; }
        public int NewProductCount { get; set; }
        public DateTime ForDate { get; set; }
        public Dictionary<int?, DateTime?> ProductAvailabilityDates { get; set; }
        public Dictionary<string, List<Us_backorders>> DealerOrders { get; set; }
        public Dictionary<string,BackOrderProductReportRow> BackOrderProducts { get; set; }
    }

    public class BackOrdersDetailReportModel
    {
        public DateTime ForDate { get; set; }
        public List<BackOrderReportRow> Rows { get; set; }
    }

    

    public class BackOrderReportRow
    {
        
        public Us_backorders Order { get; set; }
        public int ProductCount { get; set; }
        public int ProductsReady { get; set; }
        public int ProductsAwaitingETA { get; set; }
        public int ProductsNoStock { get; set; }
        public int ProductsNotInBrochure { get; set; }

        public UsBackOrderStatus Status
            =>
                ProductsNotInBrochure > 0
                    ? UsBackOrderStatus.NotInBrochure
                    : ProductsNoStock > 0
                        ? UsBackOrderStatus.OutOfStock
                        : ProductsAwaitingETA > 0 ? UsBackOrderStatus.AwaitingETA : UsBackOrderStatus.InStock;
        

        public DateTime? SendDate { get; set; }
    }

    public class BackOrdersProductDetailReportModel
    {
        public DateTime ForDate { get; set; }
        public List<BackOrderProductReportRow> Rows { get; set; }
    }

    public class BackOrderProductReportRow
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int? Qty { get; set; }
        public int? Stock { get; set; }
        public int? CustomerCount { get; set; }
        public int? QtyOnOrder { get; set; }
        public DateTime? NextETA { get; set; }

        public UsBackOrderStatus Status
            =>
                Stock >= Qty
                    ? UsBackOrderStatus.InStock
                    : NextETA != null ? UsBackOrderStatus.AwaitingETA : UsBackOrderStatus.OutOfStock;
    }

    public class OrderStatusReportModel
    {
        public List <OrderStatusModelInboundOrderRow> InBoundOrders { get; set; }
        public List<ussales_log_report> Logs { get; set; }
        public List<Sales_orders> OutBoundOrders { get; set; }
        public List<Sales_orders> PendingOrders { get; set; }
        public List<Sales_orders> NotSentForDespatchOrders { get; set; }

        public Dictionary<string,Sales_orders_headers> PendingOrderHeaders { get; set; }
        public Dictionary<string,Sales_orders_headers> OutBoundHeaders { get; set; }
        public Dictionary<string, Sales_orders_headers> NotDespatchedHeaders { get; set; }

        public string Warehouse { get; set; }
        public DateTime ForDate { get; set; }

        public Dictionary<string, DateTime?> AvailabilityDates { get; set; }
        
    }

    public class OrderStatusModelInboundOrderRow
    {
        /*public string Supplier { get; set; }
        public string custpo { get; set; }*/
        public string Ref { get; set; }
        public DateTime? ETA { get; set; }
        public string ShipmentType { get; set; }
        public int? NumberOfProducts { get; set; }
        public int? TotalQTY { get; set; }
    }
}