
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class After_sales_enquiry_comment
	{
		public int comment_id { get; set; }
		public string comment_text { get; set; }
		public int respond_to { get; set; }
		public int? enquiry_id { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }

        public string creator { get; set; }
        public string editor { get; set; }
        public string respondto_name { get; set; }
        public CompanyType respondto_role { get; set; }
        public CompanyType creator_role { get; set; }

        public List<After_sales_enquiry_comment_files> Files { get; set; }
	
	}
}	
	