using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("order_header")]
	public class OrderHeaderTests : DatabaseTestBase
	{
		private CompanyTests companyTests;
		private CustProductsTests custProductsTests;
		private OrderLinesTests orderLinesTests;

		public OrderHeaderTests(IDbConnection conn) : base(conn)
		{

		}

		public OrderHeaderTests()
		{

		}

		private void InitObjects()
		{
			companyTests = new CompanyTests(conn);
			custProductsTests = new CustProductsTests(conn);
			orderLinesTests = new OrderLinesTests(conn);
		}

		[TestInitialize]
		public override void Init()
		{
			InitObjects();
			Cleanup();
		}

		[TestMethod]
		public void GetCombinedOrders()
		{
			var orders = new List<Order_header>
			{
				new Order_header
				{
					orderid = 1,
					custpo = "test"
				},
				new Order_header
				{
					orderid = 2,
					combined_order = 1
				}
			};
			GenerateTestData(orders);
			
			var orderHeaderDal = new OrderHeaderDAL(conn, null);
			var data = orderHeaderDal.GetCombinedOrders(1);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);
		}

		[TestMethod]
		public void GetClientsOnOrders()
		{
			var factoriesAndClients = new List<Company>
			{
				new Company { user_id = 1, user_type = (int) Company_User_Type.Factory, factory_code = "f1"},
				new Company { user_id = 2, user_type = (int) Company_User_Type.Factory, factory_code = "f2"},
				new Company { user_id = 3, user_type = (int) Company_User_Type.Factory, factory_code = "f3"},
				new Company { user_id = 4, user_type = (int) Company_User_Type.Client, customer_code = "c1"},
				new Company { user_id = 5, user_type = (int) Company_User_Type.Client, customer_code = "c2", combined_factory = 1},
				new Company { user_id = 6, user_type = (int) Company_User_Type.Client, customer_code = "c3", combined_factory = 1}
			};
			companyTests.GenerateTestData(factoriesAndClients);
			var products = new List<Cust_products>
			{
				new Cust_products { 
					cprod_code1 = "p1", 
					MastProduct = new Mast_products
					{
						factory_id = 1
					}
				},
				new Cust_products
				{
					cprod_code1 = "p2",
					MastProduct = new Mast_products
					{
						factory_id = 2
					}
				}
			};
			custProductsTests.GenerateTestData(products);
			var orders = new List<Order_header>
			{
				new Order_header
				{
					orderid = 1,
					userid1 = 5,
					Lines = new List<Order_lines>
					{
						new Order_lines
						{
							cprod_id = products[0].cprod_id
						}
					}
				},
				new Order_header
				{
					orderid = 2,
					userid1 = 6,
					Lines = new List<Order_lines>
					{
						new Order_lines
						{
							cprod_id = products[1].cprod_id
						}
					}
				}
			};
			GenerateTestData(orders);
			var orderHeaderDal = new OrderHeaderDAL(conn, null);
			var data = orderHeaderDal.GetClientsOnOrders();
			Assert.IsNotNull(data);
			Assert.AreEqual(2, data.Count);

			data = orderHeaderDal.GetClientsOnOrders(new[] {1 });
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			data = orderHeaderDal.GetClientsOnOrders(combined: true);
			Assert.IsNotNull(data);
			Assert.AreEqual(3, data.Count);
			Assert.AreEqual(-1, data[2].user_id);
			Assert.AreEqual("c2/c3", data[2].customer_code);
		}

		public void GenerateTestData(IEnumerable<Order_header> data, IDbConnection conn = null)
		{
			var c = conn ?? this.conn;
			PropertyInfo pInfo = null;

			foreach(var o in data)
			{
				GenerateRecord(o, pInfo, c);
				if(o.Lines != null)
				{
					foreach(var l in o.Lines)
					{
						l.orderid = o.orderid;					
					}
					orderLinesTests.GenerateTestData(o.Lines, c);
				}
			}
			
		}

		[TestCleanup]
		public override void Cleanup()
		{
			orderLinesTests?.Cleanup();
			base.Cleanup();
			custProductsTests?.Cleanup();
			companyTests?.Cleanup();
		}

		protected override string IdField => "orderid";
		protected override bool IsAutoKey => false;

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO `order_header`
				(`orderid`,`orderdate`,`userid1`,`locid`,`stock_order`,`status`,`new_status`,
				`delivery_address1`,`delivery_address2`,`delivery_address3`,`delivery_address4`,
				`delivery_address5`,`invoice_name`,`invoice_address1`,`invoice_address2`,
				`invoice_address3`,`invoice_address4`,`currency`,`surcharge`,`notes`,`custpo`,
				`eb_invoice`,`req_etd`,`original_eta`,`system_eta`,`req_eta`,`actual_eta`,
				`booked_in_date`,`loading_details`,`reference_no`,`factory_pl`,`lme`,`packing_list`,
				`edit_sort`,`mod_flag`,`eta_flag`,`process_id`,`combined_order`,`loading_factory`,
				`upload`,`upload_flag`,`entered_by`,`payment`,`documents`,`documents2`,`loading_date`,
				`container_type`,`loading_perc`,`forwarder_name`,`despatch_note`,`payment_terms_override`,
				`price_type_override`,`qc_show_override`,`excel_PL`,`pl_total_note`,`customs_inspected`,
				`eur_usd_exchange`,`gbp_usd_exchange`,`factory_count`,`location_override`,`BDi_VAT`,
				`Freight_value`,`BDI_invoice`,`BDi_import_fees`,`sale_date`,`freight_invoice_no`,
				`bdi_import_fees_invoice_no`,`eta_offset`)
				VALUES
				(@orderid,@orderdate,@userid1,@locid,@stock_order,@status,@new_status,@delivery_address1,
				@delivery_address2,@delivery_address3,@delivery_address4,@delivery_address5,
				@invoice_name,@invoice_address1,@invoice_address2,@invoice_address3,@invoice_address4,
				@currency,@surcharge,@notes,@custpo,@order_eb_invoice,@req_etd,@original_eta,@system_eta,
				@req_eta,@actual_eta,@booked_in_date,@loading_details,@reference_no,@factory_pl,
				@lme,@packing_list,@edit_sort,@mod_flag,@eta_flag,@process_id,@combined_order,
				@loading_factory,@upload,@upload_flag,@entered_by,@payment,@documents,@documents2,
				@loading_date,@container_type,@loading_perc,@forwarder_name,@despatch_note,
				@payment_terms_override,@price_type_override,@qc_show_override,@excel_PL,
				@pl_total_note,@customs_inspected,@eur_usd_exchange,@gbp_usd_exchange,
				@factory_count,@location_override,@BDi_VAT,@Freight_value,@BDI_invoice,@BDi_import_fees,
				@sale_date,@freight_invoice_no,@bdi_import_fees_invoice_no,@eta_offset)";
		}

		
		

	}
}
