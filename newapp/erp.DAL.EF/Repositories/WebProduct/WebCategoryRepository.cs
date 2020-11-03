using erp.DAL.EF.Repositories;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF
{
    public class WebCategoryRepository : GenericRepository<Web_category>
    {
        public WebCategoryRepository(Model context) : base(context)
        {
        }

        public void Create(Web_category c)
        {
            using (var m = Model.CreateModel()) {
                m.Categories.Add(c);
                m.SaveChanges();
            }
        }

        public void Update(Web_category c)
        {
            using (var m = Model.CreateModel()) {
                m.Categories.Attach(c);
                m.Entry(c).State = EntityState.Modified;
                m.SaveChanges();
            }
        }

        public List<Web_category> GetForBrand(int brand_id, int? language_id = null, bool searchForProducts = false)
        {
            return GetByCriteria(c => c.brand_id == brand_id, language_id, searchForProducts);
        }

        private List<Web_category> GetByCriteria(Expression<Func<Web_category, bool>> filter, int? language_id = null, bool searchForProducts = false)
        {
            var cats = Get(filter, includeProperties: language_id != null ? "Translations" : "").ToList();
            var childrenCounts = GetQuery(filter).Select(c => new { c.category_id, Count = c.Children.Count() }).ToDictionary(c => c.category_id, c => c.Count);
            foreach (var c in cats) {
                if (childrenCounts.ContainsKey(c.category_id))
                    c.ChildCount = childrenCounts[c.category_id];
            }
            if (searchForProducts) {
                var prodCounts = GetQuery(filter).Select(c => new { c.category_id, Count = c.Products.Count() }).ToDictionary(c => c.category_id, c => c.Count);
                foreach (var c in cats) {
                    if (prodCounts.ContainsKey(c.category_id))
                        c.ProductCount = prodCounts[c.category_id];
                }
            }
            return cats;
        }

        public List<Web_category> GetChildren(int parent_id, int? language_id = null, bool searchForProducts = false, bool deep = false)
        {
            var cats = GetByCriteria(c => c.parent_id == parent_id, language_id, searchForProducts);
            if (deep) {
                var allChildren = new List<Web_category>();
                foreach (var c in cats) {
                    var children = GetChildren(c.category_id,deep: true);
                    if (children.Count > 0)
                        allChildren.AddRange(children);
                }
                cats.AddRange(allChildren);
            }
            return cats;
        }

        

        public List<Web_category> GetAllChildren(int cat_id)
        {
            return GetChildren(cat_id, deep: true);
        }

        public List<Web_category> BuildTreeForSelected(IList<int> catIds, int brand_id)
        {
            var list = GetForBrand(brand_id);


            foreach (var id in catIds) {
                var cat = GetByCriteria(c=>c.category_id == id).FirstOrDefault();
                var currentCat = cat;
                while (currentCat.parent_id != null) {
                    var parent = list.FirstOrDefault(c => c.category_id == currentCat.parent_id);
                    if (parent == null) {
                        parent = GetByCriteria(c=>c.category_id == currentCat.parent_id.Value).FirstOrDefault();

                    }
                    if (parent.ChildCount > 0 && (parent.Children == null || parent.Children.Count == 0)) {
                        parent.Children = GetChildren(parent.category_id);
                        var child = parent.Children.FirstOrDefault(c => c.category_id == currentCat.category_id);
                        if (child != null) {
                            child.Children = currentCat.Children;
                        }
                        list.AddRange((parent.Children));
                    }

                    currentCat = parent;
                }

            }
            
            return list.Where(c => c.parent_id == null).ToList();
        }

        public List<Web_category> GetParents(int cat_id)
        {
            var list = new List<Web_category>();
            bool dispose = false;
            
            var cat = GetByCriteria(c=>c.category_id == cat_id).FirstOrDefault();
            if (cat != null) {
                var parent_id = cat.parent_id;

                while (parent_id != null) {
                    var parent = GetByCriteria(c=>c.category_id == parent_id.Value).FirstOrDefault();
                    if (parent != null) {
                        list.Add(parent);
                        parent_id = parent.parent_id;
                    }
                }
            }

            
            list.Reverse();
            return list;
        }

        public bool CanCategoryBeDeleted(int id)
        {
            var result = true;
            var children = GetAllChildren(id);
            var ids = children.Select(c => c.category_id).ToList();
            ids.Add(id);
            
            result = Get(c=>ids.Contains(c.category_id) && c.Products.Count() > 0).Count() == 0;
            return result;
        }

        public void Rename(int category_id, string name)
        {
            context.Database.ExecuteSqlCommand("UPDATE web_category SET name = @p0 WHERE category_id = @p1", name, category_id);
        }

        
    }
}
