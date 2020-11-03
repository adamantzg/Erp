using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.Repositories
{
    public class SparesRepository : GenericRepository<Spare>
    {
        public SparesRepository(Model context) : base(context)
        {

        }



        public void HandleRemoval(List<Spare> removed)
        {
            foreach (var r in removed) {
                var existing = context.Set<Spare>().FirstOrDefault(s => s.product_cprod == r.product_cprod && s.spare_cprod == r.spare_cprod);
                if (existing != null)
                    context.Set<Spare>().Remove(existing);
            }
        }
    }
}
