using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_newDAL
	{

		public static List<Web_product_new> GetAll()
		{
			var result = new List<Web_product_new>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_new", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}


		public static Web_product_new GetById(int id)
		{
			Web_product_new result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_new WHERE web_unique = @id", conn);
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


		public static Web_product_new GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_new o = new Web_product_new();

			o.web_unique =  (int) dr["web_unique"];
			o.web_name = string.Empty + Utilities.GetReaderField(dr,"web_name");
			o.web_site_id =  (int) dr["web_site_id"];
			o.web_description = string.Empty + Utilities.GetReaderField(dr,"web_description");
			o.web_code = string.Empty + Utilities.GetReaderField(dr,"web_code");
			o.hi_res = string.Empty + Utilities.GetReaderField(dr,"hi_res");
			o.web_pic_notes = string.Empty + Utilities.GetReaderField(dr,"web_pic_notes");
			o.web_details = string.Empty + Utilities.GetReaderField(dr,"web_details");
			o.web_seq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_seq"));
			o.product_weight = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"product_weight"));
			o.bath_volume = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"bath_volume"));
			o.tech_finishes = string.Empty + Utilities.GetReaderField(dr,"tech_finishes");
			o.tech_product_type = string.Empty + Utilities.GetReaderField(dr,"tech_product_type");
			o.tech_construction = string.Empty + Utilities.GetReaderField(dr,"tech_construction");
			o.tech_material = string.Empty + Utilities.GetReaderField(dr,"tech_material");
			o.tech_basin_size = string.Empty + Utilities.GetReaderField(dr,"tech_basin_size");
			o.tech_overall_height = string.Empty + Utilities.GetReaderField(dr,"tech_overall_height");
			o.tech_tap_holes = string.Empty + Utilities.GetReaderField(dr,"tech_tap_holes");
			o.tech_fixing = string.Empty + Utilities.GetReaderField(dr,"tech_fixing");
			o.web_auto = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"web_auto"));
			o.guarantee = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"guarantee"));
			o.brand_group = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand_group"));
			o.gold_code = string.Empty + Utilities.GetReaderField(dr,"gold_code");
			o.product_gold_code = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"product_gold_code"));
			o.web_status = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_status"));
			o.overflow_class = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"overflow_class"));
			o.overflow_rate = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"overflow_rate"));
			o.combination_comments = string.Empty + Utilities.GetReaderField(dr,"combination_comments");
			o.tech_water_volume_note = string.Empty + Utilities.GetReaderField(dr,"tech_water_volume_note");
			o.image_gallery = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"image_gallery"));
			o.parent_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"parent_id"));
			o.option_name = string.Empty + Utilities.GetReaderField(dr,"option_name");
            o.option_type = string.Empty + Utilities.GetReaderField(dr,"option_type");
			o.override_length = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"override_length"));
			o.override_width = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"override_width"));
			o.override_height = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"override_height"));
			o.sale_on = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sale_on"));
			o.datecreated = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"datecreated"));
			o.created_by = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"created_by"));
			o.datemodified = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"datemodified"));
			o.modified_by = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"modified_by"));
			o.legacy_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"legacy_id"));
			o.batch_no = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"batch_no"));
			o.batch_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"batch_id"));
			o.design_template = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"design_template"));
			o.sub_title_1 = string.Empty + Utilities.GetReaderField(dr,"sub_title_1");
			o.glass_thickness = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"glass_thickness"));
			o.adjustment = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"adjustment"));
			o.whitebook_batch = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"whitebook_batch"));
			o.whitebook_title = string.Empty + Utilities.GetReaderField(dr,"whitebook_title");
			o.whitebook_description = string.Empty + Utilities.GetReaderField(dr,"whitebook_description");
			o.whitebook_material = string.Empty + Utilities.GetReaderField(dr,"whitebook_material");
			o.whitebook_notes = string.Empty + Utilities.GetReaderField(dr,"whitebook_notes");
			o.web_code_override = string.Empty + Utilities.GetReaderField(dr,"web_code_override");
            o.new_product_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "new_product_flag"));
            o.show_component_dimensions = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "show_component_dimensions"));
            o.show_component_weights = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "show_component_weights"));
            o.option_header_override = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "option_header_override"));
			o.whitebook_template_id= Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "whitebook_template_id"));
            o.template_id_link= Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "template_id_link"));
            o.marketing_description = string.Empty + Utilities.GetReaderField(dr, "marketing_description");
            return o;

		}


		public static void Create(Web_product_new o)
        {
            string insertsql = @"INSERT INTO web_product_new (web_unique,web_name,web_site_id,web_description,web_code,hi_res,web_pic_notes,web_details,web_seq,product_weight,bath_volume,
                                tech_finishes,tech_product_type,tech_construction,tech_material,tech_basin_size,tech_overall_height,tech_tap_holes,tech_fixing,web_auto,guarantee,brand_group,
                                gold_code,product_gold_code,web_status,overflow_class,overflow_rate,combination_comments,tech_water_volume_note,image_gallery,parent_id,option_name,override_length,
                                override_width,override_height,sale_on,datecreated,created_by,datemodified,modified_by,legacy_id,batch_no,batch_id,design_template,sub_title_1,glass_thickness,
                                adjustment,whitebook_batch,whitebook_title,whitebook_description,whitebook_material,whitebook_notes,web_code_override,new_product_flag,show_component_dimensions,
                                show_component_weights,option_header_override) 
                                VALUES(@web_unique,@web_name,@web_site_id,
                                @web_description,@web_code,@hi_res,@web_pic_notes,@web_details,@web_seq,@product_weight,@bath_volume,@tech_finishes,@tech_product_type,@tech_construction,
                                @tech_material,@tech_basin_size,@tech_overall_height,@tech_tap_holes,@tech_fixing,@web_auto,@guarantee,@brand_group,@gold_code,@product_gold_code,@web_status,
                                @overflow_class,@overflow_rate,@combination_comments,@tech_water_volume_note,@image_gallery,@parent_id,@option_name,@override_length,@override_width,@override_height,
                                @sale_on,@datecreated,@created_by,@datemodified,@modified_by,@legacy_id,@batch_no,@batch_id,@design_template,@sub_title_1,@glass_thickness,@adjustment,
                                @whitebook_batch,@whitebook_title,@whitebook_description,@whitebook_material,@whitebook_notes,@web_code_override,@new_product_flag,@show_component_dimensions,
                                @show_component_weights,@option_header_override)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand("SELECT MAX(web_unique)+1 FROM web_product_new", conn);
                o.web_unique = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;

                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();

            }
		}

		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_new o, bool forInsert = true)
        {

			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@web_name", o.web_name);
			cmd.Parameters.AddWithValue("@web_site_id", o.web_site_id);
			cmd.Parameters.AddWithValue("@web_description", o.web_description);
			cmd.Parameters.AddWithValue("@web_code", o.web_code);
			cmd.Parameters.AddWithValue("@hi_res", o.hi_res);
			cmd.Parameters.AddWithValue("@web_pic_notes", o.web_pic_notes);
			cmd.Parameters.AddWithValue("@web_details", o.web_details);
			cmd.Parameters.AddWithValue("@web_seq", o.web_seq);
			cmd.Parameters.AddWithValue("@product_weight", o.product_weight);
			cmd.Parameters.AddWithValue("@bath_volume", o.bath_volume);
			cmd.Parameters.AddWithValue("@tech_finishes", o.tech_finishes);
			cmd.Parameters.AddWithValue("@tech_product_type", o.tech_product_type);
			cmd.Parameters.AddWithValue("@tech_construction", o.tech_construction);
			cmd.Parameters.AddWithValue("@tech_material", o.tech_material);
			cmd.Parameters.AddWithValue("@tech_basin_size", o.tech_basin_size);
			cmd.Parameters.AddWithValue("@tech_overall_height", o.tech_overall_height);
			cmd.Parameters.AddWithValue("@tech_tap_holes", o.tech_tap_holes);
			cmd.Parameters.AddWithValue("@tech_fixing", o.tech_fixing);
			cmd.Parameters.AddWithValue("@web_auto", o.web_auto);
			cmd.Parameters.AddWithValue("@guarantee", o.guarantee);
			cmd.Parameters.AddWithValue("@brand_group", o.brand_group);
			cmd.Parameters.AddWithValue("@gold_code", o.gold_code);
			cmd.Parameters.AddWithValue("@product_gold_code", o.product_gold_code);
			cmd.Parameters.AddWithValue("@web_status", o.web_status);
			cmd.Parameters.AddWithValue("@overflow_class", o.overflow_class);
			cmd.Parameters.AddWithValue("@overflow_rate", o.overflow_rate);
			cmd.Parameters.AddWithValue("@combination_comments", o.combination_comments);
			cmd.Parameters.AddWithValue("@tech_water_volume_note", o.tech_water_volume_note);
			cmd.Parameters.AddWithValue("@image_gallery", o.image_gallery);
			cmd.Parameters.AddWithValue("@parent_id", o.parent_id);
			cmd.Parameters.AddWithValue("@option_name", o.option_name);
			cmd.Parameters.AddWithValue("@override_length", o.override_length);
			cmd.Parameters.AddWithValue("@override_width", o.override_width);
			cmd.Parameters.AddWithValue("@override_height", o.override_height);
			cmd.Parameters.AddWithValue("@sale_on", o.sale_on);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_by", o.created_by);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_by", o.modified_by);
			cmd.Parameters.AddWithValue("@legacy_id", o.legacy_id);
			cmd.Parameters.AddWithValue("@batch_no", o.batch_no);
			cmd.Parameters.AddWithValue("@batch_id", o.batch_id);
			cmd.Parameters.AddWithValue("@design_template", o.design_template);
			cmd.Parameters.AddWithValue("@sub_title_1", o.sub_title_1);
			cmd.Parameters.AddWithValue("@glass_thickness", o.glass_thickness);
			cmd.Parameters.AddWithValue("@adjustment", o.adjustment);
			cmd.Parameters.AddWithValue("@whitebook_batch", o.whitebook_batch);
			cmd.Parameters.AddWithValue("@whitebook_title", o.whitebook_title);
			cmd.Parameters.AddWithValue("@whitebook_description", o.whitebook_description);
			cmd.Parameters.AddWithValue("@whitebook_material", o.whitebook_material);
			cmd.Parameters.AddWithValue("@whitebook_notes", o.whitebook_notes);
			cmd.Parameters.AddWithValue("@web_code_override", o.web_code_override);
            cmd.Parameters.AddWithValue("@new_product_flag", o.new_product_flag);
            cmd.Parameters.AddWithValue("@show_component_dimensions", o.show_component_dimensions);
            cmd.Parameters.AddWithValue("@show_component_weights", o.show_component_weights);
            cmd.Parameters.AddWithValue("@option_header_override", o.option_header_override);
		}

		public static void Update(Web_product_new o)
		{
			string updatesql = @"UPDATE web_product_new SET web_name = @web_name,web_site_id = @web_site_id,web_description = @web_description,web_code = @web_code,hi_res = @hi_res,
                                web_pic_notes = @web_pic_notes,web_details = @web_details,web_seq = @web_seq,product_weight = @product_weight,bath_volume = @bath_volume,
                                tech_finishes = @tech_finishes,tech_product_type = @tech_product_type,tech_construction = @tech_construction,tech_material = @tech_material,
                                tech_basin_size = @tech_basin_size,tech_overall_height = @tech_overall_height,tech_tap_holes = @tech_tap_holes,tech_fixing = @tech_fixing,web_auto = @web_auto,
                                guarantee = @guarantee,brand_group = @brand_group,gold_code = @gold_code,product_gold_code = @product_gold_code,web_status = @web_status,
                                overflow_class = @overflow_class,overflow_rate = @overflow_rate,combination_comments = @combination_comments,tech_water_volume_note = @tech_water_volume_note,
                                image_gallery = @image_gallery,parent_id = @parent_id,option_name = @option_name,override_length = @override_length,override_width = @override_width,
                                override_height = @override_height,sale_on = @sale_on,datecreated = @datecreated,created_by = @created_by,datemodified = @datemodified,modified_by = @modified_by,
                                legacy_id = @legacy_id,batch_no = @batch_no,batch_id = @batch_id,design_template = @design_template,sub_title_1 = @sub_title_1,glass_thickness = @glass_thickness,
                                adjustment = @adjustment,whitebook_batch = @whitebook_batch,whitebook_title = @whitebook_title,whitebook_description = @whitebook_description,
                                whitebook_material = @whitebook_material,whitebook_notes = @whitebook_notes,web_code_override = @web_code_override, new_product_flag=@new_product_flag
                                show_component_dimensions=@show_component_dimensions, show_component_weights=@show_component_weights, option_header_override=@option_header_override WHERE web_unique = @web_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}

		public static void Delete(int web_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_product_new WHERE web_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", web_unique);
                cmd.ExecuteNonQuery();
            }
		}


	}
}

