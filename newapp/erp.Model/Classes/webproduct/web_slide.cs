using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public partial class Web_slide
    {
     
        public int id { get; set; }
        public string image_name { get; set; }
        public int? sequence { get; set; }
        public int? web_unique { get; set; }
        public int? web_page { get; set; }
    }
}
