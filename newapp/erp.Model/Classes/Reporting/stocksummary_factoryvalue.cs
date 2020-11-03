
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Stocksummary_factoryvalue
	{
		public int id { get; set; }
		public int? brand_id { get; set; }
		public int? factory_id { get; set; }
        public int? combined_factory { get; set; }
		public double? value { get; set; }
		public DateTime? dateprovided { get; set; }

        public virtual Company Factory { get; set; }
	
	}
}	
	