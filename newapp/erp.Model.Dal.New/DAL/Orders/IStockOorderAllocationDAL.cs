using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IStockOrderAllocationDAL
	{
		List<Stock_order_allocation> GetByProducts(IList<int> cprod_ids);
		List<soalloc_overallocation> GetOverAllocations();
		List<Order_lines> GetAllocationCalloffLines(int stocklineNum);
		List<Order_lines> GetAllocationSOLines(int colineNum);
		List<Order_lines> GetAvailableStockLines(int cprod_id);
		List<OrderLinesLight> GetOrderLineLightForReport(int? cprod_id);
	}
}