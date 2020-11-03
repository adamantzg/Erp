using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class LoginhistoryDAL
    {
        public static Login_history GetByCriteria(string session_id, DateTime? login_date)
        {
            Login_history result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM login_history WHERE (session_id = @session_id OR @session_id IS NULL) AND ( DATE_ADD(@login_date, INTERVAL '24' HOUR) > NOW() OR @login_date IS NULL )", conn);
                cmd.Parameters.AddWithValue("@session_id", string.IsNullOrEmpty(session_id) ? (object)DBNull.Value : session_id);
                cmd.Parameters.AddWithValue("@login_date", login_date == null ? (object)DBNull.Value : login_date.Value);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result; 
        }

        public static List<Login_history> GetByCriteria(IList<int> company_ids, DateTime? dateFrom, DateTime? dateTo, IList<int> adminTypesToInclude = null, IList<int> adminTypesToExclude = null, 
            bool showAllActiveUsers = false,IList<User> activeUsers = null, string companiesAdminTypesMappings = "", string excludedCountries = null)
        {
            var result = new List<Login_history>();

            if (activeUsers == null || (activeUsers != null && activeUsers.Count == 0))
            {
                activeUsers = UserDAL.GetAll().Where(u => u.status_flag == 0).ToList();
            }

            var dictCompaniesAdminTypes = new Dictionary<int?, int>();
            if (!string.IsNullOrEmpty(companiesAdminTypesMappings))
            {
                var pairs = companiesAdminTypesMappings.Split(';');
                foreach(var pair in pairs)
                {
                    var parts = pair.Split('|');
                    dictCompaniesAdminTypes[Convert.ToInt32(parts[0])] = Convert.ToInt32(parts[1]);
                }
            }

            var exCountriesList = excludedCountries != null ? company.Common.Utilities.GetQuotedStringsFromString(excludedCountries) : null;
            
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                var criteria = new List<string>();
                cmd.CommandText = $@"SELECT * FROM login_history INNER JOIN users ON login_history.user_id = users.user_id";
                if (company_ids != null && company_ids.Count > 0)
                    criteria.Add($"login_history.user_id IN ({Utils.CreateParametersFromIdList(cmd, company_ids)})");
                if (exCountriesList != null)
                    criteria.Add($"login_history.login_country NOT IN ({string.Join(",",exCountriesList)})");
                criteria.Add("(login_date >= @dateFrom OR @dateFrom IS NULL) AND (login_date <= @dateTo OR @dateTo IS NULL)");
                cmd.CommandText += " WHERE " + string.Join(" AND ", criteria);
                cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object)dateFrom.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo.Value : DBNull.Value);


                if (showAllActiveUsers)
                    cmd.CommandTimeout = 2147483;

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    var r = GetFromDataReader(dr);
                    r.User = activeUsers.FirstOrDefault(u => u.username == r.login_username);
                    r.Company = CompanyDAL.GetFromDataReader(dr);
                    //if(dr["useruserid"] != DBNull.Value)
                    //    r.User = UserDAL.GetFromDataReader(dr);
                    var add = true;
                    if(dictCompaniesAdminTypes.Count > 0)
                        add = !dictCompaniesAdminTypes.ContainsKey(r.user_id) || (r.User != null && dictCompaniesAdminTypes[r.user_id] == r.User.admin_type);
                    if (adminTypesToInclude != null)
                        add = r.User != null && adminTypesToInclude.Contains(r.User.admin_type ?? 0);
                    
                    if (adminTypesToExclude != null)
                        add = r.User == null || !adminTypesToExclude.Contains(r.User.admin_type ?? 0);
                    if(add)
                        result.Add(r);

                }

                dr.Close();
            }

            return result;
        }

        public static List<Login_history> GetAll()
        {
            var result = new List<Login_history>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT * FROM login_history WHERE login_date >= @date");
                cmd.Parameters.AddWithValue("@date", DateTime.Now.AddYears(-1));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();

            }
            return result;
        }

        public static List<Login_history> GetForWebsite(string website, DateTime? dateFrom, DateTime? dateTo)
        {
            var result = new List<Login_history>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT * FROM login_history WHERE (login_date >= @dateFrom OR @dateFrom IS NULL) AND
                                             (login_date <= @dateTo OR @dateTo IS NULL) AND login_history.website=@website");

                cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object)dateFrom.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@website", website.ToUpper());
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var r = GetFromDataReader(dr);
                    result.Add(r);
                }
                dr.Close();

            }
            return result;
        }

        private static Login_history GetFromDataReader(MySqlDataReader dr)
        {
            Login_history o = new Login_history();

            o.history_id = Utilities.FromDbValue<int>(dr["history_id"]);
            o.user_id = Utilities.FromDbValue<int>(dr["user_id"]);
            o.login_date = Utilities.FromDbValue<DateTime>(dr["login_date"]);
            o.login_username = (string.Empty + dr["login_username"]).Trim();
            o.login_country = string.Empty + dr["login_country"];
            o.website = string.Empty + dr["website"];
            o.ip_address = string.Empty + dr["ip_address"];
            o.pwd = string.Empty + dr["pwd"];
            o.session_id = string.Empty + dr["session_id"];
            return o;

        }

        public static void Create(Login_history o)
        {
            string insertsql = @"INSERT INTO login_history (user_id,login_date,login_username,login_country,website,ip_address,pwd) VALUES(@user_id,@login_date,@login_username,@login_country,@website,@ip_address,@pwd)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT history_id, session_id FROM login_history WHERE history_id = LAST_INSERT_ID()";
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    o.history_id = (int)dr["history_id"];
                    o.session_id = string.Empty + dr["session_id"];
                }

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Login_history o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@history_id", o.history_id);
            cmd.Parameters.AddWithValue("@user_id", o.user_id);
            cmd.Parameters.AddWithValue("@login_date", o.login_date);
            cmd.Parameters.AddWithValue("@login_username", o.login_username);
            cmd.Parameters.AddWithValue("@login_country", o.login_country);
            cmd.Parameters.AddWithValue("@website", o.website);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
            cmd.Parameters.AddWithValue("@pwd", o.pwd);
            cmd.Parameters.AddWithValue("@session_id", o.session_id);
        }

        public static void Update(Login_history o)
        {
            string updatesql = @"UPDATE login_history SET user_id = @user_id,login_date = @login_date,login_username = @login_username,login_country = @login_country,website = @website,ip_address = @ip_address,pwd = @pwd,session_id = @session_id WHERE history_id = @history_id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int history_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM login_history WHERE history_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", history_id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
