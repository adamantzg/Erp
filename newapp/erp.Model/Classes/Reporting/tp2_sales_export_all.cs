using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class tp2_sales_export_all
    {
        
        [Key]
        public int linenum { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Product_category { get; set; }
        public string Client { get; set; }
        public string PO { get; set; }
        public double? Qty { get; set; }
        public double? GBP_value { get; set; }
        public DateTime? ETA_1_week {get;set;}
        public double? USD_cost { get; set; }
        public double? GBP_cost { get; set; }
        public double? EUR_cost { get; set; }
        public string category1 { get; set; }
        public int month22 { get; set; }
    }
}
