using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.DAL.EF.Repositories;
using erp.DAL.EF.Repositories.Order;
using erp.DAL.EF.Repositories.Invoice;
using erp.DAL.EF.Repositories.InspectionV2;
using erp.Model;

namespace erp.DAL.EF
{
	public class UnitOfWork : IDisposable
	{
		private Model context = null;

		private static Model CreateModel()
		{
			var model = new Model();
			model.Configuration.ProxyCreationEnabled = false;
			model.Configuration.LazyLoadingEnabled = false;
            model.Database.CommandTimeout = 300;
			return model;
		}

	    public UnitOfWork()
	    {
	        context = CreateModel();
	    }

	    public UnitOfWork(Model context)
	    {
	        this.context = context;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
            context.Database.CommandTimeout = 300;
        }

		private GenericRepository<Category1_sub> category1SubRepository;
		private SparesRepository spareRepository;
		private MastProductRepository mastProductRepository;
		private CustProductRepository custProductRepository;
		private InspectionRepository inspectionRepository;
		private OrderLinesRepository orderLinesRepository;
		private ReturnRepository returnRepository;
		private WebProductRepository webProductRepository;
		private GenericRepository<Webproduct_search> webProductSearchRepository;
		private WebCategoryRepository webCategoryRepository;
		private GenericRepository<ConfigSetting> configSettingGenericRepository;
		private InspectionV2Repository inspectionV2Repository;
		private CompanyRepository companyRepository;
		private OrderRepository orderRepository;
		private NrHeaderRepository nrHeaderRepository;
		private GenericRepository<Nr_image_type> nrImageTypeRepository;
		private GenericRepository<Web_product_component_details> webProductComponentDetailsRepository;
		private GenericRepository<Whitebook_option> whiteBookOptionRepository;
        private GenericRepository<Whitebook_option_group> whiteBookOptionGroupRepository;
        private WhitebookCategoryRepository whitebookCategoryRepository;
		private GenericRepository<Whitebook_template_category> whitebookTemplateCategoryRepository;
		private GenericRepository<Whitebook_template> whitebookTemplateRepository;
        private GenericRepository<Returns_qcusers> returnsUserUserRepository;
        private SalesOrdersRepository salesOrdersRepository;
        private SalesDisplaysRepository salesDisplaysRepository;
        private GenericRepository<sales_orders_monthly_data> salesOrdersMonthlyRepository;
        private UserRepository userRepository;
	    private DealerRepository dealerRepository;
        private ProductTrackNumbersRepository trackNumberQcRepository;
        private GenericRepository<cust_product_file> custProductFileRepository;
        private GenericRepository<Sales_data> salesDataRepository;
        private GenericRepository<Distributor_sales> distSalesRepository;
        private InvoiceRepository invoicesRepository;
        private GenericRepository<Qc_comments> qcCommentsRepository;
        private GenericRepository<cust_products_range> custProductRangeRepository;
        private GenericRepository<us_display_orders> displayOrdersRepository;
        private GenericRepository<Us_dealers> usDealersRepository;
        private TcDocumentRepository tcDocumentRepository;
        private UsCallLogRepository usCallLogRepository;
        private GenericRepository<Us_call_log_images> usCallLogImageRepository;
       
        private GenericRepository<Us_call_log_category> usCallLogCategoryRepository;
        private GenericRepository<inspections_list2> inspectionList2Repository;

        private GenericRepository<tp2_sales_export_all> tp2SalesExportAll;
        private GenericRepository<Sales_forecast> salesForecastRepository;
        private GenericRepository<us_call_log2> us_call_log2Repository;
        private GenericRepository<us_call_log_export> us_call_log_exportRepository;

