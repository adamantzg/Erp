using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Web_product_file
    {
        public byte[] Data { get; set; }
        public bool DoesFileExist { get; set; }
        [NotMapped]
        public bool DoesPreviewFileExist { get; set; }

        public virtual Web_product_new Product { get; set; }
    }
}
