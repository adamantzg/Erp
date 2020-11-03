
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealer_brandstatus
	{
		public int dealer_id { get; set; }
		public int brand_id { get; set; }
		public int? brand_status { get; set; }

        public virtual Dealer Dealer { get; set; }
        public virtual Brand Brand { get; set; }
	
	}
}	
	