using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class ReturnsMapping : EntityTypeConfiguration<Returns>
    {
        public ReturnsMapping()
        {
            HasKey(r => r.returnsid);
            HasOptional(r => r.Creator).WithMany(u => u.Returns).HasForeignKey(r => r.request_userid);
            HasOptional(r => r.Product).WithMany(p => p.Returns).HasForeignKey(r => r.cprod_id);
            HasMany(r => r.Comments).WithRequired(c => c.Return).HasForeignKey(c => c.return_id);
            HasMany(r => r.Images).WithRequired(im => im.Return).HasForeignKey(im => im.return_id);
            HasOptional(r => r.Category).WithMany(c => c.Feedbacks).HasForeignKey(r => r.feedback_category_id);
            HasOptional(r => r.Importance).WithMany(im => im.Returns).HasForeignKey(r => r.importance_id);
            HasOptional(r => r.Client).WithMany(c => c.Returns).HasForeignKey(r => r.client_id);
            Property(r => r.importance_id).HasColumnName("importance");
            HasMany(r => r.Products).WithMany().Map(m => m.ToTable("returns_cust_products").MapLeftKey("returns_id").MapRightKey("cprod_id"));
            HasMany(r => r.Subscriptions).WithOptional(s => s.Return).HasForeignKey(s => s.subs_returnid);
            HasOptional(r => r.Return_resolution).WithMany().HasForeignKey(n=>n.resolution);

            //HasMany(r => r.ReturnsQCUsers).WithMany().Map(m => m.ToTable("returns_qcusers").MapLeftKey("return_id").MapRightKey("useruser_id"));
            //HasMany(r => r.ReturnsQCUsers).WithMany();
            HasMany(r => r.ReturnsQCUsers).WithRequired().HasForeignKey(q => q.return_id);
            HasMany(r => r.AssignedQCUsers).WithRequired().HasForeignKey(q => q.return_id);

            HasOptional(r => r.AssignedQC).WithMany(qa => qa.QCAssignedReturns).HasForeignKey(r => r.assigned_qc);
            HasOptional(r => r.IssueType).WithMany().HasForeignKey(r => r.issue_type_id);
			HasOptional(r => r.DecisionFinal).WithMany().HasForeignKey(r => r.decision_final);

            HasMany(r => r.Events).WithRequired(e => e.Return).HasForeignKey(e => e.return_id);
        }
    }

    public class ReturnCommentMapping : EntityTypeConfiguration<Returns_comments>
    {
        public ReturnCommentMapping()
        {
            HasKey(c => c.comments_id);
            HasOptional(c => c.Creator).WithMany(u => u.ReturnsComments).HasForeignKey(c => c.comments_from);
            HasMany(c => c.Files).WithRequired(f => f.Comment).HasForeignKey(f => f.return_comment_id);
            HasOptional(c => c.Creator).WithMany(u => u.ReturnsComments).HasForeignKey(f => f.comments_from);
        }
    }

    public class ReturnCommentFileMapping : EntityTypeConfiguration<Returns_comments_files>
    {
        public ReturnCommentFileMapping()
        {
            HasKey(f => f.return_comment_file_id);
        }
    }

    public class FeedbackSubscriptionMapping : EntityTypeConfiguration<Feedback_subscriptions>
    {
        public FeedbackSubscriptionMapping()
        {
            HasOptional(f => f.User).WithMany().HasForeignKey(f => f.subs_useruserid);
        }
    }

    public class Returns_qcusers_mappings : EntityTypeConfiguration<Returns_qcusers>
    {
        public Returns_qcusers_mappings()
        {
            HasKey(c => new { c.return_id, c.useruser_id, c.type });

            //HasRequired(q => q.Return)
            //    .WithMany(r => r.ReturnsQCUsers)
            //    .HasForeignKey(q => q.return_id);
                
            //HasRequired(q => q.User)
            //    .WithMany(u => u.ReturnsQCUsers)
            //    .HasForeignKey(q => q.useruser_id);

        }
    }

    public class FeedbackAuthorizationMapping : EntityTypeConfiguration<feedback_authorization>
    {
        public FeedbackAuthorizationMapping()
        {
            HasMany(f => f.Levels).WithRequired(l=>l.Authorization).HasForeignKey(l => l.feedback_authorization_id);
        }
    }

    public class ReturnEventMappings : EntityTypeConfiguration<returns_events>
    {
        public ReturnEventMappings()
        {
            HasKey(e => e.event_id);
        }
    }

}
