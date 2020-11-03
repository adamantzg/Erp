
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
    
	
	public partial class Dealer_display_rebate
	{
	    public const int TypeArcade = 1;
	    public const int TypeGeneral = 0;
	    public const int TypeSales = 2;

        [Key]
		public int unique_id { get; set; }
		public int dealer_id { get; set; }
		public double? value { get; set; }
		public DateTime? date_created { get; set; }
		public int? useruserid { get; set; }
		public int? type { get; set; }
        public int? brand_id { get; set; }
        public string reference { get; set; }

        public Dealer Dealer { get; set; }
	}
}	
	