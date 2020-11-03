using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Cust_product_files
    {
        public int cprod_id { get; set; }
        public string file_name { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string path { get; set; }
    }
    public class cust_product_file
    {
        [Key]
        public int id { get; set; }
        public int cprod_id { get; set; }
        public string file_name { get; set; }
        [ForeignKey("CustProductsFileType")]
        public int file_type { get; set; }
        //public int MyProperty { get; set; }

        public Cust_products_file_type CustProductsFileType { get; set; }
    }


}
