using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using erp.Model.Properties;

namespace erp.Model
{
    public class BrochureRequest
    {
        
        public int user_id { get; set; }
        public Nullable<int> distributor { get; set; }
        public string user_surname { get; set; }
        public string user_firstname { get; set; }
        public string user_title { get; set; }
        public string user_address1 { get; set; }
        public string user_address2 { get; set; }
        public string user_address3 { get; set; }
        public string user_address4 { get; set; }
        public string user_address5 { get; set; }
        public string postcode { get; set; }
        public Nullable<int> ie_region { get; set; }
        public string user_country { get; set; }
        public string user_tel { get; set; }
        public string user_mobile { get; set; }
        public string user_email { get; set; }
        public Nullable<int> user_type { get; set; }
        public Nullable<System.DateTime> user_created { get; set; }
        public Nullable<System.DateTime> date_sent { get; set; }
        public Nullable<double> longitude { get; set; }
        public Nullable<double> latitude { get; set; }
        public string ip_address { get; set; }
        public Nullable<int> dealer_id { get; set; }
        public Nullable<int> lead_source { get; set; }
        public Nullable<int> sales_status { get; set; }
        public string user_password { get; set; }
        public DateTime? email_date_sent { get; set; }
        public int? brand_id { get; set; }
        public int? contact_optout { get; set; }

        public BrochureRequest()
        {
            user_created = DateTime.Now;
        }
        public Brand Brand { get; set; }
        public Dealer Dealer { get; set; }

    }
}