        private GenericRepository<MastProduct_Component> mastProdComponentsRepository;
        private GenericRepository<Exchange_rates> exchangeRateRepository;
        private GenericRepository<Brand> brandRepository;
        private GenericRepository<Web_site> webSiteRepository;
        private GenericRepository<Image_type> imageTypeRepository;
        private WebProductFileTypeRepository webProductFileTypeRepository;
        private GenericRepository<Web_product_pressure> webProductPressureRepository;
        private GenericRepository<Web_product_part> webProductPartRepository;
        private GenericRepository<Sale> saleRepository;
        private GenericRepository<Login_history> loginHistoryRepository;
        private GenericRepository<Login_history_detail> loginHistoryDetailRepository;
        private GenericRepository<Admin_pages> adminPagesRepository;
        private GenericRepository<Admin_pages_new> adminPagesNewRepository;
        private GenericRepository<Client_pages_allocated> clientPagesRepository;
        private GenericRepository<Inspection_v2_si_subject> inspectionSubjectRepository;
        private GenericRepository<Inspection_v2_image_type> inspectionImageTypeRepository;
        private BudgetRepository budgetActualDataRepository;
        private GenericRepository<cust_products_bundle> custProductBundleRepository;
        private SalesOrdersHeadersRepository salesOrderHeaderRepository;
        private GenericRepository<Sales_orders_headers_shipping> salesOrderHeaderShippingRepository;
        private GenericRepository<Us_backorders> backOrdersRepository;
        private UsSalesLogRepository usSalesLogRepository;
        private GenericRepository<Cust_product_details> custProductDetailRepository;
        private GenericRepository<Cust_product_files> custProductFilesRepository;
        private GenericRepository<ds_guarantee_registration> digitalShowerGuaranteeRegistration;
	    private GenericRepository<inspection_lines_notified_v2_table> inspLinesNotifiedRepository;
	    private GenericRepository<Container_types> containerTypeRepository;
	    private GenericRepository<process> processRepository;
	    private GenericRepository<BrochureDownloadLog> brochureDownloadLogRepository;
        private GenericRepository<Ca_reasons> caReasonsRepository;
	    private GenericRepository<Log_functions> logFunctionsRepository;
        private GenericRepository<Return_category> returnCategoryRepository;
        private GenericRepository<change_notice_reasons> changeNoticeReasonsRepository;
        private ChangeNoticeRepository changeNoticeRepository;
        private GenericRepository<nr_type> nrTypeRepository;
        private GenericRepository<Ip2> ip2Repository;
        //private GenericRepository<manual> manualV2Repository;
        private GenericRepository<feedback_issue_type> feedbackIssueTypeRepository;
        private GenericRepository<feedback_authorization> feedbackAuthorizationRepository;
        private GenericRepository<UserGroup> userGroupRepository;

        private GenericRepository<Form> formRepository;
        private GenericRepository<Form_question_type> formQuestionTypeRepository;
        private GenericRepository<Form_question_rendermethod> formQuestionRenderMethodRepository;
        private FormSubmissionRepository formSubmissionRepository;
        private ContainerCalculationRepository containerCalcRepository;
        private GenericRepository<Sales_data_contracts> salesDataContractsRepository;
        private InstructionsRepository instructionsRepository;
        private GenericRepository<returns_events> returnsEventsRepository;
		private GenericRepository<ProductPricing_settings> productPricingSettingsRepository;
		private ProductPricingModelRepository productPricingModelRepository;
		private GenericRepository<Freightcost> freightCostRepository;
		private GenericRepository<Market> marketRepository;
		private ProductPricingProjectRepository productPricingProjectRepository;
		private GenericRepository<sales_orders_brand> salesOrderBrandRepository;
        private ManualV2Repository manualV2Repository;
		private GenericRepository<Cw_product> cwProductRepository;
		private GenericRepository<Cw_feature> cwFeatureRepository;
		private GenericRepository<Cw_component> cwComponentRepository;
		private GenericRepository<cw_site> cwSiteRepository;
		private GenericRepository<Cw_product_file> cwProductFileRepository;
		private GenericRepository<Ar_report> arReportRepository;
        private GenericRepository<Factory_client_settings> factoryClientSettings;
        private GenericRepository<Order_line_detail2_v6> orderLineViewRepository;
        private GenericRepository<Web_slide> webSlideRepository;
        private GenericRepository<Inspection_v2_image> inspectionV2ImageRepository;
        
        public GenericRepository<Category1_sub> Category1SubRepository 
		{
			get
			{

				if (this.category1SubRepository == null) {
					this.category1SubRepository = new GenericRepository<Category1_sub>(context);
				}
				return category1SubRepository;
			}
		}

		public InspectionRepository InspectionRepository
		{
			get
			{
				if (this.inspectionRepository == null) {
					this.inspectionRepository = new InspectionRepository(context);
				}
				return inspectionRepository;
			}
		}

		public InspectionV2Repository InspectionV2Repository
		{
			get
			{
				if (this.inspectionV2Repository == null) {
					this.inspectionV2Repository = new InspectionV2Repository(context);
				}
				return inspectionV2Repository;
			}
		}

		public OrderLinesRepository OrderLinesRepository
		{
			get
			{
				if (this.orderLinesRepository == null) {
					this.orderLinesRepository = new OrderLinesRepository(context);
				}
				return orderLinesRepository;
			}
		}

		public SparesRepository SpareRepository
		{
			get
			{

				if (this.spareRepository == null) {
					this.spareRepository = new SparesRepository(context);
				}
				return spareRepository;
			}
		}

