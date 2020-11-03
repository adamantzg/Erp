
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Ar_report
	{
		public int id { get; set; }
		public string name { get; set; }

		public List<Ar_report_section> Sections { get; set; }
	
	}
}	
	