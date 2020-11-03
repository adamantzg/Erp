
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Web_siteDAL
	{
	
		public static List<Web_site> GetAll()
		{
			var result = new List<Web_site>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_site", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_site GetById(int id)
		{
			Web_site result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_site WHERE id = @id", conn);
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

        public static Web_site GetByBrandId(int id)
        {
            Web_site result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_site WHERE brand_id = @id", conn);
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

        public static Web_site GetByCode(string code)
        {
            Web_site result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_site WHERE code = @code", conn);
                cmd.Parameters.AddWithValue("@code", code);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
		
	
		public static Web_site GetFromDataReader(MySqlDataReader dr)
		{
			Web_site o = new Web_site();
		
			o.id =  (int) dr["id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.code = string.Empty + Utilities.GetReaderField(dr,"code");
			o.brand_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand_id"));
		    o.Url = string.Empty + dr["url"];
			
			return o;

		}
		
		
		public static void Create(Web_site o)
        {
            string insertsql = @"INSERT INTO web_site (name,code,brand_id,url) VALUES(@name,@code,@brand_id,@url)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM web_site WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_site o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@code", o.code);
			cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
		    cmd.Parameters.AddWithValue("@url", o.Url);
        }
		
		public static void Update(Web_site o)
		{
			string updatesql = @"UPDATE web_site SET name = @name,code = @code,brand_id = @brand_id,url = @url WHERE id = @id";

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
				var cmd = Utils.GetCommand("DELETE FROM web_site WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			