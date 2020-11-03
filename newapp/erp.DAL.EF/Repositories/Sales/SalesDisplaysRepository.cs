using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories
{
    public class SalesDisplaysRepository : GenericRepository<us_display_orders>
    {
        public SalesDisplaysRepository(Model context) : base(context)
        {
            

        }

        public void DeleteAll()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM us_display_orders");
        }
    }
}
