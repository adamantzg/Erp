
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Issue_typeDAL
	{
	
		public static List<Issue_type> GetAll()
		{
			List<Issue_type> result = new List<Issue_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issue_type", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Issue_type GetById(int id)
		{
			Issue_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issue_type WHERE type_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
	
		private static Issue_type GetFromDataReader(MySqlDataReader dr)
		{
			Issue_type o = new Issue_type();
		
			o.type_id =  (int) dr["type_id"];
			o.type_name = string.Empty + dr["type_name"];
			
			return o;

		}
		
		public static void Create(Issue_type o)
        {
            string insertsql = @"INSERT INTO issue_type (type_id,type_name) VALUES(@issuetype_id,@type_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(type_id)+1 FROM issue_type", conn);
                o.type_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue_type o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@type_id", o.type_id);
			cmd.Parameters.AddWithValue("@type_name", o.type_name);
		}
		
		public static void Update(Issue_type o)
		{
			string updatesql = @"UPDATE issue_type SET type_name = @type_name WHERE type_id = @issuetype_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int issuetype_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM issue_type WHERE type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", issuetype_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			