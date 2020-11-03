using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IRoleDAL : IGenericDal<Role>
	{
		List<Role> GetRolesForUser(int user_id);
		List<Role> GetRolesForUser(string username);
		List<User> GetUsersInRole(int role_id);
	}
}