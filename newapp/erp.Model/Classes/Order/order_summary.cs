using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class order_summary
    {
        public int orderid { get; set; }
        public string custpo { get; set; }
        public DateTime? orderdate { get; set; }
        public DateTime? req_eta { get; set; }
        public double? orderqty { get; set; }
        public double? received_qty { get; set; }
    }

    
}
