using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class manual_edit_history
    {
        [Key]
        public int history_id { get; set; }
        public int user_id { get; set; }
        public int node_id { get; set; }
        public DateTime edit_timestamp { get; set; }

        public virtual manual_node Node { get; set; }
        public virtual User EditUser {get;set;}
    }
}
