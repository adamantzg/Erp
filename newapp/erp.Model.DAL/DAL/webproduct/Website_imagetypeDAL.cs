
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Website_imagetypeDAL
	{
	
		public static List<Website_imagetype> GetAll()
		{
			var result = new List<Website_imagetype>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM website_imagetype", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Website_imagetype GetById(int id)
		{
			Website_imagetype result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM website_imagetype WHERE imagetype_id = @id", conn);
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
		
	
		public static Website_imagetype GetFromDataReader(MySqlDataReader dr)
		{
			Website_imagetype o = new Website_imagetype();
		
			o.website_id =  (int) dr["website_id"];
			o.imagetype_id =  (int) dr["imagetype_id"];
			
			return o;

		}
		
		
		public static void Create(Website_imagetype o)
        {
            string insertsql = @"INSERT INTO website_imagetype (website_id,imagetype_id) VALUES(@website_id,@imagetype_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(imagetype_id)+1 FROM website_imagetype", conn);
                o.imagetype_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Website_imagetype o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@website_id", o.website_id);
			cmd.Parameters.AddWithValue("@imagetype_id", o.imagetype_id);
		}
		
		public static void Update(Website_imagetype o)
		{
			string updatesql = @"UPDATE website_imagetype SET  WHERE imagetype_id = @imagetype_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int imagetype_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM website_imagetype WHERE imagetype_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", imagetype_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			