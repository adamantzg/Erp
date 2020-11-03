using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class LoginhistoryDAL
    {
        public static Login_history GetByCriteria(string session_id, DateTime? login_date)
        {
            Login_history result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM login_history WHERE (session_id = @session_id OR @session_id IS NULL) AND ( DATE_ADD(@login_date, INTERVAL '24' HOUR) > NOW() OR @login_date IS NULL )", conn);
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

        private static Login_history GetFromDataReader(MySqlDataReader dr)
        {
            Login_history o = new Login_history();

            o.history_id = Utilities.FromDbValue<int>(dr["history_id"]);
            o.user_id = Utilities.FromDbValue<int>(dr["user_id"]);
            o.login_date = Utilities.FromDbValue<DateTime>(dr["login_date"]);
            o.login_username = string.Empty + dr["login_username"];
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

                MySqlCommand cmd = new MySqlCommand(insertsql, conn);
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
                MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int history_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM login_history WHERE history_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", history_id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
