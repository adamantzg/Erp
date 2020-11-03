using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IReturnsDAL : IGenericDal<Returns>
	{
		List<Returns> GetAllForProduct(int cprod_id);
		List<Returns> GetByClient(int client_id);
		List<Returns> GetInPeriod(DateTime? from = null, DateTime? to = null);
		List<Returns> GetForClaimType(int claim_type, bool products=false, IList<int?> groupsId = null);
		List<Returns> Search(int? claim_type, string text);
		List<ReturnAggregateDataByMonth> GetQtyForPeriodGroupedByMonths(int cprod_user, IList<string> cprodCode);
		int GetNoOfReturns(int company_id, DateTime? date = null,int? claim_type = null);
		int GetNextITFeedbackNum();
		int GetNextFeedbackNum(int type);
		ReturnAggregateData GetQtyInPeriod(string cprod_code1, int company_id, DateTime? dateFrom = null, DateTime? dateTo = null);

		List<ReturnAggregateDataProduct> GetTopNTotalsPerProduct(
			DateTime? dateFrom = null,DateTime? dateTo = null, int? top = 10, IList<int> incClients = null,
			IList<int> exClients = null, bool groupByBrands = true,bool excludeSpares = true,
			bool groupByReason = true,bool useETA=false,bool extendToEndOfMonthForUnits = false, bool filterCprodStatus=true,CountryFilter countryFilter = CountryFilter.UKOnly,
			SortField sortBy = SortField.ReturnToSalesRatio,int? minUnitsShipped = null);

		List<ReturnAggregateDataPrice> GetTotalsPerClient(DateTime? dateFrom = null, DateTime? dateTo = null, int? brand_user_id = null, IList<int> incClients = null, IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2");
		List<ReturnAggregateDataPrice> GetTotalsPerFactory(DateTime? dateFrom = null, DateTime? dateTo = null, int? brand_user_id = null, IList<int> incClients = null, IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2");
		void UpdateRejection(Returns o);
		void UpdateSimpleRecheck(Recheck o);
		void Update(Returns o, List<int> deletedImagesIds = null);
		List<ClaimsAnalyticsRow> GetAnalytics(List<int> ids, DateTime? from = null, DateTime? to = null);
		List<ClaimsAnalyticsRow> GetAnalytics(string cprod_code1, DateTime? from = null, DateTime? to = null);
		void UpdateOpenClose(int feedback_id,int openClose);
		void UpdateStatus(int id, FeedbackStatus status, int? authorizationLevel = null);
		List<Returns> GetForMastProducts(IList<int> mast_ids, bool commentsOnly = false, DateTime? fromDate = null);
		string GetCustomerIdInWhereClause(string customerIds, string inOut);
		List<CAReportCAItem> GetCAReportCAItems(DateTime dateForCAFrom, DateTime dateForCATo, string customerIdsIn,string customerIdsOut);
		CAReportInspectionItem GetCAReportInspectionItem(DateTime dateForCAInspectionItemFrom, DateTime dateForCAInspectionItemTo, string customerIdsIn, string customersIdOut);
		List<Returns> GetForClaimTypeSimple(int claim_type, bool products = false, IList<int?> groupsId = null);
	}
}

