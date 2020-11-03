
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_optionset
	{
		public int option_set_id { get; set; }
		public string name { get; set; }

        public List<Survey_optionset_option> Options { get; set; }
	
	}
}	
	