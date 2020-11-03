using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using asaq2.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	public class AnalyticsDalTests : DatabaseTestBase
	{
		private CompanyTests companyTests;
		private MastProductTests mastProductTests;
		private CustProductsTests custProductsTests;
		private OrderHeaderTests orderHeaderTests;
		private OrderLinesTests orderLinesTests;
		private CustproductsDAL custproductsDal;
		private BrandsDalTest brandsDalTest;
		private PorderHeaderTest porderHeaderTest;
		private POrderLinesTests pOrderLinesTests;

		private DateTime fromDate;
		private DateTime toDate;
		private int from, to;

		private List<Mast_products> mastProducts;
		private List<Cust_products> products;
		private string[] statuses;
		private DateTime[] dates;
		private List<Order_header> orders;
		private List<Porder_header> pOrders;
		private List<Order_lines> lines;
		private List<Porder_lines> pOrderLines;
		private List<Company> clients;
		private int brandUserId;
		private const int startBrandUserId = 4;
		private List<Brand> brands;


		public AnalyticsDalTests()
		{

		}

		public AnalyticsDalTests(IDbConnection conn) : base(conn)
		{

		}

		
		[TestInitialize]
		public void Init()
		{
			mastProductTests = new MastProductTests(conn);
			companyTests = new CompanyTests(conn);
			brandsDalTest = new BrandsDalTest(conn);
			orderHeaderTests = new OrderHeaderTests(conn);
			orderLinesTests = new OrderLinesTests(conn);
			pOrderLinesTests = new POrderLinesTests(conn);
			porderHeaderTest = new PorderHeaderTest(conn);
			custProductsTests = new CustProductsTests(conn);
			Cleanup();

			
			brandUserId = startBrandUserId;
			clients = new List<Company>
			{
				new Company {user_id = 1, distributor = 78, user_country = "UK", user_name = "normal customer", user_type = 2, customer_code = "c"},
				new Company {user_id = 2, distributor = 78, user_country = "UK", user_name = "excluded customer", user_type = 2, customer_code = "ex"},
				new Company {user_id = 3, distributor = 78, user_country = "RU", user_name = "Non uk customer", user_type = 2, customer_code = "c"},
				new Company {user_id = brandUserId++, user_country = "UK", user_name = "included client", user_type = 2},
				new Company {user_id = brandUserId++, user_country = "UK", user_name = "excluded client", user_type = 2},
				new Company {user_id = brandUserId++, user_country = "UK", user_name = "brand client", user_type = 2},
				new Company {user_id = 100, user_name = "factory", user_type = 1}
			};
			
			companyTests.GenerateTestData(clients);
			
			mastProducts = new List<Mast_products>
			{
				new Mast_products {factory_ref = "ns", asaq_name = "non spare", category1 = 1, factory_id = 100},
				new Mast_products {factory_ref = "s", asaq_name = "spare", category1 = Category1.category1_spares, factory_id = 100}
			};
			//mastProductTests.GenerateTestData(mastProducts);
			var mastNospare = mastProducts[0];
			var mastSpare = mastProducts[1];

			brands = new List<Brand>();
			for (int i = startBrandUserId; i < startBrandUserId + 3; i++)
			{
				brands.Add(new Brand
				{
					brandname = "brand" + i,
					user_id = i
				});	
			}
			
			brandsDalTest.GenerateTestData(brands);

			products = new List<Cust_products>();
			foreach (var mp in mastProducts)
			{
				foreach (var cl in clients.Where(x => x.user_id >= startBrandUserId && x.user_type == 2))
				{
					products.Add(new Cust_products
					{
						cprod_code1 = $"{mp.factory_ref} {cl.user_id}", 
						cprod_name = $"{mp.asaq_name} {cl.user_id}", 
						//cprod_mast = mp.mast_id, 
						brand_userid = cl.user_id,
						cprod_user = cl.user_id, 
						MastProduct = mp,
						cprod_cgflag = 0	//brand_sales_analysis_product2 doesn't have brand_user_id comparison but uses this field
					});
				}
			}
			
			custProductsTests.GenerateTestData(products);

			var offset = 6;
			fromDate = DateTime.Now.AddMonths(-1 * offset);
			toDate = DateTime.Now.AddDays(-1);
			from = Month21.FromDate(fromDate).Value;
			to = Month21.FromDate(toDate).Value;

			statuses = new[] {"X", "N"};
			dates = new[] {fromDate.AddMonths(-1), fromDate.AddMonths(1)};

			orders = new List<Order_header>();
			var orderid = 1;
			foreach (var s in statuses)
			{
				foreach (var d in dates)
				{
					foreach (var cl in clients.Where(x => x.user_id < startBrandUserId))
					{
						orders.Add(new Order_header
						{
							orderid = orderid++,
							userid1 = cl.user_id,
							orderdate = d,
							status = s,
							Client = cl
						});
					}
				}
			};
			
			orderHeaderTests.GenerateTestData(orders);

			var porderid = 1000;
			pOrders = new List<Porder_header>();
			foreach (var o in orders)
			{
				pOrders.Add(new Porder_header
				{
					porderid = porderid++,
					soorderid = o.orderid,
					po_req_etd = o.orderdate.AddDays(10)
				});
			}
			porderHeaderTest.GenerateTestData(pOrders);

			lines = new List<Order_lines>();
			foreach (var o in orders)
			{
				foreach (var p in products)
				{
					lines.Add(new Order_lines
					{
						orderid = o.orderid, 
						orderqty = 1, 
						unitprice = 100, 
						unitcurrency = 1, 
						cprod_id = p.cprod_id,
						Cust_Product = p,
						Header = o,
						description = string.Empty //view doesn't allow null
					});
				}
			}
			
			orderLinesTests.GenerateTestData(lines);

			pOrderLines = new List<Porder_lines>();
			
			foreach (var l in lines)
			{
				pOrderLines.Add(new Porder_lines
				{
					soline = l.linenum,
					orderqty = l.orderqty,
					unitcurrency = 0,
					unitprice = l.unitprice - 10,
					cprod_id = l.cprod_id,
					porderid = pOrders.FirstOrDefault(x=>x.soorderid == l.orderid)?.porderid
				});
			}
			pOrderLinesTests.GenerateTestData(pOrderLines);

		}

		[TestMethod]
		public void GetProductSales()
		{
			//Testing only for brands - brand_sales_analysis_product2

			var countriesDal = new CountriesDAL(conn);
			var analyticsDal = new AnalyticsDAL(conn,countriesDal);

			//1. from - to month21
			var data = analyticsDal.GetProductSales(from: @from, to: to);
			Assert.IsNotNull(data);

			//calculate number of products
			var numOfProducts = clients.Count(c => c.user_id >= startBrandUserId && c.user_type != 1) 
			                    * mastProducts.Count(x=>x.category1 != Category1.category1_spares);
			Assert.AreEqual(numOfProducts, data.Count);

			var ukCountryCodes = countriesDal.GetUkCountryCodes();
			var statuses = new[] {"X", "Y"};
			//calculate amount
			if (data.Count > 0)
			{
				var selectedPOrders = pOrders.Where(po =>
					Month21.FromDate(po.po_req_etd.Value) >= @from && Month21.FromDate(po.po_req_etd.Value) <= to).ToList();
				
				var orderids = orders.Where(o =>
					ukCountryCodes.Contains(o.Client.user_country) 
					&& selectedPOrders.Select(x => x.soorderid).Contains(o.orderid)
					&& !statuses.Contains(o.status)).Select(o=>(int?) o.orderid).ToList();
				var amount = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                    && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.unitprice * l.orderqty);
				var qty = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                    && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.orderqty);
				Assert.AreEqual(amount, data[0].Amount);
				Assert.AreEqual(qty, data[0].numOfUnits);
			}

			//fromDate only
			data = analyticsDal.GetProductSales(fromDate: fromDate);
			Assert.AreEqual(numOfProducts, data.Count);
			if (data.Count > 0)
			{
				var selectedPOrders = pOrders.Where(po =>
					po.po_req_etd >= fromDate).ToList();
				
				var orderids = orders.Where(o =>
					ukCountryCodes.Contains(o.Client.user_country) 
					&& selectedPOrders.Select(x => x.soorderid).Contains(o.orderid)
					&& !statuses.Contains(o.status)).Select(o=>(int?) o.orderid).ToList();
				var amount = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                                                           && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.unitprice * l.orderqty);
				var qty = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                                                        && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.orderqty);
				Assert.AreEqual(amount, data[0].Amount);
				Assert.AreEqual(qty, data[0].numOfUnits);
			}

			//toDate only
			data = analyticsDal.GetProductSales(toDate: toDate);
			Assert.AreEqual(numOfProducts, data.Count);
			if (data.Count > 0)
			{
				var selectedPOrders = pOrders.Where(po =>
					po.po_req_etd <= toDate).ToList();
				
				var orderids = orders.Where(o =>
					ukCountryCodes.Contains(o.Client.user_country) 
					&& selectedPOrders.Select(x => x.soorderid).Contains(o.orderid)
					&& !statuses.Contains(o.status)).Select(o=>(int?) o.orderid).ToList();
				var amount = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                                                           && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.unitprice * l.orderqty);
				var qty = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                                                        && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.orderqty);
				Assert.AreEqual(amount, data[0].Amount);
				Assert.AreEqual(qty, data[0].numOfUnits);
			}
			
			//prodIds, orderdate
			var prodIds = products.Where(p => p.MastProduct.category1 != Category1.category1_spares).Take(2).Select(p=>p.cprod_id).ToList();
			data = analyticsDal.GetProductSales(toDate: toDate, prodIds:prodIds,useOrderDate: true);
			Assert.AreEqual(prodIds.Count, data.Count);
			if (data.Count > 0)
			{
				var orderids = orders.Where(o =>
					ukCountryCodes.Contains(o.Client.user_country) 
					&& o.orderdate <= toDate
					&& !statuses.Contains(o.status)).Select(o=>(int?) o.orderid).ToList();
				var amount = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                                                           && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.unitprice * l.orderqty);
				var qty = lines.Where(l => orderids.Contains(l.orderid) && l.cprod_id == data[0].cprod_id 
				                                                        && l.Cust_Product.MastProduct.category1 != Category1.category1_spares).Sum(l => l.orderqty);
				Assert.AreEqual(amount, data[0].Amount);
				Assert.AreEqual(qty, data[0].numOfUnits);
			}

		}

		[TestCleanup]
		public override void Cleanup()
		{
			orderLinesTests?.Cleanup();
			orderHeaderTests?.Cleanup();
			pOrderLinesTests?.Cleanup();
			porderHeaderTest?.Cleanup();
			custProductsTests.Cleanup();
			mastProductTests?.Cleanup();
			brandsDalTest?.Cleanup();
			companyTests?.Cleanup();

		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		
	}
}
