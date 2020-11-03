
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Dist_products
	{
        [Key]
		public int distprod_id { get; set; }
		public int? client_id { get; set; }
		public int? dist_cprod_id { get; set; }
		public int? dist_opening_stock { get; set; }
		public string dist_special_code { get; set; }
		public string dist_special_desc { get; set; }
		public double? dist_special_price { get; set; }
		public int? dist_special_curr { get; set; }
		public int? dist_special_moq { get; set; }
		public int? dist_seq { get; set; }
		public int? dist_spec_disc { get; set; }
        public int? dist_stock { get; set; }
        public DateTime? dist_stock_date { get; set; }
        public string client_system_code { get; set; }
        public int? dist_onorder { get; set; }

        public virtual Company Distributor { get; set; }
        public virtual Cust_products Product { get; set; }
            
	}
}	
	