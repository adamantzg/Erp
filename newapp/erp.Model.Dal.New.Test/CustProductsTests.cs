using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using asaq2.Model;
using System.Reflection;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("cust_products")]
	public class CustProductsTests : DatabaseTestBase
	{

		private CompanyTests companyTests;
		private MastProductTests mastProductTests;
		private OrderHeaderTests orderHeaderTests;
		private OrderLinesTests orderLinesTests;
		private CustproductsDAL custproductsDal;
		private BrandsDalTest brandsDalTest;

		public CustProductsTests()
		{

		}

		public CustProductsTests(IDbConnection conn) : base(conn)
		{
			InitObjects();
		}

		private void InitObjects()
		{
			mastProductTests = new MastProductTests(conn);
			companyTests = new CompanyTests(conn);
			brandsDalTest = new BrandsDalTest(conn);
			orderHeaderTests = new OrderHeaderTests(conn);
			orderLinesTests = new OrderLinesTests(conn);
		}

		
		[TestInitialize]
		public void Init()
		{
			InitObjects();
			Cleanup();
		}

		
		[TestMethod]
		public void GetProductGroupClassData()
		{
			
			var brandUserId = 4;
			var clients = new List<Company>
			{
				new Company {user_id = 1, distributor = 78, user_country = "UK", user_name = "uk distributor"},
				new Company {user_id = 2, distributor = 0, user_country = "UK", user_name = "uk non distributor"},
				new Company {user_id = 3, distributor = 78, user_country = "RU", user_name = "Non uk distributor"},
				new Company {user_id = brandUserId, distributor = 0, user_country = "UK", user_name = "Brand user"}
			};
			
			companyTests.GenerateTestData(clients);
			
			var mastProducts = new List<Mast_products>
			{
				new Mast_products {asaq_name = "non spare", category1 = 1},
				new Mast_products {asaq_name = "spare", category1 = Category1.category1_spares}
			};
			mastProductTests.GenerateTestData(mastProducts);
			var mast_nospare = mastProducts[0];
			var mast_spare = mastProducts[1];

			
			var brand = new Brand
			{
				brandname = "brand",
				user_id = 4
			};
			brandsDalTest.GenerateTestData(new[] {brand});

			var products = new List<Cust_products>
			{
				new Cust_products {cprod_code1 = "cns", cprod_name = "no spare", cprod_mast = mast_nospare.mast_id, product_group_id = 1, 
					brand_userid = brandUserId},
				new Cust_products {cprod_code1 = "cs", cprod_name = "spare", cprod_mast = mast_spare.mast_id, product_group_id = 2, 
					brand_userid = brandUserId}
			};
			GenerateTestData(products);

			
			var orders = new List<Order_header>
			{
				new Order_header { orderid = 1, custpo = "not in range", userid1 = clients[0].user_id, orderdate = DateTime.Now.AddMonths(-3)},
				new Order_header { orderid = 2, custpo = "in range, status x", userid1 = clients[0].user_id, orderdate = DateTime.Now.AddMonths(-1), status = "X"},
				new Order_header { orderid = 3, custpo = "in range", userid1 = clients[0].user_id, orderdate = DateTime.Now.AddMonths(-1), status = "N"},
				new Order_header { orderid = 4, custpo = "in range, non uk", userid1 = clients[2].user_id, orderdate = DateTime.Now.AddMonths(-1), status = "N"},
				new Order_header { orderid = 5, custpo = "in range, non dist", userid1 = clients[1].user_id, orderdate = DateTime.Now.AddMonths(-1), status = "N"}
			};
			orderHeaderTests.GenerateTestData(orders);
			
			var lines = new List<Order_lines>
			{
				new Order_lines { orderid = orders[0].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[0].cprod_id},
				new Order_lines { orderid = orders[0].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[1].cprod_id},
				new Order_lines { orderid = orders[1].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[0].cprod_id},
				new Order_lines { orderid = orders[1].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[1].cprod_id},
				new Order_lines { orderid = orders[2].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[0].cprod_id},
				new Order_lines { orderid = orders[2].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[1].cprod_id},
				new Order_lines { orderid = orders[3].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[0].cprod_id},
				new Order_lines { orderid = orders[3].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[1].cprod_id},
				new Order_lines { orderid = orders[4].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[0].cprod_id},
				new Order_lines { orderid = orders[4].orderid, orderqty = 1, unitprice = 100, unitcurrency = 1, cprod_id = products[1].cprod_id}
			};
			orderLinesTests.GenerateTestData(lines);

			var brandsDal = new BrandsDAL(conn);
			custproductsDal = new CustproductsDAL(conn, brandsDal);

			var from = DateTime.Now.AddMonths(-2);
			var to = DateTime.Now.AddMonths(1);
			var data = custproductsDal.GetProductGroupClassData(from, to);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);
			Assert.AreEqual(100, data[0].amount);
			Assert.AreEqual(1, data[0].qty);
			Assert.AreEqual(brand.brandname, data[0].brandname);
			Assert.AreEqual(products[0].cprod_code1, data[0].cprod_code1);
			Assert.AreEqual(products[0].cprod_name, data[0].cprod_name);
			Assert.AreEqual(products[0].product_group_id, data[0].old_status_id);

		}

		[TestMethod]
		public void GetByCompanies()
		{
			var companies = new List<Company>
			{
				new Company {user_id = 259, distributor = 78, user_country = "UK", user_name = "brand1"},
				new Company {user_id = 80, distributor = 0, user_country = "UK", user_name = "brand2"},
				new Company {user_id = 260, distributor = 0, user_country = "UK", user_name = "brand3"},
				new Company {user_id = 32, distributor = 0, user_country = "UK", user_name = "brand4"},
				new Company {user_id = 78, distributor = 0, user_country = "UK", user_name = "brand5"}
			};
			
			companyTests.GenerateTestData(companies);

			var brands = new List<Brand>
			{
				new Brand {brandname = "brand1", user_id = 259},
				new Brand {brandname = "brand2", user_id = 80},
				new Brand {brandname = "brand3", user_id = 260},
				new Brand {brandname = "brand4", user_id = 32},
				new Brand {brandname = "brand5", user_id = 78}
			};
			brandsDalTest.GenerateTestData(brands);

			var products = new List<Cust_products>();
			var textSamples =new[] {"text1", "text2", "text3"};
			foreach (var c in companies)
			{
				foreach (var t in textSamples)
				{
					products.Add(new Cust_products
					{
						cprod_code1 = t, cprod_name = "prod", brand_userid = c.user_id, cprod_status =  "N"
					});
					products.Add(new Cust_products
					{
						cprod_code1 = "code", cprod_name = t, brand_userid = c.user_id, cprod_status =  "N"
					});
				}
			}
			GenerateTestData(products);
			var brandsDal = new BrandsDAL(conn);
			custproductsDal = new CustproductsDAL(conn, brandsDal);
			var result = custproductsDal.GetByCompanies(new List<int?> {78}, "text1");
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);

			result = custproductsDal.GetByCompanies(new List<int?> {260}, "text1");
			Assert.IsNotNull(result);
			Assert.AreEqual(8, result.Count);

		}

		public void GenerateTestData(IEnumerable<Cust_products> data, IDbConnection conn = null)
		{
			var c = conn ?? this.conn;
			var factories = data.Select(x=>x.MastProduct?.Factory).Where(x=>x != null).ToList();
			if(companyTests == null)
				companyTests = new CompanyTests(c);
			if(mastProductTests == null)
				mastProductTests = new MastProductTests(c);
			companyTests.GenerateTestData(factories, c);
			
			//base.GenerateTestData(data, c);
			foreach(var cp in data)
			{
				PropertyInfo pInfo = null;
				if(cp.MastProduct != null)
				{
					if(cp.MastProduct.mast_id <= 0)
					{
						mastProductTests.GenerateRecord(cp.MastProduct);
						cp.MastProduct.mast_id = GetLastInsertId();
					}					
					cp.cprod_mast = cp.MastProduct.mast_id;
				}				
				GenerateRecord(cp, pInfo, c);
			}
		}

		public void GenerateAutoAddedProductsData(IEnumerable<auto_add_products> data, IDbConnection conn = null)
		{
			var c = conn ?? this.conn;
			foreach(var d in data)
			{
				c.Execute(GetAutoAddProductsCreateSql(), d);
				d.id = GetLastInsertId();
			}
		}

		protected override string IdField => "cprod_id";

		protected override string GetCreateSql()
		{
			return 
			@"INSERT INTO `cust_products`
			(`cprod_mast`,`cprod_user`,`cprod_name`,`cprod_name_web_override`,`cprod_name2`,`whitebook_cprod_name`,
			`cprod_code1`,`cprod_code1_web_override`,`whitebook_cprod_code1`,`cprod_code2`,`cprod_price1`,`cprod_price2`,
			`cprod_price3`,`cprod_price4`,`cprod_image1`,`cprod_instructions2`,`cprod_instructions`,`cprod_label`,
			`cprod_packaging`,`cprod_dwg`,`cprod_spares`,`cprod_pdf1`,`cprod_cgflag`,`cprod_curr`,`cprod_opening_qty`,
			`cprod_opening_date`,`cprod_status`,`cprod_oldcode`,`cprod_lme`,`cprod_brand_cat`,`cprod_brand_subcat`,
			`cprod_retail`,`cprod_retail_uk`,`cprod_retail_pending`,`cprod_retail_pending_date`,`cprod_retail_web_override`,
			`cprod_old_retail`,`cprod_override_margin`,`cprod_disc`,`cprod_seq`,`cprod_stock_code`,`days30_sales`,
			`brand_grouping`,`b_gold`,`cprod_loading`,`moq`,`WC_2011`,`cprod_stock`,`cprod_stock2`,`cprod_stock_date`,
			`cprod_stock_lvh`,`cprod_priority`,`cprod_status2`,`cprod_pending_price`,`cprod_pending_date`,
			`pack_image1`,`pack_image2`,`pack_image2b`,`pack_image2c`,`pack_image2d`,`pack_image3`,`pack_image4`,
			`aql_A`,`aql_D`,`aql_F`,`aql_M`,`insp_level_a`,`insp_level_D`,`insp_level_F`,`insp_level_M`,
			`criteria_status`,`cprod_confirmed`,`tech_template`,`tech_template2`,`cprod_returnable`,
			`client_cat1`,`client_cat2`,`client_image`,`cprod_track_image1`,`cprod_track_image2`,
			`cprod_track_image3`,`bs_visible`,`original_cprod_id`,`cprod_range`,`EU_supplier`,`on_order_qty`,
			`cprod_combined_product`,`UK_production`,`cprod_supplier`,`client_range`,`report_exception`,`brand_user_id`,
			`barcode`,`brand_id`,`buffer_stock_override_days`,`analytics_category`,`analytics_option`,`sale_retail`,
			`wras`,`warning_report`,`cprod_special_payment_terms`,`product_type`,`pending_discontinuation`,
			`cwb_stock_type`,`pallet_grouping`,`dist_status`,`proposed_discontinuation`,`analysis_d`,`locked_sorder_qty`,
			`color_id`,`consolidated_port_override`,`stock_check`,`cust_product_range_id`,`discontinued_visible`,
			`bin_location`,`product_group_id`)
			VALUES
			(@cprod_mast,@cprod_user,@cprod_name,@cprod_name_web_override,@cprod_name2,@whitebook_cprod_name,
			@cprod_code1,@cprod_code1_web_override,@whitebook_cprod_code1,@cprod_code2,@cprod_price1,@cprod_price2,
			@cprod_price3,@cprod_price4,@cprod_image1,@cprod_instructions2,@cprod_instructions,@cprod_label,
			@cprod_packaging,@cprod_dwg,@cprod_spares,@cprod_pdf1,@cprod_cgflag,@cprod_curr,
			@cprod_opening_qty,@cprod_opening_date,@cprod_status,@cprod_oldcode,@cprod_lme,@cprod_brand_cat,
			@cprod_brand_subcat,@cprod_retail,@cprod_retail_uk,@cprod_retail_pending,@cprod_retail_pending_date,
			@cprod_retail_web_override,@cprod_old_retail,@cprod_override_margin,@cprod_disc,@cprod_seq,
			@cprod_stock_code,@days30_sales,@brand_grouping,@b_gold,@cprod_loading,@moq,@WC_2011,
			@cprod_stock,@cprod_stock2,@cprod_stock_date,@cprod_stock_lvh,@cprod_priority,
			@cprod_status2,@cprod_pending_price,@cprod_pending_date,@pack_image1,@pack_image2,
			@pack_image2b,@pack_image2c,@pack_image2d,@pack_image3,@pack_image4,@aql_A,@aql_D,
			@aql_F,@aql_M,@insp_level_a,@insp_level_D,@insp_level_F,@insp_level_M,@criteria_status,
			@cprod_confirmed,@tech_template,@tech_template2,@cprod_returnable,@client_cat1,
			@client_cat2,@client_image,@cprod_track_image1,@cprod_track_image2,@cprod_track_image3,
			@bs_visible,@original_cprod_id,@cprod_range,@EU_supplier,@on_order_qty,@cprod_combined_product,
			@UK_production,@cprod_supplier,@client_range,@report_exception,@brand_userid,@barcode,
			@brand_id,@buffer_stock_override_days,@analytics_category,@analytics_option,@sale_retail,
			@wras,@warning_report,@cprod_special_payment_terms,@product_type,@pending_discontinuation,
			@cwb_stock_type,@pallet_grouping,@dist_status,@proposed_discontinuation,@analysis_d,@locked_sorder_qty,
			@color_id,@consolidated_port_override,@stock_check,@cust_product_range_id,@discontinued_visible,
			@bin_location,@product_group_id)";
		}

		protected string GetAutoAddProductsCreateSql()
		{
			return @"INSERT INTO `asaq`.`auto_add_products`
					(`id`,`trigger_cprod_id`,`added_cprod_id`,
					`unitprice`,`unitcurrency`,`startdate`)
					VALUES
					(@id,@trigger_cprod_id,@added_cprod_id,
					@unitprice,@unitcurrency,@startdate);
					";
		}

		[TestCleanup]
		public override void Cleanup()
		{
			orderLinesTests?.Cleanup();
			orderHeaderTests?.Cleanup();
			conn.Execute("DELETE FROM auto_add_products");
			base.Cleanup();			
			mastProductTests.Cleanup();
			brandsDalTest.Cleanup();
			companyTests.Cleanup();		

		}
	}
}
