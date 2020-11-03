using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public enum OutstandingOrdersMode
    {
        Both,
        Production,
        ShippingInNextNDays,
        Transit,
        TransitInPeriod,
        ProductionETA,
        TransitETA
    }

    public enum CountryFilter
    {
        All = 0,
        UKOnly = 1,
        NonUK = 2,
        NonUKExcludingAsia = 3
    }

    public enum BrandStockSummaryItemType
    {
        US,
        Others
    }

    public class SalesByMonth
    {
        //public int Year { get; set; }
        public int Month21 { get; set; }
        public int? year { get; set; }
        public int? month { get; set; }
        public double? Amount { get; set; }
        public int? Qty { get; set; }
        public double? SpecialAmount { get; set; }
        public int numOfOrders { get; set; }
    }

    public class BrandSalesByMonth : SalesByMonth
    {
        //public int Year { get; set; }
        public string brandname { get; set; }
        public int? brand_id { get; set; }
        
    }

    public class BrandSalesByMonthEx : BrandSalesByMonth
    {
        public string cprod_code { get; set; }
        public string cprod_name { get; set; }
        public int? cprod_stock { get; set; }
        public DateTime? stock_date { get; set; }
        public int? dist_stock { get; set; }
        public DateTime? dist_stock_date { get; set; }
        public string product_group { get; set; }
        public int customer_id { get; set; }
        public string customer_code { get; set; }
        
        public int? AnalyticsSubCategoryId { get; set; }
    }
    /***/
    public class FactorySalesByMonth : SalesByMonth {
        public string factoryCode { get; set; }
        public int factory_id { get; set; }
        public int combined_factory { get; set; }
        public int user_id { get; set; }
    }
    /***/
    public class Category1SalesByMonth : SalesByMonth
    {
        //public int Year { get; set; }
        public int category_id { get; set; }
        public string catname { get; set; }

    }

    public class CountrySales
    {
        public string user_country { get; set; }
        public string country_name { get; set; }
        public double Amount { get; set; }
    }

    public class ProductSales
    {
        public int cprod_id { get; set; }
        public string brand_code { get; set; }
        public string cprod_name { get; set; }
        public string cprod_code { get; set; }
        public string factory_code { get; set; }
        //public int? factory_stock { get; set; }
        //public double? factory_stock_value { get; set; }
        public string cprod_mast { get; set; }
        public double Amount { get; set; }
        public double POAmount { get; set; }
        public double POAmountGBP { get; set; }
        public int numOfUnits { get; set; }
        public double Margin { get; set; }
        public int Month21 { get; set; }

        public int Month22 { get; set; }

        public DateTime? Req_eta { get; set; }
        public string customer_code { get; set; }
    }

    public class CustomerSalesByMonth : SalesByMonth
    {
        public bool isUK { get; set; }
        public bool isOEM { get; set; }
        public int client_id { get; set; }
        public string customer_name { get; set; }
		public Company Client { get; set; }
        //public int Period { get; set; }
    }

    public class OrderStats
    {
        public string client_code { get; set; }
        public int newOrdersCount { get; set; }
        public double newOrdersAmount { get; set; }
        public int shippedOrdersCount { get; set; }
        public double shippedOrdersAmount { get; set; }
        public int outstandingOrdersCount { get; set; }
        public double outstandingOrdersAmount { get; set; }
    }

    public class OrderStatTempRecord
    {
        public int orderid { get; set; }
        public string customer_code { get; set; }
        public Dictionary<string, int> productgroup_lines { get; set; }
        public string product_group { get; set; }
        //public DateTime po_req_etd { get; set; }
        //public DateTime req_eta { get; set; }
        public double totalGPB { get; set; }
        
    }

    public class OrderProductGroupStats
    {
        public string client_code { get; set; }
        public string product_group { get; set; }
        public int orders_count { get; set; }
        public double totalGPB { get; set; }
    }

    public class OrderBrandsStats
    {
        public string client_code { get; set; }
        public int brandCount { get; set; }
        public int orderCount { get; set; }
    }

    public class OrderFactoriesStats
    {
        public string client_code { get; set; }
        public int factoryCount { get; set; }
        public int orderCount { get; set; }
    }

    public class OrderLocationStats
    {
        public string client_code { get; set; }
        public int location { get; set; }
        public int orderCount { get; set; }
    }

    public class OrderClientStat
    {
        public string client_code { get; set; }
        public int ordersCount { get; set; }
        public double totalGPB { get; set; }
    }

    public class ProductStats
    {
        public int? brand_userid { get; set; }
        public string brandname { get; set; }
        public string product_group { get; set; }
        public int numOfProducts { get; set; }
    }

    public class ProductLocationStats
    {
        public string cprod_code { get; set; }
        public string cprod_name { get; set; }
        public string brandname { get; set; }
        public int? location { get; set; }
        public string[] productgroup_others { get; set; }

        public string sproductgroup_others { get; set; }
        public string maxgroup { get; set; }

    }

    public class ProductLocationStatsSummary
    {
        public string brandname { get; set; }
        public int? location { get; set; }
        public int numOfProducts { get; set; }

    }

    public class AnalyticsCategorySummaryRow
    {
        public int analytics_category_id { get; set; }
        public int? analytics_option_id { get; set; }
        public double OrderQty { get; set; }
    }

    public class ProductDisplayCount
    {
        public int cprod_id { get; set; }
        public int DisplayCount { get; set; }
    }

    

    public class ProductDistributorDisplayCount
    {
        public int cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int DisplayCount { get; set; }
        public string distributor_code { get; set; }
    }

    public class StockSummary
    {
        public int? factory_id { get; set; }
        public string factory_code { get; set; }
        public double? InProduction { get; set; }
        public double? ReadyAtFactory { get; set; }
        public double? OnWater { get; set; }
        public double? Warehouse { get; set; }
        public double? UnallocatedAtFactoryValue { get; set; }
        public double? AllocatedAtFactory { get; set; }
    }

    public class StockFactoryRow
    {
        public int FactoryId { get; set; }
        public string FactoryCode { get; set; }
        public int? Qty { get; set; }
        public double? Value { get; set; }
    }

    public class CostValueItem
    {
        public DateTime date_entered { get;set; }
        public int order_qty { get;set;}
        public double price { get;set;}
    }

    public class StockReceiptItem
    {
        public DateTime req_eta { get; set; }
        public DateTime booked_in_date { get;set;}
        public int factory_id { get;set;}
        public int orderqty { get;set;}
        public double unit_price { get;set;}
    }

    public class BrandStockSummaryChartItem
    {
        public BrandStockSummaryItemType ItemType { get; set; }
        public int Year { get;set;}
        public int Month { get;set;}
        public string DateValue { get;set;}
        public double totalcalc { get;set;}
        public double cost { get; set; }
        public double receipts { get; set; }
    }
}
