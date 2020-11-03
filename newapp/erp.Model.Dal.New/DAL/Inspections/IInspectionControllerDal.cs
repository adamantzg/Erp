using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionControllerDal : IGenericDal<Inspection_controller>
	{
		List<Inspection_controller> GetByInspection(int inspection_id);
	}
}