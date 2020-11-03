
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class inspection_notified_summary_images_table
	{
        [Key]
		public int image_unique { get; set; }
		public int? insp_nr_unique { get; set; }
		public string insp_image { get; set; }
		public string insp_type { get; set; }
		public int? rej_flag { get; set; }
	
	}
}	
	