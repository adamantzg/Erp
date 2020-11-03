
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Web_product_part
	{
	    public const int Handset = 1;
	    public const int Rose = 2;
	    public const int Spout = 3;
	    public const int SpoutAerator = 4;

		public int id { get; set; }
		public string name { get; set; }
        public string LegacyField { get; set; }
        public int? partseq { get; set; }
	
	}
}	
	