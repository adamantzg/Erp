using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class InspectionV2Mappings : EntityTypeConfiguration<Inspection_v2>
    {
        public InspectionV2Mappings()
        {
            HasOptional(i => i.Factory).WithMany().HasForeignKey(i => i.factory_id);
            HasOptional(i => i.Client).WithMany().HasForeignKey(i => i.client_id);
            HasOptional(i => i.InspectionType).WithMany().HasForeignKey(i => i.type);
            HasMany(i => i.Lines).WithOptional(i => i.Inspection).HasForeignKey(i => i.insp_id);
            HasMany(i => i.Controllers).WithRequired(i => i.Inspection).HasForeignKey(i => i.inspection_id);
            HasMany(i => i.Containers).WithRequired(c => c.Inspection).HasForeignKey(c => c.insp_id);
            HasMany(i => i.MixedPallets).WithOptional(p => p.Inspection).HasForeignKey(p => p.insp_id);
            HasOptional(i => i.Subject).WithMany().HasForeignKey(i => i.si_subject_id);
        }
    }

    public class InspectionV2_Line_Mappings : EntityTypeConfiguration<Inspection_v2_line>
    {
        public InspectionV2_Line_Mappings()
        {
            HasOptional(i => i.OrderLine).WithMany(l=>l.InspectionV2Lines).HasForeignKey(i => i.orderlines_id);
            HasOptional(i => i.Product).WithMany().HasForeignKey(i => i.cprod_id);
            HasMany(l => l.Loadings).WithOptional(l => l.Line).HasForeignKey(l => l.insp_line);
            HasMany(l => l.Images).WithOptional(i=>i.Line).HasForeignKey(i => i.insp_line);
            HasMany(l => l.Rejections).WithOptional(r => r.Line).HasForeignKey(r => r.line_id);
            HasMany(l => l.SiDetails).WithOptional().HasForeignKey(d => d.insp_line);
        }
    }

    public class InspectionV2_Loading_Mappings : EntityTypeConfiguration<Inspection_v2_loading>
    {
        public InspectionV2_Loading_Mappings()
        {
            HasMany(l => l.Areas).WithMany().Map(m => m.MapLeftKey("loading_id").MapRightKey("area_id").ToTable("inspection_v2_loading_area"));
            HasMany(l => l.QtyMixedPallets).WithOptional(mp => mp.Loading).HasForeignKey(mp => mp.loading_id);
        }
    }

    public class InspectionV2_Container_Mappings : EntityTypeConfiguration<Inspection_v2_container>
    {
        public InspectionV2_Container_Mappings()
        {
            HasMany(c => c.Images).WithOptional(i=>i.Container).HasForeignKey(i => i.container_id);
            HasMany(c => c.Loadings).WithOptional(l => l.Container).HasForeignKey(l => l.container_id);
            HasOptional(c => c.ContainerType).WithMany().HasForeignKey(c => c.container_size);
        }
    }

    public class InspectionV2_Controller_Mappings : EntityTypeConfiguration<Inspection_v2_controller>
    {
        public InspectionV2_Controller_Mappings()
        {
            HasRequired(c => c.Controller).WithMany().HasForeignKey(c => c.controller_id);
        }
    }

    public class InspectionV2_Image_Mappings : EntityTypeConfiguration<Inspection_v2_image>
    {
        public InspectionV2_Image_Mappings()
        {
            HasOptional(l => l.Type).WithMany().HasForeignKey(l => l.type_id);

        }
    }

    public class InspectionV2_Line_Sidetails_Mappings : EntityTypeConfiguration<Inspection_v2_line_si_details>
    {
        public InspectionV2_Line_Sidetails_Mappings()
        {
            HasOptional(l => l.Type).WithMany().HasForeignKey(l => l.type_id);

        }
    }


}
