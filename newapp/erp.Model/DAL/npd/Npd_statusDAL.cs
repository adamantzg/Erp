
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Npd_statusDAL
	{
	
		public static List<Npd_status> GetAll()
		{
			var result = new List<Npd_status>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_status", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Npd_status GetById(int id)
		{
			Npd_status result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_status WHERE npdstatus_id = @id", conn);
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
		
	
		private static Npd_status GetFromDataReader(MySqlDataReader dr)
		{
			Npd_status o = new Npd_status();
		
			o.npdstatus_id =  (int) dr["npdstatus_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Npd_status o)
        {
            string insertsql = @"INSERT INTO npd_status (npdstatus_id,name) VALUES(@npdstatus_id,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand("SELECT MAX(npdstatus_id)+1 FROM npd_status", conn);
                o.npdstatus_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Npd_status o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@npdstatus_id", o.npdstatus_id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Npd_status o)
		{
			string updatesql = @"UPDATE npd_status SET name = @name WHERE npdstatus_id = @npdstatus_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int npdstatus_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM npd_status WHERE npdstatus_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", npdstatus_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			