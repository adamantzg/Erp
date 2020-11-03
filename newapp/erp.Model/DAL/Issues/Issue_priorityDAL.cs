
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Issue_priorityDAL
	{
	
		public static List<Issue_priority> GetAll()
		{
			List<Issue_priority> result = new List<Issue_priority>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issue_priority", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Issue_priority GetById(int id)
		{
			Issue_priority result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issue_priority WHERE priority_id = @id", conn);
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
	
		private static Issue_priority GetFromDataReader(MySqlDataReader dr)
		{
			Issue_priority o = new Issue_priority();
		
			o.priority_id =  (int) dr["priority_id"];
			o.priority_text = string.Empty + dr["priority_text"];
			
			return o;

		}
		
		public static void Create(Issue_priority o)
        {
            string insertsql = @"INSERT INTO issue_priority (priority_id,priority_text) VALUES(@priority_id,@priority_text)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(priority_id)+1 FROM issue_priority", conn);
                o.priority_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue_priority o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@priority_id", o.priority_id);
			cmd.Parameters.AddWithValue("@priority_text", o.priority_text);
		}
		
		public static void Update(Issue_priority o)
		{
			string updatesql = @"UPDATE issue_priority SET priority_text = @priority_text WHERE priority_id = @priority_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int priority_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM issue_priority WHERE priority_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", priority_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			