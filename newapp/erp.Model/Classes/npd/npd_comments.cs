
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Npd_comments
	{
		public int comments_id { get; set; }
		public int? npd_id { get; set; }
		public int? comments_from { get; set; }
		public string comments { get; set; }
		public DateTime? comments_date { get; set; }
        public int? type { get; set; }

        public User FromUser { get; set; }

        public List<Npd_comments_files> Files { get; set; }

	    public Npd_comments()
	    {
	        type = 1;   //1-external 2-internal
	    }
	}
}	
	