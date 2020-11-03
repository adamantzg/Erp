using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionsLoadingDal : IGenericDal<Inspections_loading>
	{
		List<Inspections_loading> GetForInspection(int insp_id);
		List<Inspections_loading> GetForInspection(IList<int> insp_ids);
	}
}