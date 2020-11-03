
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Stock_code_factoryDAL
	{
	
		public static List<Stock_code_factory> GetAll()
		{
			var result = new List<Stock_code_factory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_code_factory", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Stock_code_factory GetById(int id)
		{
			Stock_code_factory result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_code_factory WHERE stock_code_id = @id", conn);
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
		
	
		public static Stock_code_factory GetFromDataReader(MySqlDataReader dr)
		{
			Stock_code_factory o = new Stock_code_factory();
		
			o.stock_code_id =  (int) dr["stock_code_id"];
			o.factory_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"factory_id"));
			o.target_weeks = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"target_weeks"));
			
			return o;

		}
		
		
		public static void Create(Stock_code_factory o)
        {
            string insertsql = @"INSERT INTO stock_code_factory (stock_code_id,factory_id,target_weeks) VALUES(@stock_code_id,@factory_id,@target_weeks)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand("SELECT MAX(stock_code_id)+1 FROM stock_code_factory", conn);
                o.stock_code_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Stock_code_factory o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@stock_code_id", o.stock_code_id);
			cmd.Parameters.AddWithValue("@factory_id", o.factory_id);
			cmd.Parameters.AddWithValue("@target_weeks", o.target_weeks);
		}
		
		public static void Update(Stock_code_factory o)
		{
			string updatesql = @"UPDATE stock_code_factory SET factory_id = @factory_id,target_weeks = @target_weeks WHERE stock_code_id = @stock_code_id";

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
				var cmd = new MySqlCommand("DELETE FROM stock_code_factory WHERE stock_code_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", stock_code_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			