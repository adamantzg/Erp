using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    [Table("component_table")]
    public partial class ProductComponent
    {
        [Key]
        public int component_id { get; set; }
        public string component_name { get; set; }
        public int? component_qty { get; set; }
        public string component_description { get; set; }
        public string component_comments { get; set; }
        public double? component_price_euro { get; set; }
        public double? component_price_dollar { get; set; }
        public double? component_price_pound { get; set; }
        public int? component_factory { get; set; }
        public int? component_user_id { get; set; }
        public string component_createdate { get; set; }
        public int? component_alarm_qty { get; set; }
        public int? show_on_invoice { get; set; }
        public DateTime? cutoff_ETD_date { get; set; }
        public DateTime? end_etd_date { get; set; }

        public virtual List<MastProduct_Component> MastProductComponents {get;set;}
	
	}
}	
	