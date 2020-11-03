using System.Web.Http;
using Unity;

using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using System.Data;
using NLog;
using Unity.Injection;
using System.Data.Entity;
using MySql.Data.MySqlClient;
using backend.ApiServices;
using backend.Properties;
using System;
using AegisImplicitMail;

namespace backend
{
    public static class UnityConfig
    {
		#region Unity Container
		private static Lazy<IUnityContainer> container =
		  new Lazy<IUnityContainer>(() =>
		  {
			  var container = new UnityContainer();
			  RegisterTypes(container);
			  return container;
		  });

		/// <summary>
		/// Configured Unity Container.
		/// </summary>
		public static IUnityContainer Container => container.Value;
		#endregion

		public static void RegisterTypes(IUnityContainer container)
        {
			

			// register all your components with the container here
			// it is NOT necessary to register your controllers

			// e.g. container.RegisterType<ITestService, TestService>();
			container.RegisterType<IDbConnection, MySqlConnection>(
				new InjectionConstructor(erp.Model.Dal.New.Properties.Settings.Default.connString));
			container.RegisterType<IUnitOfWork, UnitOfWork>();
			container.RegisterType<DbContext, Model>(new InjectionConstructor("name=erp.Model.Dal.New.Properties.Settings.connString"));

			container.RegisterFactory<ILogger>(c=> LogManager.GetCurrentClassLogger());

			//DAL and repos
			container.RegisterType<IAccountService, AccountService>();
			container.RegisterType<IAdminPagesDAL, AdminPagesDAL>();
			container.RegisterType<IAdminPagesNewDAL, AdminPagesNewDAL>();
			container.RegisterType<IAdminPermissionsDal, AdminPermissionsDal>();
			container.RegisterType<IAmendmentsDAL, AmendmentsDAL>();
			container.RegisterType<IAnalyticsCategoryDal, AnalyticsCategoryDal>();
			container.RegisterType<IAnalyticsDAL, AnalyticsDAL>();
			container.RegisterType<IAnalyticsOptionDal, AnalyticsOptionDal>();
			container.RegisterType<IAnalyticsSubcategoryDal, AnalyticsSubcategoryDal>();
			container.RegisterType<IAqlDal, AqlDal>();
			container.RegisterType<IBrandsDAL, BrandsDAL>();
			container.RegisterType<IBrandCategoriesDal, BrandCategoriesDal>();
			container.RegisterType<IBrandSalesAnalysis2DAL, BrandSalesAnalysis2DAL>();
			container.RegisterType<IBrandSubCategoriesDal, BrandSubCategoriesDal>();
			container.RegisterType<ICategory1DAL, Category1Dal>();
			container.RegisterType<ICategory1SubDal, Category1SubDal>();
			container.RegisterType<IClaimsInvestigationReportsDAL, ClaimsInvestigationReportsDAL>();
			container.RegisterType<IClaimsInvestigationReportsActionDAL, ClaimsInvestigationReportsActionDAL>();
			container.RegisterType<IClaimsInvestigationReportActionImageDAL, ClaimsInvestigationReportActionImageDAL>();			
			container.RegisterType<IClaimsService, ClaimsService>();
			container.RegisterType<IClientPagesAllocatedDAL, ClientPagesAllocatedDAL>();
			container.RegisterType<ICompanyDAL, CompanyDAL>();
			container.RegisterType<ICompanyService, CompanyService>();
			container.RegisterType<IContainerDal, ContainerDal>();
			container.RegisterType<IContainerTypesDal, ContainerTypesDal>();
			container.RegisterType<IContinentsDAL, ContinentsDAL>();
			container.RegisterType<IContractSalesForecastLinesDal, ContractSalesForecastLinesDal>();
			container.RegisterType<ICountriesDAL, CountriesDAL>();
			container.RegisterType<ICreditNoteLinesDAL, CreditNoteLinesDAL>();
			container.RegisterType<ICurrenciesDAL, CurrenciesDAL>();
			container.RegisterType<ICustproductsDAL, CustproductsDAL>();
			container.RegisterType<ICustProductDocTypeDal, CustProductDocTypeDal>();
			container.RegisterType<IDealerImagesDal, DealerImagesDal>();
			container.RegisterType<IDeliveryLocationsDAL, DeliveryLocationsDAL>();
			container.RegisterType<IDistProductsDal, DistProductsDal>();
			container.RegisterType<IEmailRecipientsDAL, EmailRecipientsDAL>();
			container.RegisterType<IFeedbackCategoryDAL, FeedbackCategoryDAL>();
			container.RegisterType<IFeedbackSubscriptionsDAL, FeedbackSubscriptionsDAL>();
			container.RegisterType<IFeedbackTypeDAL, FeedbackTypeDAL>();			
			container.RegisterType<IFileService, FileService>();
			container.RegisterType<IFileTypeDal, FileTypeDal>();
			container.RegisterType<ILanguageDal, LanguageDal>();
			container.RegisterType<ILocationDAL, LocationDAL>();
			container.RegisterType<ILoginhistoryDAL, LoginhistoryDAL>();
			container.RegisterType<ILoginHistoryDetailDAL, LoginHistoryDetailDAL>();
			container.RegisterType<ILoginHistoryPageDal, LoginHistoryPageDal>();
			container.RegisterType<IMailHelper, MailHelper>();
			container.RegisterType<IMimeMailer, MimeMailer>();
			container.RegisterType<IInspectionControllerDal, InspectionControllerDal>();
			container.RegisterType<IInspectionCriteriaDal, InspectionCriteriaDal>();
			container.RegisterType<IInspectionImagesDAL, InspectionImagesDAL>();
			container.RegisterType<IInspectionLinesAcceptedDal, InspectionLinesAcceptedDal>();
			container.RegisterType<IInspectionLinesNotifiedDal, InspectionLinesNotifiedDal>();
			container.RegisterType<IInspectionLinesRejectedDal, InspectionLinesRejectedDal>();
			container.RegisterType<IInspectionLinesTestedDal, InspectionLinesTestedDal>();
			container.RegisterType<IInspectionsDAL, InspectionsDAL>();			
			container.RegisterType<IInspectionsLoadingDal, InspectionsLoadingDal>();
			container.RegisterType<IInspectionsV2DAL, InspectionsV2DAL>();
			container.RegisterType<IInstructionsDal, InstructionsDal>();
			container.RegisterType<IInvoicesDAL, InvoicesDAL>();
			container.RegisterType<IInvoiceLinesDAL, InvoiceLinesDAL>();
			container.RegisterType<IInvoiceTypeDAL, InvoiceTypeDAL>();
			container.RegisterType<IMastProductsDal, MastProductsDal>();
			
			container.RegisterType<IOrderHeaderDAL, OrderHeaderDAL>();
			container.RegisterType<IOrderLineExportDal, OrderLineExportDal>();
			container.RegisterType<IOrderLinesDAL, OrderLinesDAL>();
			container.RegisterType<IOrderLinesManualDAL, OrderLinesManualDAL>();
			container.RegisterType<IOrderMgmtDetailDAL, OrderMgmtDetailDAL>();
			container.RegisterType<IOrderService, OrderService>();
			container.RegisterType<IPaymentDetailsDAL, PaymentDetailsDAL>();
			container.RegisterType<IPermissionDAL, PermissionDAL>();
			container.RegisterType<IPorderLinesDal, PorderLinesDal>();
			container.RegisterType<IProductfaultReasonDescriptionDAL, ProductfaultReasonDescriptionDAL>();
			container.RegisterType<IProductFaultsDAL, ProductFaultsDAL>();
			container.RegisterType<IProductGroupClassDal, ProductGroupClassDAL>();
			container.RegisterType<IProductInvestigationStatusDAL, ProductInvestigationStatusDAL>();
			container.RegisterType<IProductInvestigationsDAL, ProductInvestigationsDAL>();
			container.RegisterType<IProductInvestigationImagesDAL, ProductInvestigationImagesDAL>();
			container.RegisterType<IProductPricingModelRepository, ProductPricingModelRepository>();
			container.RegisterType<IProductPricingProjectRepository, ProductPricingProjectRepository>();
			container.RegisterType<IProductPricingService, ProductPricingService>();
			container.RegisterType<IProductService, ProductService>();
			container.RegisterType<IProductTrackNumberFcDal, ProductTrackNumberFcDal>();
			container.RegisterType<IRangeDAL, RangeDAL>();
			container.RegisterType<IRegistryReader, RegistryReader>();
			container.RegisterType<IReportingDal, ReportingDal>();
			container.RegisterType<IReturnCategoryDAL, ReturnCategoryDAL>();
			container.RegisterType<IReturnResolutionDAL, ReturnResolutionDAL>();
			container.RegisterType<IReturnsCommentsDAL, ReturnsCommentsDAL>();
			container.RegisterType<IReturnsCommentsFilesDAL, ReturnsCommentsFilesDAL>();
			container.RegisterType<IReturnsDAL, ReturnsDAL>();
			container.RegisterType<IReturnsImagesDAL, ReturnsImagesDAL>();
			container.RegisterType<IReturnsImportanceDAL, ReturnsImportanceDAL>();			
			container.RegisterType<IReturnsService, ReturnsService>();
			container.RegisterType<IRoleDAL, RoleDAL>();
			container.RegisterType<ISabcSortDal, SabcSortDal>();
			container.RegisterType<ISalesDataDal, SalesDataDal>();
			container.RegisterType<ISalesForecastDal, SalesForecastDal>();
			container.RegisterType<ISchemaInfoDal, SchemaInfoDal>();
			container.RegisterFactory<ISettings>(x => Properties.Settings.Default);
			container.RegisterType<IShipmentsDal, ShipmentsDal>();
			container.RegisterType<IStandardResponseDAL, StandardResponseDAL>();
			container.RegisterType<IStockCodeDal, StockCodeDal>();
			container.RegisterType<IStockCodeFactoryDal, StockCodeFactoryDal>();
			container.RegisterType<IStockMovementsDal, StockMovementsDal>();
			container.RegisterType<IStockOrderAllocationDAL, StockOrderAllocationDAL>();			
			container.RegisterType<ITariffsDal, TariffsDal>();
			container.RegisterType<ITechnicalDataService, TechnicalDataService>();
			container.RegisterType<ITechnicalDataTypeDal, TechnicalDataTypeDal>();
			container.RegisterType<ITechnicalProductDataDal, TechnicalProductDataDal>();
			container.RegisterType<IUploadedFileHandler, UploadedFileHandler>();
			container.RegisterType<IUserDAL, UserDAL>();
			container.RegisterType<IWebCategoryDal, WebCategoryDal>();
			container.RegisterType<IWebProductComponentDal, WebProductComponentDal>();
			container.RegisterType<IWebProductFileDal, WebProductFileDal>();
			container.RegisterType<IWebProductFileTypeDAL, WebProductFileTypeDAL>();
			container.RegisterType<IWebProductFlowDal, WebProductFlowDal>();
			container.RegisterType<IWebProductInfoDal, WebProductInfoDal>();
			container.RegisterType<IWebProductNewDal, WebProductNewDal>();
			container.RegisterType<IWebProductPartDal, WebProductPartDal>();
			container.RegisterType<IWebProductPressureDal, WebProductPressureDal>();
			container.RegisterType<IWebSiteDal, WebSiteDal>();
			container.RegisterType<IWhitebookCategoryDal, WhitebookCategoryDal>();			
			
        }

		public static ILogger CreateLogger(IUnityContainer container)
		{
			return LogManager.GetCurrentClassLogger();
		}
    }
}