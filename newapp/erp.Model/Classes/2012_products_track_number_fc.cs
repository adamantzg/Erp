
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class products_track_number_fc
	{
		public int producttrack_id { get; set; }
		public int? mastid { get; set; }
		public string track_number { get; set; }
		public int? orderid { get; set; }
		public string producttrack_date { get; set; }
	
	}
}	
	