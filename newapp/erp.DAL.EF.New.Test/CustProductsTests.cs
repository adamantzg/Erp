using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using asaq2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace asaq2.DAL.EF.New.Test
{
	[TestClass]
	[Table("cust_products")]
	public class CustProductsTests : asaq2.Model.Dal.New.Test.CustProductsTests
	{
		UnitOfWork unitOfWork = new UnitOfWork(new Model("name=connString"));

		public CustProductsTests()
		{

		}
		
		public CustProductsTests(IDbConnection conn) : base(conn)
		{
			
		}

		[TestMethod]
		public void GetAutoAddedProducts()
		{
			var data = new List<Cust_products>
			{
				new Cust_products { cprod_code1 = "trigger"},
				new Cust_products
				{
					cprod_code1 = "added1", 
					MastProduct = new Mast_products
					{
						factory_ref = "mast1",
						factory_id = 1,
						Factory = new Company
						{
							user_id = 1,
							user_type = (int) Company_User_Type.Factory,
							user_name = "Fact1"
						}
					}
				},
				new Cust_products
				{
					cprod_code1 = "added2",
					MastProduct = new Mast_products
					{
						factory_ref = "mast2", 
						factory_id = 2,
						Factory = new Company
						{
							user_id = 2,
							user_type = (int) Company_User_Type.Factory,
							user_name = "Fact2"
						}
					}
				}
			};
			GenerateTestData(data, conn);
			var autoAddData = new List<auto_add_products>
			{
				new auto_add_products
				{
					trigger_cprod_id = data[0].cprod_id,
					added_cprod_id = data[1].cprod_id					
				}, 
				new auto_add_products
				{
					trigger_cprod_id = data[0].cprod_id,
					added_cprod_id = data[2].cprod_id					
				}
			};
			GenerateAutoAddedProductsData(autoAddData, conn);

			//var unitOfWork = new UnitOfWork(new Model("name=connString"));
			var returned = unitOfWork.CustProductRepository.GetAutoAddedProducts(new List<int?> { data[0].cprod_id});

			Assert.IsNotNull(returned);
			Assert.AreEqual(2, returned.Count);
			Assert.IsNotNull(returned[0].AddedProduct?.MastProduct?.Factory);
		}

		[TestMethod] 
		public void ExtraData()
		{
			//Insert
			var cp = new Cust_products
			{
				cprod_code1 = "test",
				cprod_name = "test"
			};
			cp.ExtraData = new cust_products_extradata();
			unitOfWork.CustProductRepository.Insert(cp);
			unitOfWork.Save();
			var data = this.conn.QueryFirstOrDefault<cust_products_extradata>("SELECT * FROM cust_products_extradata WHERE cprod_id = @id", 
				new {id = cp.cprod_id});
			Assert.IsNotNull(data);
			Assert.AreEqual(cp.cprod_id, data.cprod_id);

			//Get
			var list = new List<Cust_products> 
			{ 
				new Cust_products {cprod_code1 = "test1", cprod_name = "test2"}
			};
			GenerateTestData(list);

			conn.Execute("INSERT INTO cust_products_extradata(cprod_id) VALUES(@id)", new { id = list[0].cprod_id});

			var id = list[0].cprod_id;
			var result = unitOfWork.CustProductRepository.Get(p=>p.cprod_id == id, includeProperties: "ExtraData").FirstOrDefault();
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.ExtraData);


		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();			
		}
	}
}
