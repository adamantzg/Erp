using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class ProductPricingProject
	{
		public int id { get; set; }
		public string name { get; set; }
		public int? pricing_model_id { get; set; }
		public int? currency_id { get; set; }

		public ProductPricing_model PricingModel { get; set; }
		public Currencies Currency { get; set; }

		public List<Cust_products> Products { get; set; }
		[NotMapped]
		public List<ProductPricing_settings> Settings { get; set; }
		public List<ProductPricingProjectSettings> ProjectSettings { get; set; }
	}
}
