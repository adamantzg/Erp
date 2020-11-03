
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Inspection_lines_rejected
	{
        [Key]
		public int insp_line_unique { get; set; }
		public int? insp_unique { get; set; }
		public int? insp_line_id { get; set; }
		public string insp_line_type { get; set; }
		public string insp_line_rejection { get; set; }
		public string insp_line_action { get; set; }
		public int? insp_po_linenum { get; set; }
		public int? insp_qty2 { get; set; }
		public int? insp_qty3 { get; set; }
		public int? insp_ca { get; set; }
		public string insp_comments { get; set; }
		public string insp_reason { get; set; }
		public string insp_permanent_action { get; set; }
		public string insp_document { get; set; }
		public string insp_pdf { get; set; }
		public int? master_line { get; set; }
		public int? criteria_id { get; set; }
		public int? reworked { get; set; }

        public Inspection_lines_tested LineTested { get; set; }
	
	}
}	
	