
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Stock_codeDAL
	{
	
		public static List<Stock_code> GetAll()
		{
			var result = new List<Stock_code>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_code", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Stock_code GetById(int id)
		{
			Stock_code result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_code WHERE stock_code_id = @id", conn);
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
		
	
		public static Stock_code GetFromDataReader(MySqlDataReader dr)
		{
			Stock_code o = new Stock_code();
		
			o.stock_code_id =  (int) dr["stock_code_id"];
			o.stock_code_name = string.Empty + Utilities.GetReaderField(dr,"stock_code_name");
			o.target_weeks = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"target_weeks"));
			
			return o;

		}
		
		
		public static void Create(Stock_code o)
        {
            string insertsql = @"INSERT INTO stock_code (stock_code_id,stock_code_name,target_weeks) VALUES(@stock_code_id,@stock_code_name,@target_weeks)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand("SELECT MAX(stock_code_id)+1 FROM stock_code", conn);
                o.stock_code_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Stock_code o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@stock_code_id", o.stock_code_id);
			cmd.Parameters.AddWithValue("@stock_code_name", o.stock_code_name);
			cmd.Parameters.AddWithValue("@target_weeks", o.target_weeks);
		}
		
		public static void Update(Stock_code o)
		{
			string updatesql = @"UPDATE stock_code SET stock_code_name = @stock_code_name,target_weeks = @target_weeks WHERE stock_code_id = @stock_code_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int stock_code_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM stock_code WHERE stock_code_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", stock_code_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			