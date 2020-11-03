
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public partial class Web_product_file_type
	{
		public int id { get; set; }
		public string name { get; set; }
		public string code { get; set; }
		public int? site_id { get; set; }
		public int? file_type_id { get; set; }
		public string path { get; set; }
		public string previewpath { get; set; }
		public string suffix { get; set; }


    }
}
