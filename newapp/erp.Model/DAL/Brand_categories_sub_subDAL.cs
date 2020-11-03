
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Brand_categories_sub_subDAL
	{

        public static List<Brand_categories_sub_sub> GetForSubCat(int brand_sub_id, string language_id = null)
		{
			List<Brand_categories_sub_sub> result = new List<Brand_categories_sub_sub>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSelectClause(language_id != null);
                sql += " WHERE brand_sub_id = @sub_id";
                sql += " ORDER BY brand_categories_sub_sub.seq";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@sub_id", brand_sub_id);
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        private static string GetSelectClause(bool forTranslation = false)
        {
            if (!forTranslation)
                return "SELECT * FROM brand_categories_sub_sub ";
            else
                return @"SELECT brand_categories_sub_sub.*, 
                            brand_categories_sub_sub_translate.brand_sub_sub_desc AS brand_sub_sub_desc_t
                        FROM
                        brand_categories_sub_sub
                            LEFT JOIN brand_categories_sub_sub_translate ON (brand_categories_sub_sub_translate.brand_sub_sub_id = brand_categories_sub_sub.brand_sub_sub_id 
                                    AND brand_categories_sub_sub_translate.lang = @lang)";

        }

        public static List<Brand_categories_sub_sub> GetAllForBrand(int brandid, string language_id = null)
        {
            var result = new List<Brand_categories_sub_sub>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string sql = GetSelectClause(language_id != null);
                sql += @" INNER JOIN brand_categories_sub ON brand_categories_sub_sub.brand_sub_id = brand_categories_sub.brand_sub_id 
                          INNER JOIN brand_categories ON brand_categories_sub.brand_cat_id = brand_categories.brand_cat_id";
                sql += " WHERE brand = @brandid";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                cmd.Parameters.Add(new MySqlParameter("@brandid", brandid));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var cat = GetFromDataReader(dr);
                    result.Add(cat);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Brand_categories_sub_sub GetById(int id)
		{
			Brand_categories_sub_sub result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM brand_categories_sub_sub WHERE brand_sub_sub_id = @id", conn);
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
	
		private static Brand_categories_sub_sub GetFromDataReader(MySqlDataReader dr)
		{
			Brand_categories_sub_sub o = new Brand_categories_sub_sub();
		
			o.brand_sub_sub_id =  (int) dr["brand_sub_sub_id"];
			o.brand_sub_id = Utilities.FromDbValue<int>(dr["brand_sub_id"]);
			o.brand_sub_sub_desc = Utilities.CheckLocalized(dr,"brand_sub_sub_desc");
			o.seq = Utilities.FromDbValue<int>(dr["seq"]);
			o.image1 = string.Empty + dr["image1"];
            o.display_type = Utilities.FromDbValue<int>(dr["display_type"]);
            o.sale_retail_percentage = Utilities.FromDbValue<double>(dr["sale_retail_percentage"]);
			return o;

		}
		
		public static void Create(Brand_categories_sub_sub o)
        {
            string insertsql = @"INSERT INTO brand_categories_sub_sub (brand_sub_id,brand_sub_sub_desc,seq,image1,display_type) VALUES(@brand_sub_id,@brand_sub_sub_desc,@seq,@image1,@display_type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT brand_sub_sub_id FROM brand_categories_sub_sub WHERE brand_sub_sub_id = LAST_INSERT_ID()";
                o.brand_sub_sub_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Brand_categories_sub_sub o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@brand_sub_sub_id", o.brand_sub_sub_id);
			cmd.Parameters.AddWithValue("@brand_sub_id", o.brand_sub_id);
			cmd.Parameters.AddWithValue("@brand_sub_sub_desc", o.brand_sub_sub_desc);
			cmd.Parameters.AddWithValue("@seq", o.seq);
			cmd.Parameters.AddWithValue("@image1", o.image1);
            cmd.Parameters.AddWithValue("@display_type", o.display_type);
		}
		
		public static void Update(Brand_categories_sub_sub o)
		{
            string updatesql = @"UPDATE brand_categories_sub_sub SET brand_sub_id = @brand_sub_id,brand_sub_sub_desc = @brand_sub_sub_desc,seq = @seq,image1 = @image1, display_type = @display_type WHERE brand_sub_sub_id = @brand_sub_sub_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int brand_sub_sub_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM brand_categories_sub_sub WHERE brand_sub_sub_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", brand_sub_sub_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			