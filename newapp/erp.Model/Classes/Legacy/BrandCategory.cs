using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    /// <summary>
    /// BrandCategory class - maps brand_categories table, property names are identical to field names to make mapping simpler
    /// </summary>
    public class BrandCategory
    {
        [Key]
        public int brand_cat_id	{get;set;}
        public string brand_cat_desc {get;set;}	
        public int? brand {get;set;}	
        public int? unit_ordering {get;set;}	
        public string web_description {get;set;}	
        public string brand_cat_image {get;set;}	
        public int? image_width {get;set;}	
        public int? image_height {get;set;}	
        public int? web_seq {get;set;}
        public int? unique_ordering { get; set; }
        public string why_so_good { get; set; }
        public string why_so_good_title { get; set; }
        public double? sale_retail_percentage { get; set; }
        public int? group_id { get; set; }

        [NotMapped]
        public int childcount { get; set; }

        public ICollection<BrandSubCategory> Subcategories { get; set; }

        public virtual Brand_categories_group Group { get; set; }


    }
}
