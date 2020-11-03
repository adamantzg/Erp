using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class ProductFit
    {
        public int cprod_id { get; set; }

        public string cprod_name { get; set; }

        public string cprod_status { get; set; }

        public string cprod_dwg { get; set; }

        public string mastProduct_prod_image3 { get; set; }

        public string mastProduct_prod_image5 { get; set; }

        public string mastProduct_prod_image4 { get; set; }

        public string mastProduct_prod_instructions { get; set; }

        public string cprod_code1 { get; set; }

        public string cprod_label { get; set; }

        public string cprod_packaging { get; set; }

        public int? mastProduct_prod_SpecCount { get; set; }

        public int? cprod_user { get; set; }

        public string cprod_instructions { get; set; }
        public string hi_res { get; set; }
    }
}
