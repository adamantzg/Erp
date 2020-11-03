using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public partial class change_notice_reasons
    {
        
        public int id { get; set; }
        public string description { get; set; }
    }
}
