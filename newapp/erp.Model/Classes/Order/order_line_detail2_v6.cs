using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    public class Order_line_detail2_v6
    {
        
        [Key]
        public int linenum { get; set; }
        public int orderid { get; set; }
        public DateTime? linedate { get; set; }
        public DateTime? po_req_etd { get; set;}
        public DateTime? orderdate { get; set; }
        public string customer_code { get; set; }
        public int Month22 { get; set; }
        public double orderqty { get; set; }
        public double orig_orderqty { get; set; }
        public int userid1 { get; set; }
        public int? container_type { get; set; }
        public string custpo { get; set; }
        
        [NotMapped]
        public int counted_orders { get; set; }
        [NotMapped]
        public string week_day { get; set; }
        [NotMapped]
        public int completed_delivered_lines { get; set; }
        [NotMapped]
        public int ordered_lines { get; set; }
        [NotMapped]
        public double line_fill_rate { get; set; }
        [NotMapped]
        public int lead_days { get; set; }

        [NotMapped]
        public Mast_products MastProduct { get; set; }
        [NotMapped]
        public Company Client { get; set; }
        [NotMapped]
        public Company Factory { get; set; }
    }
}
