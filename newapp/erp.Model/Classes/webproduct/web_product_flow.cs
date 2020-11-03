
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Web_product_flow
	{
		public int id { get; set; }
		public int? web_unique { get; set; }
		public int? part_id { get; set; }
		public int? pressure_id { get; set; }
		public double? value { get; set; }


		public virtual Web_product_new Product { get; set; }
		public virtual Web_product_pressure Pressure { get; set; }

	}
}	
	