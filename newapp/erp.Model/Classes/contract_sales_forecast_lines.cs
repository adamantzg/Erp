
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Contract_sales_forecast_lines
	{
		public int lines_id { get; set; }
		public int? forecast_id { get; set; }
		public int? cprod_id { get; set; }
		public double? qty { get; set; }
        public int? monthduration { get; set; }

        public string cprod_name { get; set; }
        public string cprod_code { get; set; }

        public Contract_sales_forecast Forecast { get; set; }
	
	}
}	
	