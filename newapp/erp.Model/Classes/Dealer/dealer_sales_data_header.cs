using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace erp.Model
{
	public partial class Dealer_sales_data_header
	{
        [Key]
		public int orderid { get; set; }
		public int? dealer_id { get; set; }
		public DateTime? order_date { get; set; }
		public int? entered_by { get; set; }
        public string reference { get; set; }
        public int? for_arcade { get; set; }

        public virtual List<Dealer_sales_data_lines> Lines { get; set; }
        public virtual Dealer Dealer { get; set; }
	}
}	
	