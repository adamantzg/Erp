
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Invoice_type
	{
		public const int CreditNoteReturn = 4;
		public const int CreditNote = 3;

		public int invoice_type_id { get; set; }
		public string invoice_type_name { get; set; }
		public bool? showOnForm { get; set; }
	
	}
}	
	