using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Issue_subscription
    {
        public User User { get; set; }
        //later implement group reference - when group table is available
        public int subs_id { get; set; }
        public int issue_id { get; set; }
        public int? user_id { get; set; }
        public int? group_id { get; set; }
    }
}
