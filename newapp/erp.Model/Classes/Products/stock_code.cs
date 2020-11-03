
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Stock_code
	{
	    public const int Key = 1;
	    public const int New = 4;

		public int stock_code_id { get; set; }
		public string stock_code_name { get; set; }
		public int? target_weeks { get; set; }
	
	}
}	
	