
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{

    public enum DistributorSalesValueType
    {
        Sales = 0,
        Stock
    }
	
	public partial class Distributor_sales
	{
		public int id { get; set; }
		public int brand_id { get; set; }
		public int distributor_id { get; set; }
		public int month21 { get; set; }
		public double value { get; set; }
        public DistributorSalesValueType value_type { get; set; }

        public Brand Brand { get; set; }
        public Company Distributor { get; set; }
	
	}
}	
	