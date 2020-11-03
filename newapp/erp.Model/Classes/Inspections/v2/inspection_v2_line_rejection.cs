
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_line_rejection
	{
		public int id { get; set; }
		public int? line_id { get; set; }
		public string rejection { get; set; }
		public string action { get; set; }
        public string comments { get; set; }
        public string document { get; set; }
		public string reason { get; set; }
		public string permanentaction { get; set; }
		public bool? ca { get; set; }
        public int? type { get; set; }

        public virtual Inspection_v2_line Line { get; set; }
	}
}	
	