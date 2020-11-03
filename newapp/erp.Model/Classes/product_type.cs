using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Product_type
    {
        public int id { get; set; }
        public string product_type { get; set; }

        public virtual List<Cust_products> Products { get; set; }
    }
}
