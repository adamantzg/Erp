using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class sales_orders_monthly_data
    {
        public int id { get; set; }
        public string warehouse { get; set; }
        public int? month21 { get; set; }
        public double? value { get; set; }
    }
}
