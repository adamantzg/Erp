using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IProductFaultsDAL
	{
		List<Product_faults> GetProductFaults(int cprod_id, DateTime? from, DateTime? to);
		List<Product_faults> GetProductFaultsForCompanies(IList<int> company_ids, DateTime? from, DateTime? to);
	}
}