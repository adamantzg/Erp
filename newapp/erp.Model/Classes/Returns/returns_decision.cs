using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class returns_decision
    {
        [Key]
        public int code { get; set; }
        public string description { get; set; }
        public int seq { get; set; }
        public string notes { get; set; }

    }
}
