using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace erp.Model.DAL
{
    public partial class PermissionDAL
    {
        public static List<Permission> GetForUser(int user_id, IDbConnection conn = null)
        {
            var result = new List<Permission>();
            var roles = RoleDAL.GetRolesForUser(user_id,conn);
            foreach (var role in roles)
            {
                result.AddRange(role.Permissions);
            }
            //Eliminate duplicates
            return result.Select(r => new Permission {id = r.id, name = r.name}).Distinct().ToList();
        }
    }
}
