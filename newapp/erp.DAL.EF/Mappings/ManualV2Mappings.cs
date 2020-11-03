using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class ManualV2Mappings : EntityTypeConfiguration<manual>
    {
        public ManualV2Mappings()
        {
            HasMany(m => m.AdministrationGroups).WithRequired(ag => ag.Manual).HasForeignKey(ag => ag.manual_id);
            HasMany(m => m.Nodes).WithRequired(n => n.Manual).HasForeignKey(n => n.manual_id);
        }
    }

    public class ManualV2MessageMappings : EntityTypeConfiguration<manual_message>
    {
        public ManualV2MessageMappings()
        {
            HasMany(m => m.Files).WithRequired(f => f.Message).HasForeignKey(f => f.message_id);
            HasMany(m => m.Audience).WithRequired(a => a.Message).HasForeignKey(a => a.message_id);
        }
    }

    public class ManualV2NodeMappings : EntityTypeConfiguration<manual_node>
    {
        public ManualV2NodeMappings()
        {
            HasMany(n => n.Messages).WithRequired(m => m.Node).HasForeignKey(m => m.node_id);
            HasOptional(n => n.Parent).WithMany(n => n.Children).HasForeignKey(n => n.node_parent_id);
            HasMany(n => n.EditHistoryRecords).WithRequired(r => r.Node).HasForeignKey(r => r.node_id);
        }
    }

    public class ManualV2EditHistoryRecordsMappings : EntityTypeConfiguration<manual_edit_history>
    {
        public ManualV2EditHistoryRecordsMappings()
        {
            HasRequired(n => n.EditUser).WithMany(u => u.ManualEditHistoryRecords).HasForeignKey(u => u.user_id);
        }
    }
}
