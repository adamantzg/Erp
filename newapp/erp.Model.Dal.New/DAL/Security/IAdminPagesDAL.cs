using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IAdminPagesDAL
	{
		List<Admin_pages> GetForUser(int user_id);
	}
}