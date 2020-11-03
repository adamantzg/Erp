using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Web_product_info_box
    {
        public const int Show = 1;
        public const int ShowBurlington = 2;

        public int id { get; set; }
        public int? web_unique{ get; set; }
        public int? info_type { get; set; }
    }
}
