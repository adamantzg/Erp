using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface ILoginHistoryDetailDAL : IGenericDal<Login_history_detail>
	{
		List<login_history_page_count> GetPageCount(IList<int> adminTypesToInclude = null, IList<int> adminTypesToExclude = null, DateTime? dateFrom = null, DateTime? dateTo = null, 
			bool groupByPageType = false, IList<int> companyIds = null, string excludedCountries = null);
	}
}