using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;
using erp.Model;

namespace erp.Model.DAL
{
    public class ImageDAL
    {
        public static List<ImageFile> GetAll()
        {
            var result = new List<ImageFile>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM image", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;

        }

        
        

        public static void Create(ImageFile  o)
        {
            string insertsql = @"INSERT INTO image(IdImage,Comment,Big)
                                VALUES(@IdImage,@Comment,@Big)";
            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(IdImage) + 1 FROM image", conn, tr);
                o.IdImage = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = insertsql;
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

                tr.Commit();
            }
            catch
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                conn = null;
            }
        }

        public static void Delete(int idImage)
        {
            using (var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM image WHERE ImageId = @id", conn);
                cmd.Parameters.AddWithValue("@id", idImage);
                cmd.ExecuteNonQuery();
            }
        }
        private static void BuildSqlParameters(MySqlCommand cmd, ImageFile o)
        {
            cmd.Parameters.AddWithValue("@IdImage", o.IdImage);
           // cmd.Parameters.AddWithValue("@Thumbnail", o.Thumbnail);
            cmd.Parameters.AddWithValue("@Big", o.BigImage);
            cmd.Parameters.AddWithValue("@Comment", o.Description);
            //cmd.Parameters.AddWithValue("@Name", o.Name);
        }

        private static ImageFile GetFromDataReader(MySqlDataReader dr)
        {
            ImageFile o=new ImageFile();
            o.IdImage = (int)dr["IdImage"];
           
            o.BigImage = (byte[]) dr["Big"];
            o.Description = String.Empty + dr["Comment"];
            // o.Name = String.Empty + dr["Name"];
           
            return o;
        }
        

    }
}
