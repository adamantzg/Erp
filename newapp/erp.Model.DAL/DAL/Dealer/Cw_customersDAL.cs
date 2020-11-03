
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Cw_customersDAL
	{
	
		public static List<Cw_customer> GetAll()
		{
			var result = new List<Cw_customer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM cw_customers", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Cw_customer> GetMatches(DateTime? from = null)
        {
            var result = new List<Cw_customer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT cw_customers.*,dealers.* FROM cw_customers INNER JOIN dealers ON cw_customers.address6 = dealers.postcode WHERE dealers.cw_code IS NULL AND date_last_issue >= @from", conn);
                cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
                cmd.CommandTimeout = 300;
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var cust = GetFromDataReader(dr);
                    cust.Dealer = DealerDAL.GetDealerFromReader(dr);
                    result.Add(cust);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Cw_customer> GetNonMatched(DateTime? from = null)
        {
            var result = new List<Cw_customer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT cw_customers.* FROM cw_customers WHERE NOT EXISTS(SELECT user_id FROM dealers WHERE postcode = cw_customers.address6 OR cw_code = cw_customers.customer) AND date_last_issue >= @from", conn);
                cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
                cmd.CommandTimeout = 300;
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var cust = GetFromDataReader(dr);
                    result.Add(cust);
                }
                dr.Close();
            }
            return result;
        }
		
		public static Cw_customer GetById(int id)
		{
			Cw_customer result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM cw_customers WHERE customer = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		public static Cw_customer GetFromDataReader(MySqlDataReader dr)
		{
			Cw_customer o = new Cw_customer();
		
			o.customer = string.Empty + Utilities.GetReaderField(dr,"customer");
			o.alpha = string.Empty + Utilities.GetReaderField(dr,"alpha");
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.address1 = string.Empty + Utilities.GetReaderField(dr,"address1");
			o.address2 = string.Empty + Utilities.GetReaderField(dr,"address2");
			o.address3 = string.Empty + Utilities.GetReaderField(dr,"address3");
			o.address4 = string.Empty + Utilities.GetReaderField(dr,"address4");
			o.address5 = string.Empty + Utilities.GetReaderField(dr,"address5");
			o.town_city = string.Empty + Utilities.GetReaderField(dr,"town_city");
			o.county = string.Empty + Utilities.GetReaderField(dr,"county");
			o.state_region = string.Empty + Utilities.GetReaderField(dr,"state_region");
			o.iso_country_code = string.Empty + Utilities.GetReaderField(dr,"iso_country_code");
			o.country = string.Empty + Utilities.GetReaderField(dr,"country");
			o.credit_category = string.Empty + Utilities.GetReaderField(dr,"credit_category");
			o.export_indicator = string.Empty + Utilities.GetReaderField(dr,"export_indicator");
			o.cust_disc_code = string.Empty + Utilities.GetReaderField(dr,"cust_disc_code");
			o.currency = string.Empty + Utilities.GetReaderField(dr,"currency");
			o.territory = string.Empty + Utilities.GetReaderField(dr,"territory");
			o.Class = string.Empty + Utilities.GetReaderField(dr,"class");
			o.region = string.Empty + Utilities.GetReaderField(dr,"region");
			o.invoice_customer = string.Empty + Utilities.GetReaderField(dr,"invoice_customer");
			o.statement_customer = string.Empty + Utilities.GetReaderField(dr,"statement_customer");
			o.group_customer = string.Empty + Utilities.GetReaderField(dr,"group_customer");
			o.date_last_issue = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date_last_issue"));
			o.date_created = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date_created"));
			o.analysis_codes1 = string.Empty + Utilities.GetReaderField(dr,"analysis_codes1");
			o.analysis_codes2 = string.Empty + Utilities.GetReaderField(dr,"analysis_codes2");
			o.analysis_codes3 = string.Empty + Utilities.GetReaderField(dr,"analysis_codes3");
			o.analysis_codes4 = string.Empty + Utilities.GetReaderField(dr,"analysis_codes4");
			o.analysis_codes5 = string.Empty + Utilities.GetReaderField(dr,"analysis_codes5");
			o.reminder_cat = string.Empty + Utilities.GetReaderField(dr,"reminder_cat");
			o.settlement_code = string.Empty + Utilities.GetReaderField(dr,"settlement_code");
			o.sett_days_code = string.Empty + Utilities.GetReaderField(dr,"sett_days_code");
			o.price_list = string.Empty + Utilities.GetReaderField(dr,"price_list");
			o.letter_code = string.Empty + Utilities.GetReaderField(dr,"letter_code");
			o.balance_fwd = string.Empty + Utilities.GetReaderField(dr,"balance_fwd");
			o.credit_limit = string.Empty + Utilities.GetReaderField(dr,"credit_limit");
            o.ytd_sales = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "ytd_sales"));
			o.ytd_cost_of_sales = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"ytd_cost_of_sales"));
			o.cumulative_sales = string.Empty + Utilities.GetReaderField(dr,"cumulative_sales");
			o.order_balance = string.Empty + Utilities.GetReaderField(dr,"order_balance");
			o.sales_nl_cat = string.Empty + Utilities.GetReaderField(dr,"sales_nl_cat");
			o.special_price = string.Empty + Utilities.GetReaderField(dr,"special_price");
			o.vat_registration = string.Empty + Utilities.GetReaderField(dr,"vat_registration");
			o.direct_debit = string.Empty + Utilities.GetReaderField(dr,"direct_debit");
			o.invoices_printed = string.Empty + Utilities.GetReaderField(dr,"invoices_printed");
			o.consolidated_inv = string.Empty + Utilities.GetReaderField(dr,"consolidated_inv");
			o.comment_only_inv = string.Empty + Utilities.GetReaderField(dr,"comment_only_inv");
			o.bank_account_no = string.Empty + Utilities.GetReaderField(dr,"bank_account_no");
			o.bank_sort_code = string.Empty + Utilities.GetReaderField(dr,"bank_sort_code");
			o.bank_name = string.Empty + Utilities.GetReaderField(dr,"bank_name");
			o.bank_address1 = string.Empty + Utilities.GetReaderField(dr,"bank_address1");
			o.bank_address2 = string.Empty + Utilities.GetReaderField(dr,"bank_address2");
			o.bank_address3 = string.Empty + Utilities.GetReaderField(dr,"bank_address3");
			o.bank_address4 = string.Empty + Utilities.GetReaderField(dr,"bank_address4");
			o.analysis_code_6 = string.Empty + Utilities.GetReaderField(dr,"analysis_code_6");
			o.produce_statements = string.Empty + Utilities.GetReaderField(dr,"produce_statements");
			o.edi_customer = string.Empty + Utilities.GetReaderField(dr,"edi_customer");
			o.vat_type = string.Empty + Utilities.GetReaderField(dr,"vat_type");
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.delivery_method = string.Empty + Utilities.GetReaderField(dr,"delivery_method");
			o.carrier = string.Empty + Utilities.GetReaderField(dr,"carrier");
			o.vat_reg_number = string.Empty + Utilities.GetReaderField(dr,"vat_reg_number");
			o.vat_exe_number = string.Empty + Utilities.GetReaderField(dr,"vat_exe_number");
			o.paydays1 = string.Empty + Utilities.GetReaderField(dr,"paydays1");
			o.paydays2 = string.Empty + Utilities.GetReaderField(dr,"paydays2");
			o.paydays3 = string.Empty + Utilities.GetReaderField(dr,"paydays3");
			o.bank_branch_code = string.Empty + Utilities.GetReaderField(dr,"bank_branch_code");
			o.print_cp_with_stat = string.Empty + Utilities.GetReaderField(dr,"print_cp_with_stat");
			o.payment_method = string.Empty + Utilities.GetReaderField(dr,"payment_method");
			o.customer_class = string.Empty + Utilities.GetReaderField(dr,"customer_class");
			o.sales_type = string.Empty + Utilities.GetReaderField(dr,"sales_type");
			o.cp_lower_value = string.Empty + Utilities.GetReaderField(dr,"cp_lower_value");
			o.address6 = string.Empty + Utilities.GetReaderField(dr,"address6");
			o.fax = string.Empty + Utilities.GetReaderField(dr,"fax");
			o.telex = string.Empty + Utilities.GetReaderField(dr,"telex");
			o.btx = string.Empty + Utilities.GetReaderField(dr,"btx");
			o.cp_charge = string.Empty + Utilities.GetReaderField(dr,"cp_charge");
			o.control_digit = string.Empty + Utilities.GetReaderField(dr,"control_digit");
			o.payer = string.Empty + Utilities.GetReaderField(dr,"payer");
			o.responsibility = string.Empty + Utilities.GetReaderField(dr,"responsibility");
			o.despatch_held = string.Empty + Utilities.GetReaderField(dr,"despatch_held");
			o.credit_controller = string.Empty + Utilities.GetReaderField(dr,"credit_controller");
			o.reminder_letters = string.Empty + Utilities.GetReaderField(dr,"reminder_letters");
			o.severity_days1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"severity_days1"));
			o.severity_days2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"severity_days2"));
			o.severity_days3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"severity_days3"));
			o.severity_days4 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"severity_days4"));
			o.severity_days5 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"severity_days5"));
			o.severity_days6 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"severity_days6"));
			o.delivery_reason = string.Empty + Utilities.GetReaderField(dr,"delivery_reason");
			o.shipper_code1 = string.Empty + Utilities.GetReaderField(dr,"shipper_code1");
			o.shipper_code2 = string.Empty + Utilities.GetReaderField(dr,"shipper_code2");
			o.shipper_code3 = string.Empty + Utilities.GetReaderField(dr,"shipper_code3");
			o.shipping_note_ind = string.Empty + Utilities.GetReaderField(dr,"shipping_note_ind");
			o.account_type = string.Empty + Utilities.GetReaderField(dr,"account_type");
			o.admin_fee = string.Empty + Utilities.GetReaderField(dr,"admin_fee");
			o.intrest_rate = string.Empty + Utilities.GetReaderField(dr,"intrest_rate");
			o.iban = string.Empty + Utilities.GetReaderField(dr,"iban");
			o.bic = string.Empty + Utilities.GetReaderField(dr,"bic");
			o.email = string.Empty + Utilities.GetReaderField(dr,"email");
			o.transaction_email = string.Empty + Utilities.GetReaderField(dr,"transaction_email");
			o.credit_limit_safe = string.Empty + Utilities.GetReaderField(dr,"credit_limit_safe");
			
			return o;

		}
		
		
		public static void Create(Cw_customer o)
        {
            string insertsql = @"INSERT INTO cw_customers (customer,alpha,name,address1,address2,address3,address4,address5,town_city,county,state_region,iso_country_code,country,credit_category,export_indicator,cust_disc_code,currency,territory,class,region,invoice_customer,statement_customer,group_customer,date_last_issue,date_created,analysis_codes1,analysis_codes2,analysis_codes3,analysis_codes4,analysis_codes5,reminder_cat,settlement_code,sett_days_code,price_list,letter_code,balance_fwd,credit_limit,ytd_sales,ytd_cost_of_sales,cumulative_sales,order_balance,sales_nl_cat,special_price,vat_registration,direct_debit,invoices_printed,consolidated_inv,comment_only_inv,bank_account_no,bank_sort_code,bank_name,bank_address1,bank_address2,bank_address3,bank_address4,analysis_code_6,produce_statements,edi_customer,vat_type,lang,delivery_method,carrier,vat_reg_number,vat_exe_number,paydays1,paydays2,paydays3,bank_branch_code,print_cp_with_stat,payment_method,customer_class,sales_type,cp_lower_value,address6,fax,telex,btx,cp_charge,control_digit,payer,responsibility,despatch_held,credit_controller,reminder_letters,severity_days1,severity_days2,severity_days3,severity_days4,severity_days5,severity_days6,delivery_reason,shipper_code1,shipper_code2,shipper_code3,shipping_note_ind,account_type,admin_fee,intrest_rate,iban,bic,email,transaction_email,credit_limit_safe) VALUES(@customer,@alpha,@name,@address1,@address2,@address3,@address4,@address5,@town_city,@county,@state_region,@iso_country_code,@country,@credit_category,@export_indicator,@cust_disc_code,@currency,@territory,@class,@region,@invoice_customer,@statement_customer,@group_customer,@date_last_issue,@date_created,@analysis_codes1,@analysis_codes2,@analysis_codes3,@analysis_codes4,@analysis_codes5,@reminder_cat,@settlement_code,@sett_days_code,@price_list,@letter_code,@balance_fwd,@credit_limit,@ytd_sales,@ytd_cost_of_sales,@cumulative_sales,@order_balance,@sales_nl_cat,@special_price,@vat_registration,@direct_debit,@invoices_printed,@consolidated_inv,@comment_only_inv,@bank_account_no,@bank_sort_code,@bank_name,@bank_address1,@bank_address2,@bank_address3,@bank_address4,@analysis_code_6,@produce_statements,@edi_customer,@vat_type,@lang,@delivery_method,@carrier,@vat_reg_number,@vat_exe_number,@paydays1,@paydays2,@paydays3,@bank_branch_code,@print_cp_with_stat,@payment_method,@customer_class,@sales_type,@cp_lower_value,@address6,@fax,@telex,@btx,@cp_charge,@control_digit,@payer,@responsibility,@despatch_held,@credit_controller,@reminder_letters,@severity_days1,@severity_days2,@severity_days3,@severity_days4,@severity_days5,@severity_days6,@delivery_reason,@shipper_code1,@shipper_code2,@shipper_code3,@shipping_note_ind,@account_type,@admin_fee,@intrest_rate,@iban,@bic,@email,@transaction_email,@credit_limit_safe)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand(insertsql, conn);
                
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Cw_customer o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@customer", o.customer);
			cmd.Parameters.AddWithValue("@alpha", o.alpha);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@address1", o.address1);
			cmd.Parameters.AddWithValue("@address2", o.address2);
			cmd.Parameters.AddWithValue("@address3", o.address3);
			cmd.Parameters.AddWithValue("@address4", o.address4);
			cmd.Parameters.AddWithValue("@address5", o.address5);
			cmd.Parameters.AddWithValue("@town_city", o.town_city);
			cmd.Parameters.AddWithValue("@county", o.county);
			cmd.Parameters.AddWithValue("@state_region", o.state_region);
			cmd.Parameters.AddWithValue("@iso_country_code", o.iso_country_code);
			cmd.Parameters.AddWithValue("@country", o.country);
			cmd.Parameters.AddWithValue("@credit_category", o.credit_category);
			cmd.Parameters.AddWithValue("@export_indicator", o.export_indicator);
			cmd.Parameters.AddWithValue("@cust_disc_code", o.cust_disc_code);
			cmd.Parameters.AddWithValue("@currency", o.currency);
			cmd.Parameters.AddWithValue("@territory", o.territory);
			cmd.Parameters.AddWithValue("@class", o.Class);
			cmd.Parameters.AddWithValue("@region", o.region);
			cmd.Parameters.AddWithValue("@invoice_customer", o.invoice_customer);
			cmd.Parameters.AddWithValue("@statement_customer", o.statement_customer);
			cmd.Parameters.AddWithValue("@group_customer", o.group_customer);
			cmd.Parameters.AddWithValue("@date_last_issue", o.date_last_issue);
			cmd.Parameters.AddWithValue("@date_created", o.date_created);
			cmd.Parameters.AddWithValue("@analysis_codes1", o.analysis_codes1);
			cmd.Parameters.AddWithValue("@analysis_codes2", o.analysis_codes2);
			cmd.Parameters.AddWithValue("@analysis_codes3", o.analysis_codes3);
			cmd.Parameters.AddWithValue("@analysis_codes4", o.analysis_codes4);
			cmd.Parameters.AddWithValue("@analysis_codes5", o.analysis_codes5);
			cmd.Parameters.AddWithValue("@reminder_cat", o.reminder_cat);
			cmd.Parameters.AddWithValue("@settlement_code", o.settlement_code);
			cmd.Parameters.AddWithValue("@sett_days_code", o.sett_days_code);
			cmd.Parameters.AddWithValue("@price_list", o.price_list);
			cmd.Parameters.AddWithValue("@letter_code", o.letter_code);
			cmd.Parameters.AddWithValue("@balance_fwd", o.balance_fwd);
			cmd.Parameters.AddWithValue("@credit_limit", o.credit_limit);
			cmd.Parameters.AddWithValue("@ytd_sales", o.ytd_sales);
			cmd.Parameters.AddWithValue("@ytd_cost_of_sales", o.ytd_cost_of_sales);
			cmd.Parameters.AddWithValue("@cumulative_sales", o.cumulative_sales);
			cmd.Parameters.AddWithValue("@order_balance", o.order_balance);
			cmd.Parameters.AddWithValue("@sales_nl_cat", o.sales_nl_cat);
			cmd.Parameters.AddWithValue("@special_price", o.special_price);
			cmd.Parameters.AddWithValue("@vat_registration", o.vat_registration);
			cmd.Parameters.AddWithValue("@direct_debit", o.direct_debit);
			cmd.Parameters.AddWithValue("@invoices_printed", o.invoices_printed);
			cmd.Parameters.AddWithValue("@consolidated_inv", o.consolidated_inv);
			cmd.Parameters.AddWithValue("@comment_only_inv", o.comment_only_inv);
			cmd.Parameters.AddWithValue("@bank_account_no", o.bank_account_no);
			cmd.Parameters.AddWithValue("@bank_sort_code", o.bank_sort_code);
			cmd.Parameters.AddWithValue("@bank_name", o.bank_name);
			cmd.Parameters.AddWithValue("@bank_address1", o.bank_address1);
			cmd.Parameters.AddWithValue("@bank_address2", o.bank_address2);
			cmd.Parameters.AddWithValue("@bank_address3", o.bank_address3);
			cmd.Parameters.AddWithValue("@bank_address4", o.bank_address4);
			cmd.Parameters.AddWithValue("@analysis_code_6", o.analysis_code_6);
			cmd.Parameters.AddWithValue("@produce_statements", o.produce_statements);
			cmd.Parameters.AddWithValue("@edi_customer", o.edi_customer);
			cmd.Parameters.AddWithValue("@vat_type", o.vat_type);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@delivery_method", o.delivery_method);
			cmd.Parameters.AddWithValue("@carrier", o.carrier);
			cmd.Parameters.AddWithValue("@vat_reg_number", o.vat_reg_number);
			cmd.Parameters.AddWithValue("@vat_exe_number", o.vat_exe_number);
			cmd.Parameters.AddWithValue("@paydays1", o.paydays1);
			cmd.Parameters.AddWithValue("@paydays2", o.paydays2);
			cmd.Parameters.AddWithValue("@paydays3", o.paydays3);
			cmd.Parameters.AddWithValue("@bank_branch_code", o.bank_branch_code);
			cmd.Parameters.AddWithValue("@print_cp_with_stat", o.print_cp_with_stat);
			cmd.Parameters.AddWithValue("@payment_method", o.payment_method);
			cmd.Parameters.AddWithValue("@customer_class", o.customer_class);
			cmd.Parameters.AddWithValue("@sales_type", o.sales_type);
			cmd.Parameters.AddWithValue("@cp_lower_value", o.cp_lower_value);
			cmd.Parameters.AddWithValue("@address6", o.address6);
			cmd.Parameters.AddWithValue("@fax", o.fax);
			cmd.Parameters.AddWithValue("@telex", o.telex);
			cmd.Parameters.AddWithValue("@btx", o.btx);
			cmd.Parameters.AddWithValue("@cp_charge", o.cp_charge);
			cmd.Parameters.AddWithValue("@control_digit", o.control_digit);
			cmd.Parameters.AddWithValue("@payer", o.payer);
			cmd.Parameters.AddWithValue("@responsibility", o.responsibility);
			cmd.Parameters.AddWithValue("@despatch_held", o.despatch_held);
			cmd.Parameters.AddWithValue("@credit_controller", o.credit_controller);
			cmd.Parameters.AddWithValue("@reminder_letters", o.reminder_letters);
			cmd.Parameters.AddWithValue("@severity_days1", o.severity_days1);
			cmd.Parameters.AddWithValue("@severity_days2", o.severity_days2);
			cmd.Parameters.AddWithValue("@severity_days3", o.severity_days3);
			cmd.Parameters.AddWithValue("@severity_days4", o.severity_days4);
			cmd.Parameters.AddWithValue("@severity_days5", o.severity_days5);
			cmd.Parameters.AddWithValue("@severity_days6", o.severity_days6);
			cmd.Parameters.AddWithValue("@delivery_reason", o.delivery_reason);
			cmd.Parameters.AddWithValue("@shipper_code1", o.shipper_code1);
			cmd.Parameters.AddWithValue("@shipper_code2", o.shipper_code2);
			cmd.Parameters.AddWithValue("@shipper_code3", o.shipper_code3);
			cmd.Parameters.AddWithValue("@shipping_note_ind", o.shipping_note_ind);
			cmd.Parameters.AddWithValue("@account_type", o.account_type);
			cmd.Parameters.AddWithValue("@admin_fee", o.admin_fee);
			cmd.Parameters.AddWithValue("@intrest_rate", o.intrest_rate);
			cmd.Parameters.AddWithValue("@iban", o.iban);
			cmd.Parameters.AddWithValue("@bic", o.bic);
			cmd.Parameters.AddWithValue("@email", o.email);
			cmd.Parameters.AddWithValue("@transaction_email", o.transaction_email);
			cmd.Parameters.AddWithValue("@credit_limit_safe", o.credit_limit_safe);
		}
		
		public static void Update(Cw_customer o)
		{
			string updatesql = @"UPDATE cw_customers SET alpha = @alpha,name = @name,address1 = @address1,address2 = @address2,address3 = @address3,address4 = @address4,address5 = @address5,town_city = @town_city,county = @county,state_region = @state_region,iso_country_code = @iso_country_code,country = @country,credit_category = @credit_category,export_indicator = @export_indicator,cust_disc_code = @cust_disc_code,currency = @currency,territory = @territory,class = @class,region = @region,invoice_customer = @invoice_customer,statement_customer = @statement_customer,group_customer = @group_customer,date_last_issue = @date_last_issue,date_created = @date_created,analysis_codes1 = @analysis_codes1,analysis_codes2 = @analysis_codes2,analysis_codes3 = @analysis_codes3,analysis_codes4 = @analysis_codes4,analysis_codes5 = @analysis_codes5,reminder_cat = @reminder_cat,settlement_code = @settlement_code,sett_days_code = @sett_days_code,price_list = @price_list,letter_code = @letter_code,balance_fwd = @balance_fwd,credit_limit = @credit_limit,ytd_sales = @ytd_sales,ytd_cost_of_sales = @ytd_cost_of_sales,cumulative_sales = @cumulative_sales,order_balance = @order_balance,sales_nl_cat = @sales_nl_cat,special_price = @special_price,vat_registration = @vat_registration,direct_debit = @direct_debit,invoices_printed = @invoices_printed,consolidated_inv = @consolidated_inv,comment_only_inv = @comment_only_inv,bank_account_no = @bank_account_no,bank_sort_code = @bank_sort_code,bank_name = @bank_name,bank_address1 = @bank_address1,bank_address2 = @bank_address2,bank_address3 = @bank_address3,bank_address4 = @bank_address4,analysis_code_6 = @analysis_code_6,produce_statements = @produce_statements,edi_customer = @edi_customer,vat_type = @vat_type,lang = @lang,delivery_method = @delivery_method,carrier = @carrier,vat_reg_number = @vat_reg_number,vat_exe_number = @vat_exe_number,paydays1 = @paydays1,paydays2 = @paydays2,paydays3 = @paydays3,bank_branch_code = @bank_branch_code,print_cp_with_stat = @print_cp_with_stat,payment_method = @payment_method,customer_class = @customer_class,sales_type = @sales_type,cp_lower_value = @cp_lower_value,address6 = @address6,fax = @fax,telex = @telex,btx = @btx,cp_charge = @cp_charge,control_digit = @control_digit,payer = @payer,responsibility = @responsibility,despatch_held = @despatch_held,credit_controller = @credit_controller,reminder_letters = @reminder_letters,severity_days1 = @severity_days1,severity_days2 = @severity_days2,severity_days3 = @severity_days3,severity_days4 = @severity_days4,severity_days5 = @severity_days5,severity_days6 = @severity_days6,delivery_reason = @delivery_reason,shipper_code1 = @shipper_code1,shipper_code2 = @shipper_code2,shipper_code3 = @shipper_code3,shipping_note_ind = @shipping_note_ind,account_type = @account_type,admin_fee = @admin_fee,intrest_rate = @intrest_rate,iban = @iban,bic = @bic,email = @email,transaction_email = @transaction_email,credit_limit_safe = @credit_limit_safe WHERE customer = @customer";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int customer)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM cw_customers WHERE customer = @id" , conn);
                cmd.Parameters.AddWithValue("@id", customer);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			