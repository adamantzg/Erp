
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Image_type
	{
	    public const int Valves = 3;
        public const int NonStandard = 4;

		public int id { get; set; }
		public string name { get; set; }
		public int? width { get; set; }
		public int? height { get; set; }
	
	}
}	
	