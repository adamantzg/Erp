using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Login_history
    {
        [Key]
        public int? history_id { get; set; }
        public int? user_id { get; set; }
        public DateTime? login_date { get; set; }
        public string login_username { get; set; }
        public string login_country { get; set; }
        public string website { get; set; }
        public string ip_address { get; set; }
        public string pwd { get; set; }
        public string session_id { get; set; }


        public Company Company { get; set; }
        [NotMapped]
        public User User { get; set; }



    }
}
