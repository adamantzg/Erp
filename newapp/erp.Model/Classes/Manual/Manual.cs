using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class User_manual
    {
        public int id { get; set; }
        public int manual_type { get; set; }
        public int version { get; set; }
        public DateTime? date_uploaded { get; set; }
        public string file_name { get; set; }

        public User_manual_types UserManualType { get; set; }
    }
}
