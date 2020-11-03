
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Admin_pages : IAdminpages//: IPage
	{
        [Key]
		public int page_id { get; set; }
        [NotMapped]
        public int? parent_id { get; set; }
        [NotMapped]
        public string page_title { get; set; }
	    public int? page_type { get; set; }
		public string top_level { get; set; }
		public string sub_level { get; set; }
		public string sub_sub_level { get; set; }
		public string notes { get; set; }
		public string page_URL { get; set; }
		public string parameter1 { get; set; }
		public string parameter1_value { get; set; }
		public string URL_value { get; set; }
        public bool? hide_menu { get; set; }
        [NotMapped]
	    public string path { get; set; }

        public List<User> Users { get; set; }
	}
}	
	