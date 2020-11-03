
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_summary_report_calloff
	{
		public int? alloc_qty { get; set; }
		public string month21 { get; set; }
		public double? FOB { get; set; }
		public double? CBM { get; set; }
		public int unique_link_ref { get; set; }
		public string description { get; set; }
		public double? orderqty { get; set; }
		public int? factory_id { get; set; }
		public DateTime? po_req_etd { get; set; }
	
	}
}	
	