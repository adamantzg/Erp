
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Feedback_type
	{
	    public const int FeedbacktypeIt = 1;
        public const int ItFeedback = 6;
        public const int Ca = 7;
        public const int Qa = 8;

		public int type_id { get; set; }
		public string typename { get; set; }
	
	}
}	
	