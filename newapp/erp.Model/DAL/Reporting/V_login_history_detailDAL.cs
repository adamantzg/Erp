
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class V_login_history_detailDAL
	{
	
		public static List<login_history_detail_report> GetByCriteria(int company_id, DateTime? dateFrom, DateTime? dateTo)
		{
			var result = new List<login_history_detail_report>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM v_login_history_detail WHERE user_id = @company_id 
                                             AND (visit_time >= @dateFrom OR @dateFrom IS NULL) AND
                                             (visit_time <= @dateTo OR @dateTo IS NULL)", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object) dateFrom.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo.Value : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();

                foreach (var lh in result)
                {
                    var urlParts = lh.visit_URL.Split('&');
                    var orderidPart = urlParts.FirstOrDefault(u => u.StartsWith("orderid"));
                    var custPoPart = urlParts.FirstOrDefault(u => u.StartsWith("custpo"));
                    if (custPoPart != null)
                        lh.custpo = custPoPart.Split('=')[1];
                    else if (orderidPart != null)
                    {
                        int orderid;
                        if (int.TryParse(orderidPart.Split('=')[1],out orderid))
                        {
                            Order_header o = Order_headerDAL.GetById(orderid);
                            if (o != null)
                                lh.custpo = o.custpo;
                        }
                    }

                }
            }
            return result;
		}

        public static List<Company> GetCompanies()
        {
            var result = new List<Company>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT DISTINCT users.user_id, user_name, customer_code FROM login_history INNER JOIN users ON login_history.user_id = users.user_id ORDER BY user_name", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Company{user_id = (int) dr["user_id"], user_name = string.Empty + dr["user_name"], customer_code = string.Empty + dr["customer_code"]});
                }
                dr.Close();
            }
            return result;
        }


        private static login_history_detail_report GetFromDataReader(MySqlDataReader dr)
		{
			login_history_detail_report o = new login_history_detail_report();
		
			o.history_id =  (int) dr["history_id"];
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.detail_unique =  (int) dr["detail_unique"];
			o.visit_page = string.Empty + dr["visit_page"];
			o.visit_URL = string.Empty + dr["visit_URL"];
			o.visit_time = Utilities.FromDbValue<DateTime>(dr["visit_time"]);
			o.user_id = Utilities.FromDbValue<int>(dr["user_id"]);
			o.login_date = Utilities.FromDbValue<DateTime>(dr["login_date"]);
			o.login_username = string.Empty + dr["login_username"];
			o.login_country = string.Empty + dr["login_country"];
			o.website = string.Empty + dr["website"];
			o.ip_address = string.Empty + dr["ip_address"];
			o.pwd = string.Empty + dr["pwd"];
			o.session_id = string.Empty + dr["session_id"];
			o.cprod_code1 = string.Empty + dr["cprod_code1"];
			o.cprod_name = string.Empty + dr["cprod_name"];
			o.page_description = string.Empty + dr["page_description"];
			o.userwelcome = string.Empty + dr["userwelcome"];
			o.user_name = string.Empty + dr["user_name"];
			o.customer_code = string.Empty + dr["customer_code"];
			
			return o;

		}
		
		
	}
}
			
			