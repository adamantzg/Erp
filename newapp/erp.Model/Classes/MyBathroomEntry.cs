using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class MyBathroomEntry
    {
        public double? quantity { get; set; }
        public int productid { get; set; }
        
        //public virtual WebProduct Product { get; set; }
        public virtual Web_product_new ProductNew { get; set; }
    }
}
