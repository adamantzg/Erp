
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
	
	public class Return_category
	{
        [Key]
		public int returncategory_id { get; set; }
		public string category_name { get; set; }
		public string category_code { get; set; }
		public bool? inspection_full_check { get; set; }
	
	}
}	
	