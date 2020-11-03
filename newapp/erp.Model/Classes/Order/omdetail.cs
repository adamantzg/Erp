
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class OrderMgtmDetail
	{
		public int? orderid { get; set; }
		public int? cprod_id { get; set; }
		public int? container_type { get; set; }
		public DateTime? linedate { get; set; }
		public string description { get; set; }
		public double? orderqty { get; set; }
		public double? unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public int? cprod_mast { get; set; }
		public int? factory_id { get; set; }
		public string factory_ref { get; set; }
        public string factory_code { get; set; }
		public string factory_name { get; set; }
		public int? stock_order { get; set; }
		public DateTime? cprod_stock_date { get; set; }
		public string asaq_ref { get; set; }
		public string asaq_name { get; set; }
		public DateTime? original_eta { get; set; }
		public string special_comments { get; set; }
		public string user_name { get; set; }
		public int? mc_qty { get; set; }
		public int? pallet_qty { get; set; }
		public int? unit_qty { get; set; }
		public int? cprod_user { get; set; }
		public int? cprod_brand_cat { get; set; }
		public string status { get; set; }
		public DateTime? po_req_etd { get; set; }
		public DateTime? original_po_req_etd { get; set; }
		public int? om_seq_number { get; set; }
		public string month21 { get; set; }
		public string month22 { get; set; }
		public string week22 { get; set; }
		public int? userid1 { get; set; }
		public string custpo { get; set; }
		public DateTime? req_eta { get; set; }
		public DateTime? orderdate { get; set; }
		public int? consolidated_port { get; set; }
		public string customer_code { get; set; }
		public int? combined_factory { get; set; }
		public int? distributor { get; set; }
		public DateTime? po_ready_date { get; set; }
		public DateTime? booked_in_date { get; set; }
        public int? porder_id { get; set; }

        public DateTime? req_eta_norm
        {
            get
            {
                if (booked_in_date == null)
                {
                    if (req_eta <= DateTime.Today)
                        return DateTime.Today.AddDays(1);
                    else
                        return req_eta;
                }
                else
                {
                    return booked_in_date;
                }
            }
        }

    }
}	
	