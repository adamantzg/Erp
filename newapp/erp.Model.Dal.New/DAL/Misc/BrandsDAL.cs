
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public class BrandsDAL : GenericDal<Brand>, IBrandsDAL
	{
		
		public BrandsDAL(IDbConnection conn) : base(conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public List<Brand> GetAll(bool eb_brands_only = true)
		{
			return conn.Query<Brand>("SELECT * FROM brands WHERE (@eb_brand IS NULL OR eb_brand = @eb_brand)",
				new {eb_brand = eb_brands_only ? (object) 1 : null}).ToList();
		}

        public List<Brand> GetByIds(IList<int> ids )
        {
	        return conn.Query<Brand>("SELECT * FROM brands WHERE brand_id IN @ids", new {ids}).ToList();
        }

        public List<Brand> GetByCompanyIds(IList<int> ids)
        {
	        return conn.Query<Brand>("SELECT * FROM brands WHERE user_id IN @ids", new {ids}).ToList();
        }			
		
		public Brand GetByCode(string brand_code)
		{
			return conn.QueryFirstOrDefault<Brand>("SELECT * FROM brands WHERE code = @brand_code", new {brand_code});
		}

		public Brand GetByCompanyId(int company_id)
		{
			return conn.QueryFirstOrDefault<Brand>("SELECT * FROM brands WHERE user_id = @company_id",
				new {company_id});
		}

		public List<Brand> GetBrandsByCompanyId(int company_id)
		{
			var result = new List<Brand>();
			var b = conn.QueryFirstOrDefault<Brand>("SELECT * FROM brands WHERE user_id = @company_id",
				new {company_id});
			result.Add(b);
			if (b.brand_group != null)
			{
				result.AddRange(conn.Query<Brand>("SELECT * FROM brands WHERE COALESCE(user_id,0) <> @company_id AND brand_group = @group", new {group = b.brand_group}));
			}
			return result;
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM brands";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM brands WHERE brand_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO `asaq`.`brands`
				(`brand_id`,`brandname`,`user_id`,`code`,`dealerstatus_view`,
				`image`,`eb_brand`,`category_flag`,`dealerstatus_manual`,
				`show_as_eb_default`,`show_as_eb_products`,`brand_group`,
				`dealersearch_view`)
				VALUES
				(@brand_id,@brandname,@user_id,@code,@dealerstatus_view,
				@image,@eb_brand,@category_flag,@dealerstatus_manual,
				@show_as_eb_default,@show_as_eb_products,@brand_group,
				@dealersearch_view)
				";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE `asaq`.`brands`
					SET
					`brand_id` = @brand_id,
					`brandname` = @brandname,
					`user_id` = @user_id,
					`code` = @code,
					`dealerstatus_view` = @dealerstatus_view,
					`image` = @image,
					`eb_brand` = @eb_brand,
					`category_flag` = @category_flag,
					`dealerstatus_manual` = @dealerstatus_manual,
					`show_as_eb_default` = @show_as_eb_default,
					`show_as_eb_products` = @show_as_eb_products,
					`brand_group` = @brand_group,
					`dealersearch_view` = @dealersearch_view
					WHERE `brand_id` = @brand_id;";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM brands WHERE brand_id = @id";
		}

		protected override string IdField => "brand_id";
		protected override bool IsAutoKey => true;
	}
}
			
			