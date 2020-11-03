using erp.Model;
using System.Collections.Generic;

namespace erp.DAL.EF.New
{
	public interface IInvoiceRepository : IGenericRepository<Invoices>
	{
		Invoice_export_settings GetExportSettingsByClient(int client_id);
		void UpdateExportSettings(Invoice_export_settings i);
		void CreateExportSettings(Invoice_export_settings i);
		List<Shipments> GetShipmentsForOrderIds(IList<int?> orderids);
		void Insert(Invoices inv, bool createEBinvoice = true, int? invoice_sequence_type = null);
	}
}