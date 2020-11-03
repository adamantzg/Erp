using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IEmailRecipientsDAL : IGenericDal<Email_recipients>
	{
		List<Email_recipients> GetByCriteria(int company_id,string area = null,object param1 = null, object param2 = null);
	}
}