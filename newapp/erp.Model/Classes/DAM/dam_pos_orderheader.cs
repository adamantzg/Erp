using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model
{
    public class Dam_pos_orderheader
    {
        public int orderid { get; set; }
        public int? dam_user { get; set; }
        public DateTime? orderdate { get; set; }
        public string invoice_add1 { get; set; }
        public string invoice_add2 { get; set; }
        public string invoice_add3 { get; set; }
        public string invoice_add4 { get; set; }
        public string invoice_postcode { get; set; }
        public string delivery_add1 { get; set; }
        public string delivery_add4 { get; set; }
        public string delivery_add3 { get; set; }
        public string delivery_add2 { get; set; }
        public string delivery_postcode { get; set; }
        public string invoice_status { get; set; }

        public List<Dam_pos_orderline> Lines { get; set; }
    }
}
