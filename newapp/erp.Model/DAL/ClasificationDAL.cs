
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class ClasificationDAL
	{
	
		public static List<Clasification> GetAll()
		{
			List<Clasification> result = new List<Clasification>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM clasification", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Clasification GetById(int id)
		{
			Clasification result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM clasification WHERE clasification_id = @id", conn);
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
	
		private static Clasification GetFromDataReader(MySqlDataReader dr)
		{
			Clasification o = new Clasification();
		
			o.clasification_id =  (int) dr["clasification_id"];
			o.clasification_name = string.Empty + dr["clasification_name"];
			
			return o;

		}
		
		public static void Create(Clasification o)
        {
            string insertsql = @"INSERT INTO clasification (clasification_id,clasification_name) VALUES(@clasification_id,@clasification_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(clasification_id)+1 FROM clasification", conn);
                o.clasification_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Clasification o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@clasification_id", o.clasification_id);
			cmd.Parameters.AddWithValue("@clasification_name", o.clasification_name);
		}
		
		public static void Update(Clasification o)
		{
			string updatesql = @"UPDATE clasification SET clasification_name = @clasification_name WHERE clasification_id = @clasification_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int clasification_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM clasification WHERE clasification_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", clasification_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			