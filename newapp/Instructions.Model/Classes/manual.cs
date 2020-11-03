
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Instructions.Model
{
	
	public class Manual
	{
		public int manual_id { get; set; }
		public string title { get; set; }
		public string logo { get; set; }
		public DateTime? create_date { get; set; }

        public List<Chapter> Chapters { get; set; }
	
	}
}	
	