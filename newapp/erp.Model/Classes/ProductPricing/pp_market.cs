
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Market
	{
		public int id { get; set; }
		public string name { get; set; }
		public double? internal_cost { get; set; }	
		public double? vat { get; set; }
		public int? currency_id { get; set; }
	}
}	
	