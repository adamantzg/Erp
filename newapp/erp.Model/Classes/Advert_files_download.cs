using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Advert_files_download
    {
        public int id { get; set; }
        public string postcode { get; set; }
        public string ip_address { get; set; }
        public DateTime? date { get; set; }
        public string filename { get; set; }
        public int? type { get; set; }
    }
}
