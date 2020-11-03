
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Quotation_companies
	{
		public int company_id { get; set; }
		public string company_name { get; set; }
		public string address { get; set; }
		public string contact { get; set; }
		public string email { get; set; }
		public string phone { get; set; }
		public int? country_id { get; set; }
		public int? currency_id { get; set; }

        public string country_name { get; set; }
        public string curr_desc { get; set; }
	
	}
}	
	