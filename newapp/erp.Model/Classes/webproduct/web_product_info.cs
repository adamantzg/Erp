
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public class Web_product_info
	{
        public const int Standards = 1;
        public const int AdditionalInformation = 2;
		public int id { get; set; }
		public int? web_unique { get; set; }
		public int? type { get; set; }
		public string value { get; set; }
        public int? order { get; set; }

       
        public virtual Web_product_new Product { get; set; }

	}
}
