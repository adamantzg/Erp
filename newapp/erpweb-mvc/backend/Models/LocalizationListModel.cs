using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using asaq2.Model;

namespace asaq2back.Models
{
    public class LocalizationListModel
    {
        public List<SiteEdit> SiteEdits { get; set; }

        public int? brand_id { get; set; }
        public string lang { get; set; }
        public List<WebProduct> Products_Original { get; set; }
        public List<WebProduct> Products { get; set; }

        public List<BrandCategory> Categories_Original { get; set; }
        public List<BrandCategory> Categories { get; set; }
        public List<BrandSubCategory> SubCategories_Original { get; set;}
        public List<BrandSubCategory> SubCategories { get; set; }
        public List<Brand_categories_sub_sub> SubsubCategories_Original { get; set; }
        public List<Brand_categories_sub_sub> SubsubCategories { get; set; }
        public List<BrandGroup> BrandGroups_Original { get; set; }
        public List<BrandGroup> BrandGroups { get; set; }
        public List<WebProduct> WebProducts_Original { get; set; }
        public List<WebProduct> WebProducts { get; set; }

    }
}