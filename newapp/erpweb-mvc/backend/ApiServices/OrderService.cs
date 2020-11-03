using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using company.Common;
using erp.Model;
using Utilities = company.Common.Utilities;

namespace backend.ApiServices
{
	public class OrderService : IOrderService
	{
		public int GetSalesForecastInPeriod(DateTime from, DateTime to, List<Sales_forecast> forecasts)
		{
			var date = from;
			var forecast = 0.0;
			while(date < to)
			{
				var endDate = Utilities.Min(Utilities.GetMonthEnd(date), to);
				forecast += (forecasts.FirstOrDefault(f => f.month21 == Month21.FromDate(date).Value)?.sales_qty ?? 0) * 
				(endDate.Day - (date.Day == 1 ? 0 : date.Day)) * 1.0 / DateTime.DaysInMonth(date.Year, date.Month);
				date = Utilities.GetMonthStart(date.AddMonths(1));
			}
			return Convert.ToInt32(Math.Round(forecast,0));
		}

		public double? GetSuggestedOrder(double initialStock, DateTime fromDate, DateTime arrivalDay, List<Order_lines> arrivingLines, 
			List<Sales_forecast> forecasts, List<int> companies_BookedInDate, int? client_id, int weeksOfStock)
		{
		
			//calculate last day of current month and stock
			
			/*var endDate = Utilities.Min(Utilities.GetMonthEnd(date), arrivalDay);
			var forecast = (forecasts.FirstOrDefault(f => f.month21 == Month21.FromDate(date).Value)?.sales_qty ?? 0) * 
				(endDate.Day - date.Day) / date.Day;*/
			var forecastUntilArrival = GetSalesForecastInPeriod(fromDate, arrivalDay, forecasts);
			var arrivingOrdersSum =
                    arrivingLines.Where(l => ResolveDate(l,companies_BookedInDate, client_id) != null && 
					ResolveDate(l,companies_BookedInDate, client_id) >= fromDate && 
					ResolveDate(l,companies_BookedInDate, client_id) <= arrivalDay).Sum(l => l.orderqty) ?? 0.0;
			var stockOnArrival = initialStock + arrivingOrdersSum - forecastUntilArrival;

			var forecastAfterArrival = GetSalesForecastInPeriod(arrivalDay.AddDays(1), arrivalDay.AddDays(weeksOfStock*7), forecasts);
			
			return stockOnArrival < forecastAfterArrival ? (double?) forecastAfterArrival - stockOnArrival : null;			

		}

		public DateTime? ResolveDate(Order_lines l, List<int> companies_BookedInDate, int? client_id)
		{
			var result = l.Header.req_eta;
			if (client_id != null && companies_BookedInDate.Contains(client_id.Value)) {
				result = l.Header.req_eta_norm;
			}
			return result;
		}
	}
}