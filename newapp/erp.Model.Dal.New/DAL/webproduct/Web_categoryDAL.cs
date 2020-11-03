using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using Dapper;

namespace erp.Model.Dal.New
{
	public class WebCategoryDal : GenericDal<Web_category>, IWebCategoryDal
    {
	    
	   
	    public WebCategoryDal(IDbConnection conn) : base(conn)
	    {
		    
	    }

        public List<Web_category> GetForBrand(int brand_id, int? language_id = null, bool searchForProducts = false)
        {
            
            var sql = GetSelect(@"SELECT web_category.*, {0} (SELECT COUNT(*) FROM web_category child WHERE child.parent_id = web_category.category_id) AS childCount {2}
                        FROM web_category {1} WHERE brand_id = @brand_id", language_id != null, extraArgs: searchForProducts ? new object[] { ", (SELECT COUNT(*) FROM web_product_category WHERE web_product_category.category_id = web_category.category_id) AS productCount" } : new object[] { "" });
	        return conn.Query<Web_category>(sql, new {brand_id, language_id}).ToList();
            
        }

        public List<Web_category> GetForSite(int site_id, int? language_id = null)
        {
            var result = new List<Web_category>();
            var sql = GetSelect(@"SELECT web_category.*, {0} (SELECT COUNT(*) FROM web_category child WHERE child.parent_id = web_category.category_id) AS childCount
                                                 FROM web_category INNER JOIN brands ON web_category.brand_id = brands.brand_id INNER JOIN web_site ON brands.brand_id = web_site.brand_id {1} 
												WHERE web_site.id = @site_id", language_id != null);
	        return conn.Query<Web_category>(sql, new {site_id, language_id}).ToList();
        }

        public List<Web_category> GetForIds(IList<int> ids)
        {
	        return conn.Query<Web_category>("SELECT web_category.* FROM web_category WHERE category_id IN @ids",
		        new {ids}).ToList();
        }

        public List<Web_category> GetChildren(int parent_id, int? language_id = null, bool searchForProducts = false)
        {
            var result = new List<Web_category>();
            result.AddRange(GetChildrenInternal(parent_id, conn, language_id: language_id, searchForProducts: searchForProducts));
            return result;
        }

        public List<Web_category> GetForProduct(int web_unique, IDbConnection conn = null, bool searchForProducts = false)
        {
            var sql =
                string.Format(@"SELECT web_category.*,(SELECT COUNT(*) FROM web_category child WHERE child.parent_id = web_category.category_id) AS childCount
                                  {0} FROM web_category INNER JOIN web_product_category ON web_category.category_id = web_product_category.category_id
                                  WHERE web_product_category.web_unique = @web_unique",searchForProducts ? ",(SELECT COUNT(*) FROM web_product_category WHERE web_product_category.category_id = web_category.category_id) AS productCount" : "");
	        return (conn ?? this.conn).Query<Web_category>(sql, new {web_unique}).ToList();
        }

        public List<Web_category> BuildTreeForSelected(IList<int> catIds,int brand_id)
        {
            var list = GetForBrand(brand_id);
            
            foreach (var id in catIds)
            {
                var cat = GetById(id, conn);
                var currentCat = cat;
                while (currentCat.parent_id != null)
                {
                    var parent = list.FirstOrDefault(c => c.category_id == currentCat.parent_id);
                    if (parent == null)
                    {
                        parent = GetById(currentCat.parent_id.Value, conn);
                        
                    }
                    if (parent.ChildCount > 0 && (parent.Children == null || parent.Children.Count == 0))
                    {
                        parent.Children = GetChildren(parent.category_id);
                        var child = parent.Children.FirstOrDefault(c => c.category_id == currentCat.category_id);
                        if (child != null)
                        {
                            child.Children = currentCat.Children;
                        }
                        list.AddRange((parent.Children));
                    }

                    currentCat = parent;
                }

            }
            
            return list.Where(c=>c.parent_id == null).ToList();
        }

        public List<Web_category> GetAllChildren(int parent_id)
        {
            var result = new List<Web_category>();
            var children = GetChildrenInternal(parent_id, conn, true);
            if(children.Count > 0)
                result.AddRange(children);
	        return result;
        }

        private List<Web_category> GetChildrenInternal(int parent_id, MySqlConnection conn, bool deep = false,int? language_id = null,bool searchForProducts= false)
        {
            
            var sql = GetSelect(@"SELECT web_category.*,{0} 
                        (SELECT COUNT(*) FROM web_category child WHERE child.parent_id = web_category.category_id ) AS childCount {2} FROM web_category {1} 
                          WHERE parent_id = @parent_id",language_id != null,
                       extraArgs: searchForProducts ? new object[] {", (SELECT COUNT(*) FROM web_product_category WHERE web_product_category.category_id = web_category.category_id) AS productCount"} : new object[]{""});
            var result = conn.Query<Web_category>(sql, new {parent_id, language_id}).ToList();
            if (deep)
            {
                var allChildren = new List<Web_category>();
                foreach (var c in result)
                {
                    var children = GetChildrenInternal(c.category_id, conn, true);
                    if(children.Count > 0)
                        allChildren.AddRange(children);
                }
                result.AddRange(allChildren);
            }
            
            return result;
        }

        public bool CanCategoryBeDeleted(int id)
        {
            var children = GetAllChildren(id);
            var ids = children.Select(c => c.category_id).ToList();
            ids.Add(id);
            
            var sql = "SELECT COUNT(*) FROM web_product_new INNER JOIN web_product_category ON web_product_new.web_unique = web_product_category.web_unique WHERE category_id IN @ids";
            var count = Convert.ToInt32(conn.ExecuteScalar(sql, new {ids}));
            return count == 0;
        }

        public void Rename(int category_id, string name)
        {
            string updatesql = @"UPDATE web_category SET name = @name WHERE category_id = @category_id";
            conn.Execute(updatesql, new {category_id, name});
        }

        public Web_category GetById(int id, IDbConnection conn = null,int? language_id = null)
        {
            var sql = GetSelect("SELECT web_category.*,{0} (SELECT COUNT(*) FROM web_category child WHERE child.parent_id = web_category.category_id) AS childCount FROM web_category {1} WHERE category_id = @id",language_id != null);
	        return (conn ?? this.conn).Query<Web_category>(sql, new {id, language_id}).FirstOrDefault();
        }

        public List<Web_category> GetParents(int cat_id, IDbConnection conn = null)
        {
            var list = new List<Web_category>();
            var cat = GetById(cat_id, conn);
            if(cat != null)
            {
                var parent_id = cat.parent_id;
				while (parent_id != null)
                {
                    var parent = GetById(parent_id.Value);
                    if (parent != null)
                    {
                        list.Add(parent);
                        parent_id = parent.parent_id;
                    }
                }
            }
            
            list.Reverse();
            return list;
        }

        public bool IsInBrand(Web_category cat, int brand_id, IDbConnection conn = null)
        {
            if (cat.brand_id != null)
                return cat.brand_id == brand_id;
            var parents = GetParents(cat.category_id,conn);
            if (parents != null)
                return parents.Count(p => p.brand_id == brand_id) > 0;
            return false;
        }

        
        private static string GetTranslationFields()
        {
            return @"web_category_translate.category_id,
                    web_category_translate.language_id,
                    web_category_translate.`name`,
                    web_category_translate.path,
                    web_category_translate.title,
                    web_category_translate.description,
                    web_category_translate.pricing_note,
                    web_category_translate.group,
                    web_category_translate.alternate_name";
        }

        private string GetTranslationJoin()
        {
            return @" LEFT OUTER JOIN web_category_translate ON (web_category.category_id = web_category_translate.category_id AND web_category_translate.language_id = @lang)";
        }

        private string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true, object[] extraArgs = null)
        {
            List<string> fields = new List<string>();
            string join = string.Empty;
            var args = new List<object>();
            if (localize)
            {
                fields.Add(GetTranslationFields());
                join = GetTranslationJoin();
            }
            args.Add((commaBeforeFields && fields.Count > 0 ? ", " : "") + string.Join(",", fields.ToArray()) + (commaAfterFields && fields.Count > 0 ? "," : ""));
            args.Add(join);
            if (extraArgs != null)
                args.AddRange(extraArgs);
            
            return string.Format(initialSql,args.ToArray());
        }

        public List<Web_category> GetProductCategoriesForSearch(string text, int? site_id = null, bool files = false, List<Search_word> words = null, 
	        bool useFullText = true,int? catid = null, int minPrice = 0, int maxPrice = 15000, int minWidth = 0, int maxWidth = 5000, 
	        int minHeight = 0, int maxHeight = 5000, string tech_type = null, int minWeight = 0, int maxWeight = 500)
        {
            var result = new List<Web_category>();
            var synonims = new List<string>();
            text = text.Trim();
            text = useFullText ? text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "") : text;
            if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, "^[a-zA-Z0-9_-]+$"))
            {
                synonims.Add(string.Join(" ", text.Split(' ').Select(s => "+" + s)));
                if (words != null)
                {
                    var word = words.FirstOrDefault(w => w.word.ToLower() == text.ToLower());
                    if (word != null)
                        synonims = words.Where(w => w.group_id == word.group_id).Select(w => string.Format("({0})", string.Join(" ", w.word.Split(' ').Select(s => "+" + s)))).ToList();

                    //Handle individual words
                    var wordParts = text.Split(' ');
                    if (wordParts.Length > 1)
                    {
                        var synonim = synonims[0]; //modify original combination
                        foreach (string ws in wordParts)
                        {
                            word = words.FirstOrDefault(w => w.word.ToLower() == ws.ToLower());
                            if (word != null)
                            {
                                var partSynonims = words.Where(w => w.group_id == word.group_id);
                                synonim = synonim.Replace("+" + ws, string.Format("+({0})", string.Join(" ", partSynonims.Select(p => p.word))));
                                //synonims.AddRange(words.Where(w => w.group_id == word.group_id && w.word != word.word).Select(wordSynonim => string.Join(" ", text.Replace(ws, wordSynonim.word).Split(' ').Select(s => "+" + s))));
                            }
                        }
                        synonims[0] = synonim;
                    }

                }
            }

            
            var fromClause = useFullText
                                 ? @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique {0} 
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND ((MATCH(web_name) AGAINST(@text IN BOOLEAN MODE) OR MATCH(web_code) AGAINST(@text IN BOOLEAN MODE)) OR web_product_new.web_unique = @web_unique OR whitebook_title LIKE @text 
                    OR (MATCH(cust_products.cprod_name) AGAINST (@text IN BOOLEAN MODE) OR MATCH(cust_products.cprod_code1) AGAINST(@text IN BOOLEAN MODE)))"
                                 : @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique {0} 
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND (web_name LIKE @text OR web_code LIKE @text OR web_product_new.web_unique = @web_unique OR whitebook_title LIKE @text 
                    OR (cust_products.cprod_name LIKE @text OR cust_products.cprod_code1 LIKE @text))";
            
