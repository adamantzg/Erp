using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL
{
    public partial class DAM_user_permissionsDAL
    {
        public static List<DAM_user_permissions> GetAll()
        {
            var result = new List<DAM_user_permissions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_user_permissions", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static List<DAM_user_permissions> GetById(int id)
        {
            var result = new List<DAM_user_permissions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_user_permissions WHERE user_id = @user_id", conn);
                cmd.Parameters.AddWithValue("@user_id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_user_permissions GetFromDataReader(MySqlDataReader dr)
        {
            DAM_user_permissions o = new DAM_user_permissions();

            o.user_id = (int)dr["user_id"];
            o.function_id = (int)dr["function_id"];

            return o;

        }

        public static void Create(DAM_user_permissions o)
        {
            string insertsql = @"INSERT INTO dam_user_permissions (user_id,function_id) VALUES(@user_id,@function_id)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, DAM_user_permissions o)
        {
            cmd.Parameters.AddWithValue("@user_id", o.user_id);
            cmd.Parameters.AddWithValue("@function_id", o.function_id);
        }

        public static void Update(DAM_user_permissions o)
        {
            string updatesql = @"UPDATE dam_user_permissions SET user_id = @user_id, function_id = @function_id WHERE user_id = @user_id AND function_id = @function_id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int user_id, int? function_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var deletesql = "DELETE FROM dam_user_permissions WHERE user_id = @user_id {0}";
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@user_id", user_id);
                if (function_id != null)
                {
                    deletesql = string.Format(deletesql, "AND function_id = @function_id");
                    cmd.Parameters.AddWithValue("@function_id", function_id);
                }
                else
                    deletesql = string.Format(deletesql, "");

                cmd.CommandText = deletesql;
                cmd.ExecuteNonQuery();
            }
        }
    }
}


