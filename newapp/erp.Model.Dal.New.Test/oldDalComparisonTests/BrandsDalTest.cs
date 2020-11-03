using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass, TestCategory("Brands")]
	[Table("brands")]
	public class BrandsDalTest : DatabaseTestBase
	{
		private BrandsDAL brandsDal;
		private List<Brand> brands;
		private CompanyTests companyTests;

		public BrandsDalTest()
		{

		}

		public BrandsDalTest(IDbConnection conn) : base(conn)
		{

		}

		[TestInitialize]
		public void Init()
		{
			companyTests = new CompanyTests(conn);
			Cleanup();

			
			var company = new Company {user_id = 1, user_name = "test_user"};
			companyTests.GenerateTestData(new []{company});
			
			brands = new List<Brand>
			{
				new Brand {brandname = "brand1", eb_brand = 1, user_id = 1},
				new Brand {brandname = "brand2", eb_brand = 1, user_id = 1},
				new Brand {brandname = "brand3", eb_brand = null, user_id = 1}
			};
			GenerateTestData(brands, conn);

			brandsDal = new BrandsDAL(conn);
		}
		

		[TestMethod]
		public void GetAll()
		{
			var brands = brandsDal.GetAll();
			var brandsOld = DAL.BrandsDAL.GetAll();

			Assert.IsNotNull(brands);
			Assert.IsNotNull(brandsOld);
			Assert.AreEqual(2, brands.Count);
			Assert.AreEqual(brands.Count, brandsOld.Count);

			brands = brandsDal.GetAll(false);
			brandsOld = DAL.BrandsDAL.GetAll(false);
			Assert.AreEqual(3, brands.Count);
			Assert.AreEqual(brands.Count, brandsOld.Count);
		}

		[TestMethod]
		public void GetById()
		{
			var brand = brandsDal.GetById(brands[0].brand_id);
			var brandOld = DAL.BrandsDAL.GetById(brands[0].brand_id);
			Assert.IsNotNull(brand);
			Assert.IsNotNull(brandOld);
			Assert.AreEqual(brand.brandname, brandOld.brandname);
		}

		[TestMethod]
		public void Create()
		{
			var brand = new Brand
			{
				brandname = "brand4",
				eb_brand = 1,
				user_id = 1,
				code = "code",
				category_flag = null
			};
			brandsDal.Create(brand);
			Assert.IsTrue(brand.brand_id > 0);
			var dbBrand = conn.QueryFirstOrDefault<Brand>("SELECT * FROM brands WHERE brand_id = @brand_id", brand);

			Assert.IsTrue(Utils.CompareObjects(dbBrand, brand));
		}

		protected override string IdField => "brand_id";

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO `asaq`.`brands`
				(`brand_id`,`brandname`,`user_id`,`code`,`dealerstatus_view`,
				`image`,`eb_brand`,`category_flag`,`dealerstatus_manual`,`show_as_eb_default`,
				`show_as_eb_products`,`brand_group`,`dealersearch_view`)
				VALUES
				(@brand_id,@brandname,@user_id,@code,@dealerstatus_view,@image,
				@eb_brand,@category_flag,@dealerstatus_manual,@show_as_eb_default,
				@show_as_eb_products,@brand_group,@dealersearch_view)";
		}

		public override void Cleanup()
		{
			base.Cleanup();
			companyTests?.Cleanup();
		}
	}
}
