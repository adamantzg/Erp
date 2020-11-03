
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[NotMapped]
	public class Web_shortcuts
	{
		public int id { get; set; }
		public string shortcut_url { get; set; }
		public string destination_url { get; set; }
		public int? web_site_id { get; set; }        	
	}
}	
	