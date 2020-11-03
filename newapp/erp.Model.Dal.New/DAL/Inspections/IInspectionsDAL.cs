using System;
using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IInspectionsDAL : IGenericDal<Inspections>
	{
		Inspections GetByNewInspId(int newid);
		Inspections GetByLoadingId(int loadingId);
		List<Inspections> GetFinalInspectionsForNS(DateTime? from);
		void Update(Inspections o, List<Inspection_controller> deletedControllers, IDbTransaction tr = null);
		List<Inspections> GetByCriteria(DateTime dateFrom, DateTime dateTo, int? locationId);

		/// <summary>
		/// Possibly restricts inspections if user is not admin
		/// </summary>
		/// <param name="dateFrom"></param>
		/// <param name="dateTo"></param>
		/// <param name="user_id"></param>
		/// <returns></returns>
		List<Inspections> GetForExport(DateTime dateFrom, DateTime dateTo,string[] factorycodes, int? user_id = null);

		List<Inspections> GetLoadingInspections(string[] factory_codes, string[] customer_codes,string[] custpos);
		int GetChangedProductCount(string[] custpo,string factory_code);
		Inspections GetInspection(string custpo, string factory_code, string customer_code);
		List<Inspections> GetForCustPo(string custpo);
		List<Inspections> GetForFeedback(Returns r, bool pendingOnly = true);
		List<Inspections> GetForQcs(DateTime? from, DateTime? to, IList<int?> qc_ids);
		int GetIdFromIdString(string sId, bool isV2 = false);
		List<Inspections> GetByCriteria(DateTime? dateFrom = null, DateTime? dateTo = null, int? factoryId = null, int? clientId = null, 
			string custpo = null,string productCode = null, int? statusId = null, int? userId = null);
		List<Inspections> GetRelatedLoadingInspections(IList<int> ids);
		List<nr_line_legacy> GetNRLegacyLines(IList<string> custpos);
	}
}