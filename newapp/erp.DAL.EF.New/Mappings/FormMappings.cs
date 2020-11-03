using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Mappings
{
    public class FormMappings : EntityTypeConfiguration<Form>
    {
        public FormMappings()
        {
            ToTable("form");
            HasMany(f => f.Sections).WithRequired(s => s.Form).HasForeignKey(s => s.form_id);
            HasMany(f => f.Submissions).WithOptional(s=>s.Form).HasForeignKey(s => s.form_id);
        }
    }

    public class FormFormSectionMappings : EntityTypeConfiguration<Form_formsection>
    {
        public FormFormSectionMappings()
        {
            ToTable("Form_formsection");
            HasKey(f => new { f.form_id, f.section_id });
        }
    }

    public class FormSectionMappings : EntityTypeConfiguration<Form_section>
    {
        public FormSectionMappings()
        {
            ToTable("form_section");
            HasMany(f => f.Questions).WithOptional(q => q.Section).HasForeignKey(q => q.section_id);            
        }
    }

    public class FormSectionQuestionMappings : EntityTypeConfiguration<Form_section_question>
    {
        public FormSectionQuestionMappings()
        {
            ToTable("Form_section_question");
            HasOptional(s => s.QuestionGroup).WithMany().HasForeignKey(s => s.question_group_id);
            HasOptional(s => s.Question).WithMany().HasForeignKey(s => s.question_id);
        }
    }

    public class FormQuestionMappings : EntityTypeConfiguration<Form_question>
    {
        public FormQuestionMappings()
        {
            ToTable("Form_question");
            HasOptional(q => q.ChoiceGroup).WithMany().HasForeignKey(q => q.choice_group_id);
            HasOptional(q => q.QuestionRenderMethod).WithMany().HasForeignKey(q => q.render_id);
            HasOptional(q => q.QuestionType).WithMany().HasForeignKey(q => q.question_type);
            HasMany(q => q.QuestionGroups).WithRequired(g => g.Question).HasForeignKey(g => g.question_id);
        }
    }

    public class FormChoiceGroupMappings : EntityTypeConfiguration<Form_choice_group>
    {
        public FormChoiceGroupMappings()
        {
            ToTable("Form_choice_group");
            HasMany(f => f.Choices).WithMany().Map(m => m.ToTable("form_choicegroup_choice").MapLeftKey("group_id").MapRightKey("choice_id"));
        }
    }

    public class FormQuestionGroupMappings : EntityTypeConfiguration<Form_question_group>
    {
        public FormQuestionGroupMappings()
        {
            ToTable("Form_question_group");
            HasMany(g => g.Questions).WithRequired(q => q.Group).HasForeignKey(q => q.group_id);            
        }
    }

    public class FormQuestionGroupQuestionMappings : EntityTypeConfiguration<Form_questiongroup_question>
    {
        public FormQuestionGroupQuestionMappings()
        {
            HasKey(f => new { f.question_id, f.group_id });
        }
    }

    public class FormSubmissionMappings : EntityTypeConfiguration<Form_submission>
    {
        public FormSubmissionMappings()
        {
            ToTable("Form_submission");
            HasMany(s => s.Answers).WithOptional(a => a.Submission).HasForeignKey(a => a.submission_id);
            HasOptional(s => s.User).WithMany().HasForeignKey(s => s.user_id);
        }
    }

    public class FormQuestionAnswerMappings: EntityTypeConfiguration<form_question_answer>
    {
        public FormQuestionAnswerMappings()
        {
            HasMany(f => f.Choices).WithMany().Map(m => m.ToTable("form_submission_answer_choice").MapLeftKey("answer_id").MapRightKey("choice_id"));
            HasOptional(f => f.Question).WithMany().HasForeignKey(f => f.question_id);
        }
    }

    public class FormSubmissionAnswerMappings : EntityTypeConfiguration<Form_submission_answer>
    {
        public FormSubmissionAnswerMappings()
        {
            ToTable("Form_submission_answer");            
            HasOptional(a => a.SectionQuestion).WithMany().HasForeignKey(a => a.formsection_question_id);
            HasMany(a => a.QuestionAnswers).WithOptional().HasForeignKey(qa => qa.form_submission_answer_id);
        }
    }

    public class FormQuestionTypeMappings : EntityTypeConfiguration<Form_question_type>
    {
        public FormQuestionTypeMappings()
        {
            ToTable("Form_question_type");
            HasOptional(t => t.RenderMethod).WithMany().HasForeignKey(t => t.default_render_id);
        }
    }

}
