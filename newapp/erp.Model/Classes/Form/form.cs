
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Form
	{
		public int id { get; set; }
		public string title { get; set; }
	
        public virtual List<Form_formsection> Sections { get; set; }
        public virtual List<Form_submission> Submissions { get; set; }
	}
}	
	