using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Ip2
    {
        [Key]
        public long? ip_from { get; set; }
        public long? ip_to { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
    }
}
