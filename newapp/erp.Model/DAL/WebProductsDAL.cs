using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class WebProductsDAL
    {
        public static List<WebProduct> GetWebProducts(int subcatId, string language_id = null, string option_name = "", bool alwaysShowProductsWithNoOption = false)
        {
            var result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string sql = GetSelectClause(language_id != null);
                sql += @" WHERE web_sub_category = @subcatid AND 
                        ((@option <> '' AND (
                                                (@alwaysShow = 1 AND COALESCE(web_products2.option_name,'') = '')  OR web_products2.option_name = @option
                                            )
                            ) 
                            OR 
                            (@option = '' AND parent_id IS NULL)
                        ) AND web_status <> 2 
                        ORDER BY seq, web_seq";
                var cmd = new MySqlCommand(sql , conn);
                cmd.Parameters.Add(new MySqlParameter("@subcatid", subcatId));
                cmd.Parameters.Add(new MySqlParameter("@alwaysShow", alwaysShowProductsWithNoOption ? 1 : 0));
                cmd.Parameters.AddWithValue("@option", option_name);
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var p = GetProductFromReader(dr);
                    p.Components = GetComponents(p, dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> GetWebProductsForCat(int catId)
        {
            var result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                
                var sql = @" SELECT * FROM web_products2 WHERE web_category = @catid  AND web_status <> 2 
                        ORDER BY seq, web_seq";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@catid", catId));
                
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetProductFromReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        private static string GetSelectClause(bool localize = false)
        {
            if (!localize)
                return "SELECT * FROM web_products2 ";
            else
                return string.Format(@"SELECT web_products2.*, {0}  FROM
                        web_products2  {1}", GetTranslationFields(false), GetTranslationJoin());

        }

        private static string GetTranslationFields(bool productOnly = true)
        {
            return @"web_products_translate.web_name AS web_name_t,
                            web_products_translate.web_description AS web_description_t,
                            web_products_translate.web_pic_notes AS web_pic_notes_t,
                            web_products_translate.web_details AS web_details_t,
                            web_products_translate.tech_finishes AS tech_finishes_t,
                            web_products_translate.tech_product_type AS tech_product_type_t,
                            web_products_translate.tech_construction AS tech_construction_t,
                            web_products_translate.tech_material AS tech_material_t,
                            web_products_translate.tech_basin_size AS tech_basin_size_t ,
                            web_products_translate.tech_overall_height AS tech_overall_height_t,
                            web_products_translate.tech_tap_holes AS tech_tap_holes_t,
                            web_products_translate.tech_fixing AS tech_fixing_t,
                            web_products_translate.tech_compliance1 AS tech_compliance1_t,
                            web_products_translate.tech_compliance2 AS tech_compliance2_t,
                            web_products_translate.tech_compliance3 AS tech_compliance3_t,
                            web_products_translate.tech_compliance4 AS tech_compliance4_t,
                            web_products_translate.tech_compliance5 AS tech_compliance5_t,
                            web_products_translate.tech_additional1 AS tech_additional1_t,
                            web_products_translate.tech_additional2 AS tech_additional2_t,
                            web_products_translate.tech_additional3 AS tech_additional3_t,
                            web_products_translate.tech_additional4 AS tech_additional4_t,
                            web_products_translate.tech_additional5 AS tech_additional5_t,
                            web_products_translate.tech_additional6 AS tech_additional6_t,
                            web_products_translate.tech_additional7 AS tech_additional7_t,
                            web_products_translate.tech_additional8 AS tech_additional8_t,
                            web_products_translate.tech_additional9 AS tech_additional9_t,
                            web_products_translate.tech_additional10 AS tech_additional10_t,
                            web_products_translate.tech_additional11 AS tech_additional11_t,
                            web_products_translate.bar01 AS bar01_t,
                            web_products_translate.bar02 AS bar02_t,
                            web_products_translate.bar05 AS bar05_t,
                            web_products_translate.bar10 AS bar10_t,
                            web_products_translate.bar20 AS bar20_t,
                            web_products_translate.bar30 AS bar30_t,
                            web_products_translate.combination_comments AS combination_comments_t,
                            web_products_translate.tech_water_volume_note AS tech_water_volume_note_t,
                            web_products_translate.option_name AS option_name_t" + (productOnly ? "" : 
                            @",brand_categories_translate.web_description AS category_name_t,
                            brand_categories_sub_translate.brand_sub_desc AS brand_sub_desc_t,
                            brand_categories_sub_translate.pricing_note AS pricing_note_t,
                            brand_categories_sub_sub_translate.brand_sub_sub_desc AS brand_sub_sub_desc_t
                                ");
        }


        public static List<WebProduct> GetWebProductsForSubSubCat(int subsubcatid, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSelectClause(language_id != null);
                sql += "WHERE web_sub_sub_category = @subsubcatid  AND web_status <> 2 ORDER BY seq, web_seq";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@subsubcatid", subsubcatid));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    p.Components = GetComponents(p, dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> GetAll(string websiteId, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql;
                 if (language_id == null)
                    sql = @"SELECT web_products2.*, brand_categories_sub.seq AS sub_seq, " + GetInstructionsSql() + "  FROM web_products2 LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id ";
                else
                    sql = string.Format(@"SELECT web_products2.*,{2}, {0}, brand_categories_sub.seq AS sub_seq FROM
                        web_products2  {1} LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id ", GetTranslationFields(false), GetTranslationJoin(), GetInstructionsSql() );

                sql += " WHERE web_site = @site AND web_status <> 2 ORDER BY web_seq2,sub_seq, web_seq";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@site", websiteId));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> GetAllMinimal(string websiteId, string lang)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                var sql = string.Format(@"SELECT web_products.*, {0} FROM
                        web_products LEFT OUTER JOIN web_products_translate ON (web_products.web_unique = web_products_translate.web_unique AND web_products_translate.lang = @lang) ", GetTranslationFields());

                sql += " WHERE web_site = @site  AND web_status <> 2";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@site", websiteId));
                cmd.Parameters.AddWithValue("@lang", lang);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr,true,true);
                    p.NonLocalized = GetProductFromReader(dr, false,true);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        private static string GetInstructionsSql()
        {
            return
                @"(CASE WHEN comp1_instructions IS NOT NULL OR comp2_instructions IS NOT NULL OR comp3_instructions IS NOT NULL OR comp4_instructions IS NOT NULL OR comp5_instructions IS NOT NULL OR comp6_instructions IS NOT NULL 
                    OR comp7_instructions IS NOT NULL OR comp8_instructions IS NOT NULL OR comp9_instructions IS NOT NULL OR comp10_instructions IS NOT NULL THEN 1 ELSE 0 END) AS instructions ";
        }

        public static WebProduct GetWebProduct(int id, bool includeChildren = false, string language_id = null)
        {
            WebProduct result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string sql = GetSelectClauseSingle(language_id != null);
                sql += " WHERE web_products2.web_unique = @id  AND web_status <> 2 ";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetProductFromReader(dr);
                    result.Components = GetComponents(result,dr);
                }
                
                dr.Close();
                if (result != null && includeChildren)
                    result.Children = GetChildren(result.web_unique, language_id);
            }
            return result;
        }

        /// <summary>
        /// Get full component objects
        /// </summary>
        /// <param name="prod"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        internal static List<Cust_products> GetComponents(WebProduct prod, MySqlDataReader dr, bool minimal = false)
        {
            List<Cust_products> result = new List<Cust_products>();
            List<int> ids = new List<int>();
            //Components collection
            for (int i = 0; i < Properties.Settings.Default.MaxComponents; i++)
            {
                int? compid = Utilities.FromDbValue<int>(dr["web_component" + (i + 1).ToString()]);
                if (!minimal)
                {
                    if (compid != null && compid > 0)
                    {
                        ids.Add(compid.Value);
                        //Cust_products comp = new Cust_products();
                        //comp.cprod_id = compid.Value;
                        //comp.cprod_code1 = string.Empty + dr[string.Format("comp{0}_code", i + 1)];
                        //comp.cprod_name = Utilities.CheckLocalized(dr, string.Format("comp{0}_name", i + 1));
                        //comp.cprod_retail = Utilities.FromDbValue<double>(dr[string.Format("comp{0}_retail", i + 1)]);
                        //comp.cprod_pdf1 = string.Empty + dr[string.Format("comp{0}_pdf", i + 1)];
                        //comp.cprod_instructions = string.Empty + dr[string.Format("comp{0}_instructions", i + 1)];
                        //comp.Products = new List<WebProduct>();
                        //comp.Products.Add(p);
                        //p.Components.Add(comp);
                    }
                }
                else
                {
                    if (compid != null && compid > 0)
                    {
                        var code = string.Empty + dr[string.Format("comp{0}_code", i + 1)];
                        var price = Utilities.FromDbValue<double>(dr[string.Format("comp{0}_retail", i + 1)]);
                        var name = string.Empty + dr[string.Format("comp{0}_name", i + 1)];
                        result.Add(new Cust_products
                            {
                                cprod_id = compid.Value,
                                cprod_code1 = code,
                                cprod_name = name,
                                cprod_retail = price
                            });
                    }
                }
            }

            if (minimal)
                return result;
            else
            {
                if (ids.Count > 0)
                {
                    var components = Cust_productsDAL.GetForIds(ids);

                    foreach (var id in ids)
                    {
                        var component = components.FirstOrDefault(c => c.cprod_id == id);
                        if (component.Products == null)
                        {
                            component.Products = new List<WebProduct>();
                            component.Products.Add(prod);
                        }
                        result.Add(component);
                    }
                    return result;
                }
                else
                    return new List<Cust_products>();
            }
        }



        private static string GetSelectClauseSingle(bool localize = false)
        {
            return GetSelect(@"SELECT web_products2.*,{0} COALESCE(brand_categories_sub.option_component,0) AS sub_option_component, brand_categories_sub.pricing_note,
                                                    (CASE WHEN brand_categories_sub_sub.display_type IS NOT NULL THEN brand_categories_sub_sub.display_type ELSE COALESCE(brand_categories_sub.display_type, 0) END) AS display_type 
                                                     FROM web_products2 LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id LEFT OUTER JOIN 
                                                  brand_categories_sub_sub ON web_products2.web_sub_sub_category = brand_categories_sub_sub.brand_sub_sub_id {1}", localize,false, true, true);
                         
            
            
        }

        private static string GetTranslationJoin()
        {
            return @" LEFT OUTER JOIN web_products_translate ON (web_products2.web_unique = web_products_translate.web_unique AND web_products_translate.lang = @lang)
                      LEFT OUTER JOIN brand_categories_translate ON (web_products2.web_category = brand_categories_translate.brand_cat_id AND brand_categories_translate.lang = @lang)
                      LEFT OUTER JOIN brand_categories_sub_translate ON (brand_categories_sub_translate.brand_sub_id = web_products2.web_sub_category 
                                    AND brand_categories_sub_translate.lang = @lang)
                      LEFT OUTER JOIN brand_categories_sub_sub_translate ON (brand_categories_sub_sub_translate.brand_sub_sub_id = web_products2.web_sub_sub_category 
                                    AND brand_categories_sub_sub_translate.lang = @lang)
                        ";
        }

        private static string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true, bool componentLocalization = false)
        {
            List<string> fields = new List<string>();
            string join = string.Empty;
            if (localize)
            {
                fields.Add(GetTranslationFields(false));
                join = GetTranslationJoin();
            }
            //if (localize)
            //{
            //    fields = GetTranslationFields();
            //    //fields = (commaBeforeFields ? ", ": "") + GetTranslationFields() + (commaAfterFields ? "," : "");
            //    join = GetTranslationJoin();
            //}
            if (localize && componentLocalization)
            {
                for (int i = 0; i < Properties.Settings.Default.MaxComponents; i++)
                {
                    fields.Add(string.Format("cpt_{0}.cprod_name AS comp{0}_name_t", i + 1));
                    join += string.Format(" LEFT OUTER JOIN cust_products_translate cpt_{0} ON (web_products2.web_component{0} = cpt_{0}.cprod_id AND cpt_{0}.lang = @lang)", i + 1);
                }
            }

            return string.Format(initialSql, (commaBeforeFields && fields.Count > 0 ? ", " : "") + string.Join(",", fields.ToArray()) + (commaAfterFields && fields.Count > 0 ? "," : ""), join);
        }


        public static List<WebProduct> GetWebProducts(List<int> ids, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSelectClauseSingle(language_id != null);
                sql += "  WHERE web_products2.web_unique IN ({0}) AND web_status <> 2 ";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("", conn);
                string paramstring = Utilities.CreateParametersFromIdList(cmd, ids);
                cmd.CommandText = string.Format(sql, paramstring);
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    p.Components = GetComponents(p, dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> GetProductsForDealer(string website, int dealer_id, int? subcat_id = null, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSelect(@"SELECT web_products2.*, {0} brand_categories_sub.seq AS sub_seq FROM web_products2 INNER JOIN dealer_displays ON web_products2.web_unique = dealer_displays.web_unique 
                                                    LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id {1}
                                                    WHERE (web_products2.web_site = @website OR @website = '') AND  dealer_displays.client_id = @dealer_id AND (web_products2.web_sub_category = @sub_id OR @sub_id IS NULL)  AND web_status <> 2 ORDER BY web_seq2, sub_seq,brand_sub_desc",
                                        language_id != null);
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@dealer_id", dealer_id));
                cmd.Parameters.AddWithValue("@website", website);
                cmd.Parameters.Add(new MySqlParameter("@sub_id", subcat_id == null ? (object) DBNull.Value : subcat_id.Value));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

               

        public static List<WebProduct> GetChildren(int product_id, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSelect("SELECT web_products2.* {0} FROM web_products2 {1} WHERE parent_id = @id  AND web_status <> 2 ", language_id != null, true, false, true);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", product_id);
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    p.Components = GetComponents(p, dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> SearchProducts(string website, string searchText, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                MySqlCommand cmd = new MySqlCommand(GetSelect(@"SELECT web_products2.*, {0} brand_categories_sub.seq AS sub_seq FROM web_products2 LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id {1}
                                                    WHERE web_site = @website  AND web_status <> 2 AND (web_products2.web_name LIKE @text OR web_products2.web_code LIKE @text ", language_id != null), conn);
                for (int i = 1; i <= Properties.Settings.Default.MaxComponents; i++)
                {
                    cmd.CommandText += string.Format(" OR comp{0}_code LIKE @text", i);
                }
                cmd.CommandText += ")"; //close and clause
                cmd.CommandText += " ORDER BY web_seq2, sub_seq,brand_sub_desc, web_seq, web_code,web_name ASC";
                conn.Open();
                cmd.Parameters.Add(new MySqlParameter("@website", website));
                cmd.Parameters.Add(new MySqlParameter("@text", "%" + searchText + "%"));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            
            return result;
        }

        public static List<WebProduct> GetRelatedProducts(int id, string language_id = null)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelect(@"SELECT web_products2.*, {0} brand_categories.brand_cat_image, brand_categories_sub.sub_image1 AS brand_sub_image, brand_categories_sub.same_id AS sub_same_id FROM web_products2 INNER JOIN web_products_recommended ON web_products2.web_unique = web_products_recommended.comp_product INNER JOIN 
                                                    brand_categories ON web_products2.web_category = brand_categories.brand_cat_id LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id {1}
                                                    WHERE web_products_recommended.web_unique2 = @id AND web_products2.parent_id IS NULL  AND web_status <> 2  ORDER BY seq, web_seq", language_id != null), conn);
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> GetGalleryProducts(string website, string language_id = null)
        { 
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelect("SELECT web_products2.* {0} FROM web_products2 {1} WHERE web_site = @website  AND web_status <> 2 AND parent_id IS NULL AND web_image1d IS NOT NULL AND web_image1d <> ''", language_id != null, true, false), conn);
                cmd.Parameters.Add(new MySqlParameter("@website", website));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<string> GetProductOptionsForSubCat(int subcat_id, string language_id = null)
        {
            List<string> result = new List<string>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT(option_name) AS 'option' FROM web_products2 WHERE option_name <> '' AND web_sub_category = @subcatid  AND web_status <> 2 ORDER BY 'option'", conn);
                cmd.Parameters.Add(new MySqlParameter("@subcatid", subcat_id));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Empty + dr["option"]);
                }
                dr.Close();
            }
            return result;
        }

        public static List<WebProduct> GetForBrand(string brand_code, string pattern)
        {
            List<WebProduct> result = new List<WebProduct>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT web_products2.* FROM web_products2 WHERE web_site = @website AND (web_name LIKE @pattern OR web_code LIKE @pattern OR @pattern = '') ", conn);
                cmd.Parameters.AddWithValue("@website", brand_code);
                cmd.Parameters.AddWithValue("@pattern","%" + pattern + "%");
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WebProduct p = GetProductFromReader(dr);
                    p.Components = GetComponents(p, dr, true);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static WebProduct GetProductFromReader(MySqlDataReader dr, bool checkLocalized = true, bool productOnly = false)
        {
            WebProduct p = new WebProduct();
            p.web_unique = (int) dr["web_unique"];
            p.web_name = checkLocalized ? Utilities.CheckLocalized(dr,"web_name") : string.Empty + dr["web_name"];
            p.web_site = checkLocalized ? Utilities.CheckLocalized(dr,"web_site") : string.Empty + dr["web_site"];
            p.web_description = checkLocalized ? Utilities.CheckLocalized(dr,"web_description") : string.Empty + dr["web_description"];
            p.web_code = checkLocalized ? Utilities.CheckLocalized(dr,"web_code") : string.Empty + dr["web_code"];
            p.web_category = Utilities.FromDbValue<int>(dr["web_category"]);
            p.web_sub_category = Utilities.FromDbValue<int>(dr["web_sub_category"]);
            p.web_sub_sub_category = Utilities.FromDbValue<int>(dr["web_sub_sub_category"]);
            p.web_image1 = string.Empty + dr["web_image1"];
            p.web_image1b = string.Empty + dr["web_image1b"];
            p.web_image1d = string.Empty + dr["web_image1d"];
            p.web_image2 = string.Empty + dr["web_image2"];
            p.web_image3 = string.Empty + dr["web_image3"];
            p.web_image3b = string.Empty + dr["web_image3b"];
            //These fields are momentarily not in view web_products2
            //p.web_image3c = string.Empty + dr["web_image3c"];
            //p.web_image4 = string.Empty + dr["web_image4"];
            p.hi_res = string.Empty + dr["hi_res"];
            p.image_width = Utilities.FromDbValue<int>(dr["image_width"]);
            p.image_widthb = Utilities.FromDbValue<int>(dr["image_widthb"]);
            p.image_widthd = Utilities.FromDbValue<int>(dr["image_widthd"]);
            p.image_height = Utilities.FromDbValue<int>(dr["image_height"]);
            p.image_heightb = Utilities.FromDbValue<int>(dr["image_heightb"]);
            p.image_heightd = Utilities.FromDbValue<int>(dr["image_heightd"]);
            p.web_pic_notes = checkLocalized ? Utilities.CheckLocalized(dr,"web_pic_notes") : string.Empty + dr["web_pic_notes"];
            p.web_price = Utilities.FromDbValue<double>(dr["web_price"]);
            p.web_details = checkLocalized ? Utilities.CheckLocalized(dr,"web_details") : string.Empty + dr["web_details"];
            p.web_component1 = Utilities.FromDbValue<int>(dr["web_component1"]);
            p.web_component2 = Utilities.FromDbValue<int>(dr["web_component2"]);
            p.web_component3 = Utilities.FromDbValue<int>(dr["web_component3"]);
            p.web_component4 = Utilities.FromDbValue<int>(dr["web_component4"]);
            p.web_component5 = Utilities.FromDbValue<int>(dr["web_component5"]);
            p.web_component6 = Utilities.FromDbValue<int>(dr["web_component6"]);
            p.web_component7 = Utilities.FromDbValue<int>(dr["web_component7"]);
            p.web_component8 = Utilities.FromDbValue<int>(dr["web_component8"]);
            p.web_component9 = Utilities.FromDbValue<int>(dr["web_component9"]);
            p.web_component10 = Utilities.FromDbValue<int>(dr["web_component10"]);
            p.web_component11 = Utilities.FromDbValue<int>(dr["web_component11"]);
            p.web_component12 = Utilities.FromDbValue<int>(dr["web_component12"]);
            p.web_component13 = Utilities.FromDbValue<int>(dr["web_component13"]);
            p.web_component14 = Utilities.FromDbValue<int>(dr["web_component14"]);
            p.web_component15 = Utilities.FromDbValue<int>(dr["web_component15"]);
            //p.link1 = Utilities.FromDbValue<int>(dr["link1"]);
            //p.link2 = Utilities.FromDbValue<int>(dr["link2"]);
            //p.link3 = Utilities.FromDbValue<int>(dr["link3"]);
            //p.link4 = Utilities.FromDbValue<int>(dr["link4"]);
            //p.link5 = Utilities.FromDbValue<int>(dr["link5"]);
            p.web_seq = Utilities.FromDbValue<int>(dr["web_seq"]);
            p.product_weight = Utilities.FromDbValue<double>(dr["product_weight"]);
            p.bath_volume = Utilities.FromDbValue<double>(dr["bath_volume"]);
            p.tech_finishes = checkLocalized ? Utilities.CheckLocalized(dr,"tech_finishes") : string.Empty + dr["tech_finishes"];
            p.tech_product_type = checkLocalized ? Utilities.CheckLocalized(dr,"tech_product_type") : string.Empty + dr["tech_product_type"];
            p.tech_construction = checkLocalized ? Utilities.CheckLocalized(dr,"tech_construction") : string.Empty + dr["tech_construction"];
            p.tech_material = checkLocalized ? Utilities.CheckLocalized(dr,"tech_material") : string.Empty + dr["tech_material"];
            p.tech_basin_size = checkLocalized ? Utilities.CheckLocalized(dr,"tech_basin_size") : string.Empty + dr["tech_basin_size"];
            p.tech_overall_height = checkLocalized ? Utilities.CheckLocalized(dr,"tech_overall_height") : string.Empty + dr["tech_overall_height"];
            p.tech_tap_holes = checkLocalized ? Utilities.CheckLocalized(dr,"tech_tap_holes") : string.Empty + dr["tech_tap_holes"];
            p.tech_fixing = checkLocalized ? Utilities.CheckLocalized(dr,"tech_fixing") : string.Empty + dr["tech_fixing"];
            p.tech_compliance1 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_compliance1") : string.Empty + dr["tech_compliance1"];
            p.tech_compliance2 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_compliance2") : string.Empty + dr["tech_compliance2"];
            p.tech_compliance3 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_compliance3") : string.Empty + dr["tech_compliance3"];
            p.tech_compliance4 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_compliance4") : string.Empty + dr["tech_compliance4"];
            p.tech_compliance5 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_compliance5") : string.Empty + dr["tech_compliance5"]; 
            
            p.tech_additional1 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional1") : string.Empty + dr["tech_additional1"];
            p.tech_additional2 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional2") : string.Empty + dr["tech_additional2"];
            p.tech_additional3 = checkLocalized ?  Utilities.CheckLocalized(dr,"tech_additional3") : string.Empty + dr["tech_additional3"];
            p.tech_additional4 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional4") : string.Empty + dr["tech_additional4"];
            p.tech_additional5 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional5") : string.Empty + dr["tech_additional5"];
            p.tech_additional6 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional6") : string.Empty + dr["tech_additional6"];
            p.tech_additional7 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional7") : string.Empty + dr["tech_additional7"];
            p.tech_additional8 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional8") : string.Empty + dr["tech_additional8"];
            p.tech_additional9 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional9") : string.Empty + dr["tech_additional9"];
            p.tech_additional10 = checkLocalized ? Utilities.CheckLocalized(dr,"tech_additional10") : string.Empty + dr["tech_additional10"];
            p.tech_additional11 = checkLocalized ? Utilities.CheckLocalized(dr, "tech_additional11") : string.Empty + dr["tech_additional11"];
            //p.web_auto = Utilities.FromDbValue<double>(dr["web_auto"]);
            p.guarantee = Utilities.FromDbValue<int>(dr["guarantee"]);
            //p.bar01 = string.Empty + dr["bar01"];
            //p.bar02 = string.Empty + dr["bar02"];
            //p.bar05 = string.Empty + dr["bar05"];
            //p.bar10 = string.Empty + dr["bar10"];
            //p.bar20 = string.Empty + dr["bar20"];
            //p.bar30 = string.Empty + dr["bar30"];
            p.handset02 = Utilities.FromDbValue<double>(dr["handset02"]);
            p.handset05 = Utilities.FromDbValue<double>(dr["handset05"]);
            p.handset10 = Utilities.FromDbValue<double>(dr["handset10"]);
            p.handset20 = Utilities.FromDbValue<double>(dr["handset20"]);
            p.handset30 = Utilities.FromDbValue<double>(dr["handset30"]);
            p.rose02 = Utilities.FromDbValue<double>(dr["rose02"]);
            p.rose05 = Utilities.FromDbValue<double>(dr["rose05"]);
            p.rose10 = Utilities.FromDbValue<double>(dr["rose10"]);
            p.rose20 = Utilities.FromDbValue<double>(dr["rose20"]);
            p.rose30 = Utilities.FromDbValue<double>(dr["rose30"]);
            p.spout02 = Utilities.FromDbValue<double>(dr["spout02"]);
            p.spout05 = Utilities.FromDbValue<double>(dr["spout05"]);
            p.spout10 = Utilities.FromDbValue<double>(dr["spout10"]);
            p.spout20 = Utilities.FromDbValue<double>(dr["spout20"]);
            p.spout30 = Utilities.FromDbValue<double>(dr["spout30"]);
            p.Spout02Aerator = Utilities.FromDbValue<double>(dr["spout02_aerator"]);
            p.Spout05Aerator = Utilities.FromDbValue<double>(dr["spout05_aerator"]);
            p.Spout10Aerator = Utilities.FromDbValue<double>(dr["spout10_aerator"]);
            p.Spout20Aerator = Utilities.FromDbValue<double>(dr["spout20_aerator"]);
            p.Spout30Aerator = Utilities.FromDbValue<double>(dr["spout30_aerator"]);

            if (!productOnly)
            {
                p.flow02 = Utilities.FromDbValue<double>(dr["flow02"]);
                p.flow05 = Utilities.FromDbValue<double>(dr["flow05"]);
                p.flow10 = Utilities.FromDbValue<double>(dr["flow10"]);
                p.flow20 = Utilities.FromDbValue<double>(dr["flow20"]);
                p.flow30 = Utilities.FromDbValue<double>(dr["flow30"]);

                p.aerator02 = Utilities.FromDbValue<double>(dr["aerator02"]);
                p.aerator05 = Utilities.FromDbValue<double>(dr["aerator05"]);
                p.aerator10 = Utilities.FromDbValue<double>(dr["aerator10"]);
                p.aerator20 = Utilities.FromDbValue<double>(dr["aerator20"]);
                p.aerator30 = Utilities.FromDbValue<double>(dr["aerator30"]);
            }

            p.brand_group = Utilities.FromDbValue<int>(dr["brand_group"]);
            p.gold_code = string.Empty + dr["gold_code"];
            p.web_status = Utilities.FromDbValue<int>(dr["web_status"]);
            p.overflow_class = Utilities.FromDbValue<int>(dr["overflow_class"]);
            p.overflow_rate = Utilities.FromDbValue<double>(dr["overflow_rate"]);
            p.combination_comments = checkLocalized ? Utilities.CheckLocalized(dr,"combination_comments") : string.Empty + dr["combination_comments"];
            //p.tech_water_volume_note = string.Empty + dr["tech_water_volume_note"];
            //p.image_gallery = Utilities.FromDbValue<int>(dr["image_gallery"]);
            if (!productOnly)
            {
                p.brand_sub_sub_desc = Utilities.CheckLocalized(dr, "brand_sub_sub_desc");
                p.brand_sub_sub_sub_desc = Utilities.CheckLocalized(dr, "brand_sub_sub_sub_desc");
                p.brand_sub_desc = Utilities.CheckLocalized(dr, "brand_sub_desc");
                p.category_name = Utilities.CheckLocalized(dr, "category_name");
                if (Utilities.ColumnExists(dr, "brand_cat_image"))
                    p.brand_cat_image = string.Empty + dr["brand_cat_image"];
                if (Utilities.ColumnExists(dr, "brand_sub_image"))
                    p.brand_sub_image = string.Empty + dr["brand_sub_image"];
                if (Utilities.ColumnExists(dr, "display_type"))
                    p.display_type = Convert.ToInt32(Utilities.FromDbValue<long>(dr["display_type"]));
                if (Utilities.ColumnExists(dr, "sub_same_id"))
                    p.sub_same_id = Utilities.FromDbValue<int>(dr["sub_same_id"]);
                if (Utilities.ColumnExists(dr, "sub_option_component"))
                {
                    long? option_component = Utilities.FromDbValue<long>(dr["sub_option_component"]);
                    if (option_component != null)
                        p.sub_option_component = Convert.ToInt32(option_component.Value);
                }
                if (Utilities.ColumnExists(dr, "pricing_note"))
                    p.pricing_note = checkLocalized
                                         ? Utilities.CheckLocalized(dr, "pricing_note")
                                         : string.Empty + dr["pricing_note"];
            }
            p.parent_id = Utilities.FromDbValue<int>(dr["parent_id"]);
            p.option_name = Utilities.CheckLocalized(dr,"option_name");
            p.override_height = Utilities.FromDbValue<double>(dr["override_height"]);
            p.override_width = Utilities.FromDbValue<double>(dr["override_width"]);
            p.override_length = Utilities.FromDbValue<double>(dr["override_length"]);
            p.sale_on = Utilities.FromDbValue<int>(dr["sale_on"]);
            

            p.TechCompliances = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string value = Utilities.CheckLocalized(dr,string.Format("tech_compliance{0}", i + 1));
                if (!string.IsNullOrEmpty(value))
                    p.TechCompliances.Add(value);
            }
            p.TechAdditionalInfo = new List<string>();
            for (int i = 0; i < 11; i++)
            {
                string value = Utilities.CheckLocalized(dr,string.Format("tech_additional{0}", i + 1));
                if (!string.IsNullOrEmpty(value))
                    p.TechAdditionalInfo.Add(value);
            }
            if (Utilities.ColumnExists(dr, "instructions"))
            {
                p.instructions = Convert.ToInt32(dr["instructions"]);
            }

            return p;
        }

        public static void Create(WebProduct o)
        {
            string insertsql = @"INSERT INTO web_products (web_name,web_site,web_description,web_code,web_category,web_sub_category,web_sub_sub_category,web_sub_sub_sub_category,web_image1,web_image1b,web_image1d,web_image2,web_image3,web_image3b,web_image3c,web_image4,hi_res,image_width,image_widthb,image_widthd,image_height,image_heightb,image_heightd,web_pic_notes,web_price,web_details,web_component1,web_component2,web_component3,web_component4,web_component5,web_component6,web_component7,web_component8,web_component9,web_component10,web_component11,web_component12,web_component13,web_component14,web_component15,link1,link2,link3,link4,link5,link6,web_seq,product_weight,bath_volume,tech_finishes,tech_product_type,tech_construction,tech_material,tech_basin_size,tech_overall_height,tech_tap_holes,tech_fixing,tech_compliance1,tech_compliance2,tech_compliance3,tech_compliance4,tech_compliance5,tech_additional1,tech_additional2,tech_additional3,tech_additional4,tech_additional5,tech_additional6,tech_additional7,tech_additional8,tech_additional9,tech_additional10,tech_additional11,web_auto,guarantee,bar01,bar02,bar05,bar10,bar20,bar30,handset02,handset05,handset10,handset20,handset30,rose02,rose05,rose10,rose20,rose30,spout02,spout05,spout10,spout20,spout30,brand_group,gold_code,web_status,overflow_class,overflow_rate,combination_comments,tech_water_volume_note,image_gallery,parent_id,option_name,override_length,override_width,override_height) VALUES(@web_name,@web_site,@web_description,@web_code,@web_category,@web_sub_category,@web_sub_sub_category,@web_sub_sub_sub_category,@web_image1,@web_image1b,@web_image1d,@web_image2,@web_image3,@web_image3b,@web_image3c,@web_image4,@hi_res,@image_width,@image_widthb,@image_widthd,@image_height,@image_heightb,@image_heightd,@web_pic_notes,@web_price,@web_details,@web_component1,@web_component2,@web_component3,@web_component4,@web_component5,@web_component6,@web_component7,@web_component8,@web_component9,@web_component10,@web_component11,@web_component12,@web_component13,@web_component14,@web_component15,@link1,@link2,@link3,@link4,@link5,@link6,@web_seq,@product_weight,@bath_volume,@tech_finishes,@tech_product_type,@tech_construction,@tech_material,@tech_basin_size,@tech_overall_height,@tech_tap_holes,@tech_fixing,@tech_compliance1,@tech_compliance2,@tech_compliance3,@tech_compliance4,@tech_compliance5,@tech_additional1,@tech_additional2,@tech_additional3,@tech_additional4,@tech_additional5,@tech_additional6,@tech_additional7,@tech_additional8,@tech_additional9,@tech_additional10,@tech_additional11,@web_auto,@guarantee,@bar01,@bar02,@bar05,@bar10,@bar20,@bar30,@handset02,@handset05,@handset10,@handset20,@handset30,@rose02,@rose05,@rose10,@rose20,@rose30,@spout02,@spout05,@spout10,@spout20,@spout30,@brand_group,@gold_code,@web_status,@overflow_class,@overflow_rate,@combination_comments,@tech_water_volume_note,@image_gallery,@parent_id,@option_name,@override_length,@override_width,@override_height)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT web_unique FROM web_products WHERE web_unique = LAST_INSERT_ID()";
                o.web_unique = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, WebProduct o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
            cmd.Parameters.AddWithValue("@web_name", o.web_name);
            cmd.Parameters.AddWithValue("@web_site", o.web_site);
            cmd.Parameters.AddWithValue("@web_description", o.web_description);
            cmd.Parameters.AddWithValue("@web_code", o.web_code);
            cmd.Parameters.AddWithValue("@web_category", o.web_category);
            cmd.Parameters.AddWithValue("@web_sub_category", o.web_sub_category);
            cmd.Parameters.AddWithValue("@web_sub_sub_category", o.web_sub_sub_category);
            cmd.Parameters.AddWithValue("@web_sub_sub_sub_category", o.web_sub_sub_sub_category);
            cmd.Parameters.AddWithValue("@web_image1", o.web_image1);
            cmd.Parameters.AddWithValue("@web_image1b", o.web_image1b);
            cmd.Parameters.AddWithValue("@web_image1d", o.web_image1d);
            cmd.Parameters.AddWithValue("@web_image2", o.web_image2);
            cmd.Parameters.AddWithValue("@web_image3", o.web_image3);
            cmd.Parameters.AddWithValue("@web_image3b", o.web_image3b);
            cmd.Parameters.AddWithValue("@web_image3c", o.web_image3c);
            cmd.Parameters.AddWithValue("@web_image4", o.web_image4);
            cmd.Parameters.AddWithValue("@hi_res", o.hi_res);
            cmd.Parameters.AddWithValue("@image_width", o.image_width);
            cmd.Parameters.AddWithValue("@image_widthb", o.image_widthb);
            cmd.Parameters.AddWithValue("@image_widthd", o.image_widthd);
            cmd.Parameters.AddWithValue("@image_height", o.image_height);
            cmd.Parameters.AddWithValue("@image_heightb", o.image_heightb);
            cmd.Parameters.AddWithValue("@image_heightd", o.image_heightd);
            cmd.Parameters.AddWithValue("@web_pic_notes", o.web_pic_notes);
            cmd.Parameters.AddWithValue("@web_price", o.web_price);
            cmd.Parameters.AddWithValue("@web_details", o.web_details);
            cmd.Parameters.AddWithValue("@web_component1", o.web_component1);
            cmd.Parameters.AddWithValue("@web_component2", o.web_component2);
            cmd.Parameters.AddWithValue("@web_component3", o.web_component3);
            cmd.Parameters.AddWithValue("@web_component4", o.web_component4);
            cmd.Parameters.AddWithValue("@web_component5", o.web_component5);
            cmd.Parameters.AddWithValue("@web_component6", o.web_component6);
            cmd.Parameters.AddWithValue("@web_component7", o.web_component7);
            cmd.Parameters.AddWithValue("@web_component8", o.web_component8);
            cmd.Parameters.AddWithValue("@web_component9", o.web_component9);
            cmd.Parameters.AddWithValue("@web_component10", o.web_component10);
            cmd.Parameters.AddWithValue("@web_component11", o.web_component11);
            cmd.Parameters.AddWithValue("@web_component12", o.web_component12);
            cmd.Parameters.AddWithValue("@web_component13", o.web_component13);
            cmd.Parameters.AddWithValue("@web_component14", o.web_component14);
            cmd.Parameters.AddWithValue("@web_component15", o.web_component15);
            cmd.Parameters.AddWithValue("@link1", o.link1);
            cmd.Parameters.AddWithValue("@link2", o.link2);
            cmd.Parameters.AddWithValue("@link3", o.link3);
            cmd.Parameters.AddWithValue("@link4", o.link4);
            cmd.Parameters.AddWithValue("@link5", o.link5);
            cmd.Parameters.AddWithValue("@link6", o.link6);
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
            cmd.Parameters.AddWithValue("@web_auto", o.web_auto);
            cmd.Parameters.AddWithValue("@guarantee", o.guarantee);
            cmd.Parameters.AddWithValue("@bar01", o.bar01);
            cmd.Parameters.AddWithValue("@bar02", o.bar02);
            cmd.Parameters.AddWithValue("@bar05", o.bar05);
            cmd.Parameters.AddWithValue("@bar10", o.bar10);
            cmd.Parameters.AddWithValue("@bar20", o.bar20);
            cmd.Parameters.AddWithValue("@bar30", o.bar30);
            cmd.Parameters.AddWithValue("@handset02", o.handset02);
            cmd.Parameters.AddWithValue("@handset05", o.handset05);
            cmd.Parameters.AddWithValue("@handset10", o.handset10);
            cmd.Parameters.AddWithValue("@handset20", o.handset20);
            cmd.Parameters.AddWithValue("@handset30", o.handset30);
            cmd.Parameters.AddWithValue("@rose02", o.rose02);
            cmd.Parameters.AddWithValue("@rose05", o.rose05);
            cmd.Parameters.AddWithValue("@rose10", o.rose10);
            cmd.Parameters.AddWithValue("@rose20", o.rose20);
            cmd.Parameters.AddWithValue("@rose30", o.rose30);
            cmd.Parameters.AddWithValue("@spout02", o.spout02);
            cmd.Parameters.AddWithValue("@spout05", o.spout05);
            cmd.Parameters.AddWithValue("@spout10", o.spout10);
            cmd.Parameters.AddWithValue("@spout20", o.spout20);
            cmd.Parameters.AddWithValue("@spout30", o.spout30);
            cmd.Parameters.AddWithValue("@brand_group", o.brand_group);
            cmd.Parameters.AddWithValue("@gold_code", o.gold_code);
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
        }

        public static void Update(WebProduct o)
        {
            string updatesql = @"UPDATE web_products SET web_name = @web_name,web_site = @web_site,web_description = @web_description,web_code = @web_code,web_category = @web_category,web_sub_category = @web_sub_category,web_sub_sub_category = @web_sub_sub_category,web_sub_sub_sub_category = @web_sub_sub_sub_category,web_image1 = @web_image1,web_image1b = @web_image1b,web_image1d = @web_image1d,web_image2 = @web_image2,web_image3 = @web_image3,web_image3b = @web_image3b,web_image3c = @web_image3c,web_image4 = @web_image4,hi_res = @hi_res,image_width = @image_width,image_widthb = @image_widthb,image_widthd = @image_widthd,image_height = @image_height,image_heightb = @image_heightb,image_heightd = @image_heightd,web_pic_notes = @web_pic_notes,web_price = @web_price,web_details = @web_details,web_component1 = @web_component1,web_component2 = @web_component2,web_component3 = @web_component3,web_component4 = @web_component4,web_component5 = @web_component5,web_component6 = @web_component6,web_component7 = @web_component7,web_component8 = @web_component8,web_component9 = @web_component9,web_component10 = @web_component10,web_component11 = @web_component11,web_component12 = @web_component12,web_component13 = @web_component13,web_component14 = @web_component14,web_component15 = @web_component15,link1 = @link1,link2 = @link2,link3 = @link3,link4 = @link4,link5 = @link5,link6 = @link6,web_seq = @web_seq,product_weight = @product_weight,bath_volume = @bath_volume,tech_finishes = @tech_finishes,tech_product_type = @tech_product_type,tech_construction = @tech_construction,tech_material = @tech_material,tech_basin_size = @tech_basin_size,tech_overall_height = @tech_overall_height,tech_tap_holes = @tech_tap_holes,tech_fixing = @tech_fixing,tech_compliance1 = @tech_compliance1,tech_compliance2 = @tech_compliance2,tech_compliance3 = @tech_compliance3,tech_compliance4 = @tech_compliance4,tech_compliance5 = @tech_compliance5,tech_additional1 = @tech_additional1,tech_additional2 = @tech_additional2,tech_additional3 = @tech_additional3,tech_additional4 = @tech_additional4,tech_additional5 = @tech_additional5,tech_additional6 = @tech_additional6,tech_additional7 = @tech_additional7,tech_additional8 = @tech_additional8,tech_additional9 = @tech_additional9,tech_additional10 = @tech_additional10,tech_additional11 = @tech_additional11,web_auto = @web_auto,guarantee = @guarantee,bar01 = @bar01,bar02 = @bar02,bar05 = @bar05,bar10 = @bar10,bar20 = @bar20,bar30 = @bar30,handset02 = @handset02,handset05 = @handset05,handset10 = @handset10,handset20 = @handset20,handset30 = @handset30,rose02 = @rose02,rose05 = @rose05,rose10 = @rose10,rose20 = @rose20,rose30 = @rose30,spout02 = @spout02,spout05 = @spout05,spout10 = @spout10,spout20 = @spout20,spout30 = @spout30,brand_group = @brand_group,gold_code = @gold_code,web_status = @web_status,overflow_class = @overflow_class,overflow_rate = @overflow_rate,combination_comments = @combination_comments,tech_water_volume_note = @tech_water_volume_note,image_gallery = @image_gallery,parent_id = @parent_id,option_name = @option_name,override_length = @override_length,override_width = @override_width,override_height = @override_height WHERE web_unique = @web_unique";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }
        
    }
}
