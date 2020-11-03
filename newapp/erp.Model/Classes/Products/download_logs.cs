using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Download_logs
    {
        public int id { get; set; }
        public DateTime log_date { get; set; }
        public int log_useruserid { get; set; }
        public string log_file { get; set; }
        public int log_crpodid { get; set; }
        public int log_mastid { get; set; }
    }
}
