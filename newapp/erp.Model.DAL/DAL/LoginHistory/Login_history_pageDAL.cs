
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Login_history_pageDAL
	{
	
		public static List<Login_history_page> GetAll()
		{
			var result = new List<Login_history_page>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM login_history_page", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Login_history_page GetById(int id)
		{
			Login_history_page result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM login_history_page WHERE page_id = @id", conn);
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
		
	
		public static Login_history_page GetFromDataReader(MySqlDataReader dr)
		{
			Login_history_page o = new Login_history_page();
		
			o.page_id =  (int) dr["page_id"];
			o.page_url = string.Empty + Utilities.GetReaderField(dr,"page_url");
			o.page_description = string.Empty + Utilities.GetReaderField(dr,"page_description");
			
			return o;

		}
		
		
		public static void Create(Login_history_page o)
        {
            string insertsql = @"INSERT INTO login_history_page (page_url,page_description) VALUES(@page_url,@page_description)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT page_id FROM login_history_page WHERE page_id = LAST_INSERT_ID()";
                o.page_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Login_history_page o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@page_id", o.page_id);
			cmd.Parameters.AddWithValue("@page_url", o.page_url);
			cmd.Parameters.AddWithValue("@page_description", o.page_description);
		}
		
		public static void Update(Login_history_page o)
		{
			string updatesql = @"UPDATE login_history_page SET page_url = @page_url,page_description = @page_description WHERE page_id = @page_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int page_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM login_history_page WHERE page_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", page_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			