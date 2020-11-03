using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class InspectionsMappings : EntityTypeConfiguration<Inspections>
    {
        public InspectionsMappings()
        {
            HasKey(i => i.insp_unique);
			HasMany(i => i.LinesTested).WithOptional(l=>l.Inspection).HasForeignKey(l => l.insp_id);
			HasOptional(i=>i.Inspection_V2).WithMany().HasForeignKey(i=>i.new_insp_id);
			HasMany(i=>i.Returns).WithOptional().HasForeignKey(r=>r.insp_id);
			HasMany(i => i.LinesAccepted).WithOptional().HasForeignKey(l => l.insp_unique);
			HasMany(i => i.LinesRejected).WithOptional().HasForeignKey(l => l.insp_unique);
			HasMany(i => i.NrHeaders).WithOptional().HasForeignKey(h => h.insp_id);
        }
    }

    public class InspectionLinesTestedMappings : EntityTypeConfiguration<Inspection_lines_tested>
    {
        public InspectionLinesTestedMappings()
        {
            HasKey(l => l.insp_line_unique);
            HasOptional(l => l.OrderLine).WithMany().HasForeignKey(l => l.order_linenum);
            HasMany(l => l.AcceptedLines).WithOptional(la=>la.LineTested).HasForeignKey(la => la.insp_line_id);
            HasMany(l => l.RejectedLines).WithOptional(lr=>lr.LineTested).HasForeignKey(lr => lr.insp_line_id);
            HasMany(l => l.Loadings).WithOptional(lo=>lo.LineTested).HasForeignKey(lo => lo.insp_line_unique);
        }
    }
}
