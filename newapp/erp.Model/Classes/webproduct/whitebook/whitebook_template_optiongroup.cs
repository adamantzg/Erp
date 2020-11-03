
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Whitebook_template_optiongroup
	{
		public int template_id { get; set; }
		public int group_id { get; set; }
		public int? sequence { get; set; }

        public Whitebook_option_group Group { get; set; }
	
	}
}	
	