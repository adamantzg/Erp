
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Brand_sales_analysis2DAL
	{
	
		public static List<Brand_sales_analysis2> GetAll()
		{
			var result = new List<Brand_sales_analysis2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM brand_sales_analysis2", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Brand_sales_analysis2> GetForBrand(int brand_user_id, DateTime? etdFrom)
        {
            var result = new List<Brand_sales_analysis2>();
            var options = Analytics_optionsDAL.GetAll();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT brand_sales_analysis2.*, analytics_subcategory.*,analytics_categories.* FROM brand_sales_analysis2
                                            LEFT OUTER JOIN analytics_subcategory ON brand_sales_analysis2.analytics_category = analytics_subcategory.subcat_id 
                                            LEFT OUTER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                                            WHERE (brand_user_id = @brand_user_id 
                                                     OR 
                                                  (brand_user_id IS NOT NULL 
                                                        AND EXISTS(SELECT cprod_id FROM cust_products WHERE cprod_mast = brand_sales_analysis2.cprod_mast AND brand_user_id = @brand_user_id)))
                                            AND (po_req_etd >= @etd OR @etd IS NULL)", conn);
                cmd.Parameters.AddWithValue("@brand_user_id", brand_user_id);
                cmd.Parameters.AddWithValue("@etd", etdFrom != null ? (object) etdFrom.Value : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var bs = GetFromDataReader(dr);
                    if (bs.analytics_category != null)
                    {
                        bs.Category = Analytics_categoriesDAL.GetFromDataReader(dr);
                        bs.Subcategory = Analytics_subcategoryDAL.GetFromDataReader(dr);
                    }
                    if (bs.analytics_option != null)
                        bs.Option = options.FirstOrDefault(o => o.option_id == bs.analytics_option);
                    result.Add(bs);
                }
                dr.Close();
            }
            return result;
        }

        //public static List<Brand_sales_analysis2> GetSalesForOtherBrands(int brand_user_id, DateTime? etdFrom)
		
		
	
		private static Brand_sales_analysis2 GetFromDataReader(MySqlDataReader dr)
		{
			Brand_sales_analysis2 o = new Brand_sales_analysis2();
		
			o.linenum =  (int) dr["linenum"];
			o.orderid = Utilities.FromDbValue<int>(dr["orderid"]);
			o.linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]);
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.description = string.Empty + dr["description"];
			o.orderqty = Utilities.FromDbValue<double>(dr["orderqty"]);
			o.unitprice = Utilities.FromDbValue<double>(dr["unitprice"]);
			o.rowprice = Utilities.FromDbValue<double>(dr["rowprice"]);
			o.unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]);
			o.rowprice_gbp = Utilities.FromDbValue<double>(dr["rowprice_gbp"]);
			o.PO_rowprice_gbp = Utilities.FromDbValue<double>(dr["PO_rowprice_gbp"]);
			o.cprod_code1 = string.Empty + dr["cprod_code1"];
			o.cprod_name = string.Empty + dr["cprod_name"];
			o.cprod_cgflag = Utilities.FromDbValue<int>(dr["cprod_cgflag"]);
			o.cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]);
			o.factory_id = Utilities.FromDbValue<int>(dr["factory_id"]);
			o.factory_ref = string.Empty + dr["factory_ref"];
			o.factory_name = string.Empty + dr["factory_name"];
			o.asaq_ref = string.Empty + dr["asaq_ref"];
			o.asaq_name = string.Empty + dr["asaq_name"];
			o.price_dollar = Utilities.FromDbValue<double>(dr["price_dollar"]);
			o.price_euro = Utilities.FromDbValue<double>(dr["price_euro"]);
			o.price_pound = Utilities.FromDbValue<double>(dr["price_pound"]);
			o.user_name = string.Empty + dr["user_name"];
			o.factory_code = string.Empty + dr["factory_code"];
			o.soline = Utilities.FromDbValue<int>(dr["soline"]);
			o.porderid = Utilities.FromDbValue<int>(dr["porderid"]);
			o.poqty = Utilities.FromDbValue<double>(dr["poqty"]);
			o.polinenum = Utilities.FromDbValue<int>(dr["polinenum"]);
			o.curr_symbol = string.Empty + dr["curr_symbol"];
			o.lme = Utilities.FromDbValue<double>(dr["lme"]);
			o.poprice = Utilities.FromDbValue<double>(dr["poprice"]);
			o.price_dollar_ex = Utilities.FromDbValue<double>(dr["price_dollar_ex"]);
			o.price_euro_ex = Utilities.FromDbValue<double>(dr["price_euro_ex"]);
			o.price_pound_ex = Utilities.FromDbValue<double>(dr["price_pound_ex"]);
			o.cprod_user = string.Empty + dr["cprod_user"];
			o.pocurrency = Utilities.FromDbValue<int>(dr["pocurrency"]);
			o.fact_curr_symbol = string.Empty + dr["fact_curr_symbol"];
			o.packunits = Utilities.FromDbValue<int>(dr["packunits"]);
			o.req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]);
			o.month21 = string.Empty + dr["month21"];
			o.month22 = string.Empty + dr["month22"];
			o.custpo = string.Empty + dr["custpo"];
			o.status = string.Empty + dr["status"];
			o.customer_code = string.Empty + dr["customer_code"];
			o.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
			o.user_country = string.Empty + dr["user_country"];
			o.client_name = string.Empty + dr["client_name"];
			o.product_group = string.Empty + dr["product_group"];
			o.userid1 = Utilities.FromDbValue<int>(dr["userid1"]);
			o.po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]);
			o.category1 = Utilities.FromDbValue<int>(dr["category1"]);
			o.cat1_name = string.Empty + dr["cat1_name"];
			o.brandname = string.Empty + dr["brandname"];
			o.oem_flag = Utilities.FromDbValue<int>(dr["oem_flag"]);
			o.brand_user_id = Utilities.FromDbValue<int>(dr["brand_user_id"]);
			o.cprod_brand_subcat = Utilities.FromDbValue<int>(dr["cprod_brand_subcat"]);
			o.analytics_category = Utilities.FromDbValue<int>(dr["analytics_category"]);
			o.analytics_option = Utilities.FromDbValue<int>(dr["analytics_option"]);
		    

		    return o;

		}
		
		
	}
}
			
			