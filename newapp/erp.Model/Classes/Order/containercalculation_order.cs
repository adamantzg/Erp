
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Containercalculation_order
	{
		public int id { get; set; }
		public int? orderid { get; set; }
		public double? fillfactor { get; set; }
		public int? container_type_id { get; set; }
		public string bathsExceptions { get; set; }
		public string mirrors { get; set; }
		public string factories { get; set; }
		public string productsForRestriction { get; set; }
		public double? sqmcont2 { get; set; }
		public double? full_containers1 { get; set; }
		public double? bath_adjustment2 { get; set; }
		public double? DPceramic { get; set; }
        public double? hardcode_multiple { get; set; }

        public Order_header Order { get; set; }
        public Container_types ContainerType { get; set; }
        public List<Containercalculation_order_product> Products { get; set; }

        public double? Percentage {
            get
            {
                var perc = ((sqmcont2 + full_containers1) - (0.088 * bath_adjustment2) + DPceramic) ?? 0;
                perc =  Math.Round(perc / 0.925 * 100, 0);
                return  Math.Round(perc / (hardcode_multiple ?? 1.0), 0);
            }
        }

        
        public double? TotalVolume
        {
            get
            {
                return Products?.Sum(p => p.cbm > 0 ? p.cbm : p.sqm * ContainerType?.height * 1.0 / 1000);
            }
        }

    }
}	
	