            fromClause = ((minPrice >= 0) && (maxPrice <= 15000) || (minWidth >= 0) && (maxWidth <= 5000) || (minHeight >= 0) && (maxHeight <= 5000) || (minWeight >= 0) && (maxWeight <= 500)) ? string.Format(fromClause, @" INNER JOIN web_product_component ON web_product_component.web_unique = web_product_new.web_unique INNER JOIN cust_products ON cust_products.cprod_id = web_product_component.cprod_id INNER JOIN web_category ON web_category.category_id = web_product_category.category_id INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast ") : string.Format(fromClause, "");

        
        
            if (catid != null && Convert.ToInt32(catid) > 0)
            {
                fromClause = string.Format("{0} AND web_category.parent_id=@catid ", fromClause);
            }
            if ((minPrice >= 0) && (maxPrice <= 15000))
            {
                fromClause = string.Format("{0} AND ((select SUM(cust_products.cprod_retail * web_product_component.qty) FROM cust_products INNER JOIN web_product_component ON web_product_component.cprod_id = cust_products.cprod_id WHERE web_product_component.web_unique = web_product_new.web_unique) BETWEEN {1}.00 AND {2}.00)", fromClause, minPrice, maxPrice);
            }
            if ((minWidth >= 0) && (maxWidth <= 5000))
            {
                fromClause = string.Format("{0} AND mast_products.prod_width BETWEEN {1}.00 AND {2}.00", fromClause, minWidth, maxWidth);
            }
            if ((minHeight >= 0) && (maxHeight <= 5000))
            {
                fromClause = string.Format("{0} AND mast_products.prod_height BETWEEN {1}.00 AND {2}.00", fromClause, minHeight, maxHeight);
            }
            if ((minWeight >= 0) && (maxWeight <= 500))
            {
                fromClause = string.Format("{0} AND (web_product_new.product_weight BETWEEN {1}.00 AND {2}.00 OR mast_products.prod_nw BETWEEN {1}.00 AND {2}.00)", fromClause, minWeight, maxWeight);
            }
            if (!string.IsNullOrEmpty(tech_type))
            {
                fromClause = string.Format("{0} AND web_product_new.tech_product_type LIKE {1}", fromClause, "'%" + tech_type + "%'");
                //cmd.Parameters.AddWithValue("@tech_type", "'%" + tech_type + "%'");
            }
            var query = string.Format(@"SELECT DISTINCT web_category.* {0}", fromClause);
			return conn.Query<Web_category>(query, new {catid, text = useFullText ? string.Join(" ", synonims) : "%" + text + "%", web_unique = text, site_id}).ToList();
            
        }

