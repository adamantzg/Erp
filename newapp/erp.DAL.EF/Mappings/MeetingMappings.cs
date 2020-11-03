using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using erp.Model;


namespace erp.DAL.EF.Mappings
{
    public class MeetingMappings : EntityTypeConfiguration<Meeting>
    {
        public MeetingMappings()
        {
            HasOptional(m => m.CreatedBy).WithMany().HasForeignKey(m => m.CreatedById);
            HasOptional(m => m.ModifiedBy).WithMany().HasForeignKey(m => m.ModifiedById);
            ToTable("meeting");
            HasMany(m => m.Members)
                .WithMany()
                .Map(m => m.MapLeftKey("meeting_id").MapRightKey("useruserid").ToTable("meeting_members"));
            HasMany(m => m.Details).WithRequired(d => d.Meeting).HasForeignKey(d => d.meeting_id).WillCascadeOnDelete(true);
        }
    }

    public class MeetingDetailsMappings : EntityTypeConfiguration<Meeting_detail>
    {
        public MeetingDetailsMappings()
        {
            HasMany(m => m.Responsibilities).WithRequired(r => r.Detail).HasForeignKey(r => r.meeting_details_id);
            HasMany(m => m.Images).WithOptional(i => i.Detail).HasForeignKey(i => i.meeting_detail_id);
        }
    }

    public class MeetingDetailsResponsibilitiesMappings : EntityTypeConfiguration<Meeting_detail_responsibility>
    {
        public MeetingDetailsResponsibilitiesMappings()
        {
            HasKey(m => new {m.meeting_details_id, m.useruserid});
            HasRequired(m => m.User).WithMany().HasForeignKey(m => m.useruserid);
            
        }
    }

    
}
