
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Navigation_item_permission
	{
		public int id { get; set; }
		public int? navigation_item_id { get; set; }
		public int? role_id { get; set; }
		public int? user_id { get; set; }
		public bool? remove { get; set; }

        public virtual Navigation_item NavigationItem { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
	
	}
}	
	