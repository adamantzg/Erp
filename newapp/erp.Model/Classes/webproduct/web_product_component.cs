
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Web_product_component
	{
		public int web_unique { get; set; }
		public int cprod_id { get; set; }
		public virtual Cust_products Component { get; set; }
		public int? qty { get; set; }
		public int? order { get; set; }
		//public virtual ICollection<Web_product_file> WebFiles { get; set; }
        public virtual Web_product_new Product { get; set; }
		public Web_product_component()
		{
			qty = 1;
		}

		public Web_product_file GetFileByType(int type)
		{
			if(Product != null && Product.WebFiles != null)
			{
				return Product.WebFiles.FirstOrDefault(f => f.file_type == type);
			}
			return null;
		}

        public string CprodCode1
        {
            get { return !string.IsNullOrEmpty(Component?.cprod_code1_web_override) ? Component?.cprod_code1_web_override: Component?.cprod_code1; }
        }

        public string CprodName
        {
            get { return !string.IsNullOrEmpty(Component?.cprod_name_web_override) ? Component?.cprod_name_web_override: Component?.cprod_name; }
        }

    }
}	
	