using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Product_investigation_images
    {
        public int id { get; set; }
        public string image_name { get; set; }
        public int cprod_id { get; set; }

        public int investigation_id { get; set; }
    }
}
