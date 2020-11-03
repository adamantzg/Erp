using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class mastproduct_price
	{
		public int id { get; set; }
		public int? mastproduct_id { get; set; }
		public int? currency_id { get; set; }
		public double? price { get; set; }
		public bool? isDefault { get; set; }
	}
}