		public MastProductRepository MastProductRepository
		{
			get
			{
				if (this.mastProductRepository == null) {
					this.mastProductRepository = new MastProductRepository(context);
				}
				return mastProductRepository;
			}
		}

		public CustProductRepository CustProductRepository
		{
			get
			{
				if (this.custProductRepository == null) {
					this.custProductRepository = new CustProductRepository(context);
				}
				return custProductRepository;
			}
		}

		public ReturnRepository ReturnRepository
		{
			get
			{
				if (this.returnRepository == null) {
					this.returnRepository = new ReturnRepository(context);
				}
				return returnRepository;
			}
		}

		public WebProductRepository WebProductRepository
		{
			get
			{
				if (this.webProductRepository == null) {
					this.webProductRepository = new WebProductRepository(context);
				}
				return webProductRepository;
			}
		}

		public GenericRepository<Webproduct_search> WebProductSearchRepository
		{
			get
			{
				if (this.webProductSearchRepository == null) {
					this.webProductSearchRepository = new GenericRepository<Webproduct_search>(context);
				}
				return webProductSearchRepository;
			}
		}

		public WebCategoryRepository WebCategoryRepository
		{
			get
			{
				if (this.webCategoryRepository == null) {
					this.webCategoryRepository = new WebCategoryRepository(context);
				}
				return webCategoryRepository;
			}
		}

		public GenericRepository<ConfigSetting> ConfigSettingGenericRepository
		{
			get
			{
				if (this.configSettingGenericRepository == null) {
					this.configSettingGenericRepository = new GenericRepository<ConfigSetting>(context);
				}
				return configSettingGenericRepository;
			}
		}

		public CompanyRepository CompanyRepository
		{
			get
			{
				if(companyRepository == null)
					companyRepository = new CompanyRepository(context);
				return companyRepository;
			}
		}

		public OrderRepository OrderRepository
		{
			get
			{
				if(orderRepository == null)
					orderRepository = new OrderRepository(context);
				return orderRepository;
			}
		}

		public GenericRepository<Nr_image_type> NrImageTypeRepository
		{
			get {
				return nrImageTypeRepository ?? (nrImageTypeRepository = new GenericRepository<Nr_image_type>(context));
			}
		}


		public NrHeaderRepository NrHeaderRepository
		{
			get
			{
				if(nrHeaderRepository == null)
					nrHeaderRepository = new NrHeaderRepository(context);
				return nrHeaderRepository;
			}
		}

		public GenericRepository<Web_product_component_details> WebProductComponentDetailsRepository
		{
			get
			{
				if (webProductComponentDetailsRepository == null)
					webProductComponentDetailsRepository = new GenericRepository<Web_product_component_details>(context);
				return webProductComponentDetailsRepository;
			}
		}

		public WhitebookCategoryRepository WhiteBookCategoryRepository
		{
			get
			{
				if (whitebookCategoryRepository == null)
					whitebookCategoryRepository = new WhitebookCategoryRepository(context);
				return whitebookCategoryRepository;
			}
		}
		public GenericRepository<Whitebook_template_category> WhiteBookTemplateCategoryRepository
		{
			get
			{
				if (whitebookTemplateCategoryRepository == null)
					whitebookTemplateCategoryRepository = new GenericRepository<Whitebook_template_category>(context);
				return whitebookTemplateCategoryRepository;
			}
		}
		public GenericRepository<Whitebook_template> WhiteBookTemplateRepository
		{
			get
			{
				if (whitebookTemplateRepository == null)
					whitebookTemplateRepository = new GenericRepository<Whitebook_template>(context);
				return whitebookTemplateRepository;
			}
		}

        public GenericRepository<Whitebook_option> WhiteBookOptionRepository
        {
            get
            {
                if (whiteBookOptionRepository == null)
                    whiteBookOptionRepository = new GenericRepository<Whitebook_option>(context);
                return whiteBookOptionRepository;
            }
        }

        public GenericRepository<Whitebook_option_group> WhiteBookOptionGroupRepository
        {
            get
            {
                if (whiteBookOptionGroupRepository == null)
                    whiteBookOptionGroupRepository = new GenericRepository<Whitebook_option_group>(context);
                return whiteBookOptionGroupRepository;
            }
        }

        public GenericRepository<Returns_qcusers> ReturnsUserUserRepository
		{
			get
			{
				if (returnsUserUserRepository == null)
					returnsUserUserRepository = new GenericRepository<Returns_qcusers>(context);
				return returnsUserUserRepository;
			}
		}

        public SalesOrdersRepository SalesOrdersRepository
        {
            get
            {
                if (salesOrdersRepository == null)
                    salesOrdersRepository = new SalesOrdersRepository(context);
                return salesOrdersRepository;
            }
        }

