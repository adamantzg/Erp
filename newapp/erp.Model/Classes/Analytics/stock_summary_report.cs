
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_summary_report
	{
		public int? stock_order { get; set; }
		public string status { get; set; }
		public int? cprod_id { get; set; }
		public double? orderqty { get; set; }
		public string sent_qty { get; set; }
		public string sent_qty2 { get; set; }
		public double? remaining { get; set; }
		public int? userid1 { get; set; }
		public int orderid { get; set; }
        [Key]
		public int linenum { get; set; }
		public int? st_line { get; set; }
		public DateTime? po_req_etd { get; set; }
		public string month21 { get; set; }
		public int? factory_id { get; set; }
		public string factory_code { get; set; }
        public double? rowprice_gbp { get; set; }
        public DateTime? req_eta { get; set; }
	}
}	
	