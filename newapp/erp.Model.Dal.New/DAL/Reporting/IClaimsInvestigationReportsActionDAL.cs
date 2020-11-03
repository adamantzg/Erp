using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IClaimsInvestigationReportsActionDAL : IGenericDal<Claims_investigation_reports_action>
	{
		List<Claims_investigation_reports_action> GetActionsForReports(int unique_id, bool images = false);
		Claims_investigation_reports_action GetLastAddedAction();
	}
}