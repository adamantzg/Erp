using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using erp.Model;
using backend.Models;

namespace backend.Models
{
    public class DopModel
    {
        public Cust_products Product { get; set; }
        public Category_dop Category { get; set; }
        public List<Mastproduct_characteristics> ProductCharacteristic { get; set; }
        public Company Client { get; set; }
    }

    public class MapDots
    {
        public List<Dot> Dots { get; set; }
        public int Radius { get; set; }
    }
    public class MapCompare
    {
        public Array arrDot { get; set; }

        public int Radius { get; set; }

        public double Opacity { get; set; }

        public string City { get; set; }

        public List<Dot> Yellow { get; set; }
        public List<Dot> Red { get; set; }
        public List<Dot> Green { get; set; }
        public List<Dot> Pink { get; set; }
        public List<Dot> White { get; set; }
        public List<Dot> Purple { get; set; }

        public List<DotMarker> RedNew { get; set; }
        public List<DotMarker> YellowNew { get; set; }
        public List<DotMarker> GreenNew { get; set; }
        public List<DotMarker> PinkNew { get; set; }
        public List<DotMarker> WhiteNew { get; set; }
        public List<DotMarker> OrangeNew { get; set; }
        public List<DotMarker> PurpleNew { get; set; }
    }

    public class SlotMarkers
    {
        public string Yellow { get; set; }
        public string Green { get; set; }
        public string Blue { get; set; }
        public string Red { get; set; }
        public string Violet { get; set; }

        public string Marker { get; set; }
    }

    public class DotMarker
    {
        public int userId { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
        public string text { get; set; }
        public int radius { get; set; }
        public string color { get; set; }

        public int index { get; set; }
        public string postcode { get; set; }
        public int action_flag { get; set; }
        public string location { get; set; }
    }
    //public class Dot
    //{
    //    public int id { get; set; }
    //    public double? latitude { get; set; }
    //    public double? longitude { get; set; }
    //    public string name { get; set; }
    //    public int radius { get; set; }
    //}
    public class MarkerData
    {
        public double? lat { get; set; }
        public double? lon { get; set; }
        public string text { get; set; }
        public string link { get; set; }
        public int userId { get; set; }
        public int distributorId { get; set; }
        public string distributorName { get; set; }
    }
    public class ReportProductImagesModel
    {
        public List<MarkerData> Dots { get; set; }
        public int? SelectBrandId { get; set; }
        public int? SelectBrandCatId { get; set; }

        public List<Brand> BrandsList { get; set; }
        public List<BrandCategory> BrandCategories { get; set; }
        public List<BrandSubCategory> BrandCategoriesSub { get; set; }
        public List<ListProducts> ProductList { get; set; }
        public List<ListProducts> ProductListMap { get; set; }
        public List<DealerImagesWebOnRegion> Proudcts { get; set; }
        public List<Dealer> Dealers { get; set; }
        public List<PostcodeAreas> PostcodeArea { get; set; }
        public string[] SelectedStuff { get; set; }
        public List<DealerImagesWebOnRegion> MarkerData { get; set; }
        public List<DealerImagesWebOnRegion> MarkerDataRed { get; set; }
        // public bool IsSelected { get; set; }
        // public object Tags { get; set; }

        /* Nova struktura */
        public List<Web_site> WebSites { get; set; }
        public IEnumerable<Web_category> WebCategories { get; set; }

        public List<DealerImagesWebOnRegion> Products { get; set; }

        public List<Web_product_new> ProductsTest { get; set; }

        public List<DealerImagesWebOnRegion> ProductsNew { get; set; }

        public List<Web_category> WebCategory { get; set; }

        public List<Brand> Brands { get; set; }

        public List<Web_category> WebCategoryChild { get; set; }
        public List<Web_product_new> WebProduct { get; set; }

        public List<NumberShowingImagesForRegion> NumberImagesOnRegion { get; set; }
        public List<DealerImagesWebOnRegion> Distributors { get; set; }
        public List<ddlDealer> DDLDealers { get; set; }
    }

