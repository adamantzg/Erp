using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class cust_products_extradata
	{
		public const int WrasApproved = 1;
        public const int Pending = 2; 
        public const int Compliant = 3;
        public const int NotApplicable = 4;

		[Key]
		public int cprod_id { get; set; }
		public double? lvh_terms { get; set; }
		public int? lvh_stock_type { get; set; }
		public string analysis_e { get; set; }
		public bool? removed_brochure { get; set; } 
		public bool? removed_website { get; set; }
		public bool? removed_distributor { get; set; }
        public bool? order_block { get; set; }
		public int? wras_approval { get; set;  }

		public Cust_products Cust_Products { get;set;}

	}
}
