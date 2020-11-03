using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IBrandSalesAnalysis2DAL : IGenericDal<Brand_sales_analysis2>
	{
		List<Brand_sales_analysis2> GetForAnalyticsSubcats(DateTime? etdFrom, DateTime? etdTo = null, bool useETA = false, CountryFilter countryFilter = CountryFilter.UKOnly);
		List<Brand_sales_analysis2> GetForBrand(int? brand_user_id, DateTime? etdFrom, bool useETA = false, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2,CWB", OrderDateMode dateMode = OrderDateMode.Etd);
	}
}