using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL
{
    public partial class DAM_usersDAL
    {
        public static List<DAM_users> GetAll(int? type = 0)
        {
            var result = new List<DAM_users>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_users WHERE type=@type", conn);
                cmd.Parameters.AddWithValue("@type", type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var user = GetFromDataReader(dr);
                    user.Permissions = DAM_user_permissionsDAL.GetById((int)dr["id"]);
                    result.Add(user);
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_users GetById(int id)
        {
            DAM_users result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_users WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Permissions = DAM_user_permissionsDAL.GetById(id);
                }
                dr.Close();
            }
            return result;
        }

        public static DAM_users GetUser(string name, string password)
        {
            DAM_users result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_users WHERE name = @name AND password = @password", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@password", password);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Permissions = DAM_user_permissionsDAL.GetById((int)dr["id"]);
                }
                dr.Close();
            }
            return result;
        }

        public static DAM_users GetUserByName(string name)
        {
            DAM_users result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_users WHERE name = @name", conn);
                cmd.Parameters.AddWithValue("@name", name);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Permissions = DAM_user_permissionsDAL.GetById((int)dr["id"]);
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_users GetFromDataReader(MySqlDataReader dr)
        {
            DAM_users o = new DAM_users();

            o.id = (int)dr["id"];
            o.name = (string)dr["name"];
            o.password = (string)dr["password"];
            o.email = string.Empty + dr["email"];
            o.company = string.Empty + dr["company"];
            o.isAdmin = Utilities.BoolFromLong(Utilities.FromDbValue<long>(dr["isAdmin"]));
            o.menutype = Utilities.BoolFromLong(Utilities.FromDbValue<long>(dr["menutype"]));
            o.isActivated = Utilities.BoolFromLong(Utilities.FromDbValue<long>(dr["isActivated"]));
            o.date_registered = Utilities.FromDbValue<DateTime>(dr["date_registered"]);
            o.date_activated = Utilities.FromDbValue<DateTime>(dr["date_activated"]);
            o.Contact_Optout = Utilities.BoolFromLong(dr["contact_optout"]);
            o.download_limit = Utilities.FromDbValue<int>(dr["download_limit"]) != null ? Utilities.FromDbValue<int>(dr["download_limit"]) : o.download_limit = 250;
            o.type = (int)dr["type"];
            o.dealer_id = Utilities.FromDbValue<int>(dr["dealer_id"]);
            return o;

        }

        public static void Create(DAM_users o)
        {
            string insertsql = @"INSERT INTO dam_users (name,password,email,isAdmin,menutype,isActivated,company,date_registered,date_activated,contact_optout,download_limit,type) 
                                VALUES(@name,@password,@email,@isAdmin,@menutype,@isActivated,@company,@date_registered,@date_activated,@contact_optout,@download_limit,@type)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM dam_users WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, DAM_users o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@name", o.name);
            cmd.Parameters.AddWithValue("@password", o.password);
            cmd.Parameters.AddWithValue("@email", o.email);
            cmd.Parameters.AddWithValue("@isAdmin", o.isAdmin);
            cmd.Parameters.AddWithValue("@menutype", o.menutype);
            cmd.Parameters.AddWithValue("@isActivated", o.isActivated);
            cmd.Parameters.AddWithValue("@company", o.company);
            cmd.Parameters.AddWithValue("@date_registered", o.date_registered);
            cmd.Parameters.AddWithValue("@date_activated", o.date_activated);
            cmd.Parameters.AddWithValue("@contact_optout", o.Contact_Optout == true ? 1 : 0);
            cmd.Parameters.AddWithValue("@download_limit", o.download_limit);
            cmd.Parameters.AddWithValue("@type", o.type);
        }

        public static void Update(DAM_users o)
        {
            string updatesql = @"UPDATE dam_users SET name = @name,password = @password,email = @email, isAdmin = @isAdmin, menutype = @menutype, isActivated=@isActivated, company=@company,
                                date_registered=@date_registered, date_activated=@date_activated,contact_optout = @contact_optout, download_limit=@download_limit, type=@type  WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM dam_users WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<DAM_users> GetAllUnActive()
        {
            var results = new List<DAM_users>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_users WHERE isActivated=0", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var user = GetFromDataReader(dr);
                    user.Permissions = DAM_user_permissionsDAL.GetById((int)dr["id"]);
                    results.Add(user);
                }
                dr.Close();
            }
            return results;
        }

        public static List<DAM_users> GetByDownloadActivity(int type)
        {
            var result = new List<DAM_users>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_users AS du INNER JOIN downloaded_file AS df ON df.user=du.id WHERE du.type = @type", conn);
                cmd.Parameters.AddWithValue("@type", type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var user = GetFromDataReader(dr);
                    user.Permissions = DAM_user_permissionsDAL.GetById((int)dr["id"]);
                    result.Add(user);
                }
                dr.Close();
            }
            return result;
        }
    }
}


