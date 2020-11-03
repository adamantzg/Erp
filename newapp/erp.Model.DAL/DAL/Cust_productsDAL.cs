
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using erp.Model.DAL.Properties;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;

namespace erp.Model.DAL
{
	public class Cust_productsDAL
	{

        public const int DistributorForProductStats = 85;

        public static List<Cust_products> GetAll(bool includeMastProducts = false)
		{
			var result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
                var cmd = Utils.GetCommand(string.Format("SELECT cust_products.*{0} FROM cust_products {1}",includeMastProducts ? ",mast_products.*" : "",includeMastProducts ? " LEFT OUTER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id" : ""), conn);
                //var cmd = Utils.GetCommand("SELECT cust_products.* FROM cust_products ", conn);
				var dr = cmd.ExecuteReader();
			    Cust_products p = null;
				while (dr.Read())
				{
                    try
                    {
                        p = GetFromDataReader(dr);
                        if (includeMastProducts)
                        {
                            var mast_id = Utilities.FromDbValue<int>(dr["mast_id"]);
                            if(mast_id > 0)
                                p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                        }
                        result.Add(p);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
				}
				dr.Close();
                //if (includeMastProducts)
                //{
                //    foreach (var p in result)
                //    {
                //        p.MastProduct = Mast_productsDAL.GetById(p.cprod_mast ?? 0);
                //    }
                    
                //}
                    
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
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format("SELECT * FROM asaq.cust_products " +
											   "INNER JOIN asaq.sales_data ON cust_products.cprod_id = sales_data.cprod_id " +
												"WHERE {0} AND sales_data.month21 BETWEEN @from AND @to AND cust_products.cprod_status ='N'", cprod_user != null ? string.Format("cprod_user IN ({0})", Utils.CreateParametersFromIdList(cmd, cprod_user)) : "");
			   
				cmd.Parameters.AddWithValue("@from", monthFrom);
				cmd.Parameters.AddWithValue("@to", monthTo);

				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products s = GetFromDataReader(dr);
					//s.SalesProducts = Sales_dataDAL.GetFromDataReader(dr);
					result.Add(s);
				}
				dr.Close();
			}

			return result;
		}
		/***/

