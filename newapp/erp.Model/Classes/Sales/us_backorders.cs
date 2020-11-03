
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Us_backorders
	{
		public int id { get; set; }
		public DateTime? date_entered { get; set; }
		public string customer_order_no { get; set; }
		public string warehouse { get; set; }
		public string product { get; set; }
		public string description { get; set; }
		public int? order_qty { get; set; }
		public string order_no { get; set; }
		public string customer { get; set; }
		public string delivery_reason { get; set; }
		public string bundle_line_ref { get; set; }
		public int? physical_qty { get; set; }
		public double? standard_cost { get; set; }
		public int rowid { get; set; }
		public int? cprod_id { get; set; }
        public int? bundle_id { get; set; }

        public Us_dealers Dealer { get; set; }
        public Cust_products CustProduct { get; set; }
        public cust_products_bundle Bundle { get; set; }

    }
}	
	