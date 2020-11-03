using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.Data.Entity;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.Repositories
{
    public class MastProductRepository : GenericRepository<Mast_products>
    {
        public MastProductRepository(Model context) : base(context)
        {
            
        }

        public static List<Mast_products> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.MastProducts.ToList();
            }
        }

        public static Mast_products GetById(int? mast_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.MastProducts.FirstOrDefault(mp => mp.mast_id == mast_id);
            }
        }

        public static List<Mast_products> GetByCriteria(int subcat_id )
        {
            using (var m = Model.CreateModel())
            {
                return m.MastProducts.Where(mp => mp.category1_sub == subcat_id).ToList();
            }
        }

        public static List<Category1_sub> GetSubcategories()
        {
            using (var m = Model.CreateModel())
            {
                return m.Subcategories.ToList();
            }
        }

        
        public static Category1_sub GetSubCategory(int subcat_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.Subcategories.FirstOrDefault(s => s.cat2_code == subcat_id);
            }
        }

        public static List<Mast_products> GetProductsWithFactoryStock(int factory_id)
        {
            using (var m = Model.CreateModel()) {
                return m.MastProducts.Where(mp => mp.factory_id == factory_id && mp.factory_stock > 0).ToList();
            }
        }

        public static List<Mast_products> GetByCriteria(int factory_id, int? category1_id, int client_id)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.MastProducts.Include(mp => mp.CustProducts)
                        .Where(
                            mp =>
                                mp.factory_id == factory_id && (category1_id == null || mp.category1 == category1_id) &&
                                mp.CustProducts.Any(cp => cp.brand_userid == client_id))
                        .ToList();
            }
        }

        public override void Update(Mast_products product)
        {
            //base.Update(product);
            //spares
            //if(product.CustProducts != null) {
            //    foreach (var p in product.CustProducts) {
            //        if (p.Parents != null) {
            //            foreach (var prod in p.Parents) {
            //                if (prod.cprod_id <= 0)
            //                    context.Entry(prod).State = EntityState.Added;
            //            }
            //        }
            //    }
            //}
            context.UpdateGraph(product, map => map.OwnedCollection(mp => mp.TechnicalProductData).OwnedCollection(mp=>mp.CustProducts));
            
            
        }

        public List<mast_product_file_type> GetFileTypes()
        {
            return context.Set<mast_product_file_type>().ToList();
        }

        //public List<Order_lines> GetOrderLines(int mastId)
        //{
        //    return
        //        context.OrderLines.Include(l => l.Header)
        //            .Include(l => l.PorderLines)
        //            .Include(l => "InspectionV2Lines.Inspection")
        //            .Where(l => l.Cust_Product.cprod_mast == mastId)
        //            .ToList();
        //}
    }
}
