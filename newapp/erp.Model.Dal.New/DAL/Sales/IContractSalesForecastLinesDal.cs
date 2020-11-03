using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IContractSalesForecastLinesDal
	{
		List<Contract_sales_forecast_lines> GetByForecastId(int forecast_id);
		List<Contract_sales_forecast_lines> GetForMastProductAndPeriod(int mast_id, DateTime dateFrom, DateTime dateTo);
		List<Contract_sales_forecast_lines> GetForPeriod(IList<int> cprod_ids, DateTime dateFrom, DateTime dateTo);
		List<Contract_sales_forecast_lines> GetForPeriod(int cprod_id, DateTime dateFrom, DateTime dateTo);
	}
}