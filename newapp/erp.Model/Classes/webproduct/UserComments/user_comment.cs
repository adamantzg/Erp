
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class User_comment
	{
		public int comment_id { get; set; }
		public int type_id { get; set; }
		public DateTime? date { get; set; }
		public int? user_id { get; set; }
		public string text { get; set; }
        public int? dealer_id { get; set; }

        public User User { get; set; }
	
	}
}	
	