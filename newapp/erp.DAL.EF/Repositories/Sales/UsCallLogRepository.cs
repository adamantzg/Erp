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
	public class UsCallLogRepository : GenericRepository<Us_call_log>
	{
		public UsCallLogRepository(DbContext context) : base(context)
		{
		}

		public override void Update(Us_call_log entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m => m.OwnedCollection(l => l.Images).AssociatedCollection(s => s.Categories));
		}

		public override void Insert(Us_call_log entity)
		{
			if(entity.Categories != null)
			{
				var ids = entity.Categories.Select(c => c.id).ToList();
				entity.Categories = context.Set<Us_call_log_category>().Where(c => ids.Contains(c.id)).ToList();				
			}
			base.Insert(entity);
		}
	}
}
