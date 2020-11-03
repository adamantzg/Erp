
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Sales_orders_headers_shipping
	{
		public int id { get; set; }
		public int? header_id { get; set; }
		public string refnumber { get; set; }
		public double? weight { get; set; }
		public string document { get; set; }

        [NotMapped]
        public string file_id { get; set; }
       
	
	}
}	
	