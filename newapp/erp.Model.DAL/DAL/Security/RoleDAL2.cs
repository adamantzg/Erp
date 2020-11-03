using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class RoleDAL
    {
        public static List<Role> GetRolesForUser(int user_id, IDbConnection conn = null)
        {
            var result = new List<Role>();
            var dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            
            {
                
                var cmd = Utils.GetCommand(@"SELECT role.id, role.`name`, permission.id AS permission_id, permission.`name` AS permission_name
                            FROM role LEFT JOIN role_permission ON role.id = role_permission.role_id
                            INNER JOIN user_role ON role.id = user_role.role_id
                            LEFT JOIN permission ON permission.id = role_permission.permission_id
                            WHERE user_role.user_id = @user_id
                            ORDER BY role.id", (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                var dr = cmd.ExecuteReader();
                Role r = null;
                while (dr.Read())
                {
                    var role = GetFromDataReader(dr);
                    if (r == null || role.id != r.id)
                    {
                        r = role;
                        r.Permissions = new Collection<Permission>();
                        result.Add(r);
                    }
                    var perm_id = Utilities.FromDbValue<int>(dr["permission_id"]);
                    if (perm_id != null)
                    {
                        r.Permissions.Add(new Permission
                            {   id= perm_id.Value,
                                name = string.Empty + dr["permission_name"]
                            });
                    }
                }
                dr.Close();
            }
            if(dispose)
                conn.Dispose();
            return result;
        }

        public static List<Role> GetRolesForUser(string username)
        {

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT useruserid FROM userusers WHERE userusername = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                var id = (int?) Utilities.FromDbValue<int>(cmd.ExecuteScalar());
                if (id != null)
                    return GetRolesForUser(id.Value,conn);
                return new List<Role>();
            }
        }

        

        public static List<User> GetUsersInRole(int role_id)
        {
            var result = new List<User>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    Utils.GetCommand(
                        "SELECT userusers.* FROM userusers INNER JOIN user_role ON userusers.useruserid = user_role.user_id WHERE user_role.role_id = @role_id",
                        conn);
                cmd.Parameters.AddWithValue("@role_id", role_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(UserDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
    }
}
