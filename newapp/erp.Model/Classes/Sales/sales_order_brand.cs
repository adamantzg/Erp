using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class sales_orders_brand
	{
		public int id { get; set; }
		public string name { get; set; }
		public string warehouse { get; set; }
		public string factory_ids { get; set; }

		public List<int> FactoryIds
		{
			get
			{
				return company.Common.Utilities.GetIdsFromString(factory_ids);
			}
		}
	}
}
