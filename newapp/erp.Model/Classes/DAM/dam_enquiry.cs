using System;
using System.Collections.Generic;

namespace erp.Model
{
    public class DAM_enquiry
    {
        public int id { get; set; }
        public string category { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        public int? user_id { get; set; }
        public string ip_address { get; set; }
    }
}
