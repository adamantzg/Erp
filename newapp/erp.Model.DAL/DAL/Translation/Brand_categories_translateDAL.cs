
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Brand_categories_translateDAL
	{
	
		public static List<Brand_categories_translate> GetAll(string lang)
		{
			var result = new List<Brand_categories_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_categories_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Brand_categories_translate GetById(int id, string lang)
		{
			Brand_categories_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_categories_translate WHERE brand_cat_id = @id AND lang = @lang", conn);
				cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Brand_categories_translate GetFromDataReader(MySqlDataReader dr)
		{
			Brand_categories_translate o = new Brand_categories_translate();
		
			o.brand_cat_id =  (int) dr["brand_cat_id"];
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.web_description = string.Empty + Utilities.GetReaderField(dr,"web_description");
			o.brand_cat_desc = string.Empty + Utilities.GetReaderField(dr,"brand_cat_desc");
			o.why_so_good = string.Empty + Utilities.GetReaderField(dr,"why_so_good");
			o.why_so_good_title = string.Empty + Utilities.GetReaderField(dr,"why_so_good_title");
			
			return o;

		}
		
		
		public static void Create(Brand_categories_translate o)
        {
            string insertsql = @"INSERT INTO brand_categories_translate (brand_cat_id,lang,web_description,brand_cat_desc,why_so_good,why_so_good_title) VALUES(@brand_cat_id,@lang,@web_description,@brand_cat_desc,@why_so_good,@why_so_good_title)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Brand_categories_translate o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@brand_cat_id", o.brand_cat_id);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@web_description", o.web_description);
			cmd.Parameters.AddWithValue("@brand_cat_desc", o.brand_cat_desc);
			cmd.Parameters.AddWithValue("@why_so_good", o.why_so_good);
			cmd.Parameters.AddWithValue("@why_so_good_title", o.why_so_good_title);
		}
		
		public static void Update(Brand_categories_translate o)
		{
			string updatesql = @"UPDATE brand_categories_translate SET web_description = @web_description,brand_cat_desc = @brand_cat_desc,why_so_good = @why_so_good,
                        why_so_good_title = @why_so_good_title WHERE brand_cat_id = @brand_cat_id AND lang = @lang";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id, string lang)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM brand_categories_translate WHERE brand_cat_id = @id AND lang = @lang" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			