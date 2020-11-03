using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class CompanyDAL : GenericDal<Company>, ICompanyDAL
    {
		private IUserDAL userDal;
		private static object mapper = 1;

		public CompanyDAL(IDbConnection conn, IUserDAL userDal) : base(conn)
		{
			this.conn = (MySqlConnection) conn;
			this.userDal = userDal;

			lock(mapper)
		    {
			    try
			    {
				    if (!FluentMapper.EntityMaps.ContainsKey(typeof(Company)))
					    FluentMapper.Initialize(config => config.AddMap(new CompanyMap()));
			    }
			    catch (InvalidOperationException)
			    {
					//FluentMapper can raise this
			    }
		    }
		}

        
        public List<Company> GetDistributors(string brand)
        {
            string[] allowedBrands = { "b", "wc", "z", "cu", "tr", "bb", "st" };
            if (allowedBrands.Contains(brand))
            {
	            return conn.Query<Company>($"SELECT users.* FROM users WHERE brand_{brand} = 1 AND hide_1 = 0")
		            .ToList();
            }

	        return null;
        }

        public List<Company> GetDistributors(bool includeHidden = false)
        {
	        return conn.Query<Company>(
		        @"SELECT users.* FROM users WHERE (distributor>0 OR dealer_distributor> 0) AND (hide_1 = @hide OR @hide IS NULL)",
		        new {hide = !includeHidden ? (object) 0 : null}).ToList();
        }

        public List<Company> GetNonBrandClients()
        {
	        return conn.Query<Company>(@"SELECT users.* FROM users WHERE distributor=0 AND hide_1 = 0").ToList();
        }

        public List<Company> GetClients(string prefixText = "", IList<Company_User_Type> companyUserType = null )
        {
            List<Company> result = new List<Company>();

            if (companyUserType == null || companyUserType.Count == 0)
            {
                companyUserType = new List<Company_User_Type>();
                companyUserType.Add(Company_User_Type.Client);
            }

	        return conn.Query<Company>(@"SELECT users.* FROM users WHERE (user_type IN @companyUserType ) 
				AND (user_name LIKE @criteria OR customer_code LIKE @criteria OR @criteria IS NULL)",
		        new
		        {
			        companyUserType,
			        criteria = !string.IsNullOrEmpty(prefixText) ? (object) ("%" + prefixText + "%") : null
		        }).ToList();
        }

        public List<Company> GetFactories(string prefixText)
        {
	        return conn.Query<Company>(
		        @"SELECT users.* FROM users WHERE user_type = 1 AND (user_name LIKE @criteria OR factory_code LIKE @criteria OR @criteria IS NULL)",
		        new {criteria = !string.IsNullOrEmpty(prefixText) ? (object) ("%" + prefixText + "%") : null}).ToList();
        }

        public List<Company> GetClientsFromProducts()
        {
	        return conn.Query<Company>(
			        @"SELECT users.* FROM users WHERE user_id IN (SELECT DISTINCT brand_user_id FROM cust_products)")
		        .ToList();
        }

        public List<Company> GetFactoriesForProducts()
        {

	        return conn.Query<Company>(@"SELECT users.* FROM users WHERE user_type = 1 
				AND user_id IN (SELECT DISTINCT factory_id 
					FROM mast_products INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast) ")
		        .ToList();
        }
        
        //public List<Company> GetFactories(bool combined = false)
        //{
	       // return conn.Query<Company>(@"SELECT users.* FROM users WHERE user_type = 1").ToList();
        //}

		public List<Company> GetFactories(bool combined = false, int? location_id = null, bool? files = null)
		{
			var result = new List<Company>();
			if(files == true)
			{
				conn.Query<Company, File, Company>(
					$@"SELECT users.*, file.* FROM users LEFT OUTER JOIN users_file ON users.user_id = users_file.user_id
						LEFT OUTER JOIN file ON users_file.file_id = file.id WHERE users.user_type = {(int) Company_User_Type.Factory} 
						AND (users.consolidated_port = @location_id OR @location_id IS NULL)",
					(c, f) =>
					{
						var comp = result.FirstOrDefault( x => x.user_id == c.user_id);
						if(comp == null)
						{
							comp = c;
							comp.Files = new List<File>();
							result.Add(comp);
						}
						comp.Files.Add(f);
						return comp;
					}, new {location_id}, splitOn: "id");
			} else
			{
				result = conn.Query<Company>($@"SELECT users.* FROM users WHERE users.user_type = {(int) Company_User_Type.Factory} 
						AND (users.consolidated_port = @location_id OR @location_id IS NULL) ", new { location_id }).ToList();
			}
			
            if(combined)
				result.AddRange(conn.Query<Company>($@"SELECT users.* FROM users WHERE users.user_type = {(int) Company_User_Type.Factory} 
						AND users.combined_factory > 0
						AND (users.consolidated_port = @location_id OR @location_id IS NULL) ", new { location_id }).ToList()
						.GroupBy(c => c.combined_factory).ToList()
						.Select(g=>new Company{user_id = -1*g.Key.Value,factory_code = GetCombinedFactoryCode(g.First().factory_code)})
						);
            return result;
		}

		public List<Company> GetFactories(IList<int> ids)
        {
	        return conn.Query<Company>(@"SELECT users.* FROM users WHERE user_type = 1 AND user_id IN @ids", new {ids}).ToList();
        }

        public List<Company> GetFactoriesForLocation(int? location_id = null)
        {
	        return conn.Query<Company>(
			        @"SELECT users.* FROM users WHERE user_type = 1 AND (consolidated_port = @location_id OR @location_id IS NULL OR EXISTS
						(SELECT order_header.orderid FROM order_header INNER JOIN order_lines ON order_header.orderid = order_lines.orderid
						INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
						INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
						WHERE mast_products.factory_id = users.user_id AND order_header.location_override = @location_id AND order_lines.orderqty > 0))",
			        new {location_id})
		        .ToList();
        }

        public List<Company> GetByIds(List<int> ids)
        {
	        return conn.Query<Company>("SELECT users.* FROM users WHERE user_id IN @ids", new {ids}).ToList();
        }

        public List<Company> GetCompaniesForUser(int userid, Company_User_Type type = Company_User_Type.Factory)
        {
			var user = userDal.GetById(userid);
			var isAdmin = userDal.IsUserInRole(user.username, UserRole.Administrator) && 
				!userDal.IsUserInRole(user.username, UserRole.ClientController) && 
				!userDal.IsUserInRole(user.username, UserRole.FactoryController);
            
            var sql = isAdmin
                          ? "SELECT users.* FROM users WHERE user_type = @type"
                          : @"SELECT users.* FROM users INNER JOIN admin_permissions ON users.user_id = admin_permissions.cusid WHERE user_type = @type AND admin_permissions.userid = @userid";
            sql += type == Company_User_Type.Factory ? " ORDER BY users.factory_code" : " ORDER BY users.customer_code";

	        return conn.Query<Company>(sql, new {type, userid}).ToList();

        }

        public Company GetByFactoryCode(string factory_code)
        {
	        return conn.QueryFirstOrDefault<Company>("SELECT * FROM users WHERE factory_code = @factory_code",
		        new {factory_code});
        }

        public Company GetFactoryForProduct(int cprod_id)
        {
	        return conn.QueryFirstOrDefault<Company>(
		        @"SELECT users.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                      INNER JOIN users ON users.user_id = mast_products.factory_id
                                                      WHERE cust_products.cprod_id = @cprod_id", new {cprod_id});
        }

        public List<Company> GetFactoriesForClients(IList<int> client_ids)
        {
	        return conn.Query<Company>(
			        @"SELECT DISTINCT users.* FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                      INNER JOIN users ON users.user_id = mast_products.factory_id
                                                      WHERE cust_products.cprod_user IN @client_ids", new {client_ids})
		        .ToList();
        }

        public CompanyType GetCompanyType(int company_id)
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

        public List<Company> GetCompaniesByType(Company_User_Type user_type)
        {
	        return conn.Query<Company>("SELECT * FROM users WHERE user_type = @user_type", new {user_type}).ToList();
        }
		
        
        public List<int> GetMasterDistributors()
        {
            return (from s in Properties.Settings.Default.MasterDistributors.Split(',') select int.Parse(s)).ToList();
        }

        public List<int> GetHeadDistributors()
        {
            return (from s in Properties.Settings.Default.HeadDistributors.Split(',') select int.Parse(s)).ToList();
        }

        public List<Company> GetAllSiblings(int companyId)
        {
            var company = GetById(companyId);
            if (company != null)
            {
	            return conn.Query<Company>("SELECT * FROM users WHERE combined_factory = @id", company.combined_factory)
		            .ToList();
            }
			return null;
        }

        public List<Company> GetFactoriesByCombinedCode(int combined_factory)
        {
	        return conn.Query<Company>("SELECT * FROM users WHERE combined_factory = @combined_factory",
		        new {combined_factory}).ToList();
        }

        public List<Brand> GetBrands(int company_id)
        {
	        return conn.Query<Brand>(
		        @"SELECT brands.* FROM brand_user INNER JOIN brands ON brand_user.brand_id = brands.brand_id 
				WHERE brand_user.user_id = @company_id", new {company_id}).ToList();
        }

        public Company GetByCustomerCode(string customerCode)
        {
	        return conn.QueryFirstOrDefault<Company>("SELECT * FROM users WHERE customer_code = @customerCode",
		        new {customerCode});
        }

		public void RemoveFile(int companyId, int fileId)
		{
			conn.Execute("DELETE FROM users_file WHERE user_id = @companyId AND file_id = @fileId", new { companyId, fileId});
		}

		public void UpdateFiles(Company c, int? fileType = null)
		{
			if(c.Files != null)
			{
				if(conn.State != ConnectionState.Open) 
					conn.Open();
				var tr = conn.BeginTransaction();
				try
				{
					conn.Execute(@"DELETE FROM users_file 
						WHERE (@fileType IS NULL OR file_id IN (SELECT id FROM file WHERE type_id = @fileType)) AND user_id = @user_id",
						new { c.user_id, fileType });
					foreach (var f in c.Files)
					{
						conn.Execute("INSERT INTO users_file (user_id, file_id) VALUES (@user_id, @file_id)", new { c.user_id, file_id = f.id });
					}
					tr.Commit();
				}
				catch
				{
					tr.Rollback();
					throw;
				}
			}
		}

		public List<Company> GetFactoriesWithOrders(DateTime? orderDateLimit = null)
		{
			return conn.Query<Company>(
				@"SELECT * FROM users WHERE user_id IN (SELECT factory_id FROM order_lines
					INNER JOIN order_header ON order_lines.orderid = order_header.orderid
					INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
					WHERE (linedate >= @dateLimit OR @dateLimit IS NULL)
					AND orderqty > 0
					AND order_header.status = 'N'
					", new {dateLimit = orderDateLimit}
				).ToList();
		}

		public List<Company> GetClientsWithOrders(DateTime? orderDateLimit = null)
		{
			return conn.Query<Company>(
				@"SELECT * FROM users WHERE user_id IN (SELECT userid1 FROM order_lines
					INNER JOIN order_header ON order_lines.orderid = order_header.orderid
					WHERE (linedate >= @dateLimit OR @dateLimit IS NULL)
					AND orderqty > 0
					AND order_header.status = 'N'
					", new {dateLimit = orderDateLimit}
				).ToList();
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM users ";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM users WHERE user_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO `users`
					(`user_id`,`user_name`,`reporting_name`,`reporting_filename`,`user_account`,
					`customer_code`,`distributor`,`oem_flag`,`user_status`,`clientlogo`,
					`brandlogo`,`brandname`,`brandurl`,`cashcredit`,`user_welcomename`,
					`factory_code`,`user_address1`,`user_address2`,`user_address3`,
					`user_address4`,`user_address5`,`user_address6`,`alt_inv_addressname`,
					`alt_inv_address1`,`alt_inv_address2`,`alt_inv_address3`,`alt_inv_address4`,
					`alt_inv_address5`,`user_country`,`user_country2`,`user_country_sort`,
					`user_contact`,`user_contact2`,`user_contact3`,`user_contact4`,
					`user_tel`,`user_fax`,`user_mobile`,`user_website`,`user_email`,
					`user_email2`,`user_email3`,`user_email4`,`user_type`,`user_access`,
					`user_pwd`,`user_created`,`user_curr`,`user_curr_pricing`,
					`dynamic_pricing`,`user_port`,`user_loading`,`user_price_type`,
					`user_deposits`,`user_payments_stage`,`user_payments_days`,
					`lastlogin`,`stock_req_1`,`stock_req_2`,`stock_req_3`,`stock_req_4`,
					`stock_req_5`,`reordering_freq`,`shipping_mark`,`qc_charge`,
					`qc_show`,`qc_spec`,`lme`,`factory_group`,`clearwater`,`containers`,
					`multi_location`,`product_catalogue`,`menu_co`,`menu_sc`,
					`menu_vp`,`menu_om`,`exchange_rate`,`exchange_rate2`,
					`consolidated_port_mix`,`consolidated_port2`,`consolidated_port`,
					`lcl_surcharge`,`customs_deduction`,`curr_pref_usd`,`curr_pref_gbp`,
					`curr_pref_eur`,`bank_name`,`bank_address`,`bank_swift`,
					`bank_account`,`bank_name2`,`bank_address2`,`bank_account2`,
					`bank_swift2`,`factory_usd`,`factory_gbp`,`factory_eur`,
					`vendor_name`,`vendor_add1`,`vendor_add2`,`vendor_add3`,`vendor_add4`,
					`hide_1`,`brand_b`,`brand_wc`,`brand_z`,`brand_cu`,`brand_tr`,
					`brand_bb`,`brand_st`,`brand_ac`,`brand_ar`,`lme2`,`agent_account`,
					`brsinv_comm`,`combined_factory`,`bs_code`,`bs_name`,`prod_days`,
					`ship_days`,`stock_days`,`2011_interface`,`os`,`com`,`om`,
					`prod`,`feed`,`corder`,`upload`,`eta_extra_days`,`surcharge`,`parent_id`,
					`order_management_option`,`loading_seq`,`dealer_distributor`,
					`stock_order_month_limit`,`test_account`,`invoice_sequence`,
					`invoice_logo`,`bbs_etd_from`,`non_processing_factory`,`showForBudgetActual`,
					`invoice_from`,`vat_rate`)
					VALUES
					(@user_id,@user_name,@reporting_name,@reporting_filename,@user_account,
					@customer_code,@distributor,@oem_flag,@user_status,@clientlogo,
					@brandlogo,@brandname,@brandurl,@cashcredit,@user_welcomename,@factory_code,
					@user_address1,@user_address2,@user_address3,@user_address4,@user_address5,
					@user_address6,@alt_inv_addressname,@alt_inv_address1,@alt_inv_address2,@alt_inv_address3,
					@alt_inv_address4,@alt_inv_address5,@user_country,@user_country2,@user_country_sort,
					@user_contact,@user_contact2,@user_contact3,@user_contact4,@user_tel,
					@user_fax,@user_mobile,@user_website,@user_email,@user_email2,@user_email3,
					@user_email4,@user_type,@user_access,@user_pwd,@user_created,@user_curr,
					@user_curr_pricing,@dynamic_pricing,@user_port,@user_loading,@user_price_type,
					@user_deposits,@user_payments_stage,@user_payments_days,@lastlogin,@stock_req_1,
					@stock_req_2,@stock_req_3,@stock_req_4,@stock_req_5,@reordering_freq,
					@shipping_mark,@qc_charge,@qc_show,@qc_spec,@lme,@factory_group,@clearwater,
					@containers,@multi_location,@product_catalogue,@menu_co,@menu_sc,@menu_vp,
					@menu_om,@exchange_rate,@exchange_rate2,@consolidated_port_mix,@consolidated_port2,
					@consolidated_port,@lcl_surcharge,@customs_deduction,@curr_pref_usd,@curr_pref_gbp,
					@curr_pref_eur,@bank_name,@bank_address,@bank_swift,@bank_account,@bank_name2,
					@bank_address2,@bank_account2,@bank_swift2,@factory_usd,@factory_gbp,@factory_eur,
					@vendor_name,@vendor_add1,@vendor_add2,@vendor_add3,@vendor_add4,@hide_1,
					@brand_b,@brand_wc,@brand_z,@brand_cu,@brand_tr,@brand_bb,@brand_st,@brand_ac,
					@brand_ar,@lme2,@agent_account,@brsinv_comm,@combined_factory,@bs_code,
					@bs_name,@prod_days,@ship_days,@stock_days,@EF_fix_2011_interface,@os,@com,
					@om,@prod,@feed,@corder,@upload,@eta_extra_days,@surcharge,@parent_id,
					@order_management_option,@loading_seq,@dealer_distributor,@stock_order_month_limit,@test_account,
					@invoice_sequence,@invoice_logo,@bbs_etd_from,@non_processing_factory,@showForBudgetActual,
					@invoice_from,@vat_rate)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE `users`	SET
				`user_id` = @user_id,`user_name` = @user_name,`reporting_name` = @reporting_name,
				`reporting_filename` = @reporting_filename,`user_account` = @user_account,
				`customer_code` = @customer_code,`distributor` = @distributor,
				`oem_flag` = @oem_flag,`user_status` = @user_status,`clientlogo` = @clientlogo,
				`brandlogo` = @brandlogo,`brandname` = @brandname,`brandurl` = @brandurl,
				`cashcredit` = @cashcredit,`user_welcomename` = @user_welcomename,
				`factory_code` = @factory_code,`user_address1` = @user_address1,`user_address2` = @user_address2,
				`user_address3` = @user_address3,`user_address4` = @user_address4,`user_address5` = @user_address5,
				`user_address6` = @user_address6,`alt_inv_addressname` = @alt_inv_addressname,
				`alt_inv_address1` = @alt_inv_address1,`alt_inv_address2` = @alt_inv_address2,
				`alt_inv_address3` = @alt_inv_address3,`alt_inv_address4` = @alt_inv_address4,
				`alt_inv_address5` = @alt_inv_address5,`user_country` = @user_country,
				`user_country2` = @user_country2,`user_country_sort` = @user_country_sort,
				`user_contact` = @user_contact,`user_contact2` = @user_contact2,
				`user_contact3` = @user_contact3,`user_contact4` = @user_contact4,
				`user_tel` = @user_tel,`user_fax` = @user_fax,`user_mobile` = @user_mobile,
				`user_website` = @user_website,`user_email` = @user_email,`user_email2` = @user_email2,
				`user_email3` = @user_email3,`user_email4` = @user_email4,
				`user_type` = @user_type,`user_access` = @user_access,
				`user_pwd` = @user_pwd,`user_created` = @user_created,`user_curr` = @user_curr,
				`user_curr_pricing` = @user_curr_pricing,`dynamic_pricing` = @dynamic_pricing,
				`user_port` = @user_port,`user_loading` = @user_loading,`user_price_type` = @user_price_type,
				`user_deposits` = @user_deposits,`user_payments_stage` = @user_payments_stage,
				`user_payments_days` = @user_payments_days,`lastlogin` = @lastlogin,
				`stock_req_1` = @stock_req_1,`stock_req_2` = @stock_req_2,
				`stock_req_3` = @stock_req_3,`stock_req_4` = @stock_req_4,
				`stock_req_5` = @stock_req_5,`reordering_freq` = @reordering_freq,
				`shipping_mark` = @shipping_mark,`qc_charge` = @qc_charge,`qc_show` = @qc_show,
				`qc_spec` = @qc_spec,`lme` = @lme,`factory_group` = @factory_group,
				`clearwater` = @clearwater,`containers` = @containers,`multi_location` = @multi_location,
				`product_catalogue` = @product_catalogue,`menu_co` = @menu_co,
				`menu_sc` = @menu_sc,`menu_vp` = @menu_vp,`menu_om` = @menu_om,
				`exchange_rate` = @exchange_rate,`exchange_rate2` = @exchange_rate2,
				`consolidated_port_mix` = @consolidated_port_mix,`consolidated_port2` = @consolidated_port2,
				`consolidated_port` = @consolidated_port,`lcl_surcharge` = @lcl_surcharge,
				`customs_deduction` = @customs_deduction,`curr_pref_usd` = @curr_pref_usd,
				`curr_pref_gbp` = @curr_pref_gbp,`curr_pref_eur` = @curr_pref_eur,
				`bank_name` = @bank_name,`bank_address` = @bank_address,
				`bank_swift` = @bank_swift,`bank_account` = @bank_account,
				`bank_name2` = @bank_name2,`bank_address2` = @bank_address2,
				`bank_account2` = @bank_account2,`bank_swift2` = @bank_swift2,
				`factory_usd` = @factory_usd,`factory_gbp` = @factory_gbp,
				`factory_eur` = @factory_eur,`vendor_name` = @vendor_name,
				`vendor_add1` = @vendor_add1,`vendor_add2` = @vendor_add2,
				`vendor_add3` = @vendor_add3,`vendor_add4` = @vendor_add4,
				`hide_1` = @hide_1,`brand_b` = @brand_b,`brand_wc` = @brand_wc,
				`brand_z` = @brand_z,`brand_cu` = @brand_cu,`brand_tr` = @brand_tr,
				`brand_bb` = @brand_bb,`brand_st` = @brand_st,`brand_ac` = @brand_ac,
				`brand_ar` = @brand_ar,`lme2` = @lme2,`agent_account` = @agent_account,
				`brsinv_comm` = @brsinv_comm,`combined_factory` = @combined_factory,
				`bs_code` = @bs_code,`bs_name` = @bs_name,`prod_days` = @prod_days,
				`ship_days` = @ship_days,`stock_days` = @stock_days,`2011_interface` = @EF_fix_2011_interface,
				`os` = @os,`com` = @com,`om` = @om,`prod` = @prod,`feed` = @feed,
				`corder` = @corder,`upload` = @upload,`eta_extra_days` = @eta_extra_days,
				`surcharge` = @surcharge,`parent_id` = @parent_id,
				`order_management_option` = @order_management_option,
				`loading_seq` = @loading_seq,`dealer_distributor` = @dealer_distributor,
				`stock_order_month_limit` = @stock_order_month_limit,`test_account` = @test_account,
				`invoice_sequence` = @invoice_sequence,`invoice_logo` = @invoice_logo,
				`bbs_etd_from` = @bbs_etd_from,`non_processing_factory` = @non_processing_factory,
				`showForBudgetActual` = @showForBudgetActual,`invoice_from` = @invoice_from,
				`vat_rate` = @vat_rate
				WHERE `user_id` = @user_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM users WHERE user_id = @id";
		}

		private string GetCombinedFactoryCode(string factoryCode)
        {
            if (string.IsNullOrEmpty(factoryCode))
                return string.Empty;
            return factoryCode.Substring(0, factoryCode.Length > 1 ? factoryCode.Length - 1 : 1);
        }

		public List<Company> GetFactoriesForBrandAndCategory(int brand_id, int? category1 = null)
		{
			return conn.Query<Company>(
				@"SELECT * FROM users WHERE user_id IN (SELECT DISTINCT factory_id FROM mast_products INNER JOIN cust_products ON 
				mast_products.mast_id = cust_products.cprod_mast WHERE cust_products.cprod_status <> 'D' AND cust_products.brand_id = @brand_id AND 
				(mast_products.category1 = @category1 OR @category1 IS NULL))", new { brand_id, category1 }).ToList();
		}
	}

	public class CompanyMap : EntityMap<Company>
	{
		public CompanyMap()
		{
			Map(u => u.EF_fix_2011_interface).ToColumn("2011_interface");			
		}
	}
}
