using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Image_assignment_log
    {
        public int id { get; set; }
        public string name { get; set; }
        public string old_name { get; set; }
        public int type_id { get; set; }
        public DateTime upload_date { get; set; }
        public int upload_user { get; set; }
        public bool related { get; set; }
        public string comment { get; set; }
        public int? file_id { get; set; }
        public bool Is_Undo { get; set; }
        public DateTime LogDate { get; set; }
        public int? old_site_id { get; set; }

        public Image_assignment_log()
        {
            LogDate = DateTime.Now;
        }

    }
}
