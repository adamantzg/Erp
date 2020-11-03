using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    //[Table("us_call_log_category")]
    public partial class Us_call_log_category
    {
       [Key]
        public int id { get; set; }
        public string name { get; set; }

        //public List<Us_call_log> CallLogs { get; set; }
    }
    
}
