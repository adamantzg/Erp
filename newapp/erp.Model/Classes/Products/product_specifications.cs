
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Product_specifications
	{
		public int prod_spec_unique { get; set; }
		public int? prod_mast_id { get; set; }
		public int? spec_type { get; set; }
		public string prod_data { get; set; }
	
	}
}	
	