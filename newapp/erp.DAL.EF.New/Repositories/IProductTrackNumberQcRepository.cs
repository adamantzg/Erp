using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public interface IProductTrackNumberQcRepository : IGenericRepository<products_track_number_qc>
	{
		void DeleteForInspection(int insp_id);
	}
}
