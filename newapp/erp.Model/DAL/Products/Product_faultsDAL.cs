
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Product_faultsDAL
	{
	
		public static List<Product_faults> GetAll()
		{
			var result = new List<Product_faults>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM product_faults", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Product_faults GetById(int id)
		{
			Product_faults result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM product_faults WHERE fault_id = @id", conn);
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
		
	
		private static Product_faults GetFromDataReader(MySqlDataReader dr)
		{
			Product_faults o = new Product_faults();
		
			o.fault_id =  (int) dr["fault_id"];
			o.fault_cprod = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fault_cprod"));
			o.fault_category = string.Empty + Utilities.GetReaderField(dr,"fault_category");
			o.fault_reason = string.Empty + Utilities.GetReaderField(dr,"fault_reason");
			o.fault_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fault_qty"));
			o.fault_order_number = string.Empty + Utilities.GetReaderField(dr,"fault_order_number");
			o.fault_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"fault_date"));
			o.fault_cost = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"fault_cost"));
			o.fault_comments = string.Empty + Utilities.GetReaderField(dr,"fault_comments");
			o.fault_po = string.Empty + Utilities.GetReaderField(dr,"fault_po");
			o.fault_original = string.Empty + Utilities.GetReaderField(dr,"fault_original");
			o.fault_TMS = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"fault_TMS"));
			o.fault_store = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fault_store"));
			o.fault_summary = string.Empty + Utilities.GetReaderField(dr,"fault_summary");
			
			return o;

		}
		
		
		public static void Create(Product_faults o)
        {
            string insertsql = @"INSERT INTO product_faults (fault_cprod,fault_category,fault_reason,fault_qty,fault_order_number,fault_date,fault_cost,fault_comments,fault_po,fault_original,fault_TMS,fault_store,fault_summary) VALUES(@fault_cprod,@fault_category,@fault_reason,@fault_qty,@fault_order_number,@fault_date,@fault_cost,@fault_comments,@fault_po,@fault_original,@fault_TMS,@fault_store,@fault_summary)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT fault_id FROM product_faults WHERE fault_id = LAST_INSERT_ID()";
                o.fault_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Product_faults o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@fault_id", o.fault_id);
			cmd.Parameters.AddWithValue("@fault_cprod", o.fault_cprod);
			cmd.Parameters.AddWithValue("@fault_category", o.fault_category);
			cmd.Parameters.AddWithValue("@fault_reason", o.fault_reason);
			cmd.Parameters.AddWithValue("@fault_qty", o.fault_qty);
			cmd.Parameters.AddWithValue("@fault_order_number", o.fault_order_number);
			cmd.Parameters.AddWithValue("@fault_date", o.fault_date);
			cmd.Parameters.AddWithValue("@fault_cost", o.fault_cost);
			cmd.Parameters.AddWithValue("@fault_comments", o.fault_comments);
			cmd.Parameters.AddWithValue("@fault_po", o.fault_po);
			cmd.Parameters.AddWithValue("@fault_original", o.fault_original);
			cmd.Parameters.AddWithValue("@fault_TMS", o.fault_TMS);
			cmd.Parameters.AddWithValue("@fault_store", o.fault_store);
			cmd.Parameters.AddWithValue("@fault_summary", o.fault_summary);
		}
		
		public static void Update(Product_faults o)
		{
			string updatesql = @"UPDATE product_faults SET fault_cprod = @fault_cprod,fault_category = @fault_category,fault_reason = @fault_reason,fault_qty = @fault_qty,fault_order_number = @fault_order_number,fault_date = @fault_date,fault_cost = @fault_cost,fault_comments = @fault_comments,fault_po = @fault_po,fault_original = @fault_original,fault_TMS = @fault_TMS,fault_store = @fault_store,fault_summary = @fault_summary WHERE fault_id = @fault_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int fault_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM product_faults WHERE fault_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", fault_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<Product_faults> GetProductFaults(int cprod_id, DateTime? from, DateTime? to)
        {
            var result = new List<Product_faults>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM product_faults WHERE fault_cprod = @cprod_id AND (fault_date >= @from OR @from IS NULL) AND (fault_date <= @to OR @to IS NULL)", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@from", from != null ? (object) from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object) to : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Product_faults> GetProductFaultsForCompanies(IList<int> company_ids, DateTime? from, DateTime? to)
        {
            var result = new List<Product_faults>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
//                cmd.CommandText = string.Format(@"SELECT * FROM product_faults WHERE EXISTS (SELECT linenum FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid WHERE order_header.userid1 IN ({0}) AND order_lines.cprod_id = product_faults.fault_cprod)
//                                                AND (fault_date >= @from OR @from IS NULL) AND (fault_date <= @to OR @to IS NULL)",Utilities.CreateParametersFromIdList(cmd,company_ids));
                cmd.CommandText = string.Format(@"SELECT product_faults.*, cust_products.*,mast_products.* FROM product_faults INNER JOIN cust_products ON product_faults.fault_cprod = cust_products.cprod_id 
                                                  INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                                  WHERE cust_products.cprod_user IN ({0}) AND (fault_date >= @from OR @from IS NULL) AND (fault_date <= @to OR @to IS NULL)",Utilities.CreateParametersFromIdList(cmd,company_ids));
                cmd.Parameters.AddWithValue("@from", from != null ? (object)from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object)to : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var pf = GetFromDataReader(dr);
                    pf.Product = Cust_productsDAL.GetFromDataReader(dr);
                    pf.Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(pf);
                }
                dr.Close();
            }
            return result;
        }

	}
}
			
			