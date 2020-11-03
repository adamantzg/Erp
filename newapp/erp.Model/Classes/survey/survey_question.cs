
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_question
	{
		public int question_id { get; set; }
		public int? surveydef_id { get; set; }
		public int? parent_id { get; set; }
		public int? question_type { get; set; }
		public string text { get; set; }
		public int? uitype { get; set; }
		public int? order { get; set; }
		public bool? comment { get; set; }
		public int? optionset_id { get; set; }
		public string commentlabel { get; set; }
        public string yaxislabel { get; set; }

        public List<Survey_question> Children { get; set; }
        public List<Survey_question_option> Options { get; set; }
	
	}
}	
	