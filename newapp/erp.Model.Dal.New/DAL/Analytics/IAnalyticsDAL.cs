using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public enum BrandSalesGroupOption
	{
		Default,    //depeding on brand flag
		ByBrand
	}

	public interface IAnalyticsDAL
	{
		List<ProductSales> GetProductSales(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly,
			int? brand_user_id = null, IList<int> factories = null, DateTime? fromDate = null, DateTime? toDate = null, bool brands = true,
			bool monthBreakDown = false, IList<int> incClients = null, IList<int> exClients = null, int? brand_id = null, bool ignoreFactoryCode = false,
			bool useETA = false, IList<int> prodIds = null, bool useOrderDate = false, bool clientBreakDown = false, bool useLineDate = false,
			bool shippedOrdersOnly = true, bool periodBreakDown = false, IList<string> excludedCustomers = null);

		List<SalesByMonth> GetSalesByMonth(int from, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null,
			bool useETA = false, bool? brands = true, int[] includedClients = null, int[] excludedClients = null, List<int> factory_ids = null,
			int? brand_id = null, string excludedCustomers = "NK2", IList<int> includedNonDistributors = null, bool useOrderDate = false,
			IList<int> productIds = null, bool factoryProducts = false, bool useCompanyPriceType = false);

		List<ProductSales> GetSalesForCategory(int brand_category);

		List<CustomerSalesByMonth> GetCustomerSalesByMonth(int? @from = null, int? to = null, bool UK = true, CountryFilter countryFilter = CountryFilter.UKOnly, int? userId = null,
			bool brands = true, IList<int> includedClients = null, int[] excludedClients = null, int? brand_id = null, DateTime? fromDate = null, DateTime? toDate = null, bool periodLevel = false,
			bool useETA = false, string excludedCustomers = "NK2", bool groupByMonth = false, IList<int> includedNonDistributors = null, bool useCompanyPriceType = false);

		List<Returns> GetRespondedClaims(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null);

		List<OrderProductGroupStats> GetOrderProductGroupStats_New(DateTime from, DateTime to,
							CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null,
							IList<int> includedNonDistributors = null);
		List<OrderBrandsStats> GetOrderBrandStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null);
		List<OrderFactoriesStats> GetOrderFactoryStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null);

		List<OrderLocationStats> GetOrderLocationStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null);

		List<OrderProductGroupStats> GetOrderProductGroupStats_ETA(DateTime from, DateTime to,
				CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null,
				IList<int> includedNonDistributors = null);
		List<OrderBrandsStats> GetOrderBrandStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null);

		List<OrderFactoriesStats> GetOrderFactoryStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null);

		List<OrderLocationStats> GetOrderLocationStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null);

		List<OrderProductGroupStats> GetOrderProductGroupStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
													CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null,
													int[] excludedClients = null, int? daysToShipping = null,
													DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null);

		List<OrderBrandsStats> GetOrderBrandStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
			CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null,
			DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null);
		List<OrderFactoriesStats> GetOrderFactoryStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
			CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null);

		List<OrderLocationStats> GetOrderLocationStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
			CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null);

		List<BrandSalesByMonthEx> GetBrandSalesByMonth(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, BrandSalesGroupOption groupOption = BrandSalesGroupOption.Default,
			DateTime? fromDate = null, DateTime? toDate = null, bool productLevel = false, bool customerLevel = false, string customerCode = "", int? brand_id = null,
			bool useETA = false, bool includePendingForDiscontinuation = true, bool usePeriod = false, string excludedCustomers = "NK2", bool groupByPeriod = false, IList<int> includedNonDistributors = null, bool useCompanyPriceType = false);
		List<StockSummary> GetStockSummaryReports2(IList<int> includedClients, double qcCharge, double duty, double freight, DateTime? from = null);
		List<string> GetFactoriesOnStockOrders(IList<int> clientIds, DateTime from);
		List<int> GetFactoriesOnStockOrders_NoCombined(IList<int> clientIds, DateTime from);
		List<ProductSales> GetTopNByBrands(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly);
		List<ProductSales> GetTopNUniversal(int n, int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly);
		List<ProductSales> GetTopForBrandCat(int n, int from, int to, int cprod_brand_cat, CountryFilter countryFilter = CountryFilter.UKOnly);
		List<Cust_products> GetNonSelling(int from, int to, int? brand_user_id = null);
		List<AnalyticsCategorySummaryRow> GetAnalyticsCategorySummary(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, string excludedCustomers = "NK2");
		List<ProductDisplayCount> GetDisplayCountForProducts(int brand_category);
		List<ProductDistributorDisplayCount> GetDisplayCountForProductsAndBrand(int? brand_user_id, IList<int> category_ids);

		List<ProductLocationStats> GetProductLocationStats(string product_group, int? ageForExclusion = null, IList<string> distCountries = null);
		List<FactorySalesByMonth> GetSalesOfOrdersByMonthForFactories(int from, int to);
		List<Company> GetFactoriesFromSales(int? month21From = null, int? month21To = null);

		List<SalesByMonth> GetNumOfOrdersByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool? brands = true,
			int[] includedClients = null, int[] excludedClients = null, int[] excludedContainerTypes = null, int[] includedContainerTypes = null,
			IList<int> factoryIds = null, int? brand_id = null, bool useOrderDate = false, string excludedCustomersString = "");
		List<SalesByMonth> GetNumOfOrdersByMonthForFactories(int from, int to, IList<int> factoryIds = null);

		List<SalesByMonth> GetProfitByMonth(int from, int to);

		List<Category1> GetSubCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null);
		List<Category1> GetCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null);
		List<Category1SalesByMonth> GetSubCategorySalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null);
		List<Category1SalesByMonth> GetCategory1SalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null);
		List<CountrySales> GetCountrySales(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, IList<int> factoryIds = null);
		List<Order_line_Summary> GetBrandSummary(DateTime? from, DateTime? to, bool brands_only = false);
		List<SalesByMonth> GetSalesByMonthNoView(int from, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null,
			bool useETA = false, bool? brands = true, int[] includedClients = null, int[] excludedClients = null, List<int> factory_ids = null,
			int? brand_id = null, string excludedCustomers = "NK2", IList<int> includedNonDistributors = null, bool useOrderDate = false,
			IList<int> productIds = null, bool factoryProducts = false, bool useCompanyPriceType = false);

	}
}
