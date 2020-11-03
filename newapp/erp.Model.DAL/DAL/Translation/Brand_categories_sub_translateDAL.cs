
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Brand_categories_sub_translateDAL
	{
	
		public static List<Brand_categories_sub_translate> GetAll()
		{
			var result = new List<Brand_categories_sub_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_categories_sub_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Brand_categories_sub_translate GetById(int id, string lang)
		{
			Brand_categories_sub_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_categories_sub_translate WHERE brand_sub_id = @id AND lang = @lang", conn);
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
		
	
		private static Brand_categories_sub_translate GetFromDataReader(MySqlDataReader dr)
		{
			Brand_categories_sub_translate o = new Brand_categories_sub_translate();
		
			o.brand_sub_id =  (int) dr["brand_sub_id"];
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.brand_sub_desc = string.Empty + Utilities.GetReaderField(dr,"brand_sub_desc");
			o.sub_description = string.Empty + Utilities.GetReaderField(dr,"sub_description");
			o.sub_details = string.Empty + Utilities.GetReaderField(dr,"sub_details");
			o.pricing_note = string.Empty + Utilities.GetReaderField(dr,"pricing_note");
			o.group = string.Empty + Utilities.GetReaderField(dr,"group");
			
			return o;

		}
		
		
		public static void Create(Brand_categories_sub_translate o)
        {
            string insertsql = @"INSERT INTO brand_categories_sub_translate (brand_sub_id,lang,brand_sub_desc,sub_description,sub_details,pricing_note,`group`) VALUES(@brand_sub_id,@lang,@brand_sub_desc,@sub_description,@sub_details,@pricing_note,@group)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Brand_categories_sub_translate o, bool forInsert = true)
        {
			cmd.Parameters.AddWithValue("@brand_sub_id", o.brand_sub_id);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@brand_sub_desc", o.brand_sub_desc);
			cmd.Parameters.AddWithValue("@sub_description", o.sub_description);
			cmd.Parameters.AddWithValue("@sub_details", o.sub_details);
			cmd.Parameters.AddWithValue("@pricing_note", o.pricing_note);
			cmd.Parameters.AddWithValue("@group", o.group);
		}
		
		public static void Update(Brand_categories_sub_translate o)
		{
			string updatesql = @"UPDATE brand_categories_sub_translate SET brand_sub_desc = @brand_sub_desc,sub_description = @sub_description,sub_details = @sub_details,
                            pricing_note = @pricing_note,`group` = @group WHERE brand_sub_id = @brand_sub_id AND lang = @lang";

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
				var cmd = Utils.GetCommand("DELETE FROM brand_categories_sub_translate WHERE brand_sub_id = @id AND lang = @lang" , conn);
                cmd.Parameters.AddWithValue("@id", lang);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			