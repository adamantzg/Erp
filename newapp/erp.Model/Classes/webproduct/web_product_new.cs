
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public partial class Web_product_new
	{
		public int web_unique { get; set; }
		public string web_name { get; set; }
		public int web_site_id { get; set; }
		public string web_description { get; set; }
		public string web_code { get; set; }
		public string hi_res { get; set; }
		public string web_pic_notes { get; set; }
		public string web_details { get; set; }
		public int? web_seq { get; set; }
		public double? product_weight { get; set; }
		public double? bath_volume { get; set; }
		public string tech_finishes { get; set; }
		public string tech_product_type { get; set; }
		public string tech_construction { get; set; }
		public string tech_material { get; set; }
		public string tech_basin_size { get; set; }
		public string tech_overall_height { get; set; }
		public string tech_tap_holes { get; set; }
		public string tech_fixing { get; set; }
		public double? web_auto { get; set; }
		public int? guarantee { get; set; }
		public int? brand_group { get; set; }
		public string gold_code { get; set; }
		public int? product_gold_code { get; set; }
		public int? web_status { get; set; }
		public int? overflow_class { get; set; }
		public double? overflow_rate { get; set; }
		public string combination_comments { get; set; }
		public string tech_water_volume_note { get; set; }
		public int? image_gallery { get; set; }
		public int? parent_id { get; set; }
		public string option_name { get; set; }
		public double? override_length { get; set; }
		public double? override_width { get; set; }
		public double? override_height { get; set; }
		public int? sale_on { get; set; }
		public DateTime? datecreated { get; set; }
		public int? created_by { get; set; }
		public DateTime? datemodified { get; set; }
		public int? modified_by { get; set; }
		public int? legacy_id { get; set; }
		public int? batch_no { get; set; }
		public int? batch_id { get; set; }
		public int? design_template { get; set; }
		public string sub_title_1 { get; set; }
		public int? glass_thickness { get; set; }
		public int? adjustment { get; set; }
		public int? whitebook_batch { get; set; }
		public string whitebook_title { get; set; }
		public string whitebook_description { get; set; }
		public string whitebook_material { get; set; }
		public string whitebook_notes { get; set; }
		public string web_code_override { get; set; }
        public int? new_product_flag { get; set; }
        public int? show_component_dimensions { get; set; }
        public int? show_component_weights { get; set; }
        public int? option_header_override { get; set; }
        public string illustrated_notes { get; set; }
        public string compatible_products_notes { get; set; }
        public int? template_id_link { get; set; }
    }
}
