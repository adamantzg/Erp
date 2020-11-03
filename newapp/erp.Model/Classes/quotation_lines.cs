
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Quotation_lines
	{
		public int header_id { get; set; }
		public int cprod_id { get; set; }

        public Quotation_header Header { get; set; }
        public Cust_products Product { get; set; }

        public double? FOB_Price_Pound
        {
            get
            {
                if (Product != null && Product.MastProduct != null)
                {
                    return Product.MastProduct.price_pound;
                }
                else
                    return null;
            }
        }

        public double? FOB_Price_Dollar
        {
            get
            {
                if (Product != null && Product.MastProduct != null)
                {
                    return Product.MastProduct.price_dollar;
                }
                else
                    return null;
            }
        }

        public int? QuantityPerContainer
        {
            get
            {
                if (Product != null && Product.MastProduct != null)
                {
                    return Product.MastProduct.units_per_40nopallet_gp;
                }
                else
                    return null;
            }
        }
	
	}
}	
	