using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class AutomatedTask_Attachment
    {
        public int Id { get; set; }
        public int? task_id { get; set; }
        public string url { get; set; }
        public string filename { get; set; }
        public bool? useLink { get; set; }
        public string subFolder { get; set; }

        public AutomatedTask Task { get; set; }
    }
}
