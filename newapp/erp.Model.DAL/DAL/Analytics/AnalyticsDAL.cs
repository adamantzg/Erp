using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using erp.Model.DAL.Properties;
using Dapper;
using company.Common;

namespace erp.Model.DAL
{

    public enum BrandSalesGroupOption
    {
        Default,    //depeding on brand flag
        ByBrand
    }
	
	public class AnalyticsDAL
	{
	    public static string ExcludedCprodBrandCatsCondition = String.Format("AND cprod_brand_cat NOT IN({0})",ExcludedCprodBrandCats);
        public const string ExcludedCprodBrandCats = "115,207,307,608,708,912,8081";

        public static string AsiaCountries = "";

		public static List<SalesByMonth> GetSalesByMonth(int from, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null, 
			bool useETA=false, bool? brands = true,int[] includedClients = null,int[] excludedClients=null,List<int> factory_ids = null,
			int? brand_id = null, string excludedCustomers = "NK2", IList<int> includedNonDistributors = null, bool useOrderDate = false, 
			IList<int> productIds = null, bool factoryProducts = false, bool useCompanyPriceType = false )
		{
			var result = new List<SalesByMonth>();
			var monthField = useETA ? (useCompanyPriceType ? "month23" : "month22") : 
                                      (useOrderDate ? "month20" : "month21");
			var priceField = factory_ids != null ? "po_rowprice_gbp" : (monthField == "month23" && brands == false) ? "rowprice_gbp_23" : "rowprice_gbp";
			string view =  factory_ids != null ? "factory_sales_analysis" :  brands != false ? "brand_sales_analysis3" : "brs_sales_analysis2";
            
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
                string distributorCriteria = brands == true ? $@" AND (distributor > 0 
                                    {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR client_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})" :
                            "";

                cmd.CommandText =
						$@"SELECT  {monthField}, SUM({priceField}) as total,SUM(orderqty) AS totalqty,
                            {(brands == true ? $"SUM(IF(special_terms,{priceField},0)) AS totalSpecial," : "0 AS totalSpecial,")} 
                            COUNT(DISTINCT orderid) AS numOfOrders 
                            FROM {view} 
                            WHERE ({monthField} >=  @from OR @from IS NULL) AND ({monthField} <= @to OR @to IS NULL)
							AND customer_code NOT IN({(Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_"))}) 
                            AND custpo NOT LIKE 'IR%' 
                            AND category1 <> 13 
                            AND cprod_user > 0 
                            {GetCountryCondition(countryFilter)} 
                            {HandleClients(cmd, "cprod_user", includedClients, excludedClients)}
                            {(factory_ids != null ? "" : " AND (brand_user_id = @userid OR @userid IS NULL)")} 
                            {(brand_id != null ? " AND brand_id = @brand_id " : "")} 
                            {distributorCriteria}
							{(factory_ids != null ? $" AND factory_id IN ({Utils.CreateParametersFromIdList(cmd, factory_ids)})" : "")} 
                            {(productIds != null ? $" AND {(factoryProducts ? "cprod_mast" : "cprod_id")}  IN ({Utils.CreateParametersFromIdList(cmd, productIds)})" : "")}
                            GROUP BY {monthField} ";

				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
				cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
				if (brand_id != null)
					cmd.Parameters.AddWithValue("@brand_id", brand_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new SalesByMonth
                        {
                            Month21 = Convert.ToInt32(dr[monthField]),
                            Amount = (double) dr["total"],
                            SpecialAmount = Utilities.FromDbValue<double>(dr["totalSpecial"]),
                            numOfOrders = Convert.ToInt32(dr["numOfOrders"]),
                            Qty = Convert.ToInt32(Utilities.FromDbValue<int>(dr["totalqty"]))});
				}
				dr.Close();
			}
			return result;
		}

		//Better optimized
		public static List<SalesByMonth> GetSalesByMonth2(int from, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null,
			bool useETA = false, bool? brands = true, int[] includedClients = null, int[] excludedClients = null, List<int> factory_ids = null,
			int? brand_id = null, string excludedCustomers = "NK2", IList<int> includedNonDistributors = null, bool useOrderDate = false,
			IList<int> productIds = null, bool factoryProducts = false, bool useCompanyPriceType = false, string includedCustomers = "", bool excludeSpares = true)
		{
			var result = new List<SalesByMonth>();
			var monthCondition = useETA ? 
				(useCompanyPriceType ?
				  "IF(cust.user_price_type IN ('CIF', 'FOB') OR COALESCE(`oh`.`price_type_override`,'') = 'FOB',ph.po_req_etd, oh.req_eta + interval 1 week)"
				: "IF(COALESCE(`oh`.`price_type_override`,'') = 'FOB',ph.po_req_etd, oh.req_eta + interval 1 week)") :
					(useOrderDate ? "oh.order_date" : "ph.po_req_etd");
			var monthField = "fn_Month21(" + monthCondition + ")";
			
			var fromDate = new Month21(from).Date;
			var toDate = to != null ? (DateTime?) Utilities.GetMonthEnd(new Month21(to.Value).Date) : null;
			
			var priceField = factory_ids != null ?
				"fn_unitprice_gbp (`pl`.`unitprice`,`pl`.`unitcurrency`) * `ol`.`orderqty`" : 
				useETA && useCompanyPriceType ?
				@"`fn_unitprice_gbp_exchrate` (`ol`.`unitprice`,`ol`.`unitcurrency`,
											CASE WHEN `cust`.`user_price_type` IN ('CIF', 'FOB')	OR 
												`oh`.`price_type_override` = 'FOB'
											THEN `exch_etd`.`usd_gbp`
											ELSE `exch_eta1week`.`usd_gbp` END,
											CASE WHEN `cust`.`user_price_type` IN ('CIF', 'FOB')	OR 
												`oh`.`price_type_override` = 'FOB'
											THEN `exch_etd`.`eur_gbp`
											ELSE `exch_eta1week`.`eur_gbp` END
										) * `ol`.`orderqty`" :
				"fn_unitprice_gbp (`ol`.`unitprice`,`ol`.`unitcurrency`) * `ol`.`orderqty`";
			//string view = factory_ids != null ? "factory_sales_analysis" : brands != false ? "brand_sales_analysis3" : "brs_sales_analysis2";
			var sqlFrom = @" porder_lines pl INNER JOIN porder_header ph ON pl.porderid = ph.porderid INNER JOIN order_lines ol ON pl.soline = ol.linenum
							INNER JOIN order_header oh ON ol.orderid = oh.orderid INNER JOIN users cust ON oh.userid1 = cust.user_id
							INNER JOIN cust_products cp ON ol.cprod_id = cp.cprod_id INNER JOIN mast_products mp ON cp.cprod_mast = mp.mast_id";
			if(useETA && useCompanyPriceType)
			{
				sqlFrom += @" LEFT JOIN `exchange_rates` `exch_etd` ON `fn_Month21`(`ph`.`po_req_etd`) = `exch_etd`.`month21`
						      LEFT JOIN `exchange_rates` `exch_eta1week` ON `fn_Month21`(`oh`.`req_eta` + INTERVAL 1 WEEK) = `exch_eta1week`.`month21`";
			}

			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				string distributorCriteria = brands == true ? $@" AND (cust.distributor > 0 
                                    {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR oh.userid1 IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})" :
							"";

