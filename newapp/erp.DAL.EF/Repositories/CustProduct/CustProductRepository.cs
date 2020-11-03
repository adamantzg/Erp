using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using company.Common;
using erp.DAL.EF.Repositories;
using erp.Model;
using RefactorThis.GraphDiff;
using System.Data.Entity;
using LinqKit;

namespace erp.DAL.EF
{
    public class CustProductRepository : GenericRepository<Cust_products>
    {
        public CustProductRepository(Model context) : base(context)
        {
            
        }

        public static List<Cust_products> SearchProducts(int? factory_id, int? client_id, bool includeTemplates = false)
        {
            using (var m = Model.CreateModel())
            {
                var prods =  includeTemplates ? m.CustProducts.Include("MastProduct").Include("Inspv2Templates")
                        .Where(
                            p =>
                                (p.cprod_user == client_id || client_id == null) &&
                                (p.MastProduct.factory_id == factory_id || factory_id == null))
                        .ToList() :
                    m.CustProducts.Include("MastProduct")
                        .Where(
                            p =>
                                (p.cprod_user == client_id || client_id == null) &&
                                (p.MastProduct.factory_id == factory_id || factory_id == null))
                        .ToList();
                return prods;
            }
        }

        public static List<Cust_products> SearchProducts(int? factory_id = null, IList<int> clientIds = null )
        {
            using (var m = Model.CreateModel())
            {
                var predicate = PredicateBuilder.True<Cust_products>();
                if (factory_id != null)
                    predicate = predicate.And(p => p.MastProduct.factory_id == factory_id);
                if (clientIds != null)
                    predicate = predicate.And(p => clientIds.Any(c=>c == p.cprod_user));
                return m.CustProducts.Include("MastProduct").AsExpandable().Where(predicate).ToList();
                //        .Where(
                //            p =>
                //                (clientIds.Contains(p.cprod_user ?? 0) || client_id == null) &&
                //                (p.MastProduct.factory_id == factory_id || factory_id == null))
                //        .ToList();

            }
        }

        public static Cust_products GetById(int cprod_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.CustProducts.Include("MastProduct").FirstOrDefault(p => p.cprod_id == cprod_id);
            }
        }

