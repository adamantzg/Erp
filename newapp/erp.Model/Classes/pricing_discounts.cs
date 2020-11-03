using System;

namespace erp.Model
{
	public class  Pricing_Discounts
	{
		public int discount_id { get; set; }
		public string discount_name { get; set; }
		public int? discount_brand { get; set; }
		public double? retailer_discount { get; set; }
        public double? discount_ddp_cash_40 { get; set; }
        public double? discount_ddp_cash_20 { get; set; }
        public double? discount_ddp_credit_40 { get; set; }
        public double? discount_ddp_credit_20 { get; set; }
        public double? vat_rate { get; set; }
        public int? discount_status { get; set; }
	}
}	
	