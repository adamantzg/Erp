
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Ar_report_section
	{
		public int report_id { get; set; }
		public int section_id { get; set; }
		public int? order { get; set; }
	
		public Ar_section Section { get; set; }
	}
}	
	