using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using company.Common;

namespace erp.Model.Dal.New
{
	public class AnalyticsDAL : IAnalyticsDAL
	{
		private MySqlConnection conn;
		private ICountriesDAL countriesDAL;
		private readonly IBrandsDAL brandsDal;
		private readonly ICompanyDAL companyDAL;

		public AnalyticsDAL(IDbConnection conn, ICountriesDAL countriesDAL, IBrandsDAL brandsDal, ICompanyDAL companyDAL)
		{
			this.conn = (MySqlConnection) conn;			
			this.countriesDAL = countriesDAL;
			this.brandsDal = brandsDal;
			this.companyDAL = companyDAL;
		}

		public string ExcludedCprodBrandCatsCondition = $"AND cprod_brand_cat NOT IN({ExcludedCprodBrandCats})";
		public const string ExcludedCprodBrandCats = "115,207,307,608,708,912,8081";

		public List<ProductSales> GetProductSales(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, 
			int? brand_user_id = null, IList<int> factoryIds = null, DateTime? fromDate = null, DateTime? toDate = null, 
			bool brands = true, bool monthBreakDown = false, IList<int> incClients = null, IList<int> exClients = null,
			int? brand_id = null, bool ignoreFactoryCode = false, bool useETA = false, IList<int> prodIds = null, 
			bool useOrderDate = false, bool clientBreakDown = false, bool useLineDate = false, bool shippedOrdersOnly = true, 
			bool periodBreakDown = false, IList<string> excludedCustomers = null)
		{
			var result = new List<ProductSales>();
			
			var groupfields = factoryIds != null ? "factory_ref,asaq_name,factory_code, factory_stock" : 
				brands ? $"cprod_id, cprod_code1,cprod_name,brand_code{(ignoreFactoryCode ? "" : ",factory_code, factory_stock")}" : 
					$"cprod_id, cprod_code1,cprod_name{(ignoreFactoryCode ? "" : ",factory_code, factory_stock")}";

			var selectFields = factoryIds != null ? "0 as cprod_id,factory_ref AS cprod_code,asaq_name AS cprod_name, '' AS brand_code,factory_code, factory_stock" : 
				brands ? "cprod_id,cprod_code1 as cprod_code,cprod_name,brand_code,factory_code,factory_stock" 
				: "cprod_id,cprod_code1 as cprod_code,cprod_name,factory_code";
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

			var dateField = useETA ? "req_eta" : useOrderDate ? "orderdate" : useLineDate ? "linedate" : "po_req_etd";
			var rangeCriteria = @from != null ? "month21 BETWEEN @from AND @to" : 
				$"({dateField} >=  @from OR @from IS NULL) AND ({dateField} <= @to OR @to IS NULL)";
            var view = brands ? shippedOrdersOnly ? "brand_sales_analysis_product2" : "brand_sales_analysis_product3" : "brs_sales_analysis_product2";
            var sql  =	$@"SELECT {selectFields}, sum(sogbp*orderqty) as Amount,SUM(poprice*orderqty) AS POAmount,
							SUM(fn_unitprice_gbp(unitprice,unitcurrency)*orderqty) AS POAmountGBP, sum(orderqty) as numOfUnits, 
							COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
							FROM {view} WHERE {rangeCriteria} 
							AND cprod_code1 not like 'SP%' {(brands ? " and distributor > 0 " : " AND COALESCE(distributor,0) = 0")} 
							AND rowprice > 0 {(brand_id != null ? " AND brand_id = @brand_id " : "")} 
							{(prodIds != null ? " AND cprod_id IN @prodIds" : "")}
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 AND (brand_user_id = @brand_user_id OR @brand_user_id IS NULL) 
							{countriesDAL.GetCountryCondition(countryFilter)}
							{(factoryIds != null ? " AND factory_id IN @factoryIds" : "")}
							 {(incClients != null || exClients != null ? $" AND cprod_user {(exClients != null ? " NOT " : "")} IN @clientIds" : "")} 
							{(excludedCustomers != null ? " AND customer_code NOT IN @excludedCustomers": "")} 
							GROUP BY {groupfields}";
			return conn.Query<ProductSales>(sql, 
				new {
					from = @from != null ? (object) @from : fromDate,
					to = to != null ? (object) to : toDate,
					brand_user_id,
					brand_id,
					monthyear = toDate != null ?  "20" + company.Common.Utilities.GetMonth21FromDate(toDate.Value) : null,
					day = toDate?.Day,
					factoryIds,
					prodIds,
					clientIds = exClients ?? incClients,
					excludedCustomers
					}).ToList();
				
		}