				cmd.CommandText =
						$@"SELECT {monthField}, SUM({priceField}) as total,SUM(ol.orderqty) AS totalqty,
                            {(brands == true ? $"SUM(IF(ol.special_terms,{priceField},0)) AS totalSpecial," : "0 AS totalSpecial,")} 
                            COUNT(DISTINCT oh.orderid) AS numOfOrders 
                            FROM {sqlFrom} 
                            WHERE ({monthCondition} >=  @fromDate OR @fromDate IS NULL) AND ({monthCondition} <= @toDate OR @toDate IS NULL)
							{(!string.IsNullOrEmpty(excludedCustomers) ? $"AND cust.customer_code NOT IN({(Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_"))}) " : "")}
							{(!string.IsNullOrEmpty(includedCustomers) ? $"AND cust.customer_code IN({(Utils.CreateParametersFromIdList(cmd, includedCustomers.Split(','), "cust_"))}) " : "")}
                            AND LEFT(oh.custpo,2) <> 'IR' 
                            {(excludeSpares ? " AND mp.category1 <> 13 " : "")}
                            AND cp.cprod_user > 0 
                            {GetCountryCondition(countryFilter,"cust.")} 
                            {HandleClients(cmd, "cp.cprod_user", includedClients, excludedClients)}
                            {(factory_ids != null ? "" : " AND (cp.brand_user_id = @userid OR @userid IS NULL)")} 
                            {(brand_id != null ? " AND cp.brand_id = @brand_id " : "")} 
                            {distributorCriteria}
							{(factory_ids != null ? $" AND mp.factory_id IN ({Utils.CreateParametersFromIdList(cmd, factory_ids)})" : "")} 
                            {(productIds != null ? $" AND {(factoryProducts ? "cp.cprod_mast" : "cp.cprod_id")}  IN ({Utils.CreateParametersFromIdList(cmd, productIds)})" : "")}
                            GROUP BY {monthField} ";

				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
				cmd.Parameters.AddWithValue("@fromDate", @fromDate);
				cmd.Parameters.AddWithValue("@toDate", Utilities.ToDBNull(toDate));
				cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object)brand_user_id : DBNull.Value);
				if (brand_id != null)
					cmd.Parameters.AddWithValue("@brand_id", brand_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new SalesByMonth
					{
						Month21 = Convert.ToInt32(dr.GetInt32(0)),
						Amount = (double)dr["total"],
						SpecialAmount = Utilities.FromDbValue<double>(dr["totalSpecial"]),
						numOfOrders = Convert.ToInt32(dr["numOfOrders"]),
						Qty = Convert.ToInt32(Utilities.FromDbValue<int>(dr["totalqty"]))
					});
				}
				dr.Close();
			}
			return result;
		}



		public static string HandleClients(MySqlCommand cmd, string field = "cprod_user", IList<int> includedClients=null, IList<int> excludedClients=null)
		{
			var result = String.Empty;
			if (includedClients != null)
				result = String.Format(" AND {0} IN ({1})",field, Utils.CreateParametersFromIdList(cmd, includedClients));
			else if (excludedClients != null)
			{
				result = String.Format(" AND {0} NOT IN ({1})", field, Utils.CreateParametersFromIdList(cmd, excludedClients));
			}
			return result;
		}

		public static string GetCountryCondition(CountryFilter countryFilter, string prefix = "", bool useAnd = true)
		{
            var countries = new List<string>() { "GB", "UK", "IE" };
            if(countryFilter == CountryFilter.NonUKExcludingAsia)
            {
                var asianCountries = new List<string>();
                if (string.IsNullOrEmpty(AsiaCountries))
                {
                    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                    {
                        conn.Open();
                        asianCountries = conn.Query<string>("SELECT ISO2 FROM countries WHERE continent_code = 'AS'").ToList();
                        AsiaCountries = string.Join(",", asianCountries);
                    }
                }
                else
                    asianCountries = AsiaCountries.Split(',').ToList();
                countries.AddRange(asianCountries);
            }
			return countryFilter != CountryFilter.All
					   ? $" {(useAnd ? " AND " : "")} {prefix}user_country {(countryFilter == CountryFilter.UKOnly ? "" : "NOT")} IN ({string.Join(",",countries.Select(c=>$"'{c}'"))})"
									   
					   : "";
		}

		public static List<SalesByMonth> GetNumOfOrdersByMonthForFactories(int from, int to, IList<int> factoryIds=null)
		{
			var result = new List<SalesByMonth>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = 
					Utils.GetCommand("", conn);
				cmd.CommandText = String.Format(@"SELECT count(DISTINCT orderid) as numOfOrders, month21 FROM factory_sales_analysis WHERE cprod_user > 0 {0} and category1 <> 13 AND month21 BETWEEN @from AND @to
													group by month21 order by month21 ASC",factoryIds != null ? String.Format(" AND factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factoryIds)) : "");
				cmd.Parameters.AddWithValue("@from", @from);
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand("", conn);
				cmd.CommandText = String.Format(@"SELECT count(DISTINCT orderid) as numOfOrders,sum(PO_rowprice_gbp) as salesAmount , brand_sales_analysis2.month21,brand_sales_analysis2.factory_code,users.combined_factory,users.user_id
													FROM brand_sales_analysis2 INNER JOIN users ON brand_sales_analysis2.factory_id = users.user_id 
													WHERE cprod_user > 0 and category1 <> 13 AND month21 BETWEEN @from AND @to
													group by factory_code order by salesAmount DESC");
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new FactorySalesByMonth
					{
						Month21 = Convert.ToInt32(dr["month21"]),
						Amount = Convert.ToInt32(dr["salesAmount"]),
						factoryCode = String.Empty + dr["factory_code"],
						combined_factory = Convert.ToInt32(dr["combined_factory"]),
						user_id=Convert.ToInt32(dr["user_id"])
					});
				}
				dr.Close();
			}

			
			return result;
		}
		/***/


		public static List<SalesByMonth> GetNumOfOrdersByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool? brands = true, 
            int[] includedClients = null, int[] excludedClients = null,int[] excludedContainerTypes = null,int[] includedContainerTypes = null,
            IList<int> factoryIds = null, int? brand_id = null, bool useOrderDate = false, string excludedCustomersString = "")
		{
			var result = new List<SalesByMonth>();
			if(excludedContainerTypes == null && includedContainerTypes == null)
				excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var brandsClause = brands != null ? brands.Value ? "AND users.distributor <> 0" : "AND users.distributor = 0" : "";
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand("", conn);
				cmd.CommandText = String.Format(@"SELECT month21, COUNT(orderid) AS numOfOrders FROM 
										(
										SELECT
										order_header.orderid, {7} AS month21
										FROM
										order_header
										INNER JOIN porder_header ON porder_header.soorderid = order_header.orderid INNER JOIN users ON order_header.userid1 = users.user_id
										WHERE order_header.`status` NOT IN ('X','Y') AND custpo NOT LIKE 'IR%' {8}
										AND COALESCE(order_header.container_type,0) {3} IN ({4}) AND COALESCE(order_header.combined_order,0) <= 0 
										{0} {1} 
										AND EXISTS (SELECT order_lines.linenum 
													FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
													INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id AND mast_products.category1 <> 13 {5}
													WHERE order_lines.orderid = order_header.orderid {2} {6})
										GROUP BY
										order_header.orderid ORDER BY month21, orderid) AS orders
										WHERE month21 BETWEEN @from AND @to 
										GROUP BY month21", GetCountryCondition(countryFilter),brandsClause,
										HandleClients(cmd,"cprod_user",includedClients,excludedClients),
										excludedContainerTypes != null ? "NOT" : "",
										Utils.CreateParametersFromIdList(cmd,excludedContainerTypes ?? includedContainerTypes,"containerType"),
										factoryIds != null ? String.Format(" AND mast_products.factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factoryIds,"factid")) : "",
										brand_id != null ? " AND cust_products.brand_id = @brand_id" : "",
                                        useOrderDate ? "fn_Month21(order_header.orderdate)" : "fn_Month21(MAX(`porder_header`.`po_req_etd`))",
                                        !String.IsNullOrEmpty(excludedCustomersString) ? String.Format(" AND users.customer_code NOT IN ({0})",Utils.CreateParametersFromIdList<string>(cmd,excludedCustomersString.Split(','),"custcode_")) : "");
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				if (brand_id != null)
					cmd.Parameters.AddWithValue("@brand_id", brand_id);
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						@"SELECT  month21, SUM(totalgp) as total FROM brand_sales_analysis_gp WHERE month21 BETWEEN @from AND @to 
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 GROUP BY month21 ",
						conn);
				cmd.Parameters.AddWithValue("@from", @from);
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

        /// <summary>
        /// Returns sales grouped by brand and month. Can filter by month21 or linedate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="countryFilter"></param>
        /// <param name="brands"></param>
        /// <param name="includedClients"></param>
        /// <param name="excludedClients"></param>
        /// <param name="groupOption"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
		public static List<BrandSalesByMonthEx> GetBrandSalesByMonth(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, 
            bool brands = true, int[] includedClients = null, int[] excludedClients = null,BrandSalesGroupOption groupOption = BrandSalesGroupOption.Default, 
            DateTime? fromDate = null, DateTime? toDate = null, bool productLevel = false, bool customerLevel = false,string customerCode = "", int? brand_id = null, 
            bool useETA = false, bool includePendingForDiscontinuation = true, bool usePeriod = false,string excludedCustomers = "NK2", bool groupByPeriod = false, IList<int> includedNonDistributors = null, bool useCompanyPriceType = false)
		{
			var result = new List<BrandSalesByMonthEx>();
            var monthField = useETA ? useCompanyPriceType ? "month23" : "month22" : "month21";
			string view = brands ? "brand_sales_analysis3" : "brs_sales_analysis2";
            var groupField = groupOption == BrandSalesGroupOption.ByBrand || brands ? "brand_id,brandname" : "cat1_name";
            var selectField = groupField;
            const string periodField = ",PERIOD_DIFF(@monthyear,EXTRACT(YEAR_MONTH FROM linedate) + (CASE WHEN DAY(linedate) < @day THEN -1 ELSE 0 END))";
            groupField += @from != null || groupByPeriod ? "," + monthField : usePeriod ?  periodField : "";
            if (productLevel)
                groupField += ",cprod_code1,cprod_name,analytics_category";
            if (customerLevel)
                groupField += ",userid1,client_name,customer_code";
            selectField += !usePeriod ? "," + monthField : periodField + " AS month21";
            if (productLevel)
                selectField += ",cprod_code1,cprod_name,analytics_category,cprod_stock,cprod_stock_date, product_group";
            if (customerLevel)
                selectField += ",userid1,client_name,customer_code, dist_stock, dist_stock_date";
		    var wherePeriod = @from != null ? String.Format("({0} >= @from OR @from IS NULL) AND ({0} <= @to OR @to IS NULL)",monthField) : String.Format(" {0} BETWEEN @from AND @to",usePeriod ? "linedate" : useETA ? "req_eta" : "po_req_etd");

            string nameField = groupOption == BrandSalesGroupOption.ByBrand || brands ? "brandname" : "cat1_name";
            var wherePendingDiscontinuation = includePendingForDiscontinuation
                ? ""
                : " AND COALESCE(pending_discontinuation,0) = 0";
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
			    var cmd = Utils.GetCommand("", conn);
                string distributorCriteria = brands ? $@" AND (distributor > 0 
                                    {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR client_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})" :
                            String.Empty;
                cmd.CommandText =
					
						$@"SELECT {selectField}, SUM(rowprice_gbp) as total,SUM(orderqty) AS qty, COUNT(DISTINCT orderid) AS numOfOrders 
                        FROM {view} 
                        WHERE {wherePeriod} 
						AND customer_code NOT IN  ({(Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_"))}) 
                        AND category1 <> 13 
                        AND cprod_user > 0
                        {GetCountryCondition(countryFilter)}
                        {HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients)}
                        {(!String.IsNullOrEmpty(customerCode) ? " AND customer_code = @customer_code" : "")}
                        {(brand_id != null ? " AND brand_id = @brand_id" : "")}
                        {wherePendingDiscontinuation} 
                        {distributorCriteria}
                        GROUP BY {groupField} ";
			    if (@from != null)
			    {
			        cmd.Parameters.AddWithValue("@from", @from);
			        cmd.Parameters.AddWithValue("@to", to);
			    }
			    else
			    {
			        cmd.Parameters.AddWithValue("@monthyear", "20" + (company.Common.Utilities.GetMonth21FromDate(toDate.Value)));
			        cmd.Parameters.AddWithValue("@day", toDate.Value.Day);
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    cmd.Parameters.AddWithValue("@to", toDate);
			    }
			    if (!String.IsNullOrEmpty(customerCode))
			        cmd.Parameters.AddWithValue("@customer_code", customerCode);
                if (brand_id != null)
                    cmd.Parameters.AddWithValue("@brand_id", brand_id);

			    var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var record = new BrandSalesByMonthEx 
                    {
                        brand_id = (groupOption == BrandSalesGroupOption.ByBrand || brands) ? Utilities.FromDbValue<int>(dr["brand_id"]) : null,
                        brandname = String.Empty+dr[nameField], 
                        Month21 = Convert.ToInt32(dr[monthField]), 
                        Amount = (double)dr["total"], 
                        numOfOrders = Convert.ToInt32(dr["numOfOrders"]),
                        Qty = Convert.ToInt32(dr["qty"])
                    };
				    if (productLevel)
				    {
				        record.cprod_code = String.Empty + dr["cprod_code1"];
				        record.cprod_name = String.Empty + dr["cprod_name"];
                        record.AnalyticsSubCategoryId = Utilities.FromDbValue<int>(dr["analytics_category"]);
				        record.cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]);
				        record.stock_date = Utilities.FromDbValue<DateTime>(dr["cprod_stock_date"]);
                        
				        record.product_group = String.Empty + dr["product_group"];
				    }
				    if (customerLevel)
				    {
				        record.customer_id = Convert.ToInt32(dr["userid1"]);
				        record.customer_code = String.Empty + dr["customer_code"];
                        record.dist_stock = Utilities.FromDbValue<int>(dr["dist_stock"]);
                        record.dist_stock_date = Utilities.FromDbValue<DateTime>(dr["dist_stock_date"]);
				    }
                    result.Add(record);
				}
				dr.Close();
			}
			return result;
		}

		public static List<AnalyticsCategorySummaryRow> GetAnalyticsCategorySummary(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, string excludedCustomers = "NK2")
		{
			var result = new List<AnalyticsCategorySummaryRow>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText =
					String.Format(
                        @"SELECT analytics_category,analytics_option, SUM(orderqty) as total FROM {1}  WHERE month21 BETWEEN @from AND @to AND analytics_category IS NOT NULL
							 AND category1 <> 13 AND cprod_user > 0 {0} {2} AND customer_code NOT IN({3}) GROUP BY analytics_category,analytics_option ",
						GetCountryCondition(countryFilter), "brand_sales_analysis2",
						HandleClients(cmd, includedClients: includedClients, excludedClients: excludedClients),
                        Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_") );
				cmd.Parameters.AddWithValue("@from", @from);
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						String.Format(@"SELECT category1,cat1_name, SUM(rowprice_gbp) as total
							FROM brand_sales_analysis2 WHERE month21 BETWEEN @from AND @to 
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} AND (brand_user_id = @userid OR @userid IS NULL) GROUP BY category1,cat1_name ", GetCountryCondition(countryFilter)),
						conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@userid", userid != null ? (object) userid : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Category1SalesByMonth { category_id = (int)dr["category1"], catname = String.Empty + dr["cat1_name"], Amount = (double)dr["total"] });
				}
				dr.Close();
			}
			return result;
		}

		public static List<Category1> GetCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var result = new List<Category1>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						String.Format(@"SELECT DISTINCT category1,cat1_name FROM brand_sales_analysis2 WHERE month21 BETWEEN @from AND @to 
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL)  {0} GROUP BY brandname,month21 ", GetCountryCondition(countryFilter)),
						conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@userid", userid != null ? (object)userid : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Category1{ cat1_name = String.Empty + dr["cat1_name"], category1_id = (int) dr["category1"]});
				}
				dr.Close();
			}
			return result;

		}

		public static List<Category1SalesByMonth> GetSubCategorySalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var result = new List<Category1SalesByMonth>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						String.Format(@"SELECT COALESCE(brand_sub_id,0) AS brand_sub_id,COALESCE(brand_sub_desc,'') AS brand_sub_desc, SUM(rowprice_gbp) as total
							FROM brand_sales_analysis2 LEFT OUTER JOIN brand_categories_sub ON brand_sales_analysis2.cprod_brand_subcat = brand_categories_sub.brand_sub_id  WHERE month21 BETWEEN @from AND @to 
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} AND (brand_user_id = @userid OR @userid IS NULL) GROUP BY COALESCE(brand_sub_id,0),COALESCE(brand_sub_desc,'') ", GetCountryCondition(countryFilter)),
						conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@userid", userid != null ? (object)userid : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Category1SalesByMonth { category_id = Convert.ToInt32(dr["brand_sub_id"]), catname = String.Empty + dr["brand_sub_desc"], Amount = (double)dr["total"] });
				}
				dr.Close();
			}
			return result;
		}

		public static List<Category1> GetSubCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var result = new List<Category1>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						String.Format(@"SELECT DISTINCT COALESCE(brand_sub_id,0) AS brand_sub_id,COALESCE(brand_sub_desc,'') AS brand_sub_desc FROM brand_sales_analysis2 LEFT OUTER JOIN brand_categories_sub ON brand_sales_analysis2.cprod_brand_subcat = brand_categories_sub.brand_sub_id
							WHERE month21 BETWEEN @from AND @to 
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL)  {0} ", GetCountryCondition(countryFilter)),
						conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@userid", userid != null ? (object)userid : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Category1 { cat1_name = String.Empty + dr["brand_sub_desc"], category1_id = Convert.ToInt32(dr["brand_sub_id"]) });
				}
				dr.Close();
			}
			return result;

		}

		public static List<CustomerSalesByMonth> GetCustomerSalesByMonth(int? @from=null, int? to=null, bool UK = true, CountryFilter countryFilter = CountryFilter.UKOnly, int? userId = null,
            bool brands = true, int[] includedClients = null, int[] excludedClients = null,int? brand_id = null,DateTime? fromDate = null, DateTime? toDate = null, bool periodLevel = false, 
            bool useETA = false, string excludedCustomers = "NK2", bool groupByMonth = false, IList<int> includedNonDistributors = null, bool useCompanyPriceType = false)
        {
			var result = new List<CustomerSalesByMonth>();
			UK = countryFilter != CountryFilter.NonUK && countryFilter != CountryFilter.NonUKExcludingAsia && UK;
			string view = brands ? "brand_sales_analysis3" : "brs_sales_analysis2";
		    var monthField = useETA ? useCompanyPriceType ? "month23" : "month22" : "month21";
			var priceField = monthField == "month23" && !brands ? "rowprice_gbp_23" : "rowprice_gbp";	//rowprice_gbp_23 uses exchange rate table for gbp->usd, use only for non-brands
            var wherePeriod = @from != null ? String.Format(" {0} BETWEEN @from AND @to ",monthField) : periodLevel ?  " linedate BETWEEN @from AND @to" : String.Format("{0} BETWEEN @from AND @to",useETA ? "req_eta" : "po_req_etd");
		    var groupFields = "userid1,client_name,oem_flag";
            const string periodField = ",PERIOD_DIFF(@monthyear,EXTRACT(YEAR_MONTH FROM linedate) + (CASE WHEN DAY(linedate) < @day THEN -1 ELSE 0 END))";
            var selectFields = "userid1,client_name,oem_flag";
		    if (periodLevel)
		    {
		        groupFields += periodField;
		        selectFields += periodField + " AS period";
		    }
            if(groupByMonth) {
                groupFields += "," + monthField;
                selectFields += "," + monthField + " AS period";
            }
		    using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand("", conn);

                string distributorCriteria = brands ? $@" AND (distributor > 0 
                                    {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR client_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})" :
                           String.Empty;

                cmd.CommandText = 
					     $@"SELECT {selectFields}, SUM({priceField}) as total 
                            FROM {view} 
                            WHERE {wherePeriod} 
							AND customer_code NOT IN ({Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_")}) 
                            AND category1 <> 13 
                            AND cprod_user > 0 
                            AND (brand_user_id = @userid OR @userid IS NULL) 
                            {GetCountryCondition(countryFilter)} 
						    {HandleClients(cmd, "userid1", includedClients, excludedClients)}
                            {(brand_id != null ? " AND brand_id = @brand_id" : "")}
                            {distributorCriteria}  GROUP BY {groupFields}";

			    if (@from != null)
			    {
			        cmd.Parameters.AddWithValue("@from", @from);
			        cmd.Parameters.AddWithValue("@to", to);
			    }
			    else
			    {
                    cmd.Parameters.AddWithValue("@monthyear", "20" + (company.Common.Utilities.GetMonth21FromDate(toDate.Value)));
                    cmd.Parameters.AddWithValue("@day", toDate.Value.Day);
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    cmd.Parameters.AddWithValue("@to", toDate);
			    }
			    cmd.Parameters.AddWithValue("@userid", userId != null ? (object)userId : DBNull.Value);
				if (brand_id != null)
					cmd.Parameters.AddWithValue("@brand_id", brand_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var r = new CustomerSalesByMonth {isUK  = UK,isOEM = Utilities.BoolFromLong(dr["oem_flag"]) ?? false ,Amount = (double)dr["total"],client_id = (int) dr["userid1"], customer_name = String.Empty + dr["client_name"]};
				    if (periodLevel || groupByMonth)
				        r.Month21 = Convert.ToInt32(dr["period"]);
                    result.Add(r);
				}
				dr.Close();
			}
			return result;
		}

		public static List<ProductSales> GetProductSales(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, 
            int? brand_user_id = null, IList<int> factories = null, DateTime? fromDate = null, DateTime? toDate = null, bool brands = true, bool monthBreakDown = false, 
            IList<int> incClients = null, IList<int> exClients = null,int? brand_id = null,bool ignoreFactoryCode = false, bool useETA = false, 
            IList<int> prodIds = null , bool useOrderDate = false, bool clientBreakDown = false, bool useLineDate = false, bool shippedOrdersOnly = true , bool periodBreakDown = false, IList<string> excludedCustomers = null)
		{
			var result = new List<ProductSales>();
			var groupfields = factories != null ? "factory_ref,asaq_name" : brands ? String.Format("cprod_code1,cprod_name,brand_code{0}", ignoreFactoryCode ? "" : ",factory_code") : String.Format("cprod_code1,cprod_name{0}", ignoreFactoryCode ? "" : ",factory_code");

			var selectFields = factories != null ? "0 as cprod_id,factory_ref AS cprod_code1,asaq_name AS cprod_name, '' AS brand_code,factory_code" : brands ? "cprod_id,cprod_code1,cprod_name,brand_code,factory_code,factory_stock" : "cprod_id,cprod_code1,cprod_name,factory_code";
            const string periodField = ",PERIOD_DIFF(@monthyear,EXTRACT(YEAR_MONTH FROM linedate) + (CASE WHEN DAY(linedate) < @day THEN -1 ELSE 0 END))";
			if (monthBreakDown)
			{
				if(useETA)
				{
					groupfields += ",month22";
				}
				else
				{
					groupfields += ",month21";
					
				}
				selectFields += ",month21,month22,req_eta";
			}
		    if (periodBreakDown)
		    {
		        groupfields += "," + periodField;
		        selectFields += "," + periodField + " AS month21";
		    }
		    if (clientBreakDown)
		    {
		        groupfields += ",customer_code";
		        selectFields += ",customer_code";
		    }
			var rangeCriteria = @from != null ? "month21 BETWEEN @from AND @to" : String.Format("({0} >=  @from OR @from IS NULL) AND ({0} <= @to OR @to IS NULL)",useETA ? "req_eta" : useOrderDate ? "orderdate" : useLineDate ? "linedate" :  "po_req_etd");
            var view = brands ? shippedOrdersOnly ? "brand_sales_analysis_product2" : "brand_sales_analysis_product3" : "brs_sales_analysis_product2";
            

			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText =
						String.Format(@"SELECT {2}, sum(sogbp*orderqty) as total,SUM(poprice*orderqty) AS po_total,SUM(fn_unitprice_gbp(unitprice,unitcurrency)*orderqty) AS po_total_gbp, sum(orderqty) as numOfProducts, COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM {5} WHERE {4} 
						   AND cprod_code1 not like 'SP%' {6} AND rowprice > 0 {8} {9}
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @userid OR @userid IS NULL) {0} {1} {7} {10} GROUP BY {3}", 
							GetCountryCondition(countryFilter),
							factories != null ? String.Format(" AND factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factories)) : "",
							selectFields,groupfields,rangeCriteria,view,
							brands ? " and distributor > 0 " : " AND COALESCE(distributor,0) = 0",
							incClients != null || exClients != null ? String.Format(" AND cprod_user {1} IN ({0})",Utils.CreateParametersFromIdList(cmd,incClients ?? exClients,"custid"),exClients != null ? " NOT " : "") : "",
							brand_id != null ? " AND brand_id = @brand_id " : "",
                            prodIds != null ? 
                                String.Format(" AND cprod_id IN ({0})",Utils.CreateParametersFromIdList(cmd,prodIds,"cprodid")) :  "",
                            excludedCustomers != null ? String.Format(" AND customer_code NOT IN ({0})",Utils.CreateParametersFromIdList(cmd,excludedCustomers,"custCode")) : ""
                            );

				cmd.Parameters.AddWithValue("@from", @from != null ? (object) @from : fromDate);
				cmd.Parameters.AddWithValue("@to", to != null ? (object) to : toDate);
				cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
				if (brand_id != null)
					cmd.Parameters.AddWithValue("@brand_id", brand_id);
			    if (periodBreakDown)
			    {
			        cmd.Parameters.AddWithValue("@monthyear", "20" + (company.Common.Utilities.GetMonth21FromDate(toDate.Value)));
			        cmd.Parameters.AddWithValue("@day", toDate.Value.Day);
			    }
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var item = new ProductSales
						{
							Amount = (double) dr["total"],
							POAmount = (double) dr["po_total"],
							POAmountGBP = (double) dr["po_total_gbp"],
                            cprod_id = Convert.ToInt32(dr["cprod_id"]),
							cprod_name = String.Empty + dr["cprod_name"],
							Margin = (double) dr["margin"],
							numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
							cprod_code = String.Empty + dr["cprod_code1"],
							factory_code = String.Empty + dr["factory_code"]
						};

					if (monthBreakDown)
					{
						item.Month21 = Convert.ToInt32(dr["month21"]);
						item.Req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]);
						//item.Eta=
						if (useETA)
						{
							 item.Month22 = Convert.ToInt32(dr["month22"]);
						}
					   
					}
				    if (periodBreakDown)
				    {
                        item.Month21 = Convert.ToInt32(dr["month21"]);
				    }
				    if (clientBreakDown)
				    {
				        item.customer_code = String.Empty + dr["customer_code"];
				    }
					if (brands)
						item.brand_code = String.Empty + dr["brand_code"];
					
					result.Add(item);
				}
				dr.Close();
				
			}
			return result;
		}

		public static List<CountrySales> GetCountrySales(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, IList<int> factoryIds = null )
		{
			var result = new List<CountrySales>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand("", conn);
				cmd.CommandText = 
						String.Format(@"SELECT user_country,countries.CountryName, sum(po_rowprice_gbp) as total
						FROM factory_sales_analysis INNER JOIN countries ON factory_sales_analysis.user_country = countries.ISO2 WHERE month21 BETWEEN @from AND @to AND customer_code <> 'te' AND customer_code <> ''
						   AND rowprice > 0 AND category1 <> 13 {0} {1} GROUP BY user_country", 
						  GetCountryCondition(countryFilter),factoryIds != null ? String.Format(" AND factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factoryIds)) : "");

				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new CountrySales
					{
						Amount = (double)dr["total"],
						user_country = String.Empty + dr["user_country"],
						country_name = String.Empty + dr["countryName"]
					});
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetNonSelling(int from, int to, int? brand_user_id = null)
		{
			var result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(String.Format(
						@"SELECT * FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE category1 <> 13 AND (brand_user_id = @userid OR @userid IS NULL) 
						  AND cprod_status <> 'D' {0}
						  AND cprod_id NOT IN (SELECT DISTINCT cprod_id FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to ) ",ExcludedCprodBrandCatsCondition), conn);

				cmd.Parameters.AddWithValue("@from", @from);
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = String.Format(@"SELECT SUM(credit_value*return_qty) FROM returns INNER JOIN users ON returns.client_id = users.user_id
						  WHERE status1 = 1 and claim_type <> 5 and decision_final = 1 {0} {1} {2}
						  AND (((year(`returns`.`request_date`) - 2000) * 100) + month(`returns`.`request_date`)) BETWEEN @from AND @to", GetCountryCondition(countryFilter),brandsClause,HandleClients(cmd,"client_id",includedClients,excludedClients));

				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				result = Utilities.FromDbValue<double>(cmd.ExecuteScalar()) ?? 0;
			}

			return result;
		}

		public static List<Returns> GetRespondedClaims(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
		{
			var result = new List<Returns>();
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = 
					Utils.GetCommand("",conn);
				cmd.CommandText =
					String.Format(@"SELECT returns.* FROM returns INNER JOIN users ON returns.client_id = users.user_id 
						WHERE status1 = 1 and client_id <> 184 AND cc_response_date IS NOT NULL {1}  {0} {2}
						  AND (((year(`returns`.`cc_response_date`) - 2000) * 100) + month(`returns`.`cc_response_date`)) BETWEEN @from AND @to",
								  GetCountryCondition(countryFilter),brandsClause,HandleClients(cmd,"client_id",includedClients,excludedClients));

				cmd.Parameters.AddWithValue("@from", @from);
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//First, new orders
				var cmd =
					  Utils.GetCommand(String.Format(@"SELECT orders.customer_code, COUNT(*) AS numOfOrders, COALESCE(SUM(orders.order_amount),0) AS Amount FROM 
										(SELECT order_header.*,SUM(order_lines.unitprice*(CASE unitcurrency WHEN 0 THEN 1/1.6 ELSE 1 END)*order_lines.orderqty) AS order_amount, users.customer_code
										FROM
										order_lines
										INNER JOIN order_header ON order_header.orderid = order_lines.orderid
										INNER JOIN users ON users.user_id = order_header.userid1
										WHERE users.distributor > 0 AND  order_header.`status` NOT IN ('X','Y') AND order_header.orderdate BETWEEN @from AND @to  {0}
										GROUP BY order_header.orderid) AS orders
										GROUP BY orders.customer_code", GetCountryCondition(countryFilter)), conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderStats{client_code = String.Empty + dr["customer_code"],newOrdersCount = Convert.ToInt32(dr["numOfOrders"]),
												newOrdersAmount = (double) dr["Amount"]});
				}
				dr.Close();

				//orders with etd in this period and beyond
				cmd.CommandText = String.Format(@"SELECT orders.customer_code, COUNT(CASE WHEN orders.po_req_etd <= @today THEN 1 ELSE NULL END ) AS numOfOrdersShipped, COALESCE(SUM(CASE WHEN orders.po_req_etd <= @today THEN orders.order_amount ELSE NULL END ),0) AS AmountShipped,
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
					var customer_code = String.Empty + dr["customer_code"];
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
							CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null,
                            IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderProductGroupStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				//new orders
				var cmd =
					Utils.GetCommand("",conn);
                var brandsClause  =
			        $@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                    {(includedNonDistributors != null &&
			                                                                            includedNonDistributors.Count > 0
			            ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})"
			            : "")})";
                            
			    
                cmd.CommandText =
					String.Format(
						@"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, COUNT(*) AS numOfLines
						FROM order_lines
						INNER JOIN order_header ON order_header.orderid = order_lines.orderid
						LEFT OUTER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
						LEFT OUTER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
						INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
						 INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
						INNER JOIN users ON users.user_id = order_header.userid1
						WHERE order_header.`status` NOT IN ('X','Y') {0} {2} {1}
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN ({3}) and mast_products.category1 <> 13
						AND order_header.orderdate BETWEEN @from AND @to 
						GROUP BY order_header.orderid,users.customer_code, fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) ORDER BY order_header.orderid, mast_products.product_group",
                        GetCountryCondition(countryFilter), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes , "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
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
								customer_code = String.Empty + dr["customer_code"],
								productgroup_lines = new Dictionary<string, int>()
							};
						orderList.Add(orderRecord);
					}
					orderRecord.productgroup_lines[String.Empty + dr["product_group"]] =
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
																				 CountryFilter countryFilter = CountryFilter.UKOnly, 
                                                                                 bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderProductGroupStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders

				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                    {(includedNonDistributors != null &&
                                                                                        includedNonDistributors.Count > 0
                        ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})"
                        : "")})";

                cmd.CommandText =
					String.Format(
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN ({4}) and mast_products.category1 <> 13
						{3}
						GROUP BY order_header.orderid,users.customer_code, fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) ORDER BY order_header.orderid, mast_products.product_group",
						GetCountryCondition(countryFilter), brandsClause,
                        HandleClients(cmd, "userid1", includedClients, excludedClients), GetOrdersDateCriteria(OutstandingOrdersMode.TransitInPeriod), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
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
							customer_code = String.Empty + dr["customer_code"],
							productgroup_lines = new Dictionary<string, int>()
						};
						orderList.Add(orderRecord);
					}
					orderRecord.productgroup_lines[String.Empty + dr["product_group"]] =
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
																				 CountryFilter countryFilter = CountryFilter.UKOnly,bool brands = true,int[] includedClients = null,int[] excludedClients=null,int? daysToShipping= null,
                                                                                 DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			var result = new List<OrderProductGroupStats>();
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
		    if (daysToShipping == null)
		        daysToShipping = Settings.Default.Analytics_Default_DaysToShipping;
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders

				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText =
					String.Format(
                        @"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, 
						order_header.req_eta, COUNT(*) AS numOfLines,sum(order_lines.orderqty * order_lines.unitprice) AS totalGPB
						FROM order_lines
						INNER JOIN order_header ON order_header.orderid = order_lines.orderid
						INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
						INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
						INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
						 INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
						INNER JOIN users ON users.user_id = order_header.userid1
						WHERE order_header.`status` NOT IN ('X','Y') {0} {3} {2}
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({4}) and mast_products.category1 <> 13
						AND order_header.req_eta > @from {1}
						GROUP BY order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) ORDER BY order_header.orderid, mast_products.product_group",
                        GetCountryCondition(countryFilter), GetOrdersDateCriteria(mode), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				var orderList = new List<OrderStatTempRecord>();
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@new_from", new_from);
				cmd.Parameters.AddWithValue("@new_to", new_to);
			    cmd.Parameters.AddWithValue("@daysToShipping", daysToShipping);
			    cmd.Parameters.AddWithValue("@etafrom", etaCriteriaFrom);
			    cmd.Parameters.AddWithValue("@etato", etaCriteriaTo);
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
							customer_code = String.Empty + dr["customer_code"],
							productgroup_lines = new Dictionary<string, int>(),
                            totalGPB = (double)dr["totalGPB"]
                        };
						orderList.Add(orderRecord);
					}
					orderRecord.productgroup_lines[String.Empty + dr["product_group"]] =
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
										   orders_count = g.Count(),
                                           totalGPB = g.Sum( t => t.totalGPB)
									   }).ToList();

			}
			return result;
		}

        private static string GetOrdersDateCriteria(OutstandingOrdersMode mode)
        {
            var dateCriteria = String.Empty;

            switch (mode)
            {
                case OutstandingOrdersMode.Production:
                    //Exclude shipping in next n days
                    dateCriteria =
                        " AND (porder_header.po_req_etd > @from AND order_header.orderdate NOT BETWEEN @new_from AND @new_to AND DATEDIFF(porder_header.po_req_etd,CURDATE())>@daysToShipping)";
                    break;
                case OutstandingOrdersMode.Transit:
                    //Exclude transit in period
                    dateCriteria = "AND porder_header.po_req_etd <= @from AND order_header.req_eta NOT BETWEEN @etafrom AND @etato";
                    break;
                case OutstandingOrdersMode.TransitInPeriod:
                    dateCriteria = "AND order_header.req_eta BETWEEN @from AND @to";
                    break;
                case OutstandingOrdersMode.ShippingInNextNDays:
                    dateCriteria = "AND (porder_header.po_req_etd > @from AND order_header.orderdate NOT BETWEEN @new_from AND @new_to AND DATEDIFF(porder_header.po_req_etd,CURDATE())<=@daysToShipping)";
                    break;
            }

            return dateCriteria;
        }

		public static List<OrderBrandsStats> GetOrderBrandStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, 
            bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderBrandsStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                    {(includedNonDistributors != null &&
                                                                                        includedNonDistributors.Count > 0
                        ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})"
                        : "")})";
                cmd.CommandText =
					String.Format(@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
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
                            AND users.test_account IS NULL
							AND COALESCE(order_header.container_type,0) NOT IN({3}) and mast_products.category1 <> 13
							 {1} AND  order_header.`status` NOT IN ('X','Y') {0} {2}
							GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
					GROUP BY obsummary.customer_code, obsummary.numOfBrands
					ORDER BY obsummary.customer_code, obsummary.numOfBrands", GetCountryCondition(countryFilter), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderBrandsStats{client_code = String.Empty + dr["customer_code"],brandCount = Convert.ToInt32(dr["numOfBrands"]),orderCount = Convert.ToInt32(dr["numOfOrders"])});
				}

			}
			return result;
		}

		public static List<OrderBrandsStats> GetOrderBrandStats_Out(DateTime from,DateTime new_from, DateTime new_to,OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
            CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom=null, DateTime? etaCriteriaTo = null)
		{
			var result = new List<OrderBrandsStats>();
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
		    if (daysToShipping == null)
		        daysToShipping = Settings.Default.Analytics_Default_DaysToShipping;
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText =
					String.Format(@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
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
                            AND users.test_account IS NULL
							AND COALESCE(order_header.container_type,0) NOT IN({4}) and mast_products.category1 <> 13
							AND req_eta >= @from {1}
							GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
					GROUP BY obsummary.customer_code, obsummary.numOfBrands
					ORDER BY obsummary.customer_code, obsummary.numOfBrands", GetCountryCondition(countryFilter),
								  GetOrdersDateCriteria(mode), brandsClause,
                                  HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@new_from", new_from);
				cmd.Parameters.AddWithValue("@new_to", new_to);
			    cmd.Parameters.AddWithValue("@daysToShipping", daysToShipping);
			    cmd.Parameters.AddWithValue("@etafrom", etaCriteriaFrom);
			    cmd.Parameters.AddWithValue("@etato", etaCriteriaTo);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderBrandsStats { client_code = String.Empty + dr["customer_code"], brandCount = Convert.ToInt32(dr["numOfBrands"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderBrandsStats> GetOrderBrandStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, 
            bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderBrandsStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} 
                    (users.distributor > 0 {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})";

                cmd.CommandText =
					String.Format(@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
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
                            AND users.test_account IS NULL
							AND req_eta BETWEEN @from AND @to
							AND COALESCE(order_header.container_type,0) NOT IN({3}) and mast_products.category1 <> 13
							GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
					GROUP BY obsummary.customer_code, obsummary.numOfBrands
					ORDER BY obsummary.customer_code, obsummary.numOfBrands", GetCountryCondition(countryFilter),
								  brandsClause,
                                 HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderBrandsStats { client_code = String.Empty + dr["customer_code"], brandCount = Convert.ToInt32(dr["numOfBrands"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderFactoriesStats> GetOrderFactoryStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
            bool brands = true,int[] includedClients = null,int[] excludedClients=null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderFactoriesStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                    {(includedNonDistributors != null &&
                                                                                        includedNonDistributors.Count > 0
                        ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})"
                        : "")})";
                cmd.CommandText =
					String.Format(@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({3}) and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.user_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
						GROUP BY obsummary.customer_code, obsummary.numOfFactories
						ORDER BY obsummary.customer_code, obsummary.numOfFactories",
								  GetCountryCondition(countryFilter, "users."), brandsClause,
                                  HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderFactoriesStats { client_code = String.Empty + dr["customer_code"], factoryCount = Convert.ToInt32(dr["numOfFactories"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderFactoriesStats> GetOrderFactoryStats_Out(DateTime from,DateTime new_from, DateTime new_to,OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
            CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			var result = new List<OrderFactoriesStats>();
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
		    if (daysToShipping == null)
		        daysToShipping = Settings.Default.Analytics_Default_DaysToShipping;
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText =
					String.Format(@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({4}) and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.user_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
						GROUP BY obsummary.customer_code, obsummary.numOfFactories
						ORDER BY obsummary.customer_code, obsummary.numOfFactories",
								  GetCountryCondition(countryFilter, "users."),
								  GetOrdersDateCriteria(mode), brandsClause,
                                  HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@new_from", new_from);
				cmd.Parameters.AddWithValue("@new_to", new_to);
                cmd.Parameters.AddWithValue("@daysToShipping", daysToShipping);
                cmd.Parameters.AddWithValue("@etafrom", etaCriteriaFrom);
                cmd.Parameters.AddWithValue("@etato", etaCriteriaTo);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderFactoriesStats { client_code = String.Empty + dr["customer_code"], factoryCount = Convert.ToInt32(dr["numOfFactories"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderFactoriesStats> GetOrderFactoryStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, 
            bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderFactoriesStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} 
                    (users.distributor > 0 {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})";

                cmd.CommandText =
					String.Format(@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({3}) and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.user_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
						GROUP BY obsummary.customer_code, obsummary.numOfFactories
						ORDER BY obsummary.customer_code, obsummary.numOfFactories",
                                  GetCountryCondition(countryFilter, "users."), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderFactoriesStats { client_code = String.Empty + dr["customer_code"], factoryCount = Convert.ToInt32(dr["numOfFactories"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderLocationStats> GetOrderLocationStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, 
            bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderLocationStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} 
                    (users.distributor > 0 {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})";

                cmd.CommandText =
					String.Format(@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({3}) and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.consolidated_port) AS orders
						GROUP BY orders.customer_code, orders.consolidated_port
						ORDER BY orders.customer_code, orders.consolidated_port",
                                  GetCountryCondition(countryFilter, "users."), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderLocationStats { client_code = String.Empty + dr["customer_code"], location = Convert.ToInt32(dr["consolidated_port"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderLocationStats> GetOrderLocationStats_Out(DateTime from,DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
            CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			var result = new List<OrderLocationStats>();
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
		    if (daysToShipping == null)
		        daysToShipping = Settings.Default.Analytics_Default_DaysToShipping;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText =
					String.Format(@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({4}) and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.consolidated_port) AS orders
						GROUP BY orders.customer_code, orders.consolidated_port
						ORDER BY orders.customer_code, orders.consolidated_port",
								  GetCountryCondition(countryFilter, "users."),
                                  GetOrdersDateCriteria(mode), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@new_from", new_from);
				cmd.Parameters.AddWithValue("@new_to", new_to);
			    cmd.Parameters.AddWithValue("@daysToShipping", daysToShipping);
                cmd.Parameters.AddWithValue("@etafrom", etaCriteriaFrom);
                cmd.Parameters.AddWithValue("@etato", etaCriteriaTo);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderLocationStats { client_code = String.Empty + dr["customer_code"], location = Convert.ToInt32(dr["consolidated_port"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
				}

			}
			return result;
		}

		public static List<OrderLocationStats> GetOrderLocationStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly, 
            bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderLocationStats>();
			
            var excludedContainerTypes = Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				//new orders
				var cmd = Utils.GetCommand("", conn);
                var brandsClause =
                    $@" AND {(brands ? "" : "NOT")} 
                    (users.distributor > 0 {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR users.user_id IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributors, "dist_")})" : "")})";

                cmd.CommandText =
					String.Format(@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
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
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN({3}) and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.consolidated_port) AS orders
						GROUP BY orders.customer_code, orders.consolidated_port
						ORDER BY orders.customer_code, orders.consolidated_port",
                                  GetCountryCondition(countryFilter, "users."), brandsClause, HandleClients(cmd, "userid1", includedClients, excludedClients), Utils.CreateParametersFromIdList(cmd, excludedContainerTypes, "containerType"));
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new OrderLocationStats { client_code = String.Empty + dr["customer_code"], location = Convert.ToInt32(dr["consolidated_port"]), orderCount = Convert.ToInt32(dr["numOfOrders"]) });
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT brands.brandname, mast_products.product_group, COUNT(*) AS numOfProducts
						FROM brands
						INNER JOIN cust_products ON cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
						INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
						WHERE NOT EXISTS (SELECT cust_products.cprod_id FROM cust_products other_cp WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND cust_products.cprod_user <> other_cp.cprod_user AND other_cp.cprod_status <> 'D') AND mast_products.category1 <> 13
						AND cust_products.cprod_status <> 'D' AND brands.eb_brand = 1 AND COALESCE(cust_products.report_exception,0) = 0 {0}
						GROUP BY brands.brandname, mast_products.product_group
						UNION 
						SELECT 'Universal' AS brandname, mast_products.product_group, COUNT(*) AS numOfProducts
						FROM brands
						INNER JOIN cust_products ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
						INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
						WHERE EXISTS (SELECT cust_products.cprod_id FROM cust_products other_cp WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND cust_products.cprod_user <> other_cp.cprod_user AND other_cp.cprod_status <> 'D'
						AND other_cp.cprod_user IN (SELECT brands.user_id FROM brands WHERE eb_brand = 1)
						) AND mast_products.category1 <> 13 AND brands.eb_brand = 1 AND COALESCE(cust_products.report_exception,0) = 0 {0}
						AND cust_products.cprod_status <> 'D'
						GROUP BY mast_products.product_group
						ORDER BY brandname ASC, product_group ASC",ExcludedCprodBrandCatsCondition), conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductStats{brandname = String.Empty + dr["brandname"],product_group = String.Empty + dr["product_group"], numOfProducts = Convert.ToInt32(dr["numOfProducts"])});
				}
				dr.Close();
				result = result.OrderBy(r => r.brandname == "Universal" ? "ZZ" : r.brandname).ToList();
			}
			return result;
		}

		public static List<ProductLocationStats> GetProductLocationStats(string product_group, int? ageForExclusion = null, IList<string> distCountries = null)
		{
			var result = new List<ProductLocationStats>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = String.Format(@"SELECT cust_products.cprod_code1, cust_products.cprod_name,brands.brandname,users.consolidated_port,
											(SELECT GROUP_CONCAT(other_mp.product_group) FROM  cust_products other_cp
											INNER JOIN mast_products other_mp ON other_mp.mast_id = other_cp.cprod_mast
											INNER JOIN users other_fact ON other_fact.user_id = other_mp.factory_id
											INNER JOIN brands other_b ON other_cp.cprod_user = other_b.user_id AND other_b.eb_brand = 1
											WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND other_cp.cprod_status <> 'D' AND other_mp.category1 <> 13 AND other_cp.cprod_brand_cat NOT IN ({2}) AND COALESCE(other_cp.report_exception,0) = 0
											AND cust_products.cprod_id <> other_cp.cprod_id AND other_fact.consolidated_port <> users.consolidated_port AND LEFT(other_mp.product_group,1) <> @group) AS productgroups_other
											FROM  cust_products
											INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
											INNER JOIN users ON users.user_id = mast_products.factory_id
											INNER JOIN brands ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
											WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 AND cprod_brand_cat NOT IN ({2}) 
                                            AND COALESCE(cust_products.report_exception,0) = 0
                                            AND EXISTS(SELECT client_id FROM dist_products WHERE dist_cprod_id = cust_products.cprod_id AND client_id = 85)
											{0} {1}
											AND LEFT(mast_products.product_group,1) = @group", ageForExclusion != null ? GetETDExclusionCriteria(ageForExclusion.Value) : "", distCountries != null ? GetDistributorExclusionCriteria(cmd,distCountries) : "",ExcludedCprodBrandCats);
				cmd.Parameters.AddWithValue("@group", product_group);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductLocationStats
					{
						cprod_code = String.Empty + dr["cprod_code1"],
						cprod_name = String.Empty + dr["cprod_name"],
                        brandname = String.Empty + dr["brandname"],
						location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
						productgroup_others = dr["productgroups_other"] != DBNull.Value ? (String.Empty + dr["productgroups_other"]).Split(',') : null
					});
				}
				dr.Close();
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="product_group"></param>
		/// <param name="ageForExclusion">In months. Exclude products that have shipping data not older than given parameter e.g. if ageForExlusion = 3, products with MIN(etd) > today-3 months would be excluded</param>
		/// <param name="distCountries">Include only products that are related to distributors in given countries</param>
		/// <returns></returns>
		public static List<ProductLocationStatsSummary> GetProductLocationStatsSummary(string product_group, int? ageForExclusion = null, IList<string> distCountries = null )
		{
			var result = new List<ProductLocationStatsSummary>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = String.Format(@"SELECT brands.brandname,users.consolidated_port,COUNT(*) AS numOfProducts
											FROM cust_products
											INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
											INNER JOIN users ON users.user_id = mast_products.factory_id
											INNER JOIN brands ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
											WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 {2} AND COALESCE(cust_products.report_exception,0) = 0 AND 
											mast_products.product_group = @group {0} {1}
											GROUP BY brands.brandname, users.consolidated_port", ageForExclusion != null ? GetETDExclusionCriteria(ageForExclusion.Value) : "",distCountries != null ? GetDistributorExclusionCriteria(cmd,distCountries) : "",ExcludedCprodBrandCatsCondition);
				cmd.Parameters.AddWithValue("@group", product_group);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductLocationStatsSummary
					{
						brandname = String.Empty + dr["brandname"],
						location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
						numOfProducts = Convert.ToInt32(dr["numOfProducts"])
					});
				}
				dr.Close();
			}
			return result;
		}

		private static string GetETDExclusionCriteria(int months)
		{
			return
				String.Format(@" AND DATE_ADD((SELECT MAX(porder_header.po_req_etd) FROM order_lines INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
									INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid WHERE order_lines.cprod_id = cust_products.cprod_id), INTERVAL {0} MONTH) > CURDATE()
						",months);
		}

		private static string GetDistributorExclusionCriteria(MySqlCommand cmd, IList<string> countries )
		{
			return
				String.Format(@" AND EXISTS (SELECT distprod_id FROM dist_products INNER JOIN users ON dist_products.client_id = users.user_id WHERE dist_products.dist_cprod_id = cust_products.cprod_id AND
									users.user_country IN ({0}))",Utils.CreateParametersFromIdList(cmd,countries));
		}

		public static List<ProductLocationStats> GetAlternateProductsStats(string product_group)
		{
			var result = new List<ProductLocationStats>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT cust_products.cprod_code1, cust_products.cprod_name,users.consolidated_port, mast_products.product_group,brands.brandname
											FROM  cust_products
											INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
											INNER JOIN users ON users.user_id = mast_products.factory_id
											INNER JOIN brands ON cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
											WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 {0} AND COALESCE(cust_products.report_exception,0) = 0 AND
																						EXISTS (SELECT other_mp.product_group FROM cust_products other_cp
											INNER JOIN mast_products other_mp ON other_mp.mast_id = other_cp.cprod_mast
											INNER JOIN users other_fact ON other_fact.user_id = other_mp.factory_id
											INNER JOIN brands other_b ON other_cp.brand_user_id = other_b.user_id AND other_b.eb_brand = 1
											WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND other_cp.cprod_status <> 'D' AND other_mp.category1 <> 13 AND other_cp.cprod_brand_cat NOT IN ({1}) AND COALESCE(other_cp.report_exception,0) = 0
											AND cust_products.cprod_id <> other_cp.cprod_id AND other_fact.consolidated_port <> users.consolidated_port AND other_mp.product_group = @group) AND
											mast_products.product_group <> @group",ExcludedCprodBrandCatsCondition,ExcludedCprodBrandCats), conn);
				cmd.Parameters.AddWithValue("@group", product_group);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductLocationStats
					{
						cprod_code = String.Empty + dr["cprod_code1"],
						cprod_name = String.Empty + dr["cprod_name"],
                        brandname = String.Empty + dr["brandname"],
						location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
						maxgroup = String.Empty + dr["product_group"]
					});
				}
				dr.Close();
			}
			return result;
		}

		public static List<ProductSales> GetTopNByBrands(int from, int to,CountryFilter countryFilter = CountryFilter.UKOnly )
		{
			var result = new List<ProductSales>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT brand_user_id,cprod_code1,cprod_name,brand_code, sum(sogbp*orderqty) as total, sum(orderqty) as numOfProducts, 
						COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
						   AND NOT EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 AND cust_products.brand_user_id <> brand_sales_analysis_product2.brand_user_id )
						   AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
							{1}
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0  {0} 
							GROUP BY brand_user_id,brand_code,cprod_code1,cprod_name
							ORDER BY numOfProducts DESC
							", GetCountryCondition(countryFilter),ExcludedCprodBrandCatsCondition), conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductSales
					{
						Amount = (double)dr["total"],
						cprod_name = String.Empty + dr["cprod_name"],
						brand_code = String.Empty + dr["brand_code"],
						Margin = (double)dr["margin"],
						numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
						cprod_code = String.Empty + dr["cprod_code1"]
					});
				}
				dr.Close();
			}
			return result;
		}

		public static List<ProductSales> GetTopNUniversal(int n,int from, int to,CountryFilter countryFilter = CountryFilter.UKOnly)
		{
			var result = new List<ProductSales>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT cprod_code1,cprod_name,brand_code, sum(sogbp*orderqty) as total, sum(orderqty) as numOfProducts, 
						COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
							AND EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id 
										WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 
										AND cust_products.brand_user_id <> brand_sales_analysis_product2.brand_user_id )
						   AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
							{1}
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {0} 
							GROUP BY cprod_code1,cprod_name
							ORDER BY numOfProducts DESC LIMIT @limit
							", GetCountryCondition(countryFilter),ExcludedCprodBrandCatsCondition), conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@limit", n);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductSales
					{
						Amount = (double)dr["total"],
						cprod_name = String.Empty + dr["cprod_name"],
						brand_code = String.Empty + dr["brand_code"],
						Margin = (double)dr["margin"],
						numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
						cprod_code = String.Empty + dr["cprod_code1"]
					});
				}
				dr.Close();
			}
			return result;
		}

		public static List<ProductSales> GetTopForBrandCat(int n,int from, int to,int cprod_brand_cat, CountryFilter countryFilter = CountryFilter.UKOnly)
		{
			var result = new List<ProductSales>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{

				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT cprod_code1,cprod_name,brand_code, sum(sogbp*orderqty) as total, sum(orderqty) as numOfProducts, 
						COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
							AND EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 GROUP BY cprod_code1, brand_user_id HAVING COUNT(DISTINCT brand_user_id) = 1)
						   AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
							AND cprod_brand_cat = @cprod_brand_cat
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0  {0} 
							GROUP BY brand_user_id,brand_code,cprod_code1,cprod_name
							ORDER BY numOfProducts DESC LIMIT @limit
							", GetCountryCondition(countryFilter)), conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@limit", n);
				cmd.Parameters.AddWithValue("@cprod_brand_cat", cprod_brand_cat);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductSales
					{
						Amount = (double)dr["total"],
						cprod_name = String.Empty + dr["cprod_name"],
						brand_code = String.Empty + dr["brand_code"],
						Margin = (double)dr["margin"],
						numOfUnits = Convert.ToInt32(dr["numOfProducts"]),
						cprod_code = String.Empty + dr["cprod_code1"]
					});
				}
				dr.Close();
			}
			return result;
		}

		public static List<ProductSales> GetSalesForCategory(int brand_category)
		{
			var result = new List<ProductSales>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(@"SELECT cust_products.cprod_id, cust_products.cprod_name, SUM(order_lines.orderqty*order_lines.unitprice) AS sales
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
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(@"SELECT cust_products.cprod_id, cust_products.cprod_name, COUNT(*) AS num
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

		public static List<ProductDistributorDisplayCount> GetDisplayCountForProductsAndBrand(int? brand_user_id, IList<int> category_ids)
		{
			var result = new List<ProductDistributorDisplayCount>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("",conn);
				cmd.CommandText = String.Format(@"SELECT cust_products.cprod_id, cust_products.cprod_name,cust_products.cprod_code1, dist.user_id, dist.customer_code, COUNT(*) AS num
											FROM  cust_products
											INNER JOIN web_product_component ON web_product_component.cprod_id = cust_products.cprod_id
											INNER JOIN web_product_new ON web_product_component.web_unique = web_product_new.web_unique
											INNER JOIN dealer_image_displays ON dealer_image_displays.web_unique = web_product_new.web_unique
											INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique 
											INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
											INNER JOIN users dist ON dealers.user_type = dist.user_id
											INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
											LEFT OUTER JOIN analytics_subcategory ON cust_products.analytics_category = analytics_subcategory.subcat_id
                                            {2}
											WHERE {1} AND mast_products.category1 <> 13
											{0}
											GROUP BY cust_products.cprod_id,dist.user_id",
									  category_ids != null && category_ids.Count > 0 ? 
												String.Format("AND analytics_subcategory.category_id IN ({0})", Utils.CreateParametersFromIdList(cmd,category_ids)) : "",
                                      brand_user_id != null ? "cust_products.brand_user_id = @brand_user_id" : "analytics_categories.category_type IS NULL",
                                      brand_user_id != null ? "" : "INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id");
                if(brand_user_id != null)
				    cmd.Parameters.AddWithValue("@brand_user_id", brand_user_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ProductDistributorDisplayCount { cprod_id = (int)dr["cprod_id"],cprod_code1 = String.Empty + dr["cprod_code1"],cprod_name = String.Empty + dr["cprod_name"],
						distributor_code  = String.Empty + dr["customer_code"], DisplayCount = Convert.ToInt32(dr["num"]) });
				}
			}
			return result;
		}

		public static List<Company> GetFactoriesFromSales(int? month21From = null, int? month21To = null)
		{
			var result = new List<Company>();
			
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"SELECT users.* 
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

		public static double? GetSpecialTermsTotal(int monthETA)
		{
			double? result = 0.0;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						@"SELECT SUM(rowprice_gbp) FROM brand_sales_analysis2 WHERE month22 = @month22 AND special_terms = 1",
						conn);
				cmd.Parameters.AddWithValue("@month22", monthETA);
				result = Utilities.FromDbValue<double>(cmd.ExecuteScalar());
			}
			return result;
		}

        //TODO: Adjust for more customers
	    public static List<ProductSales> GetProductSalesAfterStockDate(string customer_code)
	    {
	        var result = new List<ProductSales>();
	        using (var conn = new MySqlConnection(Settings.Default.ConnString))
	        {
	            conn.Open();
	            var cmd = Utils.GetCommand(@"SELECT cprod_code1, COALESCE(SUM(orderqty),0) AS qty, COALESCE(SUM(rowprice_gbp),0.0) AS Amount 
                                            FROM brand_sales_analysis2 WHERE customer_code = @code AND req_eta > dist_stock_date GROUP BY cprod_code1",conn);
	            cmd.Parameters.AddWithValue("@code", customer_code);
	            var dr = cmd.ExecuteReader();
	            while (dr.Read())
	            {
	                var r = new ProductSales
	                {
	                    cprod_code = String.Empty + dr["cprod_code1"],
	                    numOfUnits = Convert.ToInt32(dr["qty"]),
	                    Amount = Convert.ToDouble(dr["Amount"])
	                };
	                result.Add(r);
	            }
                dr.Close();
	        }
	        return result;

	    }

        public static List<StockSummary> GetStockSummaryReports2(IList<int> includedClients, double qcCharge, double duty, double freight, DateTime? from = null)
        {
            var result = new List<StockSummary>();
            var date = from ?? DateTime.Today;
            //var clientIds = includedClients.Select(id => (int?)id).ToList();
            var factory_codes = GetFactoriesOnStockOrders(includedClients, date);
            var unallocated = GetUnallocatedAtFactory(includedClients);
            result.AddRange(unallocated.Where(o=>factory_codes.Contains(o.FactoryCode)).Select(o => new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, UnallocatedAtFactoryValue = o.Value }));

            var allocated = GetAllocatedAtFactory(includedClients, date);
            //merge
            foreach (var o in allocated.Where(o => factory_codes.Contains(o.FactoryCode))) {
                var stockRecord = result.FirstOrDefault(s => s.factory_id == o.FactoryId);
                if (stockRecord != null)
                    stockRecord.AllocatedAtFactory = o.Value;
                else
                    result.Add(new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, AllocatedAtFactory = o.Value });
            }

            //onWater
            var onWater = GetOnWater(includedClients, date);

            //merge
            foreach (var o in onWater.Where(o => factory_codes.Contains(o.FactoryCode))) {
                var stockRecord = result.FirstOrDefault(s => s.factory_id == o.FactoryId);
                if (stockRecord != null)
                    stockRecord.OnWater = o.Value;
                else
                    result.Add(new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, OnWater = o.Value });
            }

            //stock uk
            var warehouseSummary = GetProductStock(includedClients);
            foreach (var w in warehouseSummary.Where(o => factory_codes.Contains(o.FactoryCode))) {
                var stockRecord = result.FirstOrDefault(s => s.factory_id == w.FactoryId);
                if (stockRecord != null)
                    stockRecord.Warehouse = w.Value;
                else {
                    result.Add(new StockSummary { factory_id = w.FactoryId, factory_code = w.FactoryCode, Warehouse = w.Value });
                }
            }
            
            return result;
        }

        public static List<StockFactoryRow> GetUnallocatedAtFactory(IList<int> clientIds)
        {
            var result = new List<StockFactoryRow>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factory_id, fn_combinedfactorycode(factory_code,factory.combined_factory) AS factory_code, SUM(order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum)) AS Qty, 
                                    SUM(fn_unitprice_gbp(porder_lines.unitprice, porder_lines.unitcurrency) * (order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum))) AS Value
									FROM order_lines
									INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
									INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                    INNER JOIN users factory ON factory.user_id = porder_header.userid
									WHERE order_header.stock_order = 1 AND order_header.userid1 IN ({0}) AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0 AND
                                    (order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum) > 0)
                                    GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)", Utils.CreateParametersFromIdList(cmd, clientIds));
                var dr = cmd.ExecuteReader();
                
                while (dr.Read()) {
                    var r = new StockFactoryRow
                    {
                        FactoryId = (int)dr["factory_id"],
                        FactoryCode = String.Empty + dr["factory_code"],
                        Qty = Convert.ToInt32(Utilities.FromDbValue<long>(dr["Qty"])),
                        Value = Utilities.FromDbValue<double>(dr["Value"])
                    };
                    result.Add(r);
                }

                dr.Close();
            }
            return result;
        }


	    public static List<StockFactoryRow> GetAllocatedAtFactory(IList<int> clientIds, DateTime date)
	    {
	        var result = new List<StockFactoryRow>();
	        using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
	            conn.Open();
	            var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factory_id, fn_combinedfactorycode(factory_code,factory.combined_factory) AS factory_code, SUM(order_lines.orderqty) AS Qty, SUM(fn_unitprice_gbp(porder_lines.unitprice, porder_lines.unitcurrency) * order_lines.orderqty) AS Value
									FROM order_lines
									INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
									INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                    INNER JOIN users factory ON factory.user_id = porder_header.userid
									WHERE order_header.stock_order = 8 AND order_header.userid1 IN ({0}) AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                                    AND porder_header.po_req_etd > @date
                                    GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)
									", Utils.CreateParametersFromIdList(cmd, clientIds));
                cmd.Parameters.AddWithValue("@date", date);
	            var dr = cmd.ExecuteReader();
	            while (dr.Read()) {
	                var r = new StockFactoryRow
	                {
	                    FactoryId = (int)dr["factory_id"],
	                    FactoryCode = String.Empty + dr["factory_code"],
	                    Qty = Convert.ToInt32(Utilities.FromDbValue<long>(dr["Qty"])),
	                    Value = Utilities.FromDbValue<double>(dr["Value"])
	                };
	                result.Add(r);
	            }

	            dr.Close();
	        }
	        return result;
	    }

        public static List<StockFactoryRow> GetOnWater(IList<int> clientIds, DateTime date)
        {
            var result = new List<StockFactoryRow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factory_id, fn_combinedfactorycode(factory_code,factory.combined_factory) AS factory_code, SUM(order_lines.orderqty) AS Qty, SUM(fn_unitprice_gbp(porder_lines.unitprice, porder_lines.unitcurrency) * order_lines.orderqty) AS Value
									FROM order_lines
									INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
									INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                    INNER JOIN users factory ON factory.user_id = porder_header.userid
									WHERE order_header.stock_order <> 1 AND order_header.userid1 IN ({0}) AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                                    AND porder_header.po_req_etd < @date AND order_header.req_eta > @date
                                    GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)
									", Utils.CreateParametersFromIdList(cmd, clientIds));
                cmd.Parameters.AddWithValue("@date", date);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    var r = new StockFactoryRow
                    {
                        FactoryId = (int)dr["factory_id"],
                        FactoryCode = String.Empty + dr["factory_code"],
                        Qty = Convert.ToInt32(Utilities.FromDbValue<long>(dr["Qty"])),
                        Value = Utilities.FromDbValue<double>(dr["Value"])
                    };
                    result.Add(r);
                }

                dr.Close();
            }
            return result;
        }

	    public static List<StockFactoryRow> GetProductStock(IList<int> clientIds)
	    {
            var result = new List<StockFactoryRow>();
	        using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
	        {
	            conn.Open();
                var cmd = Utils.GetCommand("", conn);
	            cmd.CommandText =
                    String.Format(@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factory_id, fn_combinedfactorycode(factory_code,factory.combined_factory) AS factory_code, cprod_stock AS Qty, SUM(cprod_stock * (CASE WHEN mast_products.price_pound > 0 THEN mast_products.price_pound ELSE mast_products.price_dollar / 1.6 END)) AS Value
                                                FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                                                WHERE cprod_user IN ({0}) AND EXISTS (SELECT * FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid WHERE stock_order = 1 AND order_lines.cprod_id = cust_products.cprod_id)
                                                GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)",
	                    Utils.CreateParametersFromIdList(cmd, clientIds));
	            var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    var r = new StockFactoryRow
                    {
                        FactoryId = (int)dr["factory_id"],
                        FactoryCode = String.Empty + dr["factory_code"],
                        Qty = Convert.ToInt32(Utilities.FromDbValue<long>(dr["Qty"])),
                        Value = Utilities.FromDbValue<double>(dr["Value"])
                    };
                    result.Add(r);
                }

                dr.Close();
	        }
	        return result;
	    }

	    public static List<string> GetFactoriesOnStockOrders(IList<int> clientIds, DateTime from)
	    {
	        var result = new List<string>();
	        using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
	        {
	            conn.Open();
	            var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT fn_combinedfactorycode(factory_code,factory.combined_factory) as factory_code
                                FROM order_lines
                                INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                                INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                INNER JOIN order_header ON order_lines.orderid = order_header.orderid
									                                INNER JOIN users factory ON factory.user_id = porder_header.userid
                                WHERE order_header.stock_order = 1 AND order_header.userid1 IN ({0}) AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                                AND porder_header.po_req_etd >= @date
                                GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory)", Utils.CreateParametersFromIdList(cmd, clientIds));
                cmd.Parameters.AddWithValue("@date", from);
                var dr = cmd.ExecuteReader();
	            while (dr.Read())
	            {
                    result.Add(string.Empty + dr["factory_code"]);
	            }
                dr.Close();
	        }
	        return result;

	    }

        public static List<int> GetFactoriesOnStockOrders_NoCombined(IList<int> clientIds, DateTime from)
        {
            var result = new List<int>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT factory.user_id AS factory_id
                                FROM order_lines
                                INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                                INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                INNER JOIN order_header ON order_lines.orderid = order_header.orderid
									                                INNER JOIN users factory ON factory.user_id = porder_header.userid
                                WHERE order_header.stock_order = 1 AND order_header.userid1 IN ({0}) AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                                AND porder_header.po_req_etd >= @date
                                GROUP BY factory.user_id", Utils.CreateParametersFromIdList(cmd, clientIds));
                cmd.Parameters.AddWithValue("@date", from);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    result.Add((int) dr["factory_id"]);
                }
                dr.Close();
            }
            return result;

        }


        /// <summary>
        /// returns current stock value for factory_id and cprod_id
        /// </summary>
        public static double GetStockValueForCurrentCprodUserFactory(int cprod_user, string includedFactories = null, string excludedFactories = null)
        {
            double result = 0;

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "BRANDSTOCKSUMMARY_CURRENT_PROC";
                /*
                cmd.CommandText = $@"SELECT SUM(cprod_stock * price_dollar) as stock_value
                                    FROM
	                                    cust_product_details_inc_delete
                                    WHERE
                                        cprod_user = @cprod_user	                                                        
                                    {(includedFactories != null ? $" AND factory_id IN ({Utils.CreateParametersFromIdList(cmd, includedFactories)})" : "")}
                                    {(excludedFactories != null ? $" AND factory_id NOT IN ({Utils.CreateParametersFromIdList(cmd, excludedFactories)})" : "")}
                                    ";
                */
                cmd.Parameters.AddWithValue("in_CPROD_USER", cprod_user);
                cmd.Parameters.AddWithValue("in_FACTORIES_INC", includedFactories);
                cmd.Parameters.AddWithValue("in_FACTORIES_EXC", excludedFactories);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    result = dr.GetDouble("stock_value");
                }
            }

            return result;
        }

        public static List<CostValueItem> GetCostValueOfItemsSoldForCprodUserFactory(int cprod_user, DateTime from,string includedFactories = "", string excludedFactories = "")
        {
            List<CostValueItem> resultList = new List<CostValueItem>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "BRANDSTOCKSUMMARY_COST_PROC";

                /*
                cmd.CommandText = $@"
                        SELECT
                            sales_orders.date_entered,
                            sales_orders.order_qty,
                            cust_product_details_inc_delete.price_dollar
                        FROM
                            sales_orders
                        INNER JOIN 
                            cust_product_details_inc_delete ON sales_orders.cprod_id = cust_product_details_inc_delete.cprod_id
                        WHERE 
                            cprod_user = @cprod_user and date_entered >= @from 
                        {(includedFactories != null ? $" AND factory_id IN ({Utils.CreateParametersFromIdList(cmd, includedFactories)})" : "")}
                        {(excludedFactories != null ? $" AND factory_id NOT IN ({Utils.CreateParametersFromIdList(cmd, excludedFactories)})" : "")}
                        ";
                */
                cmd.Parameters.AddWithValue("in_CPROD_USER", cprod_user);
                //cmd.Parameters.AddWithValue("@factory_id", factory_id);
                cmd.Parameters.AddWithValue("in_DATE_FROM", from);
                cmd.Parameters.AddWithValue("in_FACTORIES_INC", includedFactories);
                cmd.Parameters.AddWithValue("in_FACTORIES_EXC", excludedFactories);

                var dr = cmd.ExecuteReader();

                CostValueItem item;

                while (dr.Read())
                {
                    item = new CostValueItem();
                    item.date_entered = dr.GetDateTime("date_entered");
                    item.order_qty = dr.GetInt32("order_qty");
                    item.price = dr.GetDouble("price");

                    resultList.Add(item);

                }
            }

            return resultList;
        }

        public static List<StockReceiptItem> GetStockReceiptsForCprodUserFactory(int cprod_user, DateTime from, string includedFactories = null, string excludedFactories = null)
        {
            List<StockReceiptItem> resultList = new List<StockReceiptItem>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "BRANDSTOCKSUMMARY_RECEIPTS_PROC";

                /*
                cmd.CommandText = $@"
                        SELECT
                            order_line_detail2_v6.booked_in_date,
                            order_line_detail2_v6.factory_id,
                            order_line_detail2_v6.orderqty,
                            order_line_detail2_v6.unitprice
                        FROM
                            order_line_detail2_v6
                        WHERE 
                           cprod_user = @cprod_user and booked_in_date >= @from
                        {(includedFactories != null ? $" AND factory_id IN ({Utils.CreateParametersFromIdList(cmd, includedFactories)})" : "")}
                        {(excludedFactories != null ? $" AND factory_id NOT IN ({Utils.CreateParametersFromIdList(cmd, excludedFactories)})" : "")}
                ";
                */

                cmd.Parameters.AddWithValue("in_CPROD_USER", cprod_user);
                //cmd.Parameters.AddWithValue("@factory_id", factory_id);
                cmd.Parameters.AddWithValue("in_DATE_FROM", from);
                cmd.Parameters.AddWithValue("in_FACTORIES_INC", includedFactories);
                cmd.Parameters.AddWithValue("in_FACTORIES_EXC", excludedFactories);

                var dr = cmd.ExecuteReader();

                StockReceiptItem item;

                while (dr.Read()) {
                    item = new StockReceiptItem();
                    item.req_eta = dr.GetDateTime("req_eta");
                    item.booked_in_date = dr.GetDateTime("booked_in_date");
                    item.factory_id = dr.GetInt32("factory_id");
                    item.orderqty = dr.GetInt32("orderqty");
                    item.unit_price = dr.GetDouble("unitprice");

                    resultList.Add(item);
                }
            }

            return resultList;
        }
	}
}
