using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class InspectionCriteriaTemplateMappings : EntityTypeConfiguration<Inspv2_template>
    {
        public InspectionCriteriaTemplateMappings()
        {
            HasMany(t => t.Criteria).WithOptional(c => c.Template).HasForeignKey(c => c.template_id);
            HasMany(t => t.Products)
                .WithMany(p=>p.Inspv2Templates)
                .Map(m => m.MapLeftKey("template_id").MapRightKey("cprod_id").ToTable("inspv2_custproduct_template"));
            
        }
    }

    public class InspectionCriteriaMappings : EntityTypeConfiguration<Inspv2_criteria>
    {
        public InspectionCriteriaMappings()
        {
            HasOptional(c => c.Point).WithMany().HasForeignKey(c => c.point_id);
            HasOptional(c => c.Category).WithMany().HasForeignKey(c => c.category_id);
        }
    }

    public class InspectionCustomCriteriaMappings : EntityTypeConfiguration<Inspv2_customcriteria>
    {
        public InspectionCustomCriteriaMappings()
        {
            HasOptional(c => c.Criteria).WithMany().HasForeignKey(c => c.criteria_id);
            HasOptional(c => c.Point).WithMany().HasForeignKey(c => c.point_id);
            HasRequired(c => c.Product).WithMany().HasForeignKey(c => c.cprod_id);
        }
    }
}
