
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class ProductPricing_model
	{
		public int id { get; set; }
		public string name { get; set; }
		public int? market_id { get; set; }
	
		public List<ProductPricing_model_level> Levels { get; set; }
		public Market Market { get; set; }
	}
}	
	