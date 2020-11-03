using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;
using backend;

namespace backend.Models
{
    public class StockOrderAllocationsModel
    {
        public List<Company> Factories { get; set; }
        public List<Order_line_export> Lines { get; set; }
        
        public List<CalloffOrder> CallOffOrders { get; set; }
        public List<Stockorder> StockOrders { get; set; }
        public List<Product> Products { get; set; }
        public string factory_ids { get; set; }
        public DateTime FirstWeek { get; set; }
        public int? DateType { get; set; }  //0-ETD  1-ETA
        public DateTime? From { get; set; }
        public List<CheckBoxItem> Clients { get; set; }
        public bool IncludeDiscontinued { get; set; }
        public bool IncludePalletQty { get; set; }
        public bool IgnoreStockCodes { get; set; }
        public bool Excel { get; set; }
    }

    public class CalloffOrder
    {
        public DateTime? orderdate { get; set; }
        public DateTime? po_req_etd { get; set; }
        public int orderid { get; set; }
        public int LeadTime
        {
            get
            {
                if (orderdate != null && po_req_etd != null)
                    return Convert.ToInt32((po_req_etd.Value - orderdate.Value).TotalDays);
                else
                    return 0;
            }
        }
        public DateTime? req_eta { get; set; }
        public string containerName { get; set; }
        public string custpo { get; set; }
        //public List<Stock_order_allocation> Allocations { get; set; }
    }

    public class Stockorder
    {
        public DateTime? orderdate { get; set; }
        public int orderid { get; set; }
        public DateTime? po_req_etd { get; set; }
        public DateTime? po_ready_date { get; set; }
        public string custpo { get; set; }
        public int LeadTime
        {
            get
            {
                if (orderdate != null && po_req_etd != null)
                    return Convert.ToInt32((po_req_etd.Value - orderdate.Value).TotalDays);
                else
                    return 0;
            }
        }
        public double? Qty { get; set; }
        public double? Balance { get; set; }

        public List<Order_line_export> Lines { get; set; }
    }

    public class Product
    {
        public Cust_products Prod { get; set; }
        public List<Stock_order_allocation> Allocations { get; set; }
        public List<Sales_forecast> Forecasts { get; set; }
        public List<Sales_data> SalesData { get; set; }
        public List<Contract_sales_forecast_lines> CS_Lines { get; set; }
        public List<OrderMgtmDetail> ArrivingOrders { get; set; }
        public double? StockAvg3Months { get; set; }
        public List<SOProductStock> StockValues { get; set; }
    }

    public class SOProductStock
    {
        public DateTime Date { get; set; }
        public double Balance { get; set; }
        public Product Product { get; set; }
        public double? BalanceBy3MonthsAvg {
            get { return Product != null && Product.StockAvg3Months > 0 ? Balance/Product.StockAvg3Months : null; }
        }
    }

    public class SOAllocationsManagementModel
    {
        public List<Cust_products> Products { get; set; }
    }


    public class OrderHistoryModel
    {
        public List<OrderLinesLight> OrderLines { get; set; }
    }

    //public class SOAllocationsManagementEditModel
    //{
    //    public List<Order> COrders { get; set; }
    //    public List<Order> SOrders { get; set; }
    //}

}