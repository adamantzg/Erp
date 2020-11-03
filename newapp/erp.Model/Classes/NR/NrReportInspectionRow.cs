using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class NrReportInspectionRow
    {
        public int? insp_line_id { get; set; }
        public int? insp_id { get; set; }
        public int? cprod_id { get; set; }
        public int? orderid { get; set; }
        public int? factory_id { get; set; }
        public string order_custpo { get; set; }
        public string insp_custpo { get; set; }
        public DateTime? req_eta { get; set; }
        public DateTime? po_req_etd { get; set; }
        public int? userid1 { get; set; }
        public string customer_code { get; set; }
        public string factory_code { get; set; }
        public bool orderAllocated { get; set; }
        public DateTime? startdate { get; set; }
        public int? change_notice_id { get; set; }
    }
}
