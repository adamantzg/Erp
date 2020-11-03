using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RefactorThis.GraphDiff;
using erp.DAL.EF.Repositories;
using erp.Model;

namespace erp.DAL.EF
{
    public class ManualV2Repository : GenericRepository<manual>
    {
        public ManualV2Repository(Model context) : base(context)
        {

        }

        public override void Insert(manual entity)
        {
            context.UpdateGraph(entity, map => map.AssociatedCollection(m => m.Nodes));
        }

        public override void Update(manual entityToUpdate)
        {
            context.UpdateGraph(entityToUpdate, map => map.OwnedCollection(m => m.Nodes));
        }
        
    }
}
