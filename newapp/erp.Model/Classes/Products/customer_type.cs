
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Customer_type
	{
        [Key]
		public int type_id { get; set; }
		public string name { get; set; }

        public List<Dealer_external> Dealers { get; set; }
	}
}	
	