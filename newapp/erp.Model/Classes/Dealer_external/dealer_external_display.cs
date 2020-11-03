
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Dealer_external_display
	{
		public int dealer_id { get; set; }
		public int webproduct_id { get; set; }
		public int? qty { get; set; }
        public DateTime? datecreated { get; set; }

        [NotMapped]
        public bool FileExists { get; set; }

        public virtual Dealer_external Dealer { get; set; }
        public virtual Web_product_new WebProduct { get; set; }
	
	}
}	
	