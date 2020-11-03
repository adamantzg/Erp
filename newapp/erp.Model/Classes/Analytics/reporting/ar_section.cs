
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Ar_section
	{
		public int id { get; set; }
		public string name { get; set; }
	
		public List<Ar_audience> Audiences { get; set; }
			
	}
}	
	