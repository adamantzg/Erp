using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInspectionLinesTestedDal
	{
		List<Inspection_lines_tested> GetByInspection(int insp_id, bool includeProducts = false, bool includeLoadings = false);
		List<Inspection_lines_tested> GetLinesForOrders(IList<int> orderIds, int? excludedInspId = null, bool includeLoadings = false);
		List<Inspection_lines_tested> GetInspLines(int insp_id);

		/// <summary>
		/// prvi parametar za filtriranje po inspection
		/// druga dva dohvati sve pa filtriraj po factories  po distributeru
		/// </summary>
		/// <param name="insp_id"></param>
		/// <param name="factory_id"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		List<Inspections_documents> GetByInspId(int insp_id=0, string factory_code="", string customer_code="");

		List<Inspections_documents> GetByFactoryRef(int insp_id);
		List<Inspections_documents> GetByProducts(int factory_id, int customer_id);
	}
}