using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public enum OutstandingOrdersMode
    {
        Both,
        Production,
        Transit
    }

    public enum CountryFilter
    {
        All = 0,
        UKOnly = 1,
        NonUK = 2
    }

    public class AnalyticsDAL
    {
        public static List<SalesByMonth> GetSalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null, bool useETA=false, bool brands = true,int[] includedClients = null,int[] excludedClients=null,List<int> factory_ids = null)
        {
            var result = new List<SalesByMonth>();
            var monthField = useETA ? "month22" : "month21";
            var priceField = factory_ids != null ? "po_rowprice" : "rowprice_gbp";
            string view =  factory_ids != null ? "factory_sales_analysis" :  brands ? "brand_sales_analysis2" : "brs_sales_analysis2";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                        string.Format(@"SELECT  {1}, SUM({6}) as total, COUNT(DISTINCT orderid) AS numOfOrders FROM {2} WHERE {1} BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} {3} {5}
                            {4} GROUP BY {1} ", 
                            GetCountryCondition(countryFilter),monthField,view,HandleClients(cmd,"cprod_user",includedClients,excludedClients),
                            factory_ids != null ? string.Format(" AND factory_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,factory_ids)) : "",
                            factory_ids != null ? "" : " AND (brand_user_id = @userid OR @userid IS NULL)" ,priceField);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new SalesByMonth {Month21 = Convert.ToInt32(dr[monthField]), Amount = (double) dr["total"], numOfOrders = Convert.ToInt32(dr["numOfOrders"])});
                }
                dr.Close();
            }
            return result;
        }

        public static string HandleClients(MySqlCommand cmd, string field = "cprod_user", IList<int> includedClients=null, IList<int> excludedClients=null)
        {
            var result = string.Empty;
            if (includedClients != null)
                result = string.Format(" AND {0} IN ({1})",field, Utilities.CreateParametersFromIdList(cmd, includedClients));
            else if (excludedClients != null)
            {
                result = string.Format(" AND {0} NOT IN ({1})", field, Utilities.CreateParametersFromIdList(cmd, excludedClients));
            }
            return result;
        }

        private static string GetCountryCondition(CountryFilter countryFilter, string prefix = "")
        {
            return countryFilter != CountryFilter.All
                       ? string.Format(" AND {1}user_country {0} IN ('GB','IE')",
                                       countryFilter == CountryFilter.UKOnly ? "" : "NOT",prefix)
                       : "";
        }

        public static List<SalesByMonth> GetNumOfOrdersByMonthForFactories(int from, int to, IList<int> factoryIds=null)
        {
            var result = new List<SalesByMonth>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = 
                    new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT count(DISTINCT orderid) as numOfOrders, month21 FROM factory_sales_analysis WHERE cprod_user > 0 {0} and category1 <> 13 AND month21 BETWEEN @from AND @to
                                                    group by month21 order by month21 ASC",factoryIds != null ? string.Format(" AND factory_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,factoryIds)) : "");
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new SalesByMonth { Month21 = Convert.ToInt32(dr["month21"]), numOfOrders = Convert.ToInt32(dr["numOfOrders"]) });
                }
                dr.Close();
            }
            return result;
        }
        /***/
        public static List<FactorySalesByMonth> GetSalesOfOrdersByMonthForFactories(int from, int to)
        {
            var result = new List<FactorySalesByMonth>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT count(DISTINCT orderid) as numOfOrders,sum(PO_rowprice_gbp) as salesAmount , asaq.brand_sales_analysis2.month21,asaq.brand_sales_analysis2.factory_code,asaq.users.combined_factory,asaq.users.user_id
                                                    FROM brand_sales_analysis2 INNER JOIN asaq.users ON asaq.brand_sales_analysis2.factory_id = asaq.users.user_id 
                                                    WHERE cprod_user > 0 and category1 <> 13 AND month21 BETWEEN @from AND @to
                                                    group by factory_code order by salesAmount DESC");
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new FactorySalesByMonth
                    {
                        Month21 = Convert.ToInt32(dr["month21"]),
                        Amount = Convert.ToInt32(dr["salesAmount"]),
                        factoryCode = string.Empty + dr["factory_code"],
                        combined_factory = Convert.ToInt32(dr["combined_factory"]),
                        user_id=Convert.ToInt32(dr["user_id"])
                    });
                }
                dr.Close();
            }

            
            return result;
        }
        /***/


        public static List<SalesByMonth> GetNumOfOrdersByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool? brands = true, int[] includedClients = null, int[] excludedClients = null,int[] excludedContainerTypes = null,int[] includedContainerTypes = null,IList<int> factoryIds = null )
        {
            var result = new List<SalesByMonth>();
            if(excludedContainerTypes == null && includedContainerTypes == null)
                excludedContainerTypes = new[] {3,4,5};
            var brandsClause = brands != null ? brands.Value ? "AND users.distributor <> 0" : "AND users.distributor = 0" : "";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT month21, COUNT(orderid) AS numOfOrders FROM 
                                        (
                                        SELECT
                                        order_header.orderid, (((year(MAX(`porder_header`.`po_req_etd`)) - 2000) * 100) + month(MAX(`porder_header`.`po_req_etd`))) AS month21
                                        FROM
                                        order_header
                                        INNER JOIN porder_header ON porder_header.soorderid = order_header.orderid INNER JOIN users ON order_header.userid1 = users.user_id
                                        WHERE order_header.userid1 NOT IN (227) AND order_header.`status` NOT IN ('X','Y') 
                                        AND COALESCE(order_header.container_type,0) {3} IN ({4}) AND COALESCE(order_header.combined_order,0) <= 0
                                        {2} {0} {1} 
                                        AND EXISTS (SELECT order_lines.linenum 
                                                    FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                                                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id AND mast_products.category1 <> 13 {5}
						                            WHERE order_lines.orderid = order_header.orderid)
                                        GROUP BY
                                        order_header.orderid ORDER BY month21, orderid) AS orders
                                        WHERE month21 BETWEEN @from AND @to 
                                        GROUP BY month21", GetCountryCondition(countryFilter),brandsClause,
                                        HandleClients(cmd,"users.cprod_user"),
                                        excludedContainerTypes != null ? "NOT" : "",
                                        Utilities.CreateParametersFromIdList(cmd,excludedContainerTypes ?? includedContainerTypes,"containerType"),
                                        factoryIds != null ? string.Format(" AND mast_products.factory_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,factoryIds,"factid")) : "");
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new SalesByMonth {Month21 = Convert.ToInt32(dr["month21"]), numOfOrders = Convert.ToInt32(dr["numOfOrders"])});
                }
                dr.Close();
            }
            return result;
        }

        public static List<SalesByMonth> GetProfitByMonth(int from, int to)
        {
            var result = new List<SalesByMonth>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        @"SELECT  month21, SUM(totalgp) as total FROM brand_sales_analysis_gp WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 GROUP BY month21 ",
                        conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new SalesByMonth { Month21 = Convert.ToInt32(dr["month21"]), Amount = (double)dr["total"] });
                }
                dr.Close();
            }
            
            return result;
        }

        public static List<BrandSalesByMonth> GetBrandSalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<BrandSalesByMonth>();
            string view = brands ? "brand_sales_analysis2" : "brs_sales_analysis2";
            string groupField = brands ? "brandname" : "cat1_name";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT {3}, month21, SUM(rowprice_gbp) as total, COUNT(DISTINCT orderid) AS numOfOrders FROM {1} WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} {2} GROUP BY {3},month21 ",
                        GetCountryCondition(countryFilter), view,
                        HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients),groupField);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new BrandSalesByMonth {brandname = string.Empty+dr[groupField], Month21 = Convert.ToInt32(dr["month21"]), Amount = (double)dr["total"], numOfOrders = Convert.ToInt32(dr["numOfOrders"]) });
                }
                dr.Close();
            }
            return result;
        }

        public static List<AnalyticsCategorySummaryRow> GetAnalyticsCategorySummary(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<AnalyticsCategorySummaryRow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT analytics_category,analytics_option, SUM(orderqty) as total FROM {1}  WHERE month21 BETWEEN @from AND @to AND analytics_category IS NOT NULL
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} {2} GROUP BY analytics_category,analytics_option ",
                        GetCountryCondition(countryFilter), "brand_sales_analysis2",
                        HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new AnalyticsCategorySummaryRow {analytics_category_id  = (int) dr["analytics_category"],analytics_option_id = Utilities.FromDbValue<int>(dr["analytics_option"]), OrderQty = (double)dr["total"] });
                }
                dr.Close();
            }
            return result;
        }

        public static List<Category1SalesByMonth> GetCategory1SalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
        {
            var result = new List<Category1SalesByMonth>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        string.Format(@"SELECT category1,cat1_name, SUM(rowprice_gbp) as total
                            FROM brand_sales_analysis2 WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} AND (brand_user_id = @userid OR @userid IS NULL) GROUP BY category1,cat1_name ", GetCountryCondition(countryFilter)),
                        conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", userid != null ? (object) userid : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Category1SalesByMonth { category_id = (int)dr["category1"], catname = string.Empty + dr["cat1_name"], Amount = (double)dr["total"] });
                }
                dr.Close();
            }
            return result;
        }

        public static List<Category1> GetCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
        {
            var result = new List<Category1>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        string.Format(@"SELECT DISTINCT category1,cat1_name FROM brand_sales_analysis2 WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL)  {0} GROUP BY brandname,month21 ", GetCountryCondition(countryFilter)),
                        conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", userid != null ? (object)userid : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Category1{ cat1_name = string.Empty + dr["cat1_name"], category1_id = (int) dr["category1"]});
                }
                dr.Close();
            }
            return result;

        }

        public static List<Category1SalesByMonth> GetSubCategorySalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
        {
            var result = new List<Category1SalesByMonth>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        string.Format(@"SELECT COALESCE(brand_sub_id,0) AS brand_sub_id,COALESCE(brand_sub_desc,'') AS brand_sub_desc, SUM(rowprice_gbp) as total
                            FROM brand_sales_analysis2 LEFT OUTER JOIN brand_categories_sub ON brand_sales_analysis2.cprod_brand_subcat = brand_categories_sub.brand_sub_id  WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} AND (brand_user_id = @userid OR @userid IS NULL) GROUP BY COALESCE(brand_sub_id,0),COALESCE(brand_sub_desc,'') ", GetCountryCondition(countryFilter)),
                        conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", userid != null ? (object)userid : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Category1SalesByMonth { category_id = Convert.ToInt32(dr["brand_sub_id"]), catname = string.Empty + dr["brand_sub_desc"], Amount = (double)dr["total"] });
                }
                dr.Close();
            }
            return result;
        }

        public static List<Category1> GetSubCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
        {
            var result = new List<Category1>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        string.Format(@"SELECT DISTINCT COALESCE(brand_sub_id,0) AS brand_sub_id,COALESCE(brand_sub_desc,'') AS brand_sub_desc FROM brand_sales_analysis2 LEFT OUTER JOIN brand_categories_sub ON brand_sales_analysis2.cprod_brand_subcat = brand_categories_sub.brand_sub_id
                            WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL)  {0} ", GetCountryCondition(countryFilter)),
                        conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", userid != null ? (object)userid : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Category1 { cat1_name = string.Empty + dr["brand_sub_desc"], category1_id = Convert.ToInt32(dr["brand_sub_id"]) });
                }
                dr.Close();
            }
            return result;

        }

        public static List<CustomerSalesByMonth> GetCustomerSalesByMonth(int @from, int to, bool UK = true, CountryFilter countryFilter = CountryFilter.UKOnly, int? userId = null, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<CustomerSalesByMonth>();
            UK = countryFilter != CountryFilter.NonUK && UK;
            string view = brands ? "brand_sales_analysis2" : "brs_sales_analysis2";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand("", conn);
                cmd.CommandText = string.Format(
                    @"SELECT userid1,client_name,oem_flag, SUM(rowprice_gbp) as total FROM {2} WHERE month21 BETWEEN @from AND @to 
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL) {1} 
                        AND user_country {0} 'GB' {3} GROUP BY userid1,client_name,oem_flag",
                    UK ? "=" : "<>", GetCountryCondition(countryFilter),view,HandleClients(cmd,includedClients:includedClients,excludedClients:excludedClients));
                
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", userId != null ? (object)userId : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new CustomerSalesByMonth {isUK  = UK,isOEM = Utilities.BoolFromLong(dr["oem_flag"]) ?? false ,Amount = (double)dr["total"],client_id = (int) dr["userid1"], customer_name = string.Empty + dr["client_name"]});
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSales> GetProductSales(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null, IList<int> factories = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var result = new List<ProductSales>();
            var groupfields = factories != null ? "factory_ref,asaq_name" : "cprod_code1,cprod_name,brand_code";
            var selectFields = factories != null ? "factory_ref AS cprod_code1,asaq_name AS cprod_name, '' AS brand_code" : "cprod_code1,cprod_name,brand_code";
            var rangeCriteria = from != null ? "month21 BETWEEN @from AND @to" : "po_req_etd BETWEEN @from AND @to";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                        string.Format(@"SELECT {2}, sum(sogbp*orderqty) as total,SUM(poprice*orderqty) AS po_total, sum(orderqty) as numOfProducts, COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
                        FROM brand_sales_analysis_product2 WHERE {4} 
                           AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL) {0} {1} GROUP BY {3}", 
                            GetCountryCondition(countryFilter),factories != null ? string.Format(" AND factory_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,factories)) : "",selectFields,groupfields,rangeCriteria);

                cmd.Parameters.AddWithValue("@from", from != null ? (object) from : fromDate);
                cmd.Parameters.AddWithValue("@to", to != null ? (object) to : toDate);
                cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSales { Amount = (double)dr["total"],POAmount = (double) dr["po_total"],cprod_name = string.Empty + dr["cprod_name"],
                                                brand_code = string.Empty + dr["brand_code"],Margin = (double) dr["margin"], numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
                                                cprod_code = string.Empty + dr["cprod_code1"]});
                }
                dr.Close();
            }
            return result;
        }

        public static List<CountrySales> GetCountrySales(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, IList<int> factoryIds = null )
        {
            var result = new List<CountrySales>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand("", conn);
                cmd.CommandText = 
                        string.Format(@"SELECT user_country,countries.CountryName, sum(po_rowprice) as total
                        FROM factory_sales_analysis INNER JOIN countries ON factory_sales_analysis.user_country = countries.ISO2 WHERE month21 BETWEEN @from AND @to AND customer_code <> 'te' AND customer_code <> ''
                           AND rowprice > 0 AND category1 <> 13 {0} {1} GROUP BY user_country", 
                          GetCountryCondition(countryFilter),factoryIds != null ? string.Format(" AND factory_id IN ({0})",Utilities.CreateParametersFromIdList(cmd,factoryIds)) : "");

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new CountrySales
                    {
                        Amount = (double)dr["total"],
                        user_country = string.Empty + dr["user_country"],
                        country_name = string.Empty + dr["countryName"]
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<Cust_products> GetNonSelling(int from, int to, int? brand_user_id = null)
        {
            var result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        @"SELECT * FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE category1 <> 13 AND (brand_user_id = @userid OR @userid IS NULL) 
                          AND cprod_status <> 'D' AND cprod_brand_cat NOT IN(115,207,307,608,708)
                          AND cprod_id NOT IN (SELECT DISTINCT cprod_id FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to ) ", conn);

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object)brand_user_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(Cust_productsDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static double GetTotalClaims(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            double result;
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT SUM(credit_value*return_qty) FROM returns INNER JOIN users ON returns.client_id = users.user_id
                          WHERE status1 = 1 and claim_type <> 5 and decision_final = 1 {0} {1} {2}
                          AND (((year(`returns`.`request_date`) - 2000) * 100) + month(`returns`.`request_date`)) BETWEEN @from AND @to", GetCountryCondition(countryFilter),brandsClause,HandleClients(cmd,"client_id",includedClients,excludedClients));

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                result = Utilities.FromDbValue<double>(cmd.ExecuteScalar()) ?? 0;
            }

            return result;
        }

        public static List<Returns> GetRespondedClaims(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<Returns>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = 
                    new MySqlCommand("",conn);
                cmd.CommandText =
                    string.Format(@"SELECT returns.* FROM returns INNER JOIN users ON returns.client_id = users.user_id 
                        WHERE status1 = 1 and client_id <> 184 AND cc_response_date IS NOT NULL {1}  {0} {2}
                          AND (((year(`returns`.`cc_response_date`) - 2000) * 100) + month(`returns`.`cc_response_date`)) BETWEEN @from AND @to",
                                  GetCountryCondition(countryFilter),brandsClause,HandleClients(cmd,"client_id",includedClients,excludedClients));

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(ReturnsDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }

            return result;
        }

        public static List<OrderStats> GetOrderStatByDistributor(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var result = new List<OrderStats>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //First, new orders
                var cmd =
                      new MySqlCommand(string.Format(@"SELECT orders.customer_code, COUNT(*) AS numOfOrders, COALESCE(SUM(orders.order_amount),0) AS Amount FROM 
                                        (SELECT order_header.*,SUM(order_lines.unitprice*(CASE unitcurrency WHEN 0 THEN 1/1.6 ELSE 1 END)*order_lines.orderqty) AS order_amount, users.customer_code
                                        FROM
                                        order_lines
                                        INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                        INNER JOIN users ON users.user_id = order_header.userid1
                                        WHERE users.distributor > 0 AND  order_header.`status` NOT IN ('X','Y') AND order_header.orderdate BETWEEN @from AND @to  {0}
                                        GROUP BY order_header.orderid) AS orders
                                        GROUP BY orders.customer_code", GetCountryCondition(countryFilter)), conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderStats{client_code = string.Empty + dr["customer_code"],newOrdersCount = Convert.ToInt32(dr["numOfOrders"]),
                                                newOrdersAmount = (double) dr["Amount"]});
                }
                dr.Close();

                //orders with etd in this period and beyond
                cmd.CommandText = string.Format(@"SELECT orders.customer_code, COUNT(CASE WHEN orders.po_req_etd <= @today THEN 1 ELSE NULL END ) AS numOfOrdersShipped, COALESCE(SUM(CASE WHEN orders.po_req_etd <= @today THEN orders.order_amount ELSE NULL END ),0) AS AmountShipped,
                     COUNT(CASE WHEN orders.po_req_etd > @today THEN 1 ELSE NULL END ) AS numOfOrdersOutstanding, COALESCE(SUM(CASE WHEN orders.po_req_etd > @today THEN orders.order_amount ELSE NULL END ),0) AS AmountOutstanding FROM 
	                    (SELECT order_header.*,SUM(order_lines.unitprice*(CASE unitcurrency WHEN 0 THEN 1/1.6 ELSE 1 END)*order_lines.orderqty) AS order_amount, users.customer_code,
                    (SELECT MAX(po_req_etd) FROM porder_header WHERE porder_header.soorderid = order_header.orderid) AS po_req_etd
	                    FROM
	                    order_lines
	                    INNER JOIN order_header ON order_header.orderid = order_lines.orderid
	                    INNER JOIN users ON users.user_id = order_header.userid1
	                    WHERE order_header.`status` NOT IN ('X','Y') 
                        AND (SELECT MAX(po_req_etd) FROM porder_header WHERE porder_header.soorderid = order_header.orderid) >=@from 
                        {0} AND users.distributor > 0
	                    GROUP BY order_header.orderid) AS orders
	                    GROUP BY orders.customer_code
                    ORDER BY orders.customer_code",GetCountryCondition(countryFilter));
                cmd.Parameters.AddWithValue("@today", DateTime.Today);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var customer_code = string.Empty + dr["customer_code"];
                    var orderstat = result.FirstOrDefault(o => o.client_code == customer_code);
                    if (orderstat == null)
                    {
                        orderstat = new OrderStats { client_code = customer_code };
                        result.Add(orderstat);
                    }
                        
                    orderstat.shippedOrdersCount = Convert.ToInt32(dr["numOfOrdersShipped"]);
                    orderstat.shippedOrdersAmount = (double)dr["AmountShipped"];
                    orderstat.outstandingOrdersCount = Convert.ToInt32(dr["numOfOrdersOutstanding"]);
                    orderstat.outstandingOrdersAmount = (double) dr["AmountOutstanding"];
                }
                dr.Close();

            }
            return result;
        }

        public static List<OrderProductGroupStats> GetOrderProductGroupStats_New(DateTime from, DateTime to,
                                                                                 CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderProductGroupStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                //new orders
                var cmd =
                    new MySqlCommand("",conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, COUNT(*) AS numOfLines
		                FROM order_lines
		                INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
		                INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
		                 INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
		                INNER JOIN users ON users.user_id = order_header.userid1
		                WHERE order_header.`status` NOT IN ('X','Y') {0} {2} {1}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        AND order_header.orderdate BETWEEN @from AND @to 
		                GROUP BY order_header.orderid,users.customer_code, fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) ORDER BY order_header.orderid, mast_products.product_group",
                        GetCountryCondition(countryFilter), brandsClause,HandleClients(cmd,includedClients:includedClients,excludedClients:excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                //Collect order records to determine SABC type of order
                var orderList = new List<OrderStatTempRecord>();
                while (dr.Read())
                {
                    int order_id = (int) dr["orderid"];
                    var orderRecord = orderList.FirstOrDefault(o => o.orderid == order_id);
                    if (orderRecord == null)
                    {
                        orderRecord = new OrderStatTempRecord
                            {
                                orderid = order_id,
                                customer_code = string.Empty + dr["customer_code"],
                                productgroup_lines = new Dictionary<string, int>()
                            };
                        orderList.Add(orderRecord);
                    }
                    orderRecord.productgroup_lines[string.Empty + dr["product_group"]] =
                        Convert.ToInt32(dr["numOfLines"]);
                }
                dr.Close();
                DetermineProductGroup(orderList);
                result = orderList.GroupBy(o => new {o.customer_code, o.product_group}).
                                   Select(
                                       g =>
                                       new OrderProductGroupStats
                                           {
                                               client_code = g.Key.customer_code,
                                               product_group = g.Key.product_group,
                                               orders_count = g.Count()
                                           }).ToList();
            }
            return result;
        }

        public static List<OrderProductGroupStats> GetOrderProductGroupStats_ETA(DateTime from, DateTime to,
                                                                                 CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderProductGroupStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders

                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, 
                        order_header.req_eta, COUNT(*) AS numOfLines
		                FROM order_lines
		                INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
		                INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
		                 INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
		                INNER JOIN users ON users.user_id = order_header.userid1
		                WHERE order_header.`status` NOT IN ('X','Y') {0} {2} {1}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        AND req_eta BETWEEN @from AND @to
		                GROUP BY order_header.orderid,users.customer_code, fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) ORDER BY order_header.orderid, mast_products.product_group",
                        GetCountryCondition(countryFilter), brandsClause,
                        HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var orderList = new List<OrderStatTempRecord>();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int order_id = (int)dr["orderid"];
                    var orderRecord = orderList.FirstOrDefault(o => o.orderid == order_id);
                    if (orderRecord == null)
                    {
                        orderRecord = new OrderStatTempRecord
                        {
                            orderid = order_id,
                            customer_code = string.Empty + dr["customer_code"],
                            productgroup_lines = new Dictionary<string, int>()
                        };
                        orderList.Add(orderRecord);
                    }
                    orderRecord.productgroup_lines[string.Empty + dr["product_group"]] =
                        Convert.ToInt32(dr["numOfLines"]);
                }
                dr.Close();
                DetermineProductGroup(orderList);
                result = orderList.GroupBy(o => new { o.customer_code, o.product_group }).
                                   Select(
                                       g =>
                                       new OrderProductGroupStats
                                       {
                                           client_code = g.Key.customer_code,
                                           product_group = g.Key.product_group,
                                           orders_count = g.Count()
                                       }).ToList();

            }
            return result;
        }

        public static List<OrderProductGroupStats> GetOrderProductGroupStats_Out(DateTime from,DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
                                                                                 CountryFilter countryFilter = CountryFilter.UKOnly,bool brands = true,int[] includedClients = null,int[] excludedClients=null)
        {
            var result = new List<OrderProductGroupStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders

                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, 
                        order_header.req_eta, COUNT(*) AS numOfLines
		                FROM order_lines
		                INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
		                INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
		                 INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
		                INNER JOIN users ON users.user_id = order_header.userid1
		                WHERE order_header.`status` NOT IN ('X','Y') {0} {3} {2}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        AND order_header.req_eta > @from {1}
		                GROUP BY order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) ORDER BY order_header.orderid, mast_products.product_group",
                        GetCountryCondition(countryFilter),
                        (mode == OutstandingOrdersMode.Production
                             ? " AND (porder_header.po_req_etd > @from AND order_header.orderdate NOT BETWEEN @new_from AND @new_to)"
                             : mode == OutstandingOrdersMode.Transit ? "AND porder_header.po_req_etd <= @from" : ""), brandsClause, HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                var orderList = new List<OrderStatTempRecord>();
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@new_from", new_from);
                cmd.Parameters.AddWithValue("@new_to", new_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int order_id = (int)dr["orderid"];
                    var orderRecord = orderList.FirstOrDefault(o => o.orderid == order_id);
                    if (orderRecord == null)
                    {
                        orderRecord = new OrderStatTempRecord
                        {
                            orderid = order_id,
                            customer_code = string.Empty + dr["customer_code"],
                            productgroup_lines = new Dictionary<string, int>()
                        };
                        orderList.Add(orderRecord);
                    }
                    orderRecord.productgroup_lines[string.Empty + dr["product_group"]] =
                        Convert.ToInt32(dr["numOfLines"]);
                }
                dr.Close();
                DetermineProductGroup(orderList);
                result = orderList.GroupBy(o => new { o.customer_code, o.product_group }).
                                   Select(
                                       g =>
                                       new OrderProductGroupStats
                                       {
                                           client_code = g.Key.customer_code,
                                           product_group = g.Key.product_group,
                                           orders_count = g.Count()
                                       }).ToList();

            }
            return result;
        }

        public static List<OrderBrandsStats> GetOrderBrandStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderBrandsStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
                        (SELECT orders.orderid, orders.customer_code,COUNT(*) AS numOfBrands
                            FROM 
                            (SELECT order_header.orderid, brands.brand_id, users.customer_code
                            FROM order_lines
                            INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
		                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                            INNER JOIN brands ON brands.user_id = cust_products.cprod_user
                            INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                            INNER JOIN users ON users.user_id = order_header.userid1
                            WHERE order_header.orderdate BETWEEN @from AND @to 
                            AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                             {1} AND  order_header.`status` NOT IN ('X','Y') {0} {2}
                            GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
                        GROUP BY orders.customer_code, orders.orderid) AS obsummary
                    GROUP BY obsummary.customer_code, obsummary.numOfBrands
                    ORDER BY obsummary.customer_code, obsummary.numOfBrands", GetCountryCondition(countryFilter), brandsClause, HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderBrandsStats{client_code = string.Empty + dr["customer_code"],brandCount = Convert.ToInt32(dr["numOfBrands"]),orderCount = Convert.ToInt32(dr["numOfOrders"])});
                }

            }
            return result;
        }

        public static List<OrderBrandsStats> GetOrderBrandStats_Out(DateTime from,DateTime new_from, DateTime new_to,OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
            CountryFilter countryFilter = CountryFilter.UKOnly,bool brands = true,int[] includedClients = null,int[] excludedClients=null)
        {
            var result = new List<OrderBrandsStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
                        (SELECT orders.orderid, orders.customer_code,COUNT(*) AS numOfBrands
                            FROM 
                            (SELECT order_header.orderid, brands.brand_id, users.customer_code
                            FROM order_lines
                            INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                            INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
                            INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
		                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                            INNER JOIN brands ON brands.user_id = cust_products.cprod_user
                            INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                            INNER JOIN users ON users.user_id = order_header.userid1
                            WHERE 
                            order_header.`status` NOT IN ('X','Y') {0} {3} {2}
                            AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                            AND req_eta >= @from {1}
                            GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
                        GROUP BY orders.customer_code, orders.orderid) AS obsummary
                    GROUP BY obsummary.customer_code, obsummary.numOfBrands
                    ORDER BY obsummary.customer_code, obsummary.numOfBrands", GetCountryCondition(countryFilter),
                                  (mode == OutstandingOrdersMode.Production
                                       ? "AND (porder_header.po_req_etd > @from AND order_header.orderdate NOT BETWEEN @new_from AND @new_to)"
                                       : mode == OutstandingOrdersMode.Transit
                                             ? "AND porder_header.po_req_etd <= @from"
                                             : ""), brandsClause,
                                  HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@new_from", new_from);
                cmd.Parameters.AddWithValue("@new_to", new_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderBrandsStats { client_code = string.Empty + dr["customer_code"], brandCount = Convert.ToInt32(dr["numOfBrands"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderBrandsStats> GetOrderBrandStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderBrandsStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
                        (SELECT orders.orderid, orders.customer_code,COUNT(*) AS numOfBrands
                            FROM 
                            (SELECT order_header.orderid, brands.brand_id, users.customer_code
                            FROM order_lines
                            INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                            INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                            INNER JOIN brands ON brands.user_id = cust_products.cprod_user
                            INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                            INNER JOIN users ON users.user_id = order_header.userid1
                            WHERE 
                             order_header.`status` NOT IN ('X','Y') {0} {2} {1}
                            AND req_eta BETWEEN @from AND @to
                            AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                            GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
                        GROUP BY orders.customer_code, orders.orderid) AS obsummary
                    GROUP BY obsummary.customer_code, obsummary.numOfBrands
                    ORDER BY obsummary.customer_code, obsummary.numOfBrands", GetCountryCondition(countryFilter),
                                  brandsClause,
                                  HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderBrandsStats { client_code = string.Empty + dr["customer_code"], brandCount = Convert.ToInt32(dr["numOfBrands"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderFactoriesStats> GetOrderFactoryStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,bool brands = true,int[] includedClients = null,int[] excludedClients=null)
        {
            var result = new List<OrderFactoriesStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
                        (SELECT orders.orderid, orders.customer_code,COUNT(*) AS numOfFactories
                        FROM (SELECT
                        order_header.orderid,
                        factory.user_id,
                        users.customer_code
                        FROM
                        order_lines
                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN users ON users.user_id = order_header.userid1
                        WHERE order_header.orderdate BETWEEN @from AND @to
                        {1} AND  order_header.`status` NOT IN ('X','Y') {0} {2}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        GROUP BY
                        order_lines.orderid,
                        users.customer_code,
                        factory.user_id) AS orders
                        GROUP BY orders.customer_code, orders.orderid) AS obsummary
                        GROUP BY obsummary.customer_code, obsummary.numOfFactories
                        ORDER BY obsummary.customer_code, obsummary.numOfFactories",
                                  GetCountryCondition(countryFilter, "users."), brandsClause,
                                  HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderFactoriesStats { client_code = string.Empty + dr["customer_code"], factoryCount = Convert.ToInt32(dr["numOfFactories"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderFactoriesStats> GetOrderFactoryStats_Out(DateTime from,DateTime new_from, DateTime new_to,OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
            CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderFactoriesStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
                        (SELECT orders.orderid, orders.customer_code,COUNT(*) AS numOfFactories
                        FROM (SELECT
                        order_header.orderid,
                        factory.user_id,
                        users.customer_code
                        FROM
                        order_lines
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN users ON users.user_id = order_header.userid1
                        WHERE req_eta >= @from {1}
                        {2} AND  order_header.`status` NOT IN ('X','Y') {0} {3}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        GROUP BY
                        order_lines.orderid,
                        users.customer_code,
                        factory.user_id) AS orders
                        GROUP BY orders.customer_code, orders.orderid) AS obsummary
                        GROUP BY obsummary.customer_code, obsummary.numOfFactories
                        ORDER BY obsummary.customer_code, obsummary.numOfFactories",
                                  GetCountryCondition(countryFilter, "users."),
                                  (mode == OutstandingOrdersMode.Production
                                       ? " AND (porder_header.po_req_etd > @from AND order_header.orderdate NOT BETWEEN @new_from AND @new_to)"
                                       : mode == OutstandingOrdersMode.Transit
                                             ? "AND porder_header.po_req_etd <= @from"
                                             : ""), brandsClause,
                                  HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@new_from", new_from);
                cmd.Parameters.AddWithValue("@new_to", new_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderFactoriesStats { client_code = string.Empty + dr["customer_code"], factoryCount = Convert.ToInt32(dr["numOfFactories"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderFactoriesStats> GetOrderFactoryStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderFactoriesStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
                        (SELECT orders.orderid, orders.customer_code,COUNT(*) AS numOfFactories
                        FROM (SELECT
                        order_header.orderid,
                        factory.user_id,
                        users.customer_code
                        FROM
                        order_lines
                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN users ON users.user_id = order_header.userid1
                        WHERE req_eta BETWEEN @from AND @to
                        {1} AND  order_header.`status` NOT IN ('X','Y') {0} {2}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        GROUP BY
                        order_lines.orderid,
                        users.customer_code,
                        factory.user_id) AS orders
                        GROUP BY orders.customer_code, orders.orderid) AS obsummary
                        GROUP BY obsummary.customer_code, obsummary.numOfFactories
                        ORDER BY obsummary.customer_code, obsummary.numOfFactories",
                                  GetCountryCondition(countryFilter, "users."), brandsClause,HandleClients(cmd,includedClients:includedClients,excludedClients:excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderFactoriesStats { client_code = string.Empty + dr["customer_code"], factoryCount = Convert.ToInt32(dr["numOfFactories"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderLocationStats> GetOrderLocationStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderLocationStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
                        (SELECT
                        order_header.orderid,
                        factory.consolidated_port,
                        users.customer_code
                        FROM
                        order_lines
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON (CASE WHEN order_header.loading_factory > 0 THEN order_header.loading_factory ELSE mast_products.factory_id END) = factory.user_id
                        INNER JOIN users ON users.user_id = order_header.userid1
                        WHERE order_header.orderdate BETWEEN @from AND @to
                        {1} AND  order_header.`status` NOT IN ('X','Y') {0} {2}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        GROUP BY
                        order_lines.orderid,
                        users.customer_code,
                        factory.consolidated_port) AS orders
                        GROUP BY orders.customer_code, orders.consolidated_port
                        ORDER BY orders.customer_code, orders.consolidated_port",
                                  GetCountryCondition(countryFilter, "users."), brandsClause, HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderLocationStats { client_code = string.Empty + dr["customer_code"], location = Convert.ToInt32(dr["consolidated_port"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderLocationStats> GetOrderLocationStats_Out(DateTime from,DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
            CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderLocationStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
                        (SELECT
                        order_header.orderid,
                        factory.consolidated_port,
                        users.customer_code
                        FROM
                        order_lines
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON (CASE WHEN order_header.loading_factory > 0 THEN order_header.loading_factory ELSE mast_products.factory_id END) = factory.user_id
                        INNER JOIN users ON users.user_id = order_header.userid1
                        WHERE req_eta >= @from {1}
                        {2} AND  order_header.`status` NOT IN ('X','Y') {0} {3}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        GROUP BY
                        order_lines.orderid,
                        users.customer_code,
                        factory.consolidated_port) AS orders
                        GROUP BY orders.customer_code, orders.consolidated_port
                        ORDER BY orders.customer_code, orders.consolidated_port",
                                  GetCountryCondition(countryFilter, "users."),
                                  (mode == OutstandingOrdersMode.Production
                                       ? "AND (porder_header.po_req_etd > @from AND order_header.orderdate NOT BETWEEN @new_from AND @new_to)"
                                       : mode == OutstandingOrdersMode.Transit
                                             ? "AND porder_header.po_req_etd <= @from"
                                             : ""), brandsClause, HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@new_from", new_from);
                cmd.Parameters.AddWithValue("@new_to", new_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderLocationStats { client_code = string.Empty + dr["customer_code"], location = Convert.ToInt32(dr["consolidated_port"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        public static List<OrderLocationStats> GetOrderLocationStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
        {
            var result = new List<OrderLocationStats>();
            var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                //new orders
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
                        (SELECT
                        order_header.orderid,
                        factory.consolidated_port,
                        users.customer_code
                        FROM
                        order_lines
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON (CASE WHEN order_header.loading_factory > 0 THEN order_header.loading_factory ELSE mast_products.factory_id END) = factory.user_id
                        INNER JOIN users ON users.user_id = order_header.userid1
                        WHERE req_eta BETWEEN @from AND @to
                        {1} AND  order_header.`status` NOT IN ('X','Y') {0} {2}
                        AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) and mast_products.category1 <> 13
                        GROUP BY
                        order_lines.orderid,
                        users.customer_code,
                        factory.consolidated_port) AS orders
                        GROUP BY orders.customer_code, orders.consolidated_port
                        ORDER BY orders.customer_code, orders.consolidated_port",
                                  GetCountryCondition(countryFilter, "users."), brandsClause, HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients));
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderLocationStats { client_code = string.Empty + dr["customer_code"], location = Convert.ToInt32(dr["consolidated_port"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
                }

            }
            return result;
        }

        private static void DetermineProductGroup(List<OrderStatTempRecord> orderList)
        {
            foreach (var orderStatTempRecord in orderList)
            {
                orderStatTempRecord.product_group = orderStatTempRecord.productgroup_lines.Where(kv => kv.Value > 0)
                                       .OrderByDescending(kv => kv.Key == "S" ? "0" : kv.Key)
                                       .Take(1).First().Key;
            }
        }

        public static List<ProductStats> GetProductStats()
        {
            var result = new List<ProductStats>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT brands.brandname, mast_products.product_group, COUNT(*) AS numOfProducts
                        FROM brands
                        INNER JOIN cust_products ON cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
                        INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                        WHERE NOT EXISTS (SELECT cust_products.cprod_id FROM cust_products other_cp WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND cust_products.cprod_user <> other_cp.cprod_user AND other_cp.cprod_status <> 'D') AND mast_products.category1 <> 13
                        AND cust_products.cprod_status <> 'D' AND brands.eb_brand = 1 AND COALESCE(cust_products.report_exception,0) = 0 AND cprod_brand_cat NOT IN (115,207,307,608,708)
                        GROUP BY brands.brandname, mast_products.product_group
                        UNION 
                        SELECT 'Universal' AS brandname, mast_products.product_group, COUNT(*) AS numOfProducts
                        FROM brands
                        INNER JOIN cust_products ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
                        INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                        WHERE EXISTS (SELECT cust_products.cprod_id FROM cust_products other_cp WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND cust_products.cprod_user <> other_cp.cprod_user AND other_cp.cprod_status <> 'D'
                        AND other_cp.cprod_user IN (SELECT brands.user_id FROM brands WHERE eb_brand = 1)
                        ) AND mast_products.category1 <> 13 AND brands.eb_brand = 1 AND COALESCE(cust_products.report_exception,0) = 0 AND cprod_brand_cat NOT IN (115,207,307,608,708)
                        AND cust_products.cprod_status <> 'D'
                        GROUP BY mast_products.product_group
                        ORDER BY brandname ASC, product_group ASC", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductStats{brandname = string.Empty + dr["brandname"],product_group = string.Empty + dr["product_group"], numOfProducts = Convert.ToInt32(dr["numOfProducts"])});
                }
                dr.Close();
                result = result.OrderBy(r => r.brandname == "Universal" ? "ZZ" : r.brandname).ToList();
            }
            return result;
        }

        public static List<ProductLocationStats> GetProductLocationStats(string product_group)
        {
            var result = new List<ProductLocationStats>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT cust_products.cprod_code1, cust_products.cprod_name,users.consolidated_port,
                                            (SELECT GROUP_CONCAT(other_mp.product_group) FROM  cust_products other_cp
                                            INNER JOIN mast_products other_mp ON other_mp.mast_id = other_cp.cprod_mast
                                            INNER JOIN users other_fact ON other_fact.user_id = other_mp.factory_id
                                            INNER JOIN brands other_b ON other_cp.cprod_user = other_b.user_id AND other_b.eb_brand = 1
                                            WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND other_cp.cprod_status <> 'D' AND other_mp.category1 <> 13 AND other_cp.cprod_brand_cat NOT IN (115,207,307,608,708) AND COALESCE(other_cp.report_exception,0) = 0
                                            AND cust_products.cprod_id <> other_cp.cprod_id AND other_fact.consolidated_port <> users.consolidated_port AND other_mp.product_group <> @group) AS productgroups_other
                                            FROM  cust_products
                                            INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                            INNER JOIN users ON users.user_id = mast_products.factory_id
                                            INNER JOIN brands ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
                                            WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 AND cprod_brand_cat NOT IN (115,207,307,608,708) AND COALESCE(cust_products.report_exception,0) = 0
                                            AND mast_products.product_group = @group", conn);
                cmd.Parameters.AddWithValue("@group", product_group);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductLocationStats
                    {
                        cprod_code = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                        productgroup_others = dr["productgroups_other"] != DBNull.Value ? (string.Empty + dr["productgroups_other"]).Split(',') : null
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductLocationStatsSummary> GetProductLocationStatsSummary(string product_group)
        {
            var result = new List<ProductLocationStatsSummary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT brands.brandname,users.consolidated_port,COUNT(*) AS numOfProducts
                                            FROM cust_products
                                            INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                            INNER JOIN users ON users.user_id = mast_products.factory_id
                                            INNER JOIN brands ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
                                            WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 AND cprod_brand_cat NOT IN (115,207,307,608,708) AND COALESCE(cust_products.report_exception,0) = 0 AND 
                                            mast_products.product_group = @group
                                            GROUP BY brands.brandname, users.consolidated_port", conn);
                cmd.Parameters.AddWithValue("@group", product_group);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductLocationStatsSummary
                    {
                        brandname = string.Empty + dr["brandname"],
                        location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                        numOfProducts = Convert.ToInt32(dr["numOfProducts"])
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductLocationStats> GetAlternateProductsStats(string product_group)
        {
            var result = new List<ProductLocationStats>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT cust_products.cprod_code1, cust_products.cprod_name,users.consolidated_port, mast_products.product_group
                                            FROM  cust_products
                                            INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                            INNER JOIN users ON users.user_id = mast_products.factory_id
                                            INNER JOIN brands ON cust_products.cprod_user = brands.user_id AND brands.eb_brand = 1
                                            WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 AND cprod_brand_cat NOT IN (115,207,307,608,708) AND COALESCE(cust_products.report_exception,0) = 0 AND
																						EXISTS (SELECT other_mp.product_group FROM cust_products other_cp
                                            INNER JOIN mast_products other_mp ON other_mp.mast_id = other_cp.cprod_mast
                                            INNER JOIN users other_fact ON other_fact.user_id = other_mp.factory_id
                                            INNER JOIN brands other_b ON other_cp.cprod_user = other_b.user_id AND other_b.eb_brand = 1
                                            WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND other_cp.cprod_status <> 'D' AND other_mp.category1 <> 13 AND other_cp.cprod_brand_cat NOT IN (115,207,307,608,708) AND COALESCE(other_cp.report_exception,0) = 0
                                            AND cust_products.cprod_id <> other_cp.cprod_id AND other_fact.consolidated_port <> users.consolidated_port AND other_mp.product_group = @group) AND
                                            mast_products.product_group <> @group", conn);
                cmd.Parameters.AddWithValue("@group", product_group);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductLocationStats
                    {
                        cprod_code = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                        maxgroup = string.Empty + dr["product_group"]
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSales> GetTopNByBrands(int from, int to,CountryFilter countryFilter = CountryFilter.UKOnly )
        {
            var result = new List<ProductSales>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT brand_user_id,cprod_code1,cprod_name,brand_code, sum(sogbp*orderqty) as total, sum(orderqty) as numOfProducts, 
                        COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
                        FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
                           AND NOT EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 AND cust_products.brand_user_id <> brand_sales_analysis_product2.brand_user_id )
                           AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
                            AND cprod_brand_cat NOT IN(115,207,307,608,708,912)
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0  {0} 
                            GROUP BY brand_user_id,brand_code,cprod_code1,cprod_name
                            ORDER BY numOfProducts DESC
                            ", GetCountryCondition(countryFilter)), conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSales
                    {
                        Amount = (double)dr["total"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        brand_code = string.Empty + dr["brand_code"],
                        Margin = (double)dr["margin"],
                        numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
                        cprod_code = string.Empty + dr["cprod_code1"]
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSales> GetTopNUniversal(int n,int from, int to,CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var result = new List<ProductSales>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT cprod_code1,cprod_name,brand_code, sum(sogbp*orderqty) as total, sum(orderqty) as numOfProducts, 
                        COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
                        FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
                            AND EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id 
                                        WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 
                                        AND cust_products.brand_user_id <> brand_sales_analysis_product2.brand_user_id )
                           AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
                            AND cprod_brand_cat NOT IN(115,207,307,608,708,912)
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} 
                            GROUP BY cprod_code1,cprod_name
                            ORDER BY numOfProducts DESC LIMIT @limit
                            ", GetCountryCondition(countryFilter)), conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@limit", n);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSales
                    {
                        Amount = (double)dr["total"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        brand_code = string.Empty + dr["brand_code"],
                        Margin = (double)dr["margin"],
                        numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
                        cprod_code = string.Empty + dr["cprod_code1"]
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSales> GetTopForBrandCat(int n,int from, int to,int cprod_brand_cat, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var result = new List<ProductSales>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT cprod_code1,cprod_name,brand_code, sum(sogbp*orderqty) as total, sum(orderqty) as numOfProducts, 
                        COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
                        FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
                            AND EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 GROUP BY cprod_code1, brand_user_id HAVING COUNT(DISTINCT brand_user_id) = 1)
                           AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
                            AND cprod_brand_cat = @cprod_brand_cat
                            AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0  {0} 
                            GROUP BY brand_user_id,brand_code,cprod_code1,cprod_name
                            ORDER BY numOfProducts DESC LIMIT @limit
                            ", GetCountryCondition(countryFilter)), conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@limit", n);
                cmd.Parameters.AddWithValue("@cprod_brand_cat", cprod_brand_cat);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSales
                    {
                        Amount = (double)dr["total"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        brand_code = string.Empty + dr["brand_code"],
                        Margin = (double)dr["margin"],
                        numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
                        cprod_code = string.Empty + dr["cprod_code1"]
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSales> GetSalesForCategory(int brand_category)
        {
            var result = new List<ProductSales>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(@"SELECT cust_products.cprod_id, cust_products.cprod_name, SUM(order_lines.orderqty*order_lines.unitprice) AS sales
                                            FROM  cust_products
                                            INNER JOIN web_products ON web_products.web_component1 = cust_products.cprod_id
                                            INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id
                                            WHERE  web_products.web_category = @cat_id
                                            GROUP BY cust_products.cprod_id", conn);
                cmd.Parameters.AddWithValue("@cat_id", brand_category);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSales{cprod_id = (int) dr["cprod_id"],Amount = (double) dr["sales"]});
                }
            }
            return result;
        }

        public static List<ProductDisplayCount> GetDisplayCountForProducts(int brand_category)
        {
            var result = new List<ProductDisplayCount>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(@"SELECT cust_products.cprod_id, cust_products.cprod_name, COUNT(*) AS num
                                            FROM  cust_products
                                            INNER JOIN web_products ON web_products.web_component1 = cust_products.cprod_id
                                            INNER JOIN dealer_image_displays ON dealer_image_displays.web_unique = web_products.web_unique
                                            WHERE  web_products.web_category = @cat_id
                                            GROUP BY cust_products.cprod_id", conn);
                cmd.Parameters.AddWithValue("@cat_id", brand_category);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductDisplayCount { cprod_id = (int)dr["cprod_id"], DisplayCount = Convert.ToInt32(dr["num"]) });
                }
            }
            return result;
        }

        public static List<Company> GetFactoriesFromSales(int? month21From = null, int? month21To = null)
        {
            var result = new List<Company>();
            
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT users.* 
                                            FROM brand_sales_analysis2 INNER JOIN users ON brand_sales_analysis2.factory_id = users.user_id
                                            WHERE brand_sales_analysis2.customer_code <> 'te' AND brand_sales_analysis2.customer_code <> '' AND brand_sales_analysis2.customer_code <> 'NK2' AND (month21 >= @month21from OR @month21from IS NULL) 
                                            AND (month21 <= @month21to OR @month21to IS NULL) and category1 <> 13 
                                            group by factory_id DESC", conn);
                cmd.Parameters.AddWithValue("@month21from", month21From != null ? (object) month21From : DBNull.Value);
                cmd.Parameters.AddWithValue("@month21to", month21To != null ? (object)month21To : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(CompanyDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

       
    }
}
