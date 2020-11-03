
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Sales_forecast_amendments
	{
		public int id { get; set; }
		public int? cprod_id { get; set; }
		public int? old_qty { get; set; }
		public int? new_qty { get; set; }
		public int? month21 { get; set; }
		public DateTime? dateModified { get; set; }

        public Cust_products Product { get; set; }
	
	}
}	
	