
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Category_dopDAL
	{
	
		public static List<Category_dop> GetAll()
		{
			var result = new List<Category_dop>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM category_dop", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Category_dop GetById(int id)
		{
			Category_dop result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM category_dop WHERE category_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Characteristics = Catdop_characteristicsDAL.GetForCategory(result.category_id);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Category_dop GetFromDataReader(MySqlDataReader dr)
		{
			Category_dop o = new Category_dop();
		
			o.category_id =  (int) dr["category_id"];
			o.category_name = string.Empty + Utilities.GetReaderField(dr,"category_name");
			o.en_standard = string.Empty + Utilities.GetReaderField(dr,"en_standard");
			o.avcp_system = string.Empty + Utilities.GetReaderField(dr,"avcp_system");
			o.intended_use = string.Empty + Utilities.GetReaderField(dr,"intended_use");
			
			return o;

		}
		
		
		public static void Create(Category_dop o)
        {
            string insertsql = @"INSERT INTO category_dop (category_id,category_name,en_standard,avcp_system,intended_use) VALUES(@category_id,@category_name,@en_standard,@avcp_system,@intended_use)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(category_id)+1 FROM category_dop", conn);
                o.category_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Category_dop o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@category_id", o.category_id);
			cmd.Parameters.AddWithValue("@category_name", o.category_name);
			cmd.Parameters.AddWithValue("@en_standard", o.en_standard);
			cmd.Parameters.AddWithValue("@avcp_system", o.avcp_system);
			cmd.Parameters.AddWithValue("@intended_use", o.intended_use);
		}
		
		public static void Update(Category_dop o)
		{
			string updatesql = @"UPDATE category_dop SET category_name = @category_name,en_standard = @en_standard,avcp_system = @avcp_system,intended_use = @intended_use WHERE category_id = @category_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int category_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM category_dop WHERE category_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", category_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			