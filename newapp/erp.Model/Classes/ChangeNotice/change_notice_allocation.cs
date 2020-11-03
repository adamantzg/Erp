
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Change_notice_allocation
	{
        [Key]
		public int id { get; set; }
		public int? notice_id { get; set; }
		public int? cprod_id { get; set; }
		public DateTime? dateAllocated { get; set; }
        public int? freestock { get; set; }

        public virtual Change_notice Notice { get; set; }
        public virtual Cust_products Product { get; set; }
        public virtual List<Order_header> Orders { get; set; }
	}
}	
	