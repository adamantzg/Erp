
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Form_submission
	{
		public int id { get; set; }
		public DateTime? dateCreated { get; set; }
		public int? user_id { get; set; }
        public int? form_id { get; set; }

        public Form Form { get; set; }
        public virtual User User { get; set; }
        public virtual List<Form_submission_answer> Answers { get; set; }
	
	}
}	
	