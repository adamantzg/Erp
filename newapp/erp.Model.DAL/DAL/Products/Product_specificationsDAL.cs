
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Product_specificationsDAL
	{
	
		public static List<Product_specifications> GetAll()
		{
			var result = new List<Product_specifications>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM product_specifications", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Product_specifications GetById(int id)
		{
			Product_specifications result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM product_specifications WHERE prod_spec_unique = @id", conn);
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
		
	
		public static Product_specifications GetFromDataReader(MySqlDataReader dr)
		{
			Product_specifications o = new Product_specifications();
		
			o.prod_spec_unique =  (int) dr["prod_spec_unique"];
			o.prod_mast_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"prod_mast_id"));
			o.spec_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"spec_type"));
			o.prod_data = string.Empty + Utilities.GetReaderField(dr,"prod_data");
			
			return o;

		}
		
		
		public static void Create(Product_specifications o)
        {
            string insertsql = @"INSERT INTO product_specifications (prod_mast_id,spec_type,prod_data) VALUES(@prod_mast_id,@spec_type,@prod_data)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT prod_spec_unique FROM product_specifications WHERE prod_spec_unique = LAST_INSERT_ID()";
                o.prod_spec_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Product_specifications o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@prod_spec_unique", o.prod_spec_unique);
			cmd.Parameters.AddWithValue("@prod_mast_id", o.prod_mast_id);
			cmd.Parameters.AddWithValue("@spec_type", o.spec_type);
			cmd.Parameters.AddWithValue("@prod_data", o.prod_data);
		}
		
		public static void Update(Product_specifications o)
		{
			string updatesql = @"UPDATE product_specifications SET prod_mast_id = @prod_mast_id,spec_type = @spec_type,prod_data = @prod_data WHERE prod_spec_unique = @prod_spec_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int prod_spec_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM product_specifications WHERE prod_spec_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", prod_spec_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			