        public SalesDisplaysRepository SalesDisplaysRepository
        {
            get
            {
                if (salesDisplaysRepository == null)
                    salesDisplaysRepository = new SalesDisplaysRepository(context);
                return salesDisplaysRepository;
            }
        }

        public GenericRepository<sales_orders_monthly_data> SalesOrdersMonthlyRepository
        {
            get
            {
                if (salesOrdersMonthlyRepository == null)
                    salesOrdersMonthlyRepository = new GenericRepository<sales_orders_monthly_data>(context);
                return salesOrdersMonthlyRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(context);
                return userRepository;
            }
        }

        public DealerRepository DealerRepository
        {
            get
            {
                if (dealerRepository == null)
                    dealerRepository = new DealerRepository(context);
                return dealerRepository;
            }
        }

        public ProductTrackNumbersRepository TrackNumberQcRepository
        {
            get
            {
                if (trackNumberQcRepository == null)
                    trackNumberQcRepository = new ProductTrackNumbersRepository(context);
                return trackNumberQcRepository;
            }

            set
            {
                trackNumberQcRepository = value;
            }
        }

        public GenericRepository<cust_product_file> CustProductFileRepository
        {
            get
            {
                if (custProductFileRepository == null)
                {
                    custProductFileRepository = new GenericRepository<cust_product_file>(context);
                }
                return custProductFileRepository;
            }
        }

        public GenericRepository<Sales_data> SalesDataRepository
        {
            get
            {
                if (salesDataRepository == null) {
                    salesDataRepository = new GenericRepository<Sales_data>(context);
                }
                return salesDataRepository;
            }
        }

        public GenericRepository<Distributor_sales> DistSalesRepository
        {
            get
            {
                if (distSalesRepository == null) {
                    distSalesRepository = new GenericRepository<Distributor_sales>(context);
                }
                return distSalesRepository;
            }
        }

        public InvoiceRepository InvoicesRepository
        {
            get
            {
                if (invoicesRepository == null) {
                    invoicesRepository = new InvoiceRepository(context);
                }
                return invoicesRepository;
            }
        }

        public GenericRepository<Qc_comments> QcCommentsRepository
        {
            get
            {
                if (qcCommentsRepository == null) {
                    qcCommentsRepository = new GenericRepository<Qc_comments>(context);
                }
                return qcCommentsRepository;
            }
        }

        public GenericRepository<cust_products_range> CustProductRangeRepository
        {
            get
            {
                if (custProductRangeRepository == null)
                    custProductRangeRepository = new GenericRepository<cust_products_range>(context);
                return custProductRangeRepository;
            }
        }

        public GenericRepository<us_display_orders> DisplayOrdersRepository
        {
            get
            {
                if (displayOrdersRepository == null)
                    displayOrdersRepository = new GenericRepository<us_display_orders>(context);
                return displayOrdersRepository;
            }
        }

        public GenericRepository<Us_dealers> UsDealersRepository
        {
            get
            {
                if (usDealersRepository == null)
                    usDealersRepository = new GenericRepository<Us_dealers>(context);
                return usDealersRepository;
            }
        }

        public TcDocumentRepository TcDocumentRepository
        {
            get
            {
                if (tcDocumentRepository == null)
                    tcDocumentRepository = new TcDocumentRepository(context);
                return tcDocumentRepository;
            }


        }
        public UsCallLogRepository UsCallLogRepository
        {
            get
            {
                if (usCallLogRepository == null)
                    usCallLogRepository = new UsCallLogRepository(context);
                return usCallLogRepository;
            }
        }

        public GenericRepository<Us_call_log_category> UsCallLogCategoryRepository
        {
            get
            {
                if (usCallLogCategoryRepository == null)
                    usCallLogCategoryRepository = new GenericRepository<Us_call_log_category>(context);
                return usCallLogCategoryRepository;
            }
        }
        public GenericRepository<inspections_list2> InspectionList2Repository
        {
            get
            {
                if (inspectionList2Repository == null)
                    inspectionList2Repository = new GenericRepository<inspections_list2>(context);
                return inspectionList2Repository;
            }
        }
        public GenericRepository<tp2_sales_export_all> Tp2SalesExportAll
        {
            get
            {
                if (tp2SalesExportAll == null)
                    tp2SalesExportAll = new GenericRepository<tp2_sales_export_all>(context);
                return tp2SalesExportAll;
            }
        }

        public GenericRepository<Sales_forecast> SalesForecastRepository
        {
            get
            {
                if (salesForecastRepository == null)
                    salesForecastRepository = new GenericRepository<Sales_forecast>(context);
                return salesForecastRepository;
            }            
        }

