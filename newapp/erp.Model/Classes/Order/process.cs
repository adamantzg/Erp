using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class process
    {
        [Key]
        public int code { get; set; }
        public string desc { get; set; }
        public string brief { get; set; }
        public int? trigger_so { get; set; }
        public int? trigger_po { get; set; }
        public int? trigger_so_back { get; set; }
        public int? trigger_po_back { get; set; }
        public int? emails { get; set; }
        public int? auth_req { get; set; }
        public int? proc_type { get; set; }
    }
}
