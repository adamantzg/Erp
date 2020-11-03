using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Mappings
{
	public class CWProductMappings : EntityTypeConfiguration<Cw_product>
	{
		public CWProductMappings()
		{
			ToTable("cw_product");
			HasMany(p => p.Components).WithMany().Map(m => m.ToTable("cw_product_component").MapLeftKey("product_id").MapRightKey("component_id"));
			HasMany(p => p.Files).WithOptional(f=>f.Product).HasForeignKey(f => f.product_id);
			HasOptional(p => p.Site).WithMany().HasForeignKey(p => p.site_id);
			HasMany(p => p.Infos).WithOptional().HasForeignKey(i => i.product_id);
		}
	}

	public class CWComponentMappings : EntityTypeConfiguration<Cw_component>
	{
		public CWComponentMappings()
		{
			ToTable("cw_component");
			HasMany(c => c.Features).WithOptional().HasForeignKey(f => f.component_id);
		}
	}
}
