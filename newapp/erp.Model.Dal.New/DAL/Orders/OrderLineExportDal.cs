
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using System.Data;

namespace erp.Model.Dal.New
{
	public class OrderLineExportDal : GenericDal<Order_line_export>, IOrderLineExportDal
	{
		private readonly ICountriesDAL countriesDAL;

		public OrderLineExportDal(IDbConnection conn, ICountriesDAL countriesDAL) : base(conn)
		{
			this.countriesDAL = countriesDAL;
		}

		public List<Order_line_export> GetForPeriodV6(int monthFrom = 0, int monthTo = 0, IList<string> cprodCode = null)
        {
			return conn.Query<Order_line_export>($@"SELECT* FROM order_line_detail2_v6
                                            WHERE cprod_code1 IN @cprodCode 
			{( monthFrom != 0 ? "AND month21 BETWEEN @from AND @to" : "")}", new { cprodCode, from = monthFrom, to = monthTo}).ToList();            
        }

        public List<Order_line_export> GetShippingForProduct_V6(int cprod_id)
        {
			return conn.Query<Order_line_export>(@"SELECT* FROM order_line_detail2_v6
                                            WHERE cprod_id = @cprod_id", new { cprod_id}).ToList();            
        } 



		public List<Order_line_Summary> GetCustomerSummaryForPeriod(DateTime? from, DateTime? to,int? brand_user_id = null,
			string cprod_code=null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2", bool brands = true)
		{
			var customerList = excludedCustomers.Split(',');
			return conn.Query<Order_line_Summary>(
				$@"SELECT customer_code as code, SUM(orderqty * unitprice * (CASE unitcurrency WHEN 0 THEN 0.625 ELSE 1 END )) AS Total, 
					SUM(orderqty) AS Sum_order_qty
					FROM order_line_detail2_v7 WHERE (@cprod_code IS NULL OR cprod_code1 = @cprod_code) 
					AND distributor{(brands ? ">" : "<=" )}0 AND hide_1 = 0 
					AND (req_eta >= @from OR @from IS NULL) AND (req_eta <= @to OR @to IS NULL) 
					AND (brand_user_id = @userid OR @userid IS NULL) {countriesDAL.GetCountryCondition(countryFilter)}
                    AND customer_code NOT IN @customerList 
                    GROUP BY customer_code",
					new {cprod_code, userid = brand_user_id, from, to, customerList}
				).ToList();
			
		}

        public List<Order_line_Summary> GetFactorySummaryForPeriod(DateTime? from, DateTime? to, int? brand_user_id = null, string cprod_code = null, 
			CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2")
        {
			var customerList = excludedCustomers.Split(',').Select(s => $"'{s}'").ToList();
			return conn.Query<Order_line_Summary>(
				$@"SELECT factory_code as code, SUM(orderqty * unitprice * (CASE unitcurrency WHEN 0 THEN 0.625 ELSE 1 END )) AS Total, 
				SUM(orderqty) AS Sum_order_qty
				FROM order_line_detail2_v7 WHERE (@cprod_code IS NULL OR cprod_code1 = @cprod_code) 
				AND distributor > 0 AND hide_1 = 0 
				AND (req_eta >= @from OR @from IS NULL) AND (req_eta <= @to OR @to IS NULL) AND (brand_user_id = @userid OR @userid IS NULL) 
				{countriesDAL.GetCountryCondition(countryFilter)}
                AND customer_code NOT IN @customerList 
                GROUP BY factory_code",
				new {from, to, userid = brand_user_id, cprod_code, customerList}
				).ToList();
            
        }


        public List<Order_line_export> GetForCriteria(List<int> factory_ids, DateTime? etaFrom = null,DateTime? etdFrom = null,
			IList<int> client_ids=null, bool includeDiscontinued = true)
		{
			return conn.Query<Order_line_export>(
				$@"SELECT * FROM order_line_detail2_v7 
                WHERE factory_id IN @factory_ids AND stock_order IN (1,8) AND (req_eta >= @eta OR @eta IS NULL) 
                AND (po_req_etd >= @etd OR @etd IS NULL) 
				{(client_ids != null ? " AND userid1 IN @client_ids" : "")}
				{(!includeDiscontinued ? " AND cprod_status <> 'D'" : "")}
				",
				new {eta = etaFrom, etd = etdFrom, factory_ids, client_ids}
				).ToList();			
		}

		public List<Company> GetFactories()
		{
			return conn.Query<Company>(
				"SELECT DISTINCT factory_id as user_id,factory_code, combined_factory FROM order_line_detail2_v7 WHERE stock_order IN (1,8)"
				).ToList();			
		}

		public void GetAllocationLines(IEnumerable<Order_line_export> lines, string type = "so")
		{
            //var cmd = new MySqlCommand("", conn);
			var sql = $@"SELECT order_lines.*,order_header.custpo,porder_header.po_req_etd, stock_order_allocation.alloc_qty, stock_order_allocation.unique_link_ref AS allocation_id, stock_order_allocation.{type}_line AS soline
									FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.{(type == "so" ? "st" : "so")}_line 
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
										INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
										INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
									WHERE {type}_line IN @lineids ORDER BY order_lines.linedate";
            var lineids = lines.Select(l => l.linenum).ToList();
            var allocations = conn.Query<Order_line_export>(sql,new { lineids = lineids }).GroupBy(l=>l.soline).ToDictionary(g=>g.Key, g=>g.ToList());
            foreach (var line in lines)
			{
				line.AllocatedLines = new List<Order_lines>();
                if(allocations.ContainsKey(line.linenum))
                {
                    line.AllocatedLines = allocations[line.linenum].Select(a =>
                            new Order_lines
                            {
                                linenum = a.linenum,
                                cprod_id = a.cprod_id,
                                orderid = a.orderid,
                                orderqty = a.orderqty,
                                unitcurrency = a.unitcurrency,
                                unitprice = a.unitprice,
                                Header = new Order_header { custpo = a.custpo, po_req_etd = a.po_req_etd},
                                AllocQty = a.alloc_qty,
                                allocation_id = a.allocation_id
                            }).ToList();
                }	
					
			}
				
			
		}

        public void GetAllocationCOLines(IEnumerable<Order_line_export> lines)
        {
			
        }



		
        public List<Company> GetClients()
        {
			return conn.Query<Company>(
				"SELECT DISTINCT userid1 as user_id,customer_code FROM order_line_detail2_v7 WHERE stock_order IN (1)"
				).ToList();            
        }

		protected override string GetAllSql()
		{
			return "SELECT * FROM order_line_detail2_v7";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}
	}

 
}
			
			