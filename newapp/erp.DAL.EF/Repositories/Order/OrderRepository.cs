using System;
using System.Collections.Generic;
using System.Linq;
using erp.Model;
using System.Data.Entity;
using LinqKit;

namespace erp.DAL.EF.Repositories.Order
{
	public class OrderRepository : GenericRepository<Order_header>, IOrderRepository
    {
        public OrderRepository(Model context) : base(context)
        {
        }

        public OrderRepository() : base(new Model())
        {

        }

        public Order_header GetById(int id)
        {
            using (var m = Model.CreateModel())
            {
                return m.Orders.Include("Lines").Include("Lines.Cust_Product").Include("Client").FirstOrDefault(o => o.orderid == id);
            }
        }

        public Dictionary<int, List<Order_lines>> GetLinesInPeriod(IEnumerable<int> cprod_ids,DateTime? from = null, DateTime? to = null,
                                                          int? company_id = null)
        {
            using (var m = Model.CreateModel())
            {
                var result = new Dictionary<int, List<Order_lines>>();
                if (cprod_ids != null)
                {
                    foreach (var cprodId in cprod_ids)
                    {
                        result[cprodId] = m.OrderLines.Include("Cust_Product")
                                      .Where(
                                          l => l.cprod_id == cprodId && 
                                          (l.Header.orderdate >= from || from == null) &&
                                          (l.Header.orderdate <= to || to == null) &&
                                          (l.Header.userid1 == company_id || company_id == null)).ToList();
                    }
                    return result;
                }
                return null;
            }
        }

        public  List<Order_line_Summary> GetBrandSummaryForPeriod(DateTime? from, DateTime? to, int? brand_user_id = null,
            string cprod_code = null, bool brands_only = false)
        {
            using (var m = Model.CreateModel())
            {
                var brands = m.Brands.Where(b => b.eb_brand == 1).Select(b=>b.user_id).ToArray();
                var where = PredicateBuilder.True<Order_lines>();
                if (cprod_code != null)
                    where = where.And(l => l.Cust_Product.cprod_code1 == cprod_code);
                where = where.And(l => l.Header.Client.distributor > 0 && l.Header.Client.hide_1 == 0
                               && (from == null || l.Header.req_eta >= from) &&
                               (to == null || l.Header.req_eta <= to)
                               && (l.Cust_Product.brand_userid == brand_user_id || brand_user_id == null) );
                return 
                    m.OrderLines.Include("Header")
                        .Include("CustProduct")
                        .Include("Header.Client").AsExpandable()
                        .Where(where)
                        .GroupBy(l => l.Cust_Product.brand_userid)
                        .Select(g => new Order_line_Summary
                        {
                            id = g.Key,
                            total = g.Sum(l => l.orderqty*l.unitprice*(l.unitcurrency == 0 ? 0.625 : 1)) ?? 0,
                            sum_order_qty = (int?) g.Sum(l => l.orderqty) ?? 0
                        }).ToList().Where(l=>(!brands_only || brands.Contains(l.id))).ToList();
            }   
        }

        //public static List<Order_line_Summary> GetOrdersAfterStockDate(int? brand_id = null, string customerCode = null)
        //{
        //    using (var m = Model.CreateModel())
        //    {
        //        return m.OrderLines.Include("Header").Include("Header.Client").Include("CustProduct").
        //            Where(
        //                l =>
        //                    (l.Cust_Product.brand_id == brand_id || brand_id == null) &&
        //                    (l.Header.Client.customer_code == customerCode || customerCode == null ||
        //                     customerCode == string.Empty)
        //                    && l.Header.req_eta > l.Cust_Product.cprod_stock_date).
        //            GroupBy(l => l.cprod_id).
        //            Select(g => new Order_line_Summary
        //            {
        //                id=g.Key,
        //                sum_order_qty =(int?) g.Sum(l=>l.orderqty)
        //            }).ToList();
        //    }
        //}

