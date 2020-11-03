
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Web_shortcutsDAL
	{
	
		public static List<Web_shortcuts> GetAll()
		{
			var result = new List<Web_shortcuts>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_shortcuts", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_shortcuts GetById(int id)
		{
			Web_shortcuts result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_shortcuts WHERE id = @id", conn);
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

        public static List<Web_shortcuts> GetByBrandId(int id)
        {
            var result = new List<Web_shortcuts>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_shortcuts WHERE web_site_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
	
		public static Web_shortcuts GetFromDataReader(MySqlDataReader dr)
		{
			Web_shortcuts o = new Web_shortcuts();
		
			o.id =  (int) dr["id"];
            o.destination_url = string.Empty + Utilities.GetReaderField(dr, "destination_url");
            o.shortcut_url = string.Empty + Utilities.GetReaderField(dr, "shortcut_url");
            o.web_site_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "web_site_id"));
			
			return o;

		}
		
		
		public static void Create(Web_shortcuts o)
        {
            string insertsql = @"INSERT INTO web_shortcuts (destination_url,shortcut_url,web_site_id) VALUES(@destination_url,@shortcut_url,@web_site_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM web_shortcuts WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_shortcuts o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@destination_url", o.destination_url);
            cmd.Parameters.AddWithValue("@shortcut_url", o.shortcut_url);
            cmd.Parameters.AddWithValue("@web_site_id", o.web_site_id);
        }
		
		public static void Update(Web_shortcuts o)
		{
            string updatesql = @"UPDATE web_shortcuts SET destination_url = @destination_url,shortcut_url = @shortcut_url,web_site_id = @web_site_id WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_shortcuts WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			