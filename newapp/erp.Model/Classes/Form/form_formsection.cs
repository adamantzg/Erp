
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Form_formsection
	{
        public int form_id { get; set; }
        public int section_id { get; set; }
		public int? sequence { get; set; }
	
        public virtual Form Form { get; set; }
        public virtual Form_section Section { get; set; }
	}
}	
	