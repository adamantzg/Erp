
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Brand_categories_sub_sub_translateDAL
	{
	
		public static List<Brand_categories_sub_sub_translate> GetAll()
		{
			var result = new List<Brand_categories_sub_sub_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM brand_categories_sub_sub_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Brand_categories_sub_sub_translate GetById(int id, string lang)
		{
			Brand_categories_sub_sub_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM brand_categories_sub_sub_translate WHERE brand_sub_sub_id = @id  AND lang = @lang", conn);
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
		
	
		private static Brand_categories_sub_sub_translate GetFromDataReader(MySqlDataReader dr)
		{
			Brand_categories_sub_sub_translate o = new Brand_categories_sub_sub_translate();
		
			o.brand_sub_sub_id =  (int) dr["brand_sub_sub_id"];
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.brand_sub_sub_desc = string.Empty + Utilities.GetReaderField(dr,"brand_sub_sub_desc");
			
			return o;

		}
		
		
		public static void Create(Brand_categories_sub_sub_translate o)
        {
            string insertsql = @"INSERT INTO brand_categories_sub_sub_translate (brand_sub_sub_id,lang,brand_sub_sub_desc) VALUES(@brand_sub_sub_id,@lang,@brand_sub_sub_desc)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Brand_categories_sub_sub_translate o, bool forInsert = true)
        {
			cmd.Parameters.AddWithValue("@brand_sub_sub_id", o.brand_sub_sub_id);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@brand_sub_sub_desc", o.brand_sub_sub_desc);
		}
		
		public static void Update(Brand_categories_sub_sub_translate o)
		{
			string updatesql = @"UPDATE brand_categories_sub_sub_translate SET brand_sub_sub_desc = @brand_sub_sub_desc WHERE brand_sub_sub_id = @brand_sub_sub_id AND lang = @lang";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id, string lang)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM brand_categories_sub_sub_translate WHERE brand_sub_sub_id = @id AND lang = @lang" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			