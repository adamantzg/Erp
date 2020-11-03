using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class UserMappings : EntityTypeConfiguration<User>
    {
        public UserMappings()
        {
            ToTable("userusers");
            HasKey(u => u.userid);
            Property(u => u.userid).HasColumnName("useruserid");
            Property(u => u.username).HasColumnName("userusername");
            Property(u => u.company_id).HasColumnName("user_id");
            HasRequired(u => u.Company).WithMany(c => c.Users).HasForeignKey(u => u.company_id);
            HasMany(u => u.AdminPages).WithMany(a => a.Users).Map(m => m.MapLeftKey("userid").MapRightKey("page_id").ToTable("user_pages"));
            HasMany(u => u.AdminPagesNew).WithMany(a => a.Users).Map(m => m.MapLeftKey("userid").MapRightKey("page_id").ToTable("v_user_pages"));
            HasMany(u => u.Roles).WithMany(r => r.Users).Map(m => m.MapLeftKey("user_id").MapRightKey("role_id").ToTable("user_role"));
            HasMany(u => u.Groups).WithMany(g=>g.Users).Map(m => m.MapLeftKey("user_id").MapRightKey("group_id").ToTable("user_group"));
            //HasMany(u => u.Permissions).WithMany().Map(m => m.MapLeftKey("user_id").MapRightKey("permission_id").ToTable("user_permisssion"));
            HasMany(u => u.ReturnsQCUsers).WithRequired().HasForeignKey(q => q.useruser_id);
            HasMany(u => u.AssignedQCusers).WithRequired().HasForeignKey(q => q.useruser_id);
            
        }
    }

    public class CompanyMappings : EntityTypeConfiguration<Company>
    {
        public CompanyMappings()
        {
            ToTable("users");
            HasKey(c => c.user_id);
            Property(c => c.EF_fix_2011_interface).HasColumnName("2011_interface");
            HasMany(c => c.ExcludedClients).WithMany().Map(m => m.MapLeftKey("invoice_from_id").MapRightKey("client_id").ToTable("invoices_excludedclients"));
            HasOptional(c => c.Currency).WithMany().HasForeignKey(c => c.user_curr);
        }
    }

    public class AdminPermissionsMappings : EntityTypeConfiguration<Admin_permissions>
    {
        public AdminPermissionsMappings()
        {
            HasKey(a => a.permission_id);
            HasOptional(a => a.User).WithMany(u => u.AdminPermissions).HasForeignKey(a => a.userid);
            HasOptional(a => a.Company).WithMany().HasForeignKey(a => a.cusid);
        }
    }




}
