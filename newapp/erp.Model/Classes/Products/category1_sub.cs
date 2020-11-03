
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Category1_sub
	{
        [Key]
		public int cat2_code { get; set; }
		public int? cat1_code { get; set; }
		public string cat2_desc { get; set; }
	
	}
}	
	