        public static List<Cust_products> GetByMastId(int mast_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.CustProducts.Where(p => p.cprod_mast == mast_id && p.cprod_status != "D").ToList();
            }
        }

        public static List<Cust_products> GetForLVHStockMonitoring()
        {
            using (var m = Model.CreateModel())
            {
                return m.CustProducts.Where(p => p.stock_check == 1 && p.cprod_stock == 0).ToList();
            }
        }

        public static void UpdateProductsForLVHStockMonitoring()
        {
            using (var m = Model.CreateModel())
            {
                var prods = m.CustProducts.Where(p => p.stock_check == 1 && p.cprod_stock == 0).ToList();
                if (prods != null)
                {
                    foreach (var prod in prods)
                    {
                        prod.stock_check = 0;
                    }

                    m.SaveChanges();
                }

            }
        }

        public static List<Cust_products> GetByBrandId(int brand_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.CustProducts.Include("MastProduct").Include("Category").Where(p => p.brand_id == brand_id).ToList();
            }
        }

        

        public static List<BrandCategory> GetBrandCategoriesByCriteria(int brand_user_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.BrandCategories.Include("Group").Where(c => c.brand == brand_user_id).ToList();
            }
        }

        public static List<Brand_categories_group> GetBrandCategoriesGroups()
        {
            using (var m = Model.CreateModel())
            {
                return m.BrandCategoryGroups.ToList();
            }
        }
        
        public static void UpdateDiscontinuation(int cprod_id, bool flag)
        {
            using (var m = Model.CreateModel()) {
                var prod = m.CustProducts.FirstOrDefault(p => p.cprod_id == cprod_id);
                if (prod != null) {
                    prod.proposed_discontinuation = flag;
                    m.SaveChanges();
                }           
            }
        }

        public List<cust_product_file_type> GetFileTypes()
        {
            return context.Set<cust_product_file_type>().ToList();
        }

        public List<Cust_products> GetForIds(List<int> ids)
        {
            var products = Get(p => ids.Contains(p.cprod_id), includeProperties: "MastProduct.Factory, Color").ToList();
            foreach(var p in products)
                p.cprod_stock_codes = new[] { p.cprod_stock_code ?? 0 }.ToList();
            return products;
        }


        public List<OverstockReportRow> GetOverstockList(IList<int?> cprod_userIds, DateTime? forDate = null)
        {
            var date = forDate ?? DateTime.Today;
            var month21From = new Month21(date.AddMonths(1));
            var month21To = (month21From + 11);
            return
                context.Database.SqlQuery<OverstockReportRow>(
                    $@"SELECT cust_products.cprod_id,cust_products.cprod_code1,cust_products.cprod_name, cust_products.cprod_stock, mast_products.factory_ref, factory.factory_code,
                        SUM(sales_forecast.sales_qty) AS forecast_qty, (SELECT SUM(orderqty) FROM om_detail1 WHERE po_req_etd > @p0 AND cprod_id = cust_products.cprod_id) AS on_order_qty
                        FROM
                        cust_products
                        INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                        INNER JOIN users factory ON factory.user_id = mast_products.factory_id
                        INNER JOIN sales_forecast ON cust_products.cprod_id = sales_forecast.cprod_id
                        WHERE sales_forecast.month21 BETWEEN @p1 AND @p2 AND cust_products.cprod_user IN ({string.Join(",", cprod_userIds)})
                        AND (SELECT SUM(orderqty) FROM om_detail1 WHERE po_req_etd > @p0 AND cprod_id = cust_products.cprod_id) > 0
                        GROUP BY cust_products.cprod_id
                        HAVING  cust_products.cprod_stock > SUM(sales_forecast.sales_qty)",date,month21From.Value, month21To.Value).ToList();
        }

		public override void Update(Cust_products entityToUpdate)
		{
			context.UpdateGraph(entityToUpdate, m => m.OwnedCollection(p => p.MarketData).OwnedCollection(p => p.SalesForecast).
			OwnedEntity(p => p.MastProduct));
			
			if (entityToUpdate.MastProduct != null)
			{
				var mp = context.Set<Mast_products>().Include("Prices").Include("ProductPricingData").Where(p => p.mast_id == entityToUpdate.MastProduct.mast_id).FirstOrDefault();
				if(entityToUpdate.MastProduct.Prices != null)
				{
					foreach (var mpp in entityToUpdate.MastProduct.Prices)
					{
						var oldMp = mp.Prices.FirstOrDefault(omp => omp.currency_id == mpp.currency_id);
						if (oldMp == null)
							mp.Prices.Add(mpp);
						else
							context.Entry(oldMp).CurrentValues.SetValues(mpp);
					}
					var toBeRemoved = mp.Prices.Where(oldMp => entityToUpdate.MastProduct.Prices.Count(newMp => newMp.id == oldMp.id) == 0).ToList();
					foreach (var oldMp in toBeRemoved)
					{
						context.Entry(oldMp).State = EntityState.Deleted;
					}
				}
				if(entityToUpdate.MastProduct.ProductPricingData != null)
				{
					if (mp.ProductPricingData == null)
						mp.ProductPricingData = entityToUpdate.MastProduct.ProductPricingData;
					else
						context.Entry(mp.ProductPricingData).CurrentValues.SetValues(entityToUpdate.MastProduct.ProductPricingData);
				}
				
			}
		}

		public override void Insert(Cust_products entity)
		{
			if(entity.Projects != null)
			{
				var ids = entity.Projects.Select(p => p.id);
				entity.Projects = context.Set<ProductPricingProject>().Where(p => ids.Contains(p.id)).ToList();
			}

			base.Insert(entity);
		}

	}
}
