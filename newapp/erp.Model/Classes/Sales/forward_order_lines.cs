using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class forward_order_lines
	{
		public string area { get; set; }
		public string order_no { get; set; }
		public string product { get; set; }
		public string retail_flag { get; set; }
		public DateTime? date { get; set; }
		public float? qty { get; set; }
		public int? id { get; set; }
	}
}
