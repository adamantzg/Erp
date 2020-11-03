using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class manual_administration
    {
        [Key]
        public int administration_id { get; set; }
        public int manual_id { get; set; }
        public int group_id { get; set; }
        public bool edit_rights { get; set; }
        public bool view_rights { get; set; }

        public virtual manual Manual { get; set; }
    }
}
