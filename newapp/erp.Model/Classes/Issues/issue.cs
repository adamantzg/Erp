
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using erp.Model.Properties;

namespace erp.Model
{
	
	public class Issue
	{
		public int issue_id { get; set; }
        [Required(ErrorMessageResourceName="IssueValidation_Title", ErrorMessageResourceType  = typeof(Resources))]
		public string title { get; set; }
		public string description { get; set; }
		public int? priority_id { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }
		public DateTime? duedate { get; set; }
		public int? status_id { get; set; }
		public string comment { get; set; }
        [Required(ErrorMessageResourceName="IssueValidaton_Area", ErrorMessageResourceType  = typeof(Resources))]
		public int? area_id { get; set; }
        public int? type_id { get; set; }
        public int? assigned_id { get; set; }

        public string creator { get; set; }
        public string status_text { get; set; }
        public string editor { get; set; }
        public string area_name { get; set; }
        public string priority_text { get; set; }
        public string type_name { get; set; }
        public string assignedto { get; set; }

        public List<Issue_category> Categories { get; set; }
        public List<Issue_files> Files { get; set; }
        public List<Issue_comments> Comments { get; set; }
        public List<Issue_subscription> Subscriptions { get; set; }

        public User UserAssignedTo { get; set; }
        

        public Issue()
        {
            priority_id = 2;    //Normal
            type_id = 1; //Bug
        }
	
	}
}	
	