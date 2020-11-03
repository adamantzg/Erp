
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	public enum ProductPricingSettingId
	{
		Lme = 1,
		SageFreight = 2,
		FiscalAgent = 3,
		Commision = 4,
		GbpUsdRate = 5,
		GbpEurRate = 6
	}

	public partial class ProductPricing_settings
	{


		public int id { get; set; }
		public string name { get; set; }
		public double? numValue { get; set; }

		[NotMapped]
		public bool inherited { get; set; }
	}

	public partial class ProductPricingProjectSettings
	{
		public int project_id { get; set; }
		public int setting_id { get; set; }
				
		public double? numValue { get; set; }
	}
}	
	