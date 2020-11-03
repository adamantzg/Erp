using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.Data.Entity;

namespace erp.DAL.EF.Repositories
{
    public class StockOrderAllocationsRepository
    {
        public static List<Order_lines> GetCOLinesByIds(int cprod_id, IList<int> ids)
        {
            using (var m = Model.CreateModel())
            {
                var excludedStatuses = new[] {"X", "Y"};
                var lines = m.OrderLines.Where(l=>l.cprod_id == cprod_id).Include(l => l.Header).Include(l => l.SOAllocations).Include("SOAllocations.StockLine.Header")
                        .Where(l => l.Header.stock_order == Order_header.StockOrderCalloff && ids.Contains(l.orderid.Value)).ToList();
                var orderids = new HashSet<int?>();
                foreach (var l in lines) {
                    foreach (var a in l.SOAllocations) {
                        if (a.StockLine.orderid != null)
                            orderids.Add(a.StockLine.orderid.Value);
                    }
                }
                //orderid, po_req_etd dictionary
                var po_etds = m.POrderLines.Include(l => l.Header).Include(l => l.OrderLine).Where(l => orderids.Contains(l.OrderLine.orderid)).
                            GroupBy(l=>l.OrderLine.orderid).Select(g => new { orderid=g.Key, g.FirstOrDefault().Header.po_req_etd }).Distinct().ToDictionary(o => o.orderid, o => o.po_req_etd);
                var po_balances = m.OrderLines.Where(l => l.cprod_id == cprod_id && orderids.Contains(l.orderid)).Include(l => l.Allocations).
                    GroupBy(l => l.orderid).Select(g => new { orderid = g.Key, g.FirstOrDefault().Header.custpo, total = g.Sum(l => l.orderqty - l.Allocations.Sum(a => a.alloc_qty)) }).ToDictionary(o => o.orderid);
                foreach (var l in lines)
                {
                    foreach (var a in l.SOAllocations)
                    {
                        if (a.StockLine != null && a.StockLine.orderid != null && po_etds.ContainsKey(a.StockLine.orderid))
                        {
                            a.StockLine.Header.po_req_etd = po_etds[a.StockLine.orderid];
                            if (po_balances.ContainsKey(a.StockLine.orderid))
                                a.StockLine.Header.Balance = po_balances[a.StockLine.orderid].total;
                        }
                    }
                }
                return lines;
            }
        }

        public static List<Order_header> GetCOOrdersByCriteria(int cprod_id, DateTime? from = null, DateTime? to = null)
        {
            using (var m = Model.CreateModel())
            {
                var excludedStatuses = new[] { "X", "Y" };
                return m.OrderLines.Include(l => l.Header)
                        .Where(l => l.Header.stock_order == Order_header.StockOrderCalloff &&
                                    l.cprod_id == cprod_id && (from == null || l.Header.req_eta >= from) &&
                                    (to == null || l.Header.req_eta <= to) && !excludedStatuses.Contains(l.Header.status) && l.orderqty > 0).ToList().
                                    Distinct(new OrderLineByHeaderComparer()).Select(l => l.Header).ToList();

            }
        }

        public static List<Order_lines> GetAvailableStockOrderLines(int cprod_id, IList<int> idsToSkip = null, DateTime? from = null, DateTime? to = null)
        {
            using (var m = Model.CreateModel()) {
                var excludedStatuses = new[] { "X", "Y" };
                var lines =  m.OrderLines.Include(l => l.Header).Include(l => l.Allocations).
                Where(l => l.Header.stock_order == 1 && l.cprod_id == cprod_id && !idsToSkip.Contains(l.linenum) && !excludedStatuses.Contains(l.Header.status)
                && (from == null || l.Header.req_eta >= from) &&
                   (to == null || l.Header.req_eta <= to)).ToList().Where(l=>l.orderqty - l.Allocations.Sum(a => a.alloc_qty) > 0).ToList();
                var orderids = lines.Select(l => l.orderid).ToList();

                var po_etds = m.POrderLines.Include(l => l.Header).Include(l => l.OrderLine).Where(l => orderids.Contains(l.OrderLine.orderid)).
                                GroupBy(l => l.OrderLine.orderid).Select(g => new { orderid = g.Key, g.FirstOrDefault().Header.po_req_etd }).Distinct().ToDictionary(o => o.orderid, o => o.po_req_etd);
                foreach (var l in lines)
                {
                    if(po_etds.ContainsKey(l.orderid))
                        l.Header.po_req_etd = po_etds[l.orderid];
                }
                return lines;
            }
        }

        public static void UpdateAllocations(IList<Stock_order_allocation> allocations)
        {
            using (var m = Model.CreateModel())
            {
                foreach (var a in allocations)
                {
                    a.date_allocation = DateTime.Now;
                    if (a.unique_link_ref == 0)
                    {
                        m.StockOrderAllocations.Add(a);
                    }
                        
                    else
                    {
                        var oldAlloc =
                            m.StockOrderAllocations.FirstOrDefault(al => al.unique_link_ref == a.unique_link_ref);
                        if (oldAlloc != null)
                        {
                            oldAlloc.alloc_qty = a.alloc_qty;
                            oldAlloc.date_allocation = a.date_allocation;
                        }
                            
                    }
                }
                m.SaveChanges();
            }
        }
    }
}
