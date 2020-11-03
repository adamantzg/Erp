using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class OrderLineSimple
    {
        public int? cprod_id { get; set; }
        public DateTime? LineDate { get; set; }
        public double? OrderQty { get; set; }
        public double? PriceGBP { get; set; }

        //public Cust_products Product { get; set; }
        
        public double? Total
        {
            get { return OrderQty*PriceGBP; }
        }
    }
}
