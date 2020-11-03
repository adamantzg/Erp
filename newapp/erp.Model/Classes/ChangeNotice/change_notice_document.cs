using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public partial class change_notice_document
    {
        [Key, ForeignKey("ChangeNotice")]
        public int change_notice_id { get; set; }

        public bool product_drawing { get; set; }
        public bool instructions { get; set; }
        public bool label { get; set; }
        public bool packaging { get; set; }
        public bool photo { get; set; }
        public string formatted_change_doc { get; set; }
        public string formatted_change_doc_id { get; set; }

        public virtual Change_notice ChangeNotice { get; set; }
    }
}
