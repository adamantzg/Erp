using System;
using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IInvoicesDAL : IGenericDal<Invoices>
	{
		void ActivateCreditNote(int invoice_id);
		int GetLastOrderId();
		List<Invoices> GetByCriteria(List<int> custids, DateTime? from, DateTime? to,bool? brands = null, bool filterbyStatus = true, bool excludeOrders = true, int? invoice_sequence = null);
		Invoices GetCreditNoteByCriteria(int client_id,int? brand_user, DateTime invoiceDate);

		void CreateReturnCreditNotes(DateTime? from, DateTime? to, IList<int> excludedClientIds = null, IList<int> includedClientsIds = null, DateTime? invoiceDate = null,
			Dictionary<int, int> companyMappings = null, IList<int> excludedClientsForEBInvoice = null, int? invoice_from = null, int? invoice_from_nonUK = null, IList<int> ukDistributors_Exceptions = null, IList<int> brands = null);

		Invoices GetByOrder(int? orderid, IDbConnection conn = null);
		List<Invoices> GetForDealer(int id);
	}
}