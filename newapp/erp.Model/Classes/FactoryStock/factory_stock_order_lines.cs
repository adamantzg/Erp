
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Factory_stock_order_lines
	{
		public int id { get; set; }
		public int? orderid { get; set; }
		public DateTime? linedate { get; set; }
		public int? mast_id { get; set; }
		public int? qty { get; set; }
		public double? price { get; set; }
		public int? currency { get; set; }
        public string cprod_code1 { get; set; }

        public Factory_stock_order Order { get; set; }
        public Mast_products MastProduct { get; set; }
	
	}
}	
	