
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Issue_comments
	{
		public int comment_id { get; set; }
		public int? issue_id { get; set; }
		public string comment_text { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }

        public string creator { get; set; }
        public string editor { get; set; }

        public List<Issuecomment_files> Files { get; set; }
	
	}
}	
	