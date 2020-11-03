
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{

    public partial class Whitebook_template
    {
        public const int WhiteBookProductOptionView= 0;
        public const int SiteProductOptionView = 1;

        public int id { get; set; }
        public string name { get; set; }
        public string subtitle { get; set; }
        public string whitebook_notes { get; set; }
        public int? default_web_unique { get; set; }
        public int? view_type { get; set; }
        [NotMapped]
        public int CountProducts { get; set; }
        [NotMapped]
        public bool Pending { get; set; }
        [NotMapped]
        public List<Web_product_new> Products { get; set; }
		[NotMapped]
        public Whitebook_template_category WhitebookTemplateCategory
		{
			get
			{
				return Categories?.FirstOrDefault();
			}
		}
        public List<Whitebook_template_optiongroup> OptionGroups { get; set; }
		public List<Whitebook_template_category> Categories { get; set; }

        [NotMapped]
        public Web_product_new Product { get; set; }

	}
}
