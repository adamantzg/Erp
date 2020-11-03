
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Cw_customer
	{
		public string customer { get; set; }
		public string alpha { get; set; }
		public string name { get; set; }
		public string address1 { get; set; }
		public string address2 { get; set; }
		public string address3 { get; set; }
		public string address4 { get; set; }
		public string address5 { get; set; }
		public string town_city { get; set; }
		public string county { get; set; }
		public string state_region { get; set; }
		public string iso_country_code { get; set; }
		public string country { get; set; }
		public string credit_category { get; set; }
		public string export_indicator { get; set; }
		public string cust_disc_code { get; set; }
		public string currency { get; set; }
		public string territory { get; set; }
		public string Class { get; set; }
		public string region { get; set; }
		public string invoice_customer { get; set; }
		public string statement_customer { get; set; }
		public string group_customer { get; set; }
		public DateTime? date_last_issue { get; set; }
		public DateTime? date_created { get; set; }
		public string analysis_codes1 { get; set; }
		public string analysis_codes2 { get; set; }
		public string analysis_codes3 { get; set; }
		public string analysis_codes4 { get; set; }
		public string analysis_codes5 { get; set; }
		public string reminder_cat { get; set; }
		public string settlement_code { get; set; }
		public string sett_days_code { get; set; }
		public string price_list { get; set; }
		public string letter_code { get; set; }
		public string balance_fwd { get; set; }
		public string credit_limit { get; set; }
		public double? ytd_sales { get; set; }
		public double? ytd_cost_of_sales { get; set; }
		public string cumulative_sales { get; set; }
		public string order_balance { get; set; }
		public string sales_nl_cat { get; set; }
		public string special_price { get; set; }
		public string vat_registration { get; set; }
		public string direct_debit { get; set; }
		public string invoices_printed { get; set; }
		public string consolidated_inv { get; set; }
		public string comment_only_inv { get; set; }
		public string bank_account_no { get; set; }
		public string bank_sort_code { get; set; }
		public string bank_name { get; set; }
		public string bank_address1 { get; set; }
		public string bank_address2 { get; set; }
		public string bank_address3 { get; set; }
		public string bank_address4 { get; set; }
		public string analysis_code_6 { get; set; }
		public string produce_statements { get; set; }
		public string edi_customer { get; set; }
		public string vat_type { get; set; }
		public string lang { get; set; }
		public string delivery_method { get; set; }
		public string carrier { get; set; }
		public string vat_reg_number { get; set; }
		public string vat_exe_number { get; set; }
		public string paydays1 { get; set; }
		public string paydays2 { get; set; }
		public string paydays3 { get; set; }
		public string bank_branch_code { get; set; }
		public string print_cp_with_stat { get; set; }
		public string payment_method { get; set; }
		public string customer_class { get; set; }
		public string sales_type { get; set; }
		public string cp_lower_value { get; set; }
		public string address6 { get; set; }
		public string fax { get; set; }
		public string telex { get; set; }
		public string btx { get; set; }
		public string cp_charge { get; set; }
		public string control_digit { get; set; }
		public string payer { get; set; }
		public string responsibility { get; set; }
		public string despatch_held { get; set; }
		public string credit_controller { get; set; }
		public string reminder_letters { get; set; }
		public int? severity_days1 { get; set; }
		public int? severity_days2 { get; set; }
		public int? severity_days3 { get; set; }
		public int? severity_days4 { get; set; }
		public int? severity_days5 { get; set; }
		public int? severity_days6 { get; set; }
		public string delivery_reason { get; set; }
		public string shipper_code1 { get; set; }
		public string shipper_code2 { get; set; }
		public string shipper_code3 { get; set; }
		public string shipping_note_ind { get; set; }
		public string account_type { get; set; }
		public string admin_fee { get; set; }
		public string intrest_rate { get; set; }
		public string iban { get; set; }
		public string bic { get; set; }
		public string email { get; set; }
		public string transaction_email { get; set; }
		public string credit_limit_safe { get; set; }

        public Dealer Dealer { get; set; }
	}
}	
	