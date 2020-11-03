
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Qc_comments
	{
        [Key]
		public int insp_comments_id { get; set; }
		public int? insp_unique { get; set; }
        public int? inspv2_id { get; set; }
		public string insp_comments_type { get; set; }
		public int? insp_comments_from { get; set; }
		public int? insp_comments_to { get; set; }
		public string insp_comments { get; set; }
		public DateTime? insp_comments_date { get; set; }

        public virtual User Creator { get; set; }
        public virtual Inspections Inspection { get; set; }
        public virtual Inspection_v2 InspectionV2 { get; set; }
	
	}
}	
	