        public  List<Order_lines> GetLinesInPeriodETD(DateTime? from = null, DateTime? to = null, int? stock_order = null)
        {
            using (var m = Model.CreateModel())
            {

                return m.POrderLines.Include("Header").Include("OrderLine").Include("OrderLine.Header").Include("OrderLine.Cust_Product")
                    .Include("OrderLine.Cust_Product.MastProduct").Where(l=>l.OrderLine.Header.status != "X" && l.OrderLine.Header.status != "Y" && (l.OrderLine.Header.stock_order == stock_order || stock_order == null)
                        && l.OrderLine.Cust_Product.cprod_status != "Z" && (l.Header.po_req_etd >= from || from == null) && (l.Header.po_req_etd <= to || to == null))
                    .ToList().Select(l => l.OrderLine).Distinct(new OrderLineComparer()).ToList();
                
            }
        }

        public  List<Order_lines> GetLinesForOrdersAnalysis(DateTime from, CountryFilter countryFilter = CountryFilter.UKOnly, int? client_id = null)
        {
            string[] countries = { "GB", "IE" };
            string[] statuses = { "X", "Y" };
            using (var m = Model.CreateModel())
            {
                //var orders = m.Orders.Include("Client").Include("PorderHeaders").Include("Lines").Include("Lines.Cust_Product").Include("Lines.Cust_Product.MastProduct").
                //    Where(o => !statuses.Contains(o.status) && countries.Contains(o.Client.user_country) && o.Client.distributor > 0 && o.PorderHeaders.Max(po => po.po_req_etd) > from 
                //        && o.Lines.Any(l => l.Cust_Product.MastProduct.category1 != 13)).ToList();

                var where = PredicateBuilder.True<Porder_lines>();
                where.And(l => l.OrderLine.orderqty > 0 && !statuses.Contains(l.OrderLine.Header.status)
                                && l.OrderLine.Header.Client.distributor > 0 
                                && l.Header.po_req_etd > from);
                if (countryFilter == CountryFilter.UKOnly)
                    where.And(l => countries.Contains(l.OrderLine.Header.Client.user_country));
                else
                {
                    where.And(l => !countries.Contains(l.OrderLine.Header.Client.user_country));
                }
                if (client_id != null)
                    where.And(l => l.OrderLine.Header.userid1 == client_id);

                return m.POrderLines.Include("OrderLine")
                    .Include("OrderLine.Header")
                    .Include("OrderLine.Header.Client")
                    .Include("OrderLine.Cust_Product")
                    .Include("OrderLine.Cust_Product.MastProduct")
                    .Include("OrderLine.Cust_Product.MastProduct.Factory")
                    .Include("Header").AsExpandable().Where(where)
                    .ToList().Where(l => l.OrderLine.Cust_Product.MastProduct.category1 != 13).Select(l => l.OrderLine).ToList();
                //category1 clause must be executed after query - otherwise linq returns OrderLine object as null (??)

                //lines.Dump();
                
            }
            
        }

        //public static List<OrderSummaryByLocationClientRow> GetSummaryByLocationClient(int brand_id, DateTime date, CountryFilter countryFilter = CountryFilter.UKOnly)
        //{
        //    string[] countries = { "GB", "IE" };
        //    string[] statuses = { "X", "Y" };

        //    using (var m = Model.CreateModel())
        //    {
        //        var where = PredicateBuilder.True<Porder_lines>();
        //        where.And(l =>l.OrderLine.Cust_Product.brand_id == brand_id && l.OrderLine.orderqty > 0 && !statuses.Contains(l.OrderLine.Header.status)
        //                        && l.OrderLine.Header.Client.distributor > 0);
        //        if (countryFilter == CountryFilter.UKOnly)
        //            where.And(l => countries.Contains(l.OrderLine.Header.Client.user_country));
        //        else
        //        {
        //            where.And(l => !countries.Contains(l.OrderLine.Header.Client.user_country));
        //        }

        //        return m.POrderLines.Include("OrderLine")
        //            .Include("OrderLine.Header")
        //            .Include("OrderLine.Header.Client")
        //            .Include("OrderLine.Cust_Product")
        //            .Include("OrderLine.Cust_Product.MastProduct")
        //            .Include("Header").AsExpandable().Where(where)
        //            .GroupBy(l => new
        //            {
        //                l.OrderLine.Cust_Product.MastProduct.Factory.consolidated_port,
        //                Status =
        //                    l.OrderLine.Header.req_eta < date
        //                        ? OrderDeliveryStatus.Delivered
        //                        : l.Header.po_req_etd < date && l.OrderLine.Header.req_eta > date
        //                            ? OrderDeliveryStatus.OnWater
        //                            : OrderDeliveryStatus.ToBeShipped,
        //                l.OrderLine.Header.userid1
        //            })
        //            .Select(
        //                g =>
        //                    new OrderSummaryByLocationClientRow
        //                    {
        //                        CustomerCode = g.Key.userid1,
        //                        Location = g.Key.consolidated_port,
        //                        Status = g.Key.Status,
        //                        Qty = g.Sum(l => l.OrderLine.orderqty)
        //                    }).ToList();
        //    }
        //}


