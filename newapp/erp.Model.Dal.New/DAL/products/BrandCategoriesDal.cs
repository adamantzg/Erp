using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class BrandCategoriesDal : GenericDal<BrandCategory>, IBrandCategoriesDal
	{
		private readonly IBrandSubCategoriesDal brandSubCategoriesDal;

		public BrandCategoriesDal(IDbConnection conn, IBrandSubCategoriesDal brandSubCategoriesDal) : base(conn)
		{
			this.brandSubCategoriesDal = brandSubCategoriesDal;
		}

		public List<BrandCategory> GetBrandCategories(int brand, string language_id = null, bool filterByWebSeq = true)
        {
            
            List<string> where = new List<string>();
            string sql = GetCatsSelectClause(language_id != null);
            where.Add(" brand = @brand");
            if (filterByWebSeq)
            {
                where.Add(" web_seq > 0 AND web_seq < 99 ");
            }
            sql += " WHERE " + string.Join(" AND ", where.ToList());
            sql += " ORDER BY web_seq";

			return conn.Query<BrandCategory>(sql, new { brand, lang = language_id}).ToList();           
            
        }

        public List<BrandCategory> GetBrandCategoriesSimple(int brand_id)
        {
			return conn.Query<BrandCategory>(
					@"SELECT brand_categories.*, COALESCE((SELECT COUNT(*) FROM brand_categories_sub 
					WHERE brand_cat_id = brand_categories.brand_cat_id),0) AS childcount 
					FROM brand_categories INNER JOIN brands ON brand_categories.brand = brands.user_id 
					WHERE web_seq > 0 AND brand_id = @brand_id ORDER BY web_seq", new { brand_id }).ToList();            
        }



        public List<BrandCategory> GetBrandCategories(IList<int> brands = null)
        {
			return conn.Query<BrandCategory>(
				@"SELECT * FROM brand_categories INNER JOIN brands ON brand_categories.brand = brands.user_id 
				  WHERE brands.brand_id IN @brands", new { brands }).ToList();            
        }       

        public BrandCategory GetCategory(int id, bool loadSubs = false, string language_id = null)
        {
            
            var sql = GetCatsSelectClause(language_id != null);
            sql += " WHERE brand_categories.brand_cat_id = @id";
            var result = conn.QueryFirstOrDefault<BrandCategory>(sql, new { id });                
            if(loadSubs)
                result.Subcategories = brandSubCategoriesDal.GetBrandSubCategories(result.brand_cat_id);           
            return result;
        }

        private string GetCatsSelectClause(bool forTranslation = false)
        {
            if (!forTranslation)
                return "SELECT * FROM brand_categories ";
            else
                return @"SELECT brand_categories.*, 
                            brand_categories_translate.web_description AS web_description_t,
                            brand_categories_translate.brand_cat_desc AS brand_cat_desc_t,
                            brand_categories_translate.why_so_good AS why_so_good_t,
                            brand_categories_translate.why_so_good_title AS why_so_good_title_t
                        FROM
                        brand_categories
                            LEFT JOIN brand_categories_translate ON (brand_categories_translate.brand_cat_id = brand_categories.brand_cat_id 
                                    AND brand_categories_translate.lang = @lang)";

        }

        

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}
	}
}

