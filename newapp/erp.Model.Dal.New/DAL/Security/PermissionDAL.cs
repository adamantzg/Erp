using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace erp.Model.Dal.New
{
    public class PermissionDAL : IPermissionDAL
    {
	    private MySqlConnection conn;
	    private IRoleDAL roleDal;

	    public PermissionDAL(IDbConnection conn, IRoleDAL roleDal)
	    {
		    this.conn = (MySqlConnection) conn;
		    this.roleDal = roleDal;
	    }

        public List<Permission> GetForUser(int user_id)
        {
            
            var roles = roleDal.GetRolesForUser(user_id);
	        var result = roles.SelectMany(r => r.Permissions);
            //Eliminate duplicates
            return result.Select(r => new Permission {id = r.id, name = r.name}).Distinct().ToList();
        }
    }
}
