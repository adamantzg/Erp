
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Web_products_translateDAL
	{
	
		public static List<Web_products_translate> GetAll()
		{
			var result = new List<Web_products_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_products_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Web_products_translate> GetByProduct(int web_unique)
        {
            var result = new List<Web_products_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_products_translate WHERE web_unique = @web_unique", conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Web_products_translate GetById(int id, string lang)
		{
			Web_products_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_products_translate WHERE web_unique = @id AND lang = @lang", conn);
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
		
	
		private static Web_products_translate GetFromDataReader(MySqlDataReader dr)
		{
			Web_products_translate o = new Web_products_translate();
		
			o.web_unique =  (int) dr["web_unique"];
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.web_name = string.Empty + Utilities.GetReaderField(dr,"web_name");
			o.web_description = string.Empty + Utilities.GetReaderField(dr,"web_description");
			o.web_pic_notes = string.Empty + Utilities.GetReaderField(dr,"web_pic_notes");
			o.web_details = string.Empty + Utilities.GetReaderField(dr,"web_details");
			o.tech_finishes = string.Empty + Utilities.GetReaderField(dr,"tech_finishes");
			o.tech_product_type = string.Empty + Utilities.GetReaderField(dr,"tech_product_type");
			o.tech_construction = string.Empty + Utilities.GetReaderField(dr,"tech_construction");
			o.tech_material = string.Empty + Utilities.GetReaderField(dr,"tech_material");
			o.tech_basin_size = string.Empty + Utilities.GetReaderField(dr,"tech_basin_size");
			o.tech_overall_height = string.Empty + Utilities.GetReaderField(dr,"tech_overall_height");
			o.tech_tap_holes = string.Empty + Utilities.GetReaderField(dr,"tech_tap_holes");
			o.tech_fixing = string.Empty + Utilities.GetReaderField(dr,"tech_fixing");
			o.tech_compliance1 = string.Empty + Utilities.GetReaderField(dr,"tech_compliance1");
			o.tech_compliance2 = string.Empty + Utilities.GetReaderField(dr,"tech_compliance2");
			o.tech_compliance3 = string.Empty + Utilities.GetReaderField(dr,"tech_compliance3");
			o.tech_compliance4 = string.Empty + Utilities.GetReaderField(dr,"tech_compliance4");
			o.tech_compliance5 = string.Empty + Utilities.GetReaderField(dr,"tech_compliance5");
			o.tech_additional1 = string.Empty + Utilities.GetReaderField(dr,"tech_additional1");
			o.tech_additional2 = string.Empty + Utilities.GetReaderField(dr,"tech_additional2");
			o.tech_additional3 = string.Empty + Utilities.GetReaderField(dr,"tech_additional3");
			o.tech_additional4 = string.Empty + Utilities.GetReaderField(dr,"tech_additional4");
			o.tech_additional5 = string.Empty + Utilities.GetReaderField(dr,"tech_additional5");
			o.tech_additional6 = string.Empty + Utilities.GetReaderField(dr,"tech_additional6");
			o.tech_additional7 = string.Empty + Utilities.GetReaderField(dr,"tech_additional7");
			o.tech_additional8 = string.Empty + Utilities.GetReaderField(dr,"tech_additional8");
			o.tech_additional9 = string.Empty + Utilities.GetReaderField(dr,"tech_additional9");
			o.tech_additional10 = string.Empty + Utilities.GetReaderField(dr,"tech_additional10");
			o.tech_additional11 = string.Empty + Utilities.GetReaderField(dr,"tech_additional11");
			o.bar01 = string.Empty + Utilities.GetReaderField(dr,"bar01");
			o.bar02 = string.Empty + Utilities.GetReaderField(dr,"bar02");
			o.bar05 = string.Empty + Utilities.GetReaderField(dr,"bar05");
			o.bar10 = string.Empty + Utilities.GetReaderField(dr,"bar10");
			o.bar20 = string.Empty + Utilities.GetReaderField(dr,"bar20");
			o.bar30 = string.Empty + Utilities.GetReaderField(dr,"bar30");
			o.combination_comments = string.Empty + Utilities.GetReaderField(dr,"combination_comments");
			o.tech_water_volume_note = string.Empty + Utilities.GetReaderField(dr,"tech_water_volume_note");
			o.option_name = string.Empty + Utilities.GetReaderField(dr,"option_name");
			
			return o;

		}
		
		
		public static void Create(Web_products_translate o)
        {
            string insertsql = @"INSERT INTO web_products_translate (web_unique,lang,web_name,web_description,web_pic_notes,web_details,tech_finishes,tech_product_type,tech_construction,
            tech_material,tech_basin_size,tech_overall_height,tech_tap_holes,tech_fixing,tech_compliance1,tech_compliance2,tech_compliance3,tech_compliance4,tech_compliance5,tech_additional1,
            tech_additional2,tech_additional3,tech_additional4,tech_additional5,tech_additional6,tech_additional7,tech_additional8,tech_additional9,tech_additional10,tech_additional11,bar01,bar02,
            bar05,bar10,bar20,bar30,combination_comments,tech_water_volume_note,option_name) VALUES(@web_unique,@lang,@web_name,@web_description,@web_pic_notes,@web_details,@tech_finishes,@tech_product_type,@tech_construction,@tech_material,@tech_basin_size,@tech_overall_height,@tech_tap_holes,@tech_fixing,@tech_compliance1,@tech_compliance2,@tech_compliance3,@tech_compliance4,@tech_compliance5,@tech_additional1,@tech_additional2,@tech_additional3,@tech_additional4,@tech_additional5,@tech_additional6,@tech_additional7,@tech_additional8,@tech_additional9,@tech_additional10,@tech_additional11,@bar01,@bar02,@bar05,@bar10,@bar20,@bar30,@combination_comments,@tech_water_volume_note,@option_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_products_translate o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@web_name", o.web_name);
			cmd.Parameters.AddWithValue("@web_description", o.web_description);
			cmd.Parameters.AddWithValue("@web_pic_notes", o.web_pic_notes);
			cmd.Parameters.AddWithValue("@web_details", o.web_details);
			cmd.Parameters.AddWithValue("@tech_finishes", o.tech_finishes);
			cmd.Parameters.AddWithValue("@tech_product_type", o.tech_product_type);
			cmd.Parameters.AddWithValue("@tech_construction", o.tech_construction);
			cmd.Parameters.AddWithValue("@tech_material", o.tech_material);
			cmd.Parameters.AddWithValue("@tech_basin_size", o.tech_basin_size);
			cmd.Parameters.AddWithValue("@tech_overall_height", o.tech_overall_height);
			cmd.Parameters.AddWithValue("@tech_tap_holes", o.tech_tap_holes);
			cmd.Parameters.AddWithValue("@tech_fixing", o.tech_fixing);
			cmd.Parameters.AddWithValue("@tech_compliance1", o.tech_compliance1);
			cmd.Parameters.AddWithValue("@tech_compliance2", o.tech_compliance2);
			cmd.Parameters.AddWithValue("@tech_compliance3", o.tech_compliance3);
			cmd.Parameters.AddWithValue("@tech_compliance4", o.tech_compliance4);
			cmd.Parameters.AddWithValue("@tech_compliance5", o.tech_compliance5);
			cmd.Parameters.AddWithValue("@tech_additional1", o.tech_additional1);
			cmd.Parameters.AddWithValue("@tech_additional2", o.tech_additional2);
			cmd.Parameters.AddWithValue("@tech_additional3", o.tech_additional3);
			cmd.Parameters.AddWithValue("@tech_additional4", o.tech_additional4);
			cmd.Parameters.AddWithValue("@tech_additional5", o.tech_additional5);
			cmd.Parameters.AddWithValue("@tech_additional6", o.tech_additional6);
			cmd.Parameters.AddWithValue("@tech_additional7", o.tech_additional7);
			cmd.Parameters.AddWithValue("@tech_additional8", o.tech_additional8);
			cmd.Parameters.AddWithValue("@tech_additional9", o.tech_additional9);
			cmd.Parameters.AddWithValue("@tech_additional10", o.tech_additional10);
			cmd.Parameters.AddWithValue("@tech_additional11", o.tech_additional11);
			cmd.Parameters.AddWithValue("@bar01", o.bar01);
			cmd.Parameters.AddWithValue("@bar02", o.bar02);
			cmd.Parameters.AddWithValue("@bar05", o.bar05);
			cmd.Parameters.AddWithValue("@bar10", o.bar10);
			cmd.Parameters.AddWithValue("@bar20", o.bar20);
			cmd.Parameters.AddWithValue("@bar30", o.bar30);
			cmd.Parameters.AddWithValue("@combination_comments", o.combination_comments);
			cmd.Parameters.AddWithValue("@tech_water_volume_note", o.tech_water_volume_note);
			cmd.Parameters.AddWithValue("@option_name", o.option_name);
		}
		
		public static void Update(Web_products_translate o)
		{
			string updatesql = @"UPDATE web_products_translate SET web_name = @web_name,web_description = @web_description,web_pic_notes = @web_pic_notes,web_details = @web_details,
                            tech_finishes = @tech_finishes,tech_product_type = @tech_product_type,tech_construction = @tech_construction,tech_material = @tech_material,tech_basin_size = @tech_basin_size,
                            tech_overall_height = @tech_overall_height,tech_tap_holes = @tech_tap_holes,tech_fixing = @tech_fixing,tech_compliance1 = @tech_compliance1,
                            tech_compliance2 = @tech_compliance2,tech_compliance3 = @tech_compliance3,tech_compliance4 = @tech_compliance4,tech_compliance5 = @tech_compliance5,
                            tech_additional1 = @tech_additional1,tech_additional2 = @tech_additional2,tech_additional3 = @tech_additional3,tech_additional4 = @tech_additional4,
                            tech_additional5 = @tech_additional5,tech_additional6 = @tech_additional6,tech_additional7 = @tech_additional7,tech_additional8 = @tech_additional8,
                            tech_additional9 = @tech_additional9,tech_additional10 = @tech_additional10,tech_additional11 = @tech_additional11,bar01 = @bar01,bar02 = @bar02,bar05 = @bar05,
                            bar10 = @bar10,bar20 = @bar20,bar30 = @bar30,combination_comments = @combination_comments,tech_water_volume_note = @tech_water_volume_note,option_name = @option_name 
                            WHERE web_unique = @web_unique AND lang = @lang";

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
				var cmd = Utils.GetCommand("DELETE FROM web_products_translate WHERE web_unique = @web_unique AND lang = @lang" , conn);
                cmd.Parameters.AddWithValue("@web_unique", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			