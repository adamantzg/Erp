using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model.DAL
{
    public class Image_assignment_logDAL
    {
        public static List<Image_assignment_log> GetAll()
        {
            var result = new List<Image_assignment_log>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM image_assignment_log", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static Image_assignment_log GetById(int id)
        {
            Image_assignment_log result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM image_assignment_log WHERE id = @id", conn);
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


        public static Image_assignment_log GetFromDataReader(MySqlDataReader dr)
        {
            var o = new Image_assignment_log();

            o.id = (int)dr["id"];
            o.name = string.Empty + Utilities.GetReaderField(dr, "name");
            o.old_name = string.Empty + dr["old_name"];
            o.type_id = (int)dr["type_id"];
            o.upload_date = (DateTime)dr["upload_date"];
            o.LogDate = (DateTime) dr["logDate"];
            o.upload_user = (int)dr["upload_user"];
            o.related = Utilities.BoolFromLong(dr["related"]) ?? false;
            o.comment = string.Empty + Utilities.GetReaderField(dr, "comment");
            o.file_id = Utilities.FromDbValue<int>(dr["file_id"]);
            o.Is_Undo = Utilities.BoolFromLong(dr["is_undo"]) ?? false;
            o.old_site_id = Utilities.FromDbValue<int>(dr["old_site_id"]);

            return o;

        }

        public static void Create(Image_assignment_log o)
        {
            string insertsql = @"INSERT INTO image_assignment_log (name,type_id,upload_date,upload_user,related,comment,file_id,is_undo,logDate,old_name,old_site_id) 
                    VALUES(@name,@type_id,@upload_date,@upload_user,@related,@comment,@file_id,@is_undo,@logDate,@old_name,@old_site_id)";
            try
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();

                    var cmd = Utils.GetCommand(insertsql, conn);
                    BuildSqlParameters(cmd, o);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT id FROM image_assignment_log WHERE id = LAST_INSERT_ID()";
                    o.id = (int)cmd.ExecuteScalar();
                }
            }
            catch
            {

            }
            
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Image_assignment_log o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@name", o.name);
            cmd.Parameters.AddWithValue("@old_name", o.old_name);
            cmd.Parameters.AddWithValue("@type_id", o.type_id);
            cmd.Parameters.AddWithValue("@upload_date", o.upload_date);
            cmd.Parameters.AddWithValue("@upload_user", o.upload_user);
            cmd.Parameters.AddWithValue("@related", o.related);
            cmd.Parameters.AddWithValue("@comment", o.comment);
            cmd.Parameters.AddWithValue("@file_id", o.file_id);
            cmd.Parameters.AddWithValue("@is_undo", o.Is_Undo);
            cmd.Parameters.AddWithValue("@logDate", o.LogDate);
            cmd.Parameters.AddWithValue("@old_site_id", o.old_site_id);
        }

        public static void Update(Image_assignment_log o)
        {
            string updatesql = @"UPDATE image_assignment_log SET name = @name,type_id = @type_id,upload_date = @upload_date,upload_user = @upload_user, related = @related, comment = @comment, 
                                    file_id = @file_id, is_undo = @is_undo,logDate = @logDate,old_name = @old_name,old_site_id = @old_site_id WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int file_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM image_assignment_log WHERE file_id = @file_id", conn);
                cmd.Parameters.AddWithValue("@file_id", file_id);
                cmd.ExecuteNonQuery();
            }
        }

        public static int GetCountByName(string name)
        {
            var count = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT Count(*) FROM image_assignment_log WHERE name = @name", conn);
                cmd.Parameters.AddWithValue("@name", name);
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return count;
        }

        public static int GetLastRecord()
        {
            var id = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT id FROM image_assignment_log ORDER BY id DESC LIMIT 1;", conn);
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return id;
        }

        public static List<Image_assignment_log> GetByFileId(int file_id)
        {
            var result = new List<Image_assignment_log>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM image_assignment_log WHERE file_id = @file_id ORDER BY LogDate DESC", conn);
                cmd.Parameters.AddWithValue("@file_id", file_id);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Image_assignment_log> GetForCat(int catId)
        {
            var result = new List<Image_assignment_log>();
            var cats = new List<Web_category>();
            cats.Add(new Web_category { category_id = catId });
            cats.AddRange(Web_categoryDAL.GetAllChildren(catId));

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT image_assignment_log.id,image_assignment_log.`name`,image_assignment_log.old_name,image_assignment_log.type_id,image_assignment_log.upload_user,
                            image_assignment_log.upload_date,image_assignment_log.related,image_assignment_log.`comment`,image_assignment_log.file_id,image_assignment_log.is_undo,image_assignment_log.logDate,image_assignment_log.old_site_id FROM
                            image_assignment_log
                            INNER JOIN web_product_file ON web_product_file.id = image_assignment_log.file_id
                            INNER JOIN web_product_new ON web_product_new.web_unique = web_product_file.web_unique
                            INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique
                            WHERE web_product_category.category_id IN ({0})
                            ORDER BY LogDate DESC",Utils.CreateParametersFromIdList(cmd,cats.Select(c=>c.category_id).ToList()));
                cmd.Parameters.AddWithValue("@category_id", catId);
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
