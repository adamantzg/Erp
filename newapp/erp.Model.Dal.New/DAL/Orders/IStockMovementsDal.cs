using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IStockMovementsDal : IGenericDal<stock_movements>
	{
		List<stock_movements> GetByCriteria(int? mast_id = null, DateTime? dateFrom = null);
	}
}
