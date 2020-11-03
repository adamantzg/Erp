
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Dealer_imagesDAL
	{
	
		public static List<Dealer_images> GetAll()
		{
			List<Dealer_images> result = new List<Dealer_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dealer_images", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Dealer_images> GetByDealer(int dealer_id)
        {
            List<Dealer_images> result = new List<Dealer_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT dealer_images.* FROM dealer_images WHERE dealer_id = @dealer_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                cmd.CommandText =
                    "SELECT brands.* FROM brands INNER JOIN dealer_image_brand ON brands.brand_id = dealer_image_brand.brand_id WHERE dealer_image_brand.dealer_image_id = @image_id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@image_id", 0);
                foreach (var image in result)
                {
                    cmd.Parameters["@image_id"].Value = image.image_unique;
                    dr = cmd.ExecuteReader();
                    image.Brands = new List<Brand>();
                    while (dr.Read())
                    {
                        image.Brands.Add(BrandsDAL.GetFromDataReader(dr));
                    }
                    dr.Close();
                }
            }
            return result;
        }
		
		public static Dealer_images GetById(int id)
		{
			Dealer_images result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dealer_images WHERE image_unique = @id", conn);
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

        public static List<Dealer_images> GetUnallocatedImages()
        {
            var result = new List<Dealer_images>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand("SELECT dealer_images.*,dealers.* FROM dealer_images INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id WHERE COALESCE(store_page,0) = 0 AND NOT EXISTS (SELECT * FROM dealer_image_brand WHERE dealer_image_id = dealer_images.image_unique)",
                        conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var di = GetFromDataReader(dr);
                    di.Dealer = DealerDAL.GetDealerFromReader(dr);//new Dealer {user_name = string.Empty + dr["user_name"]};
                    result.Add(di);
                }

            }
            return result;
        }
	
		public static Dealer_images GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_images o = new Dealer_images();
		
			o.image_unique =  (int) dr["image_unique"];
			o.dealer_id = Utilities.FromDbValue<int>(dr["dealer_id"]);
			o.dealer_image = string.Empty + dr["dealer_image"];
			o.seq = Utilities.FromDbValue<int>(dr["seq"]);
			o.hide = Utilities.FromDbValue<int>(dr["hide"]);
            o.reviewed = Utilities.FromDbValue<int>(dr["reviewed"]);
		    o.store_page = Utilities.BoolFromLong(dr["store_page"]);
			
			return o;

		}
		
		public static void Create(Dealer_images o, MySqlTransaction tr)
        {
            string insertsql = @"INSERT INTO dealer_images (dealer_id,dealer_image,seq,hide,reviewed,store_page) VALUES(@dealer_id,@dealer_image,@seq,@hide,@reviewed,@store_page)";
		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
			if(tr == null)
                conn.Open();
				
			MySqlCommand cmd = new MySqlCommand(insertsql, conn, tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT image_unique FROM dealer_images WHERE image_unique = LAST_INSERT_ID()";
            o.image_unique = (int) cmd.ExecuteScalar();

		    if (o.Brands != null)
		    {
                cmd.Parameters.Clear();
		        cmd.Parameters.AddWithValue("@dealer_image_id", o.image_unique);
		        cmd.Parameters.AddWithValue("@brand_id", 0);
		        cmd.CommandText =
		            "INSERT INTO dealer_image_brand(dealer_image_id, brand_id) VALUES(@dealer_image_id, @brand_id)";
		        foreach (var brand in o.Brands)
		        {
		            cmd.Parameters["@brand_id"].Value = brand.brand_id;
		            cmd.ExecuteNonQuery();
		        }
		    }

        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_images o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@image_unique", o.image_unique);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
			cmd.Parameters.AddWithValue("@dealer_image", o.dealer_image);
			cmd.Parameters.AddWithValue("@seq", o.seq);
			cmd.Parameters.AddWithValue("@hide", o.hide);
            cmd.Parameters.AddWithValue("@reviewed", o.reviewed);
		    cmd.Parameters.AddWithValue("@store_page", o.store_page);
        }
		
		public static void Update(Dealer_images o, DbTransaction tr = null)
		{
            string updatesql = @"UPDATE dealer_images SET dealer_id = @dealer_id,dealer_image = @dealer_image,seq = @seq,hide = @hide, reviewed = @reviewed,store_page = @store_page WHERE image_unique = @image_unique";

            var conn = tr != null ? (MySqlConnection) tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
            MySqlCommand cmd = new MySqlCommand(updatesql, conn,(MySqlTransaction) tr);
            BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();

            if (o.Brands != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@dealer_image_id", o.image_unique);
                cmd.CommandText = "DELETE FROM dealer_image_brand WHERE dealer_image_id = @dealer_image_id";
                cmd.ExecuteNonQuery();

                cmd.Parameters.AddWithValue("@brand_id", 0);
                cmd.CommandText =
                    "INSERT INTO dealer_image_brand(dealer_image_id, brand_id) VALUES(@dealer_image_id, @brand_id)";
                foreach (var brand in o.Brands)
                {
                    cmd.Parameters["@brand_id"].Value = brand.brand_id;
                    cmd.ExecuteNonQuery();
                }
            }
            
		}
		
		public static void Delete(int image_unique, MySqlTransaction tr)
		{
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
			MySqlCommand cmd = new MySqlCommand("DELETE FROM dealer_images WHERE image_unique = @id" , conn,tr);
            cmd.Parameters.AddWithValue("@id", image_unique);
            cmd.ExecuteNonQuery();
		}
	}
}
			
			