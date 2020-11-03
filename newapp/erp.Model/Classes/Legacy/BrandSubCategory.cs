using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class SubCategoryDisplayType
    { 
        public const short ProductsNormalImages = 0;
        public const short ProductsDoubleHeightImages = 1;
        public const short SubSubCategoriesRadioButtons = 2;
        public const short SubSubCategoriesNormal = 4;
    }

    public class BrandSubCategory
    {
        [Key]
        public int brand_sub_id	{get;set;}
        public int? brand_cat_id {get;set;}
        public int? same_id	{get;set;}
        public string brand_sub_desc {get;set;}
        public string sub_description {get;set;}
        public string sub_details {get;set;}
        public string sub_image1 {get;set;}
        public int? image_width	{get;set;}
        public int? image_height {get;set;}
        public int? guarantee {get;set;}
        public int? seq {get;set;}
        public string group { get; set; }
        public short? display_type { get; set; }
        public virtual BrandCategory Category { get; set; }
        public int? option_component { get; set; }
        public string pricing_note { get; set; }
        public string sub_details_heading { get; set; }
        public double? sale_retail_percentage { get; set; }

        [NotMapped]
        public int ProductsWithOptionsCount { get; set; }
    }
}
