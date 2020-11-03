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
	public class FactoryStockOrderRepository : GenericRepository<Factory_stock_order>
	{
		public FactoryStockOrderRepository(DbContext context) : base(context)
		{
		}

		public override void Update(Factory_stock_order entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m => m.OwnedCollection(o => o.Lines));
		}
	}
}
