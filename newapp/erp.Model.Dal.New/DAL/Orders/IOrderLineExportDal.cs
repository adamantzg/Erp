using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IOrderLineExportDal
	{
		void GetAllocationCOLines(IEnumerable<Order_line_export> lines);
		void GetAllocationLines(IEnumerable<Order_line_export> lines, string type = "so");
		List<Company> GetClients();
		List<Order_line_Summary> GetCustomerSummaryForPeriod(DateTime? from, DateTime? to, int? brand_user_id = null, string cprod_code = null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2", bool brands = true);
		List<Company> GetFactories();
		List<Order_line_Summary> GetFactorySummaryForPeriod(DateTime? from, DateTime? to, int? brand_user_id = null, string cprod_code = null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2");
		List<Order_line_export> GetForCriteria(List<int> factory_ids, DateTime? etaFrom = null, DateTime? etdFrom = null, IList<int> client_ids = null, bool includeDiscontinued = true);
		List<Order_line_export> GetForPeriodV6(int monthFrom = 0, int monthTo = 0, IList<string> cprodCode = null);
		List<Order_line_export> GetShippingForProduct_V6(int cprod_id);
	}
}