using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class product_group_class
	{ 
		public const int New = 10;
		public const int Redundant = 11;

		public int id {get;set;}
		public string name {get;set;}
		public double? threshold { get; set; }
		public int? order { get; set; }
	}
}
