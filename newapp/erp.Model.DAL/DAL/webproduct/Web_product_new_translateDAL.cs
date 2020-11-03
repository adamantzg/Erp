
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_new_translateDAL
	{
	
		public static List<Web_product_new_translate> GetAll()
		{
			var result = new List<Web_product_new_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_new_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}


        public static List<Web_product_new_translate> GetByProduct(int web_unique)
		{
            var result = new List<Web_product_new_translate>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_new_translate WHERE web_unique = @id", conn);
				cmd.Parameters.AddWithValue("@id", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
			return result;
		}
		
	
		public static Web_product_new_translate GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_new_translate o = new Web_product_new_translate();
		
			o.web_unique =  (int) dr["web_unique"];
			o.language_id =  (int) dr["language_id"];
			o.web_name = string.Empty + Utilities.GetReaderField(dr,"web_name");
			o.web_description = string.Empty + Utilities.GetReaderField(dr,"web_description");
			o.web_code = string.Empty + Utilities.GetReaderField(dr,"web_code");
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
			o.gold_code = string.Empty + Utilities.GetReaderField(dr,"gold_code");
			o.combination_comments = string.Empty + Utilities.GetReaderField(dr,"combination_comments");
			o.tech_water_volume_note = string.Empty + Utilities.GetReaderField(dr,"tech_water_volume_note");
			o.option_name = string.Empty + Utilities.GetReaderField(dr,"option_name");
			
			return o;

		}
		
		
		public static void Create(Web_product_new_translate o)
        {
            string insertsql = @"INSERT INTO web_product_new_translate (web_unique,language_id,web_name,web_description,web_code,web_pic_notes,web_details,tech_finishes,tech_product_type,tech_construction,tech_material,tech_basin_size,tech_overall_height,tech_tap_holes,tech_fixing,gold_code,combination_comments,tech_water_volume_note,option_name) VALUES(@web_unique,@language_id,@web_name,@web_description,@web_code,@web_pic_notes,@web_details,@tech_finishes,@tech_product_type,@tech_construction,@tech_material,@tech_basin_size,@tech_overall_height,@tech_tap_holes,@tech_fixing,@gold_code,@combination_comments,@tech_water_volume_note,@option_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand(insertsql, conn);
                
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_new_translate o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@language_id", o.language_id);
			cmd.Parameters.AddWithValue("@web_name", o.web_name);
			cmd.Parameters.AddWithValue("@web_description", o.web_description);
			cmd.Parameters.AddWithValue("@web_code", o.web_code);
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
			cmd.Parameters.AddWithValue("@gold_code", o.gold_code);
			cmd.Parameters.AddWithValue("@combination_comments", o.combination_comments);
			cmd.Parameters.AddWithValue("@tech_water_volume_note", o.tech_water_volume_note);
			cmd.Parameters.AddWithValue("@option_name", o.option_name);
		}
		
		public static void Update(Web_product_new_translate o)
		{
			string updatesql = @"UPDATE web_product_new_translate SET web_name = @web_name,web_description = @web_description,web_code = @web_code,web_pic_notes = @web_pic_notes,web_details = @web_details,tech_finishes = @tech_finishes,tech_product_type = @tech_product_type,tech_construction = @tech_construction,tech_material = @tech_material,tech_basin_size = @tech_basin_size,tech_overall_height = @tech_overall_height,tech_tap_holes = @tech_tap_holes,tech_fixing = @tech_fixing,gold_code = @gold_code,combination_comments = @combination_comments,tech_water_volume_note = @tech_water_volume_note,option_name = @option_name WHERE language_id = @language_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int language_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_product_new_translate WHERE language_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", language_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			