using System;

namespace erp.Model
{
    public class Web_site_visits
    {
        public int id { get; set; }
        public int? web_unique { get; set; }
        public DateTime? visit_date { get; set; }
        public string visit_IP { get; set; }
        public string visit_country { get; set; }
        public int? visit_site { get; set; }

        public int? Count { get; set; }
    }
}
