using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;


namespace erp.Model.Dal.New
{
    public class RoleDAL : GenericDal<Role>, IRoleDAL
    {
		public RoleDAL(IDbConnection conn) : base(conn)
		{
		}

		public List<Role> GetRolesForUser(int user_id)
        {
	        var result = new List<Role>();
	        conn.Query<Role, Permission, Role>(
		        @"SELECT role.id, role.`name`, permission.id ,permission.`name`
                            FROM role LEFT JOIN role_permission ON role.id = role_permission.role_id
                            INNER JOIN user_role ON role.id = user_role.role_id
                            LEFT JOIN permission ON permission.id = role_permission.permission_id
                            WHERE user_role.user_id = @user_id
                            ORDER BY role.id",
		        (r, p) =>
		        {
			        var role = result.FirstOrDefault(x => x.id == r.id);
			        if (role == null)
			        {
				        role = r;
						result.Add(role);
			        }
			        if (role.Permissions == null)
				        role.Permissions = new List<Permission>();
					if(p != null)
						role.Permissions.Add(p);
			        return role;
		        }, new {user_id});
	        return result;
        }

        public List<Role> GetRolesForUser(string username)
        {
	        var id = conn.ExecuteScalar<int?>("SELECT useruserid FROM userusers WHERE userusername = @username", new { username });
	        if (id != null)
		        return GetRolesForUser(id.Value);
	        return new List<Role>();
        }

        public List<User> GetUsersInRole(int role_id)
        {
	        return conn.Query<User>(@"SELECT userusers.* 
			FROM userusers INNER JOIN user_role ON userusers.useruserid = user_role.user_id 
			WHERE user_role.role_id = @role_id", new {role_id}).ToList();
        }

		protected override string GetAllSql()
		{
			return "SELECT * FROM role";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}
	}
}
