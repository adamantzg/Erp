
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class After_sales_enquiry_sitevisit_status
	{
        public const int Pending = 1;
        public const int Complete = 2;

		public int status_id { get; set; }
		public string status_name { get; set; }
	
	}
}	
	