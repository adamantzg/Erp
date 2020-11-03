
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Nr_lines
	{
		public int id { get; set; }
		public int NR_id { get; set; }
		public int? inspection_lines_tested_id { get; set; }
		public int? inspection_lines_v2_id { get; set; }
		public string NR_line_comments { get; set; }
		public int? NR_line_type { get; set; }

        public virtual Nr_header Header { get; set; }
        public Inspection_lines_tested InspectionLineTested { get; set; }
        public Inspection_v2_line InspectionV2Line { get; set; }
        public List<Nr_line_images> Images { get; set; }
        public Nr_line_type Type { get; set; }
	
	}
}	
	