
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class change_notice_product_table
	{
        [Key]
		public int product_id { get; set; }
		public int? cn_id { get; set; }
		public int? mastid { get; set; }
		public int? product_client { get; set; }
		public string product_po { get; set; }

        public virtual Mast_products MastProduct { get; set; }
        public virtual change_notice_table ChangeNotice { get; set; }
	
	}
}	
	