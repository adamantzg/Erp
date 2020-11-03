using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class cust_product_file_type
    {
        public int id { get; set; }
        public string caption { get; set; }
        public string extensions { get; set; }
        public bool? isImage { get; set; }
        public string path { get; set; }
        public bool? IsUploadable { get; set; }
    }
}
