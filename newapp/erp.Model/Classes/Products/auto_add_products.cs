using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class auto_add_products
	{
		public int id {get;set;}
		public int? trigger_cprod_id {get;set;}
		public int? added_cprod_id {get;set;}
		public double? unitprice {get;set;}
		public int? unitcurrency {get;set;}
		public DateTime? startdate {get;set;}

		public Cust_products AddedProduct {get;set;}
	}
}
