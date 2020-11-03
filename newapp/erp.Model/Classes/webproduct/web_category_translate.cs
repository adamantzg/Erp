
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Web_category_translate
	{
        [Key]
        [Column(Order = 1)]
        public int category_id { get; set; }
        [Key]
        [Column(Order = 2)]
        public int language_id { get; set; }
		public string name { get; set; }
		public string alternate_name { get; set; }
		public string path { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public string pricing_note { get; set; }
		public string group { get; set; }
	
	}
}	
	