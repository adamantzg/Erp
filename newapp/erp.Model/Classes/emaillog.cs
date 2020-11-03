
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Emaillog
	{
		public int id { get; set; }
		public string email { get; set; }
		public DateTime? logtime { get; set; }
		public int? type { get; set; }
        public string name { get; set; }
        public string company { get; set; }	
	}
}	
	