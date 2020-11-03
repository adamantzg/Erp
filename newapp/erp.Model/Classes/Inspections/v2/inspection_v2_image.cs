
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_image
	{
		public int id { get; set; }
		public int? insp_line { get; set; }
		public string insp_image { get; set; }
		public int? type_id { get; set; }
		public int? rej_flag { get; set; }
        public int? order { get; set; }
        public string comments { get; set; }

        [NotMapped]
        public string file_id { get; set; }

        public virtual Inspection_v2_image_type ImageType { get; set; }
        public virtual Inspection_v2_line Line { get; set; }

	
	}
}	
	