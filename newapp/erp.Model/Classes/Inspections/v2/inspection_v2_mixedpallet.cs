
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_mixedpallet
	{
		public int id { get; set; }
		public int? insp_id { get; set; }
		public string name { get; set; }
		public int? area_id { get; set; }

        public Inspection_v2 Inspection { get; set; }
        public Inspection_v2_area Area { get; set; }
	
	}
}	
	