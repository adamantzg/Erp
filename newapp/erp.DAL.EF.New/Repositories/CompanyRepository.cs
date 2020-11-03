using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.New
{
	public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
	{
		public CompanyRepository(DbContext context) : base(context)
		{

		}

		public List<Company> GetFactories(bool combined = false, int? location_id = null, bool? files = null)
		{
			var set = context.Set<Company>();
			var result = Get(c => c.user_type == (int) Company_User_Type.Factory 
				&& (c.consolidated_port == location_id || location_id == null), 
				includeProperties: (files == true ? "Files" : "")).ToList();
            if(combined)
                result.AddRange(Get(c => c.user_type == (int) Company_User_Type.Factory && 
					c.combined_factory > 0 && (c.consolidated_port == location_id || location_id == null))
					.GroupBy(c => c.combined_factory).ToList()
                    .Select(g=>new Company{user_id = -1*g.Key.Value,factory_code = GetCombinedFactoryCode(g.First().factory_code)}));

            return result;
		}

		private string GetCombinedFactoryCode(string factoryCode)
        {
            if (string.IsNullOrEmpty(factoryCode))
                return string.Empty;
            return factoryCode.Substring(0, factoryCode.Length > 1 ? factoryCode.Length - 1 : 1);
        }

		public List<Company> GetClientsWithOrders(bool combined = false)
		{
			var result = Get(c => c.user_type == (int)Company_User_Type.Client && c.Orders.Any()).ToList();
			if (combined)
				result.AddRange(GetQuery(c=> c.user_type == (int)Company_User_Type.Client && c.Orders.Any() && c.combined_factory > 0).GroupBy(c => c.combined_factory).ToList()
					.Select(g => new Company { user_id = -1 * g.Key.Value, customer_code = string.Join("/", g.Select(c => c.customer_code)) }));

            return result;
		}

		public override void Update(Company entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m=>m.AssociatedCollection(e=>e.Files));
		}
	}
}
