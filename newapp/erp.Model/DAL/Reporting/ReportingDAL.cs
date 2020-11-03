using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;


namespace asaq2.Model
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
                    new MySqlCommand(
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

        public static List<OrderRow> GetOrdersReport(List<int> userIds, DateTime etd)
        {
            var result = new List<OrderRow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT factory.factory_code,factory.bs_code, om_detail1.custpo, MAX(om_detail1.po_req_etd) AS po_req_etd, om_detail1.userid1
                                      FROM om_detail1 INNER JOIN users factory ON om_detail1.factory_id = factory.user_id
                                      WHERE om_detail1.userid1 IN ({0}) AND om_detail1.status NOT IN ('X','Y') AND om_detail1.orderqty > 0
                                      GROUP BY om_detail1.orderid, om_detail1.factory_code
                                      HAVING MAX(om_detail1.po_req_etd) > @etd",
                                  Utilities.CreateParametersFromIdList(cmd, userIds));
                cmd.Parameters.AddWithValue("@etd", etd);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new OrderRow{factory_code = string.Empty + dr["factory_code"],bs_code = string.Empty + dr["bs_code"],custpo = string.Empty + dr["custpo"],
                                            po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),userid = (int) dr["userid1"]});
                }
            }
            return result;
        }
    }
}
