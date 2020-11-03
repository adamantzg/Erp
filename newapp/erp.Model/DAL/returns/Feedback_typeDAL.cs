
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Feedback_typeDAL
	{
	
		public static List<Feedback_type> GetAll()
		{
			var result = new List<Feedback_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM feedback_type", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Feedback_type GetById(int id)
		{
			Feedback_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM feedback_type WHERE type_id = @id", conn);
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
		
	
		private static Feedback_type GetFromDataReader(MySqlDataReader dr)
		{
			Feedback_type o = new Feedback_type();
		
			o.type_id =  (int) dr["type_id"];
			o.typename = string.Empty + Utilities.GetReaderField(dr,"typename");
			
			return o;

		}
		
		
		public static void Create(Feedback_type o)
        {
            string insertsql = @"INSERT INTO feedback_type (type_id,typename) VALUES(@type_id,@typename)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand("SELECT MAX(type_id)+1 FROM feedback_type", conn);
                o.type_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Feedback_type o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@type_id", o.type_id);
			cmd.Parameters.AddWithValue("@typename", o.typename);
		}
		
		public static void Update(Feedback_type o)
		{
			string updatesql = @"UPDATE feedback_type SET typename = @typename WHERE type_id = @type_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int type_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM feedback_type WHERE type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", type_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			