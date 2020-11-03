
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Porder_header
	{
        [Key]
		public int porderid { get; set; }
		public DateTime? orderdate { get; set; }
		public int? userid { get; set; }
		public int? locid { get; set; }
		public string status { get; set; }
		public string po_status { get; set; }
		public string delivery_address1 { get; set; }
		public string delivery_address2 { get; set; }
		public string delivery_address3 { get; set; }
		public string delivery_address4 { get; set; }
		public string delivery_address5 { get; set; }
		public string delivery_address6 { get; set; }
		public string delivery_address7 { get; set; }
		public int? currency { get; set; }
		public string poname { get; set; }
		public string poadd1 { get; set; }
		public string poadd2 { get; set; }
		public string poadd3 { get; set; }
		public string poadd4 { get; set; }
		public string poadd5 { get; set; }
		public string poadd6 { get; set; }
		public int? soorderid { get; set; }
		public DateTime? po_req_etd { get; set; }
		public DateTime? original_po_req_etd { get; set; }
		public DateTime? pending_po_req_etd { get; set; }
		public DateTime? po_ready_date { get; set; }
		public int? po_ready { get; set; }
		public string po_reference { get; set; }
		public string instructions { get; set; }
		public DateTime? po_cfm_etd { get; set; }
		public string FPI { get; set; }
		public int? process_id { get; set; }
		public double? system_cbm { get; set; }
		public double? system_sqm { get; set; }
		public double? factory_cbm { get; set; }
		public double? factory_sqm { get; set; }
		public string comments { get; set; }
		public string comments_factory { get; set; }
		public string special_comments { get; set; }
		public int? fi_reviewed { get; set; }
		public int? li_reviewed { get; set; }
		public int? batch_inspection { get; set; }
		public int? invoices_paid { get; set; }
		public int? invoices_bl { get; set; }
		public int? original_process_id { get; set; }

        public virtual List<Porder_lines> Lines { get; set; }
        public virtual Company Factory { get; set; }

        
        public virtual Order_header OrderHeader { get; set; }
	
	}
}	
	