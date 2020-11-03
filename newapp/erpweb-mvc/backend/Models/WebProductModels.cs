using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class WebProductListModel
    {
        public List<Brand> Brands { get; set; }
        public List<Web_site> WebSites { get; set; }
        public List<Web_category> WebCategories { get; set; }
        public List<Image_type> ImageTypes { get; set; }
    }

    public class WebProductEditModel
    {
        public Web_product_new WebProduct { get; set; }
        public EditMode EditMode { get; set; }
        public List<Web_site> WebSites { get; set; }
        public List<Web_product_part> Parts { get; set; }
        public List<Web_product_pressure> Pressures { get; set; }
        public List<Web_product_file_type> FileTypes { get; set; }
        public jsTreeNode[] Categories { get; set; }
        public jsTreeNode[] RelatedSelected { get; set; }
        public List<Sale> SalePeriods { get; set; }
        public int Page { get; set; }
    }

    public class WebCategoriesEditModel
    {
        public List<Web_site> WebSites { get; set; }
        public List<Image_type> ImageTypes { get; set; }
    }

    public class WebProductExportModel
    {
        public List<CheckBoxItem> SitesCheck { get; set; }
        public List<Web_site> Sites { get; set; }
        public List<Web_product_new> Products { get; set; }
        public List<Web_product_file_type> FileTypes { get; set; }
    }
}