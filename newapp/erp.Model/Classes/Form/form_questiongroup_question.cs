
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Form_questiongroup_question
	{
		public int group_id { get; set; }
		public int question_id { get; set; }
		public int? sequence { get; set; }
        public int? numRepeat { get; set; }

        public virtual Form_question_group Group { get; set; }
        public virtual Form_question Question { get; set; }
	
	}
}	
	