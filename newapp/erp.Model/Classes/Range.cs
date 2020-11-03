
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Range
	{
		public int rangeid { get; set; }
		public string range_name { get; set; }
		public string range_desc { get; set; }
		public string range_image { get; set; }
		public int? category1 { get; set; }
		public double? forecast_percentage { get; set; }
		public int? user_id { get; set; }

        public double? Sales { get; set; }
	}
}	
	