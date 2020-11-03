using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class BrandSubCategoriesDal : GenericDal<BrandSubCategory>, IBrandSubCategoriesDal
	{
		public BrandSubCategoriesDal(IDbConnection conn) : base(conn)
		{
		}

		private string GetSubCatsSelectClause(bool forTranslation = false, bool productsOptionCheck = false)
        {
            string sql;
            if (!forTranslation)
            {
                sql = "SELECT brand_categories_sub.* {0} FROM brand_categories_sub ";
            }
            else
                sql = @"SELECT brand_categories_sub.*, 
                            brand_categories_sub_translate.brand_sub_desc AS brand_sub_desc_t,
                            brand_categories_sub_translate.sub_description AS sub_description_t,
                            brand_categories_sub_translate.sub_details AS sub_details_t,
                            brand_categories_sub_translate.pricing_note AS pricing_note_t,
                            brand_categories_sub_translate.group AS group_t {0}
                        FROM
                        brand_categories_sub
                            LEFT JOIN brand_categories_sub_translate ON (brand_categories_sub_translate.brand_sub_id = brand_categories_sub.brand_sub_id 
                                    AND brand_categories_sub_translate.lang = @lang)";

            if (productsOptionCheck)
                sql = string.Format(sql, ",(SELECT COALESCE(COUNT(*),0) FROM web_products WHERE COALESCE(option_name,'') <> '' AND web_sub_category = brand_categories_sub.brand_sub_id) AS  optionsCount ");
            else
                sql = string.Format(sql, string.Empty);
            return sql;
        }

        public List<BrandSubCategory> GetBrandSubCategories(int catId, string language_id = null)
        {
            string sql = GetSubCatsSelectClause(language_id != null);
            sql += " WHERE brand_categories_sub.brand_cat_id = @catId ORDER BY 'group',seq";
            return conn.Query<BrandSubCategory>(sql, new { catId, lang = language_id }).ToList();
        }

        public List<BrandSubCategory> GetAllBrandSubCategories(int brandid, string language_id = null)
        {
            
            var sql = GetSubCatsSelectClause(language_id != null);
            sql +=" INNER JOIN brand_categories ON brand_categories_sub.brand_cat_id = brand_categories.brand_cat_id";
            sql += " WHERE brand = @brandid ORDER BY 'group',seq";
            return conn.Query<BrandSubCategory>(sql, new { brandid, lang = language_id }).ToList();
        }

        public BrandSubCategory GetSubCategory(int id, string language_id = null)
        {
            string sql = GetSubCatsSelectClause(language_id != null,true);
            sql += " WHERE brand_categories_sub.brand_sub_id = @id";
            return conn.QueryFirstOrDefault<BrandSubCategory>(sql, new { id, lang = language_id});
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
