using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class ProductPricingMastProductData
	{
		[Key]
		public int mastproduct_id { get; set; }
		public double? tooling_cost { get; set; }
		public int? tooling_currency_id { get; set; }
		public int? initial_stock { get; set; }
		public int? display_qty { get; set; }
	}
}
