using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model
{
    /// <summary>
    /// Maps web_product table, not all properties are mapped because it would take time, just needed at the moment
    /// Web components are mapped as collection although they are written as fixed 15 foreign keys
    /// </summary>
    public class WebProduct
    {
        public int web_unique { get; set; }
        public string web_name { get; set; }
        public string web_site { get; set; }
        public string web_description { get; set; }
        public string web_code { get; set; }
        public Nullable<int> web_category { get; set; }
        public Nullable<int> web_sub_category { get; set; }
        public Nullable<int> web_sub_sub_category { get; set; }
        public string web_image1 { get; set; }
        public string web_image1b { get; set; }
        public string web_image1d { get; set; }
        public string web_image2 { get; set; }
        public string web_image3 { get; set; }
        public string web_image3b { get; set; }
        public string web_image3c { get; set; }
        public string web_image4 { get; set; }
        public string hi_res { get; set; }
        public Nullable<int> image_width { get; set; }
        public Nullable<int> image_widthb { get; set; }
        public Nullable<int> image_widthd { get; set; }
        public Nullable<int> image_height { get; set; }
        public Nullable<int> image_heightb { get; set; }
        public Nullable<int> image_heightd { get; set; }
        public string web_pic_notes { get; set; }
        public Nullable<double> web_price { get; set; }
        public string web_details { get; set; }
        public Nullable<int> web_component1 { get; set; }
        public Nullable<int> web_component2 { get; set; }
        public Nullable<int> web_component3 { get; set; }
        public Nullable<int> web_component4 { get; set; }
        public Nullable<int> web_component5 { get; set; }
        public Nullable<int> web_component6 { get; set; }
        public Nullable<int> web_component7 { get; set; }
        public Nullable<int> web_component8 { get; set; }
        public Nullable<int> web_component9 { get; set; }
        public Nullable<int> web_component10 { get; set; }
        public Nullable<int> web_component11 { get; set; }
        public Nullable<int> web_component12 { get; set; }
        public Nullable<int> web_component13 { get; set; }
        public Nullable<int> web_component14 { get; set; }
        public Nullable<int> web_component15 { get; set; }
        public Nullable<int> link1 { get; set; }
        public Nullable<int> link2 { get; set; }
        public Nullable<int> link3 { get; set; }
        public Nullable<int> link4 { get; set; }
        public Nullable<int> link5 { get; set; }
        public Nullable<int> link6 { get; set; }
        public Nullable<int> web_seq { get; set; }
        public Nullable<double> product_weight { get; set; }
        public Nullable<double> bath_volume { get; set; }
        public string tech_finishes { get; set; }
        public string tech_product_type { get; set; }
        public string tech_construction { get; set; }
        public string tech_material { get; set; }
        public string tech_basin_size { get; set; }
        public string tech_overall_height { get; set; }
        public string tech_tap_holes { get; set; }
        public string tech_fixing { get; set; }
        public string tech_compliance1 { get; set; }
        public string tech_compliance2 { get; set; }
        public string tech_compliance3 { get; set; }
        public string tech_compliance4 { get; set; }
        public string tech_compliance5 { get; set; }
        public string tech_additional1 { get; set; }
        public string tech_additional2 { get; set; }
        public string tech_additional3 { get; set; }
        public string tech_additional4 { get; set; }
        public string tech_additional5 { get; set; }
        public string tech_additional6 { get; set; }
        public string tech_additional7 { get; set; }
        public string tech_additional8 { get; set; }
        public string tech_additional9 { get; set; }
        public string tech_additional10 { get; set; }
        public string tech_additional11 { get; set; }
        public Nullable<double> web_auto { get; set; }
        public Nullable<int> guarantee { get; set; }
        public string bar01 { get; set; }
        public string bar02 { get; set; }
        public string bar05 { get; set; }
        public string bar10 { get; set; }
        public string bar20 { get; set; }
        public string bar30 { get; set; }
        public Nullable<double> handset02 { get; set; }
        public Nullable<double> handset05 { get; set; }
        public Nullable<double> handset10 { get; set; }
        public Nullable<double> handset20 { get; set; }
        public Nullable<double> handset30 { get; set; }
        public Nullable<double> rose02 { get; set; }
        public Nullable<double> rose05 { get; set; }
        public Nullable<double> rose10 { get; set; }
        public Nullable<double> rose20 { get; set; }
        public Nullable<double> rose30 { get; set; }
        public Nullable<double> spout02 { get; set; }
        public Nullable<double> spout05 { get; set; }
        public Nullable<double> spout10 { get; set; }
        public Nullable<double> spout20 { get; set; }
        public Nullable<double> spout30 { get; set; }
        public Nullable<double> inlet02 { get; set; }
        public Nullable<double> inlet05 { get; set; }
        public Nullable<double> inlet10 { get; set; }
        public Nullable<double> inlet20 { get; set; }
        public Nullable<double> inlet30 { get; set; }
        public Nullable<double> Spout02Aerator { get; set; }
        public Nullable<double> Spout05Aerator { get; set; }
        public Nullable<double> Spout10Aerator { get; set; }
        public Nullable<double> Spout20Aerator { get; set; }
        public Nullable<double> Spout30Aerator { get; set; }
        public Nullable<int> brand_group { get; set; }
        public string gold_code { get; set; }
        public Nullable<int> web_status { get; set; }
        public Nullable<int> overflow_class { get; set; }
        public Nullable<double> overflow_rate { get; set; }
        public string combination_comments { get; set; }
        public string tech_water_volume_note { get; set; }
        public Nullable<int> image_gallery { get; set; }
        public int? parent_id { get; set; }
        public string option_name { get; set; }
        public double? override_length { get; set; }
        public double? override_width { get; set; }
        public double? override_height { get; set; }
        public int? web_sub_sub_sub_category { get; set; }
        public int? sale_on { get; set; }

        //Fields from another tables, later we should convert this to associations with other objects
        public string brand_sub_sub_desc { get; set; }
        public string brand_sub_sub_sub_desc { get; set; }
        public string brand_sub_desc { get; set; }
        public string category_name { get; set; }
        public string brand_cat_image { get; set; }
        public string brand_sub_image { get; set; }
        public int? display_type { get; set; }
        public int? sub_same_id { get; set; }
        public int? sub_option_component { get; set; }
        public string pricing_note { get; set; }

        public int? web_seq2 { get; set; }  //category sequence
        public int? seq { get; set; }   //subsub category sequence

        public double? flow02 { get; set; }
        public double? flow05 { get; set; }
        public double? flow10 { get; set; }
        public double? flow20 { get; set; }
        public double? flow30 { get; set; }

        public double? aerator02 { get; set; }
        public double? aerator05 { get; set; }
        public double? aerator10 { get; set; }
        public double? aerator20 { get; set; }
        public double? aerator30 { get; set; }
        
        public int instructions { get; set; }

        public bool visibleTrueFalse { get; set; }
                        
        public virtual List<Cust_products> Components { get; set; }
        public List<string> TechCompliances { get; set; }
        public List<string> TechAdditionalInfo { get; set; }

        public double Price
        {
            get
            {
                double? sum = Components != null ? Components.Sum(c => (c.cprod_retail_web_override > 0 ? c.cprod_retail_web_override: c.cprod_retail)) : null;
                if (sum != null)
                    return sum.Value;
                else
                    return 0;
            }
        }

        public double SalePrice
        {
            get
            {
                double? sum = Components != null ? Components.Sum(c => (c.sale_retail)) : null;
                if (sum != null)
                    return sum.Value;
                else
                    return 0;
            }
        }

        public double PercentOff
        {
            get
            {
                if (SalePrice > 0)
                    return (Price - SalePrice)/Price;
                return 0;
            }
        }

        public List<WebProduct> Children { get;set; }
        public WebProduct NonLocalized { get; set; }

        public string comp1_name { get; set; }
        public string comp2_name { get; set; }
        public string comp3_name { get; set; }
        public string comp4_name { get; set; }
        public string comp5_name { get; set; }
        public string comp6_name { get; set; }
        public string comp7_name { get; set; }
        public string comp8_name { get; set; }
        public string comp9_name { get; set; }
        public string comp10_name { get; set; }
        public string comp11_name { get; set; }
        public string comp12_name { get; set; }
        
        public string comp1_code { get; set; }
        public string comp2_code { get; set; }
        public string comp3_code { get; set; }
        public string comp4_code { get; set; }
        public string comp5_code { get; set; }

        public string comp12_code { get; set; }
        public string comp6_code { get; set; }
        public string comp7_code { get; set; }
        public string comp8_code { get; set; }
        public string comp9_code { get; set; }
        public string comp10_code { get; set; }
        public string comp11_code { get; set; }

        public double? comp1_retail { get; set; }
        public double? comp2_retail { get; set; }
        public double? comp3_retail { get; set; }
        public double? comp4_retail { get; set; }
        public double? comp5_retail { get; set; }
        public double? comp6_retail { get; set; }
        public double? comp7_retail { get; set; }
        public double? comp8_retail  { get; set; }
        public double? comp9_retail { get; set; }
        public double? comp10_retail { get; set; }
        public double? comp11_retail { get; set; }
        public double? comp12_retail { get; set; }

        public int? prod_length { get; set; }
        public int? prod_width { get; set; }
        public int? prod_height { get; set; }

        public int? prod_nw { get; set; }

        public string comp1_instructions { get; set; }

        public string prod_image3 { get; set; }
    }
}
