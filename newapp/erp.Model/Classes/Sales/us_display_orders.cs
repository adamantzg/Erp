
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("us_display_orders")]
	public partial class us_display_orders
	{
        public int id { get; set; }
        public string alpha { get; set; }
		public DateTime? date_entered { get; set; }
		public string customer_order_no { get; set; }
		public string warehouse { get; set; }
		public string product { get; set; }
		public string description { get; set; }
		public int? order_qty { get; set; }
		public double? value { get; set; }
		public string order_no { get; set; }
		public int? status { get; set; }
		public double? cost_of_sale { get; set; }
		public string delivery_reason { get; set; }
		public DateTime? date_despatched { get; set; }
	    public string customer { get; set; }
        public int? cprod_id { get; set; }
        public int? bundle_id { get; set; }
        
	}
}	
	