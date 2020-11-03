using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class feedback_authorization_level
    {
        public int id { get; set; }
        public int feedback_authorization_id { get; set; }
        public int? level { get; set; }
        public int? authorization_usergroupid { get; set; }

        public feedback_authorization Authorization { get; set; }
    }
}
