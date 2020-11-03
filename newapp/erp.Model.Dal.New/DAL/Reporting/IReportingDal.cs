using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IReportingDal
	{
		List<WeightDifferenceRow> GetProductsWithUpdatedWeight(DateTime? etd_from, DateTime? etd_to);
		List<OrderRow> GetOutstandingOrdersChangedETD(List<int> userIds);
		List<OrderRow> GetOrdersReport(List<int> userIds, DateTime etd, int type = 1);
		List<OrderSummaryByLocationClientRow> GetSummaryByLocationClient(int brand_id, DateTime date,
			CountryFilter countryFilter = CountryFilter.UKOnly);
		List<ProductDistributorDisplayCount> GetDisplayCountByCustomer(int brand_id,
			CountryFilter countryFilter = CountryFilter.UKOnly);
		int GetDisplayCountForBrand(int brand_id, CountryFilter countryFilter = CountryFilter.UKOnly);
		List<ReturnsByCustomer> GetReturnsByCustomer(int brand_id,
			CountryFilter countryFilter = CountryFilter.UKOnly);
		List<DealerSalesByCustomer> GetDealerSalesByCustomers(int brand_id,
			CountryFilter countryFilter = CountryFilter.UKOnly, IList<string> customersForSalesData = null);
		int GetDealerSalesForBrand(int brand_id, CountryFilter countryFilter = CountryFilter.UKOnly);

	}
}
