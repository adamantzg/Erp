using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionLinesRejectedDal : IGenericDal<Inspection_lines_rejected>
	{
		List<Inspection_lines_rejected> GetByInspection(int insp_id);
	}
}