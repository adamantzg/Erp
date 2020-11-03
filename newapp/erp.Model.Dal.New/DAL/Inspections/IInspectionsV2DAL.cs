using System;
using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IInspectionsV2DAL
	{
		/// <summary>
		/// Optimized version. Fills only some lists and fields
		/// </summary>
		/// <param name="factory_ids"></param>
		/// <param name="client_ids"></param>
		/// <param name="custpo"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="statuses"></param>
		/// <param name="inspectorId"></param>
		/// <returns></returns>
		List<Inspection_v2> GetByCriteria(IList<int> factory_ids, IList<int> client_ids, string custpo,
			DateTime? from,
			DateTime? to, IList<InspectionV2Status?> statuses = null, int? inspectorId = null);

		Inspection_v2 GetById(int id, bool loadLoadings = false,bool loadImages = false);
		List<Inspection_v2_line> LoadLines(int? id= null, IList<int> orderids = null, bool loadImages = false, bool loadLoadings = false, Inspection_v2 insp = null);
		List<Inspection_v2> GetOrderInspections(int orderid, bool loadLoadings = false);
	}
}