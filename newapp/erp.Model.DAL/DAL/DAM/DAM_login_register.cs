using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL
{
    public partial class DAM_login_registerDAL
    {
        public static List<DAM_login_register> GetAll(int type)
        {
            var result = new List<DAM_login_register>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_login_register AS dlr INNER JOIN dam_users AS du ON dlr.user_id = du.id WHERE du.type = @type", conn);
                cmd.Parameters.AddWithValue("@type", type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_login_register GetById(int id)
        {
            DAM_login_register result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_login_register WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_login_register GetFromDataReader(MySqlDataReader dr)
        {
            DAM_login_register o = new DAM_login_register();

            o.id = (int)dr["id"];
            o.ip_address = (string)dr["ip_address"];
            o.location = (string)dr["location"];
            o.login_date = (DateTime)dr["login_date"];
            o.user_id = (int)dr["user_id"];
            o.accepted_terms = Utilities.FromDbValue<int>(dr["accepted_terms"]);
            o.country = string.Empty + Utilities.GetReaderField(dr, "country"); ;

            return o;

        }

        public static void Create(DAM_login_register o)
        {
            string insertsql = @"INSERT INTO dam_login_register (user_id,ip_address,location,login_date,accepted_terms) VALUES(@user_id,@ip_address,@location,@login_date,@accepted_terms)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM dam_login_register WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, DAM_login_register o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@user_id", o.user_id);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
            cmd.Parameters.AddWithValue("@location", o.location);
            cmd.Parameters.AddWithValue("@login_date", o.login_date);
            cmd.Parameters.AddWithValue("@accepted_terms", o.accepted_terms);
        }

        public static void Update(DAM_login_register o)
        {
            string updatesql = @"UPDATE dam_login_register SET user_id = @user_id ,ip_address = @ip_address, location = @location, login_date = @login_date, accepted_terms=@accepted_terms WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateCountries(List<DAM_login_register> o)
        {
            string updatesql = @"UPDATE dam_login_register SET country=@country WHERE id = @id";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(updatesql, conn);
                cmd.Parameters.AddWithValue("@id", 0);
                cmd.Parameters.AddWithValue("@country", "");

                foreach (var item in o)
	            {
                    cmd.Parameters["@id"].Value = item.id;
                    cmd.Parameters["@country"].Value = item.country;
                    cmd.ExecuteNonQuery(); 
	            }
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM dam_login_register WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static DAM_login_register GetByUserId(int user_id)
        {
            DAM_login_register result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_login_register WHERE user_id = @user_id ORDER BY login_date DESC", conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Dictionary<string,int>> GetCountGroupedByMonth(int type)
        {
            var results = new List<Dictionary<string, int>>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT CONCAT(MONTHNAME(dlr.login_date),', ',YEAR(dlr.login_date)) AS monthyear, COUNT(*)
                                            AS count FROM dam_login_register AS dlr INNER JOIN dam_users AS du ON dlr.user_id = du.id WHERE du.type=@type GROUP BY MONTH(dlr.login_date) ORDER BY dlr.login_date DESC", conn);
                cmd.Parameters.AddWithValue("@type", type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new Dictionary<string, int>();
                    o.Add((string)dr["monthyear"], Convert.ToInt32(dr["count"]));
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<DAM_login_register> GetIPForCountryUpdate()
        {
            var result = new List<DAM_login_register>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT *
                                              FROM
                                                dam_login_register
                                              WHERE
                                                 ISNULL(dam_login_register.country)
                                                ", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
        
    }
}


