
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Stock_order_brand_allocationDAL
	{
	
		public static List<Stock_order_brand_allocation> GetAll()
		{
			var result = new List<Stock_order_brand_allocation>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_order_brand_allocation", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Stock_order_brand_allocation GetById(int id)
		{
			Stock_order_brand_allocation result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_order_brand_allocation WHERE id = @id", conn);
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
		
	
		private static Stock_order_brand_allocation GetFromDataReader(MySqlDataReader dr)
		{
			Stock_order_brand_allocation o = new Stock_order_brand_allocation();
		
			o.id =  (int) dr["id"];
			o.salesorder_line_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"salesorder_line_id"));
			o.stockorder_line_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"stockorder_line_id"));
			o.alloc_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"alloc_qty"));
			o.date_allocated = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date_allocated"));
			
			return o;

		}
		
		
		public static void Create(Stock_order_brand_allocation o)
        {
            string insertsql = @"INSERT INTO stock_order_brand_allocation (salesorder_line_id,stockorder_line_id,alloc_qty,date_allocated) VALUES(@salesorder_line_id,@stockorder_line_id,@alloc_qty,@date_allocated)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM stock_order_brand_allocation WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Stock_order_brand_allocation o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@salesorder_line_id", o.salesorder_line_id);
			cmd.Parameters.AddWithValue("@stockorder_line_id", o.stockorder_line_id);
			cmd.Parameters.AddWithValue("@alloc_qty", o.alloc_qty);
			cmd.Parameters.AddWithValue("@date_allocated", o.date_allocated);
		}
		
		public static void Update(Stock_order_brand_allocation o)
		{
			string updatesql = @"UPDATE stock_order_brand_allocation SET salesorder_line_id = @salesorder_line_id,stockorder_line_id = @stockorder_line_id,alloc_qty = @alloc_qty,date_allocated = @date_allocated WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM stock_order_brand_allocation WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			