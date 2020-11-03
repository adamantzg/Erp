using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    [Table("manual")]
    public partial class manual
    {
        [Key]
        public int manual_id { get; set; }
        public string title { get; set; }
        public string short_description { get; set; }
        public int? version { get; set; }
        public int? creator_id { get; set; }
        public DateTime? date_created { get; set; }


        public List<manual_node> Nodes { get; set; }
        public List<manual_administration> AdministrationGroups { get; set; }
    }
}
