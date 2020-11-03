
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Web_site
	{
		public int id { get; set; }
		public string name { get; set; }
		public string code { get; set; }
		public int? brand_id { get; set; }
        public string Url { get; set; }

        public virtual List<Web_product_new> WebProducts { get; set; }
        public virtual Brand Brand { get; set; }
        	
	}
}	
	