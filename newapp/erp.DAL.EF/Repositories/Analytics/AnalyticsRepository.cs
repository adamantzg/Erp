using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Common.EntitySql;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using company.Common;
using erp.Model;
using System.Data.Entity;
using LinqKit;

namespace erp.DAL.EF
{
    public class AnalyticsRepository
    {
        public const int DistributorForProductStats = 85;

        public static List<Order_lines> GetLinesForAnalytics(DateTime? from = null, DateTime? to = null)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.OrderLines.Include("Header")
                        .Include("Cust_Product")
                        .Include("Header.Client")
                        .Include("PorderLines")
                        .Include("PorderLines.Header")
                        .Include("Currency")
                        .Include("PorderLines.Currency")
                        .Where(l => (from == null || l.PorderLines.Any(pl => pl.Header.po_req_etd >= from)))
                        .ToList();
            }
        }

        public static List<BrandSalesByMonth> GetBrandSalesByMonth(DateTime? from = null, DateTime? to = null,
            CountryFilter countryFilter = CountryFilter.UKOnly, int[] includedClients = null,
            int[] excludedClients = null)
        {
            using (var m = Model.CreateModel())
            {
                var criteria = PredicateBuilder.True<Order_lines>();
                criteria = criteria.And(l => (l.linedate >= from || from == null) && (l.linedate <= to || to == null) && l.linedate != null);
                
                if (includedClients != null || excludedClients != null)
                {
                    criteria = includedClients != null
                        ? criteria.And(l => includedClients.Contains(l.Header.userid1 ?? 0))
                        : criteria.And(l => !excludedClients.Contains(l.Header.userid1 ?? 0));
                }
                var brands = m.Brands.ToList();
                var countryCriteria = HandleCountryCondition(countryFilter);
                if (countryCriteria != null)
                    criteria = criteria.Expand().And(countryCriteria);

                return m.OrderLines.Include("Header")
                    .Include("Header.Client")
                    .Include("Cust_Product")
                    .AsExpandable()
                    .Where(criteria).Select(l=>new {l.Cust_Product.brand_id,l.linedate,l.orderqty,l.unitcurrency,l.unitprice,l.orderid}).ToList()
                    .GroupBy(l => new {l.brand_id, l.linedate.Value.Month, l.linedate.Value.Year})
                    .Select(g => new BrandSalesByMonth
                    {
                        Month21 = (g.Key.Year - 2000)*100 + g.Key.Month,
                        brand_id = g.Key.brand_id,
                        brandname = brands.FirstOrDefault(b=>b.brand_id == g.Key.brand_id).IfNotNull(b => b.brandname),
                        Amount = g.Sum(l => l.orderqty*(l.unitcurrency == 0 ? l.unitprice/1.6 : l.unitcurrency == 2 ? l.unitprice / 1.2 : l.unitprice)),
                        numOfOrders = g.Select(l => l.orderid).Distinct().Count()
                    }).ToList();
            }
        }

        public static List<StockFactoryRow> GetUnallocatedPerFactory(IList<int> includedClientIds, Model m = null)
        {
            if (m == null)
                m = Model.CreateModel();
            var clientIds = includedClientIds.Select(id => (int?) id).ToList();
            //go from porderlines to get factory price
            return
                m.POrderLines.Include("Header.Factory")
                    .Include("OrderLine.Header")
                    .Include("OrderLine.Allocations")
                    .Where(l => l.OrderLine != null && l.OrderLine.Header != null && clientIds.Contains(l.OrderLine.Header.userid1) && l.OrderLine.Header.stock_order == 1 && l.OrderLine.Header.status != "X" && l.OrderLine.Header.status != "Y" && l.OrderLine.orderqty > 0)
                    .ToList()
                    .GroupBy(l => l.Header.userid)
                    .Select(
                        l =>
                            new StockFactoryRow
                            {
                                FactoryId = l.Key ?? 0,
                                FactoryCode = l.First().Header.Factory.factory_code,
                                Qty = Convert.ToInt32(l.Sum(pl => pl.OrderLine.orderqty - pl.OrderLine.Allocations.Sum(al=>al.alloc_qty))),
                                Value = l.Sum(pl => pl.RowPriceGBP * (pl.OrderLine.orderqty - pl.OrderLine.Allocations.Sum(al => al.alloc_qty)))
                            })
                    .ToList();
        }

        //public static List<StockFactoryRow> GetAllocatedPerFactory(IList<int> includedClientIds, DateTime? etdFrom,Model m = null)
        //{
        //    if (m == null)
        //        m = Model.CreateModel();
        //    var clientIds = includedClientIds.Select(id => (int?)id).ToList();
        //    //go from porderlines to get factory price
        //    return
        //        m.POrderLines.Include(l=>l.Header)
        //            .Include("OrderLine.Header")
        //            .Where(l => l.OrderLine != null && l.OrderLine.Header != null && clientIds.Contains(l.OrderLine.Header.userid1) && l.OrderLine.Header.stock_order == 8 && l.Header.po_req_etd > etdFrom && l.OrderLine.Header.status != "X" && l.OrderLine.Header.status != "Y" && l.OrderLine.orderqty > 0)
        //            .ToList()
        //            .GroupBy(l => l.Header.userid)
        //            .Select(
        //                l =>
        //                    new StockFactoryRow
        //                    {
        //                        FactoryId = l.Key ?? 0,
        //                        FactoryCode = l.First().Header.Factory.factory_code,
        //                        Qty = Convert.ToInt32(l.Sum(pl => pl.OrderLine.orderqty)),
        //                        Value = l.Sum(pl => pl.RowPriceGBP*pl.OrderLine.orderqty)
        //                    })
        //            .ToList();

        //}

        public static List<StockSummary> GetStockSummaryReports(IList<int> includedClients = null, DateTime? from = null)
        {
            using (var m = Model.CreateModel())
            {
                var criteria = PredicateBuilder.True<Stock_summary_report>();
                criteria = criteria.And(s => s.remaining > 0);

                if (includedClients != null)
                {
                    criteria = criteria.And(l => l.userid1 != null && includedClients.Contains(l.userid1.Value)).Expand();
                }
                var date = from ?? DateTime.Today;

                var stockSummary =  m.StockSummaryReports.AsExpandable().Where(criteria).GroupBy(s => new { s.factory_id, s.factory_code })
                    .Select(
                        g =>
                            new StockSummary
                            {
                                factory_id = g.Key.factory_id,
                                factory_code = g.Key.factory_code,
                                InProduction = g.Where(s =>s.po_req_etd > date).Sum(s => s.rowprice_gbp),
                                ReadyAtFactory = g.Where(s => s.po_req_etd <= date).Sum(s => s.rowprice_gbp)
                            }).ToList();

                
                var criteria2 = PredicateBuilder.True<Brs_sales_analysis2>();
                if (includedClients != null)
                {
                    criteria2 = criteria2.And(l =>l.userid1 != null && includedClients.Contains(l.userid1.Value)).Expand();
                }
                var onWaterSummary =
                    m.NonBrandSalesAnalysis.AsExpandable()
                        .Where(criteria2)
                        .GroupBy(s => new {s.factory_id, s.factory_code})
                        .Select(
                            g =>
                                new StockSummary
                                {
                                    factory_id = g.Key.factory_id,
                                    OnWater =
                                        g.Where(s => s.po_req_etd < date && s.req_eta > date).Sum(s => s.rowprice_gbp)
                                })
                        .ToList();
                //merge
                foreach (var o in onWaterSummary)
                {
                    var stockRecord = stockSummary.FirstOrDefault(s => s.factory_id == o.factory_id);
                    if (stockRecord != null)
                        stockRecord.OnWater = o.OnWater;
                }

                //warehouse
                var criteria3 = PredicateBuilder.True<Cust_products>();
                if (includedClients != null)
                {
                    criteria3 = criteria3.And(p => p.cprod_user != null && includedClients.Contains(p.cprod_user.Value));
                }
                var warehouseSummary = m.CustProducts.AsExpandable()
                    .Where(criteria3)
                    .GroupBy(p => p.MastProduct.factory_id)
                    .Select(
                        g =>
                            new StockSummary
                            {
                                factory_id = g.Key,
                                Warehouse =
                                    g.Sum(
                                        p => p.cprod_stock*(p.MastProduct.price_pound > 0 ? p.MastProduct.price_pound : p.MastProduct.price_dollar/1.6))
                            });
                foreach (var w in warehouseSummary)
                {
                    var stockRecord = stockSummary.FirstOrDefault(s => s.factory_id == w.factory_id);
                    if (stockRecord != null)
                        stockRecord.Warehouse = w.Warehouse;
                }

                return stockSummary;
            }
        }

        public static List<StockSummary> GetStockSummaryReports2(IList<int> includedClients, double qcCharge, double duty, double freight, DateTime? from = null)
        {
            var result = new List<StockSummary>();
            var date = from ?? DateTime.Today;
            var clientIds = includedClients.Select(id => (int?)id).ToList();
            using (var m = Model.CreateModel())
            {
                var unallocated = GetUnallocatedPerFactory(includedClients, m);
                result.AddRange(unallocated.Select(o => new StockSummary {factory_id = o.FactoryId, factory_code = o.FactoryCode, UnallocatedAtFactoryValue = o.Value}));

                //var allocated = GetAllocatedPerFactory(includedClients, date, m);
                ////merge
                //foreach (var o in allocated) {
                //    var stockRecord = result.FirstOrDefault(s => s.factory_id == o.FactoryId);
                //    if (stockRecord != null)
                //        stockRecord.AllocatedAtFactory = o.Value;
                //    else
                //        result.Add(new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, AllocatedAtFactory = o.Value });
                //}

                //onWater
                var onWater = m.POrderLines.Include("Header.Factory")
                    .Include("OrderLine.Header")
                    .Where(l => l.OrderLine != null && l.OrderLine.Header != null && clientIds.Contains(l.OrderLine.Header.userid1) && l.OrderLine.Header.stock_order != 1
                        && l.Header.po_req_etd < date && l.OrderLine.Header.req_eta > date && l.OrderLine.Header.status != "X" && l.OrderLine.Header.status != "Y" && l.OrderLine.orderqty > 0)
                    .ToList()
                    .GroupBy(l => l.Header.userid)
                    .Select(
                        l =>
                            new StockFactoryRow
                            {
                                FactoryId = l.Key ?? 0,
                                FactoryCode = l.First().Header.IfNotNull(h=>h.Factory.IfNotNull(f=>f.factory_code)),
                                Qty = Convert.ToInt32(l.Sum(pl => pl.OrderLine.IfNotNull(ol=>ol.orderqty))),
                                Value = l.Sum(pl => pl.RowPriceGBP * pl.OrderLine.IfNotNull(ol=>ol.orderqty))
                            })
                    .ToList();

                //merge
                foreach (var o in onWater) {
                    var stockRecord = result.FirstOrDefault(s => s.factory_id == o.FactoryId);
                    if (stockRecord != null)
                        stockRecord.OnWater = o.Value;
                    else
                        result.Add(new StockSummary { factory_id = o.FactoryId, factory_code = o.FactoryCode, OnWater = o.Value });
                }

                //stock uk
                var criteria3 = PredicateBuilder.True<Cust_products>();
                criteria3 = criteria3.And(p => p.MastProduct != null);
                criteria3 = criteria3.And(p => p.OrderLines.Any(l => l.Header.stock_order == 1));
                if (includedClients != null) {
                    criteria3 = criteria3.And(p => p.cprod_user != null && includedClients.Contains(p.cprod_user.Value));
                }
                var warehouseSummary = m.CustProducts.Include("MastProduct.Factory").AsExpandable()
                    .Where(criteria3)
                    .ToList()
                    .GroupBy(p => p.MastProduct.factory_id)
                    .Select(
                        g =>
                            new StockSummary
                            {
                                factory_id = g.Key,
                                factory_code = g.First().MastProduct.Factory.factory_code,
                                Warehouse =
                                    g.Sum(
                                        p => p.cprod_stock * (p.MastProduct.price_pound > 0 ? p.MastProduct.price_pound : p.MastProduct.price_dollar / 1.6) * ( 1+ qcCharge) * (1+duty) * (1 + freight))
                            });
                foreach (var w in warehouseSummary) {
                    var stockRecord = result.FirstOrDefault(s => s.factory_id == w.factory_id);
                    if (stockRecord != null)
                        stockRecord.Warehouse = w.Warehouse;
                    else
                    {
                        result.Add(new StockSummary { factory_id = w.factory_id, factory_code = w.factory_code, Warehouse = w.Warehouse });
                    }
                }
            }
            return result;
        }
        
        private static
            Expression<Func<Order_lines,bool>> HandleCountryCondition(CountryFilter filter)
        {
            var ukCountries = new[] {"GB", "IE"};
            if (filter == CountryFilter.All)
                return null;
            if (filter == CountryFilter.UKOnly)
                return l => ukCountries.Contains(l.Header.Client.user_country);
            return l => !ukCountries.Contains(l.Header.Client.user_country);
        }




        //public static List<SalesByMonth> GetBrandSalesByMonth(DateTime? from, DateTime? to,
        //    CountryFilter countryFilter, bool useBrands, int[] includedClients, int[] excludedClients,
        //    int? brand_id)
        //{
        //    using (var m = Model.CreateModel())
        //    {
        //        var criteria = PredicateBuilder.True<Porder_lines>();

                
        //        if (includedClients != null || excludedClients != null)
        //        {
        //            criteria = includedClients != null
        //                ? criteria.And(l => includedClients.Contains(l.OrderLine.Header.userid1 ?? 0))
        //                : criteria.And(l => !excludedClients.Contains(l.OrderLine.Header.userid1 ?? 0));
        //        }

        //        m.POrderLines.Include("OrderLine").Include("OrderLine.CustProduct").Include("Header").Include("OrderLine.Header")
        //    }
        //}

        public static List<ProductStats> GetProductStats(int ageForExclusion = 6)
        {
            var result = new List<ProductStats>();
            using (var m = Model.CreateModel())
            {
                var brands = m.Brands.Where(b => b.eb_brand == 1).ToList();
                var criteria = PredicateBuilder.True<Cust_products>();
                var brands_id = m.Brands.Where(b => b.eb_brand == 1).Select(b => b.user_id).ToList();
                //foreach (var b in brands)
                //{
                //    criteria = criteria.Or(p => p.brand_userid == b.user_id);
                //}
                var exclusionThreshold = DateTime.Today.AddMonths(-1*ageForExclusion);
                var excludedCats = new int?[]{115,207,307,608,708,912,8081};
                var criteriaMain = criteria.And(p => p.MastProduct.category1 != 13 && p.cprod_status != "D" && p.MastProduct.product_group != null
                                                     && (p.report_exception == null || p.report_exception == 0) && !excludedCats.Contains(p.cprod_brand_cat)
                                                     && p.DistProducts.Any(d=>d.client_id == DistributorForProductStats) /*&& 
                                                     p.OrderLines.Any(l=>l.PorderLines.Max(pol=>pol.Header.po_req_etd) > exclusionThreshold)*/ );
                var criteriaOther = criteria.And(p => p.cprod_status != "D" && brands_id.Contains(p.brand_userid));
                //criteria = criteria.And(baseCriteria);

                var products =
                    m.CustProducts.Include("MastProduct").Include("OrderLines.PorderLines.Header").AsExpandable()
                        .Where(criteriaMain)
                        .Select(p => new ProductRow {cprod_code1 = p.cprod_code1.Trim(),cprod_status = p.cprod_status,
                                            brand_userid = p.brand_userid,product_group = p.MastProduct.product_group,po_req_etd = p.OrderLines.Max(l=>l.PorderLines.Max(pol=>pol.Header.po_req_etd) )})
                        .ToList().Where(p => brands_id.Contains(p.brand_userid) && p.po_req_etd > exclusionThreshold).ToList();
                var otherProducts =
                        m.CustProducts.AsExpandable().Where(criteriaOther)
                            .Select(op => new ProductRow { cprod_code1 = op.cprod_code1.Trim(), brand_userid = op.brand_userid })
                            .ToList();

                var comparer = new CustProductCodeDistinctComparer();

                result = products.Where(
                    p =>
                        !otherProducts.Any(op=>op.cprod_code1 == p.cprod_code1 && op.brand_userid != p.brand_userid))
                    .GroupBy(p => new {p.brand_userid, p.product_group}).Select(p => new ProductStats
                    {
                        brand_userid = p.Key.brand_userid,
                        brandname = brands.FirstOrDefault(b => b.user_id == p.Key.brand_userid).brandname,
                        product_group = p.Key.product_group,
                        numOfProducts = p.Distinct(comparer).Count()
                    }).ToList();

                
                result.AddRange(products.Where(
                    p =>
                        otherProducts.Any(
                            op =>
                                op.cprod_code1 == p.cprod_code1 && op.brand_userid != p.brand_userid))
                    .GroupBy(p => new {p.product_group }).Select(p => new ProductStats
                    {
                        brand_userid = null,
                        brandname = "Universal",
                        product_group = p.Key.product_group,
                        numOfProducts = p.Count()
                    }));
                return result;
            }

        }

        public static List<OrderLineSimple> GetOrderLinesSimpleForBrand(int brand_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.BrandSalesAnalysisProduct3.Where(p => p.brand_id == brand_id).
                    Select(p => new OrderLineSimple {LineDate = p.linedate,cprod_id = p.cprod_id,OrderQty = p.orderqty,PriceGBP = p.sogbp}).ToList();
            }
        }

        public static List<ReturnsSimple> GetReturnsSimpleForBrand(int brand_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.Returns.Where(r => r.Product != null && r.Product.brand_id == brand_id && r.decision_final == 1).
                    Select(r=>new ReturnsSimple{ClaimType = r.claim_type,ClaimValue = r.claim_value,cprod_id = r.cprod_id,CreditValue = r.credit_value,RequestDate = r.request_date,ReturnQty = r.return_qty}).ToList();
            }
        }
    }

    public class CustProductCodeDistinctComparer : IEqualityComparer<ProductRow>
    {
        public bool Equals(ProductRow x, ProductRow y)
        {
            return x.cprod_code1 == y.cprod_code1;
        }

        public int GetHashCode(ProductRow obj)
        {
            return obj.cprod_id.GetHashCode();
        }
    }

    public class ProductRow
    {
        public int cprod_id { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_status { get; set; }
        public int? brand_userid { get; set; }
        public string product_group { get; set; }
        public DateTime? po_req_etd { get; set; }
    }
}
