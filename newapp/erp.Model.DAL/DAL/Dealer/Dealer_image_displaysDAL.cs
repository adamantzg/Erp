
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Dealer_image_displaysDAL
	{
	
		public static List<Dealer_image_displays> GetAll()
		{
			var result = new List<Dealer_image_displays>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dealer_image_displays", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static Dealer_image_displays GetById(int id, int web_unique)
        {
            Dealer_image_displays result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dealer_image_displays WHERE image_id = @id AND web_unique = @web_unique", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

//        public static List<Dealer_image_displays> GetForDealer(int id)
//        {
//            var result = new List<Dealer_image_displays>();
//            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
//            {
//                conn.Open();
//                var cmd = Utils.GetCommand(@"SELECT web_products2.*,dealer_images.*,dealer_image_displays.* 
//                                            FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
//                                            INNER JOIN web_products2 ON dealer_image_displays.web_unique = web_products2.web_unique                                            
//                                            WHERE dealer_images.dealer_id = @dealer_id", conn);
//                cmd.Parameters.AddWithValue("@dealer_id", id);
//                var dr = cmd.ExecuteReader();
//                while (dr.Read())
//                {
                    
//                    var did = new Dealer_image_displays();
//                    did.image_id = (int) dr["image_id"];
//                    did.ProductNew = Web_prProductsDAL.GetProductFromReader(dr);
//                    did.Product.Components = WebProductsDAL.GetComponents(did.Product, dr, true);
//                    did.qty = Utilities.FromDbValue<int>(dr["qty"]);
//                    did.web_unique = did.Product.web_unique;
//                    result.Add(did);
                    
//                }
//                dr.Close();
//            }
//            return result;
//        }

        public static List<Dealer_image_displays> GetForDealerNew(int id)
        {
            var result = new List<Dealer_image_displays>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                    var cmd = Utils.GetCommand(@"SELECT web_product_new.*,dealer_images.*,dealer_image_displays.* FROM dealer_image_displays INNER JOIN dealer_images 
                                                ON dealer_image_displays.image_id = dealer_images.image_unique INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique                                            
                                                WHERE dealer_images.dealer_id = @dealer_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    var did = new Dealer_image_displays();
                    did.image_id = (int)dr["image_id"];
                    did.ProductNew = Web_product_newDAL.GetFromDataReaderEx(dr);
                    
                    did.qty = Utilities.FromDbValue<int>(dr["qty"]);
                    did.web_unique = did.ProductNew.web_unique;
                    result.Add(did);

                }
                dr.Close();
                var sites = Web_siteDAL.GetAll();
                foreach (var di in result)
                {
                    di.ProductNew.Components = Web_product_componentDAL.GetForProduct(di.web_unique,conn);
                    di.ProductNew.SelectedCategories = Web_categoryDAL.GetForProduct(di.web_unique, conn);
                    di.ProductNew.WebSite = sites.FirstOrDefault(s => s.id == di.ProductNew.web_site_id);
                    di.ProductNew.WebFiles = Web_product_fileDAL.GetForProduct(di.web_unique, conn);
                }
            }
            return result;
        }


        private static Dealer_image_displays GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_image_displays o = new Dealer_image_displays();
		
			o.image_id =  (int) dr["image_id"];
			o.web_unique =  (int) dr["web_unique"];
			o.qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"qty"));
            o.claimed = Utilities.FromDbValue<int>(dr["claimed"]);
			
			return o;

		}
		
		
		public static void Create(Dealer_image_displays o,int? userId = null)
        {
            string insertsql = @"INSERT INTO dealer_image_displays (image_id,web_unique,qty,claimed) VALUES(@image_id,@web_unique,@qty,@claimed)";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                var cmd = Utils.GetCommand(insertsql, conn,tr);

                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT dealer_id FROM dealer_images WHERE image_unique = @image_id";
                cmd.Parameters.AddWithValue("@image_id", o.image_id);
                var dealer_id = (int) cmd.ExecuteScalar();

                var act = new Dealer_displays_activity
                    {
                        datecreated = DateTime.Now,
                        dealer_id = dealer_id,
                        new_qty = o.qty,
                        useruser_id = userId,
                        web_unique = o.web_unique
                    };
                Dealer_displays_activityDAL.Create(act,tr);

                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();   
                throw;
            }
				
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_image_displays o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@image_id", o.image_id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@qty", o.qty);
            cmd.Parameters.AddWithValue("@claimed", o.claimed);
		}
		
		public static void Update(Dealer_image_displays o)
		{
            string updatesql = @"UPDATE dealer_image_displays SET qty = @qty, claimed = @claimed WHERE web_unique = @web_unique AND image_id = @image_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int web_unique, int image_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM dealer_image_displays WHERE web_unique = @web_unique AND image_id = @image_id" , conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.Parameters.AddWithValue("@image_id", image_id);
                cmd.ExecuteNonQuery();
            }
		}


        public static int GetCountForProduct(int prodId)
        {
            int result = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    Utils.GetCommand(
                        @"SELECT Count(DISTINCT dealer_images.dealer_id) FROM dealer_image_displays INNER JOIN dealer_images ON dealer_images.image_unique = dealer_image_displays.image_id
                            INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id WHERE dealer_image_displays.web_unique = @web_unique ", conn);
                cmd.Parameters.AddWithValue("@web_unique", prodId);
                int? res = Utilities.FromDbValue<int>(cmd.ExecuteScalar());
                if (res != null)
                    result = res.Value;

            }
            return result;
        }

        public static Dealer_image_displays GetByImageAndProd(int imageId, int webUnique)
        {
            Dealer_image_displays result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dealer_image_displays WHERE image_id = @image_id AND web_unique = @web_unique", conn);
                cmd.Parameters.AddWithValue("@image_id", imageId);
                cmd.Parameters.AddWithValue("@web_unique", webUnique);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			