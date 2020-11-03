
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Order_lines_manual
	{
        [Key]
		public int linenum { get; set; }
		public int? orderid { get; set; }
		public DateTime? linedate { get; set; }
		public string cprod_id { get; set; }
		public string description { get; set; }
		public double? orderqty { get; set; }
		public double? override_cartonqty { get; set; }
		public double? unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public int? linestatus { get; set; }
		public int? record_type { get; set; }
		public double? net_weight { get; set; }
		public double? gross_weight { get; set; }
		public int? hide_on_bbs_cw_inv { get; set; }

        public double? RowPrice
        {
            get { return orderqty * unitprice; }
        }
	
	}
}	
	