        public List<Web_category> DAMSearch(string text, int? site_id = null, string connstring = null)
        {
            var query = "SELECT * FROM web_category WHERE (name LIKE @text OR category_id LIKE @text OR alternate_name LIKE @text OR category_id IN (SELECT category_id FROM web_category WHERE parent_id LIKE @text AND brand_id = @site_id)) {0}";
            query = string.Format(query, site_id != null ? " AND (brand_id = @site_id OR @site_id IS NULL OR brand_id IS NULL)" : "");
	        return this.conn.Query<Web_category>(query, new {text = "%" + text + "%", site_id}).ToList();
        }

	    public List<Web_category> GetForSlaveHost(int host_id)
	    {
		    return conn.Query<Web_category>(
			    @"SELECT web_category.* FROM web_category INNER JOIN web_category_file_transfer 
					ON web_category.category_id = web_category_file_transfer.category_id
					  WHERE web_category_file_transfer.host_id = @host_id",  new {host_id}).ToList();
	    }

	    public void DeleteTransferData(int host_id, int cat_id)
	    {
		    conn.Execute("DELETE FROM web_category_file_transfer WHERE host_id = @host_id AND category_id = @cat_id", new { host_id, cat_id});
	    }

		protected override string GetAllSql()
		{
			return "SELECT * FROM web_category";
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
