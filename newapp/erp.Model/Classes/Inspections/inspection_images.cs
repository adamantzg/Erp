
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Inspection_images
	{
		public int image_unique { get; set; }
		public int? insp_line_unique { get; set; }
		public int? insp_unique { get; set; }
		public string insp_image { get; set; }
		public string insp_type { get; set; }
		public int? rej_flag { get; set; }
	
	}
}	
	