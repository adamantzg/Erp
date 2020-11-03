
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Issue_statusDAL
	{
	
		public static List<Issue_status> GetAll()
		{
			List<Issue_status> result = new List<Issue_status>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_status", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Issue_status GetById(int id)
		{
			Issue_status result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_status WHERE status_id = @id", conn);
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
	
		private static Issue_status GetFromDataReader(MySqlDataReader dr)
		{
			Issue_status o = new Issue_status();
		
			o.status_id =  (int) dr["status_id"];
			o.statustext = string.Empty + dr["statustext"];
			
			return o;

		}

        public static void Create(Issue_status o)
        {
            string insertsql = @"INSERT INTO issue_status (status_id,statustext) VALUES(@status_id,@statustext)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(status_id)+1 FROM issue_status", conn);
                o.status_id = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = insertsql;

                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

            }
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue_status o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@status_id", o.status_id);
			cmd.Parameters.AddWithValue("@statustext", o.statustext);
		}
		
		public static void Update(Issue_status o)
		{
			string updatesql = @"UPDATE issue_status SET statustext = @statustext WHERE status_id = @status_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int status_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM issue_status WHERE status_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", status_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			