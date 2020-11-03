using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace asaq2.Model
{
    [NotMapped]
    public class dam_brand_visits
    {
        public int id { get; set; }
        public int brand_id { get; set; }
        public DateTime date { get; set; }
        public int register { get; set; }
        public string session { get; set; }
        public string ip_address { get; set; } 
    }
}
