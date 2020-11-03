using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public interface ICompanyRepository : IGenericRepository<Company>
	{
		List<Company> GetFactories(bool combined = false, int? location_id = null, bool? files = null);
		List<Company> GetClientsWithOrders(bool combined = false);
	}
}
