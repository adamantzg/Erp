
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("pp_market_product")]
	public partial class Market_product
	{
		public int market_id { get; set; }
		public int cprod_id { get; set; }
		public double? retail_price { get; set; }
	
	}
}	
	