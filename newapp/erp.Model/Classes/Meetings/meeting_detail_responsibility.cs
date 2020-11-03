
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Meeting_detail_responsibility
	{

		public int meeting_details_id { get; set; }
		public int useruserid { get; set; }
		public DateTime? expected_resolution_date { get; set; }

        public virtual Meeting_detail Detail { get; set; }
        public virtual User User { get; set; }
	
	}
}	
	