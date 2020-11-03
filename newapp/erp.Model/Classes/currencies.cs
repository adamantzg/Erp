
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Currencies
	{
	    public const int USD = 0;
	    public const int GBP = 1;
	    public const int EUR = 2;

		public int curr_code { get; set; }
		public string curr_desc { get; set; }
		public string curr_symbol { get; set; }
		public double? curr_exch1 { get; set; }
		public double? curr_exch2 { get; set; }
		public double? curr_exch3 { get; set; }

        public List<Order_lines> OrderLines { get; set; }

        public List<Porder_lines> PorderLines { get; set; }
	
	}
}	
	