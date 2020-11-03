using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionLinesNotifiedDal : IGenericDal<inspection_lines_notified>
	{
		List<inspection_lines_notified> GetByInspection(int id);
	}
}