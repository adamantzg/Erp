
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Containercalculation_order_product
	{
		public int id { get; set; }
		public int? calculation_id { get; set; }
		public int? mast_id { get; set; }
		public double? sqm { get; set; }
		public double? cbm { get; set; }
        public double? qty { get; set; }

        public Containercalculation_order Calculation { get; set; }

        public Mast_products MastProduct { get; set; }

        public double? Volume
        {
            get
            {
                return cbm > 0 ? cbm : sqm * 2.27;
            }
        }

        public double? CorrectiveFactorPerUnit
        {
            get
            {
                var containerVolume = Calculation?.ContainerType?.Volume ?? 0;
                var quantity = qty ?? 0;
                var calculationPercentage = Calculation?.Percentage ?? 0;
                if (containerVolume == 0 || quantity == 0 || calculationPercentage == 0)
                    return 1;
                return ((calculationPercentage - 100 * 1.0) / 100 )* ((Volume / containerVolume)) / qty;
            }
        }

        
	
	}
}	
	