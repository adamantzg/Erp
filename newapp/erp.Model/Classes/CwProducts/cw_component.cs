
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Cw_component
	{
		public int id { get; set; }
		public string code { get; set; }
		public double? price { get; set; }
		public string name { get; set; }

		public List<Cw_component_feature> Features { get; set; }
	
	}
}	
	