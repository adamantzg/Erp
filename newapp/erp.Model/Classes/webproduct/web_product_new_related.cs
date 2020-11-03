using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public enum RelationType
    {
        Old = 1,
        Exports = 2,
        Complementary=3
    }

    public class web_product_new_related
    {
        public int web_unique { get; set; }
		public int web_unique_related { get; set; }
		//public Cust_products Component { get; set; }
		public int? relation_type { get; set; }
        public Web_product_new Web_product_new { get; set; }
        public Web_product_new Web_product_related { get; set; }
		//public int? order { get; set; }
		//public ICollection<Web_product_file> WebFiles { get; set; }
        
        //public Web_product_component()
        //{
        //    qty = 1;
        //}

        public Web_product_file GetFileByType(int type)
        {
            if (Web_product_new != null && Web_product_new.WebFiles != null)
            {
                return Web_product_new.WebFiles.FirstOrDefault(f => f.file_type == type);
            }
            return null;
        }

        
    }
}
