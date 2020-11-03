using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using company.Common;

namespace erp.Model
{
    public partial class Inspection_v2_line
    {
        public Order_lines OrderLine { get; set; }
        public Inspection_v2 Inspection { get; set; }
        public virtual List<Inspection_v2_loading> Loadings { get; set; }
        public virtual List<Inspection_v2_line_rejection> Rejections { get; set; }
        public virtual List<Inspection_v2_line_si_details> SiDetails { get; set; }

        
        public virtual Cust_products Product { get; set; }

        //[NotMapped]
        public virtual List<Inspection_v2_image> Images { get; set; }

        public int? ComputedQty
        {
            get
            {
                if (qty == null)
                    return OrderLine.IfNotNull(ol => Convert.ToInt32(ol.orderqty));
                return qty;
            }
        }
    }
}
