using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("Inspections")]
	public class InspectionsTests : DatabaseTestBase
	{
		protected OrderHeaderTests orderHeaderTests;
		protected CompanyTests companyTests;
		protected OrderLinesTests orderLinesTests;
		protected UserTests userTests; 
		protected CustProductsTests custProductsTests;


		public InspectionsTests()
		{

		}
		
		public InspectionsTests(IDbConnection conn) : base(conn)
		{
			

		}

		[TestInitialize]
		public virtual void Init()
		{
			orderHeaderTests = new OrderHeaderTests(conn);
			orderLinesTests = new OrderLinesTests(conn);
			companyTests = new CompanyTests(conn);
			userTests = new UserTests(conn);
			custProductsTests = new CustProductsTests(conn);
			Cleanup();			
		}

		[TestMethod]
		public void GetIdFromIdString()
		{
			var inspections = new List<Inspections> { new Inspections { 
				insp_unique = 1,
				insp_start = DateTime.Today
				},
				new Inspections { 
					insp_unique = 2,
					new_insp_id = 3}
				};
			GenerateTestData(inspections);
			
			var inspectionDal = new InspectionsDAL(conn, null, null, null, null, null, null, null);
			var id = inspectionDal.GetIdFromIdString(inspections[0].insp_unique.ToString());
			Assert.IsTrue(id > 0, "v1");

			id = inspectionDal.GetIdFromIdString(inspections[1].new_insp_id.ToString(), true);
			Assert.IsNotNull(id > 0,"v2");
		}
		
		[TestMethod]
		public void GetOrderInspections()
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
									 Areas = new List<Inspection_v2_area>
									 {
										 new Inspection_v2_area
										 {
											 name = "area1"
										 }
									 }
								 }
							 }
						 }
					}
				}
			};

			GenerateInspV2TestData(inspections);

			var inspectionV2Dal = new InspectionsV2DAL(conn,null);
			var data = inspectionV2Dal.GetOrderInspections(1);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);
			Assert.AreEqual(inspections[0].id, data[0].id);

			Assert.AreEqual(inspections[0].Lines.Count, data[0].Lines.Count);
			Assert.IsNull(data[0].Lines[0].Loadings?.Count);

			data = inspectionV2Dal.GetOrderInspections(1, true);
			Assert.AreEqual(1, data[0].Lines[0].Loadings?.Count);
			Assert.AreEqual(1, data[0].Lines[0].Loadings[0].Areas?.Count);

		}

		[TestMethod]
		public void GetById()
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

			companyTests = new CompanyTests(conn);
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

			var user = new User
			{
				username = "test",
				company_id = 2,
				admin_type = 1
			};
			userTests.GenerateRecord(user, conn: conn);

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
									Areas = new List<Inspection_v2_area>
									{
										new Inspection_v2_area
										{
											name = "area1"
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
					},
					Containers = new List<Inspection_v2_container>
					{
						new Inspection_v2_container
						{
							container_no = "1"
						}
					},
					Controllers = new List<Inspection_v2_controller>
					{
						new Inspection_v2_controller
						{
							duration = 2,
							controller_id = user.userid,
							startdate = DateTime.Today
						}
					}
				}
			};
			GenerateInspV2TestData(inspections);

			var inspectionV2Dal = new InspectionsV2DAL(conn,null);
			var data = inspectionV2Dal.GetById(inspections[0].id, true, true);
			Assert.IsNotNull(data);
			Assert.AreEqual(1,data.Lines.Count);
			Assert.AreEqual(1,data.Lines[0].Loadings?.Count);
			Assert.AreEqual(1, data.Lines[0].Loadings[0].Areas?.Count);
			Assert.AreEqual(1,data.Lines[0].Images.Count);
			Assert.AreEqual(1,data.Containers.Count);
			Assert.AreEqual(1,data.Controllers.Count);

		}

		[TestMethod]
		public void GetByCriteria()
		{
			var users = new List<User>
			{
				new User
				{
					username = "qc1",
					userwelcome = "qc1"
				}
			};
			userTests.GenerateTestData(users);

			var companies = new List<Company>
			{
				new Company { user_type = 1, user_id = 1, factory_code = "AJB"},
				new Company { user_type = 2, user_id = 2, customer_code = "BW"},
				new Company { user_type = 1, user_id = 3, factory_code = "AJT"}
			};
			companyTests.GenerateTestData(companies);

			var products = new List<Cust_products>
			{
				new Cust_products { cprod_code1 = "prod1" },
				new Cust_products { cprod_code1 = "prod2" }
			};
			custProductsTests.GenerateTestData(products);
										
			var orders = new List<Order_header>
			{
				new Order_header
				{
					orderid = 1,
					custpo = "order1"					
				},
				new Order_header
				{
					orderid = 2,
					custpo = "order2"
				}
			};
			orderHeaderTests.GenerateTestData(orders);
			var lines = new List<Order_lines>
			{
				new Order_lines { orderid = 1, cprod_id = products[0].cprod_id},
				new Order_lines {orderid = 2, cprod_id = products[1].cprod_id}
			};
			orderLinesTests.GenerateTestData(lines);

			var inspv2date = DateTime.Today; 
			var insp_v2 = new Inspection_v2
			{
				insp_status = null,
				startdate = inspv2date,
				factory_id = 3,
				client_id = 2,
				Lines = new List<Inspection_v2_line>
				{
					new Inspection_v2_line
					{
						orderlines_id = lines[1].linenum					
					}
				},
				Controllers = new List<Inspection_v2_controller>
				{
					new Inspection_v2_controller
					{
						controller_id = users[0].userid,
						startdate = inspv2date,
						duration = 1
					}
				}
			};
			GenerateInspV2TestData(new List<Inspection_v2> { insp_v2});

			var inspDate = DateTime.Today.AddDays(5);
			var inspections = new List<Inspections>
			{
				new Inspections
				{
					insp_unique = 1,
					factory_code = "AJB",
					customer_code = "BW",
					acceptance_fc = 0,
					insp_status = 0,
					insp_start = inspDate,
					custpo = "custpo1",
					insp_type = "FI",
					LinesTested = new List<Inspection_lines_tested>
					{
						new Inspection_lines_tested
						{
							order_linenum = lines[0].linenum
						}
					},
					Controllers = new List<Inspection_controller>
					{
						new Inspection_controller
						{
							controller_id = users[0].userid,
							startdate = inspDate,
							duration = 1
						}
					}
				},
				new Inspections
				{
					insp_unique = 2,
					custpo = "custpo2",
					acceptance_fc = 0,
					insp_status = 0,
					insp_type = "LO",
					insp_start = insp_v2.startdate,
					new_insp_id = insp_v2.id
				}
			};
			GenerateTestData(inspections);

			var companyDal = new CompanyDAL(conn, null);
			var inspectionsDal = new InspectionsDAL(conn, null, companyDal, null, null, null, null, null);

			var data = inspectionsDal.GetByCriteria(dateFrom: DateTime.Today.AddDays(2));
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			data = inspectionsDal.GetByCriteria(dateTo: DateTime.Today.AddDays(2));
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			data = inspectionsDal.GetByCriteria(factoryId: 1);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			data = inspectionsDal.GetByCriteria(clientId: 2);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			data = inspectionsDal.GetByCriteria(custpo: "po");
			Assert.IsNotNull(data);
			Assert.AreEqual(2, data.Count);

			data = inspectionsDal.GetByCriteria(productCode: "prod1");
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			data = inspectionsDal.GetByCriteria(productCode: "prod");
			Assert.IsNotNull(data);
			Assert.AreEqual(2, data.Count);

			data = inspectionsDal.GetByCriteria(userId: users[0].userid);
			Assert.IsNotNull(data);
			Assert.AreEqual(2, data.Count);

			//Statuses
			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.Todo);
			Assert.IsNotNull(data);
			Assert.AreEqual(2, data.Count);

			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.AwaitingReview);
			Assert.IsNotNull(data);
			Assert.AreEqual(0, data.Count);

			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.Rejected);
			Assert.IsNotNull(data);
			Assert.AreEqual(0, data.Count);

			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.Accepted);
			Assert.IsNotNull(data);
			Assert.AreEqual(0, data.Count);

			inspections[0].insp_status = 1;;
			UpdateRecord(inspections[0]);
			insp_v2.insp_status = InspectionV2Status.ReportSubmitted;
			UpdateInspectionV2Record(insp_v2);
			
			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.AwaitingReview);
			Assert.IsNotNull(data);
			Assert.AreEqual(2, data.Count);

			inspections[0].acceptance_fc = 2;
			UpdateRecord(inspections[0]);
			
			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.Rejected);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

			inspections[0].acceptance_fc = 1;
			UpdateRecord(inspections[0]);
			
			data = inspectionsDal.GetByCriteria(statusId: (int) InspectionStatus.Accepted);
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);

		}

		[TestMethod]
		public void GetRelatedLoadingInspection()
		{
			var orders = new List<Order_header>
			{
				new Order_header
				{
					orderid = 1,
					custpo = "order1"					
				},
				new Order_header
				{
					orderid = 2,
					custpo = "order2"
				}
			};
			orderHeaderTests.GenerateTestData(orders);
			var lines = new List<Order_lines>
			{
				new Order_lines { orderid = 1 },
				new Order_lines {orderid = 2}
			};
			orderLinesTests.GenerateTestData(lines);

			var insp_v2 = new Inspection_v2
			{
				insp_status = null,								
				Lines = new List<Inspection_v2_line>
				{
					new Inspection_v2_line
					{
						orderlines_id = lines[0].linenum					
					}
				}
				
			};
			GenerateInspV2TestData(new List<Inspection_v2> { insp_v2});
						
			var inspections = new List<Inspections>
			{
				new Inspections
				{
					insp_unique = 1,		
					insp_type = "FI",
					LinesTested = new List<Inspection_lines_tested>
					{
						new Inspection_lines_tested
						{
							order_linenum = lines[0].linenum
						}
					}
				},
				new Inspections
				{
					insp_unique = 2,	
					insp_type = "LO",
					new_insp_id = insp_v2.id
				}
			};
			GenerateTestData(inspections);

			var companyDal = new CompanyDAL(conn, null);
			var inspectionsDal = new InspectionsDAL(conn, null, companyDal, null, null, null, null, null);
			var data = inspectionsDal.GetRelatedLoadingInspections(new[] { inspections[0].insp_unique});
			Assert.IsNotNull(data);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].Inspection_V2);
			Assert.IsNotNull(data[0].Inspection_V2.Lines);
			Assert.AreEqual(1, data[0].Inspection_V2.Lines.Count);
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspections`
					(`insp_unique`,`insp_id`,`insp_type`,`insp_start`,`insp_end`,`insp_version`,`insp_days`,`insp_porderid`,
					`insp_qc1`,`insp_qc2`,`insp_qc3`,`insp_qc6`,`insp_fc`,`customer_code`,`custpo`,`batch_no`,`factory_code`,
					`insp_comments`,`insp_comments_admin`,`insp_status`,`qc_required`,`upload`,`upload_flag`,`lcl`,
					`gp20`,`gp40`,`hc40`,`adjustment`,`LO_id`,`acceptance_qc1`,`acceptance_qc2`,`acceptance_qc3`,
					`acceptance_qc4`,`acceptance_fc`,`acceptance_cc`,`insp_qc5`,`insp_qc4`,`etd`,`eta`,`insp_batch_inspection`,
					`insp_executor`,`fc_status`,`new_insp_id`,`acceptance_qa`,`id`)
					VALUES
					(@insp_unique,@insp_id,@insp_type,@insp_start,@insp_end,@insp_version,@insp_days,@insp_porderid,
					@insp_qc1,@insp_qc2,@insp_qc3,@insp_qc6,@insp_fc,@customer_code,@custpo,@batch_no,@factory_code,
					@insp_comments,@insp_comments_admin,@insp_status,@qc_required,@upload,@upload_flag,
					@lcl,@gp20,@gp40,@hc40,@adjustment,@LO_id,@acceptance_qc1,@acceptance_qc2,@acceptance_qc3,
					@acceptance_qc4,@acceptance_fc,@acceptance_cc,@insp_qc5,@insp_qc4,@etd,@eta,@insp_batch_inspection,
					@insp_executor,@fc_status,@new_insp_id,@acceptance_qa,@id);";
		}

		protected string GetCreateLinesTestedSql()
		{
			return @"INSERT INTO `inspection_lines_tested`
					(`insp_line_unique`,`insp_id`,`insp_factory_ref`,
					`insp_client_ref`,`insp_client_desc`,
					`insp_qty`,`insp_override_qty`,`insp_custpo`,
					`order_linenum`,`photo_confirm`,`photo_confirma`,
					`photo_confirmm`,`photo_confirmd`,`photo_confirmf`,
					`photo_confirmp`,`packaging_rej`,`label_rej`,
					`instructions_rej`,`factory_override`,`mast_id`,
					`cprod_id`)
					VALUES
					(@insp_line_unique,@insp_id,@insp_factory_ref,
					@insp_client_ref,@insp_client_desc,@insp_qty,
					@insp_override_qty,@insp_custpo,@order_linenum,
					@photo_confirm,@photo_confirma,@photo_confirmm,
					@photo_confirmd,@photo_confirmf,@photo_confirmp,
					@packaging_rej,@label_rej,@instructions_rej,
					@factory_override,@mast_id,@cprod_id)";
		}

		protected string GetInspectionV2CreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2`
					(`id`,`startdate`,`type`,`custpo`,`factory_id`,`code`,`client_id`,`duration`,
					`comments`,`qc_required`,`comments_admin`,`insp_status`,`acceptance_fc`,
					`insp_batch_inspection`,`si_subject_id`,`CreatedBy`,`dateCreated`,`drawingFile`)
					VALUES
					(@id,@startdate,@type,@custpo,@factory_id,@code,@client_id,@duration,@comments,
					@qc_required,@comments_admin,@insp_status,@acceptance_fc,@insp_batch_inspection,
					@si_subject_id,@CreatedBy,@dateCreated,@drawingFile);";
		}

		protected string GetInspectionV2UpdateSql() 
		{ 
			return @"UPDATE `asaq`.`inspection_v2`
					SET
					`startdate` = @startdate,`type` = @type,`custpo` = @custpo,
					`factory_id` = @factory_id,	`code` = @code,
					`client_id` = @client_id,`duration` = @duration,
					`comments` = @comments,	`qc_required` = @qc_required,
					`comments_admin` = @comments_admin,	`insp_status` = @insp_status,
					`acceptance_fc` = @acceptance_fc,`insp_batch_inspection` = @insp_batch_inspection,
					`si_subject_id` = @si_subject_id,`CreatedBy` = @CreatedBy,
					`dateCreated` = @dateCreated,`drawingFile` = @drawingFile
					WHERE `id` = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE `asaq`.`inspections`
					SET
					`insp_id` = @insp_id,`insp_type` = @insp_type,
					`insp_start` = @insp_start,	`insp_end` = @insp_end,
					`insp_version` = @insp_version,	`insp_days` = @insp_days,
					`insp_porderid` = @insp_porderid,`insp_qc1` = @insp_qc1,
					`insp_qc2` = @insp_qc2,	`insp_qc3` = @insp_qc3,	`insp_qc6` = @insp_qc6,
					`insp_fc` = @insp_fc,	`customer_code` = @customer_code,
					`custpo` = @custpo,	`batch_no` = @batch_no,
					`factory_code` = @factory_code,	`insp_comments` = @insp_comments,
					`insp_comments_admin` = @insp_comments_admin,`insp_status` = @insp_status,
					`qc_required` = @qc_required,`upload` = @upload,`upload_flag` = @upload_flag,
					`lcl` = @lcl,`gp20` = @gp20,`gp40` = @gp40,	`hc40` = @hc40,
					`adjustment` = @adjustment,	`LO_id` = @LO_id,`acceptance_qc1` = @acceptance_qc1,
					`acceptance_qc2` = @acceptance_qc2,	`acceptance_qc3` = @acceptance_qc3,
					`acceptance_qc4` = @acceptance_qc4,	`acceptance_fc` = @acceptance_fc,
					`acceptance_cc` = @acceptance_cc,`insp_qc5` = @insp_qc5,
					`insp_qc4` = @insp_qc4,	`etd` = @etd,`eta` = @eta,
					`insp_batch_inspection` = @insp_batch_inspection,
					`insp_executor` = @insp_executor,`fc_status` = @fc_status,
					`new_insp_id` = @new_insp_id,`acceptance_qa` = @acceptance_qa,
					`id` = @id
					WHERE `insp_unique` = @insp_unique";
		}

		protected string GetInspectionV2LineCreateSql ()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_line`
					(`id`,`insp_id`,`orderlines_id`,`insp_mastproduct_code`,
					`insp_custproduct_code`,`insp_custproduct_name`,`qty`,
					`custpo`,`cprod_id`,`inspected_qty`,`comments`,`factory_code`,`si_requirement`)
					VALUES
					(@id,@insp_id,@orderlines_id,@insp_mastproduct_code,@insp_custproduct_code,
					@insp_custproduct_name,@qty,@custpo,@cprod_id,@inspected_qty,@comments,
					@factory_code,@si_requirement);";
		}

		protected string GetInspectionV2LoadingCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_loading`
					(`id`,`insp_line`,`container_id`,`full_pallets`,
					`loose_load_qty`,`mixed_pallet_qty`,`mixed_pallet_qty2`,
					`mixed_pallet_qty3`,`area_id`,`qty_per_pallet`)
					VALUES
					(@id,@insp_line,@container_id,@full_pallets,
					@loose_load_qty,@mixed_pallet_qty,@mixed_pallet_qty2,
					@mixed_pallet_qty3,@area_id,@qty_per_pallet);";
		}

		protected string GetInspectionV2ImageCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_image`
					(`id`,`insp_line`,`insp_image`,`type_id`,
					`rej_flag`,`order`,`comments`)
					VALUES
					(@id,@insp_line,@insp_image,@type_id,
					@rej_flag,@order,@comments);";
		}

		protected string GetInspectionV2LoadingAreaCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_loading_area`
				(`loading_id`,`area_id`) VALUES (@loading_id,@area_id);";
		}

		protected string GetInspectionV2AreaCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_area`
				(`id`,`name`) VALUES(@id,@name);";
		}

		protected string GetInspectionV2MixedPalletCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_mixedpallet`
					(`id`,`insp_id`,`name`,`area_id`)
					VALUES (@id,@insp_id,@name,@area_id);";
		}

		protected string GetInspectionV2LoadingMixedPalletQtysCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_loading_mixedpallet`
					(`id`,`loading_id`,`pallet_id`,`qty`)
					VALUES	(@id,@loading_id,@pallet_id,@qty);";
		}

		protected string GetInspectionV2ControllerCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_controller`
					(`id`,`inspection_id`,`controller_id`,
					`startdate`,`duration`)
					VALUES
					(@id,@inspection_id,@controller_id,
					@startdate,@duration);";
		}

		protected string GetInspectionV2ContainerCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_v2_container`
					(`id`,`insp_id`,`container_no`,`seal_no`,
					`container_size`,`container_count`,`container_space`,
					`container_comments`)
					VALUES
					(@id,@insp_id,@container_no,
					@seal_no,@container_size,@container_count,
					@container_space,@container_comments)";
		}

		protected string GetInspectionControllerCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_controller`
				(`id`,`inspection_id`,`controller_id`,
				`startdate`,`duration`)
				VALUES
				(@id,@inspection_id,@controller_id,
				@startdate,@duration)";
		}

		protected void UpdateInspectionV2Record(Inspection_v2 insp)
		{
			conn.Execute(GetInspectionV2UpdateSql(), insp);
		}
				

		protected override string IdField => "insp_unique";
		protected override bool IsAutoKey => false;

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();
			conn.Execute("DELETE FROM inspection_v2");
			orderLinesTests.Cleanup();
			custProductsTests.Cleanup();
			orderHeaderTests.Cleanup();
			companyTests.Cleanup();
			userTests.Cleanup();

		}

		public void GenerateTestData(IEnumerable<Inspections> data)
		{
			if(data != null && data.Count() > 0)
			{
				var propInfo = typeof(Inspections).GetProperty(IdField);
				foreach(var d in data)
				{
					GenerateRecord(d, propInfo, conn);
					if (d.LinesTested != null) 
					{
						foreach(var lt in d.LinesTested)
						{
							lt.insp_id = d.insp_unique;
							conn.Execute(GetCreateLinesTestedSql(), lt);
						}
					}
					if(d.Controllers != null)
					{
						foreach(var ic in d.Controllers)
						{
							ic.inspection_id = d.insp_unique;
							conn.Execute(GetInspectionControllerCreateSql(), ic);
						}
					}

				}
			}
			
		}

		public void GenerateInspV2TestData(List<Inspection_v2> data, IDbConnection conn = null)
		{
			var c = conn ?? this.conn;
			var dictAreas = new Dictionary<int, Inspection_v2_area>();
			var dictMixedPallets = new Dictionary<int, Inspection_v2_mixedpallet>();
					
			foreach (var d in data)
			{
				c.Execute(GetInspectionV2CreateSql(), d);
				d.id = GetLastInsertId();
				if(d.Controllers != null)
				{
					foreach(var co in d.Controllers)
					{
						co.inspection_id = d.id;
						c.Execute(GetInspectionV2ControllerCreateSql(), co);
						co.id = GetLastInsertId();
					}
				}

				if (d.Containers != null)
				{
					foreach(var co in d.Containers)
					{
						co.insp_id = d.id;
						c.Execute(GetInspectionV2ContainerCreateSql(), co);
						co.id = GetLastInsertId();
					}
				}
				if (d.Lines != null)
				{
					foreach(var l in d.Lines)
					{
						l.insp_id = d.id;
						c.Execute(GetInspectionV2LineCreateSql(), l);
						l.id = GetLastInsertId();
						if(l.Loadings != null)
						{
							foreach(var lo in l.Loadings)
							{
								lo.insp_line = l.id;
								c.Execute(GetInspectionV2LoadingCreateSql(), lo);
								lo.id = GetLastInsertId();
								if(lo.Areas != null)
								{
									foreach(var a in lo.Areas)
									{
										if(!dictAreas.ContainsKey(a.id))
										{
											c.Execute(GetInspectionV2AreaCreateSql(), a);
											a.id = GetLastInsertId();
											dictAreas[a.id] = a;
										}
										c.Execute(GetInspectionV2LoadingAreaCreateSql(), new {  loading_id = lo.id, area_id = a.id });
									}
								}
								if(lo.QtyMixedPallets != null)
								{
									foreach (var mp in lo.QtyMixedPallets)
									{
										if (!dictMixedPallets.ContainsKey(mp.Pallet.id))
										{
											c.Execute(GetInspectionV2MixedPalletCreateSql(), mp.Pallet);
											mp.Pallet.id = GetLastInsertId();
											mp.pallet_id = mp.Pallet.id;
											dictMixedPallets[mp.id] = mp.Pallet;
										}
										mp.pallet_id = mp.Pallet.id;
										mp.loading_id = lo.id;
										c.Execute(GetInspectionV2LoadingMixedPalletQtysCreateSql(), mp);
										mp.id = GetLastInsertId();
									}
								}
							}								
						}
						if(l.Images != null)
						{
							foreach(var im in l.Images)
							{
								im.insp_line = l.id;
								c.Execute(GetInspectionV2ImageCreateSql(), im);
								im.id = GetLastInsertId();
							}
						}
					}
				}

			}		
			
		}
	}
}
