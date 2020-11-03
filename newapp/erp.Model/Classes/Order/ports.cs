using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class ports
	{
		public int port_id { get; set;}
		public string port_code { get; set;}
		public string port_name { get; set;}
		public string port_to { get; set;}		
		public int? port_days { get; set;}
		public int? port_group { get; set;}
	}
}
