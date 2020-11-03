
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Form_question_type
	{
		public int id { get; set; }
		public string name { get; set; }
		public int? default_render_id { get; set; }

        public virtual Form_question_rendermethod RenderMethod { get; set; }
	
	}
}	
	