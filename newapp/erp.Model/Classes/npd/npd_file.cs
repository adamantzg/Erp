
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Npd_file
	{
		public int file_id { get; set; }
		public int? npd_id { get; set; }
		public string filename { get; set; }
        public int? filetype_id { get; set; }
	}
}	
	