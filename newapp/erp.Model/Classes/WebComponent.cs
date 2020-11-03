using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class WebComponent
    {
        public int cprod_id { get; set; }
        public Nullable<int> cprod_mast { get; set; }
        public Nullable<int> cprod_user { get; set; }
        public string cprod_name { get; set; }
        public string cprod_name2 { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_code2 { get; set; }
        public Nullable<double> cprod_price1 { get; set; }
        public Nullable<double> cprod_price2 { get; set; }
        public Nullable<double> cprod_price3 { get; set; }
        public Nullable<double> cprod_price4 { get; set; }
        public string cprod_image1 { get; set; }
        public string cprod_instructions2 { get; set; }
        public string cprod_instructions { get; set; }
        public string cprod_label { get; set; }
        public string cprod_packaging { get; set; }
        public string cprod_dwg { get; set; }
        public string cprod_spares { get; set; }
        public string cprod_pdf1 { get; set; }
        public Nullable<int> cprod_cgflag { get; set; }
        public Nullable<int> cprod_curr { get; set; }
        public Nullable<int> cprod_opening_qty { get; set; }
        public Nullable<System.DateTime> cprod_opening_date { get; set; }
        public string cprod_status { get; set; }
        public Nullable<int> cprod_oldcode { get; set; }
        public Nullable<int> cprod_lme { get; set; }
        public Nullable<int> cprod_brand_cat { get; set; }
        public Nullable<double> cprod_retail { get; set; }
        public Nullable<double> cprod_retail_pending { get; set; }
        public Nullable<System.DateTime> cprod_retail_pending_date { get; set; }
        public Nullable<double> cprod_retail_web_override { get; set; }
        public Nullable<double> cprod_override_margin { get; set; }
        public Nullable<int> cprod_disc { get; set; }
        public Nullable<int> cprod_seq { get; set; }
        public Nullable<int> cprod_stock_code { get; set; }
        public Nullable<double> days30_sales { get; set; }
        public Nullable<int> brand_grouping { get; set; }
        public Nullable<int> b_gold { get; set; }
        public Nullable<int> cprod_loading { get; set; }
        public Nullable<int> moq { get; set; }
        public Nullable<int> WC_2011 { get; set; }
        public Nullable<int> cprod_stock { get; set; }
        public Nullable<int> cprod_stock2 { get; set; }
        public Nullable<int> cprod_priority { get; set; }
        public string cprod_status2 { get; set; }
        public Nullable<double> cprod_pending_price { get; set; }
        public Nullable<System.DateTime> cprod_pending_date { get; set; }
        public string pack_image1 { get; set; }
        public string pack_image2 { get; set; }
        public string pack_image2b { get; set; }
        public string pack_image2c { get; set; }
        public string pack_image2d { get; set; }
        public string pack_image3 { get; set; }
        public string pack_image4 { get; set; }
        public Nullable<int> aql_A { get; set; }
        public Nullable<int> aql_D { get; set; }
        public Nullable<int> aql_F { get; set; }
        public Nullable<int> aql_M { get; set; }
        public string insp_level_a { get; set; }
        public string insp_level_D { get; set; }
        public string insp_level_F { get; set; }
        public string insp_level_M { get; set; }
        public Nullable<int> criteria_status { get; set; }
        public Nullable<int> cprod_confirmed { get; set; }
        public Nullable<int> tech_template { get; set; }
        public Nullable<int> tech_template2 { get; set; }
        public Nullable<int> cprod_returnable { get; set; }
        public Nullable<int> client_cat1 { get; set; }
        public Nullable<int> client_cat2 { get; set; }
        public string client_image { get; set; }
        public string cprod_track_image1 { get; set; }
        public string cprod_track_image2 { get; set; }
        public string cprod_track_image3 { get; set; }

        //public virtual List<WebProduct> Products { get; set; }
    }
}
