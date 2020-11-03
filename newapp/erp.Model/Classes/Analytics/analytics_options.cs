
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Analytics_options
	{
        [Key]
		public int option_id { get; set; }
		public string option_name { get; set; }
        public string color_code { get; set; }
	
	}
}	
	