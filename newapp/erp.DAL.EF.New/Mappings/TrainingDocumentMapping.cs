using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class TrainingDocumentMapping : EntityTypeConfiguration<tc_document> 
    {
        public TrainingDocumentMapping()
        {
            HasOptional(d => d.Original).WithMany(d => d.Children).HasForeignKey(d => d.original_id);
            HasMany(d => d.Users).WithMany().Map(m => m.ToTable("tc_document_user").MapLeftKey("document_id").MapRightKey("user_id"));
            HasOptional(d => d.Creator).WithMany().HasForeignKey(d => d.createdBy);
            HasOptional(d => d.Editor).WithMany().HasForeignKey(d => d.modifiedBy);
        }
    }
}
