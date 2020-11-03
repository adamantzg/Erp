
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Brand_categories_sub_sub
	{
		public int brand_sub_sub_id { get; set; }
		public int? brand_sub_id { get; set; }
		public string brand_sub_sub_desc { get; set; }
		public int? seq { get; set; }
		public string image1 { get; set; }
        public int? display_type { get; set; }
        public double? sale_retail_percentage { get; set; }
	}
}	
	