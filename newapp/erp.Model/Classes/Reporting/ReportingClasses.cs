using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public enum OrderDeliveryStatus
    {
        Delivered=1,
        OnWater=2,
        ToBeShipped=3
    }

    public class WeightDifferenceRow
    {
        public string factory_code { get; set; }
        public string cprod_code { get; set; }
        public string custpo { get; set; }
        public string customer_code { get; set; }
        public string factory_ref { get; set; }
        public double? Updated_PO_weight { get; set; }
        public double? System_weight { get; set; }
    }

    public class OrderRow
    {
        public string factory_code { get; set;}
        public string bs_code { get; set; }
        public string custpo { get; set; }
        public DateTime? po_req_etd { get; set; }
        public int userid { get; set; }
        public double? valueUSD { get; set; }
        public double? valueGBP { get; set; }
    }

    public class OrdersAnalysisRow
    {
        public int client_id { get; set; }
        public string customer_code { get; set; }
        public int NumOfOrders { get; set; }
        public int NumOfProducts { get; set; }
        public int NumOfLateProducts { get; set; }
        public int NumOfOrdersWithLateProducts { get; set; }
	    public int NumOfOrdersLateShipping { get; set; }
	    public int NumOfLateNewProducts { get; set; }
    }

    public class OrdersAnalysisReport
    {
        public List<OrdersAnalysisRow> OrdersAnalysisRows { get; set; }
        public int NumOfProducts { get; set; }
        public int NumOfLateProducts { get; set; }
		public int NumOfNewProducts { get; set; }
    }

    public class OrdersAnalysisProductReport
    {
        public List<OrdersAnalysisProductRow> Products { get; set; }
        public int NumOfOrders { get; set; }
        public int NumOfProducts { get; set; }
        public int NumOfLateProducts { get; set; }
        public int NumOfOrdersWithLateProducts { get; set; }
		public List<int?> NewProductIds { get; set; }
        
    }

    public class OrdersAnalysisProductRow
    {
        public Cust_products Product { get; set; }
        public double? Qty { get; set; }
        public int? Location { get; set; }
        public int? NumOfOrders { get; set; }
        public List<string> Distributors { get; set; }
        public double? SalesPrev3m { get; set; }
        public double? SalesLast3m { get; set; }
        public double? SalesBeforeLastnMonths { get; set; }
    }

    public class OrderSummaryByLocationClientRow
    {
        public int? Location { get; set; }
        public OrderDeliveryStatus Status { get; set; }
        public string CustomerCode {get; set;}
        public double? Qty { get; set; }
    }

    

    public class ReturnsByCustomer
    {
        public string customer_code { get; set; }
        public int Qty { get; set; }
    }

    public class DealerSalesByCustomer
    {
        public string customer_code { get; set; }
        public int Qty { get; set; }
    }

    public class DownloadLogTotal
    {
        public int user_id { get; set; }
        public long Count { get; set; }
    }

    public class ProductDate
    {
        public int id { get; set; }
        public DateTime? Date { get; set; }
    }

    public class QueryResult
    {
        public int? Int1 { get; set; }
        public int? Int2 { get; set; }
        public int? Int3 { get; set; }

        public string String1 { get; set; }
        public string String2 { get; set; }
        public string String3 { get; set; }
    }

    public class OverstockReportRow
    {
        public string factory_code { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? cprod_stock { get; set; }
        public int? forecast_qty { get; set; }
        public int? on_order_qty { get; set; }

    }

    public class KpiReportInspectionRow
    {
        public DateTime? StartDate { get; set; }
        public string factory_code { get; set; }
        public string Type { get; set; }
        public string Insp_no { get; set; }
        public int? insp_id { get; set; }
        public int? Ca { get; set; }
        public bool? New { get; set; }
    }

    public class ClaimsStatsOtherProductRow
    {
        public string brandname { get; set; }
        public string factory_code { get; set; }
        public int? cprod_id { get; set; }
        public double? value { get; set; }
    }

    public class ProductGroupClassReportRow
    {
        public string brandname { get; set; }
        public int cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public double? qty { get; set; }
        public double? amount { get; set; }
        public int? old_status_id { get; set; }
        public int? new_status_id { get; set; }
    }
}
