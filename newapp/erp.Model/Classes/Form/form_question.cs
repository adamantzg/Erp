
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Form_question
	{
		public int id { get; set; }
		public string question_text { get; set; }
        public string description { get; set; }
		public int? question_type { get; set; }
		public string color { get; set; }
		public int? choice_group_id { get; set; }
		public int? render_id { get; set; }
		public bool? has_comment { get; set; }
		public string comment_label { get; set; }
        public bool? label_editable { get; set; }

        public virtual Form_question_type QuestionType { get; set; }
        public virtual Form_choice_group ChoiceGroup { get; set; }
        public virtual Form_question_rendermethod QuestionRenderMethod { get; set; }
        public virtual List<Form_questiongroup_question> QuestionGroups { get; set; }
	
	}
}	
	