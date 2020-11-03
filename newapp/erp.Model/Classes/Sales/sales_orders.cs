
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("sales_orders")]
	public partial class Sales_orders
	{
        public int id { get; set; }
		public string alpha { get; set; }
		public DateTime? date_entered { get; set; }
		public string customer_order_no { get; set; }
		public string warehouse { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public int? order_qty { get; set; }
		public double? value { get; set; }
       
		public string order_no { get; set; }
		public int? cprod_id { get; set; }
        public string customer { get; set; }
        public int? despatched_qty { get; set; }
        public string delivery_reason { get; set; }
        public int? bundle_id { get; set; }
        public int rowid { get; set; }
        public int? status { get; set; }
        public string pick_list { get; set; }
        public double? cost_of_sale { get; set; }
        public DateTime? pick_list_date { get; set; }
        public DateTime? date_exported { get; set; }
		public DateTime? invoice_date { get; set; }
        public DateTime? date_imported { get; set; }
		public DateTime? date_report { get; set; }
		public string brand { get; set; }

        public Us_dealers Dealer { get; set; }
        public Cust_products Product { get; set; }
        public cust_products_bundle Bundle { get; set; }

        [NotMapped]
        public Sales_orders_headers Header { get; set; }

        //public List<Sales_orders_headers> Headers { get; set; }

		public bool IsCN
		{
			get
			{
				return order_no.StartsWith("CN");
			}
		} 

    }
}	
	