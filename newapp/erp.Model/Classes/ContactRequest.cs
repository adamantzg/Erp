using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class ContactRequest
    {
        public int user_id { get; set; }
        public int? distributor { get; set; }
        public string user_surname { get; set; }
        public string user_firstname { get; set; }
        public string user_title { get; set; }
        public string user_address1 { get; set; }
        public string user_address2 { get; set; }
        public string user_address3 { get; set; }
        public string user_address4 { get; set; }
        public string user_address5 { get; set; }
        public string postcode { get; set; }
        public int? ie_region { get; set; }
        public string user_country { get; set; }
        public string user_tel { get; set; }
        public string user_mobile { get; set; }
        public string user_email { get; set; }
        public int? user_type { get; set; }
        public DateTime? user_created { get; set; }
        public DateTime? date_sent { get; set; }
        public double? longitude { get; set; }
        public double? latitude { get; set; }
        public string ip_address { get; set; }
        public int? dealer_id { get; set; }
        public int? lead_source { get; set; }
        public int? sales_status { get; set; }

        public virtual ICollection<Web_product_new> Products { get; set; }
    }
}
