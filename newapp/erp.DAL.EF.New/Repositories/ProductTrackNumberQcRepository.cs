using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public class ProductTrackNumberQcRepository : GenericRepository<products_track_number_qc>, IProductTrackNumberQcRepository
	{
		public ProductTrackNumberQcRepository(DbContext context) : base(context)
		{
		}

		public void DeleteForInspection(int insp_id)
		{
			context.Database.ExecuteSqlCommand("DELETE FROM 2012_products_track_number_qc WHERE insp_id = @p0", insp_id);
		}
		
	}
}
