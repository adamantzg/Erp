
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
    /* seems obsolete - Tvrtko 12.5.2016*/
	public partial class Cust_products_file_type
	{
        [Key]
		public int id { get; set; }
		public string name { get; set; }

		public int? file_type_id { get; set; }
		public string suffix { get; set; }
		public string path { get; set; }
		//public string fieldname_mast { get; set; }
		public string fieldname { get; set; }
        public string table { get; set; }

        //public Cust_product_file CustProductFile { get; set; }
        public List<cust_product_file> CustProductFiles { get; set; }
    }
}
