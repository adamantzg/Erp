using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IInvoiceLinesDAL : IGenericDal<Invoice_lines>
	{
		List<Invoice_lines> GetByInvoice(int id);
	}
}