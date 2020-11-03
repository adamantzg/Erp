
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Dapper.FluentMap.Mapping;
using Dapper.FluentMap;

namespace erp.Model.Dal.New
{
	public class CustproductsDAL : GenericDal<Cust_products>, ICustproductsDAL
	{
		private IBrandsDAL brandsDal;
		private static object mapper = 1;

		public CustproductsDAL(IDbConnection conn, IBrandsDAL brandsDal) : base(conn)
		{
			this.conn = (MySqlConnection) conn;
			this.brandsDal = brandsDal;

			lock (mapper)
			{
				try
				{
					if (!FluentMapper.EntityMaps.ContainsKey(typeof(Cust_products)))
						FluentMapper.Initialize(config => config.AddMap(new CustProductsMap()));
				}
				catch (InvalidOperationException)
				{
					//FluentMapper can raise this
				}
			}
		}

        public const int DistributorForProductStats = 85;

        public List<Cust_products> GetAll(bool includeMastProducts = false)
        {
	        return includeMastProducts
		        ? conn.Query<Cust_products, Mast_products, Cust_products>(
			        @"SELECT cust_products.*, mast_products.* FROM cust_products 
				  LEFT OUTER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id",
			        (cp, mp) =>
			        {
				        cp.MastProduct = mp;
				        return cp;
			        }, splitOn: "mast_id").ToList()
		        : conn.Query<Cust_products>("SELECT * FROM cust_products").ToList();
			
		}
		
		public List<Cust_products> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null)
		{
			Dictionary<int, Cust_products> dict = new Dictionary<int, Cust_products>();
		   var data = conn.Query<Cust_products, Sales_data, Cust_products>($@"SELECT * FROM cust_products 
		                                    INNER JOIN sales_data ON cust_products.cprod_id = sales_data.cprod_id
		                                    WHERE sales_data.month21 BETWEEN @monthFrom AND @monthTo AND cust_products.cprod_status ='N' 
										{(cprod_user != null ? " AND cprod_user IN @cprod_user" : "")}" ,
			   (cp, sd) =>
			   {
				   if (!dict.ContainsKey(cp.cprod_id))
				   {
					   dict[cp.cprod_id] = cp;
					   cp.SalesProducts = new List<Sales_data>();
				   }

				   var prod = dict[cp.cprod_id];
				   prod.SalesProducts.Add(sd);
				   return prod;

			   },new {monthFrom, monthTo, cprod_user},splitOn: "sales_unique").ToList();
			return dict.Values.ToList();


		}
		/***/

		public Cust_products GetById(int id, bool loadParents = false)
		{
			var result = conn.Query<Cust_products, Mast_products, Cust_products>(
				@"SELECT cust_products.*, mast_products.* FROM cust_products INNER JOIN mast_products 
				  ON cust_products.cprod_mast = mast_products.mast_id WHERE cprod_id = @id",
				(cp, mp) =>
				{
					cp.MastProduct = mp;
					return cp;
				}, new {id}, splitOn: "mast_id").FirstOrDefault();
			if (loadParents && result != null)
				result.Parents = GetSpareParents(id);
			return result;
			
		}

