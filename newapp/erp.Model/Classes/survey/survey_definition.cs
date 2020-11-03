
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_definition
	{
		public int surveydef_id { get; set; }
		public string title { get; set; }
		public DateTime? datecreated { get; set; }
        public string ViewName { get; set; }
        public string result_title { get; set; }
        public string result_viewname { get; set; }

        public List<Survey_question> Questions { get; set; }
        


	
	}
}	
	