
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Claims_monthly_summary
	{
        [Key]
        public int returnsid { get; set; }
        public string factory_code { get; set; }
		public string customer_code { get; set; }
		public string return_no { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public string reason { get; set; }
		public int? return_qty { get; set; }
		public string decision { get; set; }
		public string factory_decision { get; set; }
		public double? refit_GBP { get; set; }
		public double? FOB_value { get; set; }
		public string currency { get; set; }
		public double? accepted_value { get; set; }
		public string request_month { get; set; }
        public int? claim_type { get; set; }
	}
}	
	