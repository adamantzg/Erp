
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Sale
	{
		public int IdSale { get; set; }
		public int WebSiteId { get; set; }
		public DateTime SaleStart { get; set; }
		public DateTime SaleEnd { get; set; }
	
	}
}	
	