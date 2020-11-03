using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    public class Returns_qcusers
    {
        
        public int return_id { get; set; }
        
        public int useruser_id { get; set; }

        [Column("type")]
        [Key]
        public int? type { get; set; }

        [ForeignKey("return_id")]
        [Key]
        public virtual Returns Return { get; set; }

        [ForeignKey("useruser_id")]
        [Key]
        public virtual User User { get; set; }
    }
}
