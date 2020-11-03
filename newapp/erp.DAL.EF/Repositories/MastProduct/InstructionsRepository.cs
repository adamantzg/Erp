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
    public class InstructionsRepository : GenericRepository<instructions_new>
    {
        public InstructionsRepository(DbContext context) : base(context)
        {
        }

        public override void Insert(instructions_new entity)
        {
            if(entity.Products != null)
                foreach(var p in entity.Products)
                {
                    var mast = context.Set<Mast_products>().Local.FirstOrDefault(mp => mp.mast_id == p.mast_id);
                    if (mast == null)
                        context.Set<Mast_products>().Attach(p);
                }
            base.Insert(entity);
        }

        public override void Update(instructions_new entityToUpdate)
        {
            context.UpdateGraph(entityToUpdate, m => m.AssociatedCollection(i => i.Products));
        }
    }
}
