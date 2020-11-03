using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public enum BrochureLinkType
    {
        Default=1,
        Issue=2,
        NonePrice=3,
        Showering=4,
        ClearStone=5
    }
    public class BrochureDownloadLog
    {
        

        public int id { get; set; }
        public int? brand_id { get; set; }
        public DateTime? logDate { get; set; }
        public string url { get; set; }
        public int? type { get; set; }
    }
}
