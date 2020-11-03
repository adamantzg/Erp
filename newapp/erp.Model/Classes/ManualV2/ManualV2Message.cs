using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class manual_message
    {
        [Key]
        public int message_id { get; set; }
        public int node_id { get; set; }
        public string message_content { get; set; }
        public int creator_id { get; set; }
        public DateTime date_created { get; set; }

        public List<manual_message_file> Files { get; set; }
        public List<manual_message_audience> Audience { get; set; }

        public virtual manual_node Node { get; set; }
    }
}
