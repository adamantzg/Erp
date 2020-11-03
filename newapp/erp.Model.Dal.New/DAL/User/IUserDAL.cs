using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IUserDAL
	{
		User GetUser(string login, string password);
		List<User> GetAll();
		User GetByLogin(string login);
		User GetById(int user_id);
		List<User> GetByCompany(int company_id);
		List<User> GetByCompanyCodes(IList<string> companyCodes );
		bool IsUserInRoles(string username, IList<UserRole> roleNames);
		bool IsUserInRole(string username, UserRole roleName);
		bool IsUserInRole(string username, string roleName);
		UserRole[] GetDynamicUserRoles(string username);
		string[] GetRolesForUser(string username);
		List<User> GetUsersForArea(int area_id);
		List<User> GetUsersByCriteria(string text);
		List<User> GetInspectors(int? location_id, bool includeAdminPermissions = false);
		void Create(User o);
		void Update(User o);
		void Delete(int useruserid);
	}
}