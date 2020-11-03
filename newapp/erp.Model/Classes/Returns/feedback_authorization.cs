using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class feedback_authorization
    {
        public int id { get; set; }
        public int? feedback_type_id { get; set; }
        public int? usergroup_id { get; set; }
        public int? feedback_issue_type_id { get; set; }

        public List<feedback_authorization_level> Levels { get; set; }
    }
}
