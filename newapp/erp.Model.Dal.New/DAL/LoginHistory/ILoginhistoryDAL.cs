using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface ILoginhistoryDAL : IGenericDal<Login_history>
	{
		Login_history GetByCriteria(string session_id, DateTime? login_date);

		List<Login_history> GetByCriteria(IList<int> company_ids, DateTime? dateFrom, DateTime? dateTo, IList<int> adminTypesToInclude = null, 
			IList<int> adminTypesToExclude = null, bool showAllActiveUsers = false,IList<User> activeUsers = null, 
			string companiesAdminTypesMappings = "", string excludedCountries = null);

		List<Login_history> GetForWebsite(string website, DateTime? dateFrom, DateTime? dateTo);

		Dictionary<string, DateTime?> GetLastLoginDates(IList<int> company_ids, DateTime? from);
		List<Company> GetCompanies();
	}
}