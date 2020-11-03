
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Sales_data_contractsDAL
	{
	
		public static List<Sales_data_contracts> GetAll()
		{
			var result = new List<Sales_data_contracts>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sales_data_contracts", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Sales_data_contracts GetById(int id)
		{
			Sales_data_contracts result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sales_data_contracts WHERE sales_unique = @id", conn);
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

        public static Sales_data_contracts GetByProdAndMonth(int cprod_id, int month)
        {
            Sales_data_contracts result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM Sales_data_contracts WHERE cprod_id = @cprod_id AND month21 = @month", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@month", month);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
		
	
		private static Sales_data_contracts GetFromDataReader(MySqlDataReader dr)
		{
			Sales_data_contracts o = new Sales_data_contracts();
		
			o.sales_unique =  (int) dr["sales_unique"];
			o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cprod_id"));
			o.sales_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sales_qty"));
			o.month21 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"month21"));
			o.type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"type"));
			
			return o;

		}
		
		
		public static void Create(Sales_data_contracts o)
        {
            string insertsql = @"INSERT INTO sales_data_contracts (cprod_id,sales_qty,month21,type) VALUES(@cprod_id,@sales_qty,@month21,@type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT sales_unique FROM sales_data_contracts WHERE sales_unique = LAST_INSERT_ID()";
                o.sales_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Sales_data_contracts o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@sales_unique", o.sales_unique);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@sales_qty", o.sales_qty);
			cmd.Parameters.AddWithValue("@month21", o.month21);
			cmd.Parameters.AddWithValue("@type", o.type);
		}
		
		public static void Update(Sales_data_contracts o)
		{
			string updatesql = @"UPDATE sales_data_contracts SET cprod_id = @cprod_id,sales_qty = @sales_qty,month21 = @month21,type = @type WHERE sales_unique = @sales_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int sales_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM sales_data_contracts WHERE sales_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", sales_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			