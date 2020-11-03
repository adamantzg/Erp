
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Cw_product_file
	{
		public int id { get; set; }
		public int? product_id { get; set; }
		public string filename { get; set; }
		public string filetype { get; set; }	

		public Cw_product Product { get; set; }
	}
}	
	