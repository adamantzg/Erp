using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using backend.ApiServices;
using erp.Model;

namespace backend.Models
{
    public enum ExportOrderBy
    {
        ETD,
        ETA,
        Client,
        OrderDate,
        ClientETD
    }

    public enum MonthSummaryBy
    {
        ETD = 0,
        ETA,
        OrderDate
    }

    public class CreateOrderModel
    {
        public Order_header Order { get; set; }
        public bool isEdit { get; set; }
        public Dictionary<int, string> DeliveryAddresses { get; set; }
        public int? DeliveryAddress { get; set; }
        public User User { get; set; }
    }

    public class OrderExportModel
    {
        public const int OrderTypeRegular = 1;
        public const int OrderTypeStock = 2;
        public const int OrderTypeCallOffAndRegular = 3;
        public const int OrderTypeSpares = 4;


        public List<Company> Factories { get; set; }
        public List<Location> Locations { get; set; }
        public int? location_id { get; set; }
        public int? factory_id { get; set; }
        public string factoryids { get; set; }
        public int? client_id { get; set; }
        public int? brand_user_id { get; set; }

        public List<Company> Clients { get; set; }
        public List<Brand> Brands { get; set; }
        //public List<CheckBoxItem> Brands { get; set; }
        public bool MonthlySummary { get; set; }
        public MonthSummaryBy MonthlySummaryBy { get; set; }
        public DateTime? ETD_From { get; set; }
        public DateTime? ETD_To { get; set; }
        public bool IncludeSalesForecast { get; set; }
        public bool IncludeSalesHistory { get; set; }
        public bool Dimensions { get; set; }
        public List<ExportOrderBy> OrderByList { get; set; }
        public ExportOrderBy OrderBy { get; set; }
        public bool ListOnlyOutOfStock { get; set; }
        public bool IncludeSpares { get; set; }
        public bool OrderBySeqNumber { get; set; }
        public string POCriteria { get; set; }
        //public List<int> SelectedProductIds { get; set; }
        public List<OrderProduct> Products { get; set; }
        public List<Order_lines> Lines { get; set; }
        public List<Order_lines> AllLines { get; set; }
        public List<Order_lines> ArrivingLines { get; set; }
        public List<Container_types> ContainerTypes { get; set; }
        public bool IncludeDiscontinued { get; set; }
        public bool RegularOnly { get; set; }
        public bool ShowTotalsAfterOrders { get; set; }
        public bool ShowSales { get; set; }
        public bool HighlightLowForecasts { get; set; }
        public List<CombinedOrder> CombinedOrders { get; set; }
        public bool Show9Ref { get; set; }
        public int? OrderType { get; set; } //1 - regular 2- stock null-all
        public List<LookupItem> OrderTypeList { get; set; }
        public bool ShowBrandRangeColumn { get; set; }
        public bool ShowSpecialComments { get; set; }
        public bool ShowFactoryCode { get; set; }
        public List<Range> Ranges { get; set; }
        public bool EnableContainerPriceSelection { get; set; }
        public bool ShowContainerPrice { get; set; }
        public List<Currencies> Currencies { get; set; }
        public bool IsCurrentUserFactoryUser { get; set; }
        public List<Brand> CWBrands { get; set; }
        public bool ShowHistoryForSpares { get; set; }
        public bool ShowExtraValueFields { get; set; }
        public bool EnableExtraValueFieldsSelection { get; set; }
        public List<cust_products_range> ProductRanges { get; set; }
        public bool IncludeBookedInOrders { get; set; }
        public Dictionary<int?, int?> ProductSoldQtys { get; set; }
        public Dictionary<int?, double?> ProductDeliveredQtys { get; set; }
        public Dictionary<int?, int?> DisplaysSoldQtys { get; set; }
        public bool UseSalesOrders { get; set; }
        public bool Last12mClientBreakdown { get; set; }
        public List<string> DateFormats { get; set; }
        public string DateFormat { get; set; }
        public bool SparesOnly { get; set; }
        public bool ShowSuggestedOrder { get; set; }

        public List<process> Processes { get; set; }

        public Company Client { get; set; }

        public List<ports> Ports { get; set; }

        public IOrderService OrderService { get; set; }

        public List<forward_order_lines> ForwardOrderLines { get; set; }

        public OrderExportModel()
        {
            HighlightLowForecasts = true;
            ShowFactoryCode = true;
            IncludeBookedInOrders = true;
            Excel = true;
            Last12mClientBreakdown = false;
            DateFormats = new List<string>() { "yyyy-MM-dd", "dd/MM/yyyy" };
            DateFormat = DateFormats[0];
        }

        public bool Excel { get; set; }
    }

    public class OrderProduct
    {
        public Cust_products Prod { get; set; }
        public int id { get; set; }
        public List<int> cprod_ids { get; set; }
        public List<Sales_forecast> Forecasts { get; set; }
        public List<Sales_data> SalesData { get; set; }
        //public List<Contract_sales_forecast_lines> CS_Lines { get; set; }
        public List<OrderMgtmDetail> ArrivingOrders { get; set; }
        public List<Order_lines> Lines { get; set; }
        public string BrandRange { get; set; }
        public DateTime? FirstShipmentEtd { get; set; }
        public List<int?> mast_ids { get; set; }
    }

    public class ExportOrder
    {
        public int? orderid { get; set; }
        public string customer_code { get; set; }
        public string custpo { get; set; }
        public string OrderType { get; set; }
        public string po_comments { get; set; }
        public string po_instructions { get; set; }
        public DateTime? orderdate { get; set; }
        public DateTime? po_req_etd { get; set; }
        public DateTime? original_po_req_etd { get; set; }
        public string container { get; set; }
        public DateTime? req_eta { get; set; }
        public List<Shipments> Shipments { get; set; }
        public int? combined_order { get; set; }
        public double? ContainerRowPrice { get; set; }
        public double? ContainerPrice { get; set; }
        public string ContainerCurrency { get; set; }
        public string status { get; set; }
		public string location { get; set; }
		public int? userid1 { get; set; }
		public bool isSpareOrder { get; set; }
    }

    public class CombinedOrder
    {
        public int SourceOrderId { get; set; }
        public Order_header COrder { get; set; }
    }

    public class QuantityAnalysisModel
    {
        public Order_header Order { get; set; }
        public Dictionary<int?,List<Order_lines>> ProductLineHistory { get; set; } 
    }

    //public class OrderRow
    //{
    //    public string customer_code { get; set; }
    //    public string custpo { get; set; }
    //    public string po_comments { get; set; }
    //    public DateTime? orderdate { get; set; }
    //    public DateTime? po_req_etd { get; set; }
    //    public DateTime? original_po_req_etd { get; set; }
    //    public string container { get; set; }
    //    public DateTime? req_eta { get; set; }
    //}
    public class OrderingPatternsAverage
    {
        public IList<Order_line_detail2_v6> OrderingPatterns { get; set; }
    }

    public class ArrivalsReportModel
    {
        public DateTime? StartDate { get; set; }
        public List<Order_lines> Lines { get; set; }
        public string Clients { get; set; }
        public string Factories { get; set; }
        public Dictionary<int?, double?> ProductTotalsBeforeStartDate { get; set; }
    }
}