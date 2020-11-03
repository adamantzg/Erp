using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class BrandGroupDAL
    {
        public static List<BrandGroup> GetBrandGroups(int brand_id, string language_id = null)
        {
            List<BrandGroup> result = new List<BrandGroup>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = GetSelect(@"SELECT brand_grouping.* {0} FROM brand_grouping {1} WHERE brand = @brandid", language_id != null);
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@brandid", brand_id));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BrandGroup bg = GetBrandGroupFromReader(dr);
                    result.Add(bg);
                }
                dr.Close();
            }
            return result;
        }

        public static BrandGroup GetById(int id)
        {
            BrandGroup result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_grouping WHERE brand_group = @id", conn);
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

        private static BrandGroup GetFromDataReader(MySqlDataReader dr)
        {
            var o = new BrandGroup();

            o.brand_group = (int)dr["brand_group"];
            o.brand = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "brand"));
            o.group_desc = string.Empty + Utilities.GetReaderField(dr, "group_desc");

            return o;

        }

        private static string GetSelect(string sql, bool localize = false)
        {
            string fields=string.Empty, join = string.Empty;
            if (localize)
            {
                fields = ", brand_grouping_translate.group_desc AS group_desc_t";
                join = " LEFT OUTER JOIN brand_grouping_translate ON (brand_grouping.brand_group = brand_grouping_translate.brand_group AND brand_grouping_translate.lang = @lang)";
            }
            return string.Format(sql, fields, join);
        }

        public static List<BrandGroup> GetDealerBrandGroups(int dealer_id, string language_id = null)
        { 
            List<BrandGroup> result = new List<BrandGroup>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelect(@"SELECT brand_grouping.* {0} FROM brand_grouping INNER JOIN web_products ON brand_grouping.brand_group = web_products.brand_group 
                                                            INNER JOIN dealer_displays ON web_products.web_unique = dealer_displays.web_unique {1} 
                                                            WHERE dealer_displays.client_id = @dealerid ",language_id != null), conn);
                cmd.Parameters.Add(new MySqlParameter("@dealerid", dealer_id));
                if (!string.IsNullOrEmpty(language_id))
                    cmd.Parameters.AddWithValue("@lang", language_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BrandGroup bg = GetBrandGroupFromReader(dr);
                    result.Add(bg);
                }
                dr.Close();
            }
            return result;
        }

        private static BrandGroup GetBrandGroupFromReader(MySqlDataReader dr)
        {
 	        BrandGroup b = new BrandGroup();
            b.brand_group = (int) dr["brand_group"];
            b.brand = Utilities.FromDbValue<int>(dr["brand"]);
            b.group_desc = Utilities.CheckLocalized(dr,"group_desc");
            return b;
        }
    }
}
