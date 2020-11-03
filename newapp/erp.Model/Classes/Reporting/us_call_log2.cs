using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
    
    public class us_call_log2
    {
        [Key]
        public int id { get; set; }
        public DateTime? US_datetime { get; set; }
        public string dealerid { get; set; }
        public string dealername { get; set; }
        public string state { get; set; }
        public string username { get; set; }
        public string note { get; set; }
        public int? ammara_ORDERS_YTD { get; set; }
        public int? CW_ORDERS_YTD { get; set; }
        public string caller_name { get; set; }
        public string alpha { get; set; }
        public string category { get;set;}
        public string type_name { get;set;}
    }
}