        public GenericRepository<us_call_log2> UsCallLog2Repository
        {
            get
            {
                if (us_call_log2Repository == null)
                    us_call_log2Repository = new GenericRepository<us_call_log2>(context);
                return us_call_log2Repository;
            }
        }
        public GenericRepository<us_call_log_export> UsCallLogExportRepository
        {
            get
            {
                if (us_call_log_exportRepository == null)
                    us_call_log_exportRepository = new GenericRepository<us_call_log_export>(context);
                return us_call_log_exportRepository;
            }
        }

        public GenericRepository<MastProduct_Component> MastProdComponentsRepository
        {
            get
            {
                if (mastProdComponentsRepository == null)
                    mastProdComponentsRepository = new GenericRepository<MastProduct_Component>(context);
                return mastProdComponentsRepository;
            }
            
        }

        public GenericRepository<Exchange_rates> ExchangeRateRepository
        {
            get
            {
                if (exchangeRateRepository == null)
                    exchangeRateRepository = new GenericRepository<Exchange_rates>(context);
                return exchangeRateRepository;
            }            
        }

        public GenericRepository<Brand> BrandRepository
        {
            get
            {
                if (brandRepository == null)
                    brandRepository = new GenericRepository<Brand>(context);
                return brandRepository;
            }            
        }

        public GenericRepository<Web_site> WebSiteRepository
        {
            get
            {
                if (webSiteRepository == null)
                    webSiteRepository = new GenericRepository<Web_site>(context);
                return webSiteRepository;
            }            
        }

        public GenericRepository<Image_type> ImageTypeRepository
        {
            get
            {
                if (imageTypeRepository == null)
                    imageTypeRepository = new GenericRepository<Image_type>(context);
                return imageTypeRepository;
            }            
        }

        public WebProductFileTypeRepository WebProductFileTypeRepository
        {
            get
            {
                if (webProductFileTypeRepository == null)
                    webProductFileTypeRepository = new WebProductFileTypeRepository(context);
                return webProductFileTypeRepository;
            }
            
        }

        public GenericRepository<Web_product_pressure> WebProductPressureRepository
        {
            get
            {
                if (webProductPressureRepository == null)
                    webProductPressureRepository = new GenericRepository<Web_product_pressure>(context);
                return webProductPressureRepository;
            }
            
        }

        public GenericRepository<Web_product_part> WebProductPartRepository
        {
            get
            {
                if (webProductPartRepository == null)
                    webProductPartRepository = new GenericRepository<Web_product_part>(context);
                return webProductPartRepository;
            }
            
        }

        public GenericRepository<Sale> SaleRepository
        {
            get
            {
                if (saleRepository == null)
                    saleRepository = new GenericRepository<Sale>(context);
                return saleRepository;
            }
            
        }

        public GenericRepository<Login_history> LoginHistoryRepository
        {
            get
            {
                if (loginHistoryRepository == null)
                    loginHistoryRepository = new GenericRepository<Login_history>(context);
                return loginHistoryRepository;
            }
                        
        }

        public GenericRepository<Login_history_detail> LoginHistoryDetailRepository
        {
            get
            {
                if (loginHistoryDetailRepository == null)
                    loginHistoryDetailRepository = new GenericRepository<Login_history_detail>(context);
                return loginHistoryDetailRepository;
            }

        }

        public GenericRepository<Admin_pages> AdminPagesRepository
        {
            get
            {
                if (adminPagesRepository == null)
                    adminPagesRepository = new GenericRepository<Admin_pages>(context);
                return adminPagesRepository;
            }

            
        }

        public GenericRepository<Admin_pages_new> AdminPagesNewRepository
        {
            get
            {
                if (adminPagesNewRepository == null)
                    adminPagesNewRepository = new GenericRepository<Admin_pages_new>(context);
                return adminPagesNewRepository;
            }

            
        }

        public GenericRepository<Client_pages_allocated> ClientPagesRepository
        {
            get
            {
                if (clientPagesRepository == null)
                    clientPagesRepository = new GenericRepository<Client_pages_allocated>(context);
                return clientPagesRepository;
            }

            
        }

        public GenericRepository<Inspection_v2_si_subject> InspectionSubjectRepository
        {
            get
            {
                if (inspectionSubjectRepository == null)
                    inspectionSubjectRepository = new GenericRepository<Inspection_v2_si_subject>(context);
                return inspectionSubjectRepository;
            }
            
        }

        public BudgetRepository BudgetActualDataRepository
        {
            get
            {
                if (budgetActualDataRepository == null)
                    budgetActualDataRepository = new BudgetRepository(context);
                return budgetActualDataRepository;
            }
            
        }

