
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_loading_mixedpallet
	{
		public int id { get; set; }
		public int? loading_id { get; set; }
		public int? pallet_id { get; set; }
		public int? qty { get; set; }

        public Inspection_v2_loading Loading { get; set; }
        public Inspection_v2_mixedpallet Pallet { get; set; }
	
	}
}	
	