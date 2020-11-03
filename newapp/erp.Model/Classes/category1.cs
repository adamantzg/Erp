
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Category1
	{
	    public const int category1_spares = 13;
	    public const int category1_baths = 1;
	    public const int Furniture = 5;

		[Key]
		public int category1_id { get; set; }
		public string cat1_name { get; set; }
		public double? cat1_duty { get; set; }
		public double? cat1_margin { get; set; }

		public List<Mast_products> Products { get; set;  }
	
	}
}	
	