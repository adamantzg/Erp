
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Returns_imagesDAL
	{
	
		public static List<Returns_images> GetAll()
		{
			var result = new List<Returns_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_images", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static List<Returns_images> GetByReturn(int return_id)
		{
			List<Returns_images> result = new List<Returns_images>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_images WHERE return_id = @return_id", conn);
				cmd.Parameters.AddWithValue("@return_id", return_id);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Returns_images GetFromDataReader(MySqlDataReader dr)
		{
			Returns_images o = new Returns_images();
		
			o.image_unique =  (int) dr["image_unique"];
			o.return_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"return_id"));
			o.return_image = string.Empty + Utilities.GetReaderField(dr,"return_image");
			o.user_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"user_type"));
			o.cc_use = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cc_use"));
			o.added_by = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"added_by"));
			o.added_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"added_date"));
			
			return o;

		}


        public static void Create(Returns_images o, MySqlTransaction tr)
        {
            string insertsql = @"INSERT INTO returns_images (return_id,return_image,user_type,cc_use,added_by,added_date) VALUES(@return_id,@return_image,@user_type,@cc_use,@added_by,@added_date)";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
			if(tr == null)
                conn.Open();
							
			MySqlCommand cmd = new MySqlCommand(insertsql, conn, tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT image_unique FROM returns_images WHERE image_unique = LAST_INSERT_ID()";
            o.image_unique = (int) cmd.ExecuteScalar();
			
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Returns_images o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@image_unique", o.image_unique);
			cmd.Parameters.AddWithValue("@return_id", o.return_id);
			cmd.Parameters.AddWithValue("@return_image", o.return_image);
			cmd.Parameters.AddWithValue("@user_type", o.user_type);
			cmd.Parameters.AddWithValue("@cc_use", o.cc_use);
			cmd.Parameters.AddWithValue("@added_by", o.added_by);
			cmd.Parameters.AddWithValue("@added_date", o.added_date);
		}

        public static void Update(Returns_images o, MySqlTransaction tr)
		{
			string updatesql = @"UPDATE returns_images SET return_id = @return_id,return_image = @return_image,user_type = @user_type,cc_use = @cc_use,added_by = @added_by,added_date = @added_date WHERE image_unique = @image_unique";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
			
			MySqlCommand cmd = new MySqlCommand(updatesql, conn);
            BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();
            
		}
		
		public static void Delete(int image_unique, MySqlTransaction tr)
		{
			var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
			var cmd = new MySqlCommand("DELETE FROM returns_images WHERE image_unique = @id" , conn,tr);
            cmd.Parameters.AddWithValue("@id", image_unique);
            cmd.ExecuteNonQuery();
		}
		
		
	}
}
			
			