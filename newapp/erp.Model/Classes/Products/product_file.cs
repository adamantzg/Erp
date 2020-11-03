using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class product_file
	{
		public int id { get; set; }
		public string file_name { get; set; }
		public int? cprod_id { get; set; }
		public int? mast_id { get; set; }
		public int? type_id { get; set; }
		public int? order_index { get; set; }

		public product_file_type FileType { get;set; }

		[NotMapped]
		public string file_id { get;set;}
	}
}
