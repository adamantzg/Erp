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
	public class CustProductRepository : GenericRepository<Cust_products>, ICustProductRepository
	{
		public CustProductRepository(DbContext context) : base(context)
		{
		}

		public List<auto_add_products> GetAutoAddedProducts(IList<int?> ids)
		{
			return context.Set<auto_add_products>().Include("AddedProduct.MastProduct.Factory").Where(x=>ids.Contains(x.trigger_cprod_id)).ToList();
		}

		public override void Update(Cust_products entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m => m.OwnedEntity(x => x.MastProduct));
		}

		public List<Cust_products> SearchProducts(int? factory_id, int? client_id, bool includeTemplates)
		{
			return includeTemplates ? dbSet.Include("MastProduct").Include("Inspv2Templates")
					.Where(
						p =>
							(p.cprod_user == client_id || client_id == null) &&
							(p.MastProduct.factory_id == factory_id || factory_id == null))
					.ToList() :
				dbSet.Include("MastProduct")
					.Where(
						p =>
							(p.cprod_user == client_id || client_id == null) &&
							(p.MastProduct.factory_id == factory_id || factory_id == null))
					.ToList();
		}


	}
}
