
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Dealer_external
	{
		public int id { get; set; }
		public string code { get; set; }
		public string user_name { get; set; }
		public string user_address1 { get; set; }
		public string user_address2 { get; set; }
		public string user_address3 { get; set; }
		public string user_address4 { get; set; }
		public string postcode { get; set; }
		public string user_contact { get; set; }
		public string user_tel { get; set; }
		public string user_email { get; set; }
		public string user_website { get; set; }
		public int? dealer_type { get; set; }
		public int? sales_rep_id { get; set; }
		public double? longitude { get; set; }
		public double? latitude { get; set; }
		public string sqfeetrange { get; set; }
		public string annual_turnover_range { get; set; }
		public int? customer_type { get; set; }
		//public string sales_rep { get; set; }

        [NotMapped]
        public virtual double? Distance { get; set; }

        public virtual Dealer_external_type DealerType { get; set; }
        public virtual Customer_type CustomerType { get; set; }

        public virtual List<Brand_external> Brands { get; set; }
        public virtual List<Dealer_external_comment> Comments { get; set; }
        public virtual List<Dealer_external_display> Displays { get; set; }
	
	}

    //public class Dealer_external_Ex : Dealer_external
    //{
    //    public override double Distance { get; set; }
    //}
}	
	