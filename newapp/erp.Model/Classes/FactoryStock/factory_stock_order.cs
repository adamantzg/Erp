
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Factory_stock_order
	{
		public int id { get; set; }
		public int? factory_id { get; set; }
		public string po_ref { get; set; }
		public DateTime? etd { get; set; }
		public DateTime? datecreated { get; set; }
		public int? creator_id { get; set; }

        public List<Factory_stock_order_lines> Lines { get; set; }
        public Company Factory { get; set; }
        public User Creator { get; set; }

        [NotMapped]
        public int? Balance { get; set; }
        [NotMapped]
        public double? BalanceValue { get; set; }
	    [NotMapped]
        public int? Currency { get; set; }
	}
}	
	