		public Cust_products GetByIdFull(int id)
		{
			var sql = @"SELECT p.*,mp.*, pp_m.*,  factory.*, mastproduct_price.*,pp_mastproduct.*
						FROM cust_products p						
						INNER JOIN mast_products mp ON p.cprod_mast = mp.mast_id 
						INNER JOIN users factory ON mp.factory_id = factory.user_id
						LEFT OUTER JOIN pp_market_product pp_m ON pp_m.cprod_id = p.cprod_id
						LEFT OUTER JOIN mastproduct_price ON mp.mast_id = mastproduct_price.mastproduct_id
						LEFT OUTER JOIN pp_mastproduct ON mp.mast_id = pp_mastproduct.mastproduct_id						
						WHERE p.cprod_id = @id";
			
			Cust_products result = null;
			conn.Query<Cust_products, Mast_products,Market_product, Company, mastproduct_price, 
				ProductPricingMastProductData, Cust_products>(
				sql, (p, mp, market_prod, factory, mp_price, mp_productpricing) =>
				{
					if (result == null)
						result = p;
					if (result.MastProduct == null)
						result.MastProduct = mp;
					if (result.MarketData == null)
						result.MarketData = new List<Market_product>();
					if (market_prod != null && market_prod.cprod_id > 0 && result.MarketData.FirstOrDefault(d => d.market_id == market_prod.market_id) == null)
						result.MarketData.Add(market_prod);
					if (result.MastProduct.Factory == null)
						result.MastProduct.Factory = factory;
					if (result.MastProduct.Prices == null)
						result.MastProduct.Prices = new List<mastproduct_price>();
					if (mp_price != null && result.MastProduct.Prices.FirstOrDefault(pr => pr.currency_id == mp_price.currency_id) == null)
						result.MastProduct.Prices.Add(mp_price);
					if (mp_productpricing != null && result.MastProduct.ProductPricingData == null)
						result.MastProduct.ProductPricingData = mp_productpricing;
					
					return result;
				}, new { id = id }, splitOn: "mast_id,market_id, user_id, id, mastproduct_id"
				);
			result.SalesForecast = conn.Query<Sales_forecast>("SELECT * FROM sales_forecast WHERE cprod_id = @id", new { id = id }).ToList();
			return result;
			
		}

        public List<Cust_products> GetByUser(int cprod_user)
        {
	        return conn.Query<Cust_products>(
		        @"SELECT cust_products.* FROM cust_products WHERE cprod_user = @cprod_user", new {cprod_user}).ToList();
        }

		public List<Cust_products> GetByCode1(string cprod_code1, int? cprod_user = null)
		{
			return conn.Query<Cust_products, Mast_products, Cust_products>(@"SELECT cust_products.*, mast_products.* 
                    FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
                    WHERE cprod_code1 = @cprod_code1 AND (cprod_user = @cprod_user OR @cprod_user IS NULL)",
				(cp, mp) =>
				{
					cp.MastProduct = mp;
					return cp;
				}, new {cprod_user, cprod_code1},splitOn:"mast_id").ToList();
		}

		public List<Cust_products> GetByCompanies(List<int?> companyIds, string prefixText)
		{
			var sql = companyIds.Count == 1 && companyIds[0] == 260 
				?
				@"SELECT cust_products.* FROM cust_products 
									   WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
									   AND brand_user_id IN (SELECT user_id FROM users WHERE user_id IN (260,259,80,32,45) 
									   OR (combined_factory>0 AND combined_factory = (SELECT combined_factory FROM users WHERE user_id IN (260,259,80,32) LIMIT 1)))"
				:
				 @"SELECT cust_products.* FROM cust_products 
									   WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
									   AND brand_user_id IN (SELECT user_id FROM users WHERE user_id IN @companyIds 
									   OR (combined_factory>0 AND combined_factory = 
									(SELECT combined_factory FROM users WHERE user_id IN @companyIds LIMIT 1)) OR user_id IN (260,45))";
			return conn.Query<Cust_products>(sql, new {companyIds, prefixText}).ToList();
		}

		public List<Cust_products> GetByCompany(int company_id, string prefixText = null)
		{
			return GetByCompanies(new List<int?> {company_id}, prefixText);
		}

		public List<Cust_products> GetByDistributor(int company_id)
		{
			return conn.Query<Cust_products>(@"SELECT cust_products.* FROM cust_products
													INNER JOIN dist_products ON dist_products.dist_cprod_id = cust_products.cprod_id
													WHERE dist_products.client_id = @company_id AND cprod_status <> 'd' ORDER BY cprod_code1",
				new {company_id}).ToList();
		}

