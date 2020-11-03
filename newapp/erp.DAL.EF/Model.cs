using erp.DAL.EF.Mappings;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace erp.DAL.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using erp.Model;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class Model : DbContext
    {


        public Model()
            : base("name=erp.Model.DAL.Properties.Settings.ConnString")
        {
        }

        public Model(string connString) : base(connString)
        {

        }



        public virtual DbSet<sales_forecast_contract> sales_forecast_contracts { get; set; }
        //public virtual DbSet<Analytics_categories> Analytics_categories { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<Dealer> Dealers { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Web_category> Categories { get; set; }
        public virtual DbSet<Web_product_new> WebProducts { get; set; }
        public virtual DbSet<Order_header> Orders { get; set; }
        public virtual DbSet<Order_lines> OrderLines { get; set; }
        public virtual DbSet<Porder_lines> POrderLines { get; set; }
        public virtual DbSet<Porder_header> POrders { get; set; }
        public virtual DbSet<Admin_permissions> AdminPermissions { get; set; }
        public virtual DbSet<Cust_products> CustProducts { get; set; }
        public virtual DbSet<Returns> Returns { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Dealer_external> ExternalDealers { get; set; }
        public virtual DbSet<Brand_external> ExternalBrands { get; set; }
        public virtual DbSet<Inspv2_template> InspectionCriteriaTemplates { get; set; }
        public virtual DbSet<Inspv2point> InspectionCriteriaPoints { get; set; }
        public virtual DbSet<Inspv2_criteriacategory> InspectionCriteriaCategories { get; set; }
        public virtual DbSet<Inspv2_customcriteria> InspectionCustomcriteria { get; set; }
        public virtual DbSet<Sales_forecast_amendments> SalesForecastAmendments { get; set; }
        public virtual DbSet<Sales_forecast> SalesForecasts { get; set; }
        public virtual DbSet<BudgetActualData> BudgetActualData { get; set; }
        public virtual DbSet<Qc_comments> QcComments { get; set; }
        public virtual DbSet<Web_product_transfer> WebProductTransfers { get; set; }
        public virtual DbSet<BrandCategory> BrandCategories { get; set; }
        public virtual DbSet<Brand_categories_group> BrandCategoryGroups { get; set; }
        public virtual DbSet<Dealer_sales_data_header> DealerSales { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Change_notice> ChangeNotices { get; set; }
        public virtual DbSet<change_notice_image> ChangeNoticeImages { get; set; }
        public virtual DbSet<change_notice_image_type> ChangeNoticeImageTypes { get; set; }
        public virtual DbSet<change_notice_document> ChangeNoticeDocument { get; set; }
        public virtual DbSet<inspection_lines_notified_v2_table> InspectionLinesNotified { get; set; }
        public virtual DbSet<inspection_notified_summary_table> InspectionNotifiedSummary { get; set; }
        public virtual DbSet<inspection_notified_summary_images_table> InspectionNotifiedSummaryImages { get; set; }
        public virtual DbSet<inspection_images_v2_table> InspectionImages { get; set; }
        public virtual DbSet<Stocksummary_factoryvalue> StocksummaryFactoryvalues { get; set; }
        public virtual DbSet<Inspection_v2> InspectionsV2 { get; set; }
        public virtual DbSet<Inspection_v2_area> InspectionV2Areas { get; set; }
        public virtual DbSet<Inspection_v2_loading> InspectionV2Loadings { get; set; }
        public virtual DbSet<Inspection_v2_type> InspectionV2Types { get; set; }
        public virtual DbSet<change_notice_product_table> ChangeNoticeProducts { get; set; }
        public virtual DbSet<Container_types> ContainerTypes { get; set; }
        public virtual DbSet<Inspection_v2_image_type> InspectionImageTypes { get; set; }
        public virtual DbSet<Invoice_export_settings> InvoiceExportSettings { get; set; }
        public virtual DbSet<Shipments> Shipments { get; set; }
        public virtual DbSet<Download_logs> DownloadLogs { get; set; }
        public virtual DbSet<Technical_data_type> TechnicalDataTypes { get; set; }
        public virtual DbSet<Technical_subcategory_template> TechnicalSubcatTemplates { get; set; }
        public virtual DbSet<Technical_product_data> TechnicalProductData { get; set; }
        public virtual DbSet<Mast_products> MastProducts { get; set; }
        public virtual DbSet<Category1_sub> Subcategories { get; set; }
        public virtual DbSet<AutomatedTask> AutomatedTasks { get; set; }
        public virtual DbSet<Factory_client_settings> FactoryClientSettings { get; set; }
        public virtual DbSet<Factory_stock_order> FactoryStockOrders { get; set; }
        public virtual DbSet<Factory_stock_order_lines> FactoryStockOrdersLines { get; set; }
        public virtual DbSet<Navigation_item> NavigationItems { get; set; }
        public virtual DbSet<Navigation_item_permission> NavigationItemPermissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Stock_order_allocation> StockOrderAllocations { get; set; }
        public virtual DbSet<Spare> Spares { get; set; }
        public virtual DbSet<mast_product_file_type> MastProductFileTypes { get; set; }
        public virtual DbSet<cust_product_file_type> CustProductFileTypes { get; set; }
        public virtual DbSet<Inspections> Inspections { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<order_invoice_sequence> OrderInvoiceSequences { get; set; }
        public virtual DbSet<ds_guarantee_registration> DigitalShowerGuaranteeRegistration { get; set; }


        //public virtual DbSet<Inspection_v2_image> InspectionV2Images { get; set; }
        //public virtual DbSet<Meeting_detail_image> MeetingDetailImages { get; set; }

        //Views
        public virtual DbSet<Stock_summary_report> StockSummaryReports { get; set; }
        public virtual DbSet<Web_slide> WebSlide { get; set; }
        //public virtual DbSet<Stock_summary_report_calloff> StockSummaryReportCalloffs { get; set; }
        public virtual DbSet<Brs_sales_analysis2> NonBrandSalesAnalysis { get; set; }
        public virtual DbSet<Brand_sales_analysis_product3> BrandSalesAnalysisProduct3 { get; set; }
        public virtual DbSet<Dealers_all_brands_new_export> DealersAllBrandsNewExports { get; set; }
        public virtual DbSet<Claims_monthly_summary> ClaimsMonthlySummaries { get; set; }
        public virtual DbSet<tp2_sales_export_all> SalesExportReport { get; set; }
        public virtual DbSet<us_call_log2> UsCallLog2 { get; set; }
        public virtual DbSet<Us_call_log_images> UsCallLogImages { get; set; }
        public virtual DbSet<us_call_log_export> UsCallLogExport { get; set; }
        public virtual DbSet<Us_call_log_category> UsCallLogCategory { get; set; }
        //public virtual DbSet<us_call_logs_categories> UsCallLogsCategories { get; set;}
        // Look in View from database
        public virtual DbSet<Cust_product_details> CustProductDetails { get; set; }
        public virtual DbSet<inspections_list2> InspectionList2{ get; set; }
        
        public virtual DbSet<Return_category> ReturnCategory { get; set; }
        public virtual DbSet<returns_events> ReturnsEvents { get; set; }

        public virtual DbSet<Order_line_detail2_v6> OrderLineDetailView { get; set; }
            
        //public virtual DbSet<Ca_reasons> CaReasons{ get; set; }


        //public virtual DbSet<Claims_report> ClaimsReportData { get; set; }
        //public virtual DbSet<OrderMgtmDetail> OrderMgtmDetails { get; set; }
        //public virtual DbSet<Brand_sales_analysis2> BrandSalesAnalysis { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Web_site>()
                        .HasOptional(s => s.Brand)
                        .WithMany(b => b.Sites)
                        .HasForeignKey(s => s.brand_id);
            modelBuilder.Entity<Brand>().HasKey(b => b.brand_id);

            modelBuilder.Entity<User_comment>().HasKey(u => u.comment_id);
            modelBuilder.Entity<Client_sales_data>().HasKey(d => d.id);
            modelBuilder.Entity<Sales_data>().HasKey(s => s.sales_unique);
            modelBuilder.Entity<Catdop_characteristics>().HasKey(c => c.characteristic_id);
            modelBuilder.Entity<Sale>().HasKey(s => s.IdSale);
            modelBuilder.Entity<Sale>().ToTable("Sale");

            modelBuilder.Entity<sales_forecast_contract>()
                .HasOptional(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.cprod_id);

            modelBuilder.Entity<Sales_forecast>().HasOptional(s => s.Product).WithMany().HasForeignKey(s => s.cprod_id);
            modelBuilder.Entity<Sales_forecast_amendments>()
                .HasOptional(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.cprod_id);

            modelBuilder.Entity<Qc_comments>()
                .HasOptional(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.insp_comments_from);
            modelBuilder.Entity<Qc_comments>()
                .HasOptional(c => c.Inspection)
                .WithMany()
                .HasForeignKey(c => c.insp_unique);
            modelBuilder.Entity<Qc_comments>()
                .HasOptional(c => c.InspectionV2)
                .WithMany()
                .HasForeignKey(c => c.inspv2_id);

            modelBuilder.Entity<BrandCategory>().ToTable("brand_categories");


            //modelBuilder.Entity<Stock_summary_report>().ToTable("stock_summary_report2");

            modelBuilder.Entity<BudgetActualData>().ToTable("BudgetActualData");
            modelBuilder.Entity<BudgetActualData>().HasOptional(b => b.Brand).WithMany().HasForeignKey(b => b.brand_id);
            modelBuilder.Entity<BudgetActualData>().HasOptional(b => b.Distributor).WithMany().HasForeignKey(b => b.distributor_id);

            modelBuilder.Entity<Change_notice>()
                .HasMany(n => n.Allocations)
                .WithOptional(a => a.Notice)
                .HasForeignKey(a => a.notice_id);
            modelBuilder.Entity<Change_notice_allocation>()
                .HasOptional(a => a.Product)
                .WithMany()
                .HasForeignKey(a => a.cprod_id);
            modelBuilder.Entity<Change_notice_allocation>().HasMany(a => a.Orders).WithMany().Map(m => m.ToTable("change_notice_allocation_order").MapLeftKey("change_notice_allocation_id").MapRightKey("orderid"));
                
            modelBuilder.Entity<change_notice_reasons>().ToTable("change_notice_reasons");

            modelBuilder.Entity<Change_notice>()
                .HasMany(n => n.Images)
                .WithRequired(c => c.ChangeNotice)
                .HasForeignKey(c => c.notice_id);

            modelBuilder.Entity<change_notice_image>()
                .HasRequired(t => t.ImageType)
                .WithMany()
                .HasForeignKey(t => t.type_id);

            modelBuilder.Entity<Change_notice>()
                .HasOptional(n => n.Document)
                .WithRequired(d => d.ChangeNotice);
            modelBuilder.Entity<Change_notice>()
                .HasOptional(n => n.Category)
                .WithMany().HasForeignKey(n => n.categoryId);
            modelBuilder.Entity<Change_notice>()
                .HasOptional(n => n.Reason)
                .WithMany().HasForeignKey(n => n.reason_id);

            modelBuilder.Entity<Us_call_log>().HasOptional(d => d.User).WithMany().HasForeignKey(d => d.userid);
            modelBuilder.Entity<Us_call_log>().HasMany(i => i.Images).WithOptional().HasForeignKey(i => i.log_id);
            modelBuilder.Entity<Us_call_log_images>().ToTable("us_call_log_images");
            modelBuilder.Entity<Web_slide>().ToTable("web_slide");

            modelBuilder.Entity<inspection_lines_notified_v2_table>().ToTable("2012_inspection_lines_notified_v2_table");
            modelBuilder.Entity<inspection_notified_summary_table>().ToTable("2012_inspection_notified_summary_table");
            modelBuilder.Entity<inspection_notified_summary_images_table>()
                .ToTable("2012_inspection_notified_summary_images_table");
            modelBuilder.Entity<inspection_images_v2_table>().ToTable("2012_inspection_images_v2_table");

            modelBuilder.Entity<BrandCategory>().HasOptional(c => c.Group).WithMany().HasForeignKey(c => c.group_id);

            modelBuilder.Entity<Stocksummary_factoryvalue>()
                .HasOptional(s => s.Factory)
                .WithMany()
                .HasForeignKey(s => s.factory_id);

            modelBuilder.Entity<change_notice_table>().ToTable("2011_change_notice_table");

            modelBuilder.Entity<change_notice_product_table>().ToTable("2011_change_notice_product_table")
                .HasOptional(c => c.MastProduct).WithMany().HasForeignKey(c => c.mastid);
            modelBuilder.Entity<change_notice_product_table>()
                .HasOptional(c => c.ChangeNotice)
                .WithMany()
                .HasForeignKey(c => c.cn_id);


            modelBuilder.Entity<AutomatedTask>().ToTable("automated_task").HasMany(t=>t.Attachments).WithOptional(a=>a.Task).HasForeignKey(a=>a.task_id);
            modelBuilder.Entity<AutomatedTask>()
                .HasOptional(t => t.Iterator)
                .WithMany()
                .HasForeignKey(t => t.iterator_id);
            modelBuilder.Entity<AutomatedTaskIterator>().ToTable("automatedtask_iterator");


            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Role>().HasMany(r => r.Permissions).WithMany().Map(m => m.MapLeftKey("role_id").MapRightKey("permission_id").ToTable("role_permission"));

            modelBuilder.Entity<AsaqColor>().ToTable("color");

            modelBuilder.Entity<Web_product_component_details>().ToTable("Web_product_component_details");
            modelBuilder.Entity<Web_product_component_details>().HasKey(d => new {d.web_unique, d.cprod_id});

            //modelBuilder.Entity<Whitebook_category>().ToTable("Whitebook_category");
            
            modelBuilder.Entity<us_display_orders>().ToTable("us_display_orders");
            modelBuilder.Entity<Us_dealers>().ToTable("us_dealers");
            modelBuilder.Entity<Us_dealers>().Property(d => d._internal).HasColumnName("internal");
            modelBuilder.Entity<Us_dealers>().HasMany(d => d.Sales_orders).WithOptional(o => o.Dealer).HasForeignKey(o => o.customer);
            modelBuilder.Entity<Ussales_log>().ToTable("Ussales_log");

            modelBuilder.Entity<Us_call_log>().HasOptional(d => d.UsDealer).WithMany(d => d.CallLogs).HasForeignKey(d=>d.dealer);

            modelBuilder.Entity<Us_call_log>().HasOptional(d => d.User).WithMany().HasForeignKey(d => d.userid);
            modelBuilder.Entity<Us_call_log>().HasOptional(d => d.Category).WithMany().HasForeignKey(d => d.category_id);
            modelBuilder.Entity<Us_call_log>().HasMany(d => d.Categories).WithMany().Map(m => m.ToTable("us_call_logs_categories").MapLeftKey("us_call_log_id").MapRightKey("us_call_log_category_id"));

            modelBuilder.Entity<Us_call_log_category>().ToTable("us_call_log_category");
            
            modelBuilder.Entity<inspections_list2>().ToTable("inspections_list2");
            modelBuilder.Entity<us_call_log_export>().ToTable("us_call_log_export");
            //modelBuilder.Entity<us_call_logs_categories>().ToTable("us_call_logs_categories");

            modelBuilder.Entity<sales_orders_monthly_data>().ToTable("sales_orders_monthly_data");
            modelBuilder.Entity<products_track_number_qc>().ToTable("2012_products_track_number_qc");
            modelBuilder.Entity<products_track_number_qc>().HasOptional(p => p.MastProduct).WithMany().HasForeignKey(p => p.mastid);

            modelBuilder.Entity<cust_product_file>().ToTable("cust_product_files").HasKey(c => c.id).HasRequired(c => c.CustProductsFileType);

            modelBuilder.Entity<Distributor_sales>().ToTable("distributor_sales").HasKey(d => d.id).HasRequired(d => d.Brand).WithMany().HasForeignKey(d => d.brand_id);
            modelBuilder.Entity<Distributor_sales>().HasRequired(d => d.Distributor).WithMany().HasForeignKey(d => d.distributor_id);

            modelBuilder.Entity<Inspection_v2_mixedpallet>().HasOptional(p => p.Area).WithMany().HasForeignKey(p => p.area_id);
            modelBuilder.Entity<Inspection_v2_loading_mixedpallet>().HasOptional(mp => mp.Pallet).WithMany().HasForeignKey(mp => mp.pallet_id);

            modelBuilder.Entity<ProductComponent>().HasMany(c => c.MastProductComponents).WithOptional(m => m.Component).HasForeignKey(m => m.component_id);
            modelBuilder.Entity<Exchange_rates>().ToTable("exchange_rates");

            modelBuilder.Entity<Client_pages_allocated>().HasOptional(c => c.Page).WithMany().HasForeignKey(c => c.page_id);
            modelBuilder.Entity<Admin_pages_new>().HasMany(p => p.Children).WithOptional().HasForeignKey(p => p.parent_id);
            modelBuilder.Entity<Login_history>().ToTable("login_history");
            modelBuilder.Entity<Login_history>().HasOptional(l => l.Company).WithMany().HasForeignKey(l => l.user_id);
            modelBuilder.Entity<Login_history_detail>().ToTable("login_history_detail");
            modelBuilder.Entity<Login_history_detail>().HasOptional(d => d.Header).WithMany().HasForeignKey(d => d.history_id);
            modelBuilder.Entity<Image_type>().ToTable("Image_type");
            modelBuilder.Entity<Web_product_pressure>().ToTable("Web_product_pressure");
            modelBuilder.Entity<Web_product_part>().ToTable("Web_product_part");
            modelBuilder.Entity<Web_product_file_type>().ToTable("Web_product_file_type");
            modelBuilder.Entity<File_type>().ToTable("file_type");
            modelBuilder.Entity<Inspection_v2_si_subject>().ToTable("Inspection_v2_si_subject");

            modelBuilder.Entity<process>().ToTable("2011_processes");
            modelBuilder.Entity<BrochureDownloadLog>().ToTable("brochuredownloadlog");
            modelBuilder.Entity<Ca_reasons>().ToTable("ca_reasons");
            //modelBuilder.Entity<tc_document>().ToTable("tc_document");

            modelBuilder.Entity<UserGroup>().ToTable("usergroup");

            modelBuilder.Entity<Log_functions>()
                .ToTable("log_functions")
                .HasOptional(l => l.OrderHeader)
                .WithMany()
                .HasForeignKey(l => l.log_orderid);

            modelBuilder.Entity<nr_type>().ToTable("nr_type");
            modelBuilder.Entity<Ip2>().ToTable("Ip2");

            modelBuilder.Entity<returns_decision>().ToTable("returns_decision");
            modelBuilder.Entity<Sales_data_contracts>().ToTable("sales_data_contracts");
            modelBuilder.Entity<Sales_data_contracts>().HasKey(s => s.sales_unique);
                        
            modelBuilder.Entity<Form_choice>().ToTable("form_choice");

            modelBuilder.Entity<Language>().ToTable("languages");
            modelBuilder.Entity<Language>().HasMany(l => l.Countries).WithMany().Map(m => m.ToTable("language_country").MapLeftKey("language_id").MapRightKey("country_id"));

            modelBuilder.Entity<instructions_new>().ToTable("instructions_new");
            modelBuilder.Entity<instructions_new>().HasMany(i => i.Products).WithMany().Map(m => m.ToTable("instruction_mastproduct").MapLeftKey("instruction_id").MapRightKey("mast_id"));
            modelBuilder.Entity<instructions_new>().HasOptional(i => i.Language).WithMany().HasForeignKey(i => i.language_id);
            modelBuilder.Entity<instructions_new>().HasOptional(i => i.CreatedBy).WithMany().HasForeignKey(i => i.created_by);

			modelBuilder.Entity<Market_product>().ToTable("pp_market_product");
			modelBuilder.Entity<Market_product>().HasKey(mp => new { mp.cprod_id, mp.market_id });

			modelBuilder.Entity<ProductPricingMastProductData>().ToTable("pp_mastproduct");
			modelBuilder.Entity<ProductPricingProjectSettings>().ToTable("pp_project_setting");
			modelBuilder.Entity<ProductPricingProjectSettings>().HasKey(p => new { p.project_id, p.setting_id });

			modelBuilder.Entity<sales_orders_brand>().ToTable("sales_orders_brand");
			modelBuilder.Entity<cw_site>().ToTable("cw_site");

			//analytics reports
			modelBuilder.Entity<Ar_report>().ToTable("ar_report");
			modelBuilder.Entity<Ar_section>().ToTable("ar_section");
			modelBuilder.Entity<Ar_audience>().ToTable("ar_audience");
			modelBuilder.Entity<Ar_user>().ToTable("ar_user");
			modelBuilder.Entity<Ar_report_section>().ToTable("ar_report_section");
			modelBuilder.Entity<Ar_report_section>().HasKey(r => new { r.report_id, r.section_id });
			modelBuilder.Entity<Ar_report_section>().HasRequired(rs => rs.Section).WithMany().HasForeignKey(rs => rs.section_id);

			modelBuilder.Entity<Ar_report>().HasMany(r => r.Sections).WithOptional().HasForeignKey(s => s.report_id);
			modelBuilder.Entity<Ar_section>().HasMany(s => s.Audiences).WithMany().Map(m => m.ToTable("ar_section_audience").MapLeftKey("section_id").MapRightKey("audience_id"));
			modelBuilder.Entity<Ar_audience>().HasMany(a => a.Users).WithMany().Map(m => m.ToTable("ar_audience_user").MapLeftKey("audience_id").MapRightKey("user_id"));



			modelBuilder.Configurations.Add(new DealerMappings());
            modelBuilder.Configurations.Add(new DealerImageMappings());
            modelBuilder.Configurations.Add(new Dealer_BrandstatusMappings());
            modelBuilder.Configurations.Add(new DealerImageDisplayMappings());
            modelBuilder.Configurations.Add(new WebProductMappings());
            modelBuilder.Configurations.Add(new WebCategoryMappings());
            //modelBuilder.Configurations.Add(new Mappings.WebProductsRelatedMappings());
            modelBuilder.Configurations.Add(new WebProductComponentMappings());
            modelBuilder.Configurations.Add(new WebProductFileMappings());
            modelBuilder.Configurations.Add(new CustProductMapping());
            modelBuilder.Configurations.Add(new MastProductMapping());
            modelBuilder.Configurations.Add(new MastProductCharMapping());
            modelBuilder.Configurations.Add(new OrderLinesMappings());
            modelBuilder.Configurations.Add(new OrderHeaderMappings());
            modelBuilder.Configurations.Add(new UserMappings());
            modelBuilder.Configurations.Add(new CompanyMappings());
            modelBuilder.Configurations.Add(new AdminPermissionsMappings());
            modelBuilder.Configurations.Add(new POrderHeaderMappings());
            modelBuilder.Configurations.Add(new POrderLineMappings());
            modelBuilder.Configurations.Add(new CurrencyMapping());
            modelBuilder.Configurations.Add(new ReturnsMapping());
            modelBuilder.Configurations.Add(new ReturnCommentMapping());
            modelBuilder.Configurations.Add(new ReturnCommentFileMapping());
            modelBuilder.Configurations.Add(new DistProductMapping());
            modelBuilder.Configurations.Add(new ExternalDealerMappings());
            modelBuilder.Configurations.Add(new DealerExternalDisplayMappings());
            modelBuilder.Configurations.Add(new DealerExternalCommentMappings());
            modelBuilder.Configurations.Add(new InspectionCriteriaMappings());
            modelBuilder.Configurations.Add(new InspectionCriteriaTemplateMappings());
            modelBuilder.Configurations.Add(new InspectionCustomCriteriaMappings());
            modelBuilder.Configurations.Add(new MeetingMappings());
            modelBuilder.Configurations.Add(new MeetingDetailsMappings());
            modelBuilder.Configurations.Add(new MeetingDetailsResponsibilitiesMappings());
            modelBuilder.Configurations.Add(new InspectionV2Mappings());
            modelBuilder.Configurations.Add(new InspectionV2_Line_Mappings());
            modelBuilder.Configurations.Add(new InspectionV2_Container_Mappings());
            modelBuilder.Configurations.Add(new InspectionV2_Controller_Mappings());
            modelBuilder.Configurations.Add(new InspectionV2_Loading_Mappings());
            modelBuilder.Configurations.Add(new InspectionV2_Image_Mappings());
            modelBuilder.Configurations.Add(new InspectionV2_Line_Sidetails_Mappings());
            modelBuilder.Configurations.Add(new StockAllocationMappings());
            modelBuilder.Configurations.Add(new FactoryOrderMapping());
            modelBuilder.Configurations.Add(new FactoryOrderLinesMapping());
            modelBuilder.Configurations.Add(new NavigationItemMappings());
            modelBuilder.Configurations.Add(new NavigationItemPermissionMappings());
            modelBuilder.Configurations.Add(new TechnicalDataMappings());
            modelBuilder.Configurations.Add(new TechnicalSubcategoryTemplateMappings());
            modelBuilder.Configurations.Add(new FeedbackSubscriptionMapping());
            modelBuilder.Configurations.Add(new WebProductSearchMapping());
            modelBuilder.Configurations.Add(new ConfigSettingMappings());
            modelBuilder.Configurations.Add(new NRHeaderMappings());
            modelBuilder.Configurations.Add(new NRLinesMappings());
            modelBuilder.Configurations.Add(new NrLineImagesMappings());
            modelBuilder.Configurations.Add(new InspectionsMappings());
            modelBuilder.Configurations.Add(new InspectionLinesTestedMappings());
            modelBuilder.Configurations.Add(new Whitebook_template_mappings());
            modelBuilder.Configurations.Add(new Whitebook_optiongroup_mappings());
            modelBuilder.Configurations.Add(new Whitebook_template_optiongroup_mappings());
            modelBuilder.Configurations.Add(new Whitebook_category_mappings());
            modelBuilder.Configurations.Add(new Whitebook_template_category_mappings());
            modelBuilder.Configurations.Add(new InvoiceMappings());
            modelBuilder.Configurations.Add(new Whitebook_option_mappings());
            modelBuilder.Configurations.Add(new TrainingDocumentMapping());
            modelBuilder.Configurations.Add(new CustProductBundleMapping());
            modelBuilder.Configurations.Add(new CustProductBundleComponentMapping());
            modelBuilder.Configurations.Add(new SalesOrdersMappings());
            modelBuilder.Configurations.Add(new BackOrdersMappings());
            modelBuilder.Configurations.Add(new SalesOrdersHeadersMappings());
            modelBuilder.Configurations.Add(new SalesOrdersHeadersShippingMappings());
            modelBuilder.Configurations.Add(new FeedbackAuthorizationMapping());
            modelBuilder.Configurations.Add(new ManualV2Mappings());
            modelBuilder.Configurations.Add(new ManualV2NodeMappings());
            modelBuilder.Configurations.Add(new ManualV2MessageMappings());
            modelBuilder.Configurations.Add(new ManualV2EditHistoryRecordsMappings());

            modelBuilder.Configurations.Add(new FormMappings());
            modelBuilder.Configurations.Add(new FormSectionMappings());
            modelBuilder.Configurations.Add(new FormChoiceGroupMappings());
            modelBuilder.Configurations.Add(new FormQuestionGroupMappings());
            modelBuilder.Configurations.Add(new FormQuestionTypeMappings());
            modelBuilder.Configurations.Add(new FormQuestionMappings());
            modelBuilder.Configurations.Add(new FormSectionQuestionMappings());
            modelBuilder.Configurations.Add(new FormSubmissionAnswerMappings());
            modelBuilder.Configurations.Add(new FormSubmissionMappings());
            modelBuilder.Configurations.Add(new FormFormSectionMappings());
            modelBuilder.Configurations.Add(new FormQuestionGroupQuestionMappings());
            modelBuilder.Configurations.Add(new FormQuestionAnswerMappings());
            modelBuilder.Configurations.Add(new ContainerCalculationProductMappings());
            modelBuilder.Configurations.Add(new ContainerCalculationMappings());

            modelBuilder.Configurations.Add(new Returns_qcusers_mappings());
            modelBuilder.Configurations.Add(new ReturnEventMappings());
			modelBuilder.Configurations.Add(new ProductPricingModelLevelMapping());
			modelBuilder.Configurations.Add(new ProductPricingModelMapping());
			modelBuilder.Configurations.Add(new FreightCostMapping());
			modelBuilder.Configurations.Add(new MarketMapping());
			modelBuilder.Configurations.Add(new ProductPricingSettingsMappings());
			modelBuilder.Configurations.Add(new ProductPricingProjectMapping());
			modelBuilder.Configurations.Add(new CWProductMappings());
			modelBuilder.Configurations.Add(new CWComponentMappings());

			modelBuilder.Entity<Cw_feature>().ToTable("cw_feature");
			modelBuilder.Entity<Cw_component_feature>().HasOptional(cf => cf.Feature).WithMany().HasForeignKey(cf => cf.feature_id);
						
		}

        public static Model CreateModel()
        {
            var m = new Model();
            m.Configuration.LazyLoadingEnabled = false;
            m.Configuration.ProxyCreationEnabled = false;
            m.Database.CommandTimeout = 300;
            return m;
        }

    }

    
}
