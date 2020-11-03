
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Freightcost
	{
		public int id { get; set; }
		public int? location_id { get; set; }
		public int? market_id { get; set; }
		public int? container_id { get; set; }
		public double? value { get; set; }

		public Location Location { get; set; }
		public Market Market { get; set; }
		public Container_types ContainerType { get; set; }
	
	}
}	
	