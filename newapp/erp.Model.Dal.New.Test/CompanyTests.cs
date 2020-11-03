using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("users")]
	
	public class CompanyTests : DatabaseTestBase
	{
		private CompanyDAL companyDAL;
		private UserDAL userDAL;
		public CompanyTests()
		{

		}

		public CompanyTests(IDbConnection conn) : base(conn)
		{

		}

		[TestMethod]
		public void GetFactoriesByCombinedCode()
		{
			var data = new List<Company>
			{
				new Company{user_id = 1, combined_factory = 1},
				new Company{user_id = 2, combined_factory = 1},
				new Company{user_id = 3, combined_factory = 2},
			};
			GenerateTestData(data);

			var companyDal = new CompanyDAL(conn, null);
			var result = companyDal.GetFactoriesByCombinedCode(1);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);

		}

		[TestInitialize]
		public override void Init()
		{
			userDAL = new UserDAL(conn, null, null, null, null);
			companyDAL = new CompanyDAL(conn, userDAL);
		}

		
		
		protected override bool IsAutoKey => false;
		protected override string IdField => "user_id";

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO `users`
				(`user_id`,`user_name`,`reporting_name`,`reporting_filename`,`user_account`,
				`customer_code`,`distributor`,`oem_flag`,`user_status`,`clientlogo`,`brandlogo`,
				`brandname`,`brandurl`,`cashcredit`,`user_welcomename`,`factory_code`,`user_address1`,
				`user_address2`,`user_address3`,`user_address4`,`user_address5`,`user_address6`,
				`alt_inv_addressname`,`alt_inv_address1`,`alt_inv_address2`,`alt_inv_address3`,
				`alt_inv_address4`,`alt_inv_address5`,`user_country`,`user_country2`,`user_country_sort`,
				`user_contact`,`user_contact2`,`user_contact3`,`user_contact4`,`user_tel`,
				`user_fax`,`user_mobile`,`user_website`,`user_email`,`user_email2`,`user_email3`,
				`user_email4`,`user_type`,`user_access`,`user_pwd`,`user_created`,`user_curr`,
				`user_curr_pricing`,`dynamic_pricing`,`user_port`,`user_loading`,`user_price_type`,
				`user_deposits`,`user_payments_stage`,`user_payments_days`,`lastlogin`,`stock_req_1`,
				`stock_req_2`,`stock_req_3`,`stock_req_4`,`stock_req_5`,`reordering_freq`,
				`shipping_mark`,`qc_charge`,`qc_show`,`qc_spec`,`lme`,`factory_group`,`clearwater`,
				`containers`,`multi_location`,`product_catalogue`,`menu_co`,`menu_sc`,`menu_vp`,
				`menu_om`,`exchange_rate`,`exchange_rate2`,`consolidated_port_mix`,`consolidated_port2`,
				`consolidated_port`,`lcl_surcharge`,`customs_deduction`,`curr_pref_usd`,`curr_pref_gbp`,
				`curr_pref_eur`,`bank_name`,`bank_address`,`bank_swift`,`bank_account`,`bank_name2`,
				`bank_address2`,`bank_account2`,`bank_swift2`,`factory_usd`,`factory_gbp`,`factory_eur`,
				`vendor_name`,`vendor_add1`,`vendor_add2`,`vendor_add3`,`vendor_add4`,`hide_1`,`brand_b`,
				`brand_wc`,`brand_z`,`brand_cu`,`brand_tr`,`brand_bb`,`brand_st`,`brand_ac`,`brand_ar`,
				`lme2`,`agent_account`,`brsinv_comm`,`combined_factory`,`bs_code`,`bs_name`,`prod_days`,
				`ship_days`,`stock_days`,`2011_interface`,`os`,`com`,`om`,`prod`,`feed`,`corder`,
				`upload`,`eta_extra_days`,`surcharge`,`parent_id`,`order_management_option`,
				`loading_seq`,`dealer_distributor`,`stock_order_month_limit`,`test_account`,
				`invoice_sequence`,`invoice_logo`,`bbs_etd_from`,`non_processing_factory`,
				`showForBudgetActual`,`invoice_from`)
				VALUES
				(@user_id,@user_name,@reporting_name,@reporting_filename,@user_account,@customer_code,
				@distributor,@oem_flag,@user_status,@clientlogo,@brandlogo,@brandname,@brandurl,@cashcredit,
				@user_welcomename,@factory_code,@user_address1,@user_address2,@user_address3,@user_address4,
				@user_address5,@user_address6,@alt_inv_addressname,@alt_inv_address1,@alt_inv_address2,
				@alt_inv_address3,@alt_inv_address4,@alt_inv_address5,@user_country,@user_country2,
				@user_country_sort,@user_contact,@user_contact2,@user_contact3,@user_contact4,@user_tel,
				@user_fax,@user_mobile,@user_website,@user_email,@user_email2,@user_email3,@user_email4,
				@user_type,@user_access,@user_pwd,@user_created,@user_curr,@user_curr_pricing,
				@dynamic_pricing,@user_port,@user_loading,@user_price_type,@user_deposits,@user_payments_stage,
				@user_payments_days,@lastlogin,@stock_req_1,@stock_req_2,@stock_req_3,@stock_req_4,
				@stock_req_5,@reordering_freq,@shipping_mark,@qc_charge,@qc_show,@qc_spec,@lme,@factory_group,
				@clearwater,@containers,@multi_location,@product_catalogue,@menu_co,@menu_sc,@menu_vp,
				@menu_om,@exchange_rate,@exchange_rate2,@consolidated_port_mix,@consolidated_port2,
				@consolidated_port,@lcl_surcharge,@customs_deduction,@curr_pref_usd,@curr_pref_gbp,
				@curr_pref_eur,@bank_name,@bank_address,@bank_swift,@bank_account,@bank_name2,@bank_address2,
				@bank_account2,@bank_swift2,@factory_usd,@factory_gbp,@factory_eur,@vendor_name,
				@vendor_add1,@vendor_add2,@vendor_add3,@vendor_add4,@hide_1,@brand_b,@brand_wc,@brand_z,
				@brand_cu,@brand_tr,@brand_bb,@brand_st,@brand_ac,@brand_ar,@lme2,@agent_account,
				@brsinv_comm,@combined_factory,@bs_code,@bs_name,@prod_days,@ship_days,@stock_days,
				@EF_fix_2011_interface,@os,@com,@om,@prod,@feed,@corder,@upload,@eta_extra_days,@surcharge,
				@parent_id,@order_management_option,@loading_seq,@dealer_distributor,@stock_order_month_limit,
				@test_account,@invoice_sequence,@invoice_logo,@bbs_etd_from,@non_processing_factory,
				@showForBudgetActual,@invoice_from)";

		}

		[TestMethod]
		public void GetAll()
		{
			base.GetAll(companyDAL);
		}

		[TestMethod]
		public void GetById()
		{
			base.GetById(companyDAL);
		}

		[TestMethod]
		public void Create()
		{
			base.Create(companyDAL);
		}

		[TestMethod]
		public void Update()
		{
			base.Update(companyDAL);
		}

		[TestMethod]
		public void Delete()
		{
			base.Delete(companyDAL);
		}
	}
}
