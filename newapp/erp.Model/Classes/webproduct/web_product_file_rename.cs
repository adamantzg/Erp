
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Web_product_file_rename
	{
        [NotMapped]
		public int id { get; set; }
        [NotMapped]
		public string name { get; set; }
        [NotMapped]
        public string oldname { get; set; }
        [NotMapped]
        public int? web_unique { get; set; }
        [NotMapped]
		public int? file_type { get; set; }
	
	}
}	
	