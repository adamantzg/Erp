using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("web_category")]
	public class WebCategoryTests : DatabaseTestBase
	{
		protected IWebCategoryDal webCategoryDal;
		private BrandsDalTest brandsDalTest;
		private Brand brand;

		[TestInitialize]
		public void Initialize()
		{
			brand = new Brand {brandname = "test"};
			brandsDalTest = new BrandsDalTest(conn);
			brandsDalTest.GenerateTestData(new []{brand});
			
			conn.Execute("INSERT INTO web_site (id,brand_id, name, code) VALUES(@id, @brand_id,@name, @code)", 
				new { id = 30, brand.brand_id, name = "test", code = "test" });

			webCategoryDal = new WebCategoryDal(conn);
			var parent = new Web_category
			{
				name = "cat1",
				brand_id = brand.brand_id
			};
			GenerateTestData(new[] {parent}, conn);
			var categories = new List<Web_category>
			{
				new Web_category
				{
					name = "cat2",
					parent_id = parent.category_id
				},
				new Web_category
				{
					name = "cat3",
					parent_id = parent.category_id
				}
			};
			GenerateTestData(categories, conn);
			
			
		}

		[TestCleanup]
		public override void Cleanup()
		{
			conn.Execute("DELETE FROM web_product_category");
			conn.Execute("DELETE FROM web_product_new");				
			base.Cleanup();
			conn.Execute("DELETE FROM web_site WHERE id = 30");
			conn.Execute("DELETE FROM brands");
		}

		[TestMethod, TestCategory("Web category")]
		public void GetForBrand()
		{
			var results = webCategoryDal.GetForBrand(brand.brand_id);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(2, results[0].ChildCount);

			var web_unique = 1;
			conn.Execute("INSERT INTO web_product_new(web_unique, web_name) VALUES(@web_unique, @web_name)", new {web_unique, web_name = "test"});
			conn.Execute("INSERT INTO web_product_category(category_id, web_unique) VALUES(@category_id, @web_unique)",
				new
				{
					results[0].category_id,
					web_unique
				});

			results = webCategoryDal.GetForBrand(brand.brand_id, searchForProducts: true);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(1, results[0].ProductCount);
		}

		[TestMethod, TestCategory("Web category")]
		public void GetForSite()
		{
			var results = webCategoryDal.GetForSite(30);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(2, results[0].ChildCount);
		}

		protected override string GetCreateSql()
		{
			return "INSERT INTO web_category(name, parent_id, brand_id ) VALUES(@name, @parent_id, @brand_id)";
		}

		protected override string IdField => "category_id";
	}
}
