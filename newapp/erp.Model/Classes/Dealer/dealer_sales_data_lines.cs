using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Dealer_sales_data_lines
	{
        [Key]
		public int lineid { get; set; }
		public int? orderid { get; set; }
		public double? unit_price { get; set; }
		public int? qty { get; set; }
		public int? cprod_id { get; set; }

        public virtual Dealer_sales_data_header Header { get; set; }
        public virtual Cust_products Product { get; set; }
	}
}	
	