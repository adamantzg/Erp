using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL
{
    public partial class Downloaded_fileDAL
    {
        public static List<Downloaded_file> GetAll(int type = 0)
        {
            var result = new List<Downloaded_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM downloaded_file AS df LEFT JOIN dam_users AS du ON df.user = du.id WHERE du.type = @type ORDER BY date DESC", conn);
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


        public static Downloaded_file GetById(int id)
        {
            Downloaded_file result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM downloaded_file WHERE id = @id", conn);
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


        public static Downloaded_file GetFromDataReader(MySqlDataReader dr)
        {
            Downloaded_file o = new Downloaded_file();

            o.id = (int)dr["id"];
            o.user = (int)dr["user"];
            o.date = (DateTime)dr["date"];
            o.web_unique = (int)dr["web_unique"];
            o.file_type = (int)dr["file_type"];
            o.fileext = string.Empty + dr["fileext"];

            return o;

        }

        public static void Create(Downloaded_file o)
        {
            string insertsql = @"INSERT INTO downloaded_file (web_unique,user,date,file_type,fileext) VALUES(@web_unique,@user,@date,@file_type,@fileext)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM downloaded_file WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Downloaded_file o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
            cmd.Parameters.AddWithValue("@user", o.user);
            cmd.Parameters.AddWithValue("@date", o.date);
            cmd.Parameters.AddWithValue("@file_type", o.file_type);
            cmd.Parameters.AddWithValue("@fileext", o.fileext);
        }

        public static void Update(Downloaded_file o)
        {
            string updatesql = @"UPDATE downloaded_file SET web_unique = @web_unique,user = @user,date = @date,file_type = @file_type, fileext = @fileext WHERE id = @id";

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
                var cmd = Utils.GetCommand("DELETE FROM downloaded_file WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Downloaded_file> GetDownloadedFilesByUser(int id)
        {
            var result = new List<Downloaded_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM downloaded_file WHERE user = @user", conn);
                cmd.Parameters.AddWithValue("@user", id);
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


