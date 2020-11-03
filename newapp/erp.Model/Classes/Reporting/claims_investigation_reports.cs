using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Claims_investigation_reports
    {
        public int unique_id { get; set; }
        public int cprod_id { get; set; }
        public string investigation { get; set; }
        public string extras { get; set; }
        public string created_by { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_modify { get; set; }
        public string modify_by { get; set; }

        public IList<Claims_investigation_reports_action> ReportActions { get; set; }
    }

    public class Claims_investigation_reports_action
    {

        public int id { get; set; }

        public int report_id { get; set; }

        public string comments { get; set; }

        public IList<Claims_investigation_report_action_images> ActionImages { get; set; }
    }

    public class Claims_investigation_report_action_images
    {
        public int id { get; set; }
        public int action_id { get; set; }
        public string name { get; set; }

        public string image_title { get; set; }
    }
}
