
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_result_answer
	{
		public int answer_id { get; set; }
		public int? question_id { get; set; }
		public int? value { get; set; }
		public int? result_id { get; set; }
		public string comment { get; set; }

        public Survey_result Result { get; set; }
        public List<Survey_result_answer_option> Options { get; set; }
	
	}
}	
	