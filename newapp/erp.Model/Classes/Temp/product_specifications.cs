using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Classes.Temp
{
    class product_specifications
    {
    }
    
    public class technical_data_type
    {
        public int data_type_id { get; set; }
        public string data_type_desc { get; set; }
    }

    class technical_product_data
    {
        public int unique_id { get; set; }
        public int mast_id { get; set; }
        public int technical_data_type { get; set; }
        public string technical_data { get; set; }
    }
}
