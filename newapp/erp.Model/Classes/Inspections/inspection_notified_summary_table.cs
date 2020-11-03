
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
	
	public partial class inspection_notified_summary_table
	{
        [Key]
		public int insp_nr_unique { get; set; }
		public string insp_document { get; set; }
		public string insp_summary_comments { get; set; }
		public string insp_summary_reason { get; set; }
		public string insp_summary_changedetails { get; set; }
		public int? insp_nr_status { get; set; }
	
	}
}	
	