
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class CurrenciesDAL
	{
	
		public static List<Currencies> GetAll()
		{
			List<Currencies> result = new List<Currencies>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM currencies", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Currencies GetById(int id)
		{
			Currencies result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM currencies WHERE curr_code = @id", conn);
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
	
		private static Currencies GetFromDataReader(MySqlDataReader dr)
		{
			Currencies o = new Currencies();
		
			o.curr_code =  (int) dr["curr_code"];
			o.curr_desc = string.Empty + dr["curr_desc"];
			o.curr_symbol = string.Empty + dr["curr_symbol"];
			o.curr_exch1 = Utilities.FromDbValue<double>(dr["curr_exch1"]);
			o.curr_exch2 = Utilities.FromDbValue<double>(dr["curr_exch2"]);
			o.curr_exch3 = Utilities.FromDbValue<double>(dr["curr_exch3"]);
			
			return o;

		}
		
		public static void Create(Currencies o)
        {
            string insertsql = @"INSERT INTO currencies (curr_desc,curr_symbol,curr_exch1,curr_exch2,curr_exch3) VALUES(@curr_desc,@curr_symbol,@curr_exch1,@curr_exch2,@curr_exch3)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT curr_code FROM currencies WHERE curr_code = LAST_INSERT_ID()";
                o.curr_code = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Currencies o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@curr_code", o.curr_code);
			cmd.Parameters.AddWithValue("@curr_desc", o.curr_desc);
			cmd.Parameters.AddWithValue("@curr_symbol", o.curr_symbol);
			cmd.Parameters.AddWithValue("@curr_exch1", o.curr_exch1);
			cmd.Parameters.AddWithValue("@curr_exch2", o.curr_exch2);
			cmd.Parameters.AddWithValue("@curr_exch3", o.curr_exch3);
		}
		
		public static void Update(Currencies o)
		{
			string updatesql = @"UPDATE currencies SET curr_desc = @curr_desc,curr_symbol = @curr_symbol,curr_exch1 = @curr_exch1,curr_exch2 = @curr_exch2,curr_exch3 = @curr_exch3 WHERE curr_code = @curr_code";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int curr_code)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM currencies WHERE curr_code = @id" , conn);
                cmd.Parameters.AddWithValue("@id", curr_code);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			