using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class ExternalDealerMappings : EntityTypeConfiguration<Dealer_external>
    {
        public ExternalDealerMappings()
        {
            HasMany(d => d.Brands)
                .WithMany(b => b.Dealers)
                .Map(m => m.ToTable("dealer_external_brand").MapLeftKey("dealer_id").MapRightKey("brand_id"));

            HasMany(d => d.Comments).WithOptional(c => c.Dealer).HasForeignKey(c => c.dealer_id);
            HasMany(d => d.Displays).WithRequired(disp => disp.Dealer).HasForeignKey(disp => disp.dealer_id);

            HasOptional(d => d.DealerType).WithMany(t => t.Dealers).HasForeignKey(d => d.dealer_type);
            HasOptional(d => d.CustomerType).WithMany(c => c.Dealers).HasForeignKey(d => d.customer_type);
        }
        
    }

    public class DealerExternalDisplayMappings : EntityTypeConfiguration<Dealer_external_display>
    {
        public DealerExternalDisplayMappings()
        {
            HasKey(d => new {d.dealer_id, d.webproduct_id});
            HasRequired(d => d.WebProduct).WithMany(d => d.DealerExternalDisplays).HasForeignKey(d => d.webproduct_id);
        }
    }

    public class DealerExternalCommentMappings : EntityTypeConfiguration<Dealer_external_comment>
    {
        public DealerExternalCommentMappings()
        {
            HasOptional(c => c.User).WithMany(u => u.ExternalDealerComments).HasForeignKey(c => c.user_id);
        }
    }


}
