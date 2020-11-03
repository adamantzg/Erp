
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Returns_comments_filesDAL
	{
	
		public static List<Returns_comments_files> GetAll()
		{
			var result = new List<Returns_comments_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM returns_comments_files", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Returns_comments_files> GetForComment(int comment_id)
        {
            var result = new List<Returns_comments_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM returns_comments_files WHERE return_comment_id = @comment_id", conn);
                cmd.Parameters.AddWithValue("@comment_id", comment_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Returns_comments_files GetById(int id)
		{
			Returns_comments_files result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM returns_comments_files WHERE return_comment_file_id = @id", conn);
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
		
	
		private static Returns_comments_files GetFromDataReader(MySqlDataReader dr)
		{
			Returns_comments_files o = new Returns_comments_files();
		
			o.return_comment_file_id =  (int) dr["return_comment_file_id"];
			o.return_comment_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"return_comment_id"));
			o.image_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"image_id"));
		    o.image_name = string.Empty + dr["image_name"];
			
			return o;

		}
		
		
		public static void Create(Returns_comments_files o, MySqlTransaction tr)
        {
            string insertsql = @"INSERT INTO returns_comments_files (return_comment_id,image_id,image_name) VALUES(@return_comment_id,@image_id,@image_name)";

			var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if (tr == null)
                conn.Open(); 
				
			MySqlCommand cmd = Utils.GetCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT return_comment_file_id FROM returns_comments_files WHERE return_comment_file_id = LAST_INSERT_ID()";
            o.return_comment_file_id = (int) cmd.ExecuteScalar();
				
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Returns_comments_files o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@return_comment_file_id", o.return_comment_file_id);
			cmd.Parameters.AddWithValue("@return_comment_id", o.return_comment_id);
			cmd.Parameters.AddWithValue("@image_id", o.image_id);
		    cmd.Parameters.AddWithValue("@image_name", o.image_name);
        }
		
		public static void Update(Returns_comments_files o)
		{
            string updatesql = @"UPDATE returns_comments_files SET return_comment_id = @return_comment_id,image_id = @image_id,image_name = @image_name 
                                WHERE return_comment_file_id = @return_comment_file_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int return_comment_file_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM returns_comments_files WHERE return_comment_file_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", return_comment_file_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			