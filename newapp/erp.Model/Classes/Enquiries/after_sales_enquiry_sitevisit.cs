
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class After_sales_enquiry_sitevisit
	{
		public int sitevisit_id { get; set; }
		public int enquiry_id { get; set; }
		public string issue_cause { get; set; }
		public string issue_resolution { get; set; }
		public int contractor_id { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }
        public DateTime? datecompleted { get; set; }
        public string requestmessage { get; set; }
        public int status_id { get; set; }
        public string reference_number { get; set; }
        public DateTime? datevisited { get; set; }

        public string status_name { get; set; }
        public string contractor { get; set; }

        public virtual After_sales_enquiry Enquiry { get; set; }

        public List<After_sales_enquiry_sitevisit_files> Files { get; set; }
	
	}
}	
	