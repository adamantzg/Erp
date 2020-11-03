using asaq2.Model;
using asaq2.Model.Dal.New.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.DAL.EF.New.Test
{
	[TestClass]
	[Table("Inspections")]
	public class InspectionsTests : asaq2.Model.Dal.New.Test.InspectionsTests
	{
		private OrderHeaderTests orderHeaderTests;
		private CompanyTests companyTests;
		private OrderLinesTests orderLinesTests;

		public InspectionsTests()
		{

		}
		
		public InspectionsTests(IDbConnection conn) : base(conn)
		{
			
		}

		[TestInitialize]
		public override void Init()
		{
			base.Init();
		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();
		}

		[TestMethod]
		public void MappingTest()
		{
			orderHeaderTests = new OrderHeaderTests(conn);
			orderHeaderTests.Cleanup();
			var orders = new List<Order_header>
			{
				new Order_header
				{
					orderid = 1,
					custpo = "test"					
				}
			};
			orderHeaderTests.GenerateTestData(orders);
			var lines = new List<Order_lines>
			{
				new Order_lines { orderid = 1},
				new Order_lines {orderid = 1}
			};
			orderLinesTests = new OrderLinesTests(conn);
			orderLinesTests.Cleanup();
			orderLinesTests.GenerateTestData(lines);

			var companyTests = new CompanyTests(conn);
			companyTests.Cleanup();
			companyTests.GenerateTestData(
				new List<Company> { 
					new Company
					{
						user_type = (int) Company_User_Type.Factory,
						user_id = 1
					},
					new Company
					{
						user_type = (int) Company_User_Type.Client,
						user_id = 2
					}
				}
			);

			var areas = new List<Inspection_v2_area>
			{
				new Inspection_v2_area {name = "Area1"}
			};
			var pallets = new List<Inspection_v2_mixedpallet>
			{
				new Inspection_v2_mixedpallet
				{
					name = "Pallet1"
				}
			};

			var inspections = new List<Inspection_v2>
			{
				new Inspection_v2 { 
					startdate = DateTime.Today, 
					type = Inspection_v2_type.Loading,
					client_id = 2,
					factory_id = 1,
					Lines = new List<Inspection_v2_line>
					{
						 new Inspection_v2_line
						 {
							 orderlines_id = lines[0].linenum,
							 Loadings = new List<Inspection_v2_loading>
							 {
								 new Inspection_v2_loading
								 {
									 full_pallets = 1,									 
									 Areas = areas,
									 QtyMixedPallets = new List<Inspection_v2_loading_mixedpallet>
									 {
										 new Inspection_v2_loading_mixedpallet
										 {
											 Pallet = pallets[0],
											 qty = 1											 
										 }
									 }
								 }
							 },
							 Images = new List<Inspection_v2_image>
							 {
								 new Inspection_v2_image
								 {
									 insp_image = "test"
								 }
							 }
						 }
					}
				}
			};

			GenerateInspV2TestData(inspections);

			var unitOfWork = new UnitOfWork(new Model("name=connString"));
			var id = inspections[0].id;
			var insp = unitOfWork.InspectionV2Repository.Get(i => i.id == id,
				includeProperties: @"Lines.Loadings, Lines.OrderLine.Header,
						Factory, Client, InspectionType").FirstOrDefault();

			Assert.IsNotNull(insp);
			Assert.AreEqual(1, insp.Lines.Count);

			var line = insp.Lines[0];
			unitOfWork.InspectionV2LineRepository.LoadCollection(line,"Images");
			Assert.IsNotNull(line.Images);
			Assert.AreEqual(1, line.Images.Count);

			var lo = insp.Lines[0].Loadings[0];

			unitOfWork.InspectionV2LoadingRepository.LoadCollection(lo, "Areas");
			Assert.IsNotNull(lo.Areas);
			Assert.AreEqual(1, lo.Areas.Count);

			unitOfWork.InspectionV2LoadingRepository.LoadCollection(lo, "QtyMixedPallets");
			Assert.IsNotNull(lo.QtyMixedPallets);
			Assert.AreEqual(1, lo.QtyMixedPallets.Count);
		}
		
	}
}
