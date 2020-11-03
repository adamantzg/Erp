using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class OrderSummary
    {
        public int orderid { get; set; }
        public DateTime? orderdate { get; set; }
        public string custpo { get; set; }
        public string customer_code { get; set; }
        public DateTime? po_req_etd { get; set; }
        public int brandcount { get; set; }
        public int factorycount { get; set; }
        public int[] locations { get; set; }
        public string sabc { get; set; }
        public DateTime? original_eta { get; set; }
        public DateTime? req_eta { get; set; }
    }

    
}
