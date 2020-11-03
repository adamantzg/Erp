
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Invoices
	{
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int invoice { get; set; }
		public int? orderid { get; set; }
		public DateTime? invdate { get; set; }
		public DateTime? cidate { get; set; }
		public DateTime? brsinvdate { get; set; }
		
		public int? userid1 { get; set; }
		public int? locid { get; set; }
		public double? exch_rate { get; set; }
		public string status { get; set; }
		public string delivery_address1 { get; set; }
		public string delivery_address2 { get; set; }
		public string delivery_address3 { get; set; }
		public string delivery_address4 { get; set; }
		public string delivery_address5 { get; set; }
		public string invoice_address1 { get; set; }
		public string invoice_address2 { get; set; }
		public string invoice_address3 { get; set; }
		public string invoice_address4 { get; set; }
		public string invoice_address5 { get; set; }
		public int? currency { get; set; }
		public string notes { get; set; }
		public double? inv_amount { get; set; }
		public double? inv_amount2 { get; set; }
		public double? inv_amount3 { get; set; }
		public double? inv_payment { get; set; }
		public double? int_payment_tmp { get; set; }
		public DateTime? duedate1 { get; set; }
		public DateTime? duedate2 { get; set; }
		public double? sea_freight { get; set; }
		public double? duty { get; set; }
		public double? local_charge { get; set; }
		public string invoice_number { get; set; }
		public int? invoice_type_id { get; set; }
		public int? invoice_from { get; set; }
		public string reference_number { get; set; }
		public DateTime? eta { get; set; }
		public string trading_term { get; set; }
		public int? payment_details_id { get; set; }
		public string invoice_no { get; set; }
		public int? eb_invoice { get; set; }
		public bool confirmed { get; set; }
		public int? cprod_user { get; set; }
		public bool? vat_applicable { get; set; }
        public int? rebate_type { get; set; }
        public int? dealer_id { get; set; }
        public double? inv_discount { get; set; }
        [Required(ErrorMessage = "Client must be specified")]
        public string invoice_user { get; set; }

		public Company Client { get; set; }
		public Company From { get; set; }
		public Payment_details Payment { get; set; }

        [NotMapped]
		public double? Amount { get; set; }
        [NotMapped]
        public double? AmountCN { get; set; }

		public Order_header OrderHeader { get; set; }

		
		public List<Invoice_lines> Lines { get; set; }
		public List<Creditnote_line> CreditnoteLines { get; set; }

        public List<order_invoice_sequence> Sequences { get; set; }

        public int? EbInvoice
        {
            get
            {
                if (Sequences != null && Sequences.Count > 0)
                    return Sequences[0].sequence;
                return eb_invoice;
            }

        }

        [NotMapped]
        public int? sequence { get; set; }
        [NotMapped]
        public int? invoice_sequence_type { get; set; }
    }
}	
	