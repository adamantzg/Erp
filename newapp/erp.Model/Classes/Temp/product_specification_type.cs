using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class product_specification_type
    {
        public int spec_unique { get; set; }
        public int spec_category { get; set; }
        public int spec_subcategory { get; set; }
        public string spec_desc { get; set; }
        public int spec_seq { get; set; }
    }
    //public class Technical_data_type
    //{
    //    public int data_type_id { get; set; }
    //    public string data_type_desc { get; set; }
    //}
    //public class Product_specifications
    //{
    //    public int prod_spec_unique { get; set; }
    //    public int prod_mas_id { get; set; }
    //    public int spec_type { get; set; }
    //    public string prod_data { get; set; }

    //}
    
}
