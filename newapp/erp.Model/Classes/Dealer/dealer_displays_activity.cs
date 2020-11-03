
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealer_displays_activity
	{
        [Key]
		public int unique_id { get; set; }
		public int? web_unique { get; set; }
		public int? dealer_id { get; set; }
		public int? distributor_id { get; set; }
		public int? old_qty { get; set; }
		public int? new_qty { get; set; }
		public int? useruser_id { get; set; }
		public DateTime? datecreated { get; set; }

        public virtual Dealer Dealer { get; set; }
	
	}
}	
	