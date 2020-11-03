using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IPorderLinesDal : IGenericDal<Porder_lines>
	{
		List<Porder_lines> GetLinesByCriteria(int? mast_id = null, DateTime? dateFrom = null);
	}
}