        public List<Order_lines> GetOverDueLines(List<int> clientIds, Dictionary<int, int> rules, DateTime overdueFrom, DateTime overdueTo, bool limitToStock = true)
        {
            //var startDate = overdueFrom.AddMonths(-1*rules.Values.Max());
            var endDate = overdueTo.AddMonths(-1*rules.Values.Min());
            using (var m = Model.CreateModel())
            {
                return
                    m.OrderLines.Include("Header.Client")
                        .Include("Allocations")
                        .Include("Cust_Product.MastProduct")
                        .Where(l =>l.Cust_Product.cprod_status == "N" && l.Header.userid1 != null 
						&& clientIds.Contains(l.Header.userid1.Value) && l.Header.orderdate <= endDate 
						&& l.orderqty > l.Allocations.Sum(a => a.alloc_qty) && (!limitToStock || l.Header.stock_order == 1) && (l.Header.Client.test_account < 1))
                        .ToList();
                
            }
        }

        public List<Order_header> UpdateEtaCombinedOrders(DateTime dateFrom)
        {
            using (var m = Model.CreateModel())
            {
                //var orders = m.Orders.Where(o => o.req_eta >= dateFrom).ToList();
                var orders =
                    m.Orders.Include(o => o.Parent)
                        .Where(o => o.status != "X" && o.status != "Y" && o.combined_order > 0 &&  o.Parent != null  
                            && o.req_eta >= dateFrom && o.req_eta != o.Parent.req_eta && o.Parent.req_eta != null)
                        .ToList();

                foreach (var o in orders)
                {
                    o.req_eta = o.Parent.req_eta;
                }
                m.SaveChanges();
                return orders;
            }
        }

        public List<order_summary> GetOrderSummaries(IList<int> companyIds = null, IList<int> factoryIds = null)
        {
            return context.Database.SqlQuery<order_summary>($@"SELECT order_header.orderid, order_header.orderdate, order_header.custpo,
                                                       order_header.req_eta, SUM(order_lines.orderqty) AS orderqty,
                                                       SUM(order_lines.received_qty) AS received_qty
                                                       FROM `order_header` INNER JOIN order_lines ON order_header.orderid = order_lines.orderid
                                                       WHERE order_header.status NOT IN ('X','Y') AND order_lines.orderqty > 0
                                                            {(companyIds != null ? $" AND order_header.userid1 IN ({string.Join(",", companyIds)}) " : "")}
                                                       {(factoryIds != null ? $@"AND EXISTS 
                                                       (SELECT factory_id FROM mast_products INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast
                                                        WHERE cust_products.cprod_id = order_lines.cprod_id
                                                        AND factory_id IN ({string.Join(",", factoryIds)}))" : "")}                                                       
                        GROUP BY orderid, orderdate, custpo, req_eta
                        ORDER BY req_eta").ToList();
        }

        public Dictionary<int?, string> GetOrderContainerNumbers(IList<int?> orderids)
        {
            return context.Database.SqlQuery<QueryResult>($@"SELECT order_lines.orderid AS `int1`, COALESCE(GROUP_CONCAT(DISTINCT inspection_v2_container.container_no),'') AS string1
                            FROM inspection_v2_container INNER JOIN inspection_v2_line ON inspection_v2_container.insp_id = inspection_v2_line.insp_id 
                            INNER JOIN order_lines ON inspection_v2_line.orderlines_id = order_lines.linenum WHERE order_lines.orderid  IN ({string.Join(",",orderids)})
                            GROUP BY order_lines.orderid").ToDictionary(t=>t.Int1, t=>t.String1);
        }

        public void Create(Order_header o)
        {
            using (var m = Model.CreateModel())
            {
                var nextId = m.Orders.Max(or => or.orderid) + 1;
                o.orderid = nextId;
                m.Orders.Add(o);
                m.SaveChanges();
            }
        }



    }
   
}
