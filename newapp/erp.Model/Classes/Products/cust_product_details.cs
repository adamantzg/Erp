using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public partial class Cust_product_details
    {
        [Key]
        public int cprod_id { get; set; }
        public int cprod_mast { get; set; }
        public string cprod_code1 { get; set; }
        public int cprod_user { get; set; }
        public string cprod_name { get; set; }
        public int factory_id { get; set; }

    }
}