		public List<Cust_products> GetForIds(List<int> ids)
		{
			var brands = brandsDal.GetAll();

			return conn.Query<AsaqColor, Cust_products, Mast_products, Company, Cust_products>(
				@"SELECT color.*, cust_products.*,mast_products.*, users.*
                FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                      INNER JOIN users ON mast_products.factory_id = users.user_id
                         LEFT OUTER JOIN color ON cust_products.color_id = color.color_id
					 WHERE cprod_id IN @ids",
				(color, cp, mp, company) =>
				{
					cp.MastProduct = mp;
					cp.MastProduct.Factory = company;
					cp.Color = color;
					if (cp.brand_userid != null)
						cp.Brand = brands.FirstOrDefault(b => b.user_id == cp.brand_userid.Value);
					cp.cprod_stock_codes = new[] {cp.cprod_stock_code ?? 0}.ToList();
					return cp;
				}, new {ids}, splitOn: "cprod_id, mast_id, user_id"
			).ToList();
			
		}

        
        public List<Cust_products> GetForFactories(IList<int> factory_ids, string text = null)
        {
            
            var brands = brandsDal.GetAll();
	        return conn.Query<Cust_products, Mast_products, Company, Cust_products>(
		        @"SELECT cust_products.*,mast_products.*, users.* 
                FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                      INNER JOIN users ON mast_products.factory_id = users.user_id
					 WHERE mast_products.factory_id IN ({0}) AND cprod_status = 'N' 
                    AND (cust_products.cprod_name LIKE @text OR cust_products.cprod_code1 LIKE @text 
                            OR mast_products.factory_ref LIKE @text OR mast_products.factory_name LIKE @text OR @text IS NULL)",
		        (cp, mp, company) =>
		        {
			        cp.MastProduct = mp;
			        cp.MastProduct.Factory = company;
			        return cp;
		        }, new {text = !string.IsNullOrEmpty(text) ? text + "%" : null}, splitOn: "mast_id, user_id"
	        ).ToList();
        }

        public List<Cust_products> GetForCodes(List<string> codes)
        {
	        var brands = brandsDal.GetAll();
	        return conn.Query<Cust_products, Mast_products, Cust_products>(
		        @"SELECT cust_products.*,mast_products.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
				  WHERE cprod_code1 IN @codes",
		        (cp, mp) =>
		        {
			        cp.MastProduct = mp;
			        if (cp.brand_userid != null)
				        cp.Brand = brands.FirstOrDefault(b => b.user_id == cp.brand_userid.Value);
			        cp.cprod_stock_codes = new[] { cp.cprod_stock_code ?? 0 }.ToList();
			        return cp;
		        }, new {codes}, splitOn: "mast_id").ToList();
            
        }

        public List<Cust_products> GetForMastIds(List<int> ids, bool grouped = true)
        {
		    if(grouped)
			{
				var list = new List<Cust_products>();
				conn.Query<Mast_products, Company, Cust_products, cust_products_extradata, Cust_products>(
			        @"SELECT mast_products.*,users.*, cust_products.*, cust_products_extradata.*
							FROM mast_products INNER JOIN users ON mast_products.factory_id = users.user_id INNER JOIN cust_products ON 
							mast_products.mast_id = cust_products.cprod_mast INNER JOIN cust_products_extradata ON 
							cust_products.cprod_id = cust_products_extradata.cprod_id
							WHERE mast_id IN @ids",
			        (mp, company, cp, cpe) =>
			        {
						var p = list.FirstOrDefault(x => x.cprod_id == cp.cprod_id);
						if(p == null)
						{
							p = cp;
							list.Add(p);
						}
				        p.MastProduct = mp;
				        p.MastProduct.Factory = company;				        
						p.ExtraData = cpe;
				        return p;
			        }, new {ids}, splitOn: "user_id, cprod_id, cprod_id"
		        );
				return list;
			} else
			{
				return conn.Query<Cust_products>("SELECT * FROM cust_products WHERE cprod_mast IN @ids", new {ids}).ToList();
			}   
			
		}


		public List<Cust_products> GetForCatIds(List<int> catids)
		{
			return conn.Query<Cust_products, Mast_products, Company, Cust_products>(
				@"SELECT cust_products.*,mast_products.*, users.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
				INNER JOIN users ON mast_products.factory_id = users.user_id WHERE cprod_brand_cat IN @catids",
				(cp, mp, factory) =>
				{
					cp.MastProduct = mp;
					cp.MastProduct.Factory = factory;
					return cp;
				}, new {catids}, splitOn: "mast_id, user_id"
			).ToList();
			
		}
		

