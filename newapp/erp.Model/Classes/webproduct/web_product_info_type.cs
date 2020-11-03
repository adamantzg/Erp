using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class web_product_info_type
	{
		public const int Standards = 1;
		public const int AdditionalInfo = 2;
		public const int Approvals = 3;

		public const int Range = 5;
		public const int InstallationType = 6;
		public const int ApplicationMethod = 7;
		public const int MinPressure = 8;
		public const int Headwork = 9;
		public const int Connections = 10;


		public int id { get;set; }
		public string name { get; set; }
	}
}
