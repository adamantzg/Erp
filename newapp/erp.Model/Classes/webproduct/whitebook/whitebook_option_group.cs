
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public partial class Whitebook_option_group
	{
		public int id { get; set; }
		public string name { get; set; }

        public bool HasChildOptions {
            get
            {
                return Options?.Any(o => o.ChildOptions != null && o.ChildOptions.Count > 0) == true;
            }
        }
        public List<Whitebook_option> Options { get; set; }

	}
}