		public static Cust_products GetById(int id, bool loadParents = false)
		{
			Cust_products result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE cprod_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
					result.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
				}
				dr.Close();
				if (loadParents && result != null)
					result.Parents = GetSpareParents(id, conn);
				
			}
			return result;
		}

		public static Cust_products GetByIdFull(int id)
		{
			var sql = @"SELECT p.*,mp.*, pp_m.cprod_id, pp_m.market_id, pp_m.retail_price,  factory.*, mastproduct_price.*,pp_mastproduct.*, mast_products_pricing.*
						FROM cust_products p						
						INNER JOIN mast_products mp ON p.cprod_mast = mp.mast_id 
						INNER JOIN users factory ON mp.factory_id = factory.user_id
						LEFT OUTER JOIN pp_market_product pp_m ON pp_m.cprod_id = p.cprod_id
						LEFT OUTER JOIN mastproduct_price ON mp.mast_id = mastproduct_price.mastproduct_id
						LEFT OUTER JOIN pp_mastproduct ON mp.mast_id = pp_mastproduct.mastproduct_id
						LEFT OUTER JOIN mast_products_pricing ON mp.mast_id = mast_products_pricing.mast_id
						WHERE p.cprod_id = @id";
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				Cust_products result = null;
				conn.Query<Cust_products, Mast_products,Market_product, Company, mastproduct_price, 
					ProductPricingMastProductData, Mast_products_pricing, Cust_products>(
					sql, (p, mp, market_prod, factory, mp_price, mp_productpricing, mp_clientpricing) =>
					{
						if (result == null)
							result = p;
						if (result.MastProduct == null)
							result.MastProduct = mp;
						if (result.MarketData == null)
							result.MarketData = new List<Market_product>();
						if (market_prod != null && result.MarketData.FirstOrDefault(d => d.market_id == market_prod.market_id) == null)
							result.MarketData.Add(market_prod);
						if (result.MastProduct.Factory == null)
							result.MastProduct.Factory = factory;
						if (result.MastProduct.Prices == null)
							result.MastProduct.Prices = new List<mastproduct_price>();
						if (mp_price != null && result.MastProduct.Prices.FirstOrDefault(pr => pr.currency_id == mp_price.currency_id) == null)
							result.MastProduct.Prices.Add(mp_price);
						if (mp_productpricing != null && result.MastProduct.ProductPricingData == null)
							result.MastProduct.ProductPricingData = mp_productpricing;
						if (result.MastProduct.ClientPrices == null)
							result.MastProduct.ClientPrices = new List<Mast_products_pricing>();
						if (mp_clientpricing != null && result.MastProduct.ClientPrices.FirstOrDefault(cp => cp.id == mp_clientpricing.id) == null)
							result.MastProduct.ClientPrices.Add(mp_clientpricing);
						return result;
					}, new { id = id }, splitOn: "mast_id,cprod_id, user_id, id, mastproduct_id, id"
					);
				result.SalesForecast = conn.Query<Sales_forecast>("SELECT * FROM sales_forecast WHERE cprod_id = @id", new { id = id }).ToList();
				return result;
			}
		}

        public static List<Cust_products> GetByUser(int cprod_user)
        {
            List<Cust_products> result = new List<Cust_products>();
            using (var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT cust_products.* FROM cust_products WHERE cprod_user = @cprod_user", conn);
            
                cmd.Parameters.AddWithValue("@cprod_user",cprod_user);

                MySqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    Cust_products p = GetFromDataReader(dr);
                    //p.MastProduct=Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(p);
                } 
                dr.Close();
            }
           
            return result;
        }

		public static List<Cust_products> GetByCode1(string cprod_code1, int? cprod_user = null)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(@"SELECT cust_products.*, mast_products.* 
                    FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
                    WHERE cprod_code1 = @code AND (cprod_user = @cprod_user OR @cprod_user IS NULL)", conn);
				cmd.Parameters.AddWithValue("@code", cprod_code1);
			    cmd.Parameters.AddWithValue("@cprod_user", Utilities.ToDBNull(cprod_user));
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
				var cmd = Utils.GetCommand(@"SELECT cust_products.* FROM cust_products 
									   WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
									   AND brand_user_id IN (SELECT user_id FROM users WHERE user_id = @company_id 
									   OR (combined_factory>0 AND combined_factory = (SELECT combined_factory FROM users WHERE user_id = @company_id LIMIT 1)) OR user_id = 260)", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                if(company_id == 260) //Britton uses data for britton, arcade, burlington and clearwater users
                {
                    cmd.Parameters.Clear();
                    cmd = Utils.GetCommand(@"SELECT cust_products.* FROM cust_products 
									   WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
									   AND brand_user_id IN (SELECT user_id FROM users WHERE user_id IN (260,259,80,32) 
									   OR (combined_factory>0 AND combined_factory = (SELECT combined_factory FROM users WHERE user_id IN (260,259,80,32) LIMIT 1)))", conn);
                }
				
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
				MySqlCommand cmd = Utils.GetCommand(@"SELECT cust_products.* FROM cust_products
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
		    var brands = BrandsDAL.GetAll();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.*, users.* ,color.*
                                            FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                                  INNER JOIN users ON mast_products.factory_id = users.user_id
                                                     LEFT OUTER JOIN color ON cust_products.color_id = color.color_id
												 WHERE cprod_id IN ({0})",
												Utils.CreateParametersFromIdList(cmd, ids));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Cust_products p = GetFromDataReader(dr);
					p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    p.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
                    if (p.brand_userid != null)
				        p.Brand = brands.FirstOrDefault(b => b.user_id == p.brand_userid.Value);

                    if (p.color_id != null)
                        p.Color = GetColorFromDataReader(dr);

					p.cprod_stock_codes = new[] {p.cprod_stock_code ?? 0}.ToList();
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

        public static AsaqColor GetColorFromDataReader(MySqlDataReader dr)
        {
            AsaqColor color = new AsaqColor();

            color.color_id = (int)Utilities.FromDbValue<int>(dr["color_id"]);
            color.color_hex_code = string.Empty + Utilities.GetReaderField(dr, "color_hex_code");
            color.color_description = string.Empty + Utilities.GetReaderField(dr, "color_description");

            return color;
        }

        public static List<Cust_products> GetForFactories(IList<int> factory_ids, string text = null)
        {
            List<Cust_products> result = new List<Cust_products>();
            var brands = BrandsDAL.GetAll();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.*, users.* 
                                            FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                                  INNER JOIN users ON mast_products.factory_id = users.user_id
												 WHERE mast_products.factory_id IN ({0}) AND cprod_status = 'N' 
                                                AND (cust_products.cprod_name LIKE @text OR cust_products.cprod_code1 LIKE @text 
                                                        OR mast_products.factory_ref LIKE @text OR mast_products.factory_name LIKE @text OR @text IS NULL)",
                                                Utils.CreateParametersFromIdList(cmd, factory_ids));
                cmd.Parameters.AddWithValue("@text", !string.IsNullOrEmpty(text) ? text + "%" : Utilities.ToDBNull(text));
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    Cust_products p = GetFromDataReader(dr);
                    p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    p.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);

                     

                    if (p.brand_userid != null)
                        p.Brand = brands.FirstOrDefault(b => b.user_id == p.brand_userid.Value);
                    p.cprod_stock_codes = new[] { p.cprod_stock_code ?? 0 }.ToList();
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Cust_products> GetForCodes(List<string> codes)
        {
            var result = new List<Cust_products>();
            var brands = BrandsDAL.GetAll();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
												 WHERE cprod_code1 IN ({0})",
                                                Utils.CreateParametersFromIdList(cmd, codes));
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    Cust_products p = GetFromDataReader(dr);
                    p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    if (p.brand_userid != null)
                        p.Brand = brands.FirstOrDefault(b => b.user_id == p.brand_userid.Value);
                    p.cprod_stock_codes = new[] { p.cprod_stock_code ?? 0 }.ToList();
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Cust_products> GetForMastIds(List<int> ids, bool grouped = true)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("", conn);
				cmd.CommandText = grouped ? string.Format(@"SELECT mast_products.*,users.*,
							(SELECT GROUP_CONCAT(cprod_code1 SEPARATOR ', ') FROM cust_products WHERE cust_products.cprod_mast = mast_products.mast_id ) AS cprod_code1,
							(SELECT SUM(cprod_stock) FROM cust_products WHERE cust_products.cprod_mast = mast_products.mast_id ) AS cprod_stock,
							(SELECT GROUP_CONCAT(cprod_stock_code SEPARATOR ', ') FROM cust_products WHERE cust_products.cprod_mast = mast_products.mast_id) AS cprod_stock_code
							FROM mast_products INNER JOIN users ON mast_products.factory_id = users.user_id
							WHERE mast_id IN ({0})", Utils.CreateParametersFromIdList(cmd, ids)) : 
                            string.Format("SELECT * FROM cust_products WHERE cprod_mast IN ({0})", Utils.CreateParametersFromIdList(cmd, ids)) ;
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
				    if (grouped)
				    {
				        List<int> stock_codes =
				            (string.Empty + dr["cprod_stock_code"]).Split(',')
				                .Where(s => !string.IsNullOrEmpty(s.Trim()))
				                .Select(int.Parse)
				                .Distinct()
				                .ToList();
				        Cust_products p = new Cust_products
				        {
				            cprod_code1 = string.Empty + dr["cprod_code1"],
				            cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]),
				            cprod_stock_codes = stock_codes
				        };
				        p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
				        p.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
                        result.Add(p);
				    }
				    else
				    {
				        result.Add(GetFromDataReader(dr));
				    }
				    
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
				MySqlCommand cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.*, users.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
												  INNER JOIN users ON mast_products.factory_id = users.user_id WHERE cprod_brand_cat IN ({0})",
												Utils.CreateParametersFromIdList(cmd, catids));
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
					Utils.GetCommand(
						@"SELECT cust_products.*,mast_products.*,users.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
                        INNER JOIN users ON mast_products.factory_id = users.user_id
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
                    p.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
					result.Add(p);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetByCriteria(List<int> clientIds, List<int> factoryIds,bool? spares = null, bool? discontinued=null, int? category1_id = null, bool getSpecCount = false, string productName = null)
		{
			var result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT cust_products.*,mast_products.* {0} FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id ",
                                getSpecCount ? ", (SELECT COUNT(*) FROM product_specifications WHERE prod_mast_id = mast_products.mast_id) AS speccount " : "");
				var criteria = new List<string>();
			    if (clientIds != null && clientIds.Count > 0)
			    {
			        if (clientIds.Contains(Company.BathStore1) || clientIds.Contains(Company.BathStore2))
			        {
			             criteria.Add(string.Format(" (cust_products.brand_user_id IN ({0}) OR bs_visible = 1)",
			                                   Utils.CreateParametersFromIdList(cmd, clientIds, "clId")));
			        }
			        else
			        {
			            criteria.Add(string.Format(" cust_products.brand_user_id IN ({0})",
			                                   Utils.CreateParametersFromIdList(cmd, clientIds, "clId")));
			        }

			    }
			    if(factoryIds != null && factoryIds.Count > 0)
					criteria.Add(string.Format(" mast_products.factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factoryIds,"factId")));
				if(spares == false)
					criteria.Add(" mast_products.category1 <> 13");
				if(discontinued != null)
					criteria.Add($" cust_products.cprod_status {(discontinued == true ? "=" : "<>")} 'D'");
                if (category1_id != null)
                {
                    criteria.Add(" mast_products.category1 = @category1");
                    cmd.Parameters.AddWithValue("@category1", category1_id);
                }
			    if (!string.IsNullOrEmpty(productName))
			    {
                    criteria.Add(" (cust_products.cprod_name LIKE @prodname OR cust_products.cprod_code1 LIKE @prodname)");
			        cmd.Parameters.AddWithValue("@prodname", "%" + productName + "%");
			    }
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

        public static List<ProductFit> GetProductFitByCriteria(List<int> clientIds, List<int> factoryIds, bool? spares = null, bool? discontinued = null, int? category1_id = null, bool getSpecCount = false, string productName = null)
        {
            var result = new List<ProductFit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                
                var sql = $@"SELECT 
                            p.cprod_id,
                            p.cprod_user,
                            p.cprod_instructions,
                            p.cprod_code1,
                            p.cprod_label,
                            p.cprod_name,
                            p.cprod_status,                            
                            p.cprod_dwg,
                            p.cprod_packaging,
                            mp.prod_image3 AS mastProduct_prod_image3,
                            mp.prod_image4 AS mastProduct_prod_image4,
                            mp.prod_image5 AS mastProduct_prod_image5,
                            mp.prod_instructions AS mastProduct_prod_instructions                        
                            {(getSpecCount ? ", (SELECT COUNT(*) FROM product_specifications WHERE prod_mast_id = mp.mast_id) AS mastProduct_prod_SpecCount " : "")} 
                            FROM cust_products p INNER JOIN mast_products mp ON p.cprod_mast = mp.mast_id ";

                var criteria = new List<string>();
                if (clientIds != null && clientIds.Count > 0)
                {
                    if (clientIds.Contains(Company.BathStore1) || clientIds.Contains(Company.BathStore2))
                    {
                        criteria.Add(" (p.brand_user_id IN @clientIds OR bs_visible = 1)");
                                              
                    }
                    else
                    {
                        criteria.Add(" (p.brand_user_id IN @clientIds OR bs_visible = 1)");
                    }

                }
                if (factoryIds != null && factoryIds.Count > 0)
                    criteria.Add(" (mp.factory_id IN @factoryIds)");
                if (spares == false)
                    criteria.Add(" mp.category1 <> 13");
                if (discontinued != null)
                    criteria.Add($" p.cprod_status {(discontinued == true ? "=" : "<>")} 'D'");
                if (category1_id != null)
                {
                    criteria.Add($" mp.category1 {(category1_id > 0 ? "=" : "<>" )} @category1");
                    category1_id = Math.Abs(category1_id.Value);
                    
                }
                if (!string.IsNullOrEmpty(productName))
                {
                    criteria.Add(" (p.cprod_name LIKE @prodname OR p.cprod_code1 LIKE @prodname)");                    
                }
                if (criteria.Count > 0)
                    sql += " WHERE " + string.Join(" AND ", criteria);
                result = conn.Query<ProductFit>(sql, new { category1 = category1_id, prodname = productName != null ? "%" + productName + "%" : null,clientIds = clientIds, factoryIds = factoryIds }).ToList();
            }
            return result;
        }

        public static List<Cust_products> GetProductsOrdered(IList<int> company_ids, string searchCriteria)
		{
			List<Cust_products> result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT DISTINCT cust_products.* FROM cust_products INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id
												  INNER JOIN order_header ON order_lines.orderid = order_header.orderid
												  WHERE order_header.userid1 IN ({1})
												  AND (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria) 
												  AND order_header.`status` <> 'X' AND order_header.`status` <> 'Y' AND order_lines.orderqty > 0 AND
												  cust_products.cprod_returnable = 0 AND order_header.req_eta < now() AND order_header.req_eta > (now() - interval {0} month)
												  ", Properties.Settings.Default.OrderedProductsHistoryInterval,Utils.CreateParametersFromIdList(cmd,company_ids));
				
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
				MySqlCommand cmd = Utils.GetCommand(@"SELECT cust_products.*, mast_products.* 
										FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
										WHERE cprod_code1 LIKE @criteria OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria 
                                        AND cprod_retail IS NOT NULL AND cprod_retail > 0 AND cprod_status='N' ", conn);
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

        public static List<Cust_products> GetProductData(IList<int> ids)
        {
            var result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT cprod_id, (SELECT COUNT(DISTINCT dealers.user_id)
                                        FROM
                                        dealer_image_displays
                                        INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique
                                        INNER JOIN web_product_component ON web_product_component.web_unique = web_product_new.web_unique                                        
                                        INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                                        INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                                        WHERE dealers.hide_1 = 1 AND web_product_component.cprod_id = cust_products.cprod_id) AS DisplayQty,
                                        (SELECT MIN(porder_header.po_req_etd) 
                                        FROM
                                        porder_lines
                                        INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                        INNER JOIN order_lines ON order_lines.linenum = porder_lines.soline                                        
                                        INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                        WHERE COALESCE(order_header.stock_order,0) <> 1 AND order_lines.cprod_id = cust_products.cprod_id) AS FirstShipDate
                                        FROM cust_products WHERE cprod_id IN ({0})", Utils.CreateParametersFromIdList(cmd,ids));
                var dr = cmd.ExecuteReader();
                while(dr.Read()) {
                    var prod = new Cust_products();
                    prod.cprod_id = (int)dr["cprod_id"];
                    prod.DisplayQty = Utilities.FromDbValue<int>(dr["DisplayQty"]);
                    prod.FirstShipDate = Utilities.FromDbValue<DateTime>(dr["FirstShipDate"]);
                    result.Add(prod);
                }
                dr.Close();
            }
            return result;
        }

	    public static List<Cust_products> GetSpareParents(int cprod_id, IDbConnection conn = null)
	    {
	        var result = new List<Cust_products>();
	        var dispose = conn == null;
	        if (conn == null)
	        {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }

	        try
	        {
	            var cmd = Utils.GetCommand(@"SELECT cust_products.* FROM spares
                                INNER JOIN cust_products ON spares.product_cprod = cust_products.cprod_id 
                                WHERE spares.spare_cprod = @id",(MySqlConnection) conn);
	            cmd.Parameters.AddWithValue("@id", cprod_id);
	            var dr = cmd.ExecuteReader();
	            while (dr.Read())
	            {
	                result.Add(GetFromDataReader(dr));
	            }
	            dr.Close();
	        }
	        finally
	        {
                if (dispose)
                    conn.Dispose();
            }
	        
	        return result;
	    }

		public static List<Spare> GetSpares(IList<int> cprodIds)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<Spare, Cust_products, Spare>(
					@"SELECT spares.*,cust_products.* 
					  FROM cust_products INNER JOIN spares ON cust_products.cprod_id = spares.spare_cprod
					  WHERE spares.product_cprod IN @cprodIds",
					(s, p) =>
					{
						s.SpareProduct = p;
						return s;
					} ,new {cprodIds},splitOn:"cprod_id" ).ToList();
			}
		}

        


		public static Cust_products GetFromDataReader(MySqlDataReader dr, bool includeFactRef=false)
		{
			Cust_products o = new Cust_products();

			o.cprod_id = (int)dr["cprod_id"];
			o.cprod_mast = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_mast"));
			o.cprod_user = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_user"));
			o.cprod_name = string.Empty + Utilities.GetReaderField(dr, "cprod_name");
            o.cprod_name_web_override = string.Empty + Utilities.GetReaderField(dr, "cprod_name_web_override");
			o.cprod_name2 = string.Empty + Utilities.GetReaderField(dr, "cprod_name2");
			o.cprod_code1 = string.Empty + Utilities.GetReaderField(dr, "cprod_code1");
            o.cprod_code1_web_override = string.Empty + Utilities.GetReaderField(dr, "cprod_code1_web_override");
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
			o.EU_supplier = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "EU_supplier")) != null ? (bool?)Convert.ToBoolean(Utilities.GetReaderField(dr, "EU_supplier")) : null;
			o.on_order_qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "on_order_qty"));
			o.cprod_combined_product = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_combined_product"));
			o.UK_production = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "UK_production"));
			o.cprod_supplier = string.Empty + Utilities.GetReaderField(dr,"cprod_supplier");
			o.client_range = string.Empty + Utilities.GetReaderField(dr,"client_range");
			o.brand_userid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand_user_id"));
			o.brand_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand_id"));
			o.sale_retail = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"sale_retail"));
			o.analytics_category = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"analytics_category"));
			o.analytics_option = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"analytics_option"));
            o.barcode = Utilities.FromDbValue<long>(Utilities.GetReaderField(dr, "barcode"));
            o.product_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "product_type"));
            o.wras = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "wras"));
            o.warning_report = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "warning_report"));
            o.cprod_special_payment_terms = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_special_payment_terms"));
            o.product_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "product_type"));
            o.pending_discontinuation = Utilities.BoolFromLong(Utilities.GetReaderField(dr, "pending_discontinuation"));
            o.proposed_discontinuation = Utilities.BoolFromLong(Utilities.GetReaderField(dr, "proposed_discontinuation"));
            o.cwb_stock_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cwb_stock_type"));
            o.pallet_grouping = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "pallet_grouping"));
            o.dist_status = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_status"));
		    if (Utilities.ColumnExists(dr, "DisplayQty"))
		        o.DisplayQty = Utilities.FromDbValue<int>(dr["DisplayQty"]);
		    if (Utilities.ColumnExists(dr, "FirstShipDate"))
		        o.FirstShipDate = Utilities.FromDbValue<DateTime>(dr["FirstShipDate"]);
            o.analysis_d = string.Empty + Utilities.GetReaderField(dr, "analysis_d");

            o.color_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "color_id"));
            
            return o;

		}

        public static List<Brand> GetCWBrandsFromProducts()
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();

                MySqlCommand cmd = Utils.GetCommand("SELECT DISTINCT analysis_d AS brandname FROM cust_products WHERE TRIM(COALESCE(analysis_d,'')) <> ''", conn);
                var dr = cmd.ExecuteReader();
                while(dr.Read()) {
                    result.Add(new Brand { brandname = string.Empty + dr["brandname"] });
                }
                dr.Close();
            }
            return result;
        }

        public static void Create(Cust_products o)
		{
            string insertsql = @"INSERT INTO cust_products (cprod_mast,cprod_user,cprod_name,cprod_name_web_override,cprod_name2,cprod_code1,cprod_code2,cprod_price1,cprod_price2,cprod_price3,cprod_price4,cprod_image1,cprod_instructions2,cprod_instructions,cprod_label,cprod_packaging,cprod_dwg,cprod_spares,cprod_pdf1,cprod_cgflag,cprod_curr,cprod_opening_qty,cprod_opening_date,cprod_status,cprod_oldcode,cprod_lme,cprod_brand_cat,cprod_retail,cprod_retail_pending,cprod_retail_pending_date,cprod_retail_web_override,cprod_override_margin,cprod_disc,cprod_seq,cprod_stock_code,days30_sales,brand_grouping,b_gold,cprod_loading,moq,WC_2011,cprod_stock,cprod_stock2,cprod_stock_date,cprod_priority,cprod_status2,cprod_pending_price,cprod_pending_date,pack_image1,pack_image2,pack_image2b,pack_image2c,pack_image2d,pack_image3,pack_image4,aql_A,aql_D,aql_F,aql_M,insp_level_a,insp_level_D,insp_level_F,insp_level_M,criteria_status,cprod_confirmed,tech_template,tech_template2,cprod_returnable,client_cat1,client_cat2,client_image,cprod_track_image1,cprod_track_image2,cprod_track_image3,bs_visible,eu_supplier,on_order_qty,UK_production,cprod_supplier,client_range,brand_id,cprod_code1_web_override,barcode) 
				VALUES(@cprod_mast,@cprod_user,@cprod_name,@cprod_name_web_override,@cprod_name2,@cprod_code1,@cprod_code2,@cprod_price1,@cprod_price2,@cprod_price3,@cprod_price4,@cprod_image1,@cprod_instructions2,@cprod_instructions,@cprod_label,@cprod_packaging,@cprod_dwg,@cprod_spares,@cprod_pdf1,@cprod_cgflag,@cprod_curr,@cprod_opening_qty,@cprod_opening_date,@cprod_status,@cprod_oldcode,@cprod_lme,@cprod_brand_cat,@cprod_retail,@cprod_retail_pending,@cprod_retail_pending_date,@cprod_retail_web_override,@cprod_override_margin,@cprod_disc,@cprod_seq,@cprod_stock_code,@days30_sales,@brand_grouping,@b_gold,@cprod_loading,@moq,@WC_2011,@cprod_stock,@cprod_stock2,@cprod_stock_date,@cprod_priority,@cprod_status2,@cprod_pending_price,@cprod_pending_date,@pack_image1,@pack_image2,@pack_image2b,@pack_image2c,@pack_image2d,@pack_image3,@pack_image4,@aql_A,@aql_D,@aql_F,@aql_M,@insp_level_a,@insp_level_D,@insp_level_F,@insp_level_M,@criteria_status,@cprod_confirmed,@tech_template,@tech_template2,@cprod_returnable,@client_cat1,@client_cat2,@client_image,@cprod_track_image1,@cprod_track_image2,@cprod_track_image3,@bs_visible,@eu_supplier,@on_order_qty,@UK_production,@cprod_supplier,@client_range,@brand_id,@cprod_code1_web_override,@barcode)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
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
            cmd.Parameters.AddWithValue("@cprod_name_web_override", o.cprod_name_web_override);
			cmd.Parameters.AddWithValue("@cprod_name2", o.cprod_name2);
			cmd.Parameters.AddWithValue("@cprod_code1", o.cprod_code1);
            cmd.Parameters.AddWithValue("@cprod_code1_web_override", o.cprod_code1_web_override);
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
			cmd.Parameters.AddWithValue("@eu_supplier", o.EU_supplier != null ? (int?) Convert.ToInt32(o.EU_supplier) : null);
			cmd.Parameters.AddWithValue("@on_order_qty", o.on_order_qty);
			cmd.Parameters.AddWithValue("@UK_production", o.UK_production);
			cmd.Parameters.AddWithValue("@cprod_supplier", o.cprod_supplier);
			cmd.Parameters.AddWithValue("@client_range", o.client_range);
			cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
            cmd.Parameters.AddWithValue("@barcode", o.barcode);
            cmd.Parameters.AddWithValue("@analysis_d", o.analysis_d);
		}
		
		public static void Update(Cust_products o)
		{
            string updatesql = @"UPDATE cust_products SET cprod_mast = @cprod_mast,cprod_user = @cprod_user,cprod_name = @cprod_name, cprod_name_web_override=@cprod_name_web_override, cprod_name2 = @cprod_name2,cprod_code1 = @cprod_code1,
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
					cprod_supplier = @cprod_supplier, client_range = @client_range,brand_id = @brand_id, cprod_code1_web_override=@cprod_code1_web_override, barcode=@barcode,analysis_d = @analysis_d WHERE cprod_id = @cprod_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int cprod_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM cust_products WHERE cprod_id = @id" , conn);
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
				MySqlCommand cmd = Utils.GetCommand(@"SELECT userusers.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
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


		//public static List<Cust_products> GetProductsForAnalyticsCategories(IList<int> category_ids)
        public static List<Cust_products> GetProductsForAnalyticsCategories(int? brand_user_id = null, bool useSalesOrders = false)
		{
			var result = new List<Cust_products>();
			//if (category_ids.Count > 0)
			{
				using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
				{
					conn.Open();
					var cmd = Utils.GetCommand("", conn);
                    var join = brand_user_id != null ? "LEFT OUTER" : "INNER";
                    cmd.CommandText =
                                                       
                            $@"SELECT cust_products.*,mast_products.* ,
                            (SELECT COUNT(DISTINCT dealers.user_id)
                                FROM
                                dealer_image_displays
                                INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique
                                INNER JOIN web_product_component ON web_product_component.web_unique = web_product_new.web_unique
                                INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                                INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                                WHERE dealers.hide_1 = 1 AND web_product_component.cprod_id = cust_products.cprod_id) AS DisplayQty,
                                {(!useSalesOrders ? @"(SELECT MIN(porder_header.po_req_etd) 
                                FROM
                                porder_lines
                                INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                INNER JOIN order_lines ON order_lines.linenum = porder_lines.soline
                                INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                WHERE COALESCE(order_header.stock_order,0) <> 1 AND order_lines.cprod_id = cust_products.cprod_id)" :
                                @"(SELECT MIN(date_entered) FROM sales_orders WHERE cprod_id = cust_products.cprod_id)"
                                )} AS FirstShipDate
                            FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                               {join} JOIN analytics_subcategory ON cust_products.analytics_category = analytics_subcategory.subcat_id 
							   {join} JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
								WHERE {( 
                                brand_user_id != null ?
                                @"(analytics_categories.category_type = @brand_user_id
                                            OR
                                        (brand_user_id IS NOT NULL  AND brand_user_id IN(SELECT user_id FROM brands)
                                            AND EXISTS(SELECT cprod_id FROM cust_products other_prods
                                                        WHERE cprod_mast = cust_products.cprod_mast AND other_prods.brand_user_id = @brand_user_id)
                                            AND (analytics_categories.category_id IS NULL OR analytics_categories.category_type IS NOT NULL)     ))"
                                : "analytics_categories.category_type IS NULL")}";
                    if(brand_user_id != null)
				        cmd.Parameters.AddWithValue("@brand_user_id", brand_user_id);
                    cmd.CommandTimeout = 300;
					var dr = cmd.ExecuteReader();
				    
					while (dr.Read())
					{
					    var c = GetFromDataReader(dr);
					    c.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
						result.Add(c);
					}
                    dr.Close();

                    cmd.Parameters.Clear();
                    cmd.CommandText = $@"SELECT * FROM (SELECT mast_id, linedate, fn_unitprice_gbp(unitprice,unitcurrency) price
                                        FROM porder_lines WHERE mast_id IN ({Utils.CreateParametersFromIdList(cmd,result.Select(p=>p.cprod_mast ?? 0).Distinct().ToList() )})
                                        ORDER BY mast_id, linedate DESC) x
                                        GROUP BY mast_id";
                    dr = cmd.ExecuteReader();
                    var dict = new Dictionary<int, double?>();
                    while(dr.Read()) {
                        var mast_id = (int)dr["mast_id"];
                        dict[mast_id] = Utilities.FromDbValue<double>(dr["price"]);
                    }
                    dr.Close();
				    foreach (var p in result)
				    {
				        if (p.MastProduct.lme > 0 && dict.ContainsKey(p.MastProduct.mast_id))
				        {
                            p.MastProduct.LastPoLinePrice = dict[p.MastProduct.mast_id];
				        }
				    }
				}
			}
			return result;
		}

        public static List<Range> GetRangesForBrand(int brand_id)
        {
            var result = new List<Range>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT DISTINCT cprod_range,ranges.range_name FROM cust_products INNER JOIN ranges ON cust_products.cprod_range = ranges.rangeid WHERE brand_id = @brand_id", conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Range{rangeid = (int) dr["cprod_range"],range_name = string.Empty + dr["range_name"]});
                }
                dr.Close();
            }
            return result;
        }



	    public static int GetCountByCriteria(int brand_id, int? range)
	    {
	        var result = 0;
	        using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
	        {
	            conn.Open();
	            var cmd =
	                Utils.GetCommand(
	                    "SELECT COALESCE(COUNT(*),0) FROM cust_products WHERE brand_id = @brand_id AND cprod_range = @range",conn);
	            cmd.Parameters.AddWithValue("@brand_id", brand_id);
	            cmd.Parameters.AddWithValue("@range", range);
	            result = Convert.ToInt32(cmd.ExecuteScalar());
	        }
	        return result;
	    }

        public static List<Cust_products> GetByBrandAndRange(int brand_id, int? range)
        {
            var result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    Utils.GetCommand(
                        "SELECT * FROM cust_products WHERE brand_id = @brand_id AND cprod_range = @range", conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                cmd.Parameters.AddWithValue("@range", range);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

	    public static List<Cust_products> GetDisplayedComponents(int? brand_id = null)
	    {
            var result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM cust_products WHERE (brand_id = @brand_id OR @brand_id IS NULL) AND EXISTS (SELECT *
                                    FROM web_product_component INNER JOIN dealer_image_displays ON web_product_component.web_unique = dealer_image_displays.web_unique 
                                    WHERE web_product_component.cprod_id = cust_products.cprod_id)", conn);
                cmd.Parameters.AddWithValue("@brand_id", Utilities.ToDBNull(brand_id));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
	    }

        //public static List<Cust_products> GetCustProductsByCriteria(string searchText)
        //{
        //    List<Cust_products> result = new List<Cust_products>();
        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = Utils.GetCommand(@"SELECT cust_products.*, users.* FROM cust_products INNER JOIN users ON cust_products.cprod_user = users.user_id 
        //                  WHERE cust_products.cprod_user IN (42,78,80,259,260,309) AND cust_products.cprod_status = 'N' AND (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria)", conn);
        //        cmd.Parameters.AddWithValue("@criteria", "%" + searchText + "%");
        //        //cmd.Parameters.AddWithValue("@users", "42,78,80,259,260,309");
        //        MySqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            var cp = GetFromDataReader(dr);
        //            cp.Client = CompanyDAL.GetFromDataReader(dr);
        //            result.Add(cp);
        //        }
        //        dr.Close();
        //    }
        //    return result;
        //}

        public static List<Cust_products> GetCustProductsByCriteria(string searchText, IList<int> clientIds = null)
        {
            List<Cust_products> result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("",conn);
                cmd.CommandText = $@"SELECT cust_products.*, users.* FROM cust_products INNER JOIN users ON cust_products.cprod_user = users.user_id 
                        WHERE (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria) AND cust_products.cprod_status = 'N' 
                            { (clientIds != null ? $" AND cust_products.cprod_user IN ({Utils.CreateParametersFromIdList(cmd, clientIds)})" : "" ) }";
                cmd.Parameters.AddWithValue("@criteria", "%" + searchText + "%");
                //cmd.Parameters.AddWithValue("@users", "42,78,80,259,260,309");
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    var cp = GetFromDataReader(dr);
                    cp.Client = CompanyDAL.GetFromDataReader(dr);
                    result.Add(cp);
                }
                dr.Close();
            }
            return result;
        }
        public static List<Cust_products> GetCustProductsByCriteria2(string searchText, IList<int> clientIds = null)
        {
            List<Cust_products> result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = $@"SELECT cust_products.*, users.*, mast_products.factory_ref FROM cust_products 
                        INNER JOIN users ON cust_products.cprod_user = users.user_id 
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        WHERE (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria OR mast_products.factory_ref LIKE @criteria) AND cust_products.cprod_status = 'N' 
                            { (clientIds != null ? $" AND cust_products.cprod_user IN ({Utils.CreateParametersFromIdList(cmd, clientIds)})" : "") }";
                cmd.Parameters.AddWithValue("@criteria", "%" + searchText + "%");
                //cmd.Parameters.AddWithValue("@users", "42,78,80,259,260,309");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var cp = GetFromDataReader(dr);

                    cp.factory_ref =  string.Empty + dr["factory_ref"];                    
                    cp.Client = CompanyDAL.GetFromDataReader(dr);                    
                    result.Add(cp);
                }
                dr.Close();
            }
            return result;
        }
        public static List<Company> GetDistributorsByFactory(int factory_id) 
        {
            var result = new List<Company>();
            using(var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"
                                            SELECT
                                                cust_product_details.factory_code,
                                                cust_product_details.factory_id,
                                                cust_product_details.cprod_user,
                                                cust_product_details.cprod_code1,
                                                cust_product_details.cprod_name,
                                                cust_product_details.factory_ref,
                                                users.customer_code
                                            FROM
                                                cust_product_details
                                            INNER JOIN users ON cust_product_details.cprod_user = users.user_id
                                            WHERE factory_id = @factory_id 
                                            GROUP BY cprod_user
                                            ",conn);
                cmd.Parameters.AddWithValue("@factory_id", factory_id);
                var dr=cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add
                        ( new Company{                           
                            factory_code=string.Empty+dr["factory_code"],
                            user_id=(int) dr["cprod_user"],
                            customer_code=string.Empty + dr["customer_code"]
                        });
                }
                dr.Close();
            }
            return result;

        }

        public static List<Company> GetFactoriesForClientsDetails()
        {
            var result = new List<Company>();
            using(var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT
                                            cust_product_details.factory_code,
                                            cust_product_details.factory_id,
                                            cust_product_details.cprod_user,
                                            cust_product_details.cprod_code1,
                                            cust_product_details.cprod_name,
                                            cust_product_details.factory_ref,
                                            users.customer_code
                                            FROM
                                            cust_product_details
                                            INNER JOIN users ON cust_product_details.cprod_user = users.user_id
                                            GROUP BY cust_product_details.factory_code
                                            ", conn);
                //cmd.Parameters.AddWithValue("");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(
                            new Company {
                                factory_code = string.Empty + dr["factory_code"],
                                user_id = (int)dr["factory_id"],
                                customer_code = string.Empty + dr["customer_code"]
                            }
                        );
                }
                dr.Close();
            }
            return result;
        }

	    public static IList<Cust_products> GetAllProductsWithSameCode(IList<int> prodIds)
	    {
            var result = new List<Cust_products>();

	        if (prodIds.Count > 0)
	        {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                    conn.Open();
                    var cmd = new MySqlCommand("", conn);
                    cmd.CommandText = string.Format("SELECT * FROM cust_products WHERE cprod_id IN ({0})",
                        Utils.CreateParametersFromIdList(cmd, prodIds));
                    var dr = cmd.ExecuteReader();
                    while (dr.Read()) {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                    var codes = result.Select(p => p.cprod_code1).ToList();
                    result.Clear();
                    cmd.CommandText = string.Format("SELECT * FROM cust_products WHERE cprod_code1 IN ({0})",
                        Utils.CreateParametersFromIdList(cmd, codes, "code"));
                    dr = cmd.ExecuteReader();
                    while (dr.Read()) {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                }    
	        }
	        
	        return result;
	    }

	    public static List<ProductDate> ProductFirstShipmentDates(IList<int> cprodIds)
	    {
	        var result = new List<ProductDate>();
	        using (var conn = new MySqlConnection(Settings.Default.ConnString))
	        {
	            conn.Open();
                var cmd = new MySqlCommand("",conn);
                cmd.CommandText = string.Format(@"SELECT porder_lines.cprod_id, MIN(porder_header.po_req_etd) AS po_req_etd
                                                FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid INNER JOIN order_lines ON 
                                                porder_lines.soline = order_lines.linenum INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                                INNER JOIN users  on order_header.userid1 = users.user_id
                                                WHERE porder_header.`status` NOT IN ('X','Y') AND porder_lines.orderqty > 0 AND porder_lines.cprod_id IN ({0}) AND order_header.stock_order <> 1
                                                    AND (coalesce(`users`.`test_account`,0) = 0)
                                                GROUP BY porder_lines.cprod_id", Utils.CreateParametersFromIdList(cmd,cprodIds));
	            var dr = cmd.ExecuteReader();
	            while (dr.Read())
	            {
	                result.Add(new ProductDate {id = (int) dr["cprod_id"], Date = Utilities.FromDbValue<DateTime>(dr["po_req_etd"])});   
	            }
                dr.Close();
	        }
	        return result;
	    }

        public static List<Cust_products> GetForAnalyticsSubcats(IList<int> subcat_ids = null)
        {
            var result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString)) {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = @"SELECT cust_products.*, analytics_subcategory.*, analytics_categories.*,mast_products.* FROM cust_products 
                                    INNER JOIN analytics_subcategory ON cust_products.analytics_category = analytics_subcategory.subcat_id 
							        INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id 
                                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE cprod_status <> 'D'";
                if(subcat_ids != null)
                    cmd.CommandText += $" AND cust_products.analytics_category IN ({Utils.CreateParametersFromIdList(cmd, subcat_ids)})";
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    var p = GetFromDataReader(dr);
                    p.AnalyticsSubCategory = Analytics_subcategoryDAL.GetFromDataReader(dr);
                    p.AnalyticsSubCategory.Category = Analytics_categoriesDAL.GetFromDataReader(dr);
                    p.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductStats> GetProductStats(int ageForExclusion = 6)
        {
            var result = new List<ProductStats>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var brands = conn.Query<Brand>("SELECT * FROM brands WHERE eb_brand = 1").ToList();
                
                var brands_id = brands.Select(b => b.user_id).ToList();
                //foreach (var b in brands)
                //{
                //    criteria = criteria.Or(p => p.brand_userid == b.user_id);
                //}
                var exclusionThreshold = DateTime.Today.AddMonths(-1 * ageForExclusion);
                var excludedCats = new int?[] { 115, 207, 307, 608, 708, 912, 8081 };
                var criteriaMain = @"mp.category1 != 13 AND p.cprod_status <> 'D' AND mp.product_group IS NOT NULL AND COALESCE(p.report_exception,0) = 0 AND p.cprod_brand_cat NOT IN @excludedCats AND
                                        EXISTS(SELECT dp.dist_cprod_id FROM dist_products dp WHERE p.cprod_id = dp.dist_cprod_id AND dp.client_id = @distributor)";
                var criteriaOther = "p.cprod_status <> 'D' AND p.brand_user_id IN @brands";

                //criteria = criteria.And(baseCriteria);

                var query = @"SELECT p.cprod_code1, p.cprod_status, p.brand_user_id AS brand_userid, mp.product_group, MAX(ph.po_req_etd) AS po_req_etd 
                                             FROM porder_lines pl INNER JOIN porder_header ph ON pl.porderid = ph.porderid INNER JOIN order_lines l ON pl.soline = l.linenum
                                             INNER JOIN cust_products p ON l.cprod_id = p.cprod_id INNER JOIN mast_products mp ON p.cprod_mast = mp.mast_id
                                             WHERE {0}
                                             GROUP BY p.cprod_id";

                var products =
                    conn.Query<ProductRow>(string.Format(query,criteriaMain), new { excludedCats = excludedCats, distributor = DistributorForProductStats })
                                             .Where(p => brands_id.Contains(p.brand_userid) && p.po_req_etd > exclusionThreshold).ToList();

                var otherProducts =
                        conn.Query<ProductRow>(string.Format(query, criteriaOther), new { excludedCats = excludedCats, distributor = DistributorForProductStats, brands = brands_id }).ToList();

                var comparer = new CustProductCodeDistinctComparer();

                result = products.Where(
                    p =>
                        !otherProducts.Any(op => op.cprod_code1 == p.cprod_code1 && op.brand_userid != p.brand_userid))
                    .GroupBy(p => new { p.brand_userid, p.product_group }).Select(p => new ProductStats
                    {
                        brand_userid = p.Key.brand_userid,
                        brandname = brands.FirstOrDefault(b => b.user_id == p.Key.brand_userid).brandname,
                        product_group = p.Key.product_group,
                        numOfProducts = p.Distinct(comparer).Count()
                    }).ToList();


                result.AddRange(products.Where(
                    p =>
                        otherProducts.Any(
                            op =>
                                op.cprod_code1 == p.cprod_code1 && op.brand_userid != p.brand_userid))
                    .GroupBy(p => new { p.product_group }).Select(p => new ProductStats
                    {
                        brand_userid = null,
                        brandname = "Universal",
                        product_group = p.Key.product_group,
                        numOfProducts = p.Count()
                    }));
                return result;
            }

        }

        
    }

    public class ProductRow
    {
        public int cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_status { get; set; }
        public int? brand_userid { get; set; }
        public string product_group { get; set; }
        public DateTime? po_req_etd { get; set; }
    }

    public class CustProductCodeDistinctComparer : IEqualityComparer<ProductRow>
    {
        public bool Equals(ProductRow x, ProductRow y)
        {
            return x.cprod_code1 == y.cprod_code1;
        }

        public int GetHashCode(ProductRow obj)
        {
            return obj.cprod_id.GetHashCode();
        }
    }

	public class CustProductMastProductDistinctComparer : IEqualityComparer<Cust_products>
	{
		public bool Equals(Cust_products x, Cust_products y)
		{
			return x?.cprod_mast == y?.cprod_mast;
		}

		public int GetHashCode(Cust_products obj)
		{
			return obj.cprod_mast.GetHashCode();
		}
	}
}
			
			