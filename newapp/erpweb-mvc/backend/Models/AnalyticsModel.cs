using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;
using erp.Model.Dal.New;
using backend.Controllers;
using company.Common;

namespace backend.Models
{
    public enum ReportType
    {
        Brands = 0,
        NonBrands = 1
    }

    public enum TopNReturnedMode
    {
        Show6m,
        Show12m,
        ShowBoth
    }

	public enum ProductAnalysisDateMode
	{
		ETD,
		ETA,
		OrderDate
	}

    public class AnalyticsModel
    {
        public const int ReturnsTopRecords = 15;

        public List<SalesByMonth> CurrentSalesData { get; set; }
        public List<SalesByMonth> CurrentSalesDataETA { get; set; }
        public List<SalesByMonth> PreviousYearSalesData { get; set; }

        public List<CustomerSalesByMonth> CurrentCustomerSalesData { get; set; }
        public List<CustomerSalesByMonth> PrivateLabelDistributorsCustomerSalesData { get; set; }
        public List<CustomerSalesByMonth> PreviousCustomerSalesData { get; set; }
        public List<BrandSalesByMonthEx> CurrentBrandSalesData { get; set; }
        public List<BrandSalesByMonthEx> PreviousBrandSalesData { get; set; }
        public List<ProductSales> CurrentProductSalesData { get; set; }
        public List<ProductSales> CurrentProductSalesDataETA { get; set; }
        public List<ProductSales> PreviousProductSalesData { get; set; }
        public double CurrentClaimsTotal { get; set; }
        public double PreviousClaimsTotal { get; set; }
        //public List<OrderStats> OrderStats { get; set; }
        public List<OrderProductGroupStats> OrderStatsNew { get; set; }
        public List<OrderProductGroupStats> OrderStatsETA { get; set; }
        public List<OrderProductGroupStats> OrderStatsProduction { get; set; }
        public List<OrderProductGroupStats> OrderStatsProductionNextNDays { get; set; }
        public List<OrderProductGroupStats> OrderStatsTransit { get; set; }
        public List<OrderBrandsStats> OrderBrandStatsNew { get; set; }
        public List<OrderBrandsStats> OrderBrandStatsETA { get; set; }
        public List<OrderBrandsStats> OrderBrandStatsProduction { get; set; }
        public List<OrderBrandsStats> OrderBrandStatsProductionNextNDays { get; set; }
        public List<OrderBrandsStats> OrderBrandStatsTransit { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStatsNew { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStatsETA { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStatsProduction { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStatsProductionNextNDays { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStatsTransit { get; set; }
        public List<OrderLocationStats> OrderLocationStatsNew { get; set; }
        public List<OrderLocationStats> OrderLocationStatsETA { get; set; }
        public List<OrderLocationStats> OrderLocationStatsProduction { get; set; }
        public List<OrderLocationStats> OrderLocationStatsProductionNextNDays { get; set; }
        public List<OrderLocationStats> OrderLocationStatsTransit { get; set; }
        public DateTime StatsWeek { get; set; }
        public List<ProductStats> ProductStats { get; set; }
        public List<ProductLocationStats> ProductLocationStats { get; set; }
        public List<ProductLocationStatsSummary> ProductLocationStatsSummary { get; set; }
        public List<Brand> Brands { get; set; }
        public Brand Brand { get; set; }
        public List<Company> Factories { get; set; }
        public bool ShowDistributorsInHeader { get; set; }
        public List<Company> Distributors { get; set; }

        public List<Order_line_Summary> SalesByCustomer { get; set; }
        public List<Order_line_Summary> SalesByBrand { get; set; }
        public List<ReturnAggregateDataPrice> ReturnsSummaryByCustomer { get; set; }
        public List<ReturnAggregateDataPrice> ReturnsSummaryByBrand { get; set; }
        public List<Order_line_Summary> PYSales { get; set; }
        public List<Order_line_Summary> PYSalesByBrand { get; set; }
        public List<ReturnAggregateDataPrice> PYReturnsSummaryByCustomer { get; set; }
        public List<ReturnAggregateDataPrice> PYReturnsSummaryByBrand { get; set; }

        public List<Order_line_Summary> Sales6MByCustomer { get; set; }
        public List<Order_line_Summary> Sales6MByBrand { get; set; }
        public List<ReturnAggregateDataPrice> ReturnsSummary6MByCustomer { get; set; }
        public List<ReturnAggregateDataPrice> ReturnsSummary6MByBrand { get; set; }
        public List<Order_line_Summary> PYSales6M { get; set; }
        public List<Order_line_Summary> PYSales6MBrands { get; set; }
        public List<ReturnAggregateDataPrice> PYReturnsSummary6MByCustomer { get; set; }
        public List<ReturnAggregateDataPrice> PYReturnsSummary6MByBrand { get; set; }

        public List<Cust_products> NonSelling { get; set; }
        public List<ProductSales> Top10ByBrand { get; set; }
        public List<ProductSales> Top10Universal { get; set; }
        public List<ProductSales> Top10By907 { get; set; }
        public List<ProductSales> Top10By201 { get; set; }

        public IList ExcludedSections { get; set; }
        public IList<string> IncludedSections { get; set; }

        public List<ReportSection> Sections { get; set; }

        public string Logo { get; set; }
        public CountryFilter CountryFilter { get; set; }
        public ReportType ReportType { get; set; }
        public Guid ChartKey { get; set; }

        public string ProdLocationFileName { get; set; }

        public List<AnalyticsCategorySummaryRow> AnalyticsCategorySummaries { get; set; }
        public List<Analytics_subcategory> AnalyticsSubCategories { get; set; }
        public List<Analytics_options> AnalyticsOptions { get; set; }
        public List<CountrySales> CurrentCountrySalesData { get; set; }
        public List<CountrySales> PreviousCountrySalesData { get; set; }

        public List<ReturnAggregateDataProduct> TopNReturnedProducts6m { get; set; }
        public List<ProductSales> CurrentProductSalesData3m { get; set; }
        public List<ReturnAggregateDataProduct> TopNReturnedProducts12m { get; set; }


        //public List<Returns> CurrentRespondedClaims { get; set; }
        //public List<Returns> PreviousRespondedClaims { get; set; }

        public ProductAnalysisModel ProductAnalysisModel { get; set; }
        public Brand ReportBrand { get; set; }
        public List<Sabc_sort> Sabc { get; set; }
        public int MonthSpan { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<Product_investigations> ProductInvestigations { get; set; }
        public List<Product_investigation_status> ProductInvestigationStatuses { get; set; }
        public List<Product_investigation_images> ProductInvestigationImages { get; set; }

        public Dictionary<int, double?> SpecialTermsAmounts { get; set; }

        public List<BudgetSummaryData> BudgetSummary { get; set; }

        public List<StockSummary> StockSummaries { get; set; }

        public int? brand_id_graph_split { get; set; }

        public DateMode BrandDeliveriesDateMode { get; set; }

        public DateMode BrandShipmentsDateMode { get; set; }

        public SortField SortTopNReturned { get; set; }

        public TopNReturnedMode TopNReturnedMode { get; set; }

        public List<SalesByMonth> SalesByMonth { get; set; }

        public List<BudgetActualData> BudgetActualData { get; set; }

        public List<BudgetActualData> PrivateLabelDistributorsBudgetActualData { get; set; }

        public int? TopNMinUnitsDelivered { get; set; }

        public AnalyticsModel()
        {
            ShowDistributorsInHeader = false;
            TopNReturnedMode = TopNReturnedMode.Show12m;
            ReportCurrency = "GBP";

        }

        public OrdersAnalysisReport OrdersAnalysis { get; set; }

        public Guid StatsKey { get; set; }

        public bool ExpandPreviousForClaimsAnalysis { get; set; }

        public Dictionary<int, List<Change_notice>> dictProductChangeNotices { get; set; }

        public List<Company> IncludedNonDistributors { get; set; }
        public List<OrderProductGroupStats> OrdersInHand { get; set; }
        public List<OrderBrandsStats> OrderBrandStatsInHand { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStatsInHand { get; set; }
        public List<OrderLocationStats> OrderLocationStatsInHand { get; set; }

        public List<OrderClientStat> OrderCountClientProduction { get; set; }
        public List<OrderClientStat> OrderCountClientTransit { get; set; }
        public List<OrderClientStat> PrivateLabelDistributorsOrderCount { get; set; }

        public DistributorBrandSalesModel DistributorSalesModel { get; set; }

        public List<Company> PrivateLabelDistributors { get; set; }
        public SalesOrdersReportModel SalesOrdersModel { get; set; }

        public SalesOrdersMonthlyReportModel SalesOrdersMonthlyModel { get; set; }

        public string ReportCurrency { get; set; }
        public bool ShortNotes { get; set; }

        public OrderDateMode SalesDateMode { get; set; }

        public string TitlePrefix { get; set; }

        public DateMode CustomerShipmentsDateMode { get; set; }

        public List<Countries> Countries { get; set; }
	    public string StartLateGroup { get; set; }
    }

    public class OrderSummaryModel
    {
        public List<OrderProductGroupStats> OrderStats { get; set; }
        public List<OrderBrandsStats> OrderBrandStats { get; set; }
        public List<OrderFactoriesStats> OrderFactoryStats { get; set; }
        public List<OrderLocationStats> OrderLocationStats { get; set; }
        public int brandCount { get; set; }
        public int maxFactories { get; set; }
        public string[] prod_groups { get; set; }
        public List<int> osa_locations { get; set; }
        public string heading { get; set; }

    }

    public class ProductLocationModel
    {
        public List<ProductLocationStats> ProductLocationStats { get; set; }
        public List<ProductLocationStats> AlternateProducts { get; set; }

    }

    public class ReportSection
    {
        public string Name { get; set; }
        public List<ReportSection> Subsections { get; set; }
        public bool Visible { get; set; }
        public ReportSection Parent { get; set; }

        public List<ReportSection> VisibleSections
        {
            get { return Subsections.Where(s => s.Visible).ToList(); }
        }

        public ReportSection()
        {
            Visible = true;
            Parent = null;
        }
    }

    public enum AnalyticsClaimsMode
    {
        Customer,
        Brand
    }

    public class AnalyticsClaimsModel
    {
        public List<Company> Distributors { get; set; }
        public List<Order_line_Summary> Sales { get; set; }
        public List<ReturnAggregateDataPrice> ReturnsSummary { get; set; }
        public List<Order_line_Summary> PYSales { get; set; }
        public List<ReturnAggregateDataPrice> PYReturnsSummary { get; set; }
        public int Months { get; set; }
        public List<Brand> Brands { get; set; }
        public AnalyticsClaimsMode Mode { get; set; }
        public ReportType ReportType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public CountryFilter CountryFilter { get; set; }
        public bool ForCustomer { get; set; }

        public AnalyticsClaimsModel()
        {
            Months = 12;
            Mode = AnalyticsClaimsMode.Customer;
        }
        public bool ExpandPreviousForClaimsAnalysis { get; set; }
    }

    public class NullableItem
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    public class ProductAnalysisModel
    {
        public Brand Brand { get; set; }
        public DateTime StartDate { get; set; }
        public string ChartKey { get; set; }
        public List<Cust_products> CustProducts { get; set; }
        public List<Company> Distributors { get; set; }
        public List<Brand_sales_analysis2> SalesData { get; set; }
        public List<Analytics_categories> Categories { get; set; }
        public List<Analytics_subcategory> Subcategories { get; set; }
        public List<Analytics_options> Options { get; set; }
        public List<AnalyticsSubCatSummaryRow> SortResults { get; set; }
        public List<int> brand_cats { get; set; }
        public List<ProductDistributorDisplayCount> ProductDisplayCounts { get; set; }
        public bool UseEtaForSales { get; set; }
		public ProductAnalysisDateMode DateMode { get; set; }
    }

    public class AnalyticsSubCatSummaryRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double TotalLast6m { get; set; }
        public double TotalGBPLast12m { get; set; }
        public List<STotalsOption> STotalsOpLast6m { get; set; }
        public double TotalPrevious6m { get; set; }
        public List<STotalsOption> STotalsOpPrevious6m { get; set; }
        public double? FactoryStock { get; set; }
        public double? FactoryStockValue { get; set; }
        public Cust_products Product { get; set; }
        public bool HasPendingDiscontinued { get; set; }
        public int? DisplayQty { get; set; }
        public DateTime? FirstShipDate { get; set; }
        public bool MarkDeletion { get; set; }
        public Analytics_categories Category { get; set; }
        public int ProductCount { get; set; }
        public List<CustProduct> Products { get; set; }

        public double Total
        {
            get
            {
                return TotalPrevious6m + TotalLast6m;
            }
        }
    }

    public class CustProduct
    {
        public int cprod_id { get; set; }
        public string cprod_name { get; set; }
        public string cprod_code1 { get; set; }
        public int? brand_user_id { get; set; }
        public string brand_code { get; set; }
        public double? TotalGBPLast12m { get; set; }
        public bool proposed_discontinuation { get; set; }
        public double TotalLast6m { get; set; }
        public double TotalPrevious6m { get; set; }
        public double? FactoryStock { get; set; }
        public int? DisplayQty { get; set; }
        public DateTime? FirstShipDate { get; set; }
        public double Total
        {
            get
            {
                return TotalPrevious6m + TotalLast6m;
            }
        }
    }


    public class STotalsOption
    {
        //string Name { get; set; }
        public double? TotalList { get; set; }
    }

    public class ReportHomeModel
    {
        public List<Brand> Brands { get; set; }
        public List<Company> Factories { get; set; }
        public List<FactorySalesByMonth> FactoriesBySales { get; set; }
        public List<FactoryLine> FactoriesLines { get; set; }
    }

    public class FactoryLine
    {
        public string IdFactory { get; set; }
        public string Name { get; set; }
        public double Suma { get; set; }
    }

    public class ClaimsInvestigationModel
    {
        public List<ClaimsInvestigationRow> Products { get; set; }
        public List<ProductSales> Sales { get; set; }
        public List<ClaimsAnalyticsRow> Claims { get; set; }
        public List<ReturnAggregateDataProduct> Top10ReturnedProducts6m { get; set; }
    }

    public class ClaimsInvestigationRow
    {
        public Cust_products Product { get; set; }
        public DateTime? DateAdded { get; set; }
    }

    public class BudgetSummaryData
    {
        public int Month21 { get; set; }
        public double? Amount { get; set; }
    }

    public class BrandStockReportModel
    {
        public List<BrandSalesByMonthEx> ProductData { get; set; }
        //public List<BrandSalesByMonthEx> CustomerData { get; set; }
        public List<Analytics_subcategory> AnalyticsSubcategories { get; set; }
        public List<Analytics_categories> AnalyticsCategories { get; set; }
        public bool ShowRegularProducts { get; set; }
        public List<Brand> Brands { get; set; }
        public string CustomerCode { get; set; }
        public double UpperFactor { get; set; }
        public double LowerFactor { get; set; }
        public int ProductShowThreshold { get; set; }
        public int ProductShowThresholdBrand { get; set; }
        public int HighStatusThreshold { get; set; }
        public int LowStatusThreshold { get; set; }
        public List<ProductSales> SalesAfterStockDate { get; set; }
        public int MonthsForAverage { get; set; }

        public BrandStockReportModel()
        {
            ProductShowThreshold = 20;
        }
    }

    public class GetBrandStockReportModel
    {
        public List<string> CustomerCodes { get; set; }
        public List<Brand> Brands { get; set; }
        public bool ShowRegularProducts { get; set; }
        public double LowerFactor { get; set; }
        public double UpperFactor { get; set; }
        public int ProductShowThreshold { get; set; }
    }

    public class BrandStockReportRow
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<MonthQty> MonthValues { get; set; }
        public double? Factor { get; set; }

        public double? Requirement
        {
            get
            {
                if (MonthValues != null && MonthValues.Count == 7)
                    return (Factor * MonthValues.Take(4).Sum(m => m.Qty) / 4.0 * 3 - MonthValues.Skip(4).Sum(m => m.Qty));
                return 0;
            }
        }

        public int? FutureQty { get; set; }
        public int? ExpectedReceipts { get; set; }

        public string Warning
        {
            get
            {
                var first4MonthsAvg = MonthValues.Take(4).Average(m => m.Qty) ?? 0;
                if (Requirement - (FutureQty ?? 0) > LowerFactor * first4MonthsAvg)
                    return "below expectation";
                if (Requirement - (FutureQty ?? 0) < -1 * UpperFactor * first4MonthsAvg)
                    return "above expectation";
                return string.Empty;
            }
        }
        public List<BrandStockReportRow> Children { get; set; }

        public static double UpperFactor { get; set; }
        public static double LowerFactor { get; set; }

        public BrandStockReportRow(double? factor)
        {
            Factor = factor;
        }

        static BrandStockReportRow()
        {
            UpperFactor = 1;
            LowerFactor = 1;
        }

    }

    public class BrandStockReportRow2
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public string ProductGroup { get; set; }
        public DateTime? StockDate { get; set; }
        public List<MonthQty> MonthValues { get; set; }
        public double Factor { get; set; }
        public int HighStatusThreshold { get; set; }
        public int LowStatusThreshold { get; set; }
        public int? SalesAfterStockDate { get; set; }

        public static double UpperFactor { get; set; }
        public static double LowerFactor { get; set; }
        public static int MonthsForAverage { get; set; }


        public int LeadTime
        {
            get
            {
                const int result = 42;
                int offset;
                switch (ProductGroup)
                {
                    case "S":
                        offset = 14;
                        break;
                    case "A":
                        offset = 21;
                        break;
                    case "B":
                        offset = 28;
                        break;
                    case "C":
                        offset = 35;
                        break;
                    case "C+":
                        offset = 42;
                        break;
                    default:
                        offset = 42;
                        break;
                }
                return result + offset;
            }
        }

        public double? AvgWeeklySales
        {
            get { return MonthValues.Take(MonthsForAverage).Sum(m => m.Qty) / (1.0 * (30 * MonthsForAverage)) * 7; }
        }

        public double? Requirement
        {
            get
            {
                var factor = Factor;
                if (factor > UpperFactor)
                    factor = UpperFactor;
                if (factor < LowerFactor)
                    factor = LowerFactor;
                if (MonthValues != null && MonthValues.Count == (MonthsForAverage + 3) && StockDate != null)
                    return factor * AvgWeeklySales * (LeadTime + 30 + (DateTime.Today - StockDate.Value).TotalDays) / 7 - SalesAfterStockDate - Stock;
                return 0;
            }
        }

        public string Status
        {
            get
            {
                var ratio = -1 * Requirement / AvgWeeklySales;
                if (ratio > HighStatusThreshold)
                    return "High";
                if (ratio < LowStatusThreshold)
                    return "Low";
                return "Ok";
            }
        }

        public int? FutureQty { get; set; }
        //public int? ExpectedReceipts { get; set; }

        public BrandStockReportRow2(double factor, int highStatusThreshold, int lowStatusThreshold)
        {
            Factor = factor;
            HighStatusThreshold = highStatusThreshold;
            LowStatusThreshold = lowStatusThreshold;
        }
    }

    public class MonthQty
    {
        public int Month21 { get; set; }
        public int? Qty { get; set; }
    }

    public class CategoryData
    {
        public Analytics_categories Category { get; set; }
        public List<BrandStockReportRow> Data { get; set; }
    }

    public class CategoryData2
    {
        public Analytics_categories Category { get; set; }
        public List<BrandStockReportRow2> Data { get; set; }
    }

    public class AnalysisProductInfoModel
    {
        public OrdersAnalysisProductReport OrdersAnalysisProductReport { get; set; }
        public List<Brand> Brands { get; set; }
        public Company Client { get; set; }
        public DateTime From { get; set; }
        public List<Company> Factories { get; set; }
        public bool SeparateNewProducts { get; set; }
        public bool ShowNewProducts { get; set; }
        public bool IncreaseDecreaseSplit { get; set; }
        public List<double> SummaryDataPoints { get; set; }
		public int DaysLeadTime { get; set; }
    }

    public class BrandClaimsAnalysisModel
    {
        //key is brand_id
        public Dictionary<int, List<ProductSales>> ProductSalesLast { get; set; }
        public Dictionary<int, List<ProductSales>> ProductSalesPrevious { get; set; }
        public Dictionary<int, List<Returns>> Returns { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Return_category> ReturnCategories { get; set; }
        public TwoPeriods Periods { get; set; }
        public bool ShowDetails { get; set; }
        public CountryFilter CountryFilter { get; set; }
        public double LowThresholdPercent { get; set; }
        //key is brand_id
        public Dictionary<int, Dictionary<int, int>> WeeksForClaimsSalesRatio { get; set; }
    }

    public class BrandClaimsAnalysisProductModel
    {
        public Cust_products Product { get; set; }
        public List<ProductSales> ProductSalesLast { get; set; }
        public List<Returns> Returns { get; set; }
        public List<Return_category> ReturnCategories { get; set; }
        public TwoPeriods Periods { get; set; }
        public int ChartWidth { get; set; }
        public int ChartHeight { get; set; }
        public CountryFilter CountryFilter { get; set; }
    }

    public class TwoPeriods
    {
        public DateTime FirstPeriodStart { get; set; }
        public DateTime FirstPeriodEnd { get; set; }
        public DateTime LastPeriodStart { get; set; }
        public DateTime LastPeriodEnd { get; set; }
    }

    public class DistributorBrandSalesModel
    {
        public List<Brand> Brands { get; set; }
        public List<Company> Distributors { get; set; }
        public List<Distributor_sales> Sales { get; set; }
        public Month21 ForMonth { get; set; }
        public bool SalesInThousands { get; set; }
    }

    public class SalesOrdersMonthlyReportModel
    {
        public string Warehouse { get; set; }
        public DateTime ForDate { get; set; }
        public List<SalesOrdersMonthlyReportRow> Rows { get; set; }
        public List<BudgetActualData> BudgetData { get; set; }
    }

    public class SalesOrdersMonthlyReportRow
    {
        public Month21 Month21 { get; set; }
        public double? Amount { get; set; }
    }

    public class BrandStockSummaryModel
    {
        public bool Debug { get; set; }

        public Brand Brand { get; set; }
        public Guid ChartKey { get; set; }

        public DateTime ForDate { get; set; }
        public double? StockValueUSA { get; set; }
        public double? StockValueOthers { get; set; }

        public List<CostValueItem> CostValueItemListUSA { get; set; }
        public List<StockReceiptItem> StockReceiptItemListUSA { get; set; }
        public List<CostValueItem> CostValueItemListOthers { get; set; }
        public List<StockReceiptItem> StockReceiptItemListOthers { get; set; }

        public List<BrandStockSummaryChartItem> DataForChart { get; set; }
    }

    public class CustomerSalesByMonthRow
    {
        public string region { get; set; }
        public string customerName { get; set; }
        public Company Client { get; set; }
        public double? currentAmount { get; set; }
        public double? currentRatio { get; set; }
        public double? previousAmount { get; set; }
        public double? previousRatio { get; set; }
        public double? currentPreviousRatio { get; set; }
    }

    public class LineFillRateReportModel
    {
        public DateTime? From { get; set; }
        public Guid ChartKey { get; set; }
        public List<Order_line_detail2_v6> AllOrders { get; set; }
        public List<Order_line_detail2_v6> DistributorGrouppedOrders { get; set; }
    }

    public class AvgLeadTimeOnShippedOrdersModel
    {
        public DateTime? From { get; set; }
        public Guid ChartKey { get; set; }

        public List<Order_line_detail2_v6> GrouppedData { get; set; }
    }
}