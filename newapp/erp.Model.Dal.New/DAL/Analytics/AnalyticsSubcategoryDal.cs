using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class AnalyticsSubcategoryDal : GenericDal<Analytics_subcategory>, IAnalyticsSubcategoryDal
	{
		public AnalyticsSubcategoryDal(IDbConnection conn) : base(conn)
		{
		}

		public override List<Analytics_subcategory> GetAll()
		{
			return conn.Query<Analytics_categories,Analytics_subcategory, Analytics_subcategory>(
				@"SELECT analytics_categories.*,analytics_subcategory.*
                    FROM analytics_subcategory INNER JOIN analytics_categories 
				ON analytics_subcategory.category_id = analytics_categories.category_id",
				(cat, sub) =>
				{
					sub.Category = cat;
					return sub;
				}, splitOn: "subcat_id").ToList();
		}

		public List<Analytics_subcategory> GetForBrand(int? brandId = null, bool nullBrandOnly = false)
		{
			var sql = $@"SELECT analytics_categories.*, analytics_subcategory.*  FROM analytics_subcategory
                        INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                        WHERE {(nullBrandOnly ? "analytics_categories.category_type IS NULL" :
						@"analytics_subcategory.subcat_id IN (SELECT cust_products.analytics_category FROM cust_products
                        INNER JOIN brands ON cust_products.brand_id = brands.brand_id
                        WHERE brands.brand_id = @brandId OR @brandId IS NULL)")}";
			return conn.Query<Analytics_categories, Analytics_subcategory, Analytics_subcategory>(
				sql, (c,s) =>
				{
					s.Category = c;
					return s;
				}, new { brandId }, splitOn: "subcat_id").ToList();
				
		}

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM analytics_subcategory WHERE subcat_id = @id";
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}
	}
}
