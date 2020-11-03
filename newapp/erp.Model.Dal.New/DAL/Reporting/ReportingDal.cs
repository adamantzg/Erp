using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace erp.Model.Dal.New
{
	public class ReportingDal : IReportingDal
	{
        protected MySqlConnection conn;
        private readonly ICountriesDAL countriesDAL;

        public ReportingDal(IDbConnection conn, ICountriesDAL countriesDAL)
        {
            this.conn = (MySqlConnection)conn;
            this.countriesDAL = countriesDAL;
        }

        public List<WeightDifferenceRow> GetProductsWithUpdatedWeight(DateTime? etd_from, DateTime? etd_to)
        {
            var result = new List<WeightDifferenceRow>();
            var sql = $@"SELECT DISTINCT factory.factory_code,cust_products.cprod_code1, mast_products.factory_ref, 
                        mast_products.prod_nw as System_weight,order_lines.override_nw as Updated_PO_weight, order_header.custpo, customer.customer_code
                        FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
                        INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        INNER JOIN users factory ON factory.user_id = mast_products.factory_id
                        INNER JOIN users customer ON order_header.userid1 = customer.user_id
                        WHERE (SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_lines.orderid) BETWEEN @etd_from AND @etd_to AND order_lines.override_nw>0 ";
            return conn.Query<WeightDifferenceRow>(sql, new { etd_from, etd_to }).ToList();
        }

        public List<OrderRow> GetOutstandingOrdersChangedETD(List<int> userIds)
        {
            var sql = @"SELECT order_header.orderid,order_header.custpo,amendments.timedate,
                    amendments.ref1,amendments.old_data, amendments.new_data
                    FROM order_header
                    INNER JOIN amendments ON order_header.orderid = amendments.ref1
                    where userid1 IN @userIds and timedate > now() - interval 7 day and process = 'modify etd' and old_data <> new_data";
            return conn.Query<OrderRow>(sql, new { userIds }).ToList();
        }

        public List<OrderRow> GetOrdersReport(List<int> userIds, DateTime etd, int type = 1)
        {
            var sql = $@"SELECT GROUP_CONCAT(DISTINCT factory.factory_code) as factory_code
                    ,factory.bs_code 
                    ,order_line_detail2_v6_combined.custpo
                    ,if(po_process_id < 100,NULL,MAX(order_line_detail2_v6_combined.po_req_etd)) AS po_req_etd
                    ,order_line_detail2_v6_combined.userid1 as userid
                    ,factory.user_name as bs_name
                    ,IFNULL(SUM(CASE WHEN unitcurrency = 1 THEN unitprice * orderqty ELSE 0 END),0) valueGBP
                    ,IFNULL(SUM(CASE WHEN unitcurrency = 0 THEN unitprice * orderqty ELSE 0 END),0) valueUSD
                    ,if(isnull(order_line_detail2_v6_combined.combined_custpo),if(po_process_id < 100,'',container_types.container_type_desc),CONCAT('Combined with ',order_line_detail2_v6_combined.combined_custpo)) as container_type_desc
                    ,ports.port_name
                    ,IF (po_req_etd > now() AND po_req_etd < now() + interval 1 week,'To be Shipped',
                        IF (po_req_etd < now(),'Shipped in transit',
                            IF (po_process_id < 100,'New order','In production'))) AS order_status,
                    if (po_process_id > 90,month(po_req_etd),'') as ETD_month,
                    if (po_process_id > 90,week(po_req_etd) + 1,'') as ETD_week
                    FROM order_line_detail2_v6_combined INNER JOIN users factory ON order_line_detail2_v6_combined.factory_id = factory.user_id
                        LEFT JOIN container_types ON order_line_detail2_v6_combined.container_type = container_types.container_type_id
                            INNER JOIN ports ON ports.port_code = factory.user_port AND ports.port_to = 'GB'
                    WHERE order_line_detail2_v6_combined.userid1 IN @userIds AND order_line_detail2_v6_combined.orderqty > 0 
                    {(type == 2 ? " AND order_line_detail2_v6_combined.custpo LIKE '5%'" : "")}
                    GROUP BY order_line_detail2_v6_combined.orderid
                    HAVING MAX(order_line_detail2_v6_combined.po_req_etd) > @etd";
            return conn.Query<OrderRow>(sql, new { userIds, etd }).ToList();
        }

        public List<OrderSummaryByLocationClientRow> GetSummaryByLocationClient(int brand_id, DateTime date,
            CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var sql = $@"SELECT factory.consolidated_port Location,CASE WHEN  order_header.req_eta < @date THEN 1 WHEN order_header.req_eta > @date AND porder_header.po_req_etd < @date 
                        THEN 2 ELSE 3 END AS Status, client.customer_code CustomerCode, SUM(order_lines.orderqty) AS Qty
                    FROM
                    porder_lines
                    INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                    INNER JOIN order_lines ON order_lines.linenum = porder_lines.soline
                    INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                    INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                    INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                    INNER JOIN users AS factory ON factory.user_id = mast_products.factory_id
                    INNER JOIN users AS client ON order_header.userid1 = client.user_id
                    WHERE order_header.status NOT IN ('X','Y') AND cust_products.brand_id = @brand_id  AND client.distributor > 0 AND mast_products.category1 <> 13
                    AND order_header.userid1 NOT IN (184) {countriesDAL.GetCountryCondition(countryFilter, "client.")}
					AND COALESCE (client.`test_account`, 0) = 0
                    GROUP BY factory.consolidated_port,CASE WHEN  order_header.req_eta < @date THEN 1 WHEN order_header.req_eta > @date AND porder_header.po_req_etd < @date THEN 2 ELSE 3 END,
                    order_header.userid1
                    ORDER BY factory.consolidated_port,Status,order_header.userid1";
            return conn.Query<OrderSummaryByLocationClientRow>(sql, new { brand_id, date }).ToList();
        }

        public  List<ProductDistributorDisplayCount> GetDisplayCountByCustomer(int brand_id,
            CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var sql = $@"SELECT users.customer_code distributor_code, COUNT(*) AS DisplayCount
                        FROM dealer_image_displays
                        INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                        INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                        INNER JOIN dealer_distributors ON dealer_distributors.dealer_id = dealers.user_id
                        INNER JOIN users ON dealer_distributors.distributor_id = users.user_id
                        WHERE EXISTS (SELECT web_product_component.cprod_id FROM web_product_component INNER JOIN cust_products ON web_product_component.cprod_id = cust_products.cprod_id 
                        WHERE web_product_component.web_unique = dealer_image_displays.web_unique AND cust_products.brand_id = @brand_id)
                        {countriesDAL.GetCountryCondition(countryFilter, "users.")} AND users.distributor > 0 
                        GROUP BY users.customer_code";
            return conn.Query<ProductDistributorDisplayCount>(sql, new { brand_id }).ToList();
        }

        public int GetDisplayCountForBrand(int brand_id, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var sql = $@"SELECT COUNT(*) AS numOfDisplays
                        FROM dealer_image_displays
                        INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                        INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                        WHERE EXISTS(SELECT dealer_distributors.dealer_id FROM dealer_distributors
                        INNER JOIN users ON dealer_distributors.distributor_id = users.user_id WHERE users.distributor > 0 AND dealer_distributors.dealer_id = dealers.user_id 
                        {countriesDAL.GetCountryCondition(countryFilter, "users.")}) 
                        AND EXISTS (SELECT web_product_component.cprod_id FROM web_product_component INNER JOIN cust_products ON web_product_component.cprod_id = cust_products.cprod_id 
                        WHERE web_product_component.web_unique = dealer_image_displays.web_unique AND cust_products.brand_id = @brand_id)";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new { brand_id }));
            
        }

        public List<ReturnsByCustomer> GetReturnsByCustomer(int brand_id,
            CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var sql = $@"SELECT users.customer_code,COUNT(*) AS Qty
                        FROM  `returns`
                        INNER JOIN cust_products ON cust_products.cprod_id = `returns`.cprod_id
                        INNER JOIN users ON users.user_id = `returns`.client_id
                        WHERE cust_products.brand_id = 11 {countriesDAL.GetCountryCondition(countryFilter, "users.")}
                        AND returns.decision_final = 1
                        GROUP BY users.customer_code";
            return conn.Query<ReturnsByCustomer>(sql, new { brand_id }).ToList();
        }

        public List<DealerSalesByCustomer> GetDealerSalesByCustomers(int brand_id,
            CountryFilter countryFilter = CountryFilter.UKOnly, IList<string> customersForSalesData = null)
        {
            var sql = $@"SELECT users.customer_code, SUM(invoice_lines.orderqty) AS Qty
                        FROM invoice_lines INNER JOIN invoices ON invoice_lines.invoice_id = invoices.invoice
                        INNER JOIN users ON invoices.userid1 = users.user_id
                        INNER JOIN cust_products ON invoice_lines.cprod_id = cust_products.cprod_id
                        WHERE cust_products.brand_id = @brand_id {countriesDAL.GetCountryCondition(countryFilter, "users.")} 
                        AND users.distributor > 0 AND invoices.rebate_type = 2
                        GROUP BY users.customer_code";
            var result = conn.Query<DealerSalesByCustomer>(sql, new { brand_id }).ToList();
            if (customersForSalesData != null)
            {
                var total = Utilities.FromDbValue<int>(conn.ExecuteScalar(@"SELECT SUM(sales_data.sales_qty) AS Qty
                    FROM sales_data                        
                    INNER JOIN cust_products ON sales_data.cprod_id = cust_products.cprod_id
		            WHERE cust_products.brand_id = @brand_id
                    ", new { brand_id })) ?? 0;
                    
                result.AddRange(customersForSalesData.Select(cust => new DealerSalesByCustomer
                {
                    customer_code = cust,
                    Qty = total
                }));
            }
                
            return result;
        }

        public int GetDealerSalesForBrand(int brand_id, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var sql = $@"SELECT SUM(invoice_lines.orderqty) AS Qty
                    FROM invoice_lines INNER JOIN invoices ON invoice_lines.invoice_id = invoices.invoice
                    INNER JOIN users ON invoices.userid1 = users.user_id
                    INNER JOIN cust_products ON invoice_lines.cprod_id = cust_products.cprod_id
                    WHERE cust_products.brand_id = @brand_id AND users.distributor > 0
                    {countriesDAL.GetCountryCondition(countryFilter, "users.")}
                    ";
                
             return Utilities.FromDbValue<int>(conn.ExecuteScalar(sql, new { brand_id })) ?? 0;
            
        }

        public DataTable GetViewData(string viewName, IList<string> criteria = null)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.connString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM " + viewName, conn);
                ProcessCriteria(cmd, criteria);
                var adp = new MySqlDataAdapter(cmd);
                var ds = new DataSet();
                adp.Fill(ds, viewName);
                return ds.Tables[0];
            }
        }

        private static void ProcessCriteria(MySqlCommand cmd, IList<string> criteria)
        {
            if (criteria != null)
            {
                var where = new List<string>();
                var index = 0;
                foreach (var crit in criteria)
                {
                    //Adds field ><= @param   WHERE clauses
                    var parts = Regex.Split(crit, "([<=>]+)");   // >, < , >=, <=, <>, = supported
                    if (parts.Length == 3)
                    {
                        where.Add($"{parts[0]}{parts[1]}@param{index}");
                        cmd.Parameters.AddWithValue($"@param{index}", parts[2]);
                        index++;
                    }
                }
                cmd.CommandText += " WHERE " + string.Join(" AND ", where);
            }
        }
    }
}
