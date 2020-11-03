
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Containers
	{
		public int container_id { get; set; }
		public int? insp_id { get; set; }
		public string container_no { get; set; }
		public string seal_no { get; set; }
		public string container_size { get; set; }
		public int? container_count { get; set; }
		public double? container_space { get; set; }
		public string container_comments { get; set; }
	
	}
}	
	