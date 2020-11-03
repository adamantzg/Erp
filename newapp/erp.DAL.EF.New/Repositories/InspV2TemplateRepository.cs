using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.New
{
	public class InspV2TemplateRepository : GenericRepository<Inspv2_template>, IInspV2TemplateRepository
	{
		public InspV2TemplateRepository(DbContext context) : base(context)
		{
		}

		public override void Update(Inspv2_template entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m => m.OwnedCollection(e=>e.Criteria).AssociatedCollection(e=>e.Products));
		}

		public override void Insert(Inspv2_template entity)
		{
			if (entity.Products != null)
			{
				foreach (var p in entity.Products)
				{
					var prod = context.Set<Cust_products>().Local.FirstOrDefault(x => x.cprod_id == p.cprod_id);
					if (prod == null)
						context.Set<Cust_products>().Attach(p);
				}
			}
			base.Insert(entity);
		}
	}
}
