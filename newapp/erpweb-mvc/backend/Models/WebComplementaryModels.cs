using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class WebComplementaryModels
    {
        public List<erp.Model.Web_site> WebSites { get; set; }

        public List<erp.Model.Brand> Brands { get; set; }

        public List<erp.Model.Image_type> ImageTypes { get; set; }

       // public IEnumerable<asaq2.Model.Web_category> Categories { get; set; }

        public List<erp.Model.Web_category> SubCategories { get; set; }

        public List<erp.Model.Web_category> SubSubCategories { get; set; }

        public List<erp.Model.Web_product_new> Products { get; set; }

        public ICollection<erp.Model.web_product_new_related> Complementary { get; set; }

        public erp.Model.Web_product_new WebProduct { get; set; }

        public jsTreeNode[] RelatedSelected { get; set; }

        public EditMode EditMode { get; set; }

        public jsTreeNode[] Categories { get; set; }

        public IEnumerable<erp.Model.Web_category> CategoriesII { get; set; }
    }

    public class ProductsComplementary
 
	{

        public List<erp.Model.Web_product_new> Products { get; set; }

        public int Category { get; set; }
    }

    public class EditComplementaryModel
    {
        //public List<asaq2.Model.Web_site> WebSite { get; set; }
        public erp.Model.Web_product_new WebProduct { get; set; }



        public List<erp.Model.Web_site> WebSites { get; set; }

        public jsTreeNode[] RelatedSelected { get; set; }

        public jsTreeNode[] Categories { get; set; }

        public EditMode Edit { get; set; }

        public EditMode EditMode { get; set; }
    }
}