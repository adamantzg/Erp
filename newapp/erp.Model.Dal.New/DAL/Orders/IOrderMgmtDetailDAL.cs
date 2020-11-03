using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IOrderMgmtDetailDAL
	{
		List<OrderMgtmDetail> GetByMastProduct(int mast_id, DateTime eta_from, DateTime eta_to);
		List<OrderMgtmDetail> GetByProducts(IList<int> cprod_ids, DateTime eta_from, DateTime eta_to);
		List<ProductSaleMonthSummary> GetMonthSaleByFactory(int? factory_Id, int? month21_from = null, int? month21_to = null);
		List<ProductOrderSummary> GetOrdersTotal(DateTime? from, DateTime? to);
		List<ProductSaleSummary> GetSaleByProductIds(List<int> ids, DateTime? etd_from = null, DateTime? etd_to = null);
		List<OrderMgtmDetail> SearchLines(string product, string po, int? factory_id, int? client_id, DateTime? etd_from, DateTime? etd_to, SearchCategory category, int orderby);
	}
}