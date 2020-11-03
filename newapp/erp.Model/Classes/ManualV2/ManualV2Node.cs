using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class manual_node
    {
        [Key]
        public int node_id { get; set; }
        public int manual_id { get; set; }
        public int? node_parent_id { get; set; }
        public string title { get; set; }
        public string order { get; set; }
        public string header { get; set; }
        public string content { get; set; }
        public string footer { get; set; }
        public DateTime date_created { get; set; }

        public List<manual_message> Messages { get; set; }
        public List<manual_edit_history> EditHistoryRecords { get; set;}

        public virtual manual Manual { get; set; }
        public virtual manual_node Parent { get; set; }

        public ICollection<manual_node> Children { get; set; }
    }
}
