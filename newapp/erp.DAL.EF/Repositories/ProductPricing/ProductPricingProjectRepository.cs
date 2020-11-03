using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.Repositories
{
	public class ProductPricingProjectRepository : GenericRepository<ProductPricingProject>
	{
		public ProductPricingProjectRepository(DbContext context) : base(context)
		{
		}

		public override void Insert(ProductPricingProject p)
		{
			if(p.Products != null)
			{
				var ids = p.Products.Select(cp => cp.cprod_id);
				p.Products = context.Set<Cust_products>().Where(cp => ids.Contains(cp.cprod_id)).ToList();
			}
			base.Insert(p);
		}

		public override void Update(ProductPricingProject p)
		{
			context.UpdateGraph(p, m => m.AssociatedCollection(pp => pp.Products).OwnedCollection(pp => pp.ProjectSettings));
		}
	}
}
