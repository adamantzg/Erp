
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
    
	public class Creditnote_line
	{
        [Key]
		public int line_id { get; set; }
		public int invoice_id { get; set; }
		public string return_no { get; set; }
		public string cprod_code { get; set; }
		public string client_ref { get; set; }
		public string cprod_name { get; set; }
		public double? unitprice { get; set; }
		public double? quantity { get; set; }
        public bool? overridden { get; set; }
	}
}	
	