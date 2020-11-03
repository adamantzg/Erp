
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Cw_component_feature
	{
		public int id { get; set; }
		public int? component_id { get; set; }
		public int? feature_id { get; set; }
		public int? order { get; set; }
		public string value { get; set; }

		public Cw_feature Feature { get; set; }
	
	}
}	
	