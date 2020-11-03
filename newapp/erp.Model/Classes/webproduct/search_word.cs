
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Search_word
	{
		public int id { get; set; }
		public int? site_id { get; set; }
		public string word { get; set; }
		public int? group_id { get; set; }
	
	}
}	
	