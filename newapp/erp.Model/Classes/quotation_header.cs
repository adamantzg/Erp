
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Quotation_header
	{
		public int quotation_id { get; set; }
		public int? company_id { get; set; }
		public DateTime? date_created { get; set; }
		public double? container_price { get; set; }
		public double? margin { get; set; }
		public double? exchange_rate { get; set; }
		public int? currency_id { get; set; }
		public int? container_type { get; set; }

        public string company_name { get; set; }
        public string currency_name { get; set; }
        public string container_name { get; set; }
        public double? agent_commission { get; set; }

        public List<Quotation_lines> Lines { get; set; }
	
	}
}	
	