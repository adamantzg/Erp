using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class product_file_type
	{
		public int id { get; set; }
		public string name { get; set; }
		public string path { get; set; }
		public bool? client_specific { get; set; }
	}
}
