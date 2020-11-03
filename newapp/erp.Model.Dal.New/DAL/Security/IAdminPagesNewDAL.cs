using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IAdminPagesNewDAL
	{
		List<Admin_pages_new> GetForUser(int user_id);
		void GetParent(List<Admin_pages_new> list, int? parent_id);
	}
}