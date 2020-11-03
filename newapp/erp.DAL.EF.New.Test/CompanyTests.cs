using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asaq2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using asaq2.Model.Dal.New.Test;

namespace asaq2.DAL.EF.New.Test
{
	[TestClass]
	[Table("users")]
	public class CompanyTests : asaq2.Model.Dal.New.Test.CompanyTests
	{
		private OrderHeaderTests orderHeaderTests;

		public CompanyTests()
		{
		}

		public CompanyTests(IDbConnection conn) : base(conn)
		{
		}

		[TestInitialize]
		public override void Init()
		{
			Cleanup();
			base.Init();
		}

		[TestMethod]
		public void GetClientsWithOrders()
		{
			orderHeaderTests = new OrderHeaderTests(conn);
			var clients = new List<Company>
			{
				new Company
				{
					user_id = 1,
					user_type = (int) Company_User_Type.Client,
					customer_code = "Test1"
				},
				new Company
				{
					user_id = 2,
					user_type = (int) Company_User_Type.Client,
					customer_code = "Test2",
					combined_factory = 5
				},
				new Company
				{
					user_id = 3,
					user_type = (int) Company_User_Type.Client,
					customer_code = "Test3",
					combined_factory = 5
				}
			};
			GenerateTestData(clients);
			var orders = new List<Order_header>
			{
				new Order_header
				{
					orderid = 1,
					userid1 = 1
				},
				new Order_header
				{
					orderid = 2,
					userid1 = 2
				},
				new Order_header
				{
					orderid = 3,
					userid1 = 3
				}
			};
			orderHeaderTests.GenerateTestData(orders);
			var unitOfWork = new UnitOfWork(new Model("name=connString"));
			var data = unitOfWork.CompanyRepository.GetClientsWithOrders();
			Assert.IsNotNull(data);
			Assert.AreEqual(3, data.Count);

			data = unitOfWork.CompanyRepository.GetClientsWithOrders(true);
			Assert.AreEqual(4, data.Count);
			Assert.AreEqual(-1*clients[1].combined_factory, data[3].user_id);
			Assert.AreEqual(string.Join("/", clients.Skip(1).Select(c=>c.customer_code)), data[3].customer_code);
		}

		[TestCleanup]
		public override void Cleanup()
		{
			if(orderHeaderTests != null)
				orderHeaderTests.Cleanup();
			base.Cleanup();
		}

	}
}
