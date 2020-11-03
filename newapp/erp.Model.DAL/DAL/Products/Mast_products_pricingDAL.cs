
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Mast_products_pricingDAL
	{
	
		public static List<Mast_products_pricing> GetAll()
		{
			var result = new List<Mast_products_pricing>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM mast_products_pricing", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Mast_products_pricing> GetByFactory(int? factory_id = null)
        {
            var result = new List<Mast_products_pricing>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT mast_products_pricing.*,mast_products.*,client.* ,cust_products.*
                                            FROM mast_products_pricing INNER JOIN mast_products ON mast_products_pricing.mast_id = mast_products.mast_id 
                                            INNER JOIN users client ON mast_products_pricing.client_id = client.user_id 
                                            LEFT OUTER JOIN cust_products ON client.user_id = cust_products.cprod_user AND mast_products.mast_id = cust_products.cprod_mast
                                            WHERE (mast_products.factory_id = @factory_id OR @factory_id IS NULL)", conn);
                cmd.Parameters.AddWithValue("@factory_id", factory_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var row = GetFromDataReader(dr);
                    row.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    row.Client = CompanyDAL.GetFromDataReader(dr);
                    var cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
                    if (cprod_id != null)
                        row.CustProduct = Cust_productsDAL.GetFromDataReader(dr);
                    result.Add(row);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Mast_products_pricing GetById(int id)
		{
			Mast_products_pricing result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM mast_products_pricing WHERE id = @id", conn);
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
		
	
		private static Mast_products_pricing GetFromDataReader(MySqlDataReader dr)
		{
			Mast_products_pricing o = new Mast_products_pricing();
		
			o.id =  (int) dr["id"];
			o.mast_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mast_id"));
			o.client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"client_id"));
			o.price_dollar = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"price_dollar"));
			o.price_euro = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"price_euro"));
			o.price_pound = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"price_pound"));
			o.update_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"update_date"));
			
			return o;

		}
		
		
		public static void Create(Mast_products_pricing o)
        {
            string insertsql = @"INSERT INTO mast_products_pricing (mast_id,client_id,price_dollar,price_euro,price_pound,update_date) VALUES(@mast_id,@client_id,@price_dollar,@price_euro,@price_pound,@update_date)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM mast_products_pricing WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Mast_products_pricing o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
			cmd.Parameters.AddWithValue("@client_id", o.client_id);
			cmd.Parameters.AddWithValue("@price_dollar", o.price_dollar);
			cmd.Parameters.AddWithValue("@price_euro", o.price_euro);
			cmd.Parameters.AddWithValue("@price_pound", o.price_pound);
			cmd.Parameters.AddWithValue("@update_date", o.update_date);
		}
		
		public static void Update(Mast_products_pricing o)
		{
			string updatesql = @"UPDATE mast_products_pricing SET mast_id = @mast_id,client_id = @client_id,price_dollar = @price_dollar,price_euro = @price_euro,price_pound = @price_pound,update_date = @update_date WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM mast_products_pricing WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			