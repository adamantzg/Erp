
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_loading
	{
		public int id { get; set; }
		public int? insp_line { get; set; }
		public int? container_id { get; set; }
		public int? full_pallets { get; set; }
		public int? loose_load_qty { get; set; }
		public int? mixed_pallet_qty { get; set; }
		public int? mixed_pallet_qty2 { get; set; }
		public int? mixed_pallet_qty3 { get; set; }
		public int? area_id { get; set; }
        public int? qty_per_pallet { get; set; }

    }
}	
	