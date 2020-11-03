using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface ISalesDataDal
	{
		List<Cust_products> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null );
		List<Sales_data> GetForPeriod(int cprod_id, int monthFrom, int monthTo);
		List<Sales_data> GetForPeriod(IList<int> cprod_ids,int monthFrom, int monthTo);
		List<Sales_data> GetForCompanyAndPeriod(IList<int> company_ids, int monthFrom, int monthTo);
		List<Sales_data> GetForMastProdAndPeriod(int mast_id, int monthFrom, int monthTo);
		List<Sales_data> GetForMastProdAndPeriod(IList<int> mast_ids, int monthFrom, int monthTo);
		Sales_data GetByProdAndMonth(int cprod_id, int month);
		void DeleteByIdAndMonth(int cprod_id, int? monthFrom = null, int? monthTo = null);
	}
}
