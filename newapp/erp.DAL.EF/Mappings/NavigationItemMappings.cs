using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Mappings
{
    public class NavigationItemMappings : EntityTypeConfiguration<Navigation_item>
    {
        public NavigationItemMappings()
        {
            //HasMany(i => i.ChildItems).WithOptional().HasForeignKey(i => i.parent_id);
            HasMany(n => n.Permissions).WithOptional(p => p.NavigationItem).HasForeignKey(p => p.navigation_item_id);
        }
    }

    public class NavigationItemPermissionMappings : EntityTypeConfiguration<Navigation_item_permission>
    {
        public NavigationItemPermissionMappings()
        {
            //HasMany(i => i.ChildItems).WithOptional().HasForeignKey(i => i.parent_id);
            HasOptional(n => n.Role).WithMany().HasForeignKey(n => n.role_id);
            HasOptional(n => n.User).WithMany().HasForeignKey(n => n.user_id);
        }
    }
}
