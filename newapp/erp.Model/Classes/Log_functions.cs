using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Log_functions
    {
        [Key]
        public int log_id { get; set; }
        public int? log_user { get; set; }
        public string log_function { get; set; }
        public int? log_orderid { get; set; }
        public DateTime? log_date { get; set; }

        public Order_header OrderHeader { get; set; }
    }
}