        public GenericRepository<Inspection_v2_image_type> InspectionImageTypeRepository
        {
            get
            {
                if (inspectionImageTypeRepository == null)
                    inspectionImageTypeRepository = new GenericRepository<Inspection_v2_image_type>(context);
                return inspectionImageTypeRepository;
            }

            
        }

        public GenericRepository<cust_products_bundle> CustProductBundleRepository
        {
            get
            {
                if (custProductBundleRepository == null)
                    custProductBundleRepository = new GenericRepository<cust_products_bundle>(context);
                return custProductBundleRepository;
            }
            
        }
        public GenericRepository<Cust_product_details> CustProductDetails
        {
            get
            {
                if (custProductDetailRepository == null)
                    custProductDetailRepository = new GenericRepository<Cust_product_details>(context);
                return custProductDetailRepository;
            }

        }


        public SalesOrdersHeadersRepository SalesOrderHeaderRepository
        {
            get
            {
                if (salesOrderHeaderRepository == null)
                    salesOrderHeaderRepository = new SalesOrdersHeadersRepository(context);
                return salesOrderHeaderRepository;
            }            
        }
        public GenericRepository<Sales_orders_headers_shipping> SalesOrderHeaderShippingRepository
        {
            get
            {
                if (salesOrderHeaderShippingRepository == null)
                    salesOrderHeaderShippingRepository = new GenericRepository<Sales_orders_headers_shipping>(context);
                return salesOrderHeaderShippingRepository;
            }
        }
        public GenericRepository<Us_backorders> BackOrdersRepository
        {
            get
            {
                if (backOrdersRepository == null)
                    backOrdersRepository = new GenericRepository<Us_backorders>(context);
                return backOrdersRepository;
            }            
        }

        public GenericRepository<ds_guarantee_registration> DigitalShowerGuarantyRegistrationRepository
        {
            get
            {
                if (digitalShowerGuaranteeRegistration == null)
                    digitalShowerGuaranteeRegistration = new GenericRepository<ds_guarantee_registration>(context);
                return digitalShowerGuaranteeRegistration;
            }
        }

        public UsSalesLogRepository UsSalesLogRepository
        {
            get
            {
                if (usSalesLogRepository == null)
                    usSalesLogRepository = new UsSalesLogRepository(context);
                return usSalesLogRepository;
            }

            
        }

	    public GenericRepository<Cust_product_files> CustProductFilesRepository
	    {
	        get
	        {
                if (custProductFilesRepository == null)
                    custProductFilesRepository = new GenericRepository<Cust_product_files>(context);
                return custProductFilesRepository;
	        }
	    }

        public GenericRepository<inspection_lines_notified_v2_table> InspLinesNotifiedRepository
        {
            get
            {
                if (inspLinesNotifiedRepository == null)
                    inspLinesNotifiedRepository = new GenericRepository<inspection_lines_notified_v2_table>(context);
                return inspLinesNotifiedRepository;
            }
        }

        public GenericRepository<Container_types> ContainerTypeRepository
        {
            get
            {
                if (containerTypeRepository == null)
                    containerTypeRepository = new GenericRepository<Container_types>(context);
                return containerTypeRepository;
            }
        }

        public GenericRepository<process> ProcessRepository
        {
            get
            {
                if (processRepository == null)
                    processRepository = new GenericRepository<process>(context);
                return processRepository;
            }
        }

        public GenericRepository<BrochureDownloadLog> BrochureDownloadLogRepository
        {
            get
            {
                if (brochureDownloadLogRepository == null)
                    brochureDownloadLogRepository = new GenericRepository<BrochureDownloadLog>(context);
                return brochureDownloadLogRepository;
            }
        }

        public GenericRepository<Ca_reasons> CaReasonsRepository
        {
            get
            {
                if (caReasonsRepository == null)
                    caReasonsRepository = new GenericRepository<Ca_reasons>(context);
                return caReasonsRepository;
            }
        }


        public GenericRepository<Log_functions> LogFunctionsRepository
        {
            get
            {
                if (logFunctionsRepository == null)
                    logFunctionsRepository = new GenericRepository<Log_functions>(context);
                return logFunctionsRepository;
            }
        }

        public GenericRepository<Return_category> ReturnCategoryRepository
        {
            get
            {
                if (returnCategoryRepository == null)
                    returnCategoryRepository = new GenericRepository<Return_category>(context);
                return returnCategoryRepository;
            }
        }

        public GenericRepository<change_notice_reasons> ChangeNoticeReasonsRepository
        {
            get
            {
                if (changeNoticeReasonsRepository == null)
                    changeNoticeReasonsRepository = new GenericRepository<change_notice_reasons>(context);
                return changeNoticeReasonsRepository;
            }
        }

