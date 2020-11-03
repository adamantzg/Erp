using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class CustProductFile
    {
        public int id { get; set; }
        public string filename { get; set; }
        public cust_product_file_type FileType { get; set; }
        public int? file_type_id { get; set; }
        public bool isPackFile { get; set; }
        public object file_id { get; set; }
        [NotMapped]
        public bool Exists { get; set; }
        
        
    }
}
