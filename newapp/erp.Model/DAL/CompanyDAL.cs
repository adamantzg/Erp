using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class CompanyDAL
    {
        public static List<Company> GetDistributors(string brand)
        {
            List<Company> result = new List<Company>();
            string[] allowedBrands = { "b", "wc", "z", "cu", "tr", "bb", "st" };
            if (allowedBrands.Contains(brand))
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(string.Format(@"SELECT users.* FROM users WHERE brand_{0} = 1 AND hide_1 = 0",brand), conn);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Company u = GetFromDataReader(dr);
                        result.Add(u);
                    }
                    dr.Close();
                }
            }
            return result;
        }

        public static List<Company> GetDistributors(bool includeHidden = false)
        {
            List<Company> result = new List<Company>();
            
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE (distributor>0 OR dealer_distributor> 0) AND (hide_1 = @hide OR @hide IS NULL)", conn);
                cmd.Parameters.AddWithValue("@hide", !includeHidden ? (object) 0 : DBNull.Value);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }
            
            return result;
        }

        public static List<Company> GetClients(string prefixText = "")
        {
            List<Company> result = new List<Company>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE user_type = 3 AND (user_name LIKE @criteria OR customer_code LIKE @criteria OR @criteria IS NULL)", conn);
                cmd.Parameters.AddWithValue("@criteria", !string.IsNullOrEmpty(prefixText) ? (object) ("%" + prefixText + "%") : DBNull.Value);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }

        public static List<Company> GetClientsFromProducts()
        {
            var result = new List<Company>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE user_id IN (SELECT DISTINCT brand_user_id FROM cust_products)", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }

        public static List<Company> GetFactoriesForProducts()
        {
            var result = new List<Company>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE user_type = 1 AND user_id IN (SELECT DISTINCT factory_id FROM mast_products INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast) ", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }

        
        public static List<Company> GetFactories()
        {
            var result = new List<Company>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE user_type = 1", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }

        public static List<Company> GetFactoriesForLocation(int? location_id = null)
        {
            var result = new List<Company>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE user_type = 1 AND (consolidated_port = @location_id OR @location_id IS NULL)", conn);
                cmd.Parameters.AddWithValue("@location_id", Utilities.ToDBNull(location_id));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }

        public static List<Company> GetFactories(List<int> ids)
        {
            var result = new List<Company>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format("SELECT users.* FROM users WHERE user_type = 1 AND user_id IN ({0})",
                                                Utilities.CreateParametersFromIdList(cmd, ids));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }


        public static List<Company> GetFactoriesForUser(int userid)
        {
            var result = new List<Company>();
            var user = UserDAL.GetById(userid);

            var isAdmin = UserDAL.IsUserInRole(user.username, UserRole.Administrator);
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var sql = isAdmin
                              ? "SELECT users.* FROM users WHERE user_type = 1"
                              : @"SELECT users.* FROM users INNER JOIN admin_permissions ON users.user_id = admin_permissions.cusid WHERE user_type = 1 AND admin_permissions.userid = @userid";
                sql += " ORDER BY users.factory_code";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Company u = GetFromDataReader(dr);
                    result.Add(u);
                }
                dr.Close();
            }

            return result;
        }

        public static Company GetById(int id)
        {
            Company result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE user_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static Company GetByFactoryCode(string factory_code)
        {
            Company result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE factory_code = @fcode", conn);
                cmd.Parameters.AddWithValue("@fcode", factory_code);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static Company GetFactoryForProduct(int cprod_id)
        {
            Company result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT users.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                      INNER JOIN users ON users.user_id = mast_products.factory_id
                                                      WHERE cust_products.cprod_id = @cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Company> GetFactoriesForClients(IList<int> client_ids)
        {
            var result = new List<Company>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT DISTINCT users.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                      INNER JOIN users ON users.user_id = mast_products.factory_id
                                                      WHERE cust_products.cprod_user IN ({0})",
                                  Utilities.CreateParametersFromIdList(cmd, client_ids));
                
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static CompanyType GetCompanyType(int company_id)
        {
            if (GetMasterDistributors().Contains(company_id))
                return CompanyType.MasterDistributor;
            else if (GetHeadDistributors().Contains(company_id))
                return CompanyType.Distributor;
            else
            {
                Company company = GetById(company_id);
                if (company.distributor > 0)
                    return CompanyType.Distributor;
                else
                    return CompanyType.Manufacturer;
            }
        }

        public static List<Company> GetCompaniesByType(Company_User_Type user_type)
        {
            var result = new List<Company>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM users WHERE user_type = @user_type", conn);
                cmd.Parameters.AddWithValue("@user_type", user_type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<SiteEdit> GetSiteEdits(int company_id)
        {
            var result = new List<SiteEdit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT brands.brand_id,brands.brandname,brands.user_id,brands.`code`,brands.dealerstatus_view,brands.image,user_brand_lang.language_id,
                                                    user_brand_lang.canedit,languages.`code` AS lang_code,languages.`name` FROM brands INNER JOIN user_brand_lang ON user_brand_lang.brand_id = brands.brand_id
                                                    INNER JOIN languages ON user_brand_lang.language_id = languages.language_id
                                                    WHERE user_brand_lang.user_id = @company_id", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var se = new SiteEdit();
                    se.Brand = BrandsDAL.GetFromDataReader(dr);
                    var ce = Utilities.FromDbValue<long>(dr["canedit"]);
                    if (ce != null)
                        se.CanEdit = (ce != null && ce == 1);
                    se.Lang = new Language {language_id = (int) dr["language_id"], name = string.Empty + dr["name"], code = string.Empty + dr["lang_code"]};
                    result.Add(se);
                }
                dr.Close();
            }
            return result;
        }



        public static Company GetFromDataReader(MySqlDataReader dr)
	    {
		    Company o = new Company();
	
	        o.user_id = (int)(dr["user_id"]);
	        o.user_name = string.Empty + dr["user_name"];
	        o.reporting_name = string.Empty + dr["reporting_name"];
	        o.reporting_filename = string.Empty + dr["reporting_filename"];
	        o.user_account = string.Empty + dr["user_account"];
	        o.customer_code = string.Empty + dr["customer_code"];
	        o.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
	        o.oem_flag = Utilities.FromDbValue<int>(dr["oem_flag"]);
	        o.user_status = Utilities.FromDbValue<int>(dr["user_status"]);
	        o.clientlogo = string.Empty + dr["clientlogo"];
	        o.brandlogo = string.Empty + dr["brandlogo"];
	        o.brandname = string.Empty + dr["brandname"];
	        o.brandurl = string.Empty + dr["brandurl"];
	        o.cashcredit = Utilities.FromDbValue<int>(dr["cashcredit"]);
	        o.user_welcomename = string.Empty + dr["user_welcomename"];
	        o.user_address1 = string.Empty + dr["user_address1"];
	        o.factory_code = string.Empty + dr["factory_code"];
            //add 
          //  o.order_id = (int)(dr["orderid"]);
            //o.row_price=(double)(dr["rowprice"]);
	        o.user_address2 = string.Empty + dr["user_address2"];
	        o.user_address3 = string.Empty + dr["user_address3"];
	        o.user_address4 = string.Empty + dr["user_address4"];
	        o.user_address5 = string.Empty + dr["user_address5"];
	        o.user_address6 = string.Empty + dr["user_address6"];
	        o.user_country = string.Empty + dr["user_country"];
	        o.user_country2 = string.Empty + dr["user_country2"];
	        o.user_country_sort = Utilities.FromDbValue<int>(dr["user_country_sort"]);
	        o.user_contact = string.Empty + dr["user_contact"];
	        o.user_contact2 = string.Empty + dr["user_contact2"];
	        o.user_contact3 = string.Empty + dr["user_contact3"];
	        o.user_contact4 = string.Empty + dr["user_contact4"];
	        o.user_tel = string.Empty + dr["user_tel"];
	        o.user_fax = string.Empty + dr["user_fax"];
	        o.user_mobile = string.Empty + dr["user_mobile"];
	        o.user_website = string.Empty + dr["user_website"];
	        o.user_email = string.Empty + dr["user_email"];
	        o.user_email2 = string.Empty + dr["user_email2"];
	        o.user_email3 = string.Empty + dr["user_email3"];
	        o.user_email4 = string.Empty + dr["user_email4"];
	        o.user_type = Utilities.FromDbValue<int>(dr["user_type"]);
	        o.user_access = Utilities.FromDbValue<int>(dr["user_access"]);
	        o.user_pwd = string.Empty + dr["user_pwd"];
	        o.user_created = Utilities.FromDbValue<DateTime>(dr["user_created"]);
	        o.user_curr = Utilities.FromDbValue<int>(dr["user_curr"]);
	        o.user_curr_pricing = Utilities.FromDbValue<int>(dr["user_curr_pricing"]);
	        o.dynamic_pricing = Utilities.FromDbValue<int>(dr["dynamic_pricing"]);
	        o.user_port = string.Empty + dr["user_port"];
	        o.user_loading = string.Empty + dr["user_loading"];
	        o.user_price_type = string.Empty + dr["user_price_type"];
	        o.user_deposits = Utilities.FromDbValue<int>(dr["user_deposits"]);
	        o.user_payments_stage = Utilities.FromDbValue<int>(dr["user_payments_stage"]);
	        o.user_payments_days = Utilities.FromDbValue<int>(dr["user_payments_days"]);
	        o.lastlogin = Utilities.FromDbValue<DateTime>(dr["lastlogin"]);
	        o.stock_req_1 = Utilities.FromDbValue<double>(dr["stock_req_1"]);
	        o.stock_req_2 = Utilities.FromDbValue<double>(dr["stock_req_2"]);
	        o.stock_req_3 = Utilities.FromDbValue<double>(dr["stock_req_3"]);
	        o.stock_req_4 = Utilities.FromDbValue<double>(dr["stock_req_4"]);
	        o.stock_req_5 = Utilities.FromDbValue<double>(dr["stock_req_5"]);
	        o.reordering_freq = Utilities.FromDbValue<double>(dr["reordering_freq"]);
	        o.shipping_mark = string.Empty + dr["shipping_mark"];
	        o.qc_charge = Utilities.FromDbValue<double>(dr["qc_charge"]);
	        o.qc_show = Utilities.FromDbValue<int>(dr["qc_show"]);
	        o.qc_spec = Utilities.FromDbValue<int>(dr["qc_spec"]);
	        o.lme = Utilities.FromDbValue<int>(dr["lme"]);
	        o.factory_group = Utilities.FromDbValue<int>(dr["factory_group"]);
	        o.clearwater = Utilities.FromDbValue<int>(dr["clearwater"]);
	        o.containers = Utilities.FromDbValue<int>(dr["containers"]);
	        o.multi_location = Utilities.FromDbValue<int>(dr["multi_location"]);
	        o.product_catalogue = Utilities.FromDbValue<int>(dr["product_catalogue"]);
	        o.menu_co = Utilities.FromDbValue<int>(dr["menu_co"]);
	        o.menu_sc = Utilities.FromDbValue<int>(dr["menu_sc"]);
	        o.menu_vp = Utilities.FromDbValue<int>(dr["menu_vp"]);
	        o.menu_om = Utilities.FromDbValue<int>(dr["menu_om"]);
	        o.exchange_rate = Utilities.FromDbValue<double>(dr["exchange_rate"]);
	        o.exchange_rate2 = Utilities.FromDbValue<double>(dr["exchange_rate2"]);
	        o.consolidated_port_mix = Utilities.FromDbValue<int>(dr["consolidated_port_mix"]);
	        o.consolidated_port2 = Utilities.FromDbValue<int>(dr["consolidated_port2"]);
	        o.consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]);
	        o.lcl_surcharge = Utilities.FromDbValue<double>(dr["lcl_surcharge"]);
	        o.customs_deduction = Utilities.FromDbValue<double>(dr["customs_deduction"]);
	        o.curr_pref_usd = Utilities.FromDbValue<int>(dr["curr_pref_usd"]);
	        o.curr_pref_gbp = Utilities.FromDbValue<int>(dr["curr_pref_gbp"]);
	        o.curr_pref_eur = Utilities.FromDbValue<int>(dr["curr_pref_eur"]);
	        o.bank_name = string.Empty + dr["bank_name"];
	        o.bank_address = string.Empty + dr["bank_address"];
	        o.bank_swift = string.Empty + dr["bank_swift"];
	        o.bank_account = string.Empty + dr["bank_account"];
	        o.bank_name2 = string.Empty + dr["bank_name2"];
	        o.bank_address2 = string.Empty + dr["bank_address2"];
	        o.bank_account2 = string.Empty + dr["bank_account2"];
	        o.bank_swift2 = string.Empty + dr["bank_swift2"];
	        o.factory_usd = Utilities.FromDbValue<int>(dr["factory_usd"]);
	        o.factory_gbp = Utilities.FromDbValue<int>(dr["factory_gbp"]);
	        o.factory_eur = Utilities.FromDbValue<int>(dr["factory_eur"]);
	        o.vendor_name = string.Empty + dr["vendor_name"];
	        o.vendor_add1 = string.Empty + dr["vendor_add1"];
	        o.vendor_add2 = string.Empty + dr["vendor_add2"];
	        o.vendor_add3 = string.Empty + dr["vendor_add3"];
	        o.vendor_add4 = string.Empty + dr["vendor_add4"];
	        o.hide_1 = Utilities.FromDbValue<int>(dr["hide_1"]);
	        o.brand_b = Utilities.FromDbValue<int>(dr["brand_b"]);
	        o.brand_wc = Utilities.FromDbValue<int>(dr["brand_wc"]);
	        o.brand_z = Utilities.FromDbValue<int>(dr["brand_z"]);
	        o.brand_cu = Utilities.FromDbValue<int>(dr["brand_cu"]);
	        o.brand_tr = Utilities.FromDbValue<int>(dr["brand_tr"]);
	        o.brand_bb = Utilities.FromDbValue<int>(dr["brand_bb"]);
	        o.brand_st = Utilities.FromDbValue<int>(dr["brand_st"]);
	        o.lme2 = Utilities.FromDbValue<int>(dr["lme2"]);
	        o.agent_account = Utilities.FromDbValue<int>(dr["agent_account"]);
	        o.brsinv_comm = Utilities.FromDbValue<double>(dr["brsinv_comm"]);
	        o.combined_factory = Utilities.FromDbValue<int>(dr["combined_factory"]);
	        o.bs_code = string.Empty + dr["bs_code"];
	        o.bs_name = string.Empty + dr["bs_name"];
	        o.prod_days = Utilities.FromDbValue<int>(dr["prod_days"]);
	        o.ship_days = Utilities.FromDbValue<int>(dr["ship_days"]);
	        o.stock_days = Utilities.FromDbValue<int>(dr["stock_days"]);
	        o._2011_interface = Utilities.FromDbValue<int>(dr["2011_interface"]);
	        o.os = Utilities.FromDbValue<int>(dr["os"]);
	        o.com = Utilities.FromDbValue<int>(dr["com"]);
	        o.om = Utilities.FromDbValue<int>(dr["om"]);
	        o.prod = Utilities.FromDbValue<int>(dr["prod"]);
	        o.feed = Utilities.FromDbValue<int>(dr["feed"]);
	        o.corder = Utilities.FromDbValue<int>(dr["corder"]);
	        o.upload = Utilities.FromDbValue<int>(dr["upload"]);
	        o.eta_extra_days = Utilities.FromDbValue<int>(dr["eta_extra_days"]);
            o.loading_seq = Utilities.FromDbValue<int>(dr["loading_seq"]);
            o.dealer_distributor = Utilities.FromDbValue<int>(dr["dealer_distributor"]);

            return o;
    	}

        public static User GetMasterDistributorAccount(int distributor_id)
        { 
            //Now it is only Crosswater, in the future, this will be written in database
            User user = new User();
            user.company_id = Properties.Settings.Default.Crosswateruser_id;
            user.user_email = Properties.Settings.Default.AfterSalesMailAccount;
            return user;
        }

        public static List<int> GetMasterDistributors()
        {
            return (from s in Properties.Settings.Default.MasterDistributors.Split(',') select int.Parse(s)).ToList();
        }

        public static List<int> GetHeadDistributors()
        {
            return (from s in Properties.Settings.Default.HeadDistributors.Split(',') select int.Parse(s)).ToList();
        }

        public static List<Company> GetAllSiblings(int companyId)
        {
            List<Company> result = new List<Company>();
            Company company = CompanyDAL.GetById(companyId);
            if (company != null)
            {
                //result.Add(company);
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM users WHERE combined_factory = @id", conn);
                    cmd.Parameters.AddWithValue("@id", company.combined_factory);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                }
            }
            return result;
        }

        public static List<Brand> GetBrands(int company_id)
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                        "SELECT brands.* FROM brand_user INNER JOIN brands ON brand_user.brand_id = brands.brand_id WHERE brand_user.user_id = @user_id",
                        conn);
                cmd.Parameters.AddWithValue("@user_id", company_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(BrandsDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static Company GetByCustomerCode(string customerCode)
        {
            Company result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE customer_code = @code", conn);
                cmd.Parameters.AddWithValue("@code", customerCode);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
    }
}
