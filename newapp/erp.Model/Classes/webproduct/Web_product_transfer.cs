using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Web_product_transfer
    {
        [Key]
        public int unique_id { get; set; }
        public int web_unique { get; set; }
        public int brand_id { get; set; }
        public int? web_unique_new { get; set; }
        public int? move { get; set; }
    }
}
