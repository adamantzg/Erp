
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
	
	public class External_user
	{
		public int user_id { get; set; }

        [Required(ErrorMessageResourceName="ExternalUser_Validation_surname", ErrorMessageResourceType=typeof(Properties.Resources))]
		public string surname { get; set; }
        [Required(ErrorMessageResourceName="ExternalUser_Validation_firstname", ErrorMessageResourceType=typeof(Properties.Resources))]
		public string firstname { get; set; }
        [Required(ErrorMessageResourceName="ExternalUser_Validation_username", ErrorMessageResourceType=typeof(Properties.Resources))]
		public string username { get; set; }
		public string password { get; set; }
		public string email { get; set; }
		public string postcode { get; set; }
		public string address1 { get; set; }
		public string address2 { get; set; }
		public string address3 { get; set; }
		public string address4 { get; set; }
		public string address5 { get; set; }
		public string country { get; set; }
		public int? city { get; set; }
		public string tel { get; set; }
		public string mobile { get; set; }
		public int? company_id { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_userid { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_userid { get; set; }
        public string creator { get; set; }
        public string editor { get; set; }

        public string fullname
        {
            get
            {
                return firstname + " " + surname;
            }
        }
	
	}
}	
	