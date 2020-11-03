using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private DbContext context = null;

		public void Dispose()
		{
			((IDisposable)context).Dispose();
		}

		public UnitOfWork(DbContext context)
		{
			this.context = context;
			context.Configuration.ProxyCreationEnabled = false;
			context.Configuration.LazyLoadingEnabled = false;
			context.Database.CommandTimeout = 300;
		}

		public void Save()
		{
			context.SaveChanges();
		}
		
		public ICompanyRepository CompanyRepository => new CompanyRepository(context);

		public IGenericRepository<cust_products_range> CustProductRangeRepository => new GenericRepository<cust_products_range>(context);

		public IGenericRepository<process> ProcessRepository => new GenericRepository<process>(context);

		public ICustProductRepository CustProductRepository => new CustProductRepository(context);

		public IGenericRepository<cust_products_bundle> CustProductBundleRepository => new GenericRepository<cust_products_bundle>(context);

		public IGenericRepository<Sales_orders> SalesOrdersRepository => new GenericRepository<Sales_orders>(context);

		public IGenericRepository<Sales_data> SalesDataRepository => new GenericRepository<Sales_data>(context);

		public IGenericRepository<Order_lines> OrderLinesRepository => new GenericRepository<Order_lines>(context);

		public IGenericRepository<Order_header> OrderRepository => new GenericRepository<Order_header>(context);

		public IGenericRepository<Container_types> ContainerTypeRepository => new GenericRepository<Container_types>(context);

		public IGenericRepository<Containercalculation_order> ContainerCalculationRepository => new ContainerCalculationRepository(context);

		public IInvoiceRepository InvoiceRepository => new InvoiceRepository(context);

		public IGenericRepository<Exchange_rates> ExchangeRateRepository => new GenericRepository<Exchange_rates>(context);
		public IGenericRepository<MastProduct_Component> MastProdComponentsRepository => new GenericRepository<MastProduct_Component>(context);
		public IGenericRepository<Return_category> ReturnCategoryRepository => new GenericRepository<Return_category>(context);
		public IGenericRepository<change_notice_reasons> ChangeNoticeReasonsRepository => new GenericRepository<change_notice_reasons>(context);
		public IChangeNoticeRepository ChangeNoticeRepository => new ChangeNoticeRepository(context);
		public IInspV2TemplateRepository InspectionV2TemplateRepository => new InspV2TemplateRepository(context);
		public IGenericRepository<Inspv2point> InspectionV2PointRepository => new GenericRepository<Inspv2point>(context);
		public IGenericRepository<Inspv2_criteriacategory> InspectionV2CriteriaCategoryRepository => new GenericRepository<Inspv2_criteriacategory>(context);
		public IGenericRepository<Inspv2_customcriteria> InspectionV2CustomCriteriaRepository => new GenericRepository<Inspv2_customcriteria>(context);
		public IGenericRepository<Inspection_v2_type> InspectionV2TypeRepository => new GenericRepository<Inspection_v2_type>(context);
		public IGenericRepository<change_notice_product_table> ChangeNoticeProductTableRepository => new GenericRepository<change_notice_product_table>(context);
		public IInspectionV2Repository InspectionV2Repository => new InspectionV2Repository(context);
		public IGenericRepository<Inspection_v2_line> InspectionV2LineRepository => new GenericRepository<Inspection_v2_line>(context);
		public IGenericRepository<Inspection_v2_loading> InspectionV2LoadingRepository => new GenericRepository<Inspection_v2_loading>(context);
		public IGenericRepository<Inspection_v2_area> InspectionV2AreaRepository => new GenericRepository<Inspection_v2_area>(context);
		public IGenericRepository<Inspection_v2_image_type> InspectionV2ImageTypeRepository => new GenericRepository<Inspection_v2_image_type>(context);
		public IGenericRepository<Qc_comments> QcCommentsRepository => new GenericRepository<Qc_comments>(context);
		public IProductTrackNumberQcRepository ProductTrackNumberQcRepository => new ProductTrackNumberQcRepository(context);
		public INrHeaderRepository NrHeaderRepository => new NrHeaderRepository(context);
		public IGenericRepository<Nr_image_type> NrImageTypeRepository => new GenericRepository<Nr_image_type>(context);
		public IGenericRepository<nr_type> NrTypeRepository => new GenericRepository<nr_type>(context);
		public IGenericRepository<ConfigSetting> ConfigSettingRepository => new GenericRepository<ConfigSetting>(context);
		public IMastProductRepository MastProductRepository => new MastProductRepository(context);

		public IGenericRepository<Material> MaterialRepository => new GenericRepository<Material>(context);
		public IGenericRepository<Packaging> PackagingRepository => new GenericRepository<Packaging>(context);
		public IGenericRepository<Category1> Category1Repository => new GenericRepository<Category1>(context);

		public IGenericRepository<File> FileRepository => new GenericRepository<File>(context);

		public GenericRepository<ProductPricing_settings> ProductPricingSettingsRepository => new GenericRepository<ProductPricing_settings>(context);

		public IProductPricingModelRepository ProductPricingModelRepository => new ProductPricingModelRepository(context);

		public GenericRepository<Freightcost> FreightCostRepository => new GenericRepository<Freightcost>(context);

		public GenericRepository<Market> MarketRepository => new GenericRepository<Market>(context);

		public IProductPricingProjectRepository ProductPricingProjectRepository => new ProductPricingProjectRepository(context);

		public GenericRepository<Technical_subcategory_template> TechnicalSubcatTemplatesRepository => new GenericRepository<Technical_subcategory_template>(context);

		public GenericRepository<Technical_data_type> TechnicalDataTypeRepository => new GenericRepository<Technical_data_type>(context);

		public GenericRepository<Technical_product_data> TechnicalProductDataRepository => new GenericRepository<Technical_product_data>(context);

		public IReturnRepository ReturnRepository => new ReturnRepository(context);

		public IGenericRepository<Inspection_v2_si_subject> InspectionSubjectRepository => new GenericRepository<Inspection_v2_si_subject>(context);

		public IGenericRepository<Inspection_v2_image_type> InspectionImageTypeRepository => new GenericRepository<Inspection_v2_image_type>(context);

		public IUserRepository UserRepository => new UserRepository(context);

		public IGenericRepository<UserGroup> UserGroupRepository => new GenericRepository<UserGroup>(context);

		public IGenericRepository<feedback_authorization> FeedbackAuthorizationRepository => new GenericRepository<feedback_authorization>(context);

		public IGenericRepository<Returns_qcusers> ReturnsUserUserRepository => new GenericRepository<Returns_qcusers>(context);

		public IGenericRepository<Inspections> InspectionRepository => new GenericRepository<Inspections>(context);

		public IGenericRepository<feedback_issue_type> FeedbackIssueTypeRepository => new GenericRepository<feedback_issue_type>(context);

		public IGenericRepository<returns_events> ReturnsEventsRepository => new GenericRepository<returns_events>(context);

		public IGenericRepository<Ca_reasons> CaReasonsRepository => new GenericRepository<Ca_reasons>(context);

		public IGenericRepository<ports> PortsRepository => new GenericRepository<ports>(context);

		public IGenericRepository<Sales_forecast> SalesForecastRepository => new GenericRepository<Sales_forecast>(context);

		public IGenericRepository<sales_forecast_contract> SalesForecastContractRepository => new GenericRepository<sales_forecast_contract>(context);

		public IGenericRepository<product_file_type> ProductFileTypesRepository => new GenericRepository<product_file_type>(context);

		public IGenericRepository<product_file> ProductFileRepository => new GenericRepository<product_file>(context);

		public IGenericRepository<Web_category> WebCategoryRepository => new GenericRepository<Web_category>(context);

		public IGenericRepository<forward_order_lines> ForwardOrderLinesRepository => new GenericRepository<forward_order_lines>(context);

		public IGenericRepository<web_product_info_type> WebProductInfoTypeRepository => new GenericRepository<web_product_info_type>(context);

		public IGenericRepository<Web_product_new> WebProductNewRepository => new GenericRepository<Web_product_new>(context);

		public IGenericRepository<Factory_stock_order> FactoryStockOrderRepository => new FactoryStockOrderRepository(context);

		public IGenericRepository<BudgetActualData> BudgetActualDataRepository => new GenericRepository<BudgetActualData>(context);

		public IGenericRepository<Distributor_sales> DistSalesRepository => new GenericRepository<Distributor_sales>(context);

		public IGenericRepository<Cust_product_details> CustProductDetailsRepository => new GenericRepository<Cust_product_details>(context);

		public IGenericRepository<Us_dealers> UsDealerRepository => new GenericRepository<Us_dealers>(context);

		public IGenericRepository<Form> FormRepository => new GenericRepository<Form>(context);

		public IGenericRepository<Form_question_type> FormQuestionTypeRepository => new GenericRepository<Form_question_type>(context);

		public IGenericRepository<Form_question_rendermethod> FormQuestionRenderMethodRepository => new GenericRepository<Form_question_rendermethod>(context);

		public IGenericRepository<Form_submission> FormSubmissionRepository => new GenericRepository<Form_submission>(context);

		public IGenericRepository<instructions_new> InstructionsRepository => new GenericRepository<instructions_new>(context);

		public IGenericRepository<manual> ManualV2Repository => new GenericRepository<manual>(context);

		public IGenericRepository<Porder_lines> POrderLineRepository => new GenericRepository<Porder_lines>(context);

		public IGenericRepository<Stock_order_allocation> StockOrderAllocationRepository => new GenericRepository<Stock_order_allocation>(context);

		public IGenericRepository<Ip2> Ip2Repository => new GenericRepository<Ip2>(context);

		public IGenericRepository<Factory_client_settings> FactoryClientSettingsRepository => new GenericRepository<Factory_client_settings>(context);

		public IGenericRepository<Navigation_item> NavigationItemRepository => new GenericRepository<Navigation_item>(context);

		public IGenericRepository<Navigation_item_permission> NavigationItemPermissionRepository => new GenericRepository<Navigation_item_permission>(context);

		public IGenericRepository<Role> RoleRepository => new GenericRepository<Role>(context);
	}
}
