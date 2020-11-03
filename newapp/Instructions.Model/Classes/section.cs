
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Instructions.Model
{
	
	public class Section
	{
		public int section_id { get; set; }
		public int? chapter_id { get; set; }
        public string chapter_title { get; set; }
		public string heading { get; set; }
		public string image { get; set; }
		public int? sequence { get; set; }

        
        public List<Detail> Details { get; set; }
	}
}	
	