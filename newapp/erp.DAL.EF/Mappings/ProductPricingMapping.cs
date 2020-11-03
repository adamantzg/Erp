using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
	class ProductPricingModelLevelMapping : EntityTypeConfiguration<ProductPricing_model_level>
	{
		public ProductPricingModelLevelMapping()
		{
			ToTable("pp_model_level");			
		}
	}

	class ProductPricingModelMapping : EntityTypeConfiguration<ProductPricing_model>
	{
		public ProductPricingModelMapping()
		{
			ToTable("pp_model");
			HasMany(m => m.Levels).WithOptional(l=>l.Model).HasForeignKey(l => l.model_id);
			HasOptional(m => m.Market).WithMany().HasForeignKey(m => m.market_id);
		}
	}

	class FreightCostMapping : EntityTypeConfiguration<Freightcost>
	{
		public FreightCostMapping()
		{
			ToTable("pp_freightcost");
			HasOptional(c => c.Market).WithMany().HasForeignKey(c => c.market_id);
			HasOptional(c => c.Location).WithMany().HasForeignKey(c => c.location_id);
			HasOptional(c => c.ContainerType).WithMany().HasForeignKey(c => c.container_id);
		}
	}

	class MarketMapping : EntityTypeConfiguration<Market>
	{
		public MarketMapping()
		{
			ToTable("pp_market");
		}
	}

	class ProductPricingSettingsMappings: EntityTypeConfiguration<ProductPricing_settings>
	{
		public ProductPricingSettingsMappings()
		{
			ToTable("pp_settings");

		}
	}

	class ProductPricingProjectMapping : EntityTypeConfiguration<ProductPricingProject>
	{
		public ProductPricingProjectMapping()
		{
			ToTable("pp_project");
			HasOptional(p => p.PricingModel).WithMany().HasForeignKey(p => p.pricing_model_id);
			HasMany(p => p.Products).WithMany(p=>p.Projects).Map(m => m.ToTable("pp_project_product").MapLeftKey("project_id").MapRightKey("cprod_id"));
			HasOptional(p => p.Currency).WithMany().HasForeignKey(p => p.currency_id);
			HasMany(p => p.ProjectSettings).WithOptional().HasForeignKey(s => s.project_id);
		}
	}
}
