
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class CountriesDAL
	{
	
		public static List<Countries> GetAll()
		{
			List<Countries> result = new List<Countries>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM countries", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Countries GetById(int id)
		{
			Countries result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM countries WHERE country_id = @id", conn);
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
	
		private static Countries GetFromDataReader(MySqlDataReader dr)
		{
			Countries o = new Countries();
		
			o.ISO2 = string.Empty + dr["ISO2"];
			o.ISO3 = string.Empty + dr["ISO3"];
			o.CountryName = string.Empty + dr["CountryName"];
            if(Utilities.ColumnExists(dr, "country_id"))
			    o.country_id =  (int) dr["country_id"];
			
			return o;

		}
		
		public static void Create(Countries o)
        {
            string insertsql = @"INSERT INTO countries (ISO2,ISO3,CountryName) VALUES(@ISO2,@ISO3,@CountryName)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT country_id FROM countries WHERE country_id = LAST_INSERT_ID()";
                o.country_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Countries o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@ISO2", o.ISO2);
			cmd.Parameters.AddWithValue("@ISO3", o.ISO3);
			cmd.Parameters.AddWithValue("@CountryName", o.CountryName);
			if(!forInsert)
				cmd.Parameters.AddWithValue("@country_id", o.country_id);
		}
		
		public static void Update(Countries o)
		{
			string updatesql = @"UPDATE countries SET ISO2 = @ISO2,ISO3 = @ISO3,CountryName = @CountryName WHERE country_id = @country_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int country_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM countries WHERE country_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", country_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			