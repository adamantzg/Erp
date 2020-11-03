using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class feedback_issue_type
    {
        public const int Bug = 1;
        public const int ChangeRequest = 2;

        public int id { get; set; }
        public string name { get; set; }
        public int? feedback_type_id { get; set; }
    }
}
