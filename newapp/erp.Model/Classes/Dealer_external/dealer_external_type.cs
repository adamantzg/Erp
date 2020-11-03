
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Dealer_external_type
	{
		public int id { get; set; }
		public string name { get; set; }

        public List<Dealer_external> Dealers { get; set; }
	}
}	
	