using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Whitebook_category
    {
        public int id { get; set; }
        public string category_name { get; set; }
        public int? parent_id { get; set; }
        public int? seq { get; set; }
        public string image { get; set; }
        public int? brand_id { get; set; }
        public int? nosub { get; set;  }
        //public virtual Whitebook_template_category WhitebookTemplateCategory { get; set; }
        public virtual ICollection<Whitebook_template_category> WhitebookTemplateCategory { get; set; }

        public Whitebook_category Parent { get; set; }
        public List<Whitebook_category> Children { get; set; }
    }
}
