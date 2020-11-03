using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class order_invoice_sequence
    {
        public int id { get; set; }
        public int? orderid { get; set; }
        public int? invoiceid { get; set; }
        public int? sequence { get; set; }
        public int? type { get; set; }


    }
}
