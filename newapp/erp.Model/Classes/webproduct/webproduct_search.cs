using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Webproduct_search
	{
        [Key]
		public int web_unique { get; set; }
		public string web_name { get; set; }
		public int? web_site_id { get; set; }
		public string web_code { get; set; }
		public string whitebook_title { get; set; }
		public string web_code_override { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? cprod_id { get; set; }
        public int? category_id { get; set; }
        public string barcode { get; set; }
        
        public List<Web_product_file> WebFiles { get; set; }
        public Web_category Category { get; set; }
        public Web_site Site { get; set; }
        public Web_product_new WebProduct { get; set; }
    }
}	
	