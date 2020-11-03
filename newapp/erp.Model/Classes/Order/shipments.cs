
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Shipments
	{
        [Key]
		public int ship_id { get; set; }
		public int? orderid { get; set; }
		public int? poid { get; set; }
		public string packing_items { get; set; }
		public string shipped_from { get; set; }
		public string shipper_per { get; set; }
		public DateTime? sailing_date { get; set; }
		public string shipped_by { get; set; }
		public string container_no { get; set; }
		public string notes { get; set; }
		public string cbm { get; set; }
		public string qty_type_container { get; set; }
		public double? gross_weight { get; set; }
		public string forwarder { get; set; }
	
	}
}	
	