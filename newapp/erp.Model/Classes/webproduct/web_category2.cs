using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Web_category
    {
        public int? ChildCount { get; set; }
        public bool ImageExists { get; set; }
        public int? ProductCount { get; set; }

        public ICollection<Web_category> Children { get; set; }
        public virtual Web_category Parent { get; set; }
        public List<Web_product_new> Products { get; set; }
        public List<Web_category_translate> Translations { get; set; }
        public int? new_flag { get; set; }
    }
}
