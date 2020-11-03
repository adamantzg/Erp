using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Repositories
{
    public class ProductTrackNumbersRepository : GenericRepository<products_track_number_qc>
    {
        public ProductTrackNumbersRepository(Model context) : base(context)
        {
        }

        public void DeleteForInspection(int insp_id)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM 2012_products_track_number_qc WHERE insp_id = @p0", insp_id);
        }
    }
}
