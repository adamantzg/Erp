using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class TechnicalDataMappings : EntityTypeConfiguration<Technical_product_data>
    {
        public TechnicalDataMappings()
        {
            HasOptional(t => t.TechnicalDataType)
            .WithMany()
            .HasForeignKey(t => t.technical_data_type);
            
            HasOptional(t => t.MastProduct)
            .WithMany(m=>m.TechnicalProductData)
            .HasForeignKey(t => t.mast_id);
        }
    }

    public class TechnicalSubcategoryTemplateMappings : EntityTypeConfiguration<Technical_subcategory_template>
    {
        public TechnicalSubcategoryTemplateMappings()
        {
            HasKey(t => new {
                t.category1_sub,
                t.technical_data_type
            })
                .HasRequired(t => t.TechnicalDataType)
                .WithMany()
                .HasForeignKey(t => t.technical_data_type);
        }
    }
}
