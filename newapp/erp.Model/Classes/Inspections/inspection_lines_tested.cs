
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Inspection_lines_tested
	{
        [Key]
		public int insp_line_unique { get; set; }
		public int? insp_id { get; set; }
		public string insp_factory_ref { get; set; }
		public string insp_client_ref { get; set; }
		public string insp_client_desc { get; set; }
		public double? insp_qty { get; set; }
		public double? insp_override_qty { get; set; }
		public string insp_custpo { get; set; }
		public int? order_linenum { get; set; }
		public int? photo_confirm { get; set; }
		public int? photo_confirma { get; set; }
		public int? photo_confirmm { get; set; }
		public int? photo_confirmd { get; set; }
		public int? photo_confirmf { get; set; }
		public int? photo_confirmp { get; set; }
		public int? packaging_rej { get; set; }
		public int? label_rej { get; set; }
		public int? instructions_rej { get; set; }
		[NotMapped]
		public Returns CA { get; set; }

        public Order_lines OrderLine { get; set; }
        public Inspections Inspection { get; set; }
        public List<Inspection_lines_rejected> RejectedLines { get; set; }
        public List<Inspection_lines_accepted> AcceptedLines { get; set; }
        public List<Inspections_loading> Loadings { get; set; }
	
	}
}	
	