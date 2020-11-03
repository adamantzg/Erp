using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IPermissionDAL
	{
		List<Permission> GetForUser(int user_id);
	}
}