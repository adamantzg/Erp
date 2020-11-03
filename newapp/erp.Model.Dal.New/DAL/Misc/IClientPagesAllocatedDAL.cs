using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IClientPagesAllocatedDAL : IGenericDal<Client_pages_allocated>
	{
		List<Client_pages_allocated> GetByPageAndUser(int user_id, string page_Url = "");
	}
}