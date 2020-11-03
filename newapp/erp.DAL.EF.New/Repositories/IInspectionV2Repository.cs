using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public interface IInspectionV2Repository : IGenericRepository<Inspection_v2>
	{
		void UpdateLoading(Inspection_v2 insp);
		void UpdateCombinedLoadings(List<Inspection_v2_loading> loadings);
		void UpdateFinal(Inspection_v2 insp);
		Inspection_v2 GetSiById(int id, bool loadAllSubobjects);
		List<KpiReportInspectionRow> GetInspectionsForKpi(int qc_id, int? month21);
	}
}
