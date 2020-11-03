
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_order_brand_allocation
	{
		public int id { get; set; }
		public int? salesorder_line_id { get; set; }
		public int? stockorder_line_id { get; set; }
		public int? alloc_qty { get; set; }
		public DateTime? date_allocated { get; set; }
	
	}
}	
	