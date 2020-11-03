
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Language
	{
        [Key]
		public int language_id { get; set; }
		public string code { get; set; }
		public string name { get; set; }

        public List<Countries> Countries { get; set; }
	
	}
}	
	