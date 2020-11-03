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
	public class MastProductRepository : GenericRepository<Mast_products>, IMastProductRepository
	{
		public MastProductRepository(DbContext context) : base(context)
		{
		}

		public override void Update(Mast_products entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m=>m.OwnedCollection(mp=>mp.PackagingMaterials).AssociatedCollection(mp=>mp.OtherFiles));
		}
	}
}
