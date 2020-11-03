using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using erp.Model.DAL.Properties;

//using asaq2.Model.Properties;

namespace erp.Model.DAL
{
    public class OrderSummayDAL
    {
        public static List<OrderSummary> GetCreatedInPeriod(DateTime? from, DateTime? to, bool gbie_only = true, IList<int> userIds = null )
        {
            var result = new List<OrderSummary>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("",conn);
                cmd.CommandText = string.Format(@"SELECT
                                        order_header.orderid,
                                        order_header.orderdate,
                                        order_header.custpo,
                                        customer.customer_code,
                                        Max(porder_header.po_req_etd) AS po_req_etd,
                                        COUNT(DISTINCT cust_products.cprod_user) AS brandcount,
                                        COUNT(DISTINCT mast_products.factory_id) AS factorycount,
                                        GROUP_CONCAT(DISTINCT factory.consolidated_port) AS location ,
                                        fn_compute_sabc(order_header.orderdate, MAX(porder_header.po_req_etd), mast_products.product_group) AS sabc,
                                        order_header.original_eta,order_header.req_eta
                                        FROM porder_lines
                                        INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                        RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum
                                        INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                        INNER JOIN users AS customer ON customer.user_id = order_header.userid1
                                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                                        INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                        
                                        INNER JOIN users AS factory ON (CASE WHEN order_header.loading_factory > 0 THEN order_header.loading_factory ELSE mast_products.factory_id END) = factory.user_id
                                        WHERE COALESCE(order_header.container_type,0) NOT IN(3,4,5)  AND
                                        mast_products.category1 <> 13 AND
                                        COALESCE(customer.test_account,0) = 0 AND
                                        order_header.`status` NOT IN ('X', 'Y') {1} AND order_header.orderdate BETWEEN @from AND @to
                                        {0}
                                        GROUP BY
                                        order_header.orderid,
                                        order_header.orderdate,
                                        order_header.custpo,
                                        customer.customer_code ", gbie_only ? "AND customer.user_country IN ('GB','IE')" : "",
                            userIds != null ? string.Format(" AND customer.user_id IN ({0})",Utils.CreateParametersFromIdList(cmd,userIds)) : "AND customer.distributor > 0");
                cmd.Parameters.AddWithValue("@from", from != null ? (object)from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object)to : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        private static OrderSummary GetFromDataReader(MySqlDataReader dr)
        {
            return new OrderSummary
            {
                customer_code = string.Empty + dr["customer_code"],
                orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]),
                custpo = string.Empty + dr["custpo"],
                po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),
                original_eta = Utilities.FromDbValue<DateTime>(dr["original_eta"]),
                req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]),
                brandcount = Convert.ToInt32(dr["brandcount"]),
                factorycount = Convert.ToInt32(dr["factorycount"]),
                locations = (string.Empty + dr["location"]).Split(',').Select(int.Parse).ToArray(),
                orderid = (int)dr["orderid"],
                sabc = string.Empty + dr["sabc"]
            };
        }

        public static List<OrderSummary> GetProducedInPeriod(DateTime? from = null, DateTime? to = null, DateTime? created_until = null, bool gbie_only = true, IList<int> userIds = null)
        {
            var result = new List<OrderSummary>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT
                                        order_header.orderid,
                                        order_header.orderdate,
                                        order_header.custpo,
                                        customer.customer_code,
                                        Max(porder_header.po_req_etd) AS po_req_etd,
                                        COUNT(DISTINCT cust_products.cprod_user) AS brandcount,
                                        COUNT(DISTINCT mast_products.factory_id) AS factorycount,
                                        GROUP_CONCAT(DISTINCT factory.consolidated_port) AS location,
                                        fn_compute_sabc(order_header.orderdate, MAX(porder_header.po_req_etd), mast_products.product_group) AS sabc,
                                        order_header.original_eta,order_header.req_eta
                                        FROM porder_lines
                                        INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                        RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum
                                        INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                        INNER JOIN users AS customer ON customer.user_id = order_header.userid1
                                        INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                                        INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                        
                                        INNER JOIN users AS factory ON (CASE WHEN order_header.loading_factory > 0 THEN order_header.loading_factory ELSE mast_products.factory_id END) = factory.user_id
                                        WHERE COALESCE(order_header.container_type,0) NOT IN(3,4,5)  AND
                                        mast_products.category1 <> 13 AND
                                        COALESCE(customer.test_account,0) = 0 AND
                                        order_header.`status` NOT IN ('X', 'Y') {1} AND (order_header.orderdate < @created_until OR @created_until IS NULL)
                                        {0}
                                        GROUP BY
                                        order_header.orderid,
                                        order_header.orderdate,
                                        order_header.custpo,
                                        customer.customer_code
                                        HAVING (MAX(porder_header.po_req_etd) >= @from OR @from IS NULL) AND (MAX(porder_header.po_req_etd)<= @to OR @to IS NULL)
                                        ", gbie_only ? "AND customer.user_country IN ('GB','IE')" : "", 
                                        userIds != null ? string.Format(" AND customer.user_id IN ({0})", Utils.CreateParametersFromIdList(cmd, userIds)) : "AND customer.distributor > 0");
                cmd.Parameters.AddWithValue("@from", from != null ? (object)from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object)to : DBNull.Value);
                cmd.Parameters.AddWithValue("@created_until",
                                            created_until != null ? (object)created_until : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
    }
}
