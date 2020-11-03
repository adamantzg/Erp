
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_order_lines
	{
		public int linenum { get; set; }
		public int? porderid { get; set; }
		public DateTime? linedate { get; set; }
		public int? cprod_id { get; set; }
		public string desc1 { get; set; }
		public double? orderqty { get; set; }
		public double? shipped { get; set; }
		public double? unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public int? linestatus { get; set; }
        //public int? mast_id { get; set; }
        //public string mfg_code { get; set; }
		public double? lme { get; set; }

        public Cust_products Product { get; set; }
	
	}
}	
	