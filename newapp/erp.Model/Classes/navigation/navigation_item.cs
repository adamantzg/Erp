
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Navigation_item
	{
		public int id { get; set; }
		public int? parent_id { get; set; }
		public string text { get; set; }
		public string url { get; set; }
        public string image_url { get; set; }

        public List<Navigation_item_permission> Permissions { get; set; }
        //public virtual Navigation_item Parent { get; set; }

        [NotMapped]
        public bool Active { get; set; }

        //public List<Navigation_item> ChildItems { get; set; }
	
	}
}	
	