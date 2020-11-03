using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class CategoriesDAL
    {
        public static List<BrandCategory> GetBrandCategories(int brand, string language_id = null, bool filterByWebSeq = true)
        {
            List<BrandCategory> result = new List<BrandCategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                List<string> where = new List<string>();
                string sql = GetCatsSelectClause(language_id != null);
                where.Add(" brand = @brand");
                if (filterByWebSeq)
                {
                    where.Add(" web_seq > 0 AND web_seq < 99 ");
                }
                sql += " WHERE " + string.Join(" AND ", where.ToList());
                sql += " ORDER BY web_seq";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@brand",brand));
                if(!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BrandCategory cat = GetCategoryFromReader(dr);
                    result.Add(cat);
                }
                dr.Close();
            }
            return result;
        }

        public static List<BrandCategory> GetBrandCategoriesSimple(int brand_id)
        {
            var result = new List<BrandCategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = new MySqlCommand("SELECT brand_categories.*, COALESCE((SELECT COUNT(*) FROM brand_categories_sub WHERE brand_cat_id = brand_categories.brand_cat_id),0) AS childcount FROM brand_categories INNER JOIN brands ON brand_categories.brand = brands.user_id WHERE web_seq > 0 AND brand_id = @brand_id ORDER BY web_seq", conn);
                cmd.Parameters.Add(new MySqlParameter("@brand_id", brand_id));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetCategoryFromReader(dr));
                }
                dr.Close();
            }
            return result;
        }



        public static List<BrandCategory> GetBrandCategories(IList<int> brands = null)
        {
            var result = new List<BrandCategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format("SELECT * FROM brand_categories INNER JOIN brands ON brand_categories.brand = brands.user_id {0}",brands != null ? 
                    string.Format("WHERE brands.brand_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,brands)) : "");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetCategoryFromReader(dr));
                }
                dr.Close();

            }
            return result;
        }


        public static BrandCategory GetCategoryFromReader(MySqlDataReader dr)
        {
            BrandCategory cat = new BrandCategory();
            cat.brand_cat_id = (int)dr["brand_cat_id"];
            cat.brand_cat_desc = Utilities.CheckLocalized(dr, "brand_cat_desc");
            cat.brand = Utilities.FromDbValue<int>(dr["brand"]);
            cat.unit_ordering = Utilities.FromDbValue<int>(dr["unit_ordering"]);
            cat.web_description = Utilities.CheckLocalized(dr,"web_description");
            cat.brand_cat_image = string.Empty + dr["brand_cat_image"];
            cat.image_width = Utilities.FromDbValue<int>(dr["image_width"]);
            cat.image_height = Utilities.FromDbValue<int>(dr["image_height"]);
            cat.web_seq = Utilities.FromDbValue<int>(dr["web_seq"]);
            cat.unique_ordering = Utilities.FromDbValue<int>(dr["unique_ordering"]);
            cat.why_so_good = Utilities.CheckLocalized(dr,"why_so_good");
            cat.why_so_good_title = Utilities.CheckLocalized(dr,"why_so_good_title");
            cat.sale_retail_percentage = Utilities.FromDbValue<double>(dr["sale_retail_percentage"]);
            if (Utilities.ColumnExists(dr, "childcount"))
                cat.childcount = Convert.ToInt32(dr["childcount"]);
            return cat;
        }

        public static BrandCategory GetCategory(int id, bool loadSubs = false, string language_id = null)
        {
            BrandCategory result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetCatsSelectClause(language_id != null);
                sql += " WHERE brand_categories.brand_cat_id = @id";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                if(!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetCategoryFromReader(dr);
                }
                dr.Close();
                if(loadSubs)
                    result.Subcategories = GetBrandSubCategories(result.brand_cat_id);
            }
            return result;
        }

        private static string GetCatsSelectClause(bool forTranslation = false)
        {
            if (!forTranslation)
                return "SELECT * FROM brand_categories ";
            else
                return @"SELECT brand_categories.*, 
                            brand_categories_translate.web_description AS web_description_t,
                            brand_categories_translate.brand_cat_desc AS brand_cat_desc_t,
                            brand_categories_translate.why_so_good AS why_so_good_t,
                            brand_categories_translate.why_so_good_title AS why_so_good_title_t
                        FROM
                        brand_categories
                            LEFT JOIN brand_categories_translate ON (brand_categories_translate.brand_cat_id = brand_categories.brand_cat_id 
                                    AND brand_categories_translate.lang = @lang)";

        }

        private static string GetSubCatsSelectClause(bool forTranslation = false, bool productsOptionCheck = false)
        {
            string sql;
            if (!forTranslation)
            {
                sql = "SELECT brand_categories_sub.* {0} FROM brand_categories_sub ";
            }
            else
                sql = @"SELECT brand_categories_sub.*, 
                            brand_categories_sub_translate.brand_sub_desc AS brand_sub_desc_t,
                            brand_categories_sub_translate.sub_description AS sub_description_t,
                            brand_categories_sub_translate.sub_details AS sub_details_t,
                            brand_categories_sub_translate.pricing_note AS pricing_note_t,
                            brand_categories_sub_translate.group AS group_t {0}
                        FROM
                        brand_categories_sub
                            LEFT JOIN brand_categories_sub_translate ON (brand_categories_sub_translate.brand_sub_id = brand_categories_sub.brand_sub_id 
                                    AND brand_categories_sub_translate.lang = @lang)";

            if (productsOptionCheck)
                sql = string.Format(sql, ",(SELECT COALESCE(COUNT(*),0) FROM web_products WHERE COALESCE(option_name,'') <> '' AND web_sub_category = brand_categories_sub.brand_sub_id) AS  optionsCount ");
            else
                sql = string.Format(sql, string.Empty);
            return sql;
        }

        public static List<BrandSubCategory> GetBrandSubCategories(int catId, string language_id = null)
        {
            List<BrandSubCategory> result = new List<BrandSubCategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSubCatsSelectClause(language_id != null);
                sql += " WHERE brand_categories_sub.brand_cat_id = @id ORDER BY 'group',seq";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@id", catId));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BrandSubCategory cat = GetSubCategoryFromReader(dr);
                    result.Add(cat);
                }
                dr.Close();
            }
            return result;
        }

        public static List<BrandSubCategory> GetAllBrandSubCategories(int brandid, string language_id = null)
        {
            List<BrandSubCategory> result = new List<BrandSubCategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string sql = GetSubCatsSelectClause(language_id != null);
                sql +=" INNER JOIN brand_categories ON brand_categories_sub.brand_cat_id = brand_categories.brand_cat_id";
                sql += " WHERE brand = @brandid ORDER BY 'group',seq";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@brandid", brandid));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BrandSubCategory cat = GetSubCategoryFromReader(dr);
                    result.Add(cat);
                }
                dr.Close();
            }
            return result;
        }

        public static BrandSubCategory GetSubCategory(int id, string language_id = null)
        {
            BrandSubCategory result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSubCatsSelectClause(language_id != null,true);
                sql += " WHERE brand_categories_sub.brand_sub_id = @id";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetSubCategoryFromReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        private static BrandSubCategory GetSubCategoryFromReader(MySqlDataReader dr)
        {
            BrandSubCategory subcat = new BrandSubCategory();
            subcat.brand_sub_id = (int)dr["brand_sub_id"];
            subcat.brand_cat_id = Utilities.FromDbValue<int>(dr["brand_cat_id"]);
            subcat.same_id = Utilities.FromDbValue<int>(dr["same_id"]);
            subcat.brand_sub_desc = Utilities.CheckLocalized(dr,"brand_sub_desc");
            subcat.sub_description = Utilities.CheckLocalized(dr,"sub_description");
            subcat.sub_details = Utilities.CheckLocalized(dr,"sub_details");
            subcat.sub_image1 = string.Empty + dr["sub_image1"];
            subcat.guarantee = Utilities.FromDbValue<int>(dr["guarantee"]);
            subcat.image_width = Utilities.FromDbValue<int>(dr["image_width"]);
            subcat.image_height = Utilities.FromDbValue<int>(dr["image_height"]);
            subcat.seq = Utilities.FromDbValue<int>(dr["seq"]);
            subcat.group = Utilities.CheckLocalized(dr,"group");
            subcat.display_type = Utilities.FromDbValue<short>(dr["display_type"]);
            subcat.sub_details_heading = string.Empty+dr["sub_details_heading"];
            subcat.option_component = Utilities.FromDbValue<int>(dr["option_component"]);
            subcat.pricing_note = Utilities.CheckLocalized(dr,"pricing_note");
            subcat.sale_retail_percentage = Utilities.FromDbValue<double>(dr["sale_retail_percentage"]);

            if (Utilities.ColumnExists(dr, "optionsCount"))
                subcat.ProductsWithOptionsCount = Convert.ToInt32(dr["optionsCount"]);
            return subcat;
        }

    }
}
