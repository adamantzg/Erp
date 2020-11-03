using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.Data.Entity;

namespace erp.DAL.EF.Repositories
{
    public class FactoryStockOrderRepository
    {
        public static void Create(Factory_stock_order order)
        {
            using (var m = Model.CreateModel()) {
                if(order.Lines != null) {
                    foreach (var l in order.Lines) {
                        l.linedate = DateTime.Now;
                    }
                }                
                m.FactoryStockOrders.Add(order);
                m.SaveChanges();
            }
        }

        public static Factory_stock_order GetById(int id, bool includeLines = true)
        {
            using (var m = Model.CreateModel()) {
                if(includeLines)
                    return m.FactoryStockOrders.Include(o=>o.Lines).Include("Lines.MastProduct").FirstOrDefault(o => o.id == id);
                return m.FactoryStockOrders.FirstOrDefault(o => o.id == id);
            }
        }

        public static void Update(Factory_stock_order o)
        {
            using (var m = Model.CreateModel()) {
                var old = m.FactoryStockOrders.Include(fso=>fso.Lines).FirstOrDefault(fso => fso.id == o.id );
                if(old != null) {
                    old.etd = o.etd;
                    old.factory_id = o.factory_id;
                    old.po_ref = o.po_ref;
                    foreach(var newLine in o.Lines) {
                        if (newLine.id < 0) {
                            newLine.linedate = DateTime.Now;
                            old.Lines.Add(newLine);
                        }                            
                        else {
                            var oldLine = old.Lines.FirstOrDefault(l => l.id == newLine.id);
                            if (oldLine != null) {
                                oldLine.currency = newLine.currency;
                                oldLine.mast_id = newLine.mast_id;
                                oldLine.price = newLine.price;
                                oldLine.qty = newLine.qty;
                            }
                        }
                        
                    }
                    var deletedLines =
                                    old.Lines.Where(l => o.Lines == null || o.Lines.Count(i => i.id == l.id) == 0).ToList();
                    foreach (var d in deletedLines) {
                        m.Entry(d).State = EntityState.Deleted;
                    }
                }
                m.SaveChanges();
            }
        }

        public static List<Factory_stock_order_lines> GetOrderLinesWithProducts(IList<int?> mastIds)
        {
            using (var m = Model.CreateModel()) {
                return m.FactoryStockOrdersLines.Where(l => mastIds.Contains(l.mast_id)).ToList();
            }
        }

        public static List<Factory_stock_order_lines> GetOrderLinesWithStock()
        {
            using (var m = Model.CreateModel()) {
                return m.FactoryStockOrdersLines.Include(l => l.MastProduct.Factory).Include(l=>l.MastProduct.CustProducts).Where(l => l.MastProduct != null && l.MastProduct.factory_stock > 0).ToList();
            }
        }

    }
}
