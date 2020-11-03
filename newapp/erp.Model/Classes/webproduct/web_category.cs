
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Web_category
	{
		public int category_id { get; set; }
		public int? brand_id { get; set; }
		public string name { get; set; }
		public string alternate_name { get; set; }
		public int? parent_id { get; set; }
		public string path { get; set; }
		public int? legacy_id { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public int? sequence { get; set; }
		public string image { get; set; }
		public int? display_type { get; set; }
		public int? option_component { get; set; }
		public string pricing_note { get; set; }
		public string group { get; set; }
		public double? sale_retail_percentage { get; set; }
		public int? sibling_id { get; set; }
		public int? image_type { get; set; }
		public int? hide_dimensions { get; set; }
        public int? category_status { get; set; }
        
        
    }
}	
	