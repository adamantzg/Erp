
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Client_pages_allocated
	{
        [Key]
		public int userpage_id { get; set; }
		public int? userid { get; set; }
		public int? page_id { get; set; }
		public int? option1 { get; set; }

        public Client_page Page { get; set; }
	
	}
}	
	