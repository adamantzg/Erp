using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using erp.DAL.EF.Repositories;
using System.Data.Entity;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace erp.DAL.EF
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(Model context) : base(context)
        {
        }

        public static List<Admin_permissions> GetPermissionsByClient(int company_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.AdminPermissions.Include("User").Where(a => a.cusid == company_id).ToList();
            }
        }

        public static List<Admin_permissions> GetUserCompanies(int company_id)
        {
            using (var m = Model.CreateModel()) {
                return m.AdminPermissions.Include("User").Where(a => a.cusid == company_id).ToList();
            }
        }

        public static void AddUserToRole(int user_id, int role_id)
        {
            using (var m = Model.CreateModel()) {
                var user = m.Users.FirstOrDefault(u => u.userid == user_id);
                var role = m.Roles.FirstOrDefault(r => r.id == role_id);
                if (user != null && role != null) {
                    if (user.Roles == null)
                        user.Roles = new List<Role>();
                    user.Roles.Add(role);
                    m.SaveChanges();
                }                    
            }
        }

        public static void RemoveUserFromRole(int user_id, int role_id)
        {
            using (var m = Model.CreateModel()) {
                m.Database.ExecuteSqlCommand("DELETE FROM user_role WHERE user_id = @p0 AND role_id = @p1", user_id, role_id);
            }
        }

        public List<User> GetUsersInRole(UserRole role)
        {
            if (role == UserRole.Inspector)
                return Get(u => u.admin_type == 5).ToList();
            if (role == UserRole.ClientController)
                return Get(u => u.AdminPermissions.Any(a => a.Company.user_type == (int)Company_User_Type.Client), includeProperties: "AdminPermissions.Company").ToList();
            if (role == UserRole.FactoryController)
                return Get(u => u.AdminPermissions.Any(a => a.Company.user_type == (int)Company_User_Type.Factory && a.processing == 1), includeProperties: "AdminPermissions.Company").ToList();
            if (role == UserRole.QualityAssurance)
                return Get(u => u.AdminPermissions.Any(a => a.Company.user_type == (int)Company_User_Type.Factory && a.processing == 0), includeProperties: "AdminPermissions.Company").ToList();
            return null;
        }

        public void GetRoles(User user)
        {
            context.Entry(user).Collection(u => u.Roles).Query().Include(r=>r.Permissions).Load();
        }

        public void GetPermissions(User user)
        {
            if((user.Permissions == null || user.Permissions.Count == 0) &&  user.Roles != null) {
                if (user.Permissions == null)
                    user.Permissions = new List<Permission>();
                foreach(var r in user.Roles) {
                    if (r.Permissions != null)
                        user.Permissions.AddRange(r.Permissions);
                }
            }
        }

        public UserRole[] GetDynamicUserRoles(string username)
        {
            List<UserRole> roles = new List<UserRole>();
            if (IsUserInRole(username, UserRole.Distributor))
                roles.Add(UserRole.Distributor);
            if (IsUserInRole(username, UserRole.MasterDistributor))
                roles.Add(UserRole.MasterDistributor);
            if (IsUserInRole(username, UserRole.HeadDistributor))
                roles.Add(UserRole.HeadDistributor);
            if (IsUserInRole(username, UserRole.ExternalUser))
                roles.Add(UserRole.ExternalUser);
            if (IsUserInRole(username, UserRole.Manufacturer))
                roles.Add(UserRole.Manufacturer);
            if (IsUserInRole(username, UserRole.Administrator))
                roles.Add(UserRole.Administrator);
            if (IsUserInRole(username, UserRole.Inspector))
                roles.Add(UserRole.Inspector);
            if (IsUserInRole(username, UserRole.EBManagement))
                roles.Add(UserRole.EBManagement);
            return roles.ToArray();
            
        }

        public string[] GetRolesForUser(string username)
        {
            var roles = GetDynamicUserRoles(username).Select(r => r.ToString()).ToList();
            var user = Get(u => u.username == username, includeProperties: "Roles").FirstOrDefault();
            if(user != null) {
                roles.AddRange(user.Roles.Where(dr => roles.Count(r => r == dr.name) == 0).Select(dr => dr.name));
            }                        
            return roles.ToArray();

            //var roles = new List<string>(); //GetDynamicUserRoles(username).Select(r => r.ToString()).ToList();
            //var dbRoles = RoleDAL.GetRolesForUser(username);
            ////roles.AddRange(dbRoles.Where(dr => roles.Count(r => r == dr.name) == 0).Select(dr=>dr.name));
            ////roles = dbRoles.Select(dr=>dr.name).ToList()
            ////return roles.ToArray();
            //return dbRoles.Select(dr => dr.name).ToArray();
        }

        public bool IsUserInRole(string username, string roleName)
        {
            return IsUserInRole(username, (UserRole)Enum.Parse(typeof(UserRole), roleName));
        }

        public bool IsUserInRole(string username, UserRole roleName)
        {
            return IsUserInRoles(username, new[] { roleName });
        }

        public bool IsUserInRoles(string username, IList<UserRole> roleNames)
        {
            bool result = false;
            string sql = string.Empty;
            
            foreach (var roleName in roleNames) {
                if (roleName == UserRole.Administrator) {
                    sql = "SELECT COUNT(*) FROM userusers WHERE userusername = @username AND admin_type > 0 && admin_type <> 5";
                }
                else if (roleName == UserRole.Inspector) {
                    sql = "SELECT COUNT(*) FROM userusers WHERE userusername = @username AND admin_type = 5";
                }
                else if (roleName == UserRole.Distributor) {
                    sql =
                        "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.distributor > 0";
                }
                else if (roleName == UserRole.MasterDistributor) {
                    sql =
                        string.Format(
                            "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_id IN ({0})",
                            Properties.Settings.Default.MasterDistributors);
                }
                else if (roleName == UserRole.HeadDistributor) {
                    sql =
                        string.Format(
                            "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_id IN ({0})",
                            Properties.Settings.Default.HeadDistributors);
                }
                else if (roleName == UserRole.ExternalUser) {
                    sql = "SELECT COUNT(*) FROM external_user WHERE username = @username";
                }
                else if (roleName == UserRole.Manufacturer) {
                    sql =
                        @"SELECT COUNT(*) FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                    INNER JOIN admin_permissions ON mast_products.factory_id = admin_permissions.cusid
                                                    INNER JOIN userusers ON userusers.useruserid = admin_permissions.userid
                                                    WHERE admin_permissions.`returns` = 1 AND userusers.userusername = @username";
                }
                else if (roleName == UserRole.EBManagement) {
                    sql =
                        "SELECT COUNT(*) FROM userusers WHERE user_id = 1 AND userusername = @username AND admin_type = 10";
                }
                else if (roleName == UserRole.AccountUser) {
                    sql =
                        "SELECT COUNT(*) FROM userusers WHERE user_id = 1 AND userusername = @username AND admin_type = 3";
                }
                else if (roleName == UserRole.FactoryController || roleName == UserRole.ClientController || roleName == UserRole.QualityAssurance) {
                    sql = $@"SELECT COUNT(*) FROM users
                    INNER JOIN admin_permissions ON users.user_id = admin_permissions.cusid 
                    INNER JOIN userusers ON admin_permissions.userid = userusers.useruserid
                    WHERE userusers.userusername = @username AND  users.user_type = {(roleName == UserRole.FactoryController ? "1" : "3")} 
                        {(roleName == UserRole.ClientController ? "" : $" AND admin_permissions.processing = {(roleName == UserRole.FactoryController ? "1" : "0")}")}";
                }
                if (!string.IsNullOrEmpty(sql)) {
                    
                    int count = context.Database.SqlQuery<int>(sql, new MySqlParameter("@username", username)).ToList()[0];
                    result = count > 0;
                    if (result)
                        break;
                }
            }
            

            return result;
        }
    }
}
