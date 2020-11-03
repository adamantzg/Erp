
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Amendments
	{
		public int processid { get; set; }
		public string userid { get; set; }
		public DateTime? timedate { get; set; }
		public string tablea { get; set; }
		public string ref1 { get; set; }
		public string ref2 { get; set; }
		public string old_data { get; set; }
		public string new_data { get; set; }
		public string process { get; set; }
		public int? _checked { get; set; }
		public int? checked_user { get; set; }
		public DateTime? checked_date { get; set; }
		public int? reason { get; set; }
	
	}
}	
	