using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class login_history_page_count
    {
        public string Username { get; set; }
        public string Url { get; set; }
        public int? page_type { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }
}
