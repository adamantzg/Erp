using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionLinesAcceptedDal : IGenericDal<Inspection_lines_accepted>
	{
		List<Inspection_lines_accepted> GetByInspection(int insp_id);
	}
}