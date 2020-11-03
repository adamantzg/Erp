
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Dealer_image_displaysDAL
	{
	
		public static List<Dealer_image_displays> GetAll()
		{
			var result = new List<Dealer_image_displays>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_image_displays", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static Dealer_image_displays GetById(int id)
        {
            Dealer_image_displays result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_image_displays WHERE web_unique = @id", conn);
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

        public static List<Dealer_image_displays> GetForDealer(int id)
        {
            var result = new List<Dealer_image_displays>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT web_products2.*,dealer_images.*,dealer_image_displays.* 
                                            FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                                            INNER JOIN web_products2 ON dealer_image_displays.web_unique = web_products2.web_unique
                                            WHERE dealer_images.dealer_id = @dealer_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    
                    var did = new Dealer_image_displays();
                    did.image_id = (int) dr["image_id"];
                    did.Product = WebProductsDAL.GetProductFromReader(dr);
                    did.Product.Components = WebProductsDAL.GetComponents(did.Product, dr, true);
                    did.qty = Utilities.FromDbValue<int>(dr["qty"]);
                    did.web_unique = did.Product.web_unique;
                    result.Add(did);
                    
                }
                dr.Close();
            }
            return result;
        }


        private static Dealer_image_displays GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_image_displays o = new Dealer_image_displays();
		
			o.image_id =  (int) dr["image_id"];
			o.web_unique =  (int) dr["web_unique"];
			o.qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"qty"));
			
			return o;

		}
		
		
		public static void Create(Dealer_image_displays o)
        {
            string insertsql = @"INSERT INTO dealer_image_displays (image_id,web_unique,qty) VALUES(@image_id,@web_unique,@qty)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand(insertsql, conn);
                
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_image_displays o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@image_id", o.image_id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@qty", o.qty);
		}
		
		public static void Update(Dealer_image_displays o)
		{
			string updatesql = @"UPDATE dealer_image_displays SET qty = @qty WHERE web_unique = @web_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int web_unique, int image_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM dealer_image_displays WHERE web_unique = @web_unique AND image_id = @image_id" , conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.Parameters.AddWithValue("@image_id", image_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			