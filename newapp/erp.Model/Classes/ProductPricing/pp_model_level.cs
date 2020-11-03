
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class ProductPricing_model_level
	{
		public int id { get; set; }
		public int? model_id { get; set; }
		public int? level { get; set; }
		public double? value { get; set; }

		public ProductPricing_model Model { get; set; }
	
	}
}	
	