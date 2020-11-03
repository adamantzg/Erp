using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public enum CompanyType
    {
        Distributor = 1,
        MasterDistributor,
        Manufacturer
    }

    public enum Company_User_Type
    {
        Factory = 1,
        Reserved,
        Client,
        Base
    }
    
    /// <summary>
    /// Refers to table User in db
    /// </summary>
    public class Company
    {
        public const int BathStore1 = 37;
        public const int BathStore2 = 200;
        public const int Bathroomsourcing = 1;
        

        public int user_id { get; set; }
		public string user_name { get; set; }
		public string reporting_name { get; set; }
		public string reporting_filename { get; set; }
		public string user_account { get; set; }
		public string customer_code { get; set; }
		public int? distributor { get; set; }
		public int? oem_flag { get; set; }
		public int? user_status { get; set; }
		public string clientlogo { get; set; }
		public string brandlogo { get; set; }
		public string brandname { get; set; }
		public string brandurl { get; set; }
		public int? cashcredit { get; set; }
		public string user_welcomename { get; set; }
		public string user_address1 { get; set; }
		public string factory_code { get; set; }
        // dodao
        //public int order_id { get; set; }
        //public double row_price { get; set; }
		public string user_address2 { get; set; }
		public string user_address3 { get; set; }
		public string user_address4 { get; set; }
		public string user_address5 { get; set; }
		public string user_address6 { get; set; }
		public string user_country { get; set; }
		public string user_country2 { get; set; }
		public int? user_country_sort { get; set; }
		public string user_contact { get; set; }
		public string user_contact2 { get; set; }
		public string user_contact3 { get; set; }
		public string user_contact4 { get; set; }
		public string user_tel { get; set; }
		public string user_fax { get; set; }
		public string user_mobile { get; set; }
		public string user_website { get; set; }
		public string user_email { get; set; }
		public string user_email2 { get; set; }
		public string user_email3 { get; set; }
		public string user_email4 { get; set; }
		public int? user_type { get; set; }
		public int? user_access { get; set; }
		public string user_pwd { get; set; }
		public DateTime? user_created { get; set; }
		public int? user_curr { get; set; }
		public int? user_curr_pricing { get; set; }
		public int? dynamic_pricing { get; set; }
		public string user_port { get; set; }
		public string user_loading { get; set; }
		public string user_price_type { get; set; }
		public int? user_deposits { get; set; }
		public int? user_payments_stage { get; set; }
		public int? user_payments_days { get; set; }
		public DateTime? lastlogin { get; set; }
		public double? stock_req_1 { get; set; }
		public double? stock_req_2 { get; set; }
		public double? stock_req_3 { get; set; }
		public double? stock_req_4 { get; set; }
		public double? stock_req_5 { get; set; }
		public double? reordering_freq { get; set; }
		public string shipping_mark { get; set; }
		public double? qc_charge { get; set; }
		public int? qc_show { get; set; }
		public int? qc_spec { get; set; }
		public int? lme { get; set; }
		public int? factory_group { get; set; }
		public int? clearwater { get; set; }
		public int? containers { get; set; }
		public int? multi_location { get; set; }
		public int? product_catalogue { get; set; }
		public int? menu_co { get; set; }
		public int? menu_sc { get; set; }
		public int? menu_vp { get; set; }
		public int? menu_om { get; set; }
		public double? exchange_rate { get; set; }
		public double? exchange_rate2 { get; set; }
		public int? consolidated_port_mix { get; set; }
		public int? consolidated_port2 { get; set; }
		public int? consolidated_port { get; set; }
		public double? lcl_surcharge { get; set; }
		public double? customs_deduction { get; set; }
		public int? curr_pref_usd { get; set; }
		public int? curr_pref_gbp { get; set; }
		public int? curr_pref_eur { get; set; }
		public string bank_name { get; set; }
		public string bank_address { get; set; }
		public string bank_swift { get; set; }
		public string bank_account { get; set; }
		public string bank_name2 { get; set; }
		public string bank_address2 { get; set; }
		public string bank_account2 { get; set; }
		public string bank_swift2 { get; set; }
		public int? factory_usd { get; set; }
		public int? factory_gbp { get; set; }
		public int? factory_eur { get; set; }
		public string vendor_name { get; set; }
		public string vendor_add1 { get; set; }
		public string vendor_add2 { get; set; }
		public string vendor_add3 { get; set; }
		public string vendor_add4 { get; set; }
		public int? hide_1 { get; set; }
		public int? brand_b { get; set; }
		public int? brand_wc { get; set; }
		public int? brand_z { get; set; }
		public int? brand_cu { get; set; }
		public int? brand_tr { get; set; }
		public int? brand_bb { get; set; }
		public int? brand_st { get; set; }
        public int? brand_ar { get; set; }
        public int? brand_ac { get; set; }
		public int? lme2 { get; set; }
		public int? agent_account { get; set; }
		public double? brsinv_comm { get; set; }
		public int? combined_factory { get; set; }
		public string bs_code { get; set; }
		public string bs_name { get; set; }
		public int? prod_days { get; set; }
		public int? ship_days { get; set; }
		public int? stock_days { get; set; }
        [Column("2011_interface")]
		public int? EF_fix_2011_interface { get; set; }
		public int? os { get; set; }
		public int? com { get; set; }
		public int? om { get; set; }
		public int? prod { get; set; }
		public int? feed { get; set; }
		public int? corder { get; set; }
		public int? upload { get; set; }
		public int? eta_extra_days { get; set; }
        public int? loading_seq { get; set; }
        public int? dealer_distributor { get; set; }
        public int? stock_order_month_limit { get; set; }
        public int? invoice_sequence { get; set; }
        public string invoice_logo { get; set; }
        public int? test_account { get; set; }
        public bool? showForBudgetActual { get; set; }

		public double? vat_rate { get; set; }

		public Currencies Currency { get; set; }

        public List<Brand> Brands { get; set; }
        public List<Order_header> Orders { get; set; }
        public List<Porder_header> POrders { get; set; }
        //public List<SiteEdit> SiteEdits { get; set; }

        public virtual List<User> Users { get; set; }
        public virtual Collection<Dealer> Dealers { get; set; }

        public List<Mast_products> MastProducts { get; set; }
        public List<Returns> Returns { get; set; }

        public List<Dist_products> DistProducts { get; set; }

        public List<Company> ExcludedClients { get; set; }

		public List<File> Files { get; set; }

		public List<Cust_products> CustProducts { get; set; }

	}
    
}
