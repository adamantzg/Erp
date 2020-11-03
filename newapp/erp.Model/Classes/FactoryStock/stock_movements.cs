
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Stock_movements
	{
		public int id { get; set; }
		public DateTime? date { get; set; }
		public int? qty { get; set; }
		public int? mast_id { get; set; }
		public int? linenum { get; set; }
		public int? so_linenum { get; set; }
	
	}
}	
	