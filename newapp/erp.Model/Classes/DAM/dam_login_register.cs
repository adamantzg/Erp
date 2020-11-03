using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace asaq2.Model
{
    [NotMapped]
    public class DAM_login_register
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string ip_address { get; set; }
        public string location { get; set; }
        public DateTime login_date { get; set; }
        public int? accepted_terms { get; set; }
        public string country { get; set; }
    }
}