		public List<SalesByMonth> GetSalesByMonth(int from, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null, 
			bool useETA=false, bool? brands = true,int[] includedClients = null,int[] excludedClients=null,List<int> factory_ids = null,
			int? brand_id = null, string excludedCustomers = "NK2", IList<int> includedNonDistributors = null, bool useOrderDate = false, 
			IList<int> productIds = null, bool factoryProducts = false, bool useCompanyPriceType = false )
		{
			var result = new List<SalesByMonth>();
			var monthField = useETA ? (useCompanyPriceType ? "month23" : "month22") : 
                                      (useOrderDate ? "month20" : "month21");
			var priceField = factory_ids != null ? "po_rowprice_gbp" : (monthField == "month23" && brands == false) ? "rowprice_gbp_23" : "rowprice_gbp";
			string view =  factory_ids != null ? "factory_sales_analysis" :  brands != false ? "brand_sales_analysis3" : "brs_sales_analysis2";
            			
            string distributorCriteria = brands == true ? $@" AND (distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? 
								$" OR client_id IN @includedNonDistributors" : "")})" :
                        "";
			var customerCodes = excludedCustomers.Split(',');
			var clientIds = includedClients ?? excludedClients;
			var sql =
					$@"SELECT  {monthField} as Month21, SUM({priceField}) as Amount,SUM(orderqty) AS Qty,
                        {(brands == true ? $"SUM(IF(special_terms,{priceField},0)) AS SpecialAmount," : "0 AS SpecialAmount,")} 
                        COUNT(DISTINCT orderid) AS numOfOrders 
                        FROM {view} 
                        WHERE ({monthField} >=  @from OR @from IS NULL) AND ({monthField} <= @to OR @to IS NULL)
						AND customer_code NOT IN @customerCodes 
                        AND custpo NOT LIKE 'IR%' 
                        AND category1 <> 13 
                        AND cprod_user > 0 
                        {countriesDAL.GetCountryCondition(countryFilter)} 
                        {(includedClients != null || excludedClients != null ? 
							$" AND cprod_user {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")} 
                        {(factory_ids != null ? "" : " AND (brand_user_id = @brand_user_id OR @brand_user_id IS NULL)")} 
                        {(brand_id != null ? " AND brand_id = @brand_id " : "")} 
                        {distributorCriteria}
						{(factory_ids != null ? $" AND factory_id IN @factory_ids" : "")} 
                        {(productIds != null ? $" AND {(factoryProducts ? "cprod_mast" : "cprod_id")}  IN @productIds" : "")}
                        GROUP BY {monthField} ";
			return conn.Query<SalesByMonth>(sql, new
			{
				from, 
				to,
				brand_user_id,
				brand_id,
				factory_ids,
				customerCodes,
				productIds,
				clientIds,
				includedNonDistributors
			}).ToList();				
			
		}

		public List<SalesByMonth> GetSalesByMonthNoView(int from, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly, int? brand_user_id = null,
			bool useETA = false, bool? brands = true, int[] includedClients = null, int[] excludedClients = null, List<int> factory_ids = null,
			int? brand_id = null, string excludedCustomers = "NK2", IList<int> includedNonDistributors = null, bool useOrderDate = false,
			IList<int> productIds = null, bool factoryProducts = false, bool useCompanyPriceType = false)
		{
			var field = useETA ? "oh.req_eta" : useOrderDate ? "oh.orderdate" : "po.req_etd";
			string distributorCriteria = brands == true ? $@" AND (client.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0 ?
								$" OR oh.userid1 IN @includedNonDistributors" : "")})" :
						"";			
			var clientIds = includedClients ?? excludedClients;
			var fromDate = new Month21(from).Date;
			var toDate = to != null ? (DateTime?) Utilities.GetMonthEnd(new Month21(to.Value).Date) : null;

			var sql = $@"SELECT year({field}) year, month({field}) month, SUM(ol.orderqty * ol.unitprice * IF(ol.unitcurrency = 0, 0.625 , 1)) as Amount,
						SUM(ol.orderqty) AS Qty,0 AS SpecialAmount, 
						COUNT(DISTINCT ol.orderid) AS numOfOrders 
						FROM porder_header po INNER JOIN porder_lines pl ON po.porderid = pl.porderid 
						INNER JOIN order_lines ol ON pl.soline = ol.linenum INNER JOIN cust_products cp ON ol.cprod_id = cp.cprod_id
						INNER JOIN mast_products mp ON cp.cprod_mast = mp.mast_id INNER JOIN order_header oh ON ol.orderid = oh.orderid 
						INNER JOIN users client ON oh.userid1 = client.user_id
						WHERE (po_req_etd >=  @from OR @from IS NULL) AND (po_req_etd <= @to OR @to IS NULL)
						AND mp.category1 <> 13 
						AND cp.cprod_user > 0 
						  {countriesDAL.GetCountryCondition(countryFilter)} 
						  {distributorCriteria}
						{(includedClients != null || excludedClients != null ?
							$" AND cp.cprod_user {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")} 
						  AND (cp.brand_user_id = @brand_user_id OR @brand_user_id IS NULL) 
						 AND (client.distributor > 0)
						GROUP BY year({field}), month({field})";
			var data = conn.Query<SalesByMonth>(sql, new
			{
				from = fromDate,
				to = toDate,
				brand_user_id,
				brand_id,
				factory_ids,
				clientIds,
				productIds,				
				includedNonDistributors
			}).ToList();
			foreach(var d in data)
			{
				if(d.year != null && d.month != null)
					d.Month21 = Month21.FromYearMonth(d.year.Value, d.month.Value).Value;
			}
			return data;
		}

		public List<ProductSales> GetSalesForCategory(int brand_category)
		{
			return conn.Query<ProductSales>(
				@"SELECT cust_products.cprod_id, cust_products.cprod_name, 
					SUM(order_lines.orderqty*order_lines.unitprice*web_product_component.qty) AS Amount,
					SUM(order_lines.orderqty*web_product_component.qty) AS numOfUnits
					FROM  cust_products
					INNER JOIN web_product_component ON web_product_component.cprod_id = cust_products.cprod_id
					INNER JOIN web_product_category ON web_product_category.web_unique = web_product_component.web_unique
					INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id
					WHERE  web_product_category.category_id = @brand_category
					GROUP BY cust_products.cprod_id", 
				new {brand_category}).ToList();				
				
		}

		public List<CustomerSalesByMonth> GetCustomerSalesByMonth(int? @from = null, int? to = null, bool UK = true, CountryFilter countryFilter = CountryFilter.UKOnly, int? userId = null,
			bool brands = true, IList<int> includedClients = null, int[] excludedClients = null, int? brand_id = null, DateTime? fromDate = null, DateTime? toDate = null, bool periodLevel = false,
			bool useETA = false, string excludedCustomers = "NK2", bool groupByMonth = false, IList<int> includedNonDistributors = null, bool useCompanyPriceType = false)
		{
			
			UK = countryFilter != CountryFilter.NonUK && countryFilter != CountryFilter.NonUKExcludingAsia && UK;
			string view = brands ? "brand_sales_analysis3" : "brs_sales_analysis2";
			var monthField = useETA ? useCompanyPriceType ? "month23" : "month22" : "month21";
			var priceField = monthField == "month23" && !brands ? "rowprice_gbp_23" : "rowprice_gbp";   //rowprice_gbp_23 uses exchange rate table for gbp->usd, use only for non-brands
			var wherePeriod = @from != null ? String.Format(" {0} BETWEEN @from AND @to ", monthField) : periodLevel ? " linedate BETWEEN @from AND @to" : String.Format("{0} BETWEEN @from AND @to", useETA ? "req_eta" : "po_req_etd");
			var groupFields = "userid1,client_name,oem_flag";
			const string periodField = ",PERIOD_DIFF(@monthyear,EXTRACT(YEAR_MONTH FROM linedate) + (CASE WHEN DAY(linedate) < @day THEN -1 ELSE 0 END))";
			var selectFields = "userid1,client_name,oem_flag";
			if (periodLevel)
			{
				groupFields += periodField;
				selectFields += periodField + " AS period";
			}
			if (groupByMonth)
			{
				groupFields += "," + monthField;
				selectFields += "," + monthField + " AS period";
			}
			var customerCodes = excludedCustomers.Split(',');
			var clientIds = includedClients ?? excludedClients;

			string distributorCriteria = brands ? $@" AND (distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR client_id IN @includedNonDistributors" :  String.Empty)})" : string.Empty ;

			var sql =
					$@"SELECT {selectFields}, SUM({priceField}) as total 
                    FROM {view} 
                    WHERE {wherePeriod} 
					AND customer_code NOT IN @customerCodes 
                    AND category1 <> 13 
                    AND cprod_user > 0 
                    AND (brand_user_id = @userid OR @userid IS NULL) 
                    {countriesDAL.GetCountryCondition(countryFilter)} 
                        {(includedClients != null || excludedClients != null ?
							$" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")} 
                    {(brand_id != null ? " AND brand_id = @brand_id" : "")}
                    {distributorCriteria}  GROUP BY {groupFields}";



			var result = conn.Query<CustomerSalesByMonthRaw>(sql,
				new
				{
					from = (object)from ?? fromDate,
					to = (object)to ?? toDate,
					brand_id, 
					userId, customerCodes, clientIds, includedNonDistributors,
					monthYear = toDate != null ? "20" + company.Common.Utilities.GetMonth21FromDate(toDate.Value) : null,
					day = toDate != null ? (int?) toDate.Value.Day : null
				}).ToList();
			var finalData = new List<CustomerSalesByMonth>();
			foreach(var r in result)
			{
				var d = new CustomerSalesByMonth { isUK = UK, isOEM = r.oem_flag ?? false, Amount = r.total, client_id = r.userid1 ?? 0, customer_name = r.client_name };
				if (periodLevel || groupByMonth)
					d.Month21 = Convert.ToInt32(r.period);
				finalData.Add(d);
			}
			
			
			return finalData;
		}

		public List<Returns> GetRespondedClaims(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null)
		{
			
			var brandsClause = brands ? " AND users.distributor > 0" : "AND users.distributor = 0";
			var clientIds = includedClients ?? excludedClients;
			var sql = $@"SELECT returns.* FROM returns INNER JOIN users ON returns.client_id = users.user_id 
					WHERE status1 = 1 and client_id <> 184 AND cc_response_date IS NOT NULL
						AND (((year(`returns`.`cc_response_date`) - 2000) * 100) + month(`returns`.`cc_response_date`)) BETWEEN @from AND @to 
					{brandsClause}
					{ countriesDAL.GetCountryCondition(countryFilter)}
					{
					(includedClients != null || excludedClients != null ?
					   $" AND client_id {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}";
			return conn.Query<Returns>(sql, new { from, to, clientIds }).ToList();

		}

		public List<OrderProductGroupStats> GetOrderProductGroupStats_New(DateTime from, DateTime to,
							CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null,
							IList<int> includedNonDistributors = null)
		{
			
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			
			//new orders
			
			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;

			var sql = 
					$@"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, 
					COUNT(*) AS numOfLines
					FROM order_lines
					INNER JOIN order_header ON order_header.orderid = order_lines.orderid
					LEFT OUTER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
					LEFT OUTER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
					INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
					INNER JOIN users ON users.user_id = order_header.userid1
					WHERE order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter)} 
					{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
					{brandsClause}
                    AND users.test_account IS NULL
					AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
					AND order_header.orderdate BETWEEN @from AND @to 
					GROUP BY order_header.orderid,users.customer_code, fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) 
					ORDER BY order_header.orderid, product_group";

			//Collect order records to determine SABC type of order
			var data = conn.Query<OrderStatTempRecordRaw>(sql, new { from, to, excludedContainerTypes, clientIds, includedNonDistributors }).ToList();
			var orderList = new List<OrderStatTempRecord>();
			foreach(var d in data)
			{
				int order_id = d.orderid;
				var orderRecord = orderList.FirstOrDefault(o => o.orderid == order_id);
				if (orderRecord == null)
				{
					orderRecord = new OrderStatTempRecord
					{
						orderid = order_id,
						customer_code = d.customer_code,
						productgroup_lines = new Dictionary<string, int>()
					};
					orderList.Add(orderRecord);
				}
				orderRecord.productgroup_lines[string.Empty + d.product_group] = d.numOfLines;
				
			}
			
			DetermineProductGroup(orderList);
			return orderList.GroupBy(o => new { o.customer_code, o.product_group }).
				Select(
					g =>
					new OrderProductGroupStats
					{
						client_code = g.Key.customer_code,
						product_group = g.Key.product_group,
						orders_count = g.Count()
					}).ToList();			
			
		}

		public List<OrderBrandsStats> GetOrderBrandStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			var result = new List<OrderBrandsStats>();

			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;
			var sql =
				$@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
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
						AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
							{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter)}  
					{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
						GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
					GROUP BY orders.customer_code, orders.orderid) AS obsummary
				GROUP BY obsummary.customer_code, obsummary.numOfBrands
				ORDER BY obsummary.customer_code, obsummary.numOfBrands";
			var data = conn.Query<OrderBrandsStatsRaw>(sql, new { from, to, clientIds, includedNonDistributors, excludedContainerTypes }).ToList();
			return data.Select(d => new OrderBrandsStats
			{
				client_code = d.customer_code,
				brandCount = d.numOfBrands ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();						
			
		}

		public List<OrderFactoriesStats> GetOrderFactoryStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;
			var sql = 
				$@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
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
					{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter, "users.")}  
					{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                    AND users.test_account IS NULL
					AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
					GROUP BY
					order_lines.orderid,
					users.customer_code,
					factory.user_id) AS orders
					GROUP BY orders.customer_code, orders.orderid) AS obsummary
					GROUP BY obsummary.customer_code, obsummary.numOfFactories
					ORDER BY obsummary.customer_code, obsummary.numOfFactories";
			var data = conn.Query<OrderFactoriesStatsRaw>(sql, new { from, to, clientIds, includedNonDistributors, excludedContainerTypes }).ToList();
			return data.Select(d => new OrderFactoriesStats
			{
				client_code = d.customer_code,
				factoryCount = d.numOfFactories ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();			
			
		}

		public List<OrderLocationStats> GetOrderLocationStats_New(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{
			
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(int.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;

			var sql =
				$@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
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
					{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter, "users.")}  
					{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                    AND users.test_account IS NULL
					AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
					GROUP BY
					order_lines.orderid,
					users.customer_code,
					factory.consolidated_port) AS orders
					GROUP BY orders.customer_code, orders.consolidated_port
					ORDER BY orders.customer_code, orders.consolidated_port";


			var data = conn.Query<OrderLocationStatsRaw>(sql, new { from, to, clientIds, includedNonDistributors, excludedContainerTypes }).ToList();
			return data.Select(d => new OrderLocationStats
			{
				client_code = d.customer_code,
				location = d.consolidated_port ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();

		}

		public List<OrderProductGroupStats> GetOrderProductGroupStats_ETA(DateTime from, DateTime to,
				CountryFilter countryFilter = CountryFilter.UKOnly,	bool brands = true, int[] includedClients = null, int[] excludedClients = null,
				IList<int> includedNonDistributors = null)
		{
			

			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;

			var sql = $@"SELECT order_header.orderid,users.customer_code,
			fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, 
			order_header.req_eta, COUNT(*) AS numOfLines
			FROM order_lines
			INNER JOIN order_header ON order_header.orderid = order_lines.orderid
			INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
			INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
			INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
				INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
			INNER JOIN users ON users.user_id = order_header.userid1
			WHERE order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter)}  
			{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
			{brandsClause}
            AND users.test_account IS NULL
			AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
			{GetOrdersDateCriteria(OutstandingOrdersMode.TransitInPeriod)}
			GROUP BY order_header.orderid,users.customer_code, 
			fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) 
			ORDER BY order_header.orderid, product_group";

			var data = conn.Query<OrderStatTempRecordRaw>(sql, new { from, to, excludedContainerTypes, clientIds, includedNonDistributors }).ToList();
			var orderList = new List<OrderStatTempRecord>();
			foreach (var d in data)
			{
				int order_id = d.orderid;
				var orderRecord = orderList.FirstOrDefault(o => o.orderid == order_id);
				if (orderRecord == null)
				{
					orderRecord = new OrderStatTempRecord
					{
						orderid = order_id,
						customer_code = d.customer_code,
						productgroup_lines = new Dictionary<string, int>()
					};
					orderList.Add(orderRecord);
				}
				orderRecord.productgroup_lines[string.Empty + d.product_group] = d.numOfLines;

			}

			DetermineProductGroup(orderList);
			return orderList.GroupBy(o => new { o.customer_code, o.product_group }).
				Select(
					g =>
					new OrderProductGroupStats
					{
						client_code = g.Key.customer_code,
						product_group = g.Key.product_group,
						orders_count = g.Count()
					}).ToList();


		}

		public List<OrderBrandsStats> GetOrderBrandStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{

			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;
			var sql =
				$@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
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
							order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter)} 
						{brandsClause}
						{
							(includedClients != null || excludedClients != null ?
							$" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                        AND users.test_account IS NULL
						AND req_eta BETWEEN @from AND @to
						AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
						GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
					GROUP BY orders.customer_code, orders.orderid) AS obsummary
				GROUP BY obsummary.customer_code, obsummary.numOfBrands
				ORDER BY obsummary.customer_code, obsummary.numOfBrands";

			var data = conn.Query<OrderBrandsStatsRaw>(sql, new { from, to, clientIds, includedNonDistributors, excludedContainerTypes }).ToList();
			return data.Select(d => new OrderBrandsStats
			{
				client_code = d.customer_code,
				brandCount = d.numOfBrands ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();

		}

		public List<OrderFactoriesStats> GetOrderFactoryStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{

			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;
			var sql =
				$@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
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
					WHERE order_header.req_eta BETWEEN @from AND @to
					{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter, "users.")}  
					{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                    AND users.test_account IS NULL
					AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
					GROUP BY
					order_lines.orderid,
					users.customer_code,
					factory.user_id) AS orders
					GROUP BY orders.customer_code, orders.orderid) AS obsummary
					GROUP BY obsummary.customer_code, obsummary.numOfFactories
					ORDER BY obsummary.customer_code, obsummary.numOfFactories";
			var data = conn.Query<OrderFactoriesStatsRaw>(sql, new { from, to, clientIds, includedNonDistributors, excludedContainerTypes }).ToList();
			return data.Select(d => new OrderFactoriesStats
			{
				client_code = d.customer_code,
				factoryCount = d.numOfFactories ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();

		}

		public List<OrderLocationStats> GetOrderLocationStats_ETA(DateTime from, DateTime to, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, IList<int> includedNonDistributors = null)
		{

			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(int.Parse).ToArray();

			var brandsClause =
				$@" AND {(brands ? "" : "NOT")} (users.distributor > 0 
                                {(includedNonDistributors != null && includedNonDistributors.Count > 0
					? $" OR users.user_id IN @includedNonDistributors"
					: "")})";
			var clientIds = includedClients ?? excludedClients;

			var sql =
				$@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
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
					WHERE order_header.req_eta BETWEEN @from AND @to
					{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter, "users.")}  
					{
					(includedClients != null || excludedClients != null ?
					   $" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                    AND users.test_account IS NULL
					AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
					GROUP BY
					order_lines.orderid,
					users.customer_code,
					factory.consolidated_port) AS orders
					GROUP BY orders.customer_code, orders.consolidated_port
					ORDER BY orders.customer_code, orders.consolidated_port";


			var data = conn.Query<OrderLocationStatsRaw>(sql, new { from, to, clientIds, includedNonDistributors, excludedContainerTypes }).ToList();
			return data.Select(d => new OrderLocationStats
			{
				client_code = d.customer_code,
				location = d.consolidated_port ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();

		}

		public List<OrderProductGroupStats> GetOrderProductGroupStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
													CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, 
													int[] excludedClients = null, int? daysToShipping = null,
													DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
			if (daysToShipping == null)
				daysToShipping = Properties.Settings.Default.Analytics_Default_DaysToShipping;
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var clientIds = includedClients ?? excludedClients;

			var sql =
					$@"SELECT order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) AS product_group, 
						any_value(order_header.req_eta) as req_eta, COUNT(*) AS numOfLines,sum(order_lines.orderqty * order_lines.unitprice) AS totalGPB
						FROM order_lines
						INNER JOIN order_header ON order_header.orderid = order_lines.orderid
						INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
						INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
						INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
						 INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
						INNER JOIN users ON users.user_id = order_header.userid1
						WHERE order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter)} 
						{
							(includedClients != null || excludedClients != null ?
							$" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
						{brandsClause}
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
						AND order_header.req_eta > @from {GetOrdersDateCriteria(mode)}
						GROUP BY order_header.orderid,users.customer_code,fn_compute_sabc(order_header.orderdate, porder_header.po_req_etd, mast_products.product_group) 
						ORDER BY order_header.orderid, product_group";


			var data = conn.Query<OrderStatTempRecordRaw>(sql, new { from, new_from, new_to, excludedContainerTypes, clientIds,daysToShipping, 
				etaFrom = etaCriteriaFrom, etaTo = etaCriteriaTo  }).ToList();
			var orderList = new List<OrderStatTempRecord>();
			foreach (var d in data)
			{
				int order_id = d.orderid;
				var orderRecord = orderList.FirstOrDefault(o => o.orderid == order_id);
				if (orderRecord == null)
				{
					orderRecord = new OrderStatTempRecord
					{
						orderid = order_id,
						customer_code = d.customer_code,
						productgroup_lines = new Dictionary<string, int>(),
						totalGPB = d.totalGPB ?? 0.0
					};
					orderList.Add(orderRecord);
				}
				orderRecord.productgroup_lines[d.product_group] = d.numOfLines;

			}

			DetermineProductGroup(orderList);
			return orderList.GroupBy(o => new { o.customer_code, o.product_group }).
				Select(
					g =>
					new OrderProductGroupStats
					{
						client_code = g.Key.customer_code,
						product_group = g.Key.product_group,
						orders_count = g.Count(),
						totalGPB = g.Sum(t => t.totalGPB)
					}).ToList();


		}

		public List<OrderBrandsStats> GetOrderBrandStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
			CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, 
			DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
			if (daysToShipping == null)
				daysToShipping = Properties.Settings.Default.Analytics_Default_DaysToShipping;
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var clientIds = includedClients ?? excludedClients;


			var sql =
					$@"SELECT obsummary.customer_code,obsummary.numOfBrands, COUNT(*) AS numOfOrders FROM
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
							order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter)}  
							{
								(includedClients != null || excludedClients != null ?
								$" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
							{brandsClause}
                            AND users.test_account IS NULL
							AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
							AND req_eta >= @from {GetOrdersDateCriteria(mode)}
							GROUP BY order_lines.orderid, users.customer_code, brands.brand_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
					GROUP BY obsummary.customer_code, obsummary.numOfBrands
					ORDER BY obsummary.customer_code, obsummary.numOfBrands";

			var data = conn.Query<OrderBrandsStatsRaw>(sql, new {
				from,
				new_from,
				new_to,
				excludedContainerTypes,
				clientIds,
				daysToShipping,
				etaFrom = etaCriteriaFrom,
				etaTo = etaCriteriaTo
			}).ToList();
			return data.Select(d => new OrderBrandsStats
			{
				client_code = d.customer_code,
				brandCount = d.numOfBrands ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();

		}

		public  List<OrderFactoriesStats> GetOrderFactoryStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
			CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
			if (daysToShipping == null)
				daysToShipping = Properties.Settings.Default.Analytics_Default_DaysToShipping;
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var clientIds = includedClients ?? excludedClients;

			var sql = $@"SELECT obsummary.customer_code,obsummary.numOfFactories, COUNT(*) AS numOfOrders FROM
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
						WHERE req_eta >= @from {GetOrdersDateCriteria(mode)}
						{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter, "users.")}  
						{
							(includedClients != null || excludedClients != null ?
							$" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.user_id) AS orders
						GROUP BY orders.customer_code, orders.orderid) AS obsummary
						GROUP BY obsummary.customer_code, obsummary.numOfFactories
						ORDER BY obsummary.customer_code, obsummary.numOfFactories";
			var data = conn.Query<OrderFactoriesStatsRaw>(sql, new {
				from,
				new_from,
				new_to,
				excludedContainerTypes,
				clientIds,
				daysToShipping,
				etaFrom = etaCriteriaFrom,
				etaTo = etaCriteriaTo
			}).ToList();
			return data.Select(d => new OrderFactoriesStats
			{
				client_code = d.customer_code,
				factoryCount = d.numOfFactories ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();
			
		}

		public List<OrderLocationStats> GetOrderLocationStats_Out(DateTime from, DateTime new_from, DateTime new_to, OutstandingOrdersMode mode = OutstandingOrdersMode.Both,
			CountryFilter countryFilter = CountryFilter.UKOnly, bool brands = true, int[] includedClients = null, int[] excludedClients = null, int? daysToShipping = null, DateTime? etaCriteriaFrom = null, DateTime? etaCriteriaTo = null)
		{
			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
			if (daysToShipping == null)
				daysToShipping = Properties.Settings.Default.Analytics_Default_DaysToShipping;
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var clientIds = includedClients ?? excludedClients;

			var sql = $@"SELECT orders.customer_code,orders.consolidated_port, COUNT(*) AS numOfOrders FROM
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
						WHERE req_eta >= @from {GetOrdersDateCriteria(mode)}
						{brandsClause} AND  order_header.`status` NOT IN ('X','Y') {countriesDAL.GetCountryCondition(countryFilter, "users.")} 
						{
							(includedClients != null || excludedClients != null ?
							$" AND userid1 {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                        AND users.test_account IS NULL
						AND COALESCE(order_header.container_type,0) NOT IN @excludedContainerTypes and mast_products.category1 <> 13
						GROUP BY
						order_lines.orderid,
						users.customer_code,
						factory.consolidated_port) AS orders
						GROUP BY orders.customer_code, orders.consolidated_port
						ORDER BY orders.customer_code, orders.consolidated_port";
			var data = conn.Query<OrderLocationStatsRaw>(sql, new {
				from,
				new_from,
				new_to,
				excludedContainerTypes,
				clientIds,
				daysToShipping,
				etaFrom = etaCriteriaFrom,
				etaTo = etaCriteriaTo
			}).ToList();
			return data.Select(d => new OrderLocationStats
			{
				client_code = d.customer_code,
				location = d.consolidated_port ?? 0,
				orderCount = d.numOfOrders ?? 0
			}).ToList();
			
		}

		public List<BrandSalesByMonthEx> GetBrandSalesByMonth(int? from = null, int? to = null, CountryFilter countryFilter = CountryFilter.UKOnly,
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, BrandSalesGroupOption groupOption = BrandSalesGroupOption.Default,
			DateTime? fromDate = null, DateTime? toDate = null, bool productLevel = false, bool customerLevel = false, string customerCode = "", int? brand_id = null,
			bool useETA = false, bool includePendingForDiscontinuation = true, bool usePeriod = false, string excludedCustomers = "NK2", bool groupByPeriod = false, IList<int> includedNonDistributors = null, bool useCompanyPriceType = false)
		{
			var result = new List<BrandSalesByMonthEx>();
			var monthField = useETA ? useCompanyPriceType ? "month23" : "month22" : "month21";
			string view = brands ? "brand_sales_analysis3" : "brs_sales_analysis2";
			var groupField = groupOption == BrandSalesGroupOption.ByBrand || brands ? "brand_id,brandname" : "cat1_name";
			var selectField = groupOption == BrandSalesGroupOption.ByBrand || brands ? "brand_id,brandname" : "cat1_name as brandname";
			const string periodField = ",PERIOD_DIFF(@monthyear,EXTRACT(YEAR_MONTH FROM linedate) + (CASE WHEN DAY(linedate) < @day THEN -1 ELSE 0 END))";
			groupField += @from != null || groupByPeriod ? "," + monthField : usePeriod ? periodField : "";
			if (productLevel)
				groupField += ",cprod_code1,cprod_name,analytics_category";
			if (customerLevel)
				groupField += ",userid1,client_name,customer_code";
			selectField += !usePeriod ? "," + monthField + " AS month21" : periodField + " AS month21";
			if (productLevel)
				selectField += ",cprod_code1 as cprod_code,cprod_name,analytics_category as AnalyticsSubCategoryId,cprod_stock,cprod_stock_date as stock_date, product_group";
			if (customerLevel)
				selectField += ",userid1 as customer_id,client_name,customer_code, dist_stock, dist_stock_date";
			var wherePeriod = @from != null ? String.Format("({0} >= @from OR @from IS NULL) AND ({0} <= @to OR @to IS NULL)", monthField) : String.Format(" {0} BETWEEN @from AND @to", usePeriod ? "linedate" : useETA ? "req_eta" : "po_req_etd");

			var brandsClause = brands ? "AND users.distributor > 0" : "AND users.distributor = 0";
			var excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var clientIds = includedClients ?? excludedClients;

			
			var wherePendingDiscontinuation = includePendingForDiscontinuation
				? ""
				: " AND COALESCE(pending_discontinuation,0) = 0";
			
				string distributorCriteria = brands ? $@" AND (distributor > 0 
                                    {(includedNonDistributors != null && includedNonDistributors.Count > 0 ? $" OR client_id IN @clientIds" : "")})" :
							String.Empty;
				var sql = 
						$@"SELECT {selectField}, SUM(rowprice_gbp) as amount,SUM(orderqty) AS qty, COUNT(DISTINCT orderid) AS numOfOrders 
                        FROM {view} 
                        WHERE {wherePeriod} 
						AND customer_code NOT IN  @excludedCustomers 
                        AND category1 <> 13 
                        AND cprod_user > 0
                        {countriesDAL.GetCountryCondition(countryFilter)}
                        {
							(includedClients != null || excludedClients != null ?
							$" AND cprod_user {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
                        {(!string.IsNullOrEmpty(customerCode) ? " AND customer_code = @customer_code" : "")}
                        {(brand_id != null ? " AND brand_id = @brand_id" : "")}
                        {wherePendingDiscontinuation} 
                        {distributorCriteria}
                        GROUP BY {groupField} ";
			return conn.Query<BrandSalesByMonthEx>(sql,
				new
				{
					 from = from ?? (object) fromDate, monthyear = "20" + company.Common.Utilities.GetMonth21FromDate(toDate.Value),
					day = toDate.Value.Day, 
					to = to ?? (object) toDate, 
					customer_code = customerCode, 
					brand_id, 
					excludedCustomers = excludedCustomers.Split(','),
					clientIds
				}).ToList();				
			
		}

		public List<StockSummary> GetStockSummaryReports2(IList<int> includedClients, double qcCharge, double duty, double freight, DateTime? from = null)
		{
			var result = new List<StockSummary>();
			var date = from ?? DateTime.Today;
			//var clientIds = includedClients.Select(id => (int?)id).ToList();
			var factory_codes = GetFactoriesOnStockOrders(includedClients, date);
			var unallocated = GetUnallocatedAtFactory(includedClients);
			result.AddRange(unallocated.Where(o => factory_codes.Contains(o.FactoryCode))
				.Select(o => new StockSummary
				{ 
					factory_id = o.FactoryId, 
					factory_code = o.FactoryCode, 
					UnallocatedAtFactoryValue = o.Value }));

			var allocated = GetAllocatedAtFactory(includedClients, date);
			//merge
			foreach (var o in allocated.Where(o => factory_codes.Contains(o.FactoryCode)))
			{
				var stockRecord = result.FirstOrDefault(s => s.factory_id == o.FactoryId);
				if (stockRecord != null)
					stockRecord.AllocatedAtFactory = o.Value;
				else
					result.Add(new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, AllocatedAtFactory = o.Value });
			}

			//onWater
			var onWater = GetOnWater(includedClients, date);

			//merge
			foreach (var o in onWater.Where(o => factory_codes.Contains(o.FactoryCode)))
			{
				var stockRecord = result.FirstOrDefault(s => s.factory_id == o.FactoryId);
				if (stockRecord != null)
					stockRecord.OnWater = o.Value;
				else
					result.Add(new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, OnWater = o.Value });
			}

			//stock uk
			var warehouseSummary = GetProductStock(includedClients);
			foreach (var w in warehouseSummary.Where(o => factory_codes.Contains(o.FactoryCode)))
			{
				var stockRecord = result.FirstOrDefault(s => s.factory_id == w.FactoryId);
				if (stockRecord != null)
					stockRecord.Warehouse = w.Value;
				else
				{
					result.Add(new StockSummary { factory_id = w.FactoryId, factory_code = w.FactoryCode, Warehouse = w.Value });
				}
			}

			return result;
		}

		public List<StockFactoryRow> GetUnallocatedAtFactory(IList<int> clientIds)
		{
			
			var sql = $@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factory_id factoryId, 
					fn_combinedfactorycode(factory_code,factory.combined_factory) AS factoryCode, 
					SUM(order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum)) AS Qty, 
                    SUM(fn_unitprice_gbp(porder_lines.unitprice, porder_lines.unitcurrency) * (order_lines.orderqty - (SELECT SUM(alloc_qty) 
						FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum))) AS Value
					FROM order_lines
					INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
					INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
					INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                    INNER JOIN users factory ON factory.user_id = porder_header.userid
					WHERE order_header.stock_order = 1 AND order_header.userid1 IN @clientIds AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0 AND
                    (order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum) > 0)
                    GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)";
			return conn.Query<StockFactoryRow>(sql, new { clientIds }).ToList();
		}


		public List<StockFactoryRow> GetAllocatedAtFactory(IList<int> clientIds, DateTime date)
		{
			
			var sql = $@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factoryid, 
					fn_combinedfactorycode(factory_code,factory.combined_factory) AS factorycode, SUM(order_lines.orderqty) AS Qty, 
						SUM(fn_unitprice_gbp(porder_lines.unitprice, porder_lines.unitcurrency) * order_lines.orderqty) AS Value
						FROM order_lines
						INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
						INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
						INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        INNER JOIN users factory ON factory.user_id = porder_header.userid
						WHERE order_header.stock_order = 8 AND order_header.userid1 IN @clientIds AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                        AND porder_header.po_req_etd > @date
                        GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)
						";
			return conn.Query<StockFactoryRow>(sql, new { clientIds, date }).ToList();
		}

		public List<StockFactoryRow> GetOnWater(IList<int> clientIds, DateTime date)
		{
			
			var sql = $@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factoryid, fn_combinedfactorycode(factory_code,factory.combined_factory) AS factorycode,
								SUM(order_lines.orderqty) AS Qty, SUM(fn_unitprice_gbp(porder_lines.unitprice, porder_lines.unitcurrency) * order_lines.orderqty) AS Value
								FROM order_lines
								INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
								INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
								INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                INNER JOIN users factory ON factory.user_id = porder_header.userid
								WHERE order_header.stock_order <> 1 AND order_header.userid1 IN @clientIds AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                                AND porder_header.po_req_etd < @date AND order_header.req_eta > @date
                                GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)
								";
			return conn.Query<StockFactoryRow>(sql, new { clientIds, date }).ToList();
		}

		public List<StockFactoryRow> GetProductStock(IList<int> clientIds)
		{
			var sql = $@"SELECT fn_combinedfactoryid(factory.user_id, factory.combined_factory) AS factoryid, fn_combinedfactorycode(factory_code,factory.combined_factory) AS factorycode, 
						cprod_stock AS Qty, SUM(cprod_stock * (CASE WHEN mast_products.price_pound > 0 THEN mast_products.price_pound ELSE mast_products.price_dollar / 1.6 END)) AS Value
                        FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                        WHERE cprod_user IN @clientIds AND EXISTS (SELECT * FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid WHERE stock_order = 1 
						AND order_lines.cprod_id = cust_products.cprod_id)
                        GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory), fn_combinedfactorycode(factory_code,factory.combined_factory)";
			return conn.Query<StockFactoryRow>(sql, new { clientIds }).ToList();
		}

		public List<string> GetFactoriesOnStockOrders(IList<int> clientIds, DateTime from)
		{
			var sql = $@"SELECT fn_combinedfactorycode(factory_code,factory.combined_factory) as factory_code
                        FROM order_lines
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
						INNER JOIN users factory ON factory.user_id = porder_header.userid
                        WHERE order_header.stock_order = 1 AND order_header.userid1 IN @clientIds AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                        AND porder_header.po_req_etd >= @from
                        GROUP BY fn_combinedfactoryid(factory.user_id, factory.combined_factory)";
			return conn.Query<string>(sql, new { clientIds, from }).ToList();

		}

		public List<int> GetFactoriesOnStockOrders_NoCombined(IList<int> clientIds, DateTime from)
		{
			var sql = $@"SELECT factory.user_id AS factory_id
                        FROM order_lines
                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                        INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
						INNER JOIN users factory ON factory.user_id = porder_header.userid
                        WHERE order_header.stock_order = 1 AND order_header.userid1 IN ({0}) AND order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                        AND porder_header.po_req_etd >= @from
                        GROUP BY factory.user_id";
			return conn.Query<int>(sql, new { clientIds, from }).ToList();
		}

		public List<ProductSales> GetTopNByBrands(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly)
		{

			var sql = $@"SELECT brand_user_id,cprod_code1 as cprod_code,cprod_name,brand_code, sum(sogbp*orderqty) as amount, sum(orderqty) as numOfUnits, 
						COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
						   AND NOT EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 AND cust_products.brand_user_id <> brand_sales_analysis_product2.brand_user_id )
						   AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
							{ExcludedCprodBrandCatsCondition}
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0  {countriesDAL.GetCountryCondition(countryFilter)} 
							GROUP BY brand_user_id,brand_code,cprod_code1,cprod_name
							ORDER BY numOfProducts DESC";
			return conn.Query<ProductSales>(sql, new { from, to }).ToList();
		}

		public List<ProductSales> GetTopNUniversal(int n, int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly)
		{
			var sql = $@"SELECT cprod_code1 as cprod_code,cprod_name,brand_code, sum(sogbp*orderqty) as amount, sum(orderqty) as numOfUnits, 
						COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
							AND EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id 
										WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 
										AND cust_products.brand_user_id <> brand_sales_analysis_product2.brand_user_id )
						   AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
							{ExcludedCprodBrandCatsCondition}
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {countriesDAL.GetCountryCondition(countryFilter)} 
							GROUP BY cprod_code1,cprod_name
							ORDER BY numOfProducts DESC LIMIT @n
							";
			return conn.Query<ProductSales>(sql, new { from, to,  n }).ToList();

		}

		public List<ProductSales> GetTopForBrandCat(int n, int from, int to, int cprod_brand_cat, CountryFilter countryFilter = CountryFilter.UKOnly)
		{
			var sql = $@"SELECT cprod_code1 as cprod_code,cprod_name,brand_code, sum(sogbp*orderqty) as amount, sum(orderqty) as numOfUnits,
						COALESCE(sum(sogp*orderqty)/sum(sogbp*orderqty),0) as margin
						FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to 
							AND EXISTS (SELECT cprod_code1 FROM cust_products INNER JOIN brands ON cust_products.cprod_user = brands.user_id WHERE brands.eb_brand = 1 AND cust_products.cprod_status <> 'D' AND brand_sales_analysis_product2.cprod_code1 = cust_products.cprod_code1 GROUP BY cprod_code1, brand_user_id HAVING COUNT(DISTINCT brand_user_id) = 1)
						   AND cprod_code1 not like 'SP%' and distributor > 0 AND rowprice > 0
							AND cprod_brand_cat = @cprod_brand_cat
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0  {countriesDAL.GetCountryCondition(countryFilter)} 
							GROUP BY brand_user_id,brand_code,cprod_code1,cprod_name
							ORDER BY numOfProducts DESC LIMIT @n";
			return conn.Query<ProductSales>(sql, new { from, to,n, cprod_brand_cat }).ToList();
		}

		public List<Cust_products> GetNonSelling(int from, int to, int? brand_user_id = null)
		{
			var sql = $@"SELECT * FROM cust_products INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id WHERE category1 <> 13 
						AND (brand_user_id = @userid OR @userid IS NULL) 
						  AND cprod_status <> 'D' {ExcludedCprodBrandCatsCondition}
						  AND cprod_id NOT IN (SELECT DISTINCT cprod_id FROM brand_sales_analysis_product2 WHERE month21 BETWEEN @from AND @to ) ";
			return conn.Query<Cust_products>(sql, new { from, to, userid = brand_user_id }).ToList();
				
		}

		public List<AnalyticsCategorySummaryRow> GetAnalyticsCategorySummary(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, 
			bool brands = true, int[] includedClients = null, int[] excludedClients = null, string excludedCustomers = "NK2")
		{
			
			var clientIds = includedClients ?? excludedClients;
			var sql = $@"SELECT analytics_category as analytics_category_id,analytics_option as analytics_option_id, 
							SUM(orderqty) as orderqty FROM brand_sales_analysis2  
							WHERE month21 BETWEEN @from AND @to AND analytics_category IS NOT NULL
							 AND category1 <> 13 AND cprod_user > 0 {countriesDAL.GetCountryCondition(countryFilter)} 
							{
							(includedClients != null || excludedClients != null ?
							$" AND cprod_user {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}
							AND customer_code NOT IN @excludedCustomers GROUP BY analytics_category,analytics_option ";
			return conn.Query<AnalyticsCategorySummaryRow>(sql, new { from, to, clientIds, excludedCustomers = excludedCustomers.Split(',') }).ToList();
				
		}

		public List<ProductDisplayCount> GetDisplayCountForProducts(int brand_category)
		{
			var sql = $@"SELECT cust_products.cprod_id, cust_products.cprod_name, COUNT(*) AS DisplayCount
					FROM  cust_products
					INNER JOIN web_product_component ON web_product_component.cprod_id = cust_products.cprod_id
					INNER JOIN web_product_category ON web_product_category.web_unique = web_product_component.web_unique
					INNER JOIN dealer_image_displays ON dealer_image_displays.web_unique = web_product_component.web_unique
					WHERE  web_product_category.category_id = @brand_category
					GROUP BY cust_products.cprod_id";
			return conn.Query<ProductDisplayCount>(sql, new { brand_category }).ToList();
		}

		public List<ProductDistributorDisplayCount> GetDisplayCountForProductsAndBrand(int? brand_user_id, IList<int> category_ids)
		{
			var sql = $@"SELECT cust_products.cprod_id, cust_products.cprod_name,cust_products.cprod_code1, dist.user_id, 
						dist.customer_code as distributor_code, COUNT(*) AS DisplayCount
						FROM  cust_products
						INNER JOIN web_product_component ON web_product_component.cprod_id = cust_products.cprod_id
						INNER JOIN web_product_new ON web_product_component.web_unique = web_product_new.web_unique
						INNER JOIN dealer_image_displays ON dealer_image_displays.web_unique = web_product_new.web_unique
						INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique 
						INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
						INNER JOIN users dist ON dealers.user_type = dist.user_id
						INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
						LEFT OUTER JOIN analytics_subcategory ON cust_products.analytics_category = analytics_subcategory.subcat_id
                        {(brand_user_id != null ? "" : "INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id")}
						WHERE {(brand_user_id != null ?
								"cust_products.brand_user_id = @brand_user_id" :
								"analytics_categories.category_type IS NULL")} AND mast_products.category1 <> 13
						{(category_ids != null && category_ids.Count > 0 ?
							"AND analytics_subcategory.category_id IN @category_ids" : "")}
						GROUP BY cust_products.cprod_id,dist.user_id";


			return conn.Query<ProductDistributorDisplayCount>(sql,
				new { brand_user_id, category_ids }).ToList();
					
		}

		public List<ProductLocationStats> GetProductLocationStats(string product_group, int? ageForExclusion = null, IList<string> distCountries = null)
		{
			var sql = $@"SELECT cust_products.cprod_code1 as cprod_code, cust_products.cprod_name,brands.brandname,users.consolidated_port as location,
						(SELECT GROUP_CONCAT(other_mp.product_group) FROM  cust_products other_cp
						INNER JOIN mast_products other_mp ON other_mp.mast_id = other_cp.cprod_mast
						INNER JOIN users other_fact ON other_fact.user_id = other_mp.factory_id
						INNER JOIN brands other_b ON other_cp.cprod_user = other_b.user_id AND other_b.eb_brand = 1
						WHERE cust_products.cprod_code1 = other_cp.cprod_code1 AND other_cp.cprod_status <> 'D' AND other_mp.category1 <> 13 AND other_cp.cprod_brand_cat NOT IN ({2}) AND COALESCE(other_cp.report_exception,0) = 0
						AND cust_products.cprod_id <> other_cp.cprod_id AND other_fact.consolidated_port <> users.consolidated_port AND LEFT(other_mp.product_group,1) <> @group) AS sproductgroup_others
						FROM  cust_products
						INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
						INNER JOIN users ON users.user_id = mast_products.factory_id
						INNER JOIN brands ON  cust_products.brand_user_id = brands.user_id AND brands.eb_brand = 1
						WHERE cust_products.cprod_status <> 'D' AND mast_products.category1 <> 13 AND cprod_brand_cat NOT IN ({ExcludedCprodBrandCats})) 
                        AND COALESCE(cust_products.report_exception,0) = 0
                        AND EXISTS(SELECT client_id FROM dist_products WHERE dist_cprod_id = cust_products.cprod_id AND client_id = 85)
						{(ageForExclusion != null ? GetETDExclusionCriteria(ageForExclusion.Value) : "")} 
						{(distCountries != null ? GetDistributorExclusionCriteria(distCountries) : "")}
						AND LEFT(mast_products.product_group,1) = @product_group";
			var data = conn.Query<ProductLocationStats>(sql, new { product_group, countries = distCountries }).ToList();
			foreach(var d in data)
			{
				d.productgroup_others = d.sproductgroup_others.Split(',');
			}
			return data;
		}

		public List<FactorySalesByMonth> GetSalesOfOrdersByMonthForFactories(int from, int to)
		{
			var dateFrom = new Month21(from).Date;
			var dateTo = (new Month21(to) + 1).Date;
			var sql = $@"SELECT mp.factory_id, count(DISTINCT ol.orderid) as numOfOrders,SUM(ol.orderqty * ol.unitprice * IF(ol.unitcurrency = 0, 0.625 , 1)) as Amount
				FROM porder_header po INNER JOIN porder_lines pl ON po.porderid = pl.porderid 
				INNER JOIN order_lines ol ON pl.soline = ol.linenum INNER JOIN cust_products cp ON ol.cprod_id = cp.cprod_id
				INNER JOIN mast_products mp ON cp.cprod_mast = mp.mast_id
				WHERE cp.cprod_user > 0 and mp.category1 <> 13 AND po.po_req_etd BETWEEN @dateFrom AND @dateTo
				group by mp.factory_id order by Amount DESC LIMIT 10";
			var data = conn.Query<FactorySalesByMonth>(sql, new { dateFrom, dateTo }).ToList();
			var factoryIds = data.Select(d => d.factory_id).ToList();
			var factories = companyDAL.GetByIds(factoryIds);
			foreach(var d in data)
			{
				d.factoryCode = factories.FirstOrDefault(f => f.user_id == d.factory_id)?.factory_code;
			}
			return data;
		}

		public List<SalesByMonth> GetNumOfOrdersByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, bool? brands = true,
			int[] includedClients = null, int[] excludedClients = null, int[] excludedContainerTypes = null, int[] includedContainerTypes = null,
			IList<int> factoryIds = null, int? brand_id = null, bool useOrderDate = false, string excludedCustomersString = "")
		{

			if (excludedContainerTypes == null && includedContainerTypes == null)
				excludedContainerTypes = Properties.Settings.Default.DefaultExcludedContainers.Split(',').Select(Int32.Parse).ToArray();
			var brandsClause = brands != null ? brands.Value ? "AND users.distributor <> 0" : "AND users.distributor = 0" : "";
			var clientIds = includedClients ?? excludedClients;

			var sql = $@"SELECT month21, COUNT(orderid) AS numOfOrders FROM 
				(
				SELECT
				order_header.orderid, 
				{(useOrderDate ? "fn_Month21(order_header.orderdate)" : "fn_Month21(MAX(`porder_header`.`po_req_etd`))")} AS month21
				FROM
				order_header
				INNER JOIN porder_header ON porder_header.soorderid = order_header.orderid 
				INNER JOIN users ON order_header.userid1 = users.user_id
				WHERE order_header.`status` NOT IN ('X','Y') AND custpo NOT LIKE 'IR%' 
				{(!string.IsNullOrEmpty(excludedCustomersString) ? " AND users.customer_code NOT IN @excludedCustomerCodes" : "")}
				AND COALESCE(order_header.container_type,0) {(excludedContainerTypes != null ? "NOT" : "")} IN @containerTypes 
				AND COALESCE(order_header.combined_order,0) <= 0 
				{countriesDAL.GetCountryCondition(countryFilter)} {brandsClause} 
				AND EXISTS (SELECT order_lines.linenum 
					FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id AND mast_products.category1 <> 13
				{(factoryIds != null ? $" AND mast_products.factory_id IN @factoryIds": "")}
					WHERE order_lines.orderid = order_header.orderid 
				{(includedClients != null || excludedClients != null ?
					$" AND cprod_user {(excludedClients != null ? " NOT " : "")} IN @clientIds" : "")}				
				{(brand_id != null ? " AND cust_products.brand_id = @brand_id" : "")})
				GROUP BY
				order_header.orderid ORDER BY month21, orderid) AS orders
				WHERE month21 BETWEEN @from AND @to 
				GROUP BY month21";
			return conn.Query<SalesByMonth>(sql, new
			{
				factoryIds,
				clientIds,
				containerTypes = includedContainerTypes ?? excludedContainerTypes,
				brand_id,
				from,
				to,
				excludedCustomerCodes = excludedCustomersString.Split(',')
			}).ToList();
			
		}

		public List<SalesByMonth> GetNumOfOrdersByMonthForFactories(int from, int to, IList<int> factoryIds = null)
		{
			var sql = $@"SELECT count(DISTINCT orderid) as numOfOrders, month21 FROM factory_sales_analysis 
					WHERE cprod_user > 0 {(factoryIds != null ? " AND factory_id IN @factoryIds" : "")} 
					and category1 <> 13 AND month21 BETWEEN @from AND @to
					group by month21 order by month21 ASC";
			return conn.Query<SalesByMonth>(sql, new { from, to, factoryIds }).ToList();
				
		}

		public List<SalesByMonth> GetProfitByMonth(int from, int to)
		{
			var sql = @"SELECT  month21, SUM(totalgp) as amount FROM brand_sales_analysis_gp WHERE month21 BETWEEN @from AND @to 
						AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 GROUP BY month21 ";
			return conn.Query<SalesByMonth>(sql, new { from, to }).ToList();
		}

		public List<Category1> GetSubCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var sql = $@"SELECT DISTINCT COALESCE(brand_sub_id,0) AS category1_id,COALESCE(brand_sub_desc,'') AS cat1_name 
						FROM brand_sales_analysis2 LEFT OUTER JOIN brand_categories_sub 
						ON brand_sales_analysis2.cprod_brand_subcat = brand_categories_sub.brand_sub_id
						WHERE month21 BETWEEN @from AND @to 
						AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 
						AND (brand_user_id = @userid OR @userid IS NULL) {countriesDAL.GetCountryCondition(countryFilter)}  ";
			return conn.Query<Category1>(sql, new { from, to, userid }).ToList();
		}

		public List<Category1> GetCategoriesFromOrders(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var sql = $@"SELECT DISTINCT category1 as category1_id,cat1_name FROM brand_sales_analysis2 WHERE month21 BETWEEN @from AND @to 
						AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 
						AND (brand_user_id = @userid OR @userid IS NULL)  {countriesDAL.GetCountryCondition(countryFilter)} 
						GROUP BY brandname,month21 ";

			return conn.Query<Category1>(sql, new { from, to, userid }).ToList();

		}

		public List<Category1SalesByMonth> GetSubCategorySalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var sql = $@"SELECT COALESCE(brand_sub_id,0) AS category_id,COALESCE(brand_sub_desc,'') AS catname, 
					SUM(rowprice_gbp) as amount
					FROM brand_sales_analysis2 LEFT OUTER JOIN brand_categories_sub 
					ON brand_sales_analysis2.cprod_brand_subcat = brand_categories_sub.brand_sub_id  WHERE month21 BETWEEN @from AND @to 
					AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {countriesDAL.GetCountryCondition(countryFilter)} 
					AND (brand_user_id = @userid OR @userid IS NULL) GROUP BY COALESCE(brand_sub_id,0),COALESCE(brand_sub_desc,'') ";
			return conn.Query<Category1SalesByMonth>(sql, new { from, to, userid }).ToList();
		}

		public List<Category1SalesByMonth> GetCategory1SalesByMonth(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, int? userid = null)
		{
			var sql = $@"SELECT category1 as category_id,cat1_name as catname, SUM(rowprice_gbp) as amount
							FROM brand_sales_analysis2 WHERE month21 BETWEEN @from AND @to 
							AND customer_code <> 'NK2' AND category1 <> 13 AND cprod_user > 0 {countriesDAL.GetCountryCondition(countryFilter)} 
						AND (brand_user_id = @userid OR @userid IS NULL) GROUP BY category1,cat1_name ";
			return conn.Query<Category1SalesByMonth>(sql, new { from, to, userid }).ToList();
		}

		public List<CountrySales> GetCountrySales(int from, int to, CountryFilter countryFilter = CountryFilter.UKOnly, IList<int> factoryIds = null)
		{
			var sql = $@"SELECT user_country,countries.CountryName as country_name, sum(po_rowprice_gbp) as amount
						FROM factory_sales_analysis INNER JOIN countries ON factory_sales_analysis.user_country = countries.ISO2 WHERE month21 BETWEEN @from AND @to AND customer_code <> 'te' AND customer_code <> ''
						   AND rowprice > 0 AND category1 <> 13 {countriesDAL.GetCountryCondition(countryFilter)} 
						{(factoryIds != null ? " AND factory_id IN @factoryIds" : "")} 
						GROUP BY user_country,countries.CountryName";
			return conn.Query<CountrySales>(sql, new { from, to, factoryIds }).ToList();
		}

		

		private void DetermineProductGroup(List<OrderStatTempRecord> orderList)
		{
			foreach (var orderStatTempRecord in orderList)
			{
				orderStatTempRecord.product_group = orderStatTempRecord.productgroup_lines.Where(kv => kv.Value > 0)
									   .OrderByDescending(kv => kv.Key == "S" ? "0" : kv.Key)
									   .Take(1).First().Key;
			}
		}

		public List<Company> GetFactoriesFromSales(int? month21From = null, int? month21To = null)
		{
			var sql = $@"SELECT DISTINCT factory_code, user_name 
						FROM brand_sales_analysis2 WHERE (month21 >= @month21from OR @month21from IS NULL) 
						AND (month21 <= @month21to OR @month21to IS NULL) and category1 <> 13 ";
			return conn.Query<Company>(sql, new { month21From, month21To }).ToList();
		}

		private string GetOrdersDateCriteria(OutstandingOrdersMode mode)
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

		private string GetETDExclusionCriteria(int months)
		{
			return
				$@" AND DATE_ADD((SELECT MAX(porder_header.po_req_etd) FROM order_lines 
					INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
						INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
					WHERE order_lines.cprod_id = cust_products.cprod_id), INTERVAL {months} MONTH) > CURDATE()
						";
		}

		private string GetDistributorExclusionCriteria(IList<string> countries)
		{
			return
				$@" AND EXISTS (SELECT distprod_id FROM dist_products 
					INNER JOIN users ON dist_products.client_id = users.user_id WHERE dist_products.dist_cprod_id = cust_products.cprod_id AND
									users.user_country IN @countries)";
		}

		public List<Order_line_Summary> GetBrandSummary(DateTime? from, DateTime? to, bool brands_only = false)
		{
			var brandIds = brands_only ? brandsDal.GetAll().Select(b=>b.user_id).ToList() : null ;
			return conn.Query<Order_line_Summary>(
				$@"SELECT cust_products.brand_user_id, SUM(l.orderqty * l.unitprice * IF(l.unitcurrency = 0, 0.625 , 1)) total,
					SUM(l.orderqty) sum_order_qty FROM cust_products INNER JOIN order_lines l ON cust_products.cprod_id = l.cprod_id
					INNER JOIN order_header ON l.orderid = order_header.orderid INNER JOIN users client ON order_header.userid1 = client.user_id
					WHERE client.hide_1 = 0 AND client.distributor> 0 AND (order_header.req_eta >= @from OR @from IS NULL) AND 
					(order_header.req_eta <@to OR @to IS NULL) {(brandIds != null ? " AND cust_products.brand_user_id IN @brandIds" : "" )}
					GROUP BY brand_user_id",
				new { from, to, brandIds }
				).ToList();
		}
	}

	public class CustomerSalesByMonthRaw
	{
		public bool? oem_flag { get; set;  }
		public double? total { get; set; }
		public int? userid1 { get; set; }
		public string client_name { get; set;  }
		public int? period { get; set; }
	}

	public class OrderStatTempRecordRaw
	{
		public int orderid { get; set; }
		public string customer_code { get; set; }
		
		public string product_group { get; set; }
		public int numOfLines { get; set;  }
		public double? totalGPB { get; set; }
	}

	public class OrderBrandsStatsRaw
	{
		public string customer_code { get; set; }
		public int? numOfBrands { get; set; }
		public int? numOfOrders { get; set; }
	}

	public class OrderFactoriesStatsRaw : OrderBrandsStatsRaw
	{
		public int? numOfFactories { get; set; }
	}

	public class OrderLocationStatsRaw: OrderBrandsStatsRaw
	{
		public int? consolidated_port { get; set; }
	}
}
