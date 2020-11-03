using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class ussales_log_report
    {
        public const int Type_GoodsIn = 1;
        public const int Type_SalesOut = 2;

        public string userwelcome { get; set; }
        public string type { get; set; }
        public string custpo { get; set; }
        public DateTime? Logdate { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public double? old_qty { get; set; }
        public double? new_qty { get; set; }
        public DateTime? orderDate { get; set; }

        public int? type_id { get; set; }

        public int? orderqty { get; set; }

        public int? despatched_qty { get; set; }
        public int? received_qty { get; set; }

        public string Dealer { get; set; }
        public string State { get; set; }
        
    }
}
