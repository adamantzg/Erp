using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.DAL.EF.Mappings
{
    public class WebProductMappings : EntityTypeConfiguration<Web_product_new>
    {
        public WebProductMappings()
        {
            HasKey(p => p.web_unique);
            Property(p => p.web_unique).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Ignore(p => p.ImageExists);
            Ignore(p => p.ThumbExists);
            Ignore(p => p.category_id);
            Ignore(p => p.Category);
            Ignore(p => p.Siblings);
            //Ignore(p => p.SuggestedProducts);
            //Ignore(p => p.Complementary);
            Ignore(p => p.SelectedSalePeriods);
            HasRequired(p => p.WebSite).WithMany(s => s.WebProducts).HasForeignKey(p => p.web_site_id);

            HasMany(p => p.SelectedCategories)
                .WithMany(c => c.Products)
                .Map(m => m.MapLeftKey("web_unique").MapRightKey("category_id").ToTable("web_product_category"));
                            
            HasMany(p => p.Components)//Cust_products
                .WithRequired(c => c.Product).HasForeignKey(p => p.web_unique);
                                    //Web_product_new
            HasMany(p => p.WebFiles).WithOptional(f => f.Product).HasForeignKey(f => f.web_unique);
            //HasMany(p => p.Complementary).WithRequired(p => p.Web_product_new).HasForeignKey(p => p.web_unique);
            HasMany(p => p.ProductFlows).WithOptional(f => f.Product).HasForeignKey(f => f.web_unique);
            HasMany(p => p.ProductInfo).WithOptional(f => f.Product).HasForeignKey(f => f.web_unique);
            HasMany(p => p.RelatedProducts)
                .WithMany()
                .Map(
                    m => m.MapLeftKey("web_unique").MapRightKey("web_unique_related").ToTable("web_product_new_related_2"));
            HasOptional(p => p.Parent).WithMany(p => p.Children).HasForeignKey(p => p.parent_id);

            HasMany(p => p.WhitebookOptions)
                .WithMany()
                .Map(m => m.ToTable("Whitebook_webproduct_option").MapLeftKey("web_unique").MapRightKey("option_id"));
            HasOptional(p => p.WhitebookTemplate).WithMany().HasForeignKey(p => p.whitebook_template_id);
        }
    }

    public class WebCategoryMappings : EntityTypeConfiguration<Web_category>
    {
        public WebCategoryMappings()
        {
            HasKey(c => c.category_id);
            Ignore(c => c.ChildCount);
            Ignore(c => c.ProductCount);
            Ignore(c => c.ImageExists);
            HasMany(c => c.Children).WithOptional(c => c.Parent).HasForeignKey(c => c.parent_id);
            HasMany(c => c.Translations).WithOptional().HasForeignKey(t => t.category_id);

        }
    }

    public class WebProductsRelatedMappings : EntityTypeConfiguration<web_product_new_related>
    {
        public WebProductsRelatedMappings()
        {
            HasKey(w => new {w.web_unique, w.web_unique_related});
            
        }
    }

    public class WebProductComponentMappings : EntityTypeConfiguration<Web_product_component>
    {
        public WebProductComponentMappings()
        {
            HasKey(c => new {c.web_unique, c.cprod_id});
            //HasRequired(c => c.Product).WithMany(p => p.Components).HasForeignKey(p => p.web_unique);
        }
    }

    public class WebProductFileMappings : EntityTypeConfiguration<Web_product_file>
    {
        public WebProductFileMappings()
        {
            Ignore(f => f.Data);
            Ignore(f => f.DoesFileExist);
            
        }
    }

    public class WebProductSearchMapping : EntityTypeConfiguration<Webproduct_search>
    {
        public WebProductSearchMapping()
        {
            HasMany(p => p.WebFiles).WithOptional().HasForeignKey(f => f.web_unique);
            HasOptional(p => p.Category).WithMany().HasForeignKey(p => p.category_id);
            HasOptional(p => p.Site).WithMany().HasForeignKey(p => p.web_site_id);
	        HasRequired(p => p.WebProduct).WithOptional();
        }

    }
}
