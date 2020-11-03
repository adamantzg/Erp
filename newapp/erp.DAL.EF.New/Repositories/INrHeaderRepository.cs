using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public interface INrHeaderRepository : IGenericRepository<Nr_header>
	{
		List<NrReportInspectionRow> GetInspectionsForNtReport_NoNrHeaders(DateTime change_notice_from, DateTime etdReadyDate);
		List<NrReportInspectionRow> GetInspectionsForNtReport_NrHeaders(DateTime change_notice_from, DateTime etdReadyDate);
	}
}
