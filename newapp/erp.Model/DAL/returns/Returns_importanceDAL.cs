
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Returns_importanceDAL
	{
	
		public static List<Returns_importance> GetAll()
		{
			var result = new List<Returns_importance>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_importance", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Returns_importance> GetForType(int type)
        {
            var result = new List<Returns_importance>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_importance WHERE feedback_type_id = @feedback_type", conn);
                cmd.Parameters.AddWithValue("@feedback_type", type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Returns_importance GetById(int id)
		{
			Returns_importance result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_importance WHERE importance_id = @id", conn);
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
		
	
		public static Returns_importance GetFromDataReader(MySqlDataReader dr)
		{
			Returns_importance o = new Returns_importance();
		
			o.importance_id =  (int) dr["importance_id"];
			o.importance_text = string.Empty + Utilities.GetReaderField(dr,"importance_text");
			o.feedback_type_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"feedback_type_id"));
		    o.days = Utilities.FromDbValue<int>(dr["days"]);
			
			return o;

		}
		
		
		public static void Create(Returns_importance o)
        {
            string insertsql = @"INSERT INTO returns_importance (importance_id,importance_text,feedback_type_id) VALUES(@importance_id,@importance_text,@feedback_type_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand("SELECT MAX(importance_id)+1 FROM returns_importance", conn);
                o.importance_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Returns_importance o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@importance_id", o.importance_id);
			cmd.Parameters.AddWithValue("@importance_text", o.importance_text);
			cmd.Parameters.AddWithValue("@feedback_type_id", o.feedback_type_id);
		}
		
		public static void Update(Returns_importance o)
		{
			string updatesql = @"UPDATE returns_importance SET importance_text = @importance_text,feedback_type_id = @feedback_type_id WHERE importance_id = @importance_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int importance_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM returns_importance WHERE importance_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", importance_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			