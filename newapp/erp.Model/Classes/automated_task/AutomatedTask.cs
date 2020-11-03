using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class AutomatedTask
    {
        public int id { get; set; }
        public string url { get; set; }
        public string subject { get; set; }
        public string mail_to { get; set; }
        public string mail_cc { get; set; }
        public string mail_bcc { get; set; }
        public string body { get; set; }
        public string downloadfilename { get; set; }
        public int? iterator_id { get; set; }

        public List<AutomatedTask_Attachment> Attachments { get; set; }
        public virtual AutomatedTaskIterator Iterator { get; set; }
    }
}
