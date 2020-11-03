using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class ProductSaleSummary
    {
        public int cprod_id { get; set; }
        public int? QtySold { get; set; }
        public double? Amount { get; set; }
    }

    public class ProductSaleMonthSummary : ProductSaleSummary
    {
        public int month21 { get; set; }
    }

    public class ProductOrderSummary
    {
        public int cprod_id { get; set; }
        public int TotalQty { get; set; }
        
    }
}