    public class ddlDealer
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class NumberShowingImagesForRegion
    {
        public string Region { get; set; }
        public int Number { get; set; }
    }

    public class ProbaModel
    {
        //public List<WebProduct> Productsweb { get; set; }
    }

    public class ListProducts
    {
        public bool IsSelected { get; set; }
        public object Tags { get; set; }
        public string ProdId { get; set; }
        public string ProductName { get; set; }
        public int CountProducts { get; set; }

        public DealerImagesWebOnRegion Products { get; set; }


    }
    public class PostList
    {
        public string[] ListIds { get; set; }
    }
    public class ReportSalesModel
    {
        public List<Cust_products> SalesData { get; set; }
        public List<Sales_forecast> ForecastsData { get; set; }
        public List<Order_lines> OrderLinesData { get; set; }
        public IEnumerable<IGrouping<int?, Sales_forecast>> FMonth21 { get; set; }
        public IEnumerable<IGrouping<int?, Sales_data>> Month21 { get; set; }
        public List<ProductAllStatistic> ProductAllStatistics { get; set; }

        public List<ProductSaleStatistic> ProductSaleStatistics { get; set; }
        public List<ProductSaleForecastStatistic> ProductSaleForecastStatistics { get; set; }

        public List<Cust_products> SalesForecastData { get; set; }

        public List<int> FDate { get; set; }
    }

    public class ProductAllStatistic
    {
        public int IdProduct { get; set; }
        public object CprodCode { get; set; }
        public object CprodStock { get; set; }
        public string Name { get; set; }

        public List<int?> SaleQty { get; set; }
        public List<int?> ForecastQty { get; set; }

        /* koliko ima */
        public string OrderDescription { get; set; }
        public int? OrderFrequency { get; set; }


        public int SetForecastDate21 { get; set; }
        public int SetSalesDate21 { get; set; }


    }

    public class ProductSaleStatistic
    {
        public string PName { get; set; }
        public List<int?> PSales { get; set; }

        public int PId { get; set; }

        //public List<int?> PForecast { get; set; }
        //public string ProductName { get; set; }
    }
    public class ProductSaleForecastStatistic
    {
        public int FId { get; set; }
        public List<int?> FSales { get; set; }
    }

    public class UserProductSales
    {
        //public List<Sales_data> CustProducts { get; set; }
        public string CprodCode { get; set; }
        public List<string> ListCprodCode { get; set; }
        public List<Cust_products> CustProducts { get; set; }
        public List<Order_line_export> BurlingtonSeals { get; set; }
        public List<ReturnAggregateDataByMonth> AcceptedClaims { get; set; }
    }

    public class BBLRequests
    {
        public List<Emaillog> EmailLogs { get; set; }
    }

    public class MonthlyBrochureRequest
    {

        public List<BrochureRequest> Requests { get; set; }

        public Dealer DealerDetail { get; set; }
        public List<DotMarker> Yellow { get; set; }
        public List<DotMarker> Green { get; set; }
        public List<DotMarker> Blue { get; set; }
        public List<DotMarker> Red { get; set; }

        public string RedNew { get; set; }

        public string YellowNew { get; set; }

        public string GreenNew { get; set; }

        public string BlueNew { get; set; }

        public string ImageName { get; set; }

        public List<DotMarker> Coordinates { get; set; }
        public List<SlotMarkers> GroupMarkers { get; set; }

        public DateTime FromStart { get; set; }
        public DateTime? EndDate { get; set; }

        public string VioletNew { get; set; }
        public List<BrochureDownloadLog> BrochureLogs { get; internal set; }
        public List<Brand> Brands { get; internal set; }
    }



    public class SalesForecastAmendmentsModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<SalesForecastAmendmentRow> Alerts { get; set; }
        public List<Company> Companies { get; set; }
        public int CallOffDefault { get; set; }
        public int CallOffLower { get; set; }
        public int WeeksNoticeThreshold { get; set; }
        public double QcCharge { get; set; }
        public double Duty { get; set; }
        public double Freight { get; set; }
        public DateTime StartDateForStockIssues { get; set; }
        public DateTime NoticeThresholdDate { get; set; }
        public bool ShowOnlyPositiveChangesForStockIssues { get; set; }
    }

    public class SmallItemsReportModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<Order_lines> Lines { get; set; }
    }


    public class SalesForecastAmendmentRow
    {
        public Cust_products CustProduct { get; set; }
        //public string cprod_code { get; set; }
        //public string cprod_name { get; set; }
        public Mast_products MastProduct { get; set; }
        public DateTime? DateModified { get; set; }
        public string period { get; set; }
        public int Month21 { get; set; }
        public int? old_data { get; set; }
        public int? new_data { get; set; }
        public int? average { get; set; }
        public int CallOffMonthOffset { get; set; }
        public double? StockValueGBP { get; set; }
    }


    public class LogAnalyzerModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string UserNames { get; set; }
        public string CompanyNames { get; set; }
    }



    //public class BrochureRequestReport : BrochureRequest
    //{
    //    public string Brand { get; set; }
    //}

    public class GoogleMapModel
    {

        public string Blue { get; set; }

        public string Red { get; set; }

        public string Green { get; set; }

        public string Yellow { get; set; }

        public List<Dot> BlueForDynamicMap { get; set; }
    }

    public class GoogleMapArcadeModel
    {
        public bool RenderPdf { get; set; }
        public List<Dot> Pin { get; set; }
        public List<Dot> Pin2 { get; set; }

        public string ArcadePinsPage { get; set; }

        public string RedNew { get; set; }
        public string YellowNew { get; set; }
        public string GreenNew { get; set; }
        public string BlueNew { get; set; }

        public List<SlotMarkers> GroupMarkers { get; set; }

        public string VioletNew { get; set; }

        public string Marker { get; set; }

        public Web_site Brand { get; set; }

        public string Country { get; set; }

        public List<Countries> Countries { get; set; }

        public string CountryFullName { get; set; }

        public int Year { get; set; }

        public List<Dot> Pin1 { get; set; }

        public List<Dot> Pin3 { get; set; }

        public List<Dot> Pin4 { get; set; }

        public List<Dot> Pin5 { get; set; }

        public List<Dot> Pin6 { get; set; }

        public List<erp.Model.Brand> Brands { get; set; }

        public string Continent { get; set; }
        public int Id { get; internal set; }
    }

    public class StockSummaryReportModel
    {
        public List<Cust_products> Products { get; set; }
        public List<OrderSummaryByLocationClientRow> OrderSummary { get; set; }
        public List<BrandCategory> Categories { get; set; }
        public List<Brand_categories_group> CategoriesGroups { get; set; }
        public List<ProductDistributorDisplayCount> DisplayCountByDistributor { get; set; }
        public int DisplayCount { get; set; }
        public List<ReturnsByCustomer> ReturnsByCustomers { get; set; }
        public List<DealerSalesByCustomer> DealerSalesByCustomer { get; set; }
        public int DealerSalesQty { get; set; }
        public Brand Brand { get; set; }
        public CountryFilter CountryFilter { get; set; }
        public string CustomersForSalesData { get; set; }
        public List<Stocksummary_factoryvalue> StocksummaryFactoryvalues { get; set; }

    }

    public class StockSummaryCustomerData
    {
        public int OrderQty { get; set; }
        public int DisplayQty { get; set; }
        public int SalesQty { get; set; }
        public int ReturnQty { get; set; }

        public int Stock
        {
            get { return OrderQty - DisplayQty - SalesQty - ReturnQty; }
        }

        public double DisplayToOrderQty
        {
            get { return DisplayQty * 1.0 / OrderQty; }
        }

        public double SalesToOrderQty
        {
            get { return SalesQty * 1.0 / OrderQty; }
        }

        public double ReturnToOrderQty
        {
            get { return ReturnQty * 1.0 / OrderQty; }
        }

        public double StockToOrderQty
        {
            get { return Stock * 1.0 / OrderQty; }
        }
    }
    public class ExportProductsModel
    {
        public List<Company> Factories { get; set; }
        public int? factory_id { get; set; }
        public Company Factory { get; set; }
        public int clientId { get; set; }
        public Company Client { get; set; }
        public List<Cust_products> Products { get; set; }
    }

    public class CatGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BrandCategory> Categories { get; set; }
    }

    public class OverdueLinesReportModel
    {
        public List<Company> Factories { get; set; }
        public List<Order_lines> LinesInPeriod { get; set; }
        public List<Order_lines> LinesBeforePeriod { get; set; }
        public Dictionary<int, int> Rules { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class MonthlyClaimsReportParamsModel
    {
        public List<Lookup> Months { get; set; }
        public int Month { get; set; }
        public List<Lookup> Years { get; set; }
        public int Year { get; set; }
        public bool ShowRefit { get; set; }
        public bool Excel { get; set; }
    }

    public class SalesExportReportParamsModel
    {
        public List<Lookup> Months { get; set; }
        public int Month { get; set; }
        public List<Lookup> Years { get; set; }
        public int Year { get; set; }
        public bool Excel { get; set; }
    }


    public class SalesOrdersReportModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<SalesOrdersSummaryRow> Rows { get; set; }
        public List<SalesOrdersSummaryRow> YearData { get; set; }
        public List<SalesOrdersSummaryRow> Last12mData { get; set; }
        public List<Sales_orders> Data { get; set; }
        public bool ShowWTD { get; set; }
        public bool ShowMTD { get; set; }
        public bool ShowYTD { get; set; }
        public bool ShowTotal { get; set; }
        public bool ShowNotes { get; set; }
        public string[] Headers { get; set; }
        public bool ShowLast12m { get; set; }
        public string DayFormat { get; set; }
        public bool DescendingOrder { get; set; }
        public string MinDateText { get; set; }

        public SalesOrdersReportModel()
        {
            ShowNotes = ShowWTD = ShowMTD = ShowYTD = true;
            DayFormat = "ddd d MMM";
            Headers = new[] { "Value", "Order Count" };
        }
    }

    public enum SalesOrderDetailColumns
    {
        OrdersYTD,
        Dealer,
        State,
        Brand,
        Code,
        Description,
        Qty,
        ValueFormatted,
        StockStatus
    }

    public class SalesOrdersDetailModel
    {
        public List<Sales_orders> SalesOrderData { get; set; }
        public Dictionary<string, SalesOrdersDealerSummaryRow> DealerYearData { get; set; }
        public Dictionary<string,Us_dealers> Dealers { get; set; }
        public Dictionary<int?, DateTime?> ProductAvailabilityDates { get; set; }
        public Dictionary<int, DateTime?> BundleAvailabilityDates { get; set; }
        public bool ShowNote { get; set; }
        public List<SalesOrderDetailColumns> HiddenColumns { get; set; }

        public SalesOrdersDetailModel()
        {
            ShowNote = true;
        }
    }

    

    public class DisplayOrdersDetailModel
    {
        public List<Sales_orders> DisplayOrderData { get; set; }
        public Dictionary<int?, DateTime?> ProductAvailabilityDates { get; set; }
        public Dictionary<int, DateTime?> BundleAvailabilityDates { get; set; }
    }

    public class UsSummaryReportModel
    {
        public DateTime? From { get; set; }
        public DateTime? DetailsFrom { get; set; }
        public DateTime? DetailsTo { get; set; }
        public List<Sales_orders> SalesOrderData { get; set; }
        public List<Sales_orders> DisplayOrderData { get; set; }
        public List<Sales_orders> SalesOrdersNotDespatched { get; set; }
        public List<SalesOrdersSummaryRow> SalesOrdersSummaryData { get; set; }
        public List<SalesOrdersSummaryRow> SalesOrdersYTDData { get; set; }
        public List<SalesOrdersSummaryRow> SalesOrdersLast12mData { get; set; }
        public Dictionary<string, SalesOrdersDealerSummaryRow> DealerYearData { get; set; }
        public Dictionary<string, Us_dealers> Dealers { get; set; }
        public List<SalesOrdersSummaryRow> DisplayRows { get; set; }
        public List<SalesOrdersSummaryRow> DisplayYTDData { get; set; }
        public List<SalesOrdersSummaryRow> DisplayLast12mData { get; set; }
        public Dictionary<int?, DateTime?> ProductAvailabilityDates { get; set; }
        public Dictionary<int, DateTime?> BundleAvailabilityDates { get; set; }
        public List<SalesOrdersSummaryRow> BestWeekData { get; set; }
        public List<SalesOrdersSummaryRow> BestMonthData { get; set; }
        public bool ShowNotDespatchedReport { get; set; }
        public List<SalesOrdersSummaryRow> NotDespatchedByMonthData { get; set; }
        public bool ShowNotDespatchedByMonthReport { get; set; }
        public string MinDateText { get; set; }
        public bool ShowInternalTransactions { get; set; }
        public List<Sales_orders> InternalTransactions { get; set; }
        
    }

    public class SalesOrdersSummaryRow
    {
		public string Brand { get; set; }
        public int Period { get; set; }
        public DateTime Day { get; set; }
        public int? Qty { get; set; }
        public int? NoOfOrders { get; set; }
        public double? Value { get; set; }		
    }

    public class SalesOrdersDealerSummaryRow
    {
        public string DealerCode { get; set; }        
        public int? NoOfOrders { get; set; }
    }

    public class FactoryClaimsMonthlyReportModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Factories { get; set; }
        public List<FactoryClaimsMonthlyReportRow> Rows { get; set; }
    }

    public class FactoryClaimsMonthlyReportRow
    {
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? return_qty { get; set; }
    }

    public class DuplicateProductsModel
    {
        public List<Cust_products> Products { get; set; }
        public Dictionary<int?,int> LinesCount { get; set; }
    }

    public class PeriodData
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime StockQty { get; set; }
        public PeriodData Previous { get; set; }
    }

    public class DisplayOrder
    {
        public string OrderNo { get; set; }
        public Us_dealers Customer { get; set; }
        public DateTime? DateAvailable { get; set; }
        public List<us_display_orders> Lines { get; set; }
    }

    public class DisplayOrdersScheduleModel
    {
        public List<DisplayOrder> DisplayOrders { get; set; }        
        
    }

    public class UsCallLog2Model
    {
        public List<us_call_log2> CallLogList { get; set; }
        public Dictionary<string, int> CallCount { get; set; }
        public bool IsDayLightSaving { get; set; }
        public bool IsTimeSpan { get; set; }
    }

    public class ProductBundleReportModel
    {
        public Dictionary<int?, double?> Arriving { get; set; }
        public IEnumerable<cust_products_bundle> Bundles { get; set; }
        public DateTime Date { get; set; }
    }

    public class BackOrdersReportModel
    {
        public Dictionary<int?, List<Order_lines>> Arriving { get; set; }
        public IEnumerable<cust_products_bundle> Bundles { get; set; }
        public List<Us_backorders> Orders { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<int?, DateTime?> ProductAvailabilityDates { get; set; }
        public Dictionary<int, DateTime?> BundleAvailabilityDates { get; set; }
    }

    public class PackingListBillOfLadingReportRow
    {
        public DateTime? Eta { get; set; }
        public string Container { get; set; }
        public string Po { get; set; }
        public string factory_code { get; set; }
    }

}