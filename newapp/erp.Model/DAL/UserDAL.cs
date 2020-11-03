using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public enum UserRole
    {
        Distributor,
        MasterDistributor,  //45,55 CW
        HeadDistributor,     //42 , can create distributors
        Manufacturer,
        ExternalUser,
        Administrator,
        Inspector,
        EBManagement,
        Empty
    }

    public class UserDAL
    {
        public static User GetUser(string login, string password)
        {
            User result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT userusers.*, users.* FROM userusers INNER JOIN users ON userusers.user_id = users.user_id  WHERE userusername = @login AND userpassword = @password", conn);
                cmd.Parameters.Add(new MySqlParameter("@login", login));
                cmd.Parameters.Add(new MySqlParameter("@password", password));
                MySqlDataReader dr = cmd.ExecuteReader();
                if(dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Company = CompanyDAL.GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static User GetByLogin(string login)
        {
            User result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT userusers.*, users.* FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusername = @login", conn);
                cmd.Parameters.Add(new MySqlParameter("@login", login));
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Company = CompanyDAL.GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static User GetById(int user_id)
        {
            User result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT userusers.*, users.*  FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE useruserid = @user_id", conn);
                cmd.Parameters.Add(new MySqlParameter("@user_id", user_id));
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Company = CompanyDAL.GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static List<User> GetByCompany(int company_id)
        {
            List<User> result = new List<User>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM userusers WHERE user_id = @company_id", conn);
                cmd.Parameters.Add(new MySqlParameter("@company_id", company_id));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static bool IsUserInRole(string username, UserRole roleName)
        {
            bool result = false;
            string sql = "";
            if (roleName == UserRole.Administrator)
            {
                sql = "SELECT COUNT(*) FROM userusers WHERE userusername = @username AND admin_type > 0 && admin_type <> 5";
            }
            else if (roleName == UserRole.Inspector)
            {
                sql = "SELECT COUNT(*) FROM userusers WHERE userusername = @username AND admin_type = 5";
            }
            else if (roleName == UserRole.Distributor)
            {
                sql =
                    "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.distributor > 0";
            }
            else if (roleName == UserRole.MasterDistributor)
            {
                sql =
                    string.Format(
                        "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_id IN ({0})",
                        Properties.Settings.Default.MasterDistributors);
            }
            else if (roleName == UserRole.HeadDistributor)
            {
                sql =
                    string.Format(
                        "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_id IN ({0})",
                        Properties.Settings.Default.HeadDistributors);
            }
            else if (roleName == UserRole.ExternalUser)
            {
                sql = "SELECT COUNT(*) FROM external_user WHERE username = @username";
            }
            else if (roleName == UserRole.Manufacturer)
            {
                sql =
                    @"SELECT COUNT(*) FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                     INNER JOIN admin_permissions ON mast_products.factory_id = admin_permissions.cusid
                                                     INNER JOIN userusers ON userusers.useruserid = admin_permissions.userid
                                                     WHERE admin_permissions.`returns` = 1 AND userusers.userusername = @username";
            }
            else if (roleName == UserRole.EBManagement)
            {
                sql =
                    "SELECT COUNT(*) FROM userusers WHERE user_id = 1 AND userusername = @username AND admin_type = 10";
            }

            if (!string.IsNullOrEmpty(sql))
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    result = count > 0;
                }
            }
            return result;
        }

        public static bool IsUserInRole(string username, string roleName)
        {
            return IsUserInRole(username, (UserRole)Enum.Parse(typeof(UserRole), roleName));                        
        }

        public static UserRole[] GetUserRoles(string username)
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
            if(IsUserInRole(username, UserRole.Administrator))
                roles.Add(UserRole.Administrator);
            if (IsUserInRole(username, UserRole.Inspector))
                roles.Add(UserRole.Inspector);
            if (IsUserInRole(username, UserRole.EBManagement))
                roles.Add(UserRole.EBManagement);
            return roles.ToArray();
        }

        public static string[] GetRolesForUser(string username)
        {
            return GetUserRoles(username).Select(r => r.ToString()).ToArray();
        }

        

        public static List<User> GetUsersForArea(int area_id)
        {
            List<User> result = new List<User>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT userusers.* FROM issuearea_users INNER JOIN userusers ON issuearea_users.user_id = userusers.useruserid WHERE issuearea_users.area_id = @area_id", conn);
                cmd.Parameters.AddWithValue("@area_id", area_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                //Check for company
                cmd.CommandText = "SELECT userusers.* FROM issuearea_users INNER JOIN userusers ON issuearea_users.company_id = userusers.user_id WHERE issuearea_users.area_id = @area_id";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<User> GetUsersByCriteria(string text)
        {
            List<User> result = new List<User>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd =
                    new MySqlCommand(
                        "SELECT userusers.* FROM userusers WHERE userusername LIKE @text OR userwelcome LIKE @text",
                        conn);
                cmd.Parameters.AddWithValue("@text", "%" + text + "%");
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<User> GetInspectors(int? location_id)
        {
            List<User> result = new List<User>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM userusers WHERE admin_type = 5 and (consolidated_port = @location_id OR @location_id IS NULL) AND status_flag <> 1 ", conn);
                cmd.Parameters.Add(new MySqlParameter("@location_id", location_id != null ? (object) location_id : DBNull.Value));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }



        public static User GetFromDataReader(MySqlDataReader dr)
        {
            User o = new User();

            o.userid = (int) dr["useruserid"];
            o.username = string.Empty + dr["userusername"];
            o.userpassword = string.Empty + dr["userpassword"];
            o.userwelcome = string.Empty + dr["userwelcome"];
            o.company_id = (int) dr["user_id"];
            o.user_level = Utilities.FromDbValue<int>(dr["user_level"]);
            o.session = Utilities.FromDbValue<int>(dr["session"]);
            o.user_email = string.Empty + dr["user_email"];
            o.admin_type = Utilities.FromDbValue<int>(dr["admin_type"]);
            o.consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]);
            o.inspection_plan_admin = Utilities.FromDbValue<int>(dr["inspection_plan_admin"]);
            o.restrict_ip = Utilities.FromDbValue<int>(dr["restrict_ip"]);
            o.ip_address = string.Empty + dr["ip_address"];
            o.ip_address1b = string.Empty + dr["ip_address1b"];
            o.ip_address1c = string.Empty + dr["ip_address1c"];
            o.ip_address2 = string.Empty + dr["ip_address2"];
            o.mobilea = string.Empty + dr["mobilea"];
            o.mobileb = string.Empty + dr["mobileb"];
            o.email_pwd = string.Empty + dr["email_pwd"];
            o.skype = string.Empty + dr["skype"];
            o.manager = Utilities.FromDbValue<int>(dr["manager"]);
            o.user_initials = string.Empty + dr["user_initials"];
            o.status_flag = Utilities.FromDbValue<int>(dr["status_flag"]);
            o.restricted = Utilities.FromDbValue<int>(dr["restricted"]);
            o.qc_technical = Utilities.FromDbValue<int>(dr["qc_technical"]);
            o.after_sales = Utilities.BoolFromLong(Utilities.FromDbValue<long>(dr["after_sales"]));
            return o;

        }

        public static void Create(User o)
        {
            string insertsql = @"INSERT INTO userusers (userusername,userpassword,userwelcome,user_id,user_level,session,user_email,admin_type,consolidated_port,inspection_plan_admin,restrict_ip,
                                ip_address,ip_address1b,ip_address1c,ip_address2,mobilea,mobileb,email_pwd,skype,manager,user_initials,status_flag,restricted,qc_technical,after_sales) 
                                VALUES(@userusername,@userpassword,@userwelcome,@user_id,@user_level,@session,@user_email,@admin_type,@consolidated_port,@inspection_plan_admin,
                                @restrict_ip,@ip_address,@ip_address1b,@ip_address1c,@ip_address2,@mobilea,@mobileb,@email_pwd,@skype,@manager,@user_initials,@status_flag,@restricted,@qc_technical)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT useruserid FROM userusers WHERE useruserid = LAST_INSERT_ID()";
                o.userid = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, User o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@useruserid", o.userid);
            cmd.Parameters.AddWithValue("@userusername", o.username);
            cmd.Parameters.AddWithValue("@userpassword", o.userpassword);
            cmd.Parameters.AddWithValue("@userwelcome", o.userwelcome);
            cmd.Parameters.AddWithValue("@user_id", o.company_id);
            cmd.Parameters.AddWithValue("@user_level", o.user_level);
            cmd.Parameters.AddWithValue("@session", o.session);
            cmd.Parameters.AddWithValue("@user_email", o.user_email);
            cmd.Parameters.AddWithValue("@admin_type", o.admin_type);
            cmd.Parameters.AddWithValue("@consolidated_port", o.consolidated_port);
            cmd.Parameters.AddWithValue("@inspection_plan_admin", o.inspection_plan_admin);
            cmd.Parameters.AddWithValue("@restrict_ip", o.restrict_ip);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
            cmd.Parameters.AddWithValue("@ip_address1b", o.ip_address1b);
            cmd.Parameters.AddWithValue("@ip_address1c", o.ip_address1c);
            cmd.Parameters.AddWithValue("@ip_address2", o.ip_address2);
            cmd.Parameters.AddWithValue("@mobilea", o.mobilea);
            cmd.Parameters.AddWithValue("@mobileb", o.mobileb);
            cmd.Parameters.AddWithValue("@email_pwd", o.email_pwd);
            cmd.Parameters.AddWithValue("@skype", o.skype);
            cmd.Parameters.AddWithValue("@manager", o.manager);
            cmd.Parameters.AddWithValue("@user_initials", o.user_initials);
            cmd.Parameters.AddWithValue("@status_flag", o.status_flag);
            cmd.Parameters.AddWithValue("@restricted", o.restricted);
            cmd.Parameters.AddWithValue("@qc_technical", o.qc_technical);
            cmd.Parameters.AddWithValue("@after_sales", o.after_sales);
        }

        public static void Update(User o)
        {
            string updatesql = @"UPDATE userusers SET userusername = @userusername,userpassword = @userpassword,userwelcome = @userwelcome,user_id = @user_id,user_level = @user_level,
                            session = @session,user_email = @user_email,admin_type = @admin_type,consolidated_port = @consolidated_port,inspection_plan_admin = @inspection_plan_admin,
                            restrict_ip = @restrict_ip,ip_address = @ip_address,ip_address1b = @ip_address1b,ip_address1c = @ip_address1c,ip_address2 = @ip_address2,mobilea = @mobilea,
                            mobileb = @mobileb,email_pwd = @email_pwd,skype = @skype,manager = @manager,user_initials = @user_initials,status_flag = @status_flag,restricted = @restricted,
                            qc_technical = @qc_technical, after_sales = @after_sales
                            WHERE useruserid = @useruserid";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int useruserid)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM userusers WHERE useruserid = @id", conn);
                cmd.Parameters.AddWithValue("@id", useruserid);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class Location
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}