        public ChangeNoticeRepository ChangeNoticeRepository
        {
            get => changeNoticeRepository ?? new ChangeNoticeRepository(context);
        }

        public GenericRepository<nr_type> NrTypeRepository
        {
            get
            {
                if (nrTypeRepository == null)
                    nrTypeRepository = new GenericRepository<nr_type>(context);
                return nrTypeRepository;
            } 
        }

        public GenericRepository<Ip2> Ip2Repository
        {
            get
            {
                if (ip2Repository == null)
                    ip2Repository = new GenericRepository<Ip2>(context);
                return ip2Repository;
            }
        }

        public GenericRepository<Form> FormRepository
        {
            get
            {
                if (formRepository == null)
                    formRepository = new GenericRepository<Form>(context);
                return formRepository;
            }
        }

        public FormSubmissionRepository FormSubmissionRepository
        {
            get
            {
                if (formSubmissionRepository == null)
                    formSubmissionRepository = new FormSubmissionRepository(context);
                return formSubmissionRepository;
            }
        }

        public GenericRepository<Form_question_type> FormQuestionTypeRepository
        {
            get
            {
                if (formQuestionTypeRepository == null)
                    formQuestionTypeRepository = new GenericRepository<Form_question_type>(context);
                return formQuestionTypeRepository;
            }
        }

        public GenericRepository<Form_question_rendermethod> FormQuestionRenderMethodRepository
        {
            get
            {
                if (formQuestionRenderMethodRepository == null)
                    formQuestionRenderMethodRepository = new GenericRepository<Form_question_rendermethod>(context);
                return formQuestionRenderMethodRepository;
            }
        }

        public ContainerCalculationRepository ContainerCalcRepository
        {
            get
            {
                if (containerCalcRepository == null)
                    containerCalcRepository = new ContainerCalculationRepository(context);
                return containerCalcRepository;
            }
        }
        public GenericRepository<feedback_issue_type> FeedbackIssueTypeRepository
        {
            get
            {
                if (feedbackIssueTypeRepository == null)
                    feedbackIssueTypeRepository = new GenericRepository<feedback_issue_type>(context);
                return feedbackIssueTypeRepository;
            }             
        }

        public GenericRepository<feedback_authorization> FeedbackAuthorizationRepository
        {
            get
            {
                if (feedbackAuthorizationRepository == null)
                    feedbackAuthorizationRepository = new GenericRepository<feedback_authorization>(context);
                return feedbackAuthorizationRepository;
            }            
        }

        public GenericRepository<UserGroup> UserGroupRepository
        {
            get
            {
                if (userGroupRepository == null)
                    userGroupRepository = new GenericRepository<UserGroup>(context);
                return userGroupRepository;
            }
        }

        public GenericRepository<Sales_data_contracts> SalesDataContractsRepository
        {
            get
            {
                if (salesDataContractsRepository == null)
                    salesDataContractsRepository = new GenericRepository<Sales_data_contracts>(context);
                return salesDataContractsRepository;
            }
        }

        public InstructionsRepository InstructionsRepository
        {
            get
            {
                if (instructionsRepository == null)
                    instructionsRepository = new InstructionsRepository(context);
                return instructionsRepository;
            }
        }

        public GenericRepository<returns_events> ReturnsEventsRepository
        {
            get
            {
                if (returnsEventsRepository == null)
                    returnsEventsRepository = new GenericRepository<returns_events>(context);
                return returnsEventsRepository;
            }
        }

		public GenericRepository<ProductPricing_settings> ProductPricingSettingsRepository {
			get
			{
				if (productPricingSettingsRepository == null)
					productPricingSettingsRepository = new GenericRepository<ProductPricing_settings>(context);
				return productPricingSettingsRepository;
			}
		}
		public ProductPricingModelRepository ProductPricingModelRepository {
			get
			{
				if (productPricingModelRepository == null)
					productPricingModelRepository = new ProductPricingModelRepository(context);
				return productPricingModelRepository;
			}
		}
		public GenericRepository<Freightcost> FreightCostRepository {
			get
			{
				if (freightCostRepository == null)
					freightCostRepository = new GenericRepository<Freightcost>(context);
				return freightCostRepository;
			}
		}

		public GenericRepository<Market> MarketRepository
		{
			get
			{
				if (marketRepository == null)
					marketRepository = new GenericRepository<Market>(context);
				return marketRepository;
			}
		}

		public GenericRepository<sales_orders_brand> SalesOrderBrandRepository
		{
			get
			{
				if (salesOrderBrandRepository == null)
					salesOrderBrandRepository = new GenericRepository<sales_orders_brand>(context);
				return salesOrderBrandRepository;
			}
		}

