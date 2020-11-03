
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("Invoice_lines")]
	public class Invoice_lines
	{
        [Key]
		public int linenum { get; set; }
		public int? invoice_id { get; set; }
		public DateTime? linedate { get; set; }
		public string cprod_id { get; set; }
		public string description { get; set; }
		public double? orderqty { get; set; }
		public double? unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public int? linestatus { get; set; }
		public int? record_type { get; set; }
        public int? qty_type { get; set; }
        public int? image_id { get; set; }

        [NotMapped]
        public Cust_products CustProduct { get; set; }
	}
}	
	