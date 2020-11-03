
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Inspection_imagesDAL
	{
	
		public static List<Inspection_images> GetAll()
		{
			var result = new List<Inspection_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_images", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_images> GetByInspection(int insp_id)
        {
            var result = new List<Inspection_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_images WHERE insp_unique = @insp_id", conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Inspection_images GetById(int id)
		{
			Inspection_images result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_images WHERE image_unique = @id", conn);
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
		
	
		private static Inspection_images GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_images o = new Inspection_images();
		
			o.image_unique =  (int) dr["image_unique"];
			o.insp_line_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_line_unique"));
			o.insp_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_unique"));
			o.insp_image = string.Empty + Utilities.GetReaderField(dr,"insp_image");
			o.insp_type = string.Empty + Utilities.GetReaderField(dr,"insp_type");
			o.rej_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"rej_flag"));
			
			return o;

		}
		
		
		public static void Create(Inspection_images o)
        {
            string insertsql = @"INSERT INTO inspection_images (insp_line_unique,insp_unique,insp_image,insp_type,rej_flag) VALUES(@insp_line_unique,@insp_unique,@insp_image,@insp_type,@rej_flag)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT image_unique FROM inspection_images WHERE image_unique = LAST_INSERT_ID()";
                o.image_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_images o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@image_unique", o.image_unique);
			cmd.Parameters.AddWithValue("@insp_line_unique", o.insp_line_unique);
			cmd.Parameters.AddWithValue("@insp_unique", o.insp_unique);
			cmd.Parameters.AddWithValue("@insp_image", o.insp_image);
			cmd.Parameters.AddWithValue("@insp_type", o.insp_type);
			cmd.Parameters.AddWithValue("@rej_flag", o.rej_flag);
		}
		
		public static void Update(Inspection_images o)
		{
			string updatesql = @"UPDATE inspection_images SET insp_line_unique = @insp_line_unique,insp_unique = @insp_unique,insp_image = @insp_image,insp_type = @insp_type,rej_flag = @rej_flag WHERE image_unique = @image_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int image_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM inspection_images WHERE image_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", image_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			