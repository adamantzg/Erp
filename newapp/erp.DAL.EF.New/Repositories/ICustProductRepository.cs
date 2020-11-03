using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public interface ICustProductRepository : IGenericRepository<Cust_products>
	{
		List<Cust_products> SearchProducts(int? factory_id, int? client_id, bool includeTemplates);
		List<auto_add_products> GetAutoAddedProducts(IList<int?> ids);
	}
}