		public List<Cust_products> GetByPatternAndFactory(string prefixText, int? factory_id)
		{
			return conn.Query<Cust_products, Mast_products, Company, Cust_products>(
				@"SELECT cust_products.*,mast_products.*,users.* FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
                INNER JOIN users ON mast_products.factory_id = users.user_id
				WHERE cprod_status <> 'D' AND (@prefixText IS NULL OR cprod_name LIKE CONCAT(@prefixText,'%') OR cprod_code1 LIKE CONCAT(@prefixText,'%')) 
				AND (mast_products.factory_id = @factory_id OR @factory_id IS NULL)",
				(cp, mp, factory) =>
				{
					cp.MastProduct = mp;
					cp.MastProduct.Factory = factory;
					return cp;
				}, new {prefixText, factory_id}, splitOn: "mast_id, user_id"
			).ToList();
			
		}

		public List<Cust_products> GetByCriteria(IList<int> clientIds, IList<int> factoryIds,bool? spares = null, 
			bool? discontinued=null, int? category1_id = null, bool getSpecCount = false, string productName = null)
		{
			var sql = string.Format(@"SELECT cust_products.*,mast_products.* {0} FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id ",
                       getSpecCount ? ", (SELECT COUNT(*) FROM product_specifications WHERE prod_mast_id = mast_products.mast_id) AS speccount " : "");
			var criteria = new List<string>();
		    if (clientIds != null && clientIds.Count > 0)
		    {
		        if (clientIds.Contains(Company.BathStore1) || clientIds.Contains(Company.BathStore2))
		        {
		             criteria.Add(" (cust_products.brand_user_id IN @clientIds OR bs_visible = 1)");
		        }
		        else
		        {
		            criteria.Add(" cust_products.brand_user_id IN @clientIds");
		        }

		    }
		    if(factoryIds != null && factoryIds.Count > 0)
				criteria.Add(" mast_products.factory_id IN @factoryIds");
			if(spares == false)
				criteria.Add(" mast_products.category1 <> 13");
			if(discontinued != null)
				criteria.Add($" cust_products.cprod_status {(discontinued == true ? "=" : "<>")} 'D'");
            if (category1_id != null)
            {
                criteria.Add(" mast_products.category1 = @category1_id");
            }
		    if (!string.IsNullOrEmpty(productName))
		    {
                criteria.Add(" (cust_products.cprod_name LIKE @prodname OR cust_products.cprod_code1 LIKE @prodname)");
		    }
		    if (criteria.Count > 0)
				sql += " WHERE " + string.Join(" AND ", criteria);
			return conn.Query<Cust_products, Mast_products, Cust_products>(sql,
				(cp, mp) =>
				{
					cp.MastProduct = mp;
					return cp;
				}, new {category1_id, factoryIds, clientIds, prodname = "%" + productName + "%"}, splitOn: "mast_id").ToList();
		}

