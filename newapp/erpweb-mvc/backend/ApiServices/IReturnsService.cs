using System;
using System.Collections.Generic;
using erp.Model;

namespace backend.ApiServices
{
	public interface IReturnsService
	{
		List<Returns_comments> GetComments(int return_id);
		List<Returns> GetFeedbacks(IList<int> cprod_ids, int type);
		List<Returns> GetForFactories(IList<int> factoryIds, bool commentsOnly = false, DateTime? fromDate = null);
		List<Returns> GetForPeriodAndBrand(int brand_id, DateTime? from = null, DateTime? to = null);
		List<Returns> GetForPeriodAndProduct(int cprod_id, DateTime? from = null, DateTime? to = null);
		List<ReturnAggregateDataByMonth> GetReturnsByMonth(DateTime? from = null, DateTime? to = null, bool acceptedOnly = true, IList<int> cprod_ids = null);
		List<ReturnAggregateDataProduct> GetReturnsByReason(DateTime? from = null, DateTime? to = null, IList<int> cprod_ids = null, bool acceptedOnly = true);
		List<ReturnAggregateDataPrice> GetTotalsPerBrand(DateTime? dateFrom = null, DateTime? dateTo = null, IList<int> incClients = null, IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly);
	}
}