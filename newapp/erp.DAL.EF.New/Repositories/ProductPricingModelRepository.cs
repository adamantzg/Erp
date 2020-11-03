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
	public class ProductPricingModelRepository : GenericRepository<ProductPricing_model>, IProductPricingModelRepository
	{
		public ProductPricingModelRepository(DbContext context) : base(context)
		{
		}

		public override void Update(ProductPricing_model entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m => m.OwnedCollection(model => model.Levels));
		}
	}
}
