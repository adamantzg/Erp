using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Whitebook_template_category
    {
        public int template_id { get; set; }
        public int category_id { get; set; }
        public int? sequence { get; set; }
        //public ICollection<Whitebook_category> WhitebookCategories {get;set;}
        public Whitebook_template WhitebookTemplate { get; set; }
        public Whitebook_category WhitebookCategory { get; set; }
    }
}
