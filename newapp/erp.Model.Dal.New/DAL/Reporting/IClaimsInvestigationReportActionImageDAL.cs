using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IClaimsInvestigationReportActionImageDAL : IGenericDal<Claims_investigation_report_action_images>
	{
		List<Claims_investigation_report_action_images> GetImagesForAction(int id);
	}
}