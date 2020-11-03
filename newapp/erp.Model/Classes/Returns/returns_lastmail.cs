using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    public class returns_events
    {
        [Key]
        public int event_id { get; set; }

        public int return_id { get; set; }

        public DateTime event_time { get; set; }

        public bool? mail_sent { get; set; }

        public int event_type { get; set; }

        [ForeignKey("return_id")]
        [Required]
        public virtual Returns Return { get; set; }

    }
}
