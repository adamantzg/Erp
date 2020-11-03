
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("exchange_rates")]
	public partial class Exchange_rates
	{
        [Key]
		public int month21 { get; set; }
		public double? usd_gbp { get; set; }
		public double? commission_rate { get; set; }
		public double? eur_gbp { get; set; }
	
	}
}	
	