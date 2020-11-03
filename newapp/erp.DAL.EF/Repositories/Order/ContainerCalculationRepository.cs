using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.Repositories
{
    public class ContainerCalculationRepository : GenericRepository<Containercalculation_order>
    {
        public ContainerCalculationRepository(DbContext context) : base(context)
        {
        }

        public override void Update(Containercalculation_order entityToUpdate)
        {
            context.UpdateGraph(entityToUpdate, m => m.OwnedCollection(c => c.Products));
        }
    }
}
