using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IOrderHeaderDAL
	{
		List<Order_header> GetAll();
		List<Order_header> GetCreatedInPeriod(DateTime? from, DateTime? to, bool gbie_only = true);
		List<Order_header> GetProducedInPeriod(DateTime? from = null, DateTime? to = null,DateTime? created_until = null, bool gbie_only = true);
		List<OrderMonthlyData> GetQtyByMonth(string cprod_code1, int company_id, DateTime fromMonth, DateTime toMonth);
		List<OrderWeeklyData> GetCountByWeek_Users(DateTime weekStart, int weekspan);
		List<OrderWeeklyData> GetCountByWeek_Factories(DateTime weekStart, int weekspan);
		int? GetTotalQty(string cprod_code1, int company_id);
		Order_header GetById(int id,bool includeLines = false);
		DateTime? GetMaxEtd(List<int> ids );
		void Create(Order_header o);
		void Update(Order_header o);
		void Delete(int orderid);
		Order_header GetByCustpo(string custpo);
		List<Company> GetClientsOnOrders(IList<int> factory_ids=null, bool combined = false);
		List<Cust_products> GetProductsOnOrders(int? location_id = null,int? factory_id=null, int? client_id=null, string criteria = "", bool spares=false, bool discontinued=false,int? brand_user_id = null, IList<int> factory_ids = null, bool outofstockonly = false, string analysis_d = null, string excluded_distributors = null, int? category1Id = null, bool spares_only = false, string excluded_custproducts_cprodusers = null);
		int GetNumberOfOrders(int cprod_id, DateTime? from = null, DateTime? to = null);
		List<Order_header> GetByClient(int user_id, string custpo = null);
		List<Order_header> GetCombinedOrders(int orderid);
	}
}