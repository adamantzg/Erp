
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Instructions.Model
{
	
	public class Detail
	{
		public int detail_id { get; set; }
		public int? section_id { get; set; }
		public string detail { get; set; }
		public int? sequence { get; set; }
		public int? flag_id { get; set; }
        public string section_heading { get; set; }

        public Flag Flag { get; set; }
	
	}
}	
	