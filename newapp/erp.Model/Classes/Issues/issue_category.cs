
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Issue_category
	{
		public int category_id { get; set; }
		public string name { get; set; }
		public int? parent_id { get; set; }
        public int? child_count { get; set; }
	
	}
}	
	