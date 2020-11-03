using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using erp.DAL.EF.Repositories;
using RefactorThis.GraphDiff;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace erp.DAL.EF
{
    public class WebProductRepository : GenericRepository<Web_product_new>
    {
        public WebProductRepository(Model context) : base(context)
        {
        }

        public static List<Web_product_new> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.WebProducts.Include("WebFiles").Include("WebSite").ToList();
            }
        }

        public static List<Web_product_transfer> GetPendingWebProductTransfers()
        {
            using (var m = Model.CreateModel())
            {
                return m.WebProductTransfers.Where(p=>p.web_unique_new == null).ToList();
            }
        }

        public static void UpdateWebProductTransfer(Web_product_transfer t)
        {
            using (var m = Model.CreateModel())
            {
                m.WebProductTransfers.Attach(t);
                m.Entry(t).State = EntityState.Modified;
                m.SaveChanges();
            }
        }

        public override void Update(Web_product_new p)
        {
            var oldP = Get(wp => wp.web_unique == p.web_unique, includeProperties: "RelatedProducts").FirstOrDefault();
            var newProducts = p.RelatedProducts != null ? p.RelatedProducts.Where(r => oldP.RelatedProducts.Count(rp => rp.web_unique == r.web_unique) == 0).ToList() : new List<Web_product_new>();
            if(newProducts.Count == 0) {
                var obj = context.UpdateGraph(p, m => m.OwnedCollection(prod => prod.WebFiles)
                                       .OwnedCollection(prod => prod.ProductFlows)
                                       .OwnedCollection(prod => prod.ProductInfo)
                                       .OwnedCollection(prod => prod.Components)
                                       .AssociatedCollection(prod => prod.SelectedCategories)
                                       .AssociatedCollection(prod => prod.RelatedProducts));
            }
            else {
                var obj = context.UpdateGraph(p, m => m.OwnedCollection(prod => prod.WebFiles)
                                       .OwnedCollection(prod => prod.ProductFlows)
                                       .OwnedCollection(prod => prod.ProductInfo)
                                       .OwnedCollection(prod => prod.Components)
                                       .AssociatedCollection(prod => prod.SelectedCategories)
                                       );
               var removed = oldP.RelatedProducts.Where(r => p.RelatedProducts.Count(nr => nr.web_unique == r.web_unique) == 0).ToList();
               foreach(var np in newProducts) {
                    if (context.Entry(np).State == EntityState.Detached)
                        dbSet.Attach(np);
                    oldP.RelatedProducts.Add(np);
               }
                foreach (var r in removed)
                    oldP.RelatedProducts.Remove(r);
            }            
                                       
        }

        public override void Insert(Web_product_new p)
        {
            p.web_unique = GetQuery().Max(wp => wp.web_unique) + 1;
            if(p.SelectedCategories != null && p.SelectedCategories.Count > 0) {
                foreach (var c in p.SelectedCategories)
                    context.Set<Web_category>().Attach(c);
            }
            if (p.RelatedProducts != null && p.RelatedProducts.Count > 0) {
                foreach (var pr in p.RelatedProducts)
                    dbSet.Attach(pr);
            }

            base.Insert(p);
        }

        public List<Web_product_new> Search(string text, out int totalCount, int? site_id = null, int? page = null, int? pageSize = null, bool files = false, List<Search_word> words = null, bool useFullText = true, int? catid = null)
        {
            var result = new List<Web_product_new>();
            var synonims = new List<string>();
            text = text.Trim();
            if (!string.IsNullOrEmpty(text)) {
                text = useFullText ? text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "") : text;
                synonims.Add(string.Join(" ", text.Split(' ').Select(s => "+" + s)));
                if (words != null) {
                    var word = words.FirstOrDefault(w => w.word.ToLower() == text.ToLower());
                    if (word != null)
                        synonims = words.Where(w => w.group_id == word.group_id).Select(w => string.Format("({0})", string.Join(" ", w.word.Split(' ').Select(s => "+" + s)))).ToList();

                    //Handle individual words
                    var wordParts = text.Split(' ');
                    if (wordParts.Length > 1) {
                        var synonim = synonims[0]; //modify original combination
                        foreach (string ws in wordParts) {
                            word = words.FirstOrDefault(w => w.word.ToLower() == ws.ToLower());
                            if (word != null) {
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

            if (catid != null && Convert.ToInt32(catid) > 0) {
                fromClause = string.Format(fromClause, " INNER JOIN web_category ON web_category.category_id = web_product_category.category_id", " AND web_category.category_id=@catid");
            }
            else {
                fromClause = string.Format(fromClause, "", "");
            }

                
            var sql = string.Format(@"SELECT DISTINCT web_product_new.*  {0}
                    {1}", fromClause, pageSize != null ? string.Format(" ORDER BY web_product_new.web_name, web_product_new.web_code LIMIT {0},{1} ", from, pageSize) : "");

            var parameters = new List<object>();
            if (catid != null && Convert.ToInt32(catid) > 0)
                parameters.Add(new MySqlParameter("@catid", catid));
            parameters.Add(new MySqlParameter("@text", useFullText ? string.Join(" ", synonims) : "%" + text + "%"));
            parameters.Add(new MySqlParameter("@web_unique", text));
            parameters.Add(new MySqlParameter("@site_id", Utilities.ToDBNull(site_id)));

            result = context.Database.SqlQuery<Web_product_new>(sql, parameters.ToArray()).ToList();
            var ids = result.Select(p => p.web_unique).ToList();
            var products = GetQuery(p => ids.Contains(p.web_unique), includeProperties: "Components.Component,SelectedCategories").AsNoTracking().ToList();
            
            /*foreach (var p in result) {
                dbSet.Attach(p);
                context.Entry(p).Collection(wp=>wp.Components).Query().AsNoTracking().Load(); 
                p.Children = null;
                if(p.Components != null)
                    foreach (var c in p.Components)
                        c.Product = null;
                //p.Category = Web_categoryDAL.GetFromDataReader(dr);
                context.Entry(p).Collection(wp=>wp.SelectedCategories).Load();
                if(p.SelectedCategories != null)
                    foreach (var c in p.SelectedCategories)
                        c.Products = null;
                if (files)
                    context.Entry(p).Collection(wp => wp.WebFiles).Load();
            }*/

            if (pageSize != null) {
                sql = "SELECT COUNT(*) " + fromClause;
                //cmd.Parameters.Clear();
                totalCount = context.Database.SqlQuery<int?>(sql).FirstOrDefault() ?? 0;
            }
            else {
                totalCount = 0;
            }

            foreach (var p in products) {
                if (p.Components != null)
                    foreach (var c in p.Components)
                        c.Product = null;
                if (p.SelectedCategories != null)
                    foreach (var c in p.SelectedCategories)
                        c.Products = null;
            }

            return products;
        }

        public Web_product_new Copy(int web_unique, bool setParent = true, bool removeCategories = false, int? web_site_id = null)
        {
            
            var prod = GetQuery(p=>p.web_unique == web_unique, includeProperties: "ProductFlows,WebFiles,ProductInfo,Components,SelectedCategories,RelatedProducts,Children").AsNoTracking().FirstOrDefault();
            prod.web_name += " - Copy";
            context.Entry(prod).State = EntityState.Detached;
            
            if (prod.parent_id == null && setParent) {
                if (prod.Children.Count > 0)
                    prod.parent_id = web_unique;
            }
            if (removeCategories)
                prod.SelectedCategories = null;
            if (web_site_id != null)
                prod.web_site_id = web_site_id.Value;

            if (prod.ProductFlows != null) {
                foreach (var f in prod.ProductFlows) {
                    f.Product = null;
                    f.web_unique = null;
                }
                    
            }
            if (prod.ProductInfo != null) {
                foreach (var f in prod.ProductInfo) {
                    f.Product = null;
                    f.web_unique = null;
                }
            }
            if (prod.WebFiles != null) {
                foreach (var f in prod.WebFiles) {
                    f.Product = null;
                    f.web_unique = null;
                }
            }

            if (prod.Components != null) {
                foreach (var f in prod.Components) {
                    f.Product = null;
                    f.web_unique = 0;
                }
            }

            if (prod.SelectedCategories != null) {
                foreach (var f in prod.SelectedCategories) {
                    f.Products = null;                    
                }
            }
            

            Insert(prod);
            context.SaveChanges();
            return prod;
        }
        public void DeleteInfoByIds(List<int> ids,int type)
        {
            var sql = $"DELETE FROM web_product_info WHERE web_unique in ( {string.Join(",", ids)}) and type={type} ";
            context.Database.ExecuteSqlCommand(sql);
        }
    }

    


}
