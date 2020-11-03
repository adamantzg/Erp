
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Sales_data
	{
		public int sales_unique { get; set; }
		public int? cprod_id { get; set; }
		public int? sales_qty { get; set; }
		public int? month21 { get; set; }


        public Cust_products Product { get; set; }
        [NotMapped]
        public string cprodName { get; set; }
	}
}	
	