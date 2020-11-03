
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class change_notice_tableDAL
	{
	
		public static List<change_notice_table> GetAll()
		{
			var result = new List<change_notice_table>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM 2011_change_notice_table", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static change_notice_table GetById(int id)
		{
			change_notice_table result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM 2011_change_notice_table WHERE cn_id = @id", conn);
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
		
	
		public static change_notice_table GetFromDataReader(MySqlDataReader dr)
		{
			change_notice_table o = new change_notice_table();
		
			o.cn_id =  (int) dr["cn_id"];
			o.cn_category = string.Empty + Utilities.GetReaderField(dr,"cn_category");
			o.cn_reason = string.Empty + Utilities.GetReaderField(dr,"cn_reason");
			o.cn_details = string.Empty + Utilities.GetReaderField(dr,"cn_details");
			o.cn_estimated_timeline = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"cn_estimated_timeline"));
			o.cn_before_image = string.Empty + Utilities.GetReaderField(dr,"cn_before_image");
			o.cn_after_image = string.Empty + Utilities.GetReaderField(dr,"cn_after_image");
			o.cn_status = string.Empty + Utilities.GetReaderField(dr,"cn_status");
			o.cn_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"cn_date"));
			o.cn_progress_update = string.Empty + Utilities.GetReaderField(dr,"cn_progress_update");
			o.cn_client = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_client"));
			o.cn_factory = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_factory"));
			o.cn_drawing = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_drawing"));
			o.cn_instruction = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_instruction"));
			o.cn_label = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_label"));
			o.cn_packing = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_packing"));
			o.cn_photo = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_photo"));
			o.cn_confirm_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"cn_confirm_date"));
			o.cn_add_id = string.Empty + Utilities.GetReaderField(dr,"cn_add_id");
			o.cn_price = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_price"));
			o.cn_progress = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cn_progress"));
			o.cn_client_2 = string.Empty + Utilities.GetReaderField(dr,"cn_client_2");
			o.cn_category_2 = string.Empty + Utilities.GetReaderField(dr,"cn_category_2");
			o.cn_reason_2 = string.Empty + Utilities.GetReaderField(dr,"cn_reason_2");
			o.cn_nonr_reason = string.Empty + Utilities.GetReaderField(dr,"cn_nonr_reason");
			
			return o;

		}
		
		
		public static void Create(change_notice_table o)
        {
            string insertsql = @"INSERT INTO 2011_change_notice_table (cn_category,cn_reason,cn_details,cn_estimated_timeline,cn_before_image,cn_after_image,cn_status,cn_date,cn_progress_update,cn_client,cn_factory,cn_drawing,cn_instruction,cn_label,cn_packing,cn_photo,cn_confirm_date,cn_add_id,cn_price,cn_progress,cn_client_2,cn_category_2,cn_reason_2,cn_nonr_reason) VALUES(@cn_category,@cn_reason,@cn_details,@cn_estimated_timeline,@cn_before_image,@cn_after_image,@cn_status,@cn_date,@cn_progress_update,@cn_client,@cn_factory,@cn_drawing,@cn_instruction,@cn_label,@cn_packing,@cn_photo,@cn_confirm_date,@cn_add_id,@cn_price,@cn_progress,@cn_client_2,@cn_category_2,@cn_reason_2,@cn_nonr_reason)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT cn_id FROM 2011_change_notice_table WHERE cn_id = LAST_INSERT_ID()";
                o.cn_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, change_notice_table o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@cn_id", o.cn_id);
			cmd.Parameters.AddWithValue("@cn_category", o.cn_category);
			cmd.Parameters.AddWithValue("@cn_reason", o.cn_reason);
			cmd.Parameters.AddWithValue("@cn_details", o.cn_details);
			cmd.Parameters.AddWithValue("@cn_estimated_timeline", o.cn_estimated_timeline);
			cmd.Parameters.AddWithValue("@cn_before_image", o.cn_before_image);
			cmd.Parameters.AddWithValue("@cn_after_image", o.cn_after_image);
			cmd.Parameters.AddWithValue("@cn_status", o.cn_status);
			cmd.Parameters.AddWithValue("@cn_date", o.cn_date);
			cmd.Parameters.AddWithValue("@cn_progress_update", o.cn_progress_update);
			cmd.Parameters.AddWithValue("@cn_client", o.cn_client);
			cmd.Parameters.AddWithValue("@cn_factory", o.cn_factory);
			cmd.Parameters.AddWithValue("@cn_drawing", o.cn_drawing);
			cmd.Parameters.AddWithValue("@cn_instruction", o.cn_instruction);
			cmd.Parameters.AddWithValue("@cn_label", o.cn_label);
			cmd.Parameters.AddWithValue("@cn_packing", o.cn_packing);
			cmd.Parameters.AddWithValue("@cn_photo", o.cn_photo);
			cmd.Parameters.AddWithValue("@cn_confirm_date", o.cn_confirm_date);
			cmd.Parameters.AddWithValue("@cn_add_id", o.cn_add_id);
			cmd.Parameters.AddWithValue("@cn_price", o.cn_price);
			cmd.Parameters.AddWithValue("@cn_progress", o.cn_progress);
			cmd.Parameters.AddWithValue("@cn_client_2", o.cn_client_2);
			cmd.Parameters.AddWithValue("@cn_category_2", o.cn_category_2);
			cmd.Parameters.AddWithValue("@cn_reason_2", o.cn_reason_2);
			cmd.Parameters.AddWithValue("@cn_nonr_reason", o.cn_nonr_reason);
		}
		
		public static void Update(change_notice_table o)
		{
            string updatesql = @"UPDATE 2011_change_notice_table SET cn_category = @cn_category,cn_reason = @cn_reason,cn_details = @cn_details,cn_estimated_timeline = @cn_estimated_timeline,cn_before_image = @cn_before_image,cn_after_image = @cn_after_image,cn_status = @cn_status,cn_date = @cn_date,cn_progress_update = @cn_progress_update,cn_client = @cn_client,cn_factory = @cn_factory,cn_drawing = @cn_drawing,cn_instruction = @cn_instruction,cn_label = @cn_label,cn_packing = @cn_packing,cn_photo = @cn_photo,cn_confirm_date = @cn_confirm_date,cn_add_id = @cn_add_id,cn_price = @cn_price,cn_progress = @cn_progress,cn_client_2 = @cn_client_2,cn_category_2 = @cn_category_2,cn_reason_2 = @cn_reason_2,cn_nonr_reason = @cn_nonr_reason WHERE cn_id = @cn_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int cn_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM 2011_change_notice_table WHERE cn_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", cn_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<change_notice_table> GetByProductPo(string factory_ref, string po)
        {
            var result = new List<change_notice_table>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT 2011_change_notice_table.* FROM
                                2011_change_notice_table WHERE EXISTS( SELECT  2011_change_notice_product_table.cn_id
                                FROM 2011_change_notice_product_table INNER JOIN mast_products ON mast_products.mast_id = 2011_change_notice_product_table.mastid
                                WHERE 2011_change_notice_product_table.cn_id = 2011_change_notice_table.cn_id AND mast_products.factory_ref = @factory_ref AND 
                                2011_change_notice_product_table.product_po = @po)",conn);
                cmd.Parameters.AddWithValue("@factory_ref", factory_ref);
                cmd.Parameters.AddWithValue("@po", po);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();

            }
            return result;
        }
		
	}
}
			
			