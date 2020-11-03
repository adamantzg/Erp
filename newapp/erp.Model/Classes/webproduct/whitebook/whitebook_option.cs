
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public partial class Whitebook_option
	{
		public int id { get; set; }
		public int? group_id { get; set; }
		public string name { get; set; }
		public int? sequence { get; set; }
        public string override_image { get; set; }
        public int? override_id { get; set; }
        public int? parent_option { get; set; }

        public virtual Whitebook_option_group Group { get; set; }
        public virtual List<Whitebook_option> ChildOptions { get; set; }
        public virtual Whitebook_option Parent { get; set; }

    }
}
