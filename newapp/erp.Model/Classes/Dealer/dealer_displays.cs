
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealer_displays
	{
		public int unique_id { get; set; }
		public int? client_id { get; set; }
		public int? web_unique { get; set; }
		public int? qty { get; set; }

        [NotMapped]
        public bool FileExists { get; set; }

        public Dealer Dealer { get; set; }
        public Web_product_new Product { get; set; }
	
	}
}	
	