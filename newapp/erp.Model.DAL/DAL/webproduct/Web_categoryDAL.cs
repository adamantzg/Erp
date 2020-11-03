
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_categoryDAL
	{
	
		public static List<Web_category> GetAll()
		{
			var result = new List<Web_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_category", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_category GetById(int id)
		{
			Web_category result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_category WHERE category_id = @id", conn);
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
		
	
		public static Web_category GetFromDataReader(MySqlDataReader dr)
		{
			Web_category o = new Web_category();
		
			o.category_id =  (int) dr["category_id"];
			o.brand_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand_id"));
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.alternate_name = string.Empty + Utilities.GetReaderField(dr,"alternate_name");
			o.parent_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"parent_id"));
			o.path = string.Empty + Utilities.GetReaderField(dr,"path");
			o.legacy_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"legacy_id"));
			o.title = string.Empty + Utilities.GetReaderField(dr,"title");
			o.description = string.Empty + Utilities.GetReaderField(dr,"description");
			o.sequence = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sequence"));
			o.image = string.Empty + Utilities.GetReaderField(dr,"image");
			o.display_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"display_type"));
			o.option_component = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"option_component"));
			o.pricing_note = string.Empty + Utilities.GetReaderField(dr,"pricing_note");
			o.group = string.Empty + Utilities.GetReaderField(dr,"group");
			o.sale_retail_percentage = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"sale_retail_percentage"));
			o.sibling_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sibling_id"));
			o.image_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"image_type"));
			o.hide_dimensions = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"hide_dimensions"));
            o.category_status = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "category_status"));
			
			return o;

		}
		
		
		public static void Create(Web_category o)
        {
            string insertsql = @"INSERT INTO web_category (brand_id,name,alternate_name,parent_id,path,legacy_id,title,description,sequence,image,display_type,option_component,pricing_note,'group',sale_retail_percentage,sibling_id,image_type,hide_dimensions,category_status) VALUES(@brand_id,@name,@alternate_name,@parent_id,@path,@legacy_id,@title,@description,@sequence,@image,@display_type,@option_component,@pricing_note,@group,@sale_retail_percentage,@sibling_id,@image_type,@hide_dimensions,@category_status)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT category_id FROM web_category WHERE category_id = LAST_INSERT_ID()";
                o.category_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_category o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@category_id", o.category_id);
			cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@alternate_name", o.alternate_name);
			cmd.Parameters.AddWithValue("@parent_id", o.parent_id);
			cmd.Parameters.AddWithValue("@path", o.path);
			cmd.Parameters.AddWithValue("@legacy_id", o.legacy_id);
			cmd.Parameters.AddWithValue("@title", o.title);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@sequence", o.sequence);
			cmd.Parameters.AddWithValue("@image", o.image);
			cmd.Parameters.AddWithValue("@display_type", o.display_type);
			cmd.Parameters.AddWithValue("@option_component", o.option_component);
			cmd.Parameters.AddWithValue("@pricing_note", o.pricing_note);
			cmd.Parameters.AddWithValue("@group", o.group);
			cmd.Parameters.AddWithValue("@sale_retail_percentage", o.sale_retail_percentage);
			cmd.Parameters.AddWithValue("@sibling_id", o.sibling_id);
			cmd.Parameters.AddWithValue("@image_type", o.image_type);
			cmd.Parameters.AddWithValue("@hide_dimensions", o.hide_dimensions);
            cmd.Parameters.AddWithValue("@category_status", o.category_status);
		}
		
		public static void Update(Web_category o)
		{
			string updatesql = @"UPDATE web_category SET brand_id = @brand_id,name = @name,alternate_name = @alternate_name,parent_id = @parent_id,path = @path,
                        legacy_id = @legacy_id,title = @title,description = @description,sequence = @sequence,image = @image,display_type = @display_type,
                        option_component = @option_component,pricing_note = @pricing_note,`group` = @group,sale_retail_percentage = @sale_retail_percentage,
                        sibling_id = @sibling_id,image_type = @image_type,hide_dimensions = @hide_dimensions, category_status=@category_status WHERE category_id = @category_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int category_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_category WHERE category_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", category_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			