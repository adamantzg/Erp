using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class cust_products_bundle
    {
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public int? analytics_option_id { get; set; }

        public Analytics_options Option { get; set; }
        public virtual List<cust_products_bundle_component> Components { get; set; }
        
    }
}
