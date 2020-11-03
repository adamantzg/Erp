using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class Whitebook_template_mappings: EntityTypeConfiguration<Whitebook_template>
    {
        public Whitebook_template_mappings()
        {
            HasMany(t => t.OptionGroups).WithOptional().HasForeignKey(o => o.template_id);
			HasMany(t => t.Categories).WithRequired(c => c.WhitebookTemplate).HasForeignKey(c => c.template_id);
        }
    }

    public class Whitebook_optiongroup_mappings : EntityTypeConfiguration<Whitebook_option_group>
    {
        public Whitebook_optiongroup_mappings()
        {
            HasMany(g => g.Options).WithOptional(o => o.Group).HasForeignKey(o => o.group_id);
        }
    }

    public class Whitebook_template_optiongroup_mappings : EntityTypeConfiguration<Whitebook_template_optiongroup>
    {
        public Whitebook_template_optiongroup_mappings()
        {
            HasKey(t => new { t.template_id, t.group_id });
            HasRequired(o => o.Group).WithMany().HasForeignKey(o => o.group_id);
        }
    }
    public class Whitebook_category_mappings : EntityTypeConfiguration<Whitebook_category>
    {
        public Whitebook_category_mappings()
        {
            HasKey(c => c.id);
           // Ignore(c => c.WhitebookTemplateCategory);
            HasMany(c => c.WhitebookTemplateCategory)
                .WithRequired()
                .HasForeignKey(c => c.category_id);
            HasOptional(c => c.Parent).WithMany(c=>c.Children).HasForeignKey(c => c.parent_id);
            //HasRequired(t => t.WhitebookTemplateCategory)(c=>c.);
        }
    }
    public class Whitebook_template_category_mappings : EntityTypeConfiguration<Whitebook_template_category>
    {
        public Whitebook_template_category_mappings()
        {
            HasKey(c => new { c.template_id, c.category_id } );
            /*HasOptional(t => t.WhitebookTemplate)
                .WithRequired(t=>t.WhitebookTemplateCategory);*/
            HasRequired(t => t.WhitebookCategory)
                .WithMany().HasForeignKey(k=>k.category_id);
            //.WithRequired(t=>t.)
           // Ignore(c => c.WhitebookTemplate);
            //Ignore(c => c.WhitebookTemplateCategory);
            //HasMany(o => o.WhitebookTemplateCategory).WithOptional().HasForeignKey(o => o.category_id);
            //HasRequired(t => t.WhitebookTemplateCategory)(c=>c.);
        }
    }

    public class Whitebook_option_mappings: EntityTypeConfiguration<Whitebook_option>
    {
        public Whitebook_option_mappings()
        {
            HasMany(o => o.ChildOptions).WithOptional(o=>o.Parent).HasForeignKey(o => o.parent_option);
            HasOptional(o => o.Group).WithMany().HasForeignKey(o => o.group_id);
            
        }
    }

    

    //public class Whitebook_webproduct_option_mappings : EntityTypeConfiguration<Whitebook_webproduct_option>
    //{
    //    public Whitebook_webproduct_option_mappings()
    //    {
    //        HasOptional(wo => wo.Product).WithMany(p => p.WhitebookOptions).HasForeignKey(wo => wo.web_unique);
    //        HasOptional(wo => wo.Option).WithMany().HasForeignKey(wo => wo.option_id);
    //    }
    //}
}
