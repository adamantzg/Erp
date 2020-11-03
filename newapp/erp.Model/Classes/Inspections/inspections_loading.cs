
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Inspections_loading
	{
        [Key]
		public int loading_line_unique { get; set; }
		public int? insp_line_unique { get; set; }
		public int? container { get; set; }
		public int? qty_per_pallet { get; set; }
		public int? full_pallets { get; set; }
		public int? loose_load_qty { get; set; }
		public int? mixed_pallet_qty { get; set; }
		public int? mixed_pallet_qty2 { get; set; }
		public int? mixed_pallet_qty3 { get; set; }

        public string insp_client_ref { get; set; }
        public string insp_custpo { get; set; }

        public Inspection_lines_tested LineTested { get; set; }

        [NotMapped]
        public Containers Container { get; set; }
	
	}
}	
	