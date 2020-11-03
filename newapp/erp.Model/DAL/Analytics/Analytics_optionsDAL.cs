
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Analytics_optionsDAL
	{
	
		public static List<Analytics_options> GetAll()
		{
			var result = new List<Analytics_options>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM analytics_options", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Analytics_options GetById(int id)
		{
			Analytics_options result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM analytics_options WHERE option_id = @id", conn);
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
		
	
		public static Analytics_options GetFromDataReader(MySqlDataReader dr)
		{
			Analytics_options o = new Analytics_options();
		
			o.option_id =  (int) dr["option_id"];
			o.option_name = string.Empty + Utilities.GetReaderField(dr,"option_name");
			
			return o;

		}
		
		
		public static void Create(Analytics_options o)
        {
            string insertsql = @"INSERT INTO analytics_options (option_name) VALUES(@option_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT option_id FROM analytics_options WHERE option_id = LAST_INSERT_ID()";
                o.option_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Analytics_options o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@option_id", o.option_id);
			cmd.Parameters.AddWithValue("@option_name", o.option_name);
		}
		
		public static void Update(Analytics_options o)
		{
			string updatesql = @"UPDATE analytics_options SET option_name = @option_name WHERE option_id = @option_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int option_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM analytics_options WHERE option_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", option_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			