
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace erp.Model
{
	
	public partial class Product_investigation_status
	{
		public int id { get; set; }
        [Required]
		public string name { get; set; }
	
	}
}	
	