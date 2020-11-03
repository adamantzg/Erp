
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model
{
    public class Dam_pos_orderline
    {
        public int linenum { get; set; }
        public int? orderid { get; set; }
        public string description { get; set; }
        public double? price { get; set; }
        public int? quantity { get; set; }
        public int? web_unique { get; set; }

        public Cust_products Product{ get; set; }
    }
}
