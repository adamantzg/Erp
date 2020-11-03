
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Admin_pages_new : IAdminpages
	{
        [Key]
		public int page_id { get; set; }
		public int? parent_id { get; set; }
		public string page_title { get; set; }
		public int? page_type { get; set; }
		public string notes { get; set; }
		public string page_URL { get; set; }
		public string parameter1 { get; set; }
		public string parameter1_value { get; set; }
		public string URL_value { get; set; }
		public bool? hide_menu { get; set; }
		public string path { get; set; }
        public string icon { get; set; }

        public List<User> Users { get; set; }

        public List<Admin_pages_new> Children { get; set; }
	}
}	
	