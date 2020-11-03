using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	
	public class WebProductNewDal: GenericDal<Web_product_new>, IWebProductNewDal
	{
        public const int RelatedProductsType = 3;
				
		private IWebCategoryDal webCategoryDal;
		private IWebSiteDal webSiteDal;
		private IWebProductComponentDal webProductComponentDal;
		private IWebProductFileDal webProductFileDal;
		private IWebProductInfoDal webProductInfoDal;
		private IWebProductFlowDal webProductFlowDal;
		private IMastProductsDal mastProductsDal;
		
	    //public WebProductNewDal(string connString, IWebCategoryDal webCategoryDal)
	    //{
		   // this.webCategoryDal = webCategoryDal;
		   // CreateOpenConnection(connString);
	    //}

		public WebProductNewDal(IDbConnection conn, IWebCategoryDal webCategoryDal, IWebSiteDal webSiteDal, 
			IWebProductFileDal webProductFileDal, IWebProductInfoDal webProductInfoDal, IWebProductComponentDal webProductComponentDal,
			IWebProductFlowDal webProductFlowDal, IMastProductsDal mastProductsDal) : base(conn)
		{
			this.webCategoryDal = webCategoryDal;
			this.webSiteDal = webSiteDal;
			this.webProductFileDal = webProductFileDal;
			this.webProductInfoDal = webProductInfoDal;
			this.webProductFlowDal = webProductFlowDal;
			this.mastProductsDal = mastProductsDal;
			this.webProductComponentDal = webProductComponentDal;
		}
			    

        public List<Web_product_new> GetForCategory(int cat_id,bool deep = true,bool category=false,int? language_id= null,bool files = false,
	        bool related = false,bool complementary=false,bool components = true, bool previewTestProducts=false)
        {
			string sql;
			var cats = new List<Web_category>();
            
            if (deep)
            {
                cats.Add(new Web_category { category_id = cat_id });
                cats.AddRange(webCategoryDal.GetAllChildren(cat_id));
                sql = GetSelect(@"SELECT web_product_new.* {0} 
                                                FROM web_product_new INNER JOIN web_product_category ON web_product_new.web_unique = web_product_category.web_unique {1}",
                                            language_id != null,commaAfterFields:false,commaBeforeFields:true);
                if (!previewTestProducts)
                {
                    sql += " WHERE category_id IN @catIds AND web_product_new.web_status = 1";
                }
                else
                {
                    sql += @" WHERE category_id IN @catIds AND 
							((web_product_new.web_status = 0 AND web_product_new.new_product_flag = 1 ) 
								OR web_product_new.web_status = 1)";
                }
            }
            else
            {
                if(!previewTestProducts)
                    sql = GetSelect("SELECT web_product_new.* {0} FROM web_product_new INNER JOIN web_product_category ON web_product_new.web_unique = web_product_category.web_unique {1} WHERE category_id = @cat_id AND web_product_new.web_status = 1", language_id != null, commaAfterFields: false, commaBeforeFields: true);
                else
                    sql = GetSelect(@"SELECT web_product_new.* {0} 
							FROM web_product_new INNER JOIN web_product_category ON web_product_new.web_unique = web_product_category.web_unique {1} 
							WHERE category_id = @cat_id  AND ((web_product_new.web_status = 0 AND web_product_new.new_product_flag = 1 ) 
									OR web_product_new.web_status = 1)", language_id != null, commaAfterFields: false, commaBeforeFields: true);

                
            }
            
			var result = conn.Query<Web_product_new>(sql,
				new {cat_id, lang = language_id, catIds = cats.Select(c => c.category_id)}).ToList();
                
                var sites = webSiteDal.GetAll();
                foreach (var p in result)
                {
                    if(components)
                        p.Components = webProductComponentDal.GetForProduct(p.web_unique, language_id: language_id);

                    if (category)
                    {
                        p.SelectedCategories = webCategoryDal.GetForProduct(p.web_unique, conn);
                        if(p.SelectedCategories != null && p.SelectedCategories.Count > 0)
                            p.Category = p.SelectedCategories.First();
                    }
                    if(files)
                    {
                        p.WebFiles = webProductFileDal.GetForProduct(p.web_unique,conn);
                    }
                    if (related)
                    {
                        p.RelatedProducts = GetRelated(p.web_unique,(int) RelationType.Complementary,conn: conn);
                    }
                    p.WebSite = sites.FirstOrDefault(s => s.id == p.web_site_id);
                    //if (complementary)
                    //{
                    //    p.Complementary = web_product_new_relatedDAL.GetForProduct(p.web_unique, web_product_new: true, web_files: true, conn: conn);
                    ////    //p.Complementary=
                    //}
                }
                
            return result;
        }

        public List<Web_product_new> GetForIds(IList<int> ids,int? web_site_id = null,int? language_id = null,
	        bool files = false,bool searchForCategoryProducts = false)
        {
            
            var sql = GetSelect(@"SELECT web_product_new.* {0} 
                                                FROM web_product_new INNER JOIN web_product_category ON web_product_new.web_unique = web_product_category.web_unique {1}",
                                            language_id != null, commaBeforeFields:true, commaAfterFields: false);
            sql += " WHERE web_product_new.web_unique IN @ids AND (web_site_id = @site_id OR @site_id IS NULL)";
	        var result = conn.Query<Web_product_new>(sql, new {ids, site_id = web_site_id, lang = language_id}).ToList();
            foreach (var p in result)
            {
                p.Components = webProductComponentDal.GetForProduct(p.web_unique, language_id: language_id,conn:conn);
                p.SelectedCategories = webCategoryDal.GetForProduct(p.web_unique, conn,searchForCategoryProducts);
                if (files)
                    p.WebFiles = webProductFileDal.GetForProduct(p.web_unique);
            }
	        return result;

        }

        private string GetSelectClause(bool localize = false)
        {
            if (!localize)
                return "SELECT * FROM web_product_new ";
            else
                return string.Format(@"SELECT web_product_new.*, {0}  FROM
                        web_product_new  {1}", GetTranslationFields(false), GetTranslationJoin());

        }

        private string GetTranslationJoin()
        {
            return @" LEFT OUTER JOIN web_product_new_translate ON (web_product_new.web_unique = web_product_new_translate.web_unique AND web_product_new_translate.lang = @lang)";
        }
        
        public List<Web_product_new> Search(string text, out int totalCount, int? site_id = null, int? page = null, 
	        int? pageSize = null, bool files = false, List<Search_word> words = null, bool useFullText = true, int? catid = null)
        {
            var result = new List<Web_product_new>();
            var synonims = new List<string>();
            text = text.Trim();
            if(!string.IsNullOrEmpty(text))
            {
                text = useFullText ? text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "") : text;
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
            
            int? from = pageSize != null ? pageSize * page : 0;
            int? to = pageSize != null ? pageSize * (page + 1) - 1 : 0;

            var fromClause = useFullText
                                 ? @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique {0}
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND ((MATCH(web_name) AGAINST(@text IN BOOLEAN MODE) OR MATCH(web_code) AGAINST(@text IN BOOLEAN MODE)) OR web_product_new.web_unique = @web_unique
                    OR EXISTS(SELECT cust_products.cprod_id FROM cust_products INNER JOIN web_product_component ON cust_products.cprod_id = web_product_component.cprod_id 
                    WHERE web_product_component.web_unique = web_product_new.web_unique AND (MATCH(cust_products.cprod_name) AGAINST (@text IN BOOLEAN MODE) OR MATCH(cust_products.cprod_code1) AGAINST(@text IN BOOLEAN MODE))) {1})"
                                 : @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique  {0}
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND (web_name LIKE @text OR web_code LIKE @text OR web_product_new.web_unique = @web_unique
                    OR EXISTS(SELECT cust_products.cprod_id FROM cust_products INNER JOIN web_product_component ON cust_products.cprod_id = web_product_component.cprod_id 
                    WHERE web_product_component.web_unique = web_product_new.web_unique AND (cust_products.cprod_name LIKE @text OR cust_products.cprod_code1 LIKE @text))) {1}";

            if (catid != null && Convert.ToInt32(catid) > 0)
            {
                fromClause = string.Format(fromClause," INNER JOIN web_category ON web_category.category_id = web_product_category.category_id", " AND web_category.category_id=@catid");
            }
            else
            {
                fromClause = string.Format(fromClause,"","");
            }

            conn.Open();
            var sql = $@"SELECT DISTINCT web_product_new.*  {fromClause}
                    {(pageSize != null ? 
	            $" ORDER BY web_product_new.web_name, web_product_new.web_code LIMIT {from},{pageSize} " : "")}";
			var results = conn.Query<Web_product_new>(sql,
		        new
		        {
			        catid,
			        text = useFullText ? string.Join(" ", synonims) : "%" + text + "%",
			        web_unique = text,
			        site_id
		        });
	        
            foreach (var p in result)
            {
                p.Components = webProductComponentDal.GetForProduct(p.web_unique, conn);
                //p.Category = Web_categoryDAL.GetFromDataReader(dr);
                p.SelectedCategories = webCategoryDal.GetForProduct(p.web_unique, conn);
                if (files)
                    p.WebFiles = webProductFileDal.GetForProduct(p.web_unique, conn);
            }

            if (pageSize != null)
            {
	            totalCount = conn.ExecuteScalar<int>("SELECT COUNT(*) " + fromClause);
            }
            else
            {
                totalCount = 0;
            }
            return result;
        }

        public List<Web_product_new> Search(string text, int? site_id = null, bool files = false, List<Search_word> words = null, bool useFullText = true)
        {
            var result = new List<Web_product_new>();
            var synonims = new List<string>();
            text = text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                text = useFullText ? text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "") : text;
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
        
            //var cmd = Utils.GetCommand(string.Format(@"SELECT web_product_new.*  {0} ORDER BY web_product_new.web_code ASC", fromClause), conn);
            var sql = "GetProductsForAjaxSearch";
	        var results = conn.Query<Web_product_new>(sql, new
	        {
		        text = useFullText ? string.Join(" ", synonims) : "%" + text + "%",
		        web_unique = text,
		        site_id
	        }, commandType: CommandType.StoredProcedure).ToList();
            
            foreach (var p in result)
            {
                p.Components = webProductComponentDal.GetForProduct(p.web_unique, conn);
                //p.Category = Web_categoryDAL.GetFromDataReader(dr);
                p.SelectedCategories = webCategoryDal.GetForProduct(p.web_unique, conn);
                if (files)
                    p.WebFiles = webProductFileDal.GetForProduct(p.web_unique, conn);
            }
            

            return result;
        }

        public List<Web_product_new> Search(string text, out int totalCount, out double maxprice, out int maxwidth, 
	        out int maxheight, out int maxweight, out List<string> tech_types, int? site_id = null, int? page = null, 
	        int? pageSize = null, bool files = false, List<Search_word> words = null, bool useFullText = true, int? catid = null, 
	        int? minPrice = 0, int? maxPrice = 15000, int minWidth = 0, int maxWidth = 5000, int minHeight = 0, int maxHeight = 5000, 
	        string tech_type = null, int minWeight = 0, int maxWeight = 500)
        {
            var result = new List<Web_product_new>();
            var synonims = new List<string>();
            text = text.Trim();
            var tech_typesTemp = new List<string>();
            if(!string.IsNullOrEmpty(text))
            {
                text = useFullText ? text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "") : text;
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

            int? from = pageSize != null ? pageSize*page : 0;
            int? to = pageSize != null ? pageSize*(page + 1) - 1 : 0;

            var fromClause = useFullText
                                 ? @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique {0} 
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND ((MATCH(web_name) AGAINST(@text IN BOOLEAN MODE) OR MATCH(web_code) AGAINST(@text IN BOOLEAN MODE)) OR web_product_new.web_unique = @web_unique OR whitebook_title LIKE @text 
                    OR (MATCH(cust_products.cprod_name) AGAINST (@text IN BOOLEAN MODE) OR MATCH(cust_products.cprod_code1) AGAINST(@text IN BOOLEAN MODE)))"
                                 : @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique {0} 
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND (web_name LIKE @text OR web_code LIKE @text OR web_product_new.web_unique = @web_unique OR whitebook_title LIKE @text 
                    OR (cust_products.cprod_name LIKE @text OR cust_products.cprod_code1 LIKE @text))";

            fromClause = ((minPrice >= 0) && (maxPrice <= 15000) || (minWidth >= 0) && (maxWidth <= 5000) || (minHeight >= 0) && (maxHeight <= 5000) || (minWeight >= 0) && (maxWeight <= 500)) ? string.Format(fromClause, @" INNER JOIN web_product_component ON web_product_component.web_unique = web_product_new.web_unique INNER JOIN cust_products ON cust_products.cprod_id = web_product_component.cprod_id INNER JOIN web_category ON web_category.category_id = web_product_category.category_id INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast ") : string.Format(fromClause, "");
			
            if((minPrice >= 0) && (maxPrice <= 15000))
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
            if(!string.IsNullOrEmpty(tech_type))
            {
                fromClause = string.Format("{0} AND web_product_new.tech_product_type LIKE {1}", fromClause, "'%" + tech_type + "%'");
                //cmd.Parameters.AddWithValue("@tech_type", "'%" + tech_type + "%'");
            }
            var query = $@"SELECT DISTINCT web_product_new.* {fromClause} 
				{(pageSize != null ? $" ORDER BY web_product_new.web_name, web_product_new.web_code LIMIT {from},{pageSize} " : "")} ";
            
	        var results = conn.Query<Web_product_new>(query,
		        new
		        {
			        text = useFullText ? string.Join(" ", synonims) : "%" + text + "%",
			        web_unique = text,
			        site_id,
					catid
		        });
			
            var sqlCount = "SELECT COUNT(*) " + fromClause;
            
            totalCount = conn.ExecuteScalar<int>(sqlCount);

            foreach (var p in results)
            {
                p.Components = webProductComponentDal.GetForProduct(p.web_unique, conn);
                //p.Category = Web_categoryDAL.GetFromDataReader(dr);
                p.SelectedCategories = webCategoryDal.GetForProduct(p.web_unique, conn);
                if (files)
                    p.WebFiles = webProductFileDal.GetForProduct(p.web_unique, conn);
            }

            double maxW, maxH;
            var sql = "SELECT MAX(sumatable.suma) FROM(SELECT SUM(cust_products.cprod_retail_web_override * web_product_component.qty) AS suma " + fromClause + " GROUP BY web_product_new.web_unique) AS sumatable";
            maxprice = conn.ExecuteScalar<double>(sql);
            if(maxprice <= 0)
            {
                sql = "SELECT MAX(sumatable.suma) FROM(SELECT SUM(cust_products.cprod_retail * web_product_component.qty) AS suma " + fromClause + " GROUP BY web_product_new.web_unique) AS sumatable";
                maxprice = conn.ExecuteScalar<double?>(sql) ?? 15000.0;
            }

            sql = "SELECT MAX(web_product_new.override_width) " + fromClause;
            maxW = conn.ExecuteScalar<double>(sql);
            if (maxW <= 0)
            {
                sql = "SELECT MAX(mast_products.prod_width) " + fromClause;
                maxwidth = conn.ExecuteScalar<int?>(sql) ?? 5000;
            }
            else
                maxwidth = Convert.ToInt32(maxW);

            sql = "SELECT MAX(web_product_new.override_height) " + fromClause;
            maxH = conn.ExecuteScalar<double>(sql);
            if (maxH <= 0)
            {
                sql = "SELECT MAX(mast_products.prod_height) " + fromClause;
                maxheight = conn.ExecuteScalar<int?>(sql) ?? 5000;
            }
            else
                maxheight = Convert.ToInt32(maxH);

            sql = "SELECT MAX(web_product_new.product_weight) " + fromClause;
            maxweight = conn.ExecuteScalar<int>(sql);
            if (maxweight <= 0)
            {
                sql = "SELECT MAX(mast_products.prod_nw) " + fromClause;
                maxweight = conn.ExecuteScalar<int>(sql);
            }

            sql = "SELECT DISTINCT web_product_new.tech_product_type " + fromClause;
            tech_types = conn.Query<string>(sql).ToList();
            
            return result;
        }

        public int GetProductSearchCount(string text, int? site_id = null, int? catid = null, bool useFullText = false)
        {
            
            var synonims = new List<string>();
            text = text.Trim();
            text = useFullText ? text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "") : text;
            if (!string.IsNullOrEmpty(text))
            {
                synonims.Add(string.Join(" ", text.Split(' ').Select(s => "+" + s)));
            }
            
            var fromClause = useFullText
                                 ? @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique {0}
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND ((MATCH(web_name) AGAINST(@text IN BOOLEAN MODE) OR MATCH(web_code) AGAINST(@text IN BOOLEAN MODE)) OR web_product_new.web_unique = @web_unique
                    OR EXISTS(SELECT cust_products.cprod_id FROM cust_products INNER JOIN web_product_component ON cust_products.cprod_id = web_product_component.cprod_id 
                    WHERE web_product_component.web_unique = web_product_new.web_unique AND (MATCH(cust_products.cprod_name) AGAINST (@text IN BOOLEAN MODE) OR MATCH(cust_products.cprod_code1) AGAINST(@text IN BOOLEAN MODE)) ))"
                                 : @"FROM web_product_new INNER JOIN web_product_category ON web_product_category.web_unique = web_product_new.web_unique  {0}
                     WHERE (web_product_new.web_site_id = @site_id OR @site_id IS NULL) AND (web_name LIKE @text OR web_code LIKE @text OR web_product_new.web_unique = @web_unique
                    OR EXISTS(SELECT cust_products.cprod_id FROM cust_products INNER JOIN web_product_component ON cust_products.cprod_id = web_product_component.cprod_id 
                    WHERE web_product_component.web_unique = web_product_new.web_unique AND (cust_products.cprod_name LIKE @text OR cust_products.cprod_code1 LIKE @text)))";

            
            if (catid != null && Convert.ToInt32(catid) > 0)
            {
                fromClause = $"{fromClause} AND web_product_category.category_id=@catid ";
            }

            var query = string.Format(@"SELECT COUNT(*) {0}", fromClause);
	        return conn.ExecuteScalar<int>(query, new
	        {
		        text = useFullText ? string.Join(" ", synonims) : "%" + text + "%",
		        web_unique = text,
		        site_id,
		        catid
	        });
            
        }

        /// <summary>
        /// WARNING: After regenerating code in web_product_newDAL, getbyid in that file should be deleted to avoid duplication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Web_product_new GetByIdEx(int id, int? language_id = null, IDbConnection conn = null, bool loadSubObjects = true)
        {
            return GetByAnyId(GetSelect("SELECT web_product_new.* {0} FROM web_product_new {1} WHERE web_unique = @id", language_id != null,commaBeforeFields:true, commaAfterFields:false), id,language_id,conn);
        }

        public Web_product_new GetByLegacyId(int id,int? language_id = null)
        {
            return GetByAnyId(GetSelect("SELECT web_product_new.* {0} FROM web_product_new {1} WHERE legacy_id = @id", language_id != null,commaBeforeFields:true, commaAfterFields: false), id,language_id);
        }

        private Web_product_new GetByAnyId(string sql, int id, int? language_id = null, IDbConnection conn = null, bool loadSubObjects = true)
        {
	        var dbConn = (conn ?? this.conn);
	        var result = dbConn.QueryFirstOrDefault<Web_product_new>(sql, new {id});
            
            if (result != null)
            {
                if (loadSubObjects)
                {
                    result.ProductFlows = webProductFlowDal.GetForProduct(result.web_unique, dbConn);
                    result.WebFiles = webProductFileDal.GetForProduct(result.web_unique, dbConn);
                    result.ProductInfo = webProductInfoDal.GetForProduct(result.web_unique, dbConn, language_id);
                    result.Components = webProductComponentDal.GetForProduct(result.web_unique, dbConn, language_id);
                    result.SelectedCategories = webCategoryDal.GetForProduct(result.web_unique, dbConn);
                    //result.SelectedSalePeriods = SaleDAL.GetForProduct(result.web_unique, conn);
                    result.RelatedProducts = GetRelated(result.web_unique,(int) RelationType.Complementary, files: true);

                    result.Siblings = GetSiblings(result.web_unique, (MySqlConnection)dbConn);
                    //result.SuggestedProducts = GetSuggested(result.web_unique, (MySqlConnection) conn);
                    result.Children = GetChildren(result.web_unique, (MySqlConnection) dbConn);


                }
                if (result.parent_id != null)
                    result.Parent = GetParent(result.web_unique, conn: (MySqlConnection) dbConn);
            }
                
            
            return result;
        }

        //private static ICollection<Web_product_new> GetRelated(int webUnique, MySqlConnection conn,int? language_id = null,bool files = false)
        //{
        //    var result = new List<Web_product_new>();
        //    var cmd =
        //        Utils.GetCommand(
        //            GetSelect("SELECT web_product_new.* {0} FROM web_product_new INNER JOIN web_product_new_related ON web_product_new.web_unique = web_product_new_related.web_unique_related {1} WHERE web_product_new_related.web_unique = @web_unique",language_id != null,commaBeforeFields:true,commaAfterFields:false),conn);
        //    if (language_id != null)
        //        cmd.Parameters.AddWithValue("@lang", language_id);
        //    cmd.Parameters.AddWithValue("@web_unique", webUnique);
        //    var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        result.Add(GetFromDataReaderEx(dr));
        //    }
        //    dr.Close();

        //    foreach (var r in result)
        //    {
        //        if (files)
        //            r.WebFiles = Web_product_fileDAL.GetForProduct(r.web_unique, conn);
        //        r.SelectedCategories = Web_categoryDAL.GetForProduct(r.web_unique, conn);
        //        r.Components = Web_product_componentDAL.GetForProduct(r.web_unique, conn);
        //    }

        //    return result;
        //}

        public List<Web_product_new> GetRelated(int webUnique, int? relation_type = null, int? language_id = null, 
	        bool files = false, IDbConnection conn = null)
        {

	        var sql =
		        "SELECT web_product_new.* FROM web_product_new INNER JOIN web_product_new_related ON web_product_new.web_unique = web_product_new_related.web_unique_related WHERE web_product_new_related.web_unique = @web_unique";
            if (relation_type != null)
            {
                sql += " AND web_product_new_related.relation_type=@relation_type ";
            }

	        var results = (conn ?? this.conn).Query<Web_product_new>(sql, new
	        {
		        relation_type,
		        lang = language_id,
		        web_unique = webUnique
	        }).ToList();
            
            foreach (var r in results)
            {
                if (files)
                    r.WebFiles = webProductFileDal.GetForProduct(r.web_unique, conn);
                r.SelectedCategories = webCategoryDal.GetForProduct(r.web_unique, conn);
                r.Components = webProductComponentDal.GetForProduct(r.web_unique, conn);
            }
            
            return results;
        }

        public ICollection<Web_product_new> GetRecomendedProduct(int webUnique, int? language_id = null, bool files = false)
        {
            var sql =
                GetSelect("SELECT web_product_new.* {0} FROM web_products_recommended INNER JOIN web_product_new ON web_products_recommended.comp_product = web_product_new.web_unique {1} WHERE web_products_recommended.web_unique2 = @web_unique AND web_products_recommended", language_id != null, commaBeforeFields: true, commaAfterFields: false);
            
	        var results = conn.Query<Web_product_new>(sql, new {lang = language_id, web_unique = webUnique}).ToList();
            if (files)
            {
                foreach (var r in results)
                {
                    r.WebFiles = webProductFileDal.GetForProduct(r.web_unique, conn);
                }
            }
	        return results;
        }

        /// <summary>
        /// Siblings are related by same parent. Used in options - one of the products is main (parent) and others are "children"
        /// </summary>
        /// <param name="webUnique"></param>
        /// <param name="conn"></param>
        /// <param name="language_id"></param>
        /// <returns></returns>
        public ICollection<Web_product_new> GetSiblings(int webUnique, MySqlConnection conn, int? language_id = null)
        {
            var sql =
                    GetSelect("SELECT web_product_new.* {0} FROM web_product_new {1} WHERE web_product_new.parent_id = (SELECT parent_id FROM web_product_new WHERE web_unique = @web_unique)", language_id != null, commaBeforeFields: true, commaAfterFields: false);
            var results = conn.Query<Web_product_new>(sql, new {lang = language_id, web_unique = webUnique}).ToList();
            
            foreach (var r in results)
            {
                r.WebFiles = webProductFileDal.GetForProduct(r.web_unique, conn);
            }
            return results;
        }


        public ICollection<Web_product_new> GetChildren(int webUnique, MySqlConnection conn, int? language_id = null)
        {
            
            var sql =
                GetSelect("SELECT web_product_new.* {0} FROM web_product_new {1} WHERE web_product_new.parent_id = @web_unique", language_id != null, commaBeforeFields: true, commaAfterFields: false);
            var results = conn.Query<Web_product_new>(sql, new {lang = language_id, web_unique = webUnique}).ToList();
            
	        foreach (var r in results)
	        {
		        r.WebFiles = webProductFileDal.GetForProduct(r.web_unique, conn);
		        r.Components = webProductComponentDal.GetForProduct(r.web_unique);
	        }
	        return results;
        }

        public bool HasChildren(int webUnique)
        {
	        return conn.ExecuteScalar<bool>(
		        "SELECT COALESCE(COUNT(*),0) FROM web_product_new WHERE parent_id = @web_unique",
		        new {web_unique = webUnique});
        }


        public Web_product_new GetParent(int web_unique,bool loadChildren = false, IDbConnection conn = null)
        {
            
            var sql = "SELECT * FROM web_product_new WHERE web_unique = (SELECT parent_id FROM web_product_new WHERE web_unique = @id LIMIT 1)";
	        var result = (conn ?? this.conn).QueryFirstOrDefault<Web_product_new>(sql, new {id = web_unique});
            if (result != null && loadChildren)
            {
                result.Children = GetChildren(result.web_unique, (MySqlConnection) conn);
                result.Components = webProductComponentDal.GetForProduct(result.web_unique);
            }
			return result;
        }



        //private ICollection<Web_product_new> GetSuggested(int webUnique, MySqlConnection conn, int? language_id = null)
        //{
        //    var result = new List<Web_product_new>();
        //    var cmd =  Utils.GetCommand(
        //     GetSelect("SELECT web_product_new.* {0} FROM web_product_new INNER JOIN web_product_new_suggested ON web_product_new.web_unique = web_product_new_suggested.web_unique_suggested {1} WHERE web_product_new_suggested.web_unique = @web_unique",language_id != null,commaBeforeFields:true,commaAfterFields:false),conn);
        //    if (language_id != null)
        //    {
        //        cmd.Parameters.AddWithValue("@lang", language_id);
        //    }
        //    cmd.Parameters.AddWithValue("@web_unique", webUnique);
        //        var dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            result.Add(GetFromDataReaderEx(dr));
        //        }
        //        dr.Close();

        //    return result;
        //}

        public void CreateEx(Web_product_new p, bool generateId = true)
        {
            string insertsql = @"INSERT INTO web_product_new (web_unique,web_name,web_site_id,web_description,web_code,hi_res,web_pic_notes,web_details,web_seq,product_weight,bath_volume,
                    tech_finishes,tech_product_type,tech_construction,tech_material,tech_basin_size,tech_overall_height,tech_tap_holes,tech_fixing,web_auto,guarantee,brand_group,gold_code,web_status,
                    overflow_class,overflow_rate,combination_comments,tech_water_volume_note,image_gallery,parent_id,option_name,override_length,override_width,override_height,sale_on,datecreated,created_by,
                    datemodified,modified_by,legacy_id,web_code_override) 
                    VALUES(@web_unique,@web_name,@web_site_id,@web_description,@web_code,@hi_res,@web_pic_notes,@web_details,@web_seq,@product_weight,@bath_volume,@tech_finishes,
                    @tech_product_type,@tech_construction,@tech_material,@tech_basin_size,@tech_overall_height,@tech_tap_holes,@tech_fixing,@web_auto,@guarantee,@brand_group,@gold_code,@web_status,@overflow_class,
                    @overflow_rate,@combination_comments,@tech_water_volume_note,@image_gallery,@parent_id,@option_name,@override_length,@override_width,@override_height,@sale_on,@datecreated,@created_by,
                    @datemodified,@modified_by,@legacy_id,@web_code_override)";

            MySqlTransaction tr = null;
                try
                {
                    tr = conn.BeginTransaction();
                    
                    if (generateId)
                    {
                        p.web_unique = conn.ExecuteScalar<int>("SELECT MAX(web_unique)+1 FROM web_product_new");
                    }
	                conn.Execute(insertsql, p,tr);
                    if (p.Components != null)
                    {
                        var sql =
                            "INSERT INTO web_product_component(web_unique, cprod_id,qty,`order`) VALUES(@web_unique,@cprod_id,@qty,@order)";
                        foreach (var c in p.Components)
                        {
	                        c.web_unique = p.web_unique;
	                        conn.Execute(sql, c,tr);
                        }
                    }

                    if (p.SelectedSalePeriods != null)
                    {
                        var sql =
                            "INSERT INTO web_product_new_sale(web_unique, sale_id) VALUES(@web_unique,@sale_id)";
                        foreach (var s in p.SelectedSalePeriods)
                        {
	                        conn.Execute(sql, new {p.web_unique, sale_id = s.IdSale},tr);
                        }
                    }

                    if (p.SelectedCategories != null)
                    {
                        var sql =
                            "INSERT INTO web_product_category(web_unique, category_id) VALUES(@web_unique,@category_id)";
                        foreach (var c in p.SelectedCategories)
                        {
	                        conn.Execute(sql, new {p.web_unique, c.category_id},tr);
                        }
                    }

                    if (p.WebFiles != null)
                    {
                        foreach (var im in p.WebFiles)
                        {
                            im.web_unique = p.web_unique;
                            webProductFileDal.CreateEx(im, tr);
                        }
                    }
                    if (p.ProductInfo != null)
                    {
                        foreach (var pi in p.ProductInfo)
                        {
                            pi.web_unique = p.web_unique;
                            webProductInfoDal.Create(pi, tr);
                        }
                    }
                    if (p.ProductFlows != null)
                    {
                        foreach (var pf in p.ProductFlows)
                        {
                            pf.web_unique = p.web_unique;
                            webProductFlowDal.Create(pf, tr);
                        }
                    }
                    if (p.RelatedProducts != null)
                    {
                        var sql =
                            "INSERT INTO web_product_new_related(web_unique, web_unique_related) VALUES(@web_unique,@web_unique_related)";
                        
                        foreach (var rp in p.RelatedProducts)
                        {
	                        conn.Execute(sql, new {p.web_unique, web_unique_related = rp.web_unique},tr);
                        }
                    }
                    tr.Commit();
                }
                catch (Exception)
                {
	                tr?.Rollback();
	                throw;
                }
                finally
                {
                    tr = null;
                }

            
        }

        public void UpdateEx(Web_product_new p)
        {
            string updatesql = @"UPDATE web_product_new SET web_name = @web_name,web_site_id = @web_site_id,web_description = @web_description,web_code = @web_code,hi_res = @hi_res,web_pic_notes = @web_pic_notes,
                                web_details = @web_details,web_seq = @web_seq,product_weight = @product_weight,bath_volume = @bath_volume,tech_finishes = @tech_finishes,tech_product_type = @tech_product_type,
                                tech_construction = @tech_construction,tech_material = @tech_material,tech_basin_size = @tech_basin_size,tech_overall_height = @tech_overall_height,
                                tech_tap_holes = @tech_tap_holes,tech_fixing = @tech_fixing,web_auto = @web_auto,guarantee = @guarantee,brand_group = @brand_group,gold_code = @gold_code,
                                product_gold_code = @product_gold_code,web_status = @web_status,overflow_class = @overflow_class,overflow_rate = @overflow_rate,
                                combination_comments = @combination_comments,tech_water_volume_note = @tech_water_volume_note,image_gallery = @image_gallery,parent_id = @parent_id,
                                option_name = @option_name,override_length = @override_length,override_width = @override_width,override_height = @override_height,sale_on = @sale_on,
                                datecreated = @datecreated,created_by = @created_by,datemodified = @datemodified,modified_by = @modified_by,legacy_id = @legacy_id,batch_no = @batch_no,
                                batch_id = @batch_id,design_template = @design_template,sub_title_1 = @sub_title_1,glass_thickness = @glass_thickness,adjustment = @adjustment,
                                whitebook_batch=@whitebook_batch,whitebook_title=@whitebook_title,whitebook_description=@whitebook_description,whitebook_material=@whitebook_material,
                                whitebook_notes=@whitebook_notes,show_component_dimensions=@show_component_dimensions,show_component_weights=@show_component_weights, option_header_override=@option_header_override,
                                web_code_override = @web_code_override
                                WHERE web_unique = @web_unique";

            
                MySqlTransaction tr = null;
                try
                {
            
                    tr = conn.BeginTransaction();
	                conn.Execute(updatesql, p, tr);

                    if (p.WebFiles != null)
                    {
                        foreach (var im in p.WebFiles)
                        {
                            if (im.id <= 0)
                            {
                                im.web_unique = p.web_unique;
                                webProductFileDal.CreateEx(im, tr);
                            }
                            else
                            {
                                webProductFileDal.UpdateEx(im, tr);
                            }
                        }
                        if(p.WebFiles.Count > 0)
                            webProductFileDal.DeleteMissing(p.web_unique, p.WebFiles.Select(im => im.id).ToList(), tr);
                    }

                    if (p.Components != null)
                    {

                        var sql  = "DELETE FROM web_product_component WHERE web_unique = @web_unique";
	                    conn.Execute(sql, new { p.web_unique},tr);

                        sql =
                            "INSERT INTO web_product_component(web_unique, cprod_id,qty,`order`) VALUES(@web_unique,@cprod_id,@qty,@order)";
                        foreach (var c in p.Components)
                        {
	                        conn.Execute(sql, new {p.web_unique, c.cprod_id, c.qty, c.order}, tr);
                        }
                    }
                    else
                    {
                        var sql  = "DELETE FROM web_product_component WHERE web_unique = @web_unique";
	                    conn.Execute(sql, new {p.web_unique},tr);
                    }

                    if (p.SelectedSalePeriods != null)
                    {
                        var sql  = "DELETE FROM web_product_new_sale WHERE web_unique = @web_unique";
	                    conn.Execute(sql, new {p.web_unique},tr);

                        sql =
                            "INSERT INTO web_product_new_sale(web_unique, sale_id) VALUES(@web_unique,@sale_id)";
                        foreach (var s in p.SelectedSalePeriods)
                        {
	                        conn.Execute(sql, new {p.web_unique, sale_id = s.IdSale}, tr);
                        }
                    }

                    if(p.SelectedCategories != null)
                    {
                        var sql = "DELETE FROM web_product_category WHERE web_unique = @web_unique";
	                    conn.Execute(sql, new { p.web_unique},tr);

                        sql =
                            "INSERT INTO web_product_category(web_unique, category_id) VALUES(@web_unique,@category_id)";
                        foreach (var c in p.SelectedCategories)
                        {
	                        conn.Execute(sql, new {p.web_unique, c.category_id}, tr);
                        }
                    }
                    List<int> infoIds = null, flowIds = null;
                    if (p.ProductInfo != null)
                    {
                        foreach (var pi in p.ProductInfo)
                        {
                            if (pi.id <= 0)
                            {
                                pi.web_unique = p.web_unique;
                                webProductInfoDal.Create(pi, tr);
                            }
                            else
                            {
                                webProductInfoDal.Update(pi, tr);
                            }
                        }
                        if (p.ProductInfo.Count > 0)
                            infoIds = p.ProductInfo.Select(pi => pi.id).ToList();
                    }
                    webProductFileDal.DeleteMissing(p.web_unique,infoIds, tr);

                    if (p.ProductFlows != null)
                    {
                        foreach (var pf in p.ProductFlows)
                        {
                            if (pf.id <= 0)
                            {
                                pf.web_unique = p.web_unique;
                                webProductFlowDal.Create(pf, tr);
                            }
                            else
                            {
                                webProductFlowDal.Update(pf, tr);
                            }
                        }
                        if (p.ProductFlows.Count > 0)
                            flowIds = p.ProductFlows.Select(pf => pf.id).ToList();

                    }
                    webProductFlowDal.DeleteMissing(p.web_unique,flowIds, tr);


                    if (p.RelatedProducts != null)
                    {
                        var sql = $"DELETE FROM web_product_new_related WHERE web_unique = @web_unique AND relation_type = {(int) RelationType.Complementary}";
	                    conn.Execute(sql, new {p.web_unique}, tr);

                        sql =
                            $"INSERT INTO web_product_new_related(web_unique, web_unique_related,relation_type) VALUES(@web_unique,@web_unique_related,{(int) RelationType.Complementary})";

                        foreach (var wu in p.RelatedProducts.Select(rp=>rp.web_unique).Distinct())
                        {
	                        conn.Execute(sql, new {p.web_unique, web_unique_related = wu}, tr);
                            
                        }
                    }
                    else
                    {
                        var sql = $"DELETE FROM web_product_new_related WHERE web_unique = @web_unique AND relation_type = {(int)RelationType.Complementary}";
	                    conn.Execute(sql, new {p.web_unique});
                    }

                    //if (p.CompatibleProducts != null)
                    //{
                    //    cmd.CommandText = "DELETE FROM web_product_new_related WHERE web_unique = @web_unique AND relation_type = 3";
                    //    cmd.Parameters.Clear();
                    //    cmd.Parameters.AddWithValue("@web_unique", p.web_unique);
                    //    cmd.ExecuteNonQuery();

                    //    cmd.CommandText =
                    //        "INSERT INTO web_product_new_related(web_unique, web_unique_related,relation_type) VALUES(@web_unique,@web_unique_related,3)";

                    //    cmd.Parameters.AddWithValue("@web_unique_related", 0);
                    //    foreach (var wu in p.CompatibleProducts.Select(rp => rp.web_unique).Distinct())
                    //    {
                    //        cmd.Parameters[1].Value = wu;
                    //        cmd.ExecuteNonQuery();
                    //    }
                    //}

                    tr.Commit();
                }
                catch (Exception)
                {
                    if (tr != null)
                        tr.Rollback();
                    throw;
                }
                finally
                {
                    tr = null;

                }
            
        }

        public void UpdateProductInfo(Web_product_new p)
        {
            if (p.ProductInfo != null)
            {
                foreach (var pi in p.ProductInfo)
                {
                    if (pi.id <= 0)
                    {
                        pi.web_unique = p.web_unique;
                        webProductInfoDal.Create(pi);
                    }
                    else
                    {
                        webProductInfoDal.Update(pi);
                    }
                }
                if (p.ProductInfo.Count > 0)
                    webProductInfoDal.DeleteMissing(p.web_unique, p.ProductInfo.Select(pi => pi.id).ToList());
            }
        }

        public void UpdatePartInfo(Web_product_new p)
        {
            if (p.ProductInfo != null)
            {
                foreach (var pi in p.ProductInfo)
                {
                    if (pi.id <= 0)
                    {
                        pi.web_unique = p.web_unique;
                        webProductInfoDal.Create(pi);
                    }
                    else
                    {
                        webProductInfoDal.Update(pi);
                    }
                }
                if (p.ProductInfo.Count > 0)
                    webProductInfoDal.DeleteMissing(p.web_unique, p.ProductInfo.Select(pi => pi.id).ToList());
            }
        }

        public void UpdateProductFlow(Web_product_new p)
        {
            if (p.ProductFlows != null)
            {
                foreach (var pi in p.ProductFlows)
                {
                    if (pi.id <= 0)
                    {
                        pi.web_unique = p.web_unique;
                        webProductFlowDal.Create(pi);
                    }
                    else
                    {
                        webProductFlowDal.Update(pi);
                    }
                }
                if (p.ProductFlows.Count > 0)
                    webProductFlowDal.DeleteMissing(p.web_unique, p.ProductFlows.Select(pi => pi.id).ToList());
            }
        }

        private string GetTranslationFields(bool productOnly = true)
        {
            return @"web_product_new_translate.web_unique,
                    web_product_new_translate.language_id,
                    web_product_new_translate.web_name AS web_name_t,
                    web_product_new_translate.web_description AS web_description_t,
                    web_product_new_translate.web_code AS web_code_t,
                    web_product_new_translate.web_pic_notes AS web_pic_notes_t,
                    web_product_new_translate.web_details AS web_details_t,
                    web_product_new_translate.tech_finishes AS tech_finishes_t,
                    web_product_new_translate.tech_product_type AS tech_product_type_t,
                    web_product_new_translate.tech_construction AS tech_construction_t,
                    web_product_new_translate.tech_material AS tech_material_t,
                    web_product_new_translate.tech_basin_size AS tech_basin_size_t,
                    web_product_new_translate.tech_overall_height AS tech_overall_height_t,
                    web_product_new_translate.tech_tap_holes AS tech_tap_holes_t,
                    web_product_new_translate.tech_fixing AS tech_fixing_t,
                    web_product_new_translate.gold_code AS gold_code_t,
                    web_product_new_translate.combination_comments AS combination_comments_t,
                    web_product_new_translate.tech_water_volume_note AS tech_water_volume_note_t,
                    web_product_new_translate.option_name AS option_name_t";
        }

        private string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true, bool componentLocalization = false)
        {
            List<string> fields = new List<string>();
            string join = string.Empty;
            if (localize)
            {
                fields.Add(GetTranslationFields(false));
                join = GetTranslationJoin();
            }

            //if (localize && componentLocalization)
            //{
            //    for (int i = 0; i < Properties.Settings.Default.MaxComponents; i++)
            //    {
            //        fields.Add(string.Format("cpt_{0}.cprod_name AS comp{0}_name_t", i + 1));
            //        join += string.Format(" LEFT OUTER JOIN cust_products_translate cpt_{0} ON (web_products2.web_component{0} = cpt_{0}.cprod_id AND cpt_{0}.lang = @lang)", i + 1);
            //    }
            //}

            return string.Format(initialSql, (commaBeforeFields && fields.Count > 0 ? ", " : "") + string.Join(",", fields.ToArray()) + (commaAfterFields && fields.Count > 0 ? "," : ""), join);
        }

        public List<Web_product_new> GetForFileType(int web_site_id,int webprod_file_type_id, int? language_id = null)
        {
            var sql = GetSelect(@"SELECT web_product_new.* {0} 
                                                FROM web_product_file INNER JOIN web_product_new ON web_product_file.web_unique = web_product_new.web_unique {1}
                                                WHERE web_product_new.web_site_id = @site_id AND web_product_file.file_type = @file_type",
                                            language_id != null,commaBeforeFields:true, commaAfterFields: false);
            var results = conn.Query<Web_product_new>(sql,
		        new {site_id = web_site_id, file_type = webprod_file_type_id, lang = language_id}).ToList();
			foreach(var r in results)
            {
                r.Components = webProductComponentDal.GetForProduct(r.web_unique,language_id: language_id);
            }

	        return results;

        }

        //public static WebProduct ToLegacy(Web_product_new p,List<Web_product_file_type> allTypes = null,List<Web_product_file_type> types=null, List<File_type> fileTypes=null, List<Web_product_pressure> pressures = null, List<Web_product_part> parts = null,List<Web_site> sites = null, List<Web_category> categories = null  )
        //{
        //    var result = new WebProduct();
        //    if (types == null)
        //        types = allTypes != null ?  Web_product_file_typeDAL.GetForSite(p.web_site_id,allTypes,fileTypes) : Web_product_file_typeDAL.GetForSite(p.web_site_id);
        //    if(pressures == null)
        //        pressures = Web_product_pressureDAL.GetAll();
        //    if (parts == null)
        //        parts = Web_product_partDAL.GetAll();
        //    if (sites == null)
        //        sites = Web_siteDAL.GetAll();
        //    if(categories == null)
        //        categories = Web_categoryDAL.GetAll();

        //    var site = sites.FirstOrDefault(s => s.id == p.web_site_id);
        //    if(site != null)
        //        result.web_site = site.code;
        //    result.web_description = p.web_description;
        //    result.web_code = p.web_code;

        //    result.hi_res = p.hi_res;
        //    result.web_pic_notes = p.web_pic_notes;
        //    result.web_details = p.web_details;
        //    result.web_seq = p.web_seq;
        //    result.product_weight = p.product_weight;
        //    result.bath_volume = p.bath_volume;
        //    result.tech_finishes = p.tech_finishes;
        //    result.tech_product_type = p.tech_product_type;
        //    result.tech_construction = p.tech_construction;
        //    result.tech_material = p.tech_material;
        //    result.tech_basin_size = p.tech_basin_size;
        //    result.tech_overall_height = p.tech_overall_height;
        //    result.tech_tap_holes = p.tech_tap_holes;
        //    result.tech_fixing = p.tech_fixing;
        //    result.web_auto = p.web_auto;
        //    result.guarantee = p.guarantee;
        //    result.brand_group = p.brand_group;
        //    result.gold_code = p.gold_code;
        //    result.web_status = p.web_status;
        //    result.overflow_class = p.overflow_class;
        //    result.overflow_rate = p.overflow_rate;
        //    result.combination_comments = p.combination_comments;
        //    result.tech_water_volume_note = p.tech_water_volume_note;
        //    result.image_gallery = p.image_gallery;
        //    result.parent_id = p.parent_id;
        //    result.option_name = p.option_name;
        //    result.override_length = p.override_length;
        //    result.override_width = p.override_width;
        //    result.override_height = p.override_height;
        //    result.sale_on = p.sale_on;

        //    //Categories
        //    var cat = p.SelectedCategories.FirstOrDefault();
        //    while (cat != null)
        //    {
        //        var level = cat.path.Split(':').Length;
        //        if (level == 1)
        //            //subcat
        //        {
        //            result.web_sub_category = cat.category_id;
        //            result.brand_sub_desc = cat.name;
        //            result.brand_sub_image = cat.image;
        //            result.display_type = cat.display_type;
        //        }
        //        else if (level == 2)
        //        {
        //            result.web_sub_sub_category = cat.category_id;
        //            result.brand_sub_sub_desc = cat.name;
        //        }
        //        else if(level == 3)
        //        {
        //            result.web_sub_sub_sub_category = cat.category_id;
        //            result.brand_sub_sub_sub_desc = cat.name;
        //        }
        //        else if (level == 0)
        //        {
        //            result.web_category = cat.category_id;
        //            result.brand_cat_image = cat.image;
        //            result.category_name = cat.name;
        //        }
        //        cat = categories.FirstOrDefault(c => c.category_id == cat.parent_id);
        //    }


        //    result.Components = new List<Cust_products>();
        //    foreach (var c in p.Components)
        //    {
        //        for (int i = 0; i < c.qty; i++)
        //        {
        //            result.Components.Add(c.Component);
        //        }
        //    }

        //    foreach (var file in p.WebFiles)
        //    {
        //        var type = types.FirstOrDefault(t => t.id == file.file_type);
        //        if (type != null)
        //        {
        //            result.GetType().GetProperty(string.Format("web_image{0}",type.code)).SetValue(result,file.name,null);
        //        }
        //    }

        //    foreach (var flow in p.ProductFlows)
        //    {
        //        var pr = pressures.FirstOrDefault(ps => ps.id == flow.pressure_id);
        //        var part = parts.FirstOrDefault(pa => pa.id == flow.part_id);
        //        if (pr != null && part != null)
        //        {
        //            result.GetType().GetProperty(string.Format(part.LegacyField,pr.name)).SetValue(result,flow.value,null);
        //        }
        //    }

        //    var index = 1;
        //    foreach (var comp in p.ProductInfo.Where(pi=>pi.type == 1))
        //    {
        //        result.GetType().GetProperty(string.Format("tech_compliance{0}",index++)).SetValue(result,comp.value,null);

        //    }
        //    index = 1;
        //    foreach (var add in p.ProductInfo.Where(pi => pi.type == 2))
        //    {
        //        result.GetType().GetProperty(string.Format("tech_additional{0}", index++)).SetValue(result, add.value, null);

        //    }

        //    return result;

        //}

        //public static List<WebProduct> GetForCategoryLegacy(int web_site_id,int category_id, int? language_id = null)
        //{
        //    var products = GetForCategory(category_id, language_id: language_id);
        //    return
        //        ToLegacyList(web_site_id,products);
        //}

        //public static List<WebProduct> ToLegacyList(int web_site_id,List<Web_product_new> products)
        //{
        //    var types = Web_product_file_typeDAL.GetForSite(web_site_id);
        //    var parts = Web_product_partDAL.GetAll();
        //    var pressures = Web_product_pressureDAL.GetAll();
        //    var sites = Web_siteDAL.GetAll();
        //    var categories = Web_categoryDAL.GetAll();
        //    return products.Select(
        //            p =>
        //            ToLegacy(p, types: types, pressures: pressures, parts: parts, sites: sites, categories: categories))
        //                .ToList();
        //}




        public List<Web_product_new> GetCategoryForWeb(int cat)
        {
			var sql = @"SELECT *  FROM web_product_new INNER JOIN web_product_category 
				ON web_product_new.web_unique = web_product_category.web_unique  WHERE category_id = @catId";
	        var results = conn.Query<Web_product_new>(sql, new {catid = cat}).ToList();
			foreach(var p in results)
            {
                p.WebFiles = webProductFileDal.GetForProduct(p.web_unique);
                /* NE ZABORAVI PROVJERITI DALI MORAJU BITI OBA */
                p.Components = webProductComponentDal.GetForProduct(p.web_unique);
            }

	        return results;

        }


        public List<Web_product_new> GetForSite(int siteId,bool related=false, bool components = false)
        {
            var results = new List<Web_product_new>();
            var site = webSiteDal.GetById(siteId);
            if (site != null)
            {
	            results = conn.Query<Web_product_new>(@"SELECT * FROM web_product_new WHERE web_site_id = @site_id",
		            new {site_id = site.id}).ToList();
                    
                foreach (var p in results)
                {
                    p.WebFiles = webProductFileDal.GetForProduct(p.web_unique, conn);
                    p.SelectedCategories =
                        webCategoryDal.GetForProduct(p.web_unique, conn).Where(c => webCategoryDal.IsInBrand(c,site.brand_id ?? 0)).ToList();
                    if (related)
                    {
                        p.RelatedProducts = GetRelated(p.web_unique, (int)RelationType.Complementary,files:true, conn: conn);
                    }
					if (components)
						p.Components = webProductComponentDal.GetForProduct(p.web_unique, conn);
                }
            }
            return results;
        }

        public List<Web_product_new> GetForSites(List<int> ids,bool loadCats = true, string prefixText = null, bool loadSubObjects = true)
        {
            
            var sql =
                @"SELECT * FROM web_product_new WHERE (web_name LIKE @text OR @text IS NULL) AND web_site_id IN @ids";
	        var results = conn.Query<Web_product_new>(sql, new {text = prefixText + "%"}).ToList();
            if (loadSubObjects)
            {
                var sites = webSiteDal.GetAll();
                foreach (var p in results)
                {
                    p.WebFiles = webProductFileDal.GetForProduct(p.web_unique, conn);
                    p.Components = webProductComponentDal.GetForProduct(p.web_unique, conn);
                    var firstComp = p.Components.FirstOrDefault(c => c.order == 1);
                    if (firstComp != null)
                    {
                        firstComp.Component.MastProduct =
                            mastProductsDal.GetById(firstComp.Component.cprod_mast ?? 0);
                        //firstComp.Component.ProductType = Product_typeDAL.GetById(firstComp.Component.prod_type ?? 0);
                    }

                    if (p.Components != null)
                    {
                        foreach (var pro in p.Components)
                        {
                            pro.Component.ProductType = conn.QueryFirstOrDefault<Product_type>("SELECT * FROM product_type WHERE product_type_id = @id", new {id = pro.Component.product_type});
                        }
                    }
                    if (loadCats)
                    {
                        p.SelectedCategories = webCategoryDal.GetForProduct(p.web_unique, conn);
                        var newCats = new List<Web_category>();
                        foreach (var c in p.SelectedCategories)
                        {
                            foreach (var site_id in ids)
                            {
                                if (webCategoryDal.IsInBrand(c, site_id, conn))
                                {
                                    var parents = webCategoryDal.GetParents(c.category_id, conn);
                                    newCats.AddRange(parents);
                                }
                            }
                        }
                        foreach (var webCategory in newCats)
                        {
                            p.SelectedCategories.Add(webCategory);
                        }
                    }

                    p.ProductFlows = webProductFlowDal.GetForProduct(p.web_unique, conn);
                    p.ProductInfo = webProductInfoDal.GetForProduct(p.web_unique, conn);
                    p.WebSite = sites.FirstOrDefault(s => s.id == p.web_site_id);
                }

            }
            

            return results;
        }

        public List<Web_product_new> GetWebProductsByCriteria(string searchText, int? site_id = null)
        {
            var sql  = @"SELECT web_product_new.* FROM web_product_new WHERE (web_product_new.web_name LIKE @criteria OR web_product_new.web_code 
							LIKE @criteria) AND (web_product_new.web_site_id = @site_id OR @site_id IS NULL)";
	        return conn.Query<Web_product_new>(sql, new {site_id, criteria = "%" + searchText + "%"}).ToList();
        }

        public Web_product_new Copy(int web_unique, bool setParent = true,bool removeCategories = false, int? web_site_id = null)
        {
            var prod = GetByIdEx(web_unique);
            prod.web_name += " - Copy";
            if (prod.parent_id == null && setParent)
            {
                if (HasChildren(web_unique))
                    prod.parent_id = web_unique;
            }
            if (removeCategories)
                prod.SelectedCategories = null;
            if (web_site_id != null)
                prod.web_site_id = web_site_id.Value;

            CreateEx(prod);
            return prod;
        }

        public List<Web_product_new> DAMSearch(string text, int? site_id = null, string connstring = null)
        {
            var results = new List<Web_product_new>();
            //using (var conn = new MySqlConnection(connstring == null ? Properties.Settings.Default.ConnString : connstring))
            //{
            //    conn.Open();
            //    var query = "SELECT * FROM web_product_new WHERE (web_name LIKE @text OR web_code LIKE @text OR web_unique LIKE @text) {0}";
            //    query = string.Format(query, site_id != null ? " AND (web_site_id = @site_id OR @site_id IS NULL)" : "");
            //    MySqlCommand cmd = Utils.GetCommand(query, conn);
            //    cmd.Parameters.AddWithValue("@text", "%" + text + "%");
            //    if(site_id != null)
            //        cmd.Parameters.AddWithValue("@site_id", site_id);
            //    MySqlDataReader dr = cmd.ExecuteReader();
            //    while (dr.Read())
            //    {
            //        results.Add(GetFromDataReaderEx(dr));
            //    }
            //    dr.Close();
            //}
            return results;
        }

        public List<WebSiteProductCount> GetWebSiteProductCount()
        {
            
            var sql = @"SELECT web_site.id as web_site_id,web_site.name,Count(*) AS ProductCount
                                FROM  web_product_new INNER JOIN web_site ON web_product_new.web_site_id = web_site.id
                                GROUP BY web_site.id,web_site.name";
	        return conn.Query<WebSiteProductCount>(sql).ToList();
        }
        public List<SpareParts> GetSpareParts(int web_unique, IDbConnection conn = null)
        {
			var sql = @"
	                    SELECT
	                    cust_products.cprod_code1,
	                    cust_products.cprod_name,
	                    spares.hide_flag

	                    FROM
	                    web_product_component
	                    INNER JOIN spares ON web_product_component.cprod_id = spares.product_cprod
	                    INNER JOIN cust_products ON spares.spare_cprod = cust_products.cprod_id

	                    where web_unique = @web_unique

	                    group by cprod_code1
	                    order by cprod_name
	                    ";
	        return (conn ?? this.conn).Query<SpareParts>(sql, new {web_unique}).ToList();
        }

		public List<Web_product_new> GetForWhitebookTemplate(IList<int?> webUniqueIds, IList<int?> templateIds, int? wras = null)
		{
			var result = new List<Web_product_new>();
			conn.Query<Web_product_new, Web_product_file, Web_product_new>(
				$@"SELECT web_product_new.*, web_product_file.* 
					FROM web_product_new INNER JOIN web_product_file ON web_product_new.web_unique = web_product_file.web_unique
					WHERE (web_product_new.web_unique IN @webUniqueIds	OR whitebook_template_id IN @templateids)
					{(wras != null ? @" AND web_product_new.web_unique IN (SELECT web_unique FROM web_product_component INNER JOIN cust_products_extradata ON 
					web_product_component.cprod_id = cust_products_extradata.cprod_id WHERE cust_products_extradata.wras_approval between 1 and 3)" : "" )}",
				(wp, wpf) =>
				{
					var webp = result.FirstOrDefault(r => r.web_unique == wp.web_unique);
					if (webp == null)
					{
						webp = wp;
						result.Add(webp);
						webp.WebFiles = new List<Web_product_file>();
					}
					webp.WebFiles.Add(wpf);
					return webp;
				}, new { webUniqueIds, templateIds, wras });
			return result;
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM web_product_new";
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

		public Web_product_new GetByCode(string web_code)
		{
			return conn.Query<Web_product_new>("SELECT * FROM web_product_new WHERE web_code = @web_code", new { web_code }).FirstOrDefault();
		}
	}

    public class SpareParts
    {
        public string cprod_code { get; set; }
        public string cprod_name { get; set; }
        public int? hide_flag { get; set; }
    }
    public class WebSiteProductCount
    {
        public int web_site_id { get; set; }
        public string name { get; set; }
        public int? ProductCount { get; set; }
    }
}
