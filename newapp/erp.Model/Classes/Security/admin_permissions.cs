
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Admin_permissions
	{
		public int permission_id { get; set; }
		public int? userid { get; set; }
		public int? cusid { get; set; }
		public int? agent { get; set; }
		public int? clientid { get; set; }
		public int? returns { get; set; }
		public int? processing { get; set; }
		public int? feedbacks { get; set; }

        public virtual User User { get; set; }
        public virtual Company Company { get; set; }
	
	}
}	
	