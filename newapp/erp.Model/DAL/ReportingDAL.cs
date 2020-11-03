using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using asaq2.Model.Reporting;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model.Reporting
{
    public class ReportingDAL
    {
        public static List<PageVisit> GetPageVisits()
        {
            List<PageVisit> result = new List<PageVisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            { 
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT login_history_page.page_description, COUNT(*) AS PageCount 
                                                      FROM login_history_detail INNER JOIN login_history_page ON login_history_detail.visit_page = login_history_page.page_url
                                                      WHERE login_history_page.page_description IS NOT NULL
                                                      GROUP BY login_history_page.page_description", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    PageVisit pv = new PageVisit();
                    pv.PageDescription = string.Empty + dr["page_description"];
                    pv.PageCount = Convert.ToInt32(dr["PageCount"]);
                    result.Add(pv);
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductVisit> GetProductVisits()
        {
            List<ProductVisit> result = new List<ProductVisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT
                                                    cust_products.cprod_id,
                                                    cust_products.cprod_code1,
                                                    cust_products.cprod_name,
                                                    users.customer_code,
                                                    Count(*) AS pagecount
                                                    FROM
                                                    login_history_detail
                                                    INNER JOIN cust_products ON cust_products.cprod_id = login_history_detail.cprod_id
                                                    INNER JOIN users ON users.user_id = cust_products.cprod_user
                                                    GROUP BY
                                                    cust_products.cprod_id,
                                                    cust_products.cprod_code1,
                                                    cust_products.cprod_name,
                                                    users.customer_code
                                                    ORDER BY pagecount DESC", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ProductVisit pv = new ProductVisit();
                    pv.cprod_id = (int)dr["cprod_id"];
                    pv.code = string.Empty + dr["cprod_code1"];
                    pv.name = string.Empty + dr["cprod_name"];
                    pv.Custcode = string.Empty + dr["customer_code"];
                    pv.count = Convert.ToInt32(dr["pagecount"]);
                    result.Add(pv);
                }
                dr.Close();
            }
            return result;
        }
    }
    
}
