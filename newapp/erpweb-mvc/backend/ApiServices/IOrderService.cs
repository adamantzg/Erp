using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.ApiServices
{
	public interface IOrderService
	{
		double? GetSuggestedOrder(double initialStock, DateTime fromDate, DateTime arrivalDay, List<Order_lines> arrivingLines, 
			List<Sales_forecast> forecasts, List<int> companies_BookedInDate, int? client_id, int weeksOfStock);
		DateTime? ResolveDate(Order_lines l, List<int> companies_BookedInDate, int? client_id);
		int GetSalesForecastInPeriod(DateTime from, DateTime to, List<Sales_forecast> forecasts);
	}
}
