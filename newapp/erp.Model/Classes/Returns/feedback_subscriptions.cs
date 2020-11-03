
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Feedback_subscriptions
	{
        [Key]
		public int subs_id { get; set; }
		public int? subs_returnid { get; set; }
		public int? subs_useruserid { get; set; }
		public int? subs_type { get; set; }
		public int? subs_leader { get; set; }

        public User User { get; set; }
        public virtual Returns Return { get; set; }
	
	}
}	
	