using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class manual_message_file
    {
        [Key]
        public int file_id { get; set; }
        public int message_id { get; set; }
        public string file_name { get; set; }
        public string file_guid { get; set; }
        public int? order { get; set; }

        public virtual manual_message Message { get; set; }
    }
}
