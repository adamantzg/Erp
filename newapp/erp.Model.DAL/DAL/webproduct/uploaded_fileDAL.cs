using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model.DAL
{
    public partial class Uploaded_fileDAL
    {
        public static List<Uploaded_file> GetAll(int type)
        {
            var result = new List<Uploaded_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM uploaded_file AS uf INNER JOIN dam_users AS du ON uf.upload_user=du.id WHERE du.type=@type", conn);
                cmd.Parameters.AddWithValue("@type",type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static Uploaded_file GetById(int id)
        {
            Uploaded_file result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM uploaded_file WHERE id = @id", conn);
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


        public static Uploaded_file GetFromDataReader(MySqlDataReader dr)
        {
            Uploaded_file o = new Uploaded_file();

            o.id = (int)dr["id"];
            o.name = string.Empty + Utilities.GetReaderField(dr, "name");
            o.type_id =  (int)dr["type_id"];
            o.upload_date = (DateTime)dr["upload_date"];
            o.upload_user = (int)dr["upload_user"];
            o.related = Utilities.BoolFromLong(dr["related"]) ?? false;
            o.comment = string.Empty + Utilities.GetReaderField(dr, "comment");
            o.folder = string.Empty + Utilities.GetReaderField(dr, "folder");

            return o;

        }

        public static void Create(Uploaded_file o)
        {
            string insertsql = @"INSERT INTO uploaded_file (name,type_id,upload_date,upload_user,related,comment,folder) 
                                VALUES(@name,@type_id,@upload_date,@upload_user,@related,@comment,@folder)";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM uploaded_file WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
            
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Uploaded_file o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@name", o.name);
            cmd.Parameters.AddWithValue("@type_id", o.type_id);
            cmd.Parameters.AddWithValue("@upload_date", o.upload_date);
            cmd.Parameters.AddWithValue("@upload_user", o.upload_user);
            cmd.Parameters.AddWithValue("@related", o.related);
            cmd.Parameters.AddWithValue("@comment", o.comment);
            cmd.Parameters.AddWithValue("@folder", o.folder);
        }

        public static void Update(Uploaded_file o)
        {
            string updatesql = @"UPDATE uploaded_file SET name = @name,type_id = @type_id,upload_date = @upload_date,upload_user = @upload_user, related = @related, comment = @comment, folder=@folder WHERE id = @id";

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
                var cmd = Utils.GetCommand("DELETE FROM uploaded_file WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static int GetCountByName(string name, string folder=null)
        {
            var count = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = "SELECT Count(*) FROM uploaded_file WHERE name = @name {0}";
                query = string.Format(query,!string.IsNullOrEmpty(folder) ? " AND folder=@folder" : "");
                var cmd = Utils.GetCommand(query, conn);
                if (!string.IsNullOrEmpty(folder))
                    cmd.Parameters.AddWithValue("@folder", folder);
                cmd.Parameters.AddWithValue("@name", name);
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return count;
        }

        public static List<Uploaded_file> GetByUser(int user_id)
        {
            var results = new List<Uploaded_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM uploaded_file WHERE upload_user=@user_id AND folder <> ''", conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    results.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return results;
        }

        public static List<Uploaded_file> GetByFolder(string foldername, int user_id)
        {
            var results = new List<Uploaded_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM uploaded_file WHERE foldername=@foldername AND upload_user=@user_id", conn);
                cmd.Parameters.AddWithValue("@foldername", foldername);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    results.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return results;
        }
    }
}


