using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class manual_message_audience
    {

        [Key]
        public int audience_id { get; set; }
        public int message_id { get; set; }
        public int user_id { get; set; }
        public bool read_indicator { get; set; }

        public virtual manual_message Message { get; set; }
    }
}
