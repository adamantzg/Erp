using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;


namespace erp.Model.DAL
{
    public class ReportingDAL
    {
        public static List<WeightDifferenceRow> GetProductsWithUpdatedWeight(DateTime? etd_from, DateTime? etd_to)
        {
            var result = new List<WeightDifferenceRow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    Utils.GetCommand(
                        @"SELECT DISTINCT factory.factory_code,cust_products.cprod_code1, mast_products.factory_ref, mast_products.prod_nw,order_lines.override_nw, order_header.custpo, customer.customer_code
                                            FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
                                            INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                                            INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                            INNER JOIN users factory ON factory.user_id = mast_products.factory_id
                                            INNER JOIN users customer ON order_header.userid1 = customer.user_id
                                            WHERE (SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_lines.orderid) BETWEEN @from AND @to AND order_lines.override_nw>0 ",
                        conn);
                cmd.Parameters.AddWithValue("@from", etd_from);
                cmd.Parameters.AddWithValue("@to", etd_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new WeightDifferenceRow
                        {
                            cprod_code = string.Empty + dr["cprod_code1"],
                            factory_code = string.Empty + dr["factory_code"],
                            factory_ref = string.Empty + dr["factory_ref"],
                            custpo = string.Empty + dr["custpo"],
                            customer_code = string.Empty + dr["customer_code"],
                            System_weight = Utilities.FromDbValue<double>(dr["prod_nw"]),
                            Updated_PO_weight = Utilities.FromDbValue<double>(dr["override_nw"])
                        });
                }
                dr.Close();
            }
            return result;
        }

        public static List<OrderRow> GetOrdersReport(List<int> userIds, DateTime etd, int type=1)
        {
            var result = new List<OrderRow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT factory.factory_code,factory.bs_code, om_detail1.custpo, MAX(om_detail1.po_req_etd) AS po_req_etd, om_detail1.userid1
                                        ,IFNULL(SUM(CASE WHEN unitcurrency = 1 THEN unitprice * orderqty ELSE 0 END),0) valueGBP
                                        ,IFNULL(SUM(CASE WHEN unitcurrency = 0 THEN unitprice * orderqty ELSE 0 END),0) valueUSD
                                      FROM om_detail1 INNER JOIN users factory ON om_detail1.factory_id = factory.user_id
                                      WHERE om_detail1.userid1 IN ({0}) AND om_detail1.status NOT IN ('X','Y') AND om_detail1.orderqty > 0 {1}
                                      GROUP BY om_detail1.orderid, om_detail1.factory_code
                                      HAVING MAX(om_detail1.po_req_etd) > @etd",
                                         Utils.CreateParametersFromIdList(cmd, userIds),type == 2 ? " AND om_detail1.custpo LIKE '5%'" : "");
                cmd.Parameters.AddWithValue("@etd", etd);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderRow
                    {
                        factory_code = string.Empty + dr["factory_code"],
                        bs_code = string.Empty + dr["bs_code"],
                        custpo = string.Empty + dr["custpo"],
                        po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),
                        userid = (int)dr["userid1"],
                        valueGBP = Utilities.FromDbValue<double>(dr["valueGBP"]),
                        valueUSD = Utilities.FromDbValue<double>(dr["valueUSD"])
                    });
                }
            }
            return result;
        }

        public static List<OrderSummaryByLocationClientRow> GetSummaryByLocationClient(int brand_id, DateTime date,
            CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var result = new List<OrderSummaryByLocationClientRow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
						@"SELECT factory.consolidated_port,CASE WHEN  order_header.req_eta < @date THEN 1 WHEN order_header.req_eta > @date AND porder_header.po_req_etd < @date 
                                    THEN 2 ELSE 3 END AS Status, client.customer_code, SUM(order_lines.orderqty) AS Qty
                                FROM
                                porder_lines
                                INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                INNER JOIN order_lines ON order_lines.linenum = porder_lines.soline
                                INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                                INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                INNER JOIN users AS factory ON factory.user_id = mast_products.factory_id
                                INNER JOIN users AS client ON order_header.userid1 = client.user_id
                                WHERE order_header.status NOT IN ('X','Y') AND cust_products.brand_id = @brand_id {0} AND client.distributor > 0 AND mast_products.category1 <> 13
                                AND order_header.userid1 NOT IN (184)
								AND COALESCE (client.`test_account`, 0) = 0
                                GROUP BY factory.consolidated_port,CASE WHEN  order_header.req_eta < @date THEN 1 WHEN order_header.req_eta > @date AND porder_header.po_req_etd < @date THEN 2 ELSE 3 END,
                                order_header.userid1
                                ORDER BY factory.consolidated_port,Status,order_header.userid1", AnalyticsDAL.GetCountryCondition(countryFilter,"client."));
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                cmd.Parameters.AddWithValue("@date", date);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderSummaryByLocationClientRow{CustomerCode = string.Empty + dr["customer_code"],Location = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                        Status = (OrderDeliveryStatus) dr["Status"],Qty = Utilities.FromDbValue<double>(dr["Qty"])});
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductDistributorDisplayCount> GetDisplayCountByCustomer(int brand_id,
            CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var result = new List<ProductDistributorDisplayCount>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT users.customer_code, COUNT(*) AS numOfDisplays
                        FROM dealer_image_displays
                        INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                        INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                        INNER JOIN dealer_distributors ON dealer_distributors.dealer_id = dealers.user_id
                        INNER JOIN users ON dealer_distributors.distributor_id = users.user_id
                        WHERE EXISTS (SELECT web_product_component.cprod_id FROM web_product_component INNER JOIN cust_products ON web_product_component.cprod_id = cust_products.cprod_id 
                        WHERE web_product_component.web_unique = dealer_image_displays.web_unique AND cust_products.brand_id = @brand_id) {0} AND users.distributor > 0 
                        GROUP BY users.customer_code",AnalyticsDAL.GetCountryCondition(countryFilter,"users.")), conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductDistributorDisplayCount {distributor_code = string.Empty + dr["customer_code"],DisplayCount = Convert.ToInt32(dr["numOfDisplays"])});
                }
                dr.Close();
            }
            return result;
        }

        public static int GetDisplayCountForBrand(int brand_id, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT COUNT(*) AS numOfDisplays
                            FROM dealer_image_displays
                            INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                            INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                            WHERE EXISTS(SELECT dealer_distributors.dealer_id FROM dealer_distributors
                            INNER JOIN users ON dealer_distributors.distributor_id = users.user_id WHERE users.distributor > 0 AND dealer_distributors.dealer_id = dealers.user_id {0}) 
                            AND EXISTS (SELECT web_product_component.cprod_id FROM web_product_component INNER JOIN cust_products ON web_product_component.cprod_id = cust_products.cprod_id 
                            WHERE web_product_component.web_unique = dealer_image_displays.web_unique AND cust_products.brand_id = @brand_id)", AnalyticsDAL.GetCountryCondition(countryFilter, "users.")),conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static List<ReturnsByCustomer> GetReturnsByCustomer(int brand_id,
            CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var result = new List<ReturnsByCustomer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT users.customer_code,COUNT(*) AS numOfReturns
                                FROM  `returns`
                                INNER JOIN cust_products ON cust_products.cprod_id = `returns`.cprod_id
                                INNER JOIN users ON users.user_id = `returns`.client_id
                                WHERE cust_products.brand_id = 11 {0}
                                AND returns.decision_final = 1
                                GROUP BY users.customer_code", AnalyticsDAL.GetCountryCondition(countryFilter, "users.")),conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ReturnsByCustomer { customer_code = string.Empty + dr["customer_code"], Qty = Convert.ToInt32(dr["numOfReturns"]) });
                }
                dr.Close();
            }
            return result;
        }

        public static List<DealerSalesByCustomer> GetDealerSalesByCustomers(int brand_id,
            CountryFilter countryFilter = CountryFilter.UKOnly, IList<string> customersForSalesData = null )
        {
            var result = new List<DealerSalesByCustomer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    Utils.GetCommand(string.Format(@"SELECT users.customer_code, SUM(invoice_lines.orderqty) AS Qty
                        FROM invoice_lines INNER JOIN invoices ON invoice_lines.invoice_id = invoices.invoice
                        INNER JOIN users ON invoices.userid1 = users.user_id
                        INNER JOIN cust_products ON invoice_lines.cprod_id = cust_products.cprod_id
                        WHERE cust_products.brand_id = @brand_id {0} AND users.distributor > 0 AND invoices.rebate_type = 2
                        GROUP BY users.customer_code", AnalyticsDAL.GetCountryCondition(countryFilter, "users.")), conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new DealerSalesByCustomer
                    {
                        customer_code = string.Empty + dr["customer_code"],
                        Qty = Convert.ToInt32(dr["Qty"])
                    });
                }
                dr.Close();

                if (customersForSalesData != null)
                {
                    cmd.CommandText = string.Format(@"SELECT SUM(sales_data.sales_qty) AS Qty
                        FROM sales_data                        
                        INNER JOIN cust_products ON sales_data.cprod_id = cust_products.cprod_id
		                WHERE cust_products.brand_id = @brand_id
                        ");
                    var total = Utilities.FromDbValue<int>(cmd.ExecuteScalar()) ?? 0;
                    result.AddRange(customersForSalesData.Select(cust => new DealerSalesByCustomer
                    {
                        customer_code = cust, Qty = total
                    }));
                }
                
                //dr = cmd.ExecuteReader();
                //while (dr.Read()) {
                //    result.Add(new DealerSalesByCustomer
                //    {
                //        customer_code = string.Empty + dr["customer_code"],
                //        Qty = Convert.ToInt32(dr["Qty"])
                //    });
                //}
                //dr.Close();
            }
            return result;
        }

        public static int GetDealerSalesForBrand(int brand_id, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT SUM(invoice_lines.orderqty) AS Qty
                    FROM invoice_lines INNER JOIN invoices ON invoice_lines.invoice_id = invoices.invoice
                    INNER JOIN users ON invoices.userid1 = users.user_id
                    INNER JOIN cust_products ON invoice_lines.cprod_id = cust_products.cprod_id
                    WHERE cust_products.brand_id = @brand_id AND users.distributor > 0
                    {0}
                    ", AnalyticsDAL.GetCountryCondition(countryFilter, "users.")), conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                return Utilities.FromDbValue<int>(cmd.ExecuteScalar()) ?? 0;
            }
        }

        public static DataTable GetViewData(string viewName, IList<string> criteria = null)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
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
            if(criteria != null)
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
