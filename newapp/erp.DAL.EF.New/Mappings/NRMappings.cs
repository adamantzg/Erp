using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class NRHeaderMappings : EntityTypeConfiguration<Nr_header>
    {
        public NRHeaderMappings()
        {
            HasMany(h => h.Lines).WithRequired(l => l.Header).HasForeignKey(l => l.NR_id);
            HasMany(h => h.Images).WithOptional().HasForeignKey(im => im.nr_id);
            HasOptional(h => h.Inspection).WithMany().HasForeignKey(h => h.insp_id);
            HasOptional(h => h.InspectionV2).WithMany().HasForeignKey(h => h.insp_v2_id);
        }
    }

    public class NRLinesMappings : EntityTypeConfiguration<Nr_lines>
    {
        public NRLinesMappings()
        {
            HasMany(l => l.Images).WithOptional(i => i.Line).HasForeignKey(i => i.NR_line_id);
            HasOptional(l => l.InspectionLineTested).WithMany().HasForeignKey(l => l.inspection_lines_tested_id);
            HasOptional(l => l.InspectionV2Line).WithMany().HasForeignKey(l => l.inspection_lines_v2_id);
            HasOptional(l => l.Type).WithMany().HasForeignKey(l => l.NR_line_type);
        }
    }

    public class NrLineImagesMappings : EntityTypeConfiguration<Nr_line_images>
    {
        public NrLineImagesMappings()
        {
            HasOptional(i => i.Type).WithMany().HasForeignKey(i => i.image_type);
        }
    }
}
