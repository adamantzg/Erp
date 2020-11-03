using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using asaq2.Model;

namespace asaq2back.Models
{
    public class LocalizationEditModel
    {
        public BrandCategory Category { get; set; }
        public Brand_categories_translate LocCategory { get; set; }
        public BrandSubCategory Subcategory { get; set; }
        public Brand_categories_sub_translate LocSubcategory { get; set; }
        public Brand_categories_sub_sub SubsubCategory { get; set; }
        public Brand_categories_sub_sub_translate LocSubsubCategory { get; set; }
        public BrandGroup Brandgroup { get; set; }
        public Brand_grouping_translate LocBrandGroup { get; set; }
        public WebProduct Product { get; set; }
        public Web_products_translate LocProduct { get; set; }
        public List<Cust_products_translate> LocComponents { get; set; }
    }
}