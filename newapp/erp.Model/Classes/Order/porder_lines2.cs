using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Porder_lines
    {
        public virtual Porder_header Header { get; set; }
        //[NotMapped]
        public virtual Mast_products MastProduct { get; set; }

        public virtual Currencies Currency { get; set; }

        public double? RowPriceGBP
        {
            get { return (unitcurrency == 0 ? unitprice / 1.6 : unitcurrency == 2 ? unitprice / 1.2 : unitprice); }
        }

        
    }
}
