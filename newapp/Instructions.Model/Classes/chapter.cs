
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Instructions.Model
{
	
	public class Chapter
	{
		public int chapter_id { get; set; }
		public int? manual_id { get; set; }
		public string title { get; set; }
		public int? sequence { get; set; }

        public string manual_title { get; set; }

        public List<Section> Sections { get; set; }
	
	}
}	
	