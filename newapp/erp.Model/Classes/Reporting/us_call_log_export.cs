using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class us_call_log_export
    {
        [Key]
        public int id { get; set; }
        public string customer { get; set; }
        public string alpha { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string call_category { get; set; }
        public DateTime? date_created { get; set; }
        public string fedex_ref { get; set; }
        public string userwelcome { get; set; }
        public string person { get; set; }
        public string note { get; set; }
        public int? in_out { get; set; }
    }
}
