using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IClaimsInvestigationReportsDAL : IGenericDal<Claims_investigation_reports>
	{
		List<Claims_investigation_reports> GetForProduct(int cprod_id, bool reports = false);
		Claims_investigation_reports GetLastAddedReport();
	}
}