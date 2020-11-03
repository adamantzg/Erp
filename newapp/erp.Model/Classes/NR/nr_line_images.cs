
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Nr_line_images
	{
		public int id { get; set; }
		public int? NR_line_id { get; set; }
		public string image_name { get; set; }
		public int? sequence { get; set; }
		public int? image_type { get; set; }
        public int? carton_no { get; set; }
        public int? nr_id { get; set; }

        [NotMapped]
        public string file_id { get; set; }

        public Nr_lines Line { get; set; }
        public Nr_image_type Type { get; set; }
	
	}
}	
	