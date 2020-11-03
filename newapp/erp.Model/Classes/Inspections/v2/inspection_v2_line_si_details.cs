
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_line_si_details
	{
		public int id { get; set; }
		public int? insp_line { get; set; }
		public int? type_id { get; set; }
		public string requirement { get; set; }
		public string comments { get; set; }

        public virtual Inspection_v2_image_type Type { get; set; }
	
	}
}	
	