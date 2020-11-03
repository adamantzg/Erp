
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Contract_sales_forecast
	{
		public int forecast_id { get; set; }
		public int? client_id { get; set; }
		public string reference { get; set; }
		public DateTime? startmonth { get; set; }
		public int? monthduration { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }

	    public string creator { get ; set ; }
        public string editor { get; set; }

        public List<Contract_sales_forecast_lines> Lines { get; set; }
	}
}	
	