
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{

    public enum SalesOrderHeaderDeliveryStatus
    {
        Collected = 1,
        Delivered = 2
    }
	
	public partial class Sales_orders_headers
	{
        

        public int id { get; set; }
		public string order_no { get; set; }
		public string customer { get; set; }
		public string address1 { get; set; }
		public string address2 { get; set; }
		public string address4 { get; set; }
		public string address3 { get; set; }
		public string address5 { get; set; }
		public string address6 { get; set; }
		public string town_city { get; set; }
		public string county { get; set; }
		public string state_region { get; set; }
		public string iso_country_code { get; set; }
		public string country { get; set; }
		public DateTime? date_entered { get; set; }
		public DateTime? date_received { get; set; }
		public DateTime? date_required { get; set; }
		public DateTime? date_despatched { get; set; }
		public int rowid { get; set; }
		public string carrier_code { get; set; }
        public SalesOrderHeaderDeliveryStatus? delivered { get; set; }

        public List<Sales_orders_headers_shipping> Shippings { get; set; }
        [NotMapped]
        public int? order_qty { get; set; }
        [NotMapped]
        public double? value;
        [NotMapped]
        public List<Sales_orders> Lines { get; set; }

        [NotMapped]
        public int? shippingsLength { get; set; }
        [NotMapped]
        public string shippingsNumber { get; set; }

    }
}	
	