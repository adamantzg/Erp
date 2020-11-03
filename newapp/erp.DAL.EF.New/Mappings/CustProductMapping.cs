using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class CustProductMapping : EntityTypeConfiguration<Cust_products>
    {
        public CustProductMapping()
        {
            HasKey(c => c.cprod_id);
            HasMany(c => c.SalesProducts).WithOptional(s => s.Product).HasForeignKey(s => s.cprod_id);
			HasOptional(c => c.MastProduct).WithMany(m => m.CustProducts).HasForeignKey(c => c.cprod_mast);
            Property(c => c.brand_userid).HasColumnName("brand_user_id");
            //Property(c => c.prod_type).HasColumnName("product_type");
            HasOptional(c => c.ProductType).WithMany(t => t.Products).HasForeignKey(c => c.product_type);
            HasOptional(c => c.Brand).WithMany(b => b.CustProducts).HasForeignKey(b => b.brand_id);
            HasOptional(c => c.BrandCompany).WithMany().HasForeignKey(c => c.brand_userid);
            HasOptional(c => c.Category).WithMany().HasForeignKey(c => c.cprod_brand_cat);
            HasOptional(c => c.AnalyticsSubCategory).WithMany().HasForeignKey(c => c.analytics_category);
            HasMany(c => c.Parents)
                .WithMany()
                .Map(m => m.ToTable("spares").MapLeftKey("spare_cprod").MapRightKey("product_cprod"));
            HasOptional(c => c.Color).WithMany().HasForeignKey(c => c.color_id);
            HasOptional(c => c.ProductRange).WithMany().HasForeignKey(c => c.cust_product_range_id);
			HasMany(c => c.MarketData).WithOptional().HasForeignKey(m => m.cprod_id);
			HasMany(c => c.SalesForecast).WithOptional(s => s.Product).HasForeignKey(s => s.cprod_id);
			HasMany(c=>c.AutoAddedProducts).WithOptional().HasForeignKey(a=>a.trigger_cprod_id);
			HasOptional(c=>c.ExtraData).WithRequired(x=>x.Cust_Products);
			HasMany(m=>m.OtherFiles).WithMany().Map(m=>m.ToTable("cust_product_file").MapLeftKey("cprod_id").MapRightKey("file_id"));
			HasMany(p => p.ProductFiles).WithOptional().HasForeignKey(f => f.cprod_id);
        }
      
    }

    public class MastProductMapping : EntityTypeConfiguration<Mast_products>
    {
        public MastProductMapping()
        {
            HasKey(m => m.mast_id);
            //Property(m => m.categorydop_id).HasColumnName("category_dop");
            HasOptional(m => m.Factory).WithMany(f => f.MastProducts).HasForeignKey(m => m.factory_id);
            HasMany(m => m.Characteristics).WithRequired(c => c.MastProduct).HasForeignKey(c => c.mast_id);
            HasOptional(m => m.SubCategory).WithMany().HasForeignKey(m => m.category1_sub);
            HasMany(m => m.Components).WithOptional(c => c.MastProduct).HasForeignKey(c => c.mast_id);
			HasOptional(m => m.ProductPricingData).WithRequired();
			
			HasMany(m => m.Prices).WithOptional().HasForeignKey(p => p.mastproduct_id);
			HasMany(m => m.PackagingMaterials).WithOptional(p => p.MastProduct).HasForeignKey(p => p.mast_id);
			HasOptional(m=>m.Category).WithMany(c=>c.Products).HasForeignKey(m=>m.category1);
			HasMany(m=>m.OtherFiles).WithMany(f=>f.MastProducts).Map(m=>m.ToTable("mast_product_file").MapLeftKey("mast_id").MapRightKey("file_id"));
			HasMany(p => p.ProductFiles).WithOptional().HasForeignKey(f => f.mast_id);
        }
    }

    public class MastProductCharMapping : EntityTypeConfiguration<Mastproduct_characteristics>
    {
        public MastProductCharMapping()
        {
            HasKey(m => new {m.characteristics_id,m.mast_id});
            HasRequired(m => m.MastProduct).WithMany(m => m.Characteristics).HasForeignKey(m => m.mast_id);
            HasRequired(m => m.Characteristic)
                .WithMany(c => c.MastproductCharacteristics)
                .HasForeignKey(m => m.characteristics_id);
        }
    }

    public class DistProductMapping : EntityTypeConfiguration<Dist_products>
    {
        public DistProductMapping()
        {
            HasOptional(d => d.Distributor).WithMany(c => c.DistProducts).HasForeignKey(d => d.client_id);
            HasOptional(d => d.Product).WithMany(p => p.DistProducts).HasForeignKey(d => d.dist_cprod_id);
        }
    }

    public class AsaqColorMapping : EntityTypeConfiguration<AsaqColor>
    {
        public AsaqColorMapping()
        {
            HasKey(m => m.color_id);
        }
    }

    public class CustProductBundleMapping : EntityTypeConfiguration<cust_products_bundle>
    {
        public CustProductBundleMapping()
        {
            HasOptional(b => b.Option).WithMany().HasForeignKey(b => b.analytics_option_id);
            HasMany(b => b.Components).WithRequired(c => c.Bundle).HasForeignKey(c => c.bundle_id);
        }
    }

    public class CustProductBundleComponentMapping : EntityTypeConfiguration<cust_products_bundle_component>
    {
        public CustProductBundleComponentMapping()
        {
            HasKey(c => new { c.bundle_id, c.cprod_id });
            HasRequired(c => c.Component).WithMany().HasForeignKey(c => c.cprod_id);
        }
    }
}
