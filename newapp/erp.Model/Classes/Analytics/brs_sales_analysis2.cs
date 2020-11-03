
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Brs_sales_analysis2
	{
        [Key]
		public int linenum { get; set; }
		public int? orderid { get; set; }
		public DateTime? linedate { get; set; }
		public int? cprod_id { get; set; }
		public string description { get; set; }
		public double? orderqty { get; set; }
		public double? unitprice { get; set; }
		public double? rowprice { get; set; }
		public int? unitcurrency { get; set; }
		public double? rowprice_gbp { get; set; }
		public double? PO_rowprice_gbp { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public int? cprod_mast { get; set; }
		public int? factory_id { get; set; }
		public string factory_ref { get; set; }
		public string factory_name { get; set; }
		public string asaq_ref { get; set; }
		public string asaq_name { get; set; }
		public double? price_dollar { get; set; }
		public double? price_euro { get; set; }
		public double? price_pound { get; set; }
		public string user_name { get; set; }
		public string factory_code { get; set; }
		public int? soline { get; set; }
		public int? porderid { get; set; }
		public double? poqty { get; set; }
		public int? polinenum { get; set; }
		public string curr_symbol { get; set; }
		public double? lme { get; set; }
		public double? poprice { get; set; }
		public double? price_dollar_ex { get; set; }
		public double? price_euro_ex { get; set; }
		public double? price_pound_ex { get; set; }
		public int? cprod_user { get; set; }
		public int? pocurrency { get; set; }
		public string fact_curr_symbol { get; set; }
		public int? packunits { get; set; }
		public DateTime? req_eta { get; set; }
		public string month21 { get; set; }
		public string month22 { get; set; }
		public string custpo { get; set; }
		public string status { get; set; }
		public string customer_code { get; set; }
		public string client_name { get; set; }
		public int? distributor { get; set; }
		public string user_country { get; set; }
		public string product_group { get; set; }
		public DateTime? po_req_etd { get; set; }
		public int? category1 { get; set; }
		public string cat1_name { get; set; }
		public int? brand_user_id { get; set; }
		public int? oem_flag { get; set; }
		public int? userid1 { get; set; }
		public int? brand_id { get; set; }
		public string brandname { get; set; }
	
	}
}	
	