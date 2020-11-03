using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class cust_products_bundle_component
    {
        
        public int bundle_id { get; set; }        
        public int cprod_id { get; set; }
        public int? sequence { get; set; }

        public virtual cust_products_bundle Bundle { get; set; }
        public virtual Cust_products Component { get; set; }
    }
}
