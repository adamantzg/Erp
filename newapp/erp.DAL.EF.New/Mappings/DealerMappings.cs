using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.DAL.EF.Mappings
{
    public class DealerMappings : EntityTypeConfiguration<Dealer>
    {
        public DealerMappings()
        {
            HasKey(d => d.user_id);
			Property(d => d.user_id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            ToTable("dealers");
            Property(d => d.AnnualTurnover).HasColumnName("annual_turnover");
            Property(d => d.AnnualTurnoverRange).HasColumnName("annual_turnover_range");
            Ignore(d => d.brand_wc);
            Ignore(d => d.distributor_id);
            Ignore(d => d.DistributorName);
            Ignore(d => d.DistributorCode);
            Ignore(d => d.DistributorEmail);
            Ignore(d => d.Distance);
            Ignore(d => d.gold);
            Ignore(d => d.silver);
            Ignore(d => d.brand_status);
            Ignore(d => d.numOfImages);
            Ignore(d => d.numOfDisplays);
            Ignore(d => d.brand_status_manual);
            Ignore(d => d.brand_code);
            HasMany(d => d.Distributors)
                .WithMany(c => c.Dealers)
                .Map(m => m.MapLeftKey("dealer_id").MapRightKey("distributor_id").ToTable("dealer_distributors"));

            HasMany(d => d.Dealer_Images).WithRequired(im => im.Dealer).HasForeignKey(im => im.dealer_id);

            HasMany(d => d.DealerBrandstatuses).WithRequired(dbs => dbs.Dealer).HasForeignKey(dbs => dbs.dealer_id);

            HasMany(d => d.DisplayActivities).WithOptional(a => a.Dealer).HasForeignKey(a => a.dealer_id);

            HasMany(d => d.DisplayRebates).WithRequired(r => r.Dealer).HasForeignKey(r => r.dealer_id);

        }
    }

    public class DealerImageMappings : EntityTypeConfiguration<Dealer_images>
    {
        public DealerImageMappings()
        {
            ToTable("dealer_images");
            HasKey(d => d.image_unique);
            HasMany(d => d.Brands)
                .WithMany(b => b.Images)
                .Map(m => m.MapLeftKey("dealer_image_id").MapRightKey("brand_id").ToTable("dealer_image_brand"));
            HasMany(im => im.Displays).WithRequired(d => d.Image).HasForeignKey(d => d.image_id);

        }
    }

    public class DealerImageDisplayMappings: EntityTypeConfiguration<Dealer_image_displays>
    {
        public DealerImageDisplayMappings()
        {
            ToTable("dealer_image_displays");
            HasKey(d => new {d.image_id, d.web_unique});
            HasRequired(di => di.ProductNew).WithMany().HasForeignKey(d => d.web_unique);
            //HasRequired(d => d.Image).WithMany(im=>im.Displays).HasForeignKey(d => d.image_id);
        }
    }

    public class Dealer_BrandstatusMappings : EntityTypeConfiguration<Dealer_brandstatus>
    {
        public Dealer_BrandstatusMappings()
        {
            HasKey(dbs => new {dbs.dealer_id, dbs.brand_id});
        }
    }

    public class Dealer_SalesMappings : EntityTypeConfiguration<Dealer_sales_data_header>
    {
        public Dealer_SalesMappings()
        {
            HasRequired(d => d.Dealer).WithMany().HasForeignKey(d => d.dealer_id);
            HasMany(d => d.Lines).WithRequired(l => l.Header).HasForeignKey(l => l.orderid);
        }
    }
}
