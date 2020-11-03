using System.Collections.Generic;
using erp.Model;

namespace backend.ApiServices
{
	public interface ICompanyService
	{
		void ProcessFileUrls(IList<Company> companies);
	}
}