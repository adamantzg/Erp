using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IOrderLinesManualDAL
	{
		List<Order_lines_manual> GetAll();
		List<Order_lines_manual> GetForOrder(int orderid);
		List<Order_lines_manual> GetForOrders(IEnumerable<int?> orderids);
		Order_lines_manual GetById(int id);
		void Create(Order_lines_manual o);
		void Update(Order_lines_manual o);
		void Delete(int linenum);
	}
}