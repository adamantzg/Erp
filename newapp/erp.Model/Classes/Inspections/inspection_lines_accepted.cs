
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Inspection_lines_accepted
	{
        [Key]
		public int insp_line_unique { get; set; }
		public int? insp_unique { get; set; }
		public int? insp_line_id { get; set; }
		public string insp_line_type { get; set; }
		public string insp_line_comments { get; set; }
		public int? insp_po_linenum { get; set; }
		public int? insp_qty2 { get; set; }
		public int? insp_qty3 { get; set; }

        public Inspection_lines_tested LineTested { get; set; }

		[NotMapped]
		public List<Inspection_images> Images { get; set; }
	}
}	
	