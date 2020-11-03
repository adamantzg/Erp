
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	public class Order_header
	{
	    public const int StockOrderRegular = 0;
	    public const int StockOrderStock = 1;
        public const int StockOrderSpares = 2;
	    public const int StockOrderCalloff = 8;

        public int orderid { get; set; }
		public DateTime? orderdate { get; set; }
		public int? userid1 { get; set; }
		public int? locid { get; set; }
		public int? stock_order { get; set; }
		public string status { get; set; }
		public string new_status { get; set; }
        public string invoice_name { get; set; }
        public string invoice_address1 { get; set; }
        public string invoice_address2 { get; set; }
        public string invoice_address3 { get; set; }
        public string invoice_address4 { get; set; }
		public string delivery_address1 { get; set; }
		public string delivery_address2 { get; set; }
		public string delivery_address3 { get; set; }
		public string delivery_address4 { get; set; }
		public string delivery_address5 { get; set; }
		public int? currency { get; set; }
		public double? surcharge { get; set; }
		public string notes { get; set; }
		public string custpo { get; set; }
		public DateTime? req_etd { get; set; }
		public DateTime? original_eta { get; set; }
		public DateTime? req_eta { get; set; }
		public DateTime? actual_eta { get; set; }
		public string loading_details { get; set; }
		public string reference_no { get; set; }
		public int? factory_pl { get; set; }
		public double? lme { get; set; }
		public string packing_list { get; set; }
		public int? edit_sort { get; set; }
		public int? mod_flag { get; set; }
		public int? eta_flag { get; set; }
		public int? process_id { get; set; }
		public int? combined_order { get; set; }
		public int? loading_factory { get; set; }
		public string upload { get; set; }
		public int? upload_flag { get; set; }
		public int? entered_by { get; set; }
		public int? payment { get; set; }
		public int? documents { get; set; }
		public int? documents2 { get; set; }
		public DateTime? loading_date { get; set; }
		public int? container_type { get; set; }
		public double? loading_perc { get; set; }
		public string forwarder_name { get; set; }
		public string despatch_note { get; set; }
        public DateTime? booked_in_date { get; set; }
        public string price_type_override { get; set; }
        [Column("eb_invoice")]
        public int? order_eb_invoice { get; set; }
		public int? location_override { get; set; }

		public double? bdi_vat { get; set; }
		public double? freight_value { get; set; }
		public double? bdi_invoice { get; set; }
		public double? bdi_import_fees { get; set; }
		public DateTime? sale_date { get; set; }

        public string freight_invoice_no { get; set; }
        public string bdi_import_fees_invoice_no { get; set; }
		public int? eta_offset { get; set; }
		public DateTime? system_eta { get; set; }

        public DateTime? delivery_date { get; set; }

        [NotMapped]
        public DateTime? po_req_etd { get; set; }
        [NotMapped]
        public DateTime? original_po_req_etd { get; set; }
        [NotMapped]
        public string po_comments { get; set; }
        [NotMapped]
        public int? po_process_id { get; set; }

        public virtual Company Client { get; set; }

        public virtual List<Order_lines> Lines { get; set; }

        public virtual List<Order_lines_manual> ManualLines { get; set; }

        [NotMapped]
        public virtual Invoices Invoice { get; set; }

        public virtual List<Invoices> Invoices { get; set; }

        public virtual List<Shipments> Shipments { get; set; }


        public virtual List<Porder_header> PorderHeaders { get; set; }

        [NotMapped]
        public double? Balance { get; set; }

        public Order_header Parent { get; set; }
        [NotMapped]
	    public int? factory_id { get; set; }

        [NotMapped]
        public string customer_code { get; set; }

        public List<Order_header> CombinedOrders { get; set; }

        public Company LoadingFactory { get; set; }

        [NotMapped]
        public string po_instructions { get; set; }

        public DateTime? req_eta_norm
        {
            get
            {
                if (booked_in_date == null) {
                    if (req_eta <= DateTime.Today)
                        return DateTime.Today.AddDays(1);
                    else
                        return req_eta;
                }
                else {
                    return booked_in_date;
                }
            }
        }

		
		public DateTime? req_eta_1week
		{
			get
			{
				return req_eta.AddDays(7);
			}
		}
		

	    public string Ref9 => $"9{orderid}";

        [NotMapped]
        public DateTime? SentToFactoryDate { get; set; }
    }

    public class OrderMonthlyData
    {
        public DateTime MonthYear { get; set; }
        public List<OrderAggregateData> Orders { get; set; }
    }

    public class OrderAggregateData
    {
        public string custpo { get; set; }
        public double Qty { get; set; }
    }

    public class OrderWeeklyData
    {
        public int? company_id { get; set; }
        public string company_name { get; set; }
        public string customer_code { get; set; }
        public bool isBrandDistributor { get; set; }
        public int? consolidated_port { get; set; }
        public int? factory_id { get; set; }
        public DateTime WeekStart { get; set; }
        public int OrderCount { get; set; }
    }
       

    
}
