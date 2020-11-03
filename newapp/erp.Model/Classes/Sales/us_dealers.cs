using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    [Table("us_dealers")]
    public partial class Us_dealers
    {
        [Key]
        public string customer { get; set; }
        public string alpha { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string address5 { get; set; }
        public string town_city { get; set; }
        public string county { get; set; }
        public string state_region { get; set; }
        public string iso_country_code { get; set; }
        public string country { get; set; }
        public bool? isInternal { get; set; }
		public int? rowid { get; set; }

        public List<Sales_orders> Sales_orders { get; set; }
        public List<Us_call_log> CallLogs { get; set; }
    }
}
