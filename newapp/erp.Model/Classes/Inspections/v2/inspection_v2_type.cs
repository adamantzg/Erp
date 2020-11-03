
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_type
	{
	    public const int Final = 2;
	    public const int Loading = 1;
        public const int Sample = 6;

		public int id { get; set; }
		public string name { get; set; }
	
	}
}	
	