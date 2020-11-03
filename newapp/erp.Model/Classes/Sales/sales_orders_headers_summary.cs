using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class sales_orders_headers_summary
    {
        public int? header_id { get; set; }
        public string dealer_name { get; set; }
        public string order_no { get; set; }
        public DateTime? date_entered { get; set; }
        public string customer_order_no { get; set; }
        public int? pick_list { get; set; }
        public string customer { get; set; }
        public int? order_qty { get; set; }
        public int? despatched_qty { get; set; }
        public double? value { get; set; }
        public List<Sales_orders> Lines { get; set; }
        public List<Sales_orders_headers_shipping> Shippings { get; set; }
    }
}
