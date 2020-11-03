using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Order_line_Summary
    {
        public string code { get; set; }
        public int? id { get; set; }
        public double total { get; set; }

        public int? sum_order_qty { get; set; }
    }
}
