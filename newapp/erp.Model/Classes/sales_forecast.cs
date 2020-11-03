
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
    public enum ForecastType
    {
        Normal=0,
        Contract=1
    }
	
	public class Sales_forecast
	{
        [Key]
		public int sales_unique { get; set; }
		public int? cprod_id { get; set; }
		public int? sales_qty { get; set; }
		public int? month21 { get; set; }
        [NotMapped]
		public int? sales_qty_c { get; set; }
        
        public virtual Cust_products Product { get; set; }
        [NotMapped]
        public ForecastType Type { get; set; }

        public Sales_forecast()
        {
            Type = ForecastType.Normal;
        }
	
	}

    
}	
	