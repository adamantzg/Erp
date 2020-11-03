
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_line
	{
		public int id { get; set; }
		public int? insp_id { get; set; }
		public int? orderlines_id { get; set; }
		public string insp_mastproduct_code { get; set; }
		public string insp_custproduct_code { get; set; }
		public string insp_custproduct_name { get; set; }
		public int? qty { get; set; }
		public string custpo { get; set; }
        public int? cprod_id { get; set; }
        public int? inspected_qty { get; set; }
        public string comments { get; set; }
        public string factory_code { get; set; }
        public string si_requirement { get; set; }
	}
}	
	