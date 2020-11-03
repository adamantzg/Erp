using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface ISalesForecastDal
	{
		List<Sales_forecast> GetForecastForPeriod(int monthFrom, int monthTo, List<int> numCprodUser);
		List<Sales_forecast> GetForPeriod( int cprod_id,int monthFrom, int monthTo);
		List<Sales_forecast> GetForPeriod(IList<int> cprod_ids, int monthFrom, int monthTo);
		List<Sales_forecast> GetForMastProdAndPeriod(int id, int monthFrom, int monthTo);
		List<Sales_forecast> GetForMastProdAndPeriod(IList<int> ids, int monthFrom, int monthTo);
		void DeleteByIdAndMonth(int cprod_id, int? monthFrom = null, int? monthTo = null);
	}
}
