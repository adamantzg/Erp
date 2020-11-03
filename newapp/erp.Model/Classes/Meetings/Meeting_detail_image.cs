using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Meeting_detail_image
    {
        public int id { get; set; }
        public int? meeting_detail_id { get; set; }
        public string image { get; set; }

        public virtual Meeting_detail Detail { get; set; }
        
    }
}
