
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Container_imagesDAL
	{
	
		public static List<Container_images> GetAll()
		{
			var result = new List<Container_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM container_images", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Container_images> GetForInspection(int insp_id)
        {
            var result = new List<Container_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM container_images WHERE insp_unique = @insp_id", conn);
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
		
		
		public static Container_images GetById(int id)
		{
			Container_images result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM container_images WHERE image_unique = @id", conn);
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
		
	
		private static Container_images GetFromDataReader(MySqlDataReader dr)
		{
			Container_images o = new Container_images();
		
			o.image_unique =  (int) dr["image_unique"];
			o.container_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"container_id"));
			o.insp_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_unique"));
			o.insp_image = string.Empty + Utilities.GetReaderField(dr,"insp_image");
			o.insp_type = string.Empty + Utilities.GetReaderField(dr,"insp_type");
			
			return o;

		}
		
		
		public static void Create(Container_images o)
        {
            string insertsql = @"INSERT INTO container_images (container_id,insp_unique,insp_image,insp_type) VALUES(@container_id,@insp_unique,@insp_image,@insp_type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT image_unique FROM container_images WHERE image_unique = LAST_INSERT_ID()";
                o.image_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Container_images o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@image_unique", o.image_unique);
			cmd.Parameters.AddWithValue("@container_id", o.container_id);
			cmd.Parameters.AddWithValue("@insp_unique", o.insp_unique);
			cmd.Parameters.AddWithValue("@insp_image", o.insp_image);
			cmd.Parameters.AddWithValue("@insp_type", o.insp_type);
		}
		
		public static void Update(Container_images o)
		{
			string updatesql = @"UPDATE container_images SET container_id = @container_id,insp_unique = @insp_unique,insp_image = @insp_image,insp_type = @insp_type WHERE image_unique = @image_unique";

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
				var cmd = Utils.GetCommand("DELETE FROM container_images WHERE image_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", image_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			