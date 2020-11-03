
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
    
	
	public partial class Inspection_v2_image_type
	{
		public const int Appearance = 1;
		public const int Dimension = 2;
		public const int Function = 3;
		public const int Material = 4;
		public const int Packaging = 5;
		public const int Summary = 8;

		public int id { get; set; }
		public string name { get; set; }
	    public string description { get; set; }
	}
}	
	