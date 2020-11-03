using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Nr_image_type
    {
        public const int Product = 1;
        public const int Label = 2;
        public const int Loading = 3;

        public int id { get; set; }
        public string name { get; set; }
    }
}
