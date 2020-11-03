
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Web_product_file
	{
        public int id { get; set; }
		public int? web_unique { get; set; }
		public string name { get; set; }
		public int? file_type { get; set; }
		public int? width { get; set; }
		public int? height { get; set; }
		public int? image_site_id { get; set; }
        public string hires_tif { get; set; }
        public string hires_psd { get; set; }
        public string hires_eps { get; set; }
        public int? approval { get; set; }
        [NotMapped]
        public DateTime? created { get; set; }
        

    }
}	
	