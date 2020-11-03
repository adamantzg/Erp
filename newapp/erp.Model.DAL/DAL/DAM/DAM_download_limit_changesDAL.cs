using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL
{
    public partial class DAM_download_limit_changesDAL
    {
        public static List<download_limit_changes> GetAll()
        {
            var result = new List<download_limit_changes>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM download_limit_changes", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static download_limit_changes GetById(int id)
        {
            download_limit_changes result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM download_limit_changes WHERE id = @id", conn);
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


        public static download_limit_changes GetFromDataReader(MySqlDataReader dr)
        {
            download_limit_changes o = new download_limit_changes();

            o.id = (int)dr["id"];
            o.date = (DateTime)dr["date"];
            o.user_id = (int)dr["user_id"];
            o.old_limit = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "old_limit"));
            o.new_limit = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "new_limit"));


            return o;

        }

        public static void Create(download_limit_changes o)
        {
            string insertsql = @"INSERT INTO download_limit_changes (date,user_id,old_limit,new_limit) VALUES(@date,@user_id,@old_limit,@new_limit)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM download_limit_changes WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, download_limit_changes o)
        {
            cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@date", o.date);
            cmd.Parameters.AddWithValue("@user_id",o.user_id);
            cmd.Parameters.AddWithValue("@old_limit", o.old_limit);
            cmd.Parameters.AddWithValue("@new_limit", o.new_limit);
        }

        public static void Update(download_limit_changes o)
        {
            string updatesql = @"UPDATE download_limit_changes SET date = @date, user_id=@user_id, old_limit=@old_limit, new_limit=@new_limit WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM download_limit_changes WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<download_limit_changes> GetByUser(int userid)
        {
            var result = new List<download_limit_changes>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM download_limit_changes WHERE user_id = @userid", conn);
                cmd.Parameters.AddWithValue("@userid",userid);
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


