using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Repositories
{
    public class NavigationRepository
    {
        public static List<Navigation_item> GetAll()
        {
            using (var m = Model.CreateModel()) {
                return m.NavigationItems.ToList();
            }
        }
        
    }

    public class NavigationPermissionRepository
    {
        public static List<Navigation_item_permission> GetAll()
        {
            using (var m = Model.CreateModel()) {
                return m.NavigationItemPermissions.ToList();
            }
        }

        public static Navigation_item_permission Create(Navigation_item_permission item)
        {
            using (var m = Model.CreateModel()) {
                m.NavigationItemPermissions.Add(item);
                m.SaveChanges();
                if(item.role_id != null)
                    m.Entry(item).Reference(i=>i.Role).Load();
                if(item.user_id != null)
                    m.Entry(item).Reference(i=>i.User).Load();
                return item;
            }
        }

        public static void Update(Navigation_item_permission item)
        {
            using (var m = Model.CreateModel()) {
                m.NavigationItemPermissions.Attach(item);
                m.Entry(item).State = EntityState.Modified;
                m.SaveChanges();
            }
        }

        public static void Remove(int id)
        {
            using (var m = Model.CreateModel())
            {
                m.Database.ExecuteSqlCommand("DELETE FROM navigation_item_permission WHERE id = @p0", id);
            }
        }

        public static List<Navigation_item_permission> GetByItem(int id)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.NavigationItemPermissions.Include(p => p.Role)
                        .Include(p => p.User)
                        .Where(p => p.navigation_item_id == id)
                        .ToList();
            }
        }
    }
}
