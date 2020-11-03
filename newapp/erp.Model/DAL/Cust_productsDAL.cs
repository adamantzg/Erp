
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
	public class Cust_productsDAL
	{
	
		public static List<Cust_products> GetAll()
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM cust_products", conn);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}

        


		/***/
        public static List<Cust_products> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null)
        {
           var result = new List<Cust_products>();
            
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format("SELECT * FROM asaq.cust_products " +
                                               "INNER JOIN asaq.sales_data ON cust_products.cprod_id = sales_data.cprod_id " +
                                                "WHERE {0} AND sales_data.month21 BETWEEN @from AND @to AND cust_products.cprod_status ='N'", cprod_user != null ? string.Format("cprod_user IN ({0})", Utilities.CreateParametersFromIdList(cmd, cprod_user)) : "");
               
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Cust_products s = GetFromDataReader(dr);
                    s.SalesProducts = Sales_dataDAL.GetFromDataReader(dr);
                    result.Add(s);
                }
                dr.Close();
            }

            return result;
        }
        /***/

		public static Cust_products GetById(int id)
		{
			Cust_products result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE cprod_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
				    result.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetByCode1(string cprod_code1)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT cust_products.*, mast_products.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE cprod_code1 = @code", conn);
				cmd.Parameters.AddWithValue("@code", cprod_code1);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;  
		}

		public static List<Cust_products> GetByCompany(int company_id, string prefixText = null)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = new MySqlCommand(@"SELECT cust_products.* FROM cust_products 
									   WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
									   AND brand_user_id IN (SELECT user_id FROM users WHERE user_id = @company_id 
									   OR (combined_factory>0 AND combined_factory = (SELECT combined_factory FROM users WHERE user_id = @company_id LIMIT 1)))", conn);
				cmd.Parameters.AddWithValue("@company_id", company_id);
				cmd.Parameters.AddWithValue("@prefixText", prefixText != null ? (object) prefixText : DBNull.Value);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result; 
		}

		public static List<Cust_products> GetByDistributor(int company_id)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(@"SELECT cust_products.* FROM cust_products
													INNER JOIN dist_products ON dist_products.dist_cprod_id = cust_products.cprod_id
													WHERE dist_products.client_id = @company_id AND cprod_status <> 'd' ORDER BY cprod_code1", conn);
				cmd.Parameters.AddWithValue("@company_id", company_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetForIds(List<int> ids)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
												 WHERE cprod_id IN ({0})",
												Utilities.CreateParametersFromIdList(cmd, ids));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					p.cprod_stock_codes = new[] {p.cprod_stock_code != null ? p.cprod_stock_code.Value : 0}.ToList();
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetForMastIds(List<int> ids)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT mast_products.*,
							(SELECT GROUP_CONCAT(cprod_code1 SEPARATOR ', ') FROM cust_products WHERE cust_products.cprod_mast = mast_products.mast_id ) AS cprod_code1,
							(SELECT SUM(cprod_stock) FROM cust_products WHERE cust_products.cprod_mast = mast_products.mast_id ) AS cprod_stock,
							(SELECT GROUP_CONCAT(cprod_stock_code SEPARATOR ', ') FROM cust_products WHERE cust_products.cprod_mast = mast_products.mast_id) AS cprod_stock_code
							FROM mast_products
							WHERE mast_id IN ({0})", Utilities.CreateParametersFromIdList(cmd, ids));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					List<int> stock_codes = (string.Empty + dr["cprod_stock_code"]).Split(',').Where(s=>!string.IsNullOrEmpty(s.Trim())).Select(int.Parse).Distinct().ToList();
					Cust_products p = new Cust_products
						{
							cprod_code1 = string.Empty + dr["cprod_code1"],
							cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]),
							cprod_stock_codes = stock_codes
						};
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}


		public static List<Cust_products> GetForCatIds(List<int> catids)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.*, users.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
												  INNER JOIN users ON mast_products.factory_id = users.user_id WHERE cprod_brand_cat IN ({0})",
												Utilities.CreateParametersFromIdList(cmd, catids));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					p.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		

		public static List<Cust_products> GetByPatternAndFactory(string prefixText, int? factory_id)
		{
			var result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					new MySqlCommand(
						@"SELECT cust_products.*,mast_products.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
						WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
						AND (mast_products.factory_id = @factory_id OR @factory_id IS NULL)",
						conn);
				cmd.Parameters.AddWithValue("@factory_id", factory_id);
				cmd.Parameters.AddWithValue("@prefixText", prefixText != null ? (object) prefixText : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetByCriteria(List<int> clientIds, List<int> factoryIds,bool spares, bool discontinued)
		{
			var result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = new MySqlCommand("", conn);
				cmd.CommandText = @"SELECT cust_products.*,mast_products.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id ";
				var criteria = new List<string>();
				if(clientIds.Count > 0)
					criteria.Add(string.Format(" cust_products.brand_user_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,clientIds,"clId") ));
				if(factoryIds.Count > 0)
					criteria.Add(string.Format(" mast_products.factory_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,factoryIds,"factId")));
				if(!spares)
					criteria.Add(" mast_products.category1 <> 13");
				if(!discontinued)
					criteria.Add(" cust_products.cprod_status <> 'D'");
				if (criteria.Count > 0)
					cmd.CommandText += " WHERE " + string.Join(" AND ", criteria);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetProductsOrdered(IList<int> company_ids, string searchCriteria)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT DISTINCT cust_products.* FROM cust_products INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id
												  INNER JOIN order_header ON order_lines.orderid = order_header.orderid
												  WHERE order_header.userid1 IN ({1})
												  AND (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria) 
												  AND order_header.`status` <> 'X' AND order_header.`status` <> 'Y' AND order_lines.orderqty > 0 AND
												  cust_products.cprod_returnable = 0 AND order_header.req_eta < now() AND order_header.req_eta > (now() - interval {0} month)
												  ", Properties.Settings.Default.OrderedProductsHistoryInterval,Utilities.CreateParametersFromIdList(cmd,company_ids));
				
				cmd.Parameters.AddWithValue("@criteria", "%" + searchCriteria + "%");
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					//p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					//p.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetByNameOrCode(string criteria)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(@"SELECT cust_products.*, mast_products.* 
										FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
										WHERE cprod_code1 LIKE @criteria OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria", conn);
				cmd.Parameters.AddWithValue("@criteria", "%" + criteria + "%");
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}


		public static Cust_products GetFromDataReader(MySqlDataReader dr)
		{
			Cust_products o = new Cust_products();

			o.cprod_id = (int)dr["cprod_id"];
			o.cprod_mast = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_mast"));
			o.cprod_user = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_user"));
			o.cprod_name = string.Empty + Utilities.GetReaderField(dr, "cprod_name");
			o.cprod_name2 = string.Empty + Utilities.GetReaderField(dr, "cprod_name2");
			o.cprod_code1 = string.Empty + Utilities.GetReaderField(dr, "cprod_code1");
			o.cprod_code2 = string.Empty + Utilities.GetReaderField(dr, "cprod_code2");
			o.cprod_price1 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_price1"));
			o.cprod_price2 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_price2"));
			o.cprod_price3 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_price3"));
			o.cprod_price4 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_price4"));
			o.cprod_image1 = string.Empty + Utilities.GetReaderField(dr, "cprod_image1");
			o.cprod_instructions2 = string.Empty + Utilities.GetReaderField(dr, "cprod_instructions2");
			o.cprod_instructions = string.Empty + Utilities.GetReaderField(dr, "cprod_instructions");
			o.cprod_label = string.Empty + Utilities.GetReaderField(dr, "cprod_label");
			o.cprod_packaging = string.Empty + Utilities.GetReaderField(dr, "cprod_packaging");
			o.cprod_dwg = string.Empty + Utilities.GetReaderField(dr, "cprod_dwg");
			o.cprod_spares = string.Empty + Utilities.GetReaderField(dr, "cprod_spares");
			o.cprod_pdf1 = string.Empty + Utilities.GetReaderField(dr, "cprod_pdf1");
			o.cprod_cgflag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_cgflag"));
			o.cprod_curr = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_curr"));
			o.cprod_opening_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_opening_qty"));
			o.cprod_opening_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "cprod_opening_date"));
			o.cprod_status = string.Empty + Utilities.GetReaderField(dr, "cprod_status");
			o.cprod_oldcode = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_oldcode"));
			o.cprod_lme = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_lme"));
			o.cprod_brand_cat = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_brand_cat"));
			o.cprod_retail = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_retail"));
			o.cprod_retail_pending = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_retail_pending"));
			o.cprod_retail_pending_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "cprod_retail_pending_date"));
			o.cprod_retail_web_override = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_retail_web_override"));
			o.cprod_override_margin = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_override_margin"));
			o.cprod_disc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_disc"));
			o.cprod_seq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_seq"));
			o.cprod_stock_code = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_stock_code"));
			o.days30_sales = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "days30_sales"));
			o.brand_grouping = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "brand_grouping"));
			o.b_gold = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "b_gold"));
			o.cprod_loading = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_loading"));
			o.moq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "moq"));
			o.WC_2011 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "WC_2011"));
			o.cprod_stock = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_stock"));
			o.cprod_stock2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_stock2"));
			o.cprod_stock_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "cprod_stock_date"));
			o.cprod_priority = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_priority"));
			o.cprod_status2 = string.Empty + Utilities.GetReaderField(dr, "cprod_status2");
			o.cprod_pending_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_pending_price"));
			o.cprod_pending_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "cprod_pending_date"));
			o.pack_image1 = string.Empty + Utilities.GetReaderField(dr, "pack_image1");
			o.pack_image2 = string.Empty + Utilities.GetReaderField(dr, "pack_image2");
			o.pack_image2b = string.Empty + Utilities.GetReaderField(dr, "pack_image2b");
			o.pack_image2c = string.Empty + Utilities.GetReaderField(dr, "pack_image2c");
			o.pack_image2d = string.Empty + Utilities.GetReaderField(dr, "pack_image2d");
			o.pack_image3 = string.Empty + Utilities.GetReaderField(dr, "pack_image3");
			o.pack_image4 = string.Empty + Utilities.GetReaderField(dr, "pack_image4");
			o.aql_A = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "aql_A"));
			o.aql_D = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "aql_D"));
			o.aql_F = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "aql_F"));
			o.aql_M = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "aql_M"));
			o.insp_level_a = string.Empty + Utilities.GetReaderField(dr, "insp_level_a");
			o.insp_level_D = string.Empty + Utilities.GetReaderField(dr, "insp_level_D");
			o.insp_level_F = string.Empty + Utilities.GetReaderField(dr, "insp_level_F");
			o.insp_level_M = string.Empty + Utilities.GetReaderField(dr, "insp_level_M");
			o.criteria_status = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "criteria_status"));
			o.cprod_confirmed = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_confirmed"));
			o.tech_template = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "tech_template"));
			o.tech_template2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "tech_template2"));
			o.cprod_returnable = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_returnable"));
			o.client_cat1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_cat1"));
			o.client_cat2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_cat2"));
			o.client_image = string.Empty + Utilities.GetReaderField(dr, "client_image");
			o.cprod_track_image1 = string.Empty + Utilities.GetReaderField(dr, "cprod_track_image1");
			o.cprod_track_image2 = string.Empty + Utilities.GetReaderField(dr, "cprod_track_image2");
			o.cprod_track_image3 = string.Empty + Utilities.GetReaderField(dr, "cprod_track_image3");
			o.bs_visible = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "bs_visible"));
			o.original_cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "original_cprod_id"));
			o.cprod_range = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_range"));
			o.eu_supplier = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "EU_supplier")) != null ? (bool?)Convert.ToBoolean(Utilities.GetReaderField(dr, "EU_supplier")) : null;
			o.on_order_qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "on_order_qty"));
			o.cprod_combined_product = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_combined_product"));
			o.UK_production = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "UK_production"));
			o.cprod_supplier = string.Empty + dr["cprod_supplier"];
			o.client_range = string.Empty + dr["client_range"];
			o.brand_userid = Utilities.FromDbValue<int>(dr["brand_user_id"]);
			o.brand_id = Utilities.FromDbValue<int>(dr["brand_id"]);
		    o.sale_retail = Utilities.FromDbValue<double>(dr["sale_retail"]);
		    o.analytics_category = Utilities.FromDbValue<int>(dr["analytics_category"]);
		    o.analytics_option = Utilities.FromDbValue<int>(dr["analytics_option"]);
            
			return o;

		}
		
		public static void Create(Cust_products o)
		{
			string insertsql = @"INSERT INTO cust_products (cprod_mast,cprod_user,cprod_name,cprod_name2,cprod_code1,cprod_code2,cprod_price1,cprod_price2,cprod_price3,cprod_price4,cprod_image1,cprod_instructions2,cprod_instructions,cprod_label,cprod_packaging,cprod_dwg,cprod_spares,cprod_pdf1,cprod_cgflag,cprod_curr,cprod_opening_qty,cprod_opening_date,cprod_status,cprod_oldcode,cprod_lme,cprod_brand_cat,cprod_retail,cprod_retail_pending,cprod_retail_pending_date,cprod_retail_web_override,cprod_override_margin,cprod_disc,cprod_seq,cprod_stock_code,days30_sales,brand_grouping,b_gold,cprod_loading,moq,WC_2011,cprod_stock,cprod_stock2,cprod_stock_date,cprod_priority,cprod_status2,cprod_pending_price,cprod_pending_date,pack_image1,pack_image2,pack_image2b,pack_image2c,pack_image2d,pack_image3,pack_image4,aql_A,aql_D,aql_F,aql_M,insp_level_a,insp_level_D,insp_level_F,insp_level_M,criteria_status,cprod_confirmed,tech_template,tech_template2,cprod_returnable,client_cat1,client_cat2,client_image,cprod_track_image1,cprod_track_image2,cprod_track_image3,bs_visible,eu_supplier,on_order_qty,UK_production,cprod_supplier,client_range,brand_id) 
				VALUES(@cprod_mast,@cprod_user,@cprod_name,@cprod_name2,@cprod_code1,@cprod_code2,@cprod_price1,@cprod_price2,@cprod_price3,@cprod_price4,@cprod_image1,@cprod_instructions2,@cprod_instructions,@cprod_label,@cprod_packaging,@cprod_dwg,@cprod_spares,@cprod_pdf1,@cprod_cgflag,@cprod_curr,@cprod_opening_qty,@cprod_opening_date,@cprod_status,@cprod_oldcode,@cprod_lme,@cprod_brand_cat,@cprod_retail,@cprod_retail_pending,@cprod_retail_pending_date,@cprod_retail_web_override,@cprod_override_margin,@cprod_disc,@cprod_seq,@cprod_stock_code,@days30_sales,@brand_grouping,@b_gold,@cprod_loading,@moq,@WC_2011,@cprod_stock,@cprod_stock2,@cprod_stock_date,@cprod_priority,@cprod_status2,@cprod_pending_price,@cprod_pending_date,@pack_image1,@pack_image2,@pack_image2b,@pack_image2c,@pack_image2d,@pack_image3,@pack_image4,@aql_A,@aql_D,@aql_F,@aql_M,@insp_level_a,@insp_level_D,@insp_level_F,@insp_level_M,@criteria_status,@cprod_confirmed,@tech_template,@tech_template2,@cprod_returnable,@client_cat1,@client_cat2,@client_image,@cprod_track_image1,@cprod_track_image2,@cprod_track_image3,@bs_visible,@eu_supplier,@on_order_qty,@UK_production,@cprod_supplier,@client_range,@brand_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT cprod_id FROM cust_products WHERE cprod_id = LAST_INSERT_ID()";
				o.cprod_id = (int) cmd.ExecuteScalar();
				
			}
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Cust_products o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@cprod_mast", o.cprod_mast);
			cmd.Parameters.AddWithValue("@cprod_user", o.cprod_user);
			cmd.Parameters.AddWithValue("@cprod_name", o.cprod_name);
			cmd.Parameters.AddWithValue("@cprod_name2", o.cprod_name2);
			cmd.Parameters.AddWithValue("@cprod_code1", o.cprod_code1);
			cmd.Parameters.AddWithValue("@cprod_code2", o.cprod_code2);
			cmd.Parameters.AddWithValue("@cprod_price1", o.cprod_price1);
			cmd.Parameters.AddWithValue("@cprod_price2", o.cprod_price2);
			cmd.Parameters.AddWithValue("@cprod_price3", o.cprod_price3);
			cmd.Parameters.AddWithValue("@cprod_price4", o.cprod_price4);
			cmd.Parameters.AddWithValue("@cprod_image1", o.cprod_image1);
			cmd.Parameters.AddWithValue("@cprod_instructions2", o.cprod_instructions2);
			cmd.Parameters.AddWithValue("@cprod_instructions", o.cprod_instructions);
			cmd.Parameters.AddWithValue("@cprod_label", o.cprod_label);
			cmd.Parameters.AddWithValue("@cprod_packaging", o.cprod_packaging);
			cmd.Parameters.AddWithValue("@cprod_dwg", o.cprod_dwg);
			cmd.Parameters.AddWithValue("@cprod_spares", o.cprod_spares);
			cmd.Parameters.AddWithValue("@cprod_pdf1", o.cprod_pdf1);
			cmd.Parameters.AddWithValue("@cprod_cgflag", o.cprod_cgflag);
			cmd.Parameters.AddWithValue("@cprod_curr", o.cprod_curr);
			cmd.Parameters.AddWithValue("@cprod_opening_qty", o.cprod_opening_qty);
			cmd.Parameters.AddWithValue("@cprod_opening_date", o.cprod_opening_date);
			cmd.Parameters.AddWithValue("@cprod_status", o.cprod_status);
			cmd.Parameters.AddWithValue("@cprod_oldcode", o.cprod_oldcode);
			cmd.Parameters.AddWithValue("@cprod_lme", o.cprod_lme);
			cmd.Parameters.AddWithValue("@cprod_brand_cat", o.cprod_brand_cat);
			cmd.Parameters.AddWithValue("@cprod_retail", o.cprod_retail);
			cmd.Parameters.AddWithValue("@cprod_retail_pending", o.cprod_retail_pending);
			cmd.Parameters.AddWithValue("@cprod_retail_pending_date", o.cprod_retail_pending_date);
			cmd.Parameters.AddWithValue("@cprod_retail_web_override", o.cprod_retail_web_override);
			cmd.Parameters.AddWithValue("@cprod_override_margin", o.cprod_override_margin);
			cmd.Parameters.AddWithValue("@cprod_disc", o.cprod_disc);
			cmd.Parameters.AddWithValue("@cprod_seq", o.cprod_seq);
			cmd.Parameters.AddWithValue("@cprod_stock_code", o.cprod_stock_code);
			cmd.Parameters.AddWithValue("@days30_sales", o.days30_sales);
			//cmd.Parameters.AddWithValue("@days30_sales_cs", o.days30_sales_cs);
			cmd.Parameters.AddWithValue("@brand_grouping", o.brand_grouping);
			cmd.Parameters.AddWithValue("@b_gold", o.b_gold);
			cmd.Parameters.AddWithValue("@cprod_loading", o.cprod_loading);
			cmd.Parameters.AddWithValue("@moq", o.moq);
			cmd.Parameters.AddWithValue("@WC_2011", o.WC_2011);
			cmd.Parameters.AddWithValue("@cprod_stock", o.cprod_stock);
			cmd.Parameters.AddWithValue("@cprod_stock2", o.cprod_stock2);
			cmd.Parameters.AddWithValue("@cprod_stock_date", o.cprod_stock_date);
			cmd.Parameters.AddWithValue("@cprod_priority", o.cprod_priority);
			cmd.Parameters.AddWithValue("@cprod_status2", o.cprod_status2);
			cmd.Parameters.AddWithValue("@cprod_pending_price", o.cprod_pending_price);
			cmd.Parameters.AddWithValue("@cprod_pending_date", o.cprod_pending_date);
			cmd.Parameters.AddWithValue("@pack_image1", o.pack_image1);
			cmd.Parameters.AddWithValue("@pack_image2", o.pack_image2);
			cmd.Parameters.AddWithValue("@pack_image2b", o.pack_image2b);
			cmd.Parameters.AddWithValue("@pack_image2c", o.pack_image2c);
			cmd.Parameters.AddWithValue("@pack_image2d", o.pack_image2d);
			cmd.Parameters.AddWithValue("@pack_image3", o.pack_image3);
			cmd.Parameters.AddWithValue("@pack_image4", o.pack_image4);
			cmd.Parameters.AddWithValue("@aql_A", o.aql_A);
			cmd.Parameters.AddWithValue("@aql_D", o.aql_D);
			cmd.Parameters.AddWithValue("@aql_F", o.aql_F);
			cmd.Parameters.AddWithValue("@aql_M", o.aql_M);
			cmd.Parameters.AddWithValue("@insp_level_a", o.insp_level_a);
			cmd.Parameters.AddWithValue("@insp_level_D", o.insp_level_D);
			cmd.Parameters.AddWithValue("@insp_level_F", o.insp_level_F);
			cmd.Parameters.AddWithValue("@insp_level_M", o.insp_level_M);
			cmd.Parameters.AddWithValue("@criteria_status", o.criteria_status);
			cmd.Parameters.AddWithValue("@cprod_confirmed", o.cprod_confirmed);
			cmd.Parameters.AddWithValue("@tech_template", o.tech_template);
			cmd.Parameters.AddWithValue("@tech_template2", o.tech_template2);
			cmd.Parameters.AddWithValue("@cprod_returnable", o.cprod_returnable);
			cmd.Parameters.AddWithValue("@client_cat1", o.client_cat1);
			cmd.Parameters.AddWithValue("@client_cat2", o.client_cat2);
			cmd.Parameters.AddWithValue("@client_image", o.client_image);
			cmd.Parameters.AddWithValue("@cprod_track_image1", o.cprod_track_image1);
			cmd.Parameters.AddWithValue("@cprod_track_image2", o.cprod_track_image2);
			cmd.Parameters.AddWithValue("@cprod_track_image3", o.cprod_track_image3);
			cmd.Parameters.AddWithValue("@bs_visible", o.bs_visible);
			cmd.Parameters.AddWithValue("@eu_supplier", o.eu_supplier != null ? (int?) Convert.ToInt32(o.eu_supplier) : null);
			cmd.Parameters.AddWithValue("@on_order_qty", o.on_order_qty);
			cmd.Parameters.AddWithValue("@UK_production", o.UK_production);
			cmd.Parameters.AddWithValue("@cprod_supplier", o.cprod_supplier);
			cmd.Parameters.AddWithValue("@client_range", o.client_range);
			cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
		}
		
		public static void Update(Cust_products o)
		{
			string updatesql = @"UPDATE cust_products SET cprod_mast = @cprod_mast,cprod_user = @cprod_user,cprod_name = @cprod_name,cprod_name2 = @cprod_name2,cprod_code1 = @cprod_code1,
						cprod_code2 = @cprod_code2,cprod_price1 = @cprod_price1,cprod_price2 = @cprod_price2,cprod_price3 = @cprod_price3,cprod_price4 = @cprod_price4,cprod_image1 = @cprod_image1,
					cprod_instructions2 = @cprod_instructions2,cprod_instructions = @cprod_instructions,cprod_label = @cprod_label,cprod_packaging = @cprod_packaging,cprod_dwg = @cprod_dwg,
					cprod_spares = @cprod_spares,cprod_pdf1 = @cprod_pdf1,cprod_cgflag = @cprod_cgflag,cprod_curr = @cprod_curr,cprod_opening_qty = @cprod_opening_qty,
					cprod_opening_date = @cprod_opening_date,cprod_status = @cprod_status,cprod_oldcode = @cprod_oldcode,cprod_lme = @cprod_lme,cprod_brand_cat = @cprod_brand_cat,
					cprod_retail = @cprod_retail,cprod_retail_pending = @cprod_retail_pending,cprod_retail_pending_date = @cprod_retail_pending_date,
					cprod_retail_web_override = @cprod_retail_web_override,cprod_override_margin = @cprod_override_margin,cprod_disc = @cprod_disc,cprod_seq = @cprod_seq,
					cprod_stock_code = @cprod_stock_code,days30_sales = @days30_sales,brand_grouping = @brand_grouping,b_gold = @b_gold,cprod_loading = @cprod_loading,moq = @moq,
					WC_2011 = @WC_2011,cprod_stock = @cprod_stock,cprod_stock2 = @cprod_stock2,cprod_stock_date = @cprod_stock_date,cprod_priority = @cprod_priority,
					cprod_status2 = @cprod_status2,cprod_pending_price = @cprod_pending_price,cprod_pending_date = @cprod_pending_date,pack_image1 = @pack_image1,pack_image2 = @pack_image2,
					pack_image2b = @pack_image2b,pack_image2c = @pack_image2c,pack_image2d = @pack_image2d,pack_image3 = @pack_image3,pack_image4 = @pack_image4,aql_A = @aql_A,aql_D = @aql_D,
					aql_F = @aql_F,aql_M = @aql_M,insp_level_a = @insp_level_a,insp_level_D = @insp_level_D,insp_level_F = @insp_level_F,insp_level_M = @insp_level_M,
					criteria_status = @criteria_status,cprod_confirmed = @cprod_confirmed,tech_template = @tech_template,tech_template2 = @tech_template2,cprod_returnable = @cprod_returnable,
					client_cat1 = @client_cat1,client_cat2 = @client_cat2,client_image = @client_image,cprod_track_image1 = @cprod_track_image1,cprod_track_image2 = @cprod_track_image2,
					cprod_track_image3 = @cprod_track_image3,bs_visible = @bs_visible,eu_supplier = @eu_supplier,on_order_qty = @on_order_qty,UK_production = @UK_production, 
					cprod_supplier = @cprod_supplier, client_range = @client_range,brand_id = @brand_id WHERE cprod_id = @cprod_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int cprod_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM cust_products WHERE cprod_id = @id" , conn);
				cmd.Parameters.AddWithValue("@id", cprod_id);
				cmd.ExecuteNonQuery();
			}
		}

		public static List<User> GetFactoryControllerUsers(int cprod_id)
		{
			List<User> result = new List<User>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(@"SELECT userusers.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
													 INNER JOIN admin_permissions ON mast_products.factory_id = admin_permissions.cusid
													 INNER JOIN userusers ON userusers.useruserid = admin_permissions.userid
													 WHERE admin_permissions.`returns` = 1 AND cust_products.cprod_id = @cprod_id AND userusers.user_email IS NOT NULL", conn);
				cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(UserDAL.GetFromDataReader(dr));
				}
				dr.Close();   
			}
			return result;
		}


	    public static List<Cust_products> GetProductsForAnalyticsCategories(IList<int> category_ids)
	    {
	        var result = new List<Cust_products>();
	        if (category_ids.Count > 0)
	        {
	            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
	            {
	                conn.Open();
	                var cmd = new MySqlCommand("", conn);
	                cmd.CommandText =
	                    string.Format(
	                        @"SELECT cust_products.* FROM cust_products LEFT OUTER JOIN analytics_subcategory ON cust_products.analytics_category = analytics_subcategory.subcat_id 
                                            LEFT OUTER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                                            WHERE (analytics_subcategory.category_id IN ({0})
                                                     OR 
                                                  (brand_user_id IS NOT NULL 
                                                        AND EXISTS(SELECT cprod_id FROM cust_products other_prods INNER JOIN analytics_subcategory other_subcats ON other_prods.analytics_category = other_subcats.subcat_id
                                                                    WHERE cprod_mast = cust_products.cprod_mast AND other_subcats.category_id IN ({0}))))",
	                        Utilities.CreateParametersFromIdList(cmd, category_ids));
	                var dr = cmd.ExecuteReader();
	                while (dr.Read())
	                {
	                    result.Add(GetFromDataReader(dr));
	                }
	            }
	        }
	        return result;
	    }
	}
}
			
			