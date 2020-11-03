
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Product_faults
	{
		public int fault_id { get; set; }
		public int? fault_cprod { get; set; }
		public string fault_category { get; set; }
		public string fault_reason { get; set; }
		public int? fault_qty { get; set; }
		public string fault_order_number { get; set; }
		public DateTime? fault_date { get; set; }
		public double? fault_cost { get; set; }
		public string fault_comments { get; set; }
		public string fault_po { get; set; }
		public string fault_original { get; set; }
		public DateTime? fault_TMS { get; set; }
		public int? fault_store { get; set; }
		public string fault_summary { get; set; }

        public Cust_products Product { get; set; }
	
	}
}	
	