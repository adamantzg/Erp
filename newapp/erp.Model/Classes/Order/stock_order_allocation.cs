
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_order_allocation
	{
        [Key]
		public int unique_link_ref { get; set; }
		public int? so_line { get; set; }

		public int? st_line { get; set; }
		public int? alloc_qty { get; set; }
		public DateTime? date_allocation { get; set; }
		public string line_status { get; set; }

        [NotMapped]
        public int so_orderid { get; set; }
        [NotMapped]
        public int co_orderid { get; set; }
        [NotMapped]
        public int cprod_id { get; set; }
        [NotMapped]
        public DateTime? po_req_etd { get; set; }
        [NotMapped]
        public string so_custpo { get; set; }
        [NotMapped]
        public string co_custpo { get; set; }

        public virtual Order_lines StockLine { get; set; }
        public virtual Order_lines CalloffLine { get; set; }
	}
}	
	