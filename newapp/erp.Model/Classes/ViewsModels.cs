using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    [NotMapped]
    public class AmendmentsLocation
    {
        public int? cprod_id { get; set; }
        public int? cprod_mast { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? old_data { get; set; }
        public int? new_data { get; set; }
        public string process { get; set; }
    }

    [NotMapped]
    public class DistCustProductDetail2
    {
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public string old_data { get; set; }
        public string new_data { get; set; }
    }

    [NotMapped]
    public class IndexOrderSummary
    {
        public DateTime? po_req_etd { get; set; }
        public int? userid1 { get; set; }
        public int? orderid { get; set; }
        public int? invoice { get; set; }
        public string custpo { get; set; }
        public DateTime? req_eta { get; set; }
        public DateTime? actual_eta { get; set; }
        public int? process_id { get; set; }
        public bool? combined_order { get; set; }
        public string shipped_from { get; set; }
    }

    [NotMapped]
    public class OrderStatusLog2011 
    {
        public DateTime? po_req_etd { get; set; }
        public int? userid1 { get; set; }
        public int? factory_id { get; set; }
        public int? orderid { get; set; }
        public int? invoice { get; set; }
        public string inv_status { get; set; }
        public string custpo { get; set; }
        public DateTime? req_eta { get; set; }
        public DateTime? actual_eta { get; set; }
        public DateTime? duedate2 { get; set; }
        public int? process_id { get; set; }
        public bool? combined_order { get; set; }
    }

    [NotMapped]
    public class OrderLineDetail
    {
        public string custpo { get; set; }
        public DateTime? po_req_etd { get; set; }
        public DateTime? req_eta { get; set; }
        public int? orderqty { get; set; }
        public int? orderid { get; set; }
        public int? userid1 { get; set; }
        public bool? combined_order { get; set; }
        public string cprod_code1 { get; set; }
    }

    [NotMapped]
    public class BrandSalesAnalysisProduct2
    {
        public string month21 { get; set; }
        public int? orderqty { get; set; }

    }

    [NotMapped]
    public class ChangeNotice2012
    {
        public int? cn_id { get; set; }
        public string cn_details { get; set; }
        public DateTime? cn_date { get; set; }
    }

    [NotMapped]
    public class Returns2Lite
    {
        public DateTime? request_date { get; set; }
        public string return_no { get; set; }
        public int? return_qty { get; set; }
        public string client_comments { get; set; }
    }

    [NotMapped]
    public class Spares2
    {
        public int? cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? product_cprod { get; set; }
        public double? cprod_retail { get; set; }
        public double? discount_ddp_credit_40 { get; set; }
        public double? discount_ddp_cash_40 { get; set; }
        public int? dist_spec_disc { get; set; }
        public int? moq { get; set; }
        public int? units_per_carton { get; set; }
        public string spare_desc { get; set; }
        public string prod_image1 { get; set; }
    }

    [NotMapped]
    public class DistCustProductDetailIncDelete
    {
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? cprod_id { get; set; }
        public string cprod_spares { get; set; }
    }

    [NotMapped]
    public class ReturnsProductListings
    {
        public int? cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? consolidated_port { get; set; }

    }

    [NotMapped]
    public class ReturnsV2
    {
        public int? decision_final { get; set; }
        public int? status1 { get; set; }
        public int? daysdiff { get; set; }
        public double? openclosed { get; set; }
        public string sort4 { get; set; }
        public string flagged { get; set; }
        public int? returnsid { get; set; }
        public int? client_id { get; set; }
        public string return_no { get; set; }
        public DateTime? request_date { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public string reference { get; set; }
        public string flagged_reason { get; set; }
        public int? decision { get; set; }
        public DateTime? cc_response_date { get; set; }
        public int? user_id { get; set; }
        public double? credit_value { get; set; }
        public double? claim_value { get; set; }
        public int? return_qty { get; set; }
        public int? month21 { get; set; }
        public int? month22 { get; set; }
    }

    [NotMapped]
    public class OrderLinesDetail7Graph
    {
        public int? userid1 { get; set; }
        public int? orderqty { get; set; }
        public double? unitprice { get; set; }
        public int? unitcurrency { get; set; }
        public int? orderid { get; set; }
        public int? month23 { get; set; }
        public DateTime? po_req_etd { get; set; }
        public int? cprod_user { get; set; }
        public int? custpo { get; set; }
        public DateTime? orderdate { get; set; }
    }

    [NotMapped]
    public class Standards2
    {
        public int? std_id { get; set; }
        public string std_code { get; set; }
        public string std_description { get; set; }
        public string cat1_name { get; set; }
        public string cat2_desc { get; set; }
        public string cat2_desc2 { get; set; }
    }

    [NotMapped]
    public class StandardsDetail
    {
        public int? std_id { get; set; }
        public string std_code { get; set; }
        public string std_description { get; set; }
        public string std_detail_header { get; set; }
        public int? std_detail_unique { get; set; }
        public string std_detail_text { get; set; }
        public string std_detail_text2 { get; set; }
        public string std_detail_text3 { get; set; }
        public int? rowspan { get; set; }
        public int? std_dimension { get; set; }
        public int? std_positive { get; set; }
        public int? std_negative { get; set; }
        public string std_detail_type { get; set; }
        public string std_detail_seq { get; set; }
        public string std_image { get; set; }
        public string std_image2 { get; set; }
        public string std_symbol { get; set; }
        public int? std_image_left { get; set; }
        public string std_dimension_prefix { get; set; }
        public int? definition_flag { get; set; }
        public int? examination_method { get; set; }
        public int? inspection_valid { get; set; }
        public int? sub_link { get; set; }
    }

    [NotMapped]
    public class Categories2
    {
        public string cat1_name { get; set; }
        public string cat2_desc { get; set; }
        public int? cat2_code { get; set; }
    }

    [NotMapped]
    public class aql_clientsub
    {
        public int unique_aql_sub { get; set; }
    }

    [NotMapped]
    public class product_criteria_subs
    {
        public int? prod_criteria_unique { get; set; }
        public int? std_dimension { get; set; }
        public int? std_positive { get; set; }
        public int? std_detail_id { get; set; }
        public int? prod_cprod_id { get; set; }
    }

    [NotMapped]
    public class AdminFactories
    {
        public int? user_id { get; set; }
        public int? userid { get; set; }
        public string userusername { get; set; }
        public string customer_code { get; set; }
        public string factory_code { get; set; }
        public int? user_type { get; set; }
        public int? consolidated_port { get; set; }
        public int? permission_id { get; set; }
    }

    [NotMapped]
    public class returns_stats1_client_6months
    {
        public int? userid1 { get; set; }
        public double? orderqty { get; set; }
        public int? total_returns { get; set; }
        public double? GBP { get; set; }
        public string customer_code { get; set; }
        public int? GBP_returns { get; set; }
    }

    [NotMapped]
    public class brand_sales_analysis
    {
        public int? unitcurrency { get; set; }
        public string customer_code { get; set; }
        public int? cprod_user { get; set; }
        public double? rowprice { get; set; }
        public int? month21 { get; set; }
    }

    [NotMapped]
    public class returns_v4
    {
        public DateTime? comments_date { get; set; }
        public DateTime? request_date { get; set; }
        public int? claim_type { get; set; }
        public int? decision_final { get; set; }
        public int? daysdiff { get; set; }
        public int? user_id { get; set; }
        public int? highlight { get; set; }
        public int? status1 { get; set; }
        public string factory_code { get; set; }
        public string customer_code { get; set; }
        public int? returnsid { get; set; }
        public int? client_id { get; set; }
        public string return_no { get; set; }
        public string cprod_code1 { get; set; }
    }

    [NotMapped]
    public class order_status_log
    {
        public int? factory_id { get; set; }
        public string factory_code { get; set; }
        public int? process_id { get; set; }
        public int? invoice { get; set; }
        public int? orderid { get; set; }
        public int? userid1 { get; set; }
        public int? loading_factory { get; set; }
        public string custpo { get; set; }
        public DateTime? po_req_etd { get; set; }
        public DateTime? orderdate { get; set; }
        public string customer_code { get; set; }
        public string notes { get; set; }
        public string status { get; set; }
        public int? porderid { get; set; }
        public string inv_status { get; set; }
        public int? customs_deduction { get; set; }
        public string po_brief { get; set; }
        public string so_brief { get; set; }
        public int? po_process_id { get; set; }
        public int? combined_order { get; set; }
        public int? stock_order { get; set; }
    }

}
