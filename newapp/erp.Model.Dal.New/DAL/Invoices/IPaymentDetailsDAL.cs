using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IPaymentDetailsDAL
	{
		List<Payment_details> GetForCompany(int company_id);
	}
}