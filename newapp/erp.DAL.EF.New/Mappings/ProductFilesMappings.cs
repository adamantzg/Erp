using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using erp.Model;

namespace erp.DAL.EF.New.Mappings
{
	public class ProductFilesMappings : EntityTypeConfiguration<product_file>
	{
		public ProductFilesMappings()
		{
			ToTable("product_file");
			HasOptional(f => f.FileType).WithMany().HasForeignKey(f=>f.type_id);
		}
	}

	public class ProductFileTypesMappings: EntityTypeConfiguration<product_file_type>
	{
		public ProductFileTypesMappings()
		{
			ToTable("product_file_type");
		}
	}
}
