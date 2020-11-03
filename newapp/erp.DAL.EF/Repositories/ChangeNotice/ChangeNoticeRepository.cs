using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using erp.DAL.EF.Repositories;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF
{
    public class ChangeNoticeRepository : GenericRepository<Change_notice>
    {
        public ChangeNoticeRepository(DbContext context) : base(context)
        {
        }

        public static List<Change_notice> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.ChangeNotices.ToList();
            }
        }

        public static Change_notice GetById(int id)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.ChangeNotices.Include("Allocations")
                        .Include("Allocations.Product")
                        .FirstOrDefault(n => n.id == id);
            }
        }

        public static List<Change_notice> GetByCprodIdAndDate(int id, DateTime date)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.ChangeNotices.Include("Allocations")
                        .Where(n=>n.Allocations.Any(a=>a.cprod_id == id) && n.datecreated >= date).ToList();
            }
        }

        public static void Create(Change_notice n)
        {
            using (var model = Model.CreateModel())
            {
                n.datecreated = DateTime.Now;

                if (n.Allocations != null)
                {
                    foreach (var a in n.Allocations)
                    {
                        a.dateAllocated = DateTime.Now;
                    }
                }
                model.ChangeNotices.Add(n);
                model.SaveChanges();
            }
        }

        /*public static void Update(Change_notice notice)
        {
            using (var m = Model.CreateModel())
            {
                var old = m.ChangeNotices.Include("Allocations")
                        .Include("Allocations.Product")
                        .FirstOrDefault(n => n.id == notice.id);
                if (old != null)
                {
                    old.dateModified = DateTime.Now;
                    old.modifiedById = notice.modifiedById;
                    old.filename = notice.filename;
                    old.description = notice.description;

                    foreach (var alloc in notice.Allocations)
                    {
                        
                        if (alloc.id == 0)
                        {
                            alloc.dateAllocated = DateTime.Now;
                            old.Allocations.Add(alloc);
                        }
                    }
                    var deletedAllocations = old.Allocations.Where(a => notice.Allocations.Count(all => all.id == a.id) == 0).ToList();
                    foreach (var d in deletedAllocations)
                    {
                        m.Entry(d).State = EntityState.Deleted;
                    }
                }
                m.SaveChanges();
                

            }
        }*/

        public override void Update(Change_notice entityToUpdate)
        {
            context.UpdateGraph(entityToUpdate, map => map.OwnedCollection(n => n.Allocations, m2 => m2.AssociatedCollection(a => a.Orders))
                                        .OwnedCollection(n => n.Images).OwnedEntity(n => n.Document));
        }

        public override void Insert(Change_notice entity)
        {
            if(entity.Allocations != null)
            {
                
                foreach (var a in entity.Allocations)
                {
                    a.dateAllocated = DateTime.Now;
                }                

                foreach (var a in entity.Allocations.Where(a=>a.Orders != null))
                {
                    var ids = a.Orders.Select(o => o.orderid).ToList();
                    a.Orders = context.Set<Order_header>().Where(o => ids.Contains(o.orderid)).ToList();
                }
            }
            
            base.Insert(entity);
        }
    }
}
