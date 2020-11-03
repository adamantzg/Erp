
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_question_option
	{
		public int question_option_id { get; set; }
		public int? question_id { get; set; }
		public string option_text { get; set; }
		public int? option_value { get; set; }
	
	}
}	
	