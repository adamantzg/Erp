using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public interface IOrderLinesDAL : IGenericDal<Order_lines>
	{
		List<Order_lines> GetByOrderId(int order_id);
		List<Order_lines> GetByOrder();
		List<Order_line_detail2_v6> GetOrderingPatterns(string customer_code,DateTime ordersFrom,DateTime ordersTo);
		List<Order_lines> GetByCustPo(string custPo, int? company_id = null);
		List<Order_lines> GetByCustPos(string[] custPos);
		List<Order_lines> GetByCriteria(int? month, int? year, string criteria, int company_id);
		List<Order_lines> GetForProductAndCriteria(int? month, int? year, int cprod_id, int? company_id = null, bool limitETA = false);
		int GetNumOfPreviousShipments(DateTime po_req_etd, int cprod_id);
		List<Order_lines> GetByProduct(int id);
		List<Order_lines> GetByOrderIds(IList<int> order_ids);

		List<Order_lines> GetByExportCriteria(IList<int> cprod_ids=null, List<int> mast_ids=null, int? client_id=null, DateTime? etd_from = null,DateTime? 
				etd_to=null, int? factory_id=null, List<string> custpoList = null, IList<int> factory_ids = null, IList<int> excludedClients = null,
			IList<int> orderids = null, bool useLikeForCustpos = true, int? location_id = null);

		List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids, int? client_id,
			DateTime? eta_from, DateTime? eta_to, int? factory_id);

		List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids,List<int> client_ids = null, DateTime? eta_from = null, DateTime? eta_to = null, int? factory_id = null);
		List<Order_lines> GetStockOrderLinesInFactory(IList<int> cprod_ids, DateTime? from = null);
		List<Order_lines> GetNonShippedRegularOrderLines(IList<int> cprod_ids, DateTime? from = null);
		List<Order_lines> GetCalloffOrdersInShipment(IList<int> cprod_ids, DateTime? from = null);

		List<Order_lines> GetInvoicedOrderLines2(string custpo, DateTime? ciDateFrom = null,
			DateTime? ciDateTo = null, DateTime? etFrom = null,
			DateTime? etTo = null, IList<int> clientIds = null, OrderDateMode orderDateMode = OrderDateMode.EtaPlusWeek,
			IList<int> excludedClientIds = null, bool loadPorderLines = false, IList<int> factoryIds = null,
			bool usePriceOverride = false, int? invoice_sequence = null, bool? UK = null, bool? checkCompanyEtdFrom = false);

		List<Order_lines> GetLiveOrdersAnalysis(DateTime from, CountryFilter countryFilter = CountryFilter.UKOnly,int? client_id = null, 
			IList<int> includedNonDistributorsIds = null, bool brands = true);

		List<int> GetByOriginalOrderids(string ids);

        List<Order_line_detail2_v6> GetOrderLinesExport(int? clientId = null, int? factoryId = null, int? categoryId = null, bool? etaetd = null, DateTime? dateFrom = null, DateTime? dateTo = null);
        List<Order_line_detail2_v6> GetProductsOrder(List<string> cprodcodes, DateTime? from, int? userid);
		List<Order_lines> GetLinesWithSentToFactoryDate(int? factory_id = null, DateTime? from = null, DateTime? to = null);
	}
}