using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    [NotMapped]
    public class Dealer_displays_activity_bydateandbrand
    {
        public string date { get; set; }
        public int brand_id { get; set; }
        public int qty { get; set; }
        public int web_unique { get; set; }

    }

    [NotMapped]
    public class Dealer_displays_activitiy_bydateandbrand_deatils
    {
        public string web_code { get; set; }
        public string web_name { get; set; }
        public int web_unique { get; set; }
        public int qty { get; set; }
    }
}
