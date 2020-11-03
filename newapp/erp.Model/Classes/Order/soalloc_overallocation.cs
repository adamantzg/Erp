using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class soalloc_overallocation
    {
        public int stockLineNum { get; set; }
        public int AllocQty { get; set;}
        public int StockLineQty { get; set; }
        public Order_header Header { get; set; }
        public Cust_products Product { get; set; }

        public List<Order_lines> CalloffLines { get; set; }
        public List<Order_lines> AvailableStockLines { get; set; }
    }
}
