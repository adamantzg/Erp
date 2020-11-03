using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class AnalyticsCategoryDal : GenericDal<Analytics_categories>, IAnalyticsCategoryDal
	{
		public AnalyticsCategoryDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM Analytics_categories";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
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

		public List<Analytics_categories> GetForBrand(int? brand_user_id = null)
		{
			var sql = $"SELECT * FROM analytics_categories WHERE category_type {(brand_user_id != null ? "= @brand_user_id" : " IS NULL")}";
			return conn.Query<Analytics_categories>(sql, new { brand_user_id }).ToList();
		}
	}
}
