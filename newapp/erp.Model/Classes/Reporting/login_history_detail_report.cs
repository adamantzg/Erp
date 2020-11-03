
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class login_history_detail_report
	{
		public int history_id { get; set; }
		public int? cprod_id { get; set; }
		public int detail_unique { get; set; }
		public string visit_page { get; set; }
		public string visit_URL { get; set; }
		public DateTime? visit_time { get; set; }
		public int? user_id { get; set; }
		public DateTime? login_date { get; set; }
		public string login_username { get; set; }
		public string login_country { get; set; }
		public string website { get; set; }
		public string ip_address { get; set; }
		public string pwd { get; set; }
		public string session_id { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public string page_description { get; set; }
		public string userwelcome { get; set; }
		public string user_name { get; set; }
		public string customer_code { get; set; }
        //public string userusername { get; set; }
        public string custpo { get; set; }
	
	}
}	
	