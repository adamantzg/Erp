
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class After_sales_enquiry_files
	{
		public int file_id { get; set; }
		public int? enquiry_id { get; set; }
		public string file_name { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }

        
	
	}
}	
	