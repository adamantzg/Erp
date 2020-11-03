
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Returns_comments_files
	{
		public int return_comment_file_id { get; set; }
		public int? return_comment_id { get; set; }
		public int? image_id { get; set; }
        public string image_name { get; set; }

        [NotMapped]
        public string file_id { get; set; }

        public virtual Returns_comments Comment { get; set; }
	
	}
}	
	