
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_order_header
	{
		public int porderid { get; set; }
		public DateTime? orderdate { get; set; }
		public int? userid { get; set; }
		public string status { get; set; }
		public string poname { get; set; }
		public string poadd1 { get; set; }
		public string poadd2 { get; set; }
		public string poadd3 { get; set; }
		public string poadd4 { get; set; }
		public string poadd5 { get; set; }
		public string poadd6 { get; set; }
		public DateTime? po_ready_date { get; set; }
		public string po_reference { get; set; }
		public string instructions { get; set; }
		public string FPI { get; set; }
		public int? process_id { get; set; }
		public int? batch_inspection_line { get; set; }
        public int? from_id { get; set; }

        public Company From { get; set; }
        public Company Factory { get; set; }
        public List<Stock_order_lines> Lines { get; set; }
	
	}
}	
	