        public List<ProductFit> GetProductFitByCriteria(List<int> clientIds, List<int> factoryIds, bool? spares = null, bool? discontinued = null, int? category1_id = null, bool getSpecCount = false, string productName = null)
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
                return conn.Query<ProductFit>(sql, new { category1 = category1_id, 
	                prodname = productName != null ? "%" + productName + "%" : null,clientIds,factoryIds }).ToList();
            
        }

        public List<Cust_products> GetProductsOrdered(IList<int> company_ids, string searchCriteria)
        {
	        return conn.Query<Cust_products>(
		        $@"SELECT DISTINCT cust_products.* FROM cust_products INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id
				  INNER JOIN order_header ON order_lines.orderid = order_header.orderid
				  WHERE order_header.userid1 IN @company_ids
				  AND (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria) 
				  AND order_header.`status` <> 'X' AND order_header.`status` <> 'Y' AND order_lines.orderqty > 0 AND
				  cust_products.cprod_returnable = 0 AND order_header.req_eta < now() 
				AND order_header.req_eta > (now() - interval {Properties.Settings.Default.OrderedProductsHistoryInterval} month)
				  ", new {criteria = "%" + searchCriteria + "%", company_ids}).ToList();
			
		}

		public List<Cust_products> GetByNameOrCode(string criteria)
		{
			return conn.Query<Cust_products, Mast_products, Cust_products>(
				@"SELECT cust_products.*, mast_products.* 
				FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
				WHERE cprod_code1 LIKE @criteria OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria 
                AND cprod_retail IS NOT NULL AND cprod_retail > 0 AND cprod_status='N' ",
				(cp, mp) =>
				{
					cp.MastProduct = mp;
					return cp;
				}, new {criteria = "%" + criteria + "%"}, splitOn: "mast_id").ToList();
			
		}

        public List<Cust_products> GetProductData(IList<int> ids)
        {
	        return conn.Query<Cust_products>(
		        @"SELECT cprod_id, (SELECT COUNT(DISTINCT dealers.user_id)
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
                FROM cust_products WHERE cprod_id IN @ids", new {ids}
	        ).ToList();
			
        }

	    public List<Cust_products> GetSpareParents(int cprod_id)
	    {
			return conn.Query<Cust_products>(
			    @"SELECT cust_products.* FROM spares
                INNER JOIN cust_products ON spares.product_cprod = cust_products.cprod_id 
                WHERE spares.spare_cprod = @cprod_id", new {cprod_id}
		    ).ToList();
	    }

		public List<Spare> GetSpares(IList<int> cprodIds)
		{
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

        public List<Brand> GetXYBrandsFromProducts()
        {
	        return conn.Query<Brand>(
		        "SELECT DISTINCT analysis_d AS brandname FROM cust_products WHERE TRIM(COALESCE(analysis_d,'')) <> ''"
		        ).ToList();
        }

		
		public List<User> GetFactoryControllerUsers(int cprod_id)
		{
			return conn.Query<User>(
				@"SELECT userusers.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
				 INNER JOIN admin_permissions ON mast_products.factory_id = admin_permissions.cusid
				 INNER JOIN userusers ON userusers.useruserid = admin_permissions.userid
				 WHERE admin_permissions.`returns` = 1 AND cust_products.cprod_id = @cprod_id AND userusers.user_email IS NOT NULL",
				new {cprod_id}).ToList();
		}

        public List<Cust_products> GetProductsForAnalyticsCategories(int? brand_user_id = null, bool useSalesOrders = false)
		{
			var join = brand_user_id != null ? "LEFT OUTER" : "INNER";
			var products = conn.Query<Cust_products, Mast_products, Cust_products>(
				$@"SELECT cust_products.*,mast_products.* ,
				30 AS DisplayQty,
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
						: "analytics_categories.category_type IS NULL")}",
				(cp, mp) =>
				{
					cp.MastProduct = mp;
					return cp;
				},
				new {brand_user_id}, splitOn: "mast_id").ToList();

			var mastProductsDict = conn.Query<Mast_products>(
				$@"SELECT mast_id, max(price_pound) FROM (SELECT mast_id, fn_unitprice_gbp(unitprice,unitcurrency) price_pound
                FROM porder_lines WHERE mast_id IN @mast_ids
                ORDER BY mast_id, linedate DESC) x
                GROUP BY mast_id", new {mast_ids = products.Select(p => p.cprod_mast ?? 0).Distinct().ToList()}
			).ToDictionary(mp=>mp.mast_id, mp=>mp.price_pound);

			foreach (var p in products)
			{
				if (p.MastProduct.lme > 0 && mastProductsDict.ContainsKey(p.MastProduct.mast_id))
				{
					p.MastProduct.LastPoLinePrice = mastProductsDict[p.MastProduct.mast_id];
				}
			}

			return products;
		}

        public List<Range> GetRangesForBrand(int brand_id)
        {
	        return conn.Query<Range>(
		        @"SELECT DISTINCT cprod_range,ranges.range_name FROM cust_products INNER JOIN ranges ON cust_products.cprod_range = ranges.rangeid 
				WHERE brand_id = @brand_id", new {brand_id}).ToList();
        }

	    public int GetCountByCriteria(int brand_id, int? range)
	    {
		    return conn.ExecuteScalar<int>(
			    "SELECT COALESCE(COUNT(*),0) FROM cust_products WHERE brand_id = @brand_id AND cprod_range = @range",
			    new {brand_id, range});
	    }

        public List<Cust_products> GetByBrandAndRange(int brand_id, int? range)
        {
	        return conn.Query<Cust_products>(
		        "SELECT * FROM cust_products WHERE brand_id = @brand_id AND cprod_range = @range",
		        new {brand_id, range}).ToList();
        }

	    public List<Cust_products> GetDisplayedComponents(int? brand_id = null)
	    {
		    return conn.Query<Cust_products>(
			    @"SELECT * FROM cust_products WHERE (brand_id = @brand_id OR @brand_id IS NULL) AND EXISTS (SELECT *
                FROM web_product_component INNER JOIN dealer_image_displays ON web_product_component.web_unique = dealer_image_displays.web_unique 
                WHERE web_product_component.cprod_id = cust_products.cprod_id)",
			    new {brand_id}).ToList();
	    }

        
        public List<Cust_products> GetCustProductsByCriteria(string searchText, IList<int> clientIds = null)
        {
	        return conn.Query<Cust_products, Company, Cust_products>(
		        $@"SELECT cust_products.*, users.* FROM cust_products INNER JOIN users ON cust_products.cprod_user = users.user_id 
                        WHERE (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria) AND cust_products.cprod_status = 'N' 
                            {(clientIds != null ? " AND cust_products.cprod_user IN @clientIds" : "")}",
		        (cp, client) =>
		        {
			        cp.Client = client;
			        return cp;
		        }, new {clientIds, criteria = "%" + searchText + "%"}, splitOn: "user_id").ToList();
			
        }
        public List<Cust_products> GetCustProductsByCriteria2(string searchText, IList<int> clientIds = null)
        {
			return conn.Query<Cust_products, Company, Cust_products>(
				$@"SELECT cust_products.*, mast_products.factory_ref, users.* FROM cust_products
                        INNER JOIN users ON cust_products.cprod_user = users.user_id 
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        WHERE (cust_products.cprod_code1 LIKE @criteria OR cust_products.cprod_name LIKE @criteria OR mast_products.factory_ref LIKE @criteria) AND cust_products.cprod_status = 'N' 
                            { (clientIds != null ? $" AND cust_products.cprod_user IN @clientIds" : "") }",
				
				(cp, client) =>
				{
					cp.Client = client;
					return cp;
				}, new {clientIds, criteria = "%" + searchText + "%"}, splitOn: "user_id").ToList();

            
        }

        public List<Company> GetDistributorsByFactory(int factory_id)
        {
	        return conn.Query<Company>(
		        @"
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
                GROUP BY cprod_user",
		        new {factory_id}).ToList();

        }

        public List<Company> GetFactoriesForClientsDetails()
        {
	        return conn.Query<Company>(
		        @"SELECT
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
                ").ToList();
            
        }

	    public IList<Cust_products> GetAllProductsWithSameCode(IList<int> prodIds)
	    {
		    if (prodIds.Count > 0)
		    {
			    var products = conn
				    .Query<Cust_products>("SELECT * FROM cust_products WHERE cprod_id IN @prodIds", new {prodIds}).ToList();

			    var codes = products.Select(p => p.cprod_code1).ToList();
			    return conn.Query<Cust_products>("SELECT * FROM cust_products WHERE cprod_code1 IN @codes", new {codes})
				    .ToList();
		    }
			return new List<Cust_products>();
			
	    }

	    public List<ProductDate> ProductFirstShipmentDates(IList<int> cprodIds)
	    {
	        return conn.Query<ProductDate>(
		        @"SELECT porder_lines.cprod_id AS id, MIN(porder_header.po_req_etd) AS Date
	                                        FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid INNER JOIN order_lines ON 
	                                        porder_lines.soline = order_lines.linenum INNER JOIN order_header ON order_lines.orderid = order_header.orderid
	                                        INNER JOIN users  on order_header.userid1 = users.user_id
	                                        WHERE porder_header.`status` NOT IN ('X','Y') AND porder_lines.orderqty > 0 
											AND porder_lines.cprod_id IN @cprodIds AND order_header.stock_order <> 1
	                                            AND (coalesce(`users`.`test_account`,0) = 0)
	                                        GROUP BY porder_lines.cprod_id", new {cprodIds}).ToList();
	        
	    }

        public List<Cust_products> GetForAnalyticsSubcats(IList<int> subcat_ids = null)
        {
	        return conn.Query<Cust_products, Analytics_subcategory, Analytics_categories, Mast_products, Cust_products>(
		        $@"SELECT cust_products.*, analytics_subcategory.*, analytics_categories.*,mast_products.* FROM cust_products 
                INNER JOIN analytics_subcategory ON cust_products.analytics_category = analytics_subcategory.subcat_id 
		        INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id 
                INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE cprod_status <> 'D' 
				{(subcat_ids != null ? " AND cust_products.analytics_category IN @subcat_ids" : "")}
				",
		        (cp, subcat, cat, mp) =>
		        {
			        cp.MastProduct = mp;
			        cp.AnalyticsSubCategory = subcat;
			        cp.AnalyticsSubCategory.Category = cat;
			        return cp;
		        }, new {subcat_ids}, splitOn: "subcat_id, category_id, mast_id").ToList();

        }

        public List<ProductStats> GetProductStats(int ageForExclusion = 6)
        {
            
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
                conn.Query<ProductRow>(string.Format(query,criteriaMain), new { excludedCats, distributor = DistributorForProductStats })
                                         .Where(p => brands_id.Contains(p.brand_userid) && p.po_req_etd > exclusionThreshold).ToList();

            var otherProducts =
                    conn.Query<ProductRow>(string.Format(query, criteriaOther), new { excludedCats = excludedCats, distributor = DistributorForProductStats, brands = brands_id }).ToList();

            var comparer = new CustProductCodeDistinctComparer();

            var result = products.Where(
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

		public List<ProductGroupClassReportRow> GetProductGroupClassData(DateTime? from, DateTime? to, bool distributorsOnly = true)
		{
			return conn.Query<ProductGroupClassReportRow>(
				$@"SELECT brands.brandname, cust_products.cprod_id, cust_products.cprod_code1, cust_products.cprod_name, product_group_id as old_status_id, 
					SUM(order_lines.orderqty) AS qty, SUM(fn_unitprice_gbp(order_lines.unitprice, order_lines.unitcurrency) * order_lines.orderqty) AS amount
					FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
					INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
					INNER JOIN brands ON cust_products.brand_user_id = brands.user_id
					INNER JOIN users client ON order_header.userid1 = client.user_id
					WHERE order_header.orderdate BETWEEN @from AND @to AND mast_products.category1 <> 13
					AND order_header.status NOT IN ('X','Y') AND client.distributor > 0 AND client.user_country IN ('GB','UK','IE')
					AND order_lines.orderqty > 0
					GROUP BY brands.brandname, cust_products.cprod_id, cust_products.cprod_code1, cust_products.cprod_name, product_group_id
					ORDER BY amount DESC;", new {from, to}
			).ToList();
		}

		public void UpdateProductGroupId(int cprod_id, int? newValue)
		{
			conn.Execute("UPDATE cust_products SET product_group_id = @newValue WHERE cprod_id = @cprod_id", new {cprod_id, newValue});
		}


		public void UpdateOtherFiles(Cust_products cp, int? fileType = null)
		{
			if(cp.OtherFiles != null)
			{
				if(conn.State != ConnectionState.Open) 
					conn.Open();
				var tr = conn.BeginTransaction();

				try
				{
					conn.Execute(@"DELETE FROM cust_product_file 
						WHERE (@fileType IS NULL OR file_id IN (SELECT id FROM file WHERE type_id = @fileType)) AND cprod_id = @cprod_id",
						new { cp.cprod_id, fileType });
					foreach (var f in cp.OtherFiles)
					{
						conn.Execute("INSERT INTO cust_product_file (cprod_id, file_id) VALUES (@cprod_id, @file_id)", new { cp.cprod_id, file_id = f.id });
					}
					tr.Commit();
				}
				catch
				{
					tr.Rollback();
					throw;
				}
			}
		}
		
		

		protected override string GetAllSql()
		{
			return "SELECT * FROM cust_products";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM cust_products WHERE cprod_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO cust_products (cprod_mast,cprod_user,cprod_name,cprod_name_web_override,cprod_name2,cprod_code1,cprod_code2,cprod_price1,cprod_price2,cprod_price3,cprod_price4,cprod_image1,cprod_instructions2,cprod_instructions,cprod_label,cprod_packaging,cprod_dwg,cprod_spares,cprod_pdf1,cprod_cgflag,cprod_curr,cprod_opening_qty,cprod_opening_date,cprod_status,cprod_oldcode,cprod_lme,cprod_brand_cat,cprod_retail,cprod_retail_pending,cprod_retail_pending_date,cprod_retail_web_override,cprod_override_margin,cprod_disc,cprod_seq,cprod_stock_code,days30_sales,brand_grouping,b_gold,cprod_loading,moq,WC_2011,cprod_stock,cprod_stock2,cprod_stock_date,cprod_priority,cprod_status2,cprod_pending_price,cprod_pending_date,pack_image1,pack_image2,pack_image2b,pack_image2c,pack_image2d,pack_image3,pack_image4,aql_A,aql_D,aql_F,aql_M,insp_level_a,insp_level_D,insp_level_F,insp_level_M,criteria_status,cprod_confirmed,tech_template,tech_template2,cprod_returnable,client_cat1,client_cat2,client_image,cprod_track_image1,cprod_track_image2,cprod_track_image3,bs_visible,eu_supplier,on_order_qty,UK_production,cprod_supplier,client_range,brand_id,cprod_code1_web_override,barcode) 
				VALUES(@cprod_mast,@cprod_user,@cprod_name,@cprod_name_web_override,@cprod_name2,@cprod_code1,@cprod_code2,@cprod_price1,@cprod_price2,@cprod_price3,@cprod_price4,@cprod_image1,@cprod_instructions2,@cprod_instructions,@cprod_label,@cprod_packaging,@cprod_dwg,@cprod_spares,@cprod_pdf1,@cprod_cgflag,@cprod_curr,@cprod_opening_qty,@cprod_opening_date,@cprod_status,@cprod_oldcode,@cprod_lme,@cprod_brand_cat,@cprod_retail,@cprod_retail_pending,@cprod_retail_pending_date,@cprod_retail_web_override,@cprod_override_margin,@cprod_disc,@cprod_seq,@cprod_stock_code,@days30_sales,@brand_grouping,@b_gold,@cprod_loading,@moq,@WC_2011,@cprod_stock,@cprod_stock2,@cprod_stock_date,@cprod_priority,@cprod_status2,@cprod_pending_price,@cprod_pending_date,@pack_image1,@pack_image2,@pack_image2b,@pack_image2c,@pack_image2d,@pack_image3,@pack_image4,@aql_A,@aql_D,@aql_F,@aql_M,@insp_level_a,@insp_level_D,@insp_level_F,@insp_level_M,@criteria_status,@cprod_confirmed,@tech_template,@tech_template2,@cprod_returnable,@client_cat1,@client_cat2,@client_image,@cprod_track_image1,@cprod_track_image2,@cprod_track_image3,@bs_visible,@eu_supplier,@on_order_qty,@UK_production,@cprod_supplier,@client_range,@brand_id,@cprod_code1_web_override,@barcode)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE cust_products SET cprod_mast = @cprod_mast,cprod_user = @cprod_user,cprod_name = @cprod_name, cprod_name_web_override=@cprod_name_web_override, cprod_name2 = @cprod_name2,cprod_code1 = @cprod_code1,
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
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM cust_products WHERE cprod_id = @id";
		}

		public void RemoveFile(int cprodId, int fileId)
		{
			conn.Execute("DELETE FROM cust_product_file WHERE cprod_id = @cprodId AND file_id = @fileId", new { cprodId, fileId});
		}

		protected override string IdField => "cprod_id";
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

	public class CustProductsMap : EntityMap<Cust_products>
	{
		public CustProductsMap()
		{
			Map(p => p.brand_userid).ToColumn("brand_user_id");			
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
			
			