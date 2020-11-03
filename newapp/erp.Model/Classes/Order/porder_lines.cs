
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Porder_lines
	{
        [Key]
		public int linenum { get; set; }
		public int? porderid { get; set; }
		public DateTime? linedate { get; set; }
		public int? cprod_id { get; set; }
		public string desc1 { get; set; }
		public double? orderqty { get; set; }
		public double? pending_orderqty { get; set; }
		public double? unitprice { get; set; }
		public double? pending_unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public int? linestatus { get; set; }
		public int? mast_id { get; set; }
		public string mfg_code { get; set; }
		public string asaq_ref { get; set; }
		public int? soline { get; set; }
		public double? lme { get; set; }

		public virtual Order_lines OrderLine { get; set; }
	
	}
}	
	