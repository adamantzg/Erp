using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class ClaimsAnalyticsRow
    {
        public int cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int orderqty { get; set; }
        public int claims { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    

}