		public ProductPricingProjectRepository ProductPricingProjectRepository
		{
			get
			{
				if (productPricingProjectRepository == null)
					productPricingProjectRepository = new ProductPricingProjectRepository(context);
				return productPricingProjectRepository;
			}
		}

        public ManualV2Repository ManualV2Repository
        {
            get
            {
                if (manualV2Repository == null)
                    manualV2Repository = new ManualV2Repository(context);
                return manualV2Repository;
            }
        }

		public GenericRepository<Cw_product> CwProductRepository
		{
			get
			{
				if (cwProductRepository == null)
					cwProductRepository = new GenericRepository<Cw_product>(context);
				return cwProductRepository;
			}
		}

		public GenericRepository<Cw_feature> CwFeatureRepository
		{
			get
			{
				if (cwFeatureRepository == null)
					cwFeatureRepository = new GenericRepository<Cw_feature>(context);
				return cwFeatureRepository;
			}
		}

		public GenericRepository<Cw_component> CwComponentRepository
		{
			get
			{
				if (cwComponentRepository == null)
					cwComponentRepository = new GenericRepository<Cw_component>(context);
				return cwComponentRepository;
			}
		}

		public GenericRepository<cw_site> CwSiteRepository
		{
			get
			{
				if (cwSiteRepository == null)
					cwSiteRepository = new GenericRepository<cw_site>(context);
				return cwSiteRepository;
			}
		}

		public GenericRepository<Ar_report> ArReportRepository
		{
			get
			{
				if (arReportRepository == null)
					arReportRepository = new GenericRepository<Ar_report>(context);
				return arReportRepository;
				
			} 			
		}

		public GenericRepository<Cw_product_file> CwProductFileRepository
		{
			get
			{
				if (cwProductFileRepository == null)
					cwProductFileRepository = new GenericRepository<Cw_product_file>(context);
				return cwProductFileRepository;

			}
		}
        public GenericRepository<Us_call_log_images> UsCallLogImageRepository
        {
			get
			{
				if (usCallLogImageRepository == null)
                    usCallLogImageRepository = new GenericRepository<Us_call_log_images>(context);
				return usCallLogImageRepository;

			}
		}

        public GenericRepository<Factory_client_settings> FactoryClientSettings
        {
            get
            {
                if (factoryClientSettings == null)
                    factoryClientSettings = new GenericRepository<Factory_client_settings>(context);
                return factoryClientSettings;
            }
        }

        public GenericRepository<Order_line_detail2_v6> OrderLineViewRepository
        {
            get
            {
                if (orderLineViewRepository == null)
                    orderLineViewRepository = new GenericRepository<Order_line_detail2_v6>(context);
                return orderLineViewRepository;
            }

        }

        public GenericRepository<Web_slide> WebSlideRepository
        {
            get
            {
                if (webSlideRepository == null)
                    webSlideRepository = new GenericRepository<Web_slide>(context);
                return webSlideRepository;
            }
        }

        public GenericRepository<Inspection_v2_image> InspectionV2ImageRepository
        {
            get
            {
                if (inspectionV2ImageRepository == null)
                    inspectionV2ImageRepository = new GenericRepository<Inspection_v2_image>(context);
                return inspectionV2ImageRepository;
            }
        }


        public void Save()
		{
			context.SaveChanges();
            
		}

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

	    public void EnableAutoChanges()
	    {
	        context.Configuration.AutoDetectChangesEnabled = true;
	    }

        public List<Web_product_new> GetWebProductsForCategory(int cat_id, bool deep = true, bool category = false, int? language_id = null, bool files = false, bool related = false, bool complementary = false, bool components = true)
        {
            var cats = new List<Web_category>();
            cats.Add(new Web_category { category_id = cat_id });
            if (deep) {
                cats.AddRange(WebCategoryRepository.GetAllChildren(cat_id));
            }
            var ids = cats.Select(c => c.category_id).ToList();
            var includes = new List<string>() { "SelectedCategories" };
            if (files)
                includes.Add("WebFiles");
            if (related)
                includes.Add("RelatedProducts");
            if (components)
                includes.Add("Components.Component");
            return WebProductRepository.Get(p => p.web_status == 1 && p.SelectedCategories.Any(c=>ids.Contains(c.category_id)), includeProperties: string.Join(",", includes )).ToList();

        }

	    public IDbConnection Connection => context.Database.Connection;

		

		public void SetLazyLoading(bool value)
        {
            context.Configuration.LazyLoadingEnabled = value;
            context.Configuration.ProxyCreationEnabled = value;
        }
        
    }
}
