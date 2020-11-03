using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionImagesDAL : IGenericDal<Inspection_images>
	{
		List<Inspection_images> GetByInspection(int insp_id);
		void DeleteMissingForLine(int line_id, string insp_type, IList<int> existingIds);
	}
}