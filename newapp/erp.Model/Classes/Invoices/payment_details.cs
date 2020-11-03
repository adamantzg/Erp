
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Payment_details
	{
        [Key]
		public int payment_details_id { get; set; }
		public string bank_name { get; set; }
		public string address { get; set; }
		public string sort_code { get; set; }
		public string beneficiary_name { get; set; }
		public string beneficiary_accnumber { get; set; }
		public int? company_id { get; set; }
	
	}
}	
	