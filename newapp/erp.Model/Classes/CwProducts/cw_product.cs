
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Cw_product
	{
		public int id { get; set; }
		public string name { get; set; }
		public string code { get; set; }
		public string image { get; set; }
		public string folderName { get; set; }
		public int? site_id { get; set; }

		[NotMapped]
		public string parentName { get; set; }

		public List<Cw_component> Components { get; set; }
		public List<Cw_product_file> Files { get; set; }
		public cw_site Site { get; set; }
		public List<cw_product_info> Infos { get; set; }
	
	}
}	
	