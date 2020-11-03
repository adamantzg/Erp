using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.New
{
	public interface IUnitOfWork
	{
		void Dispose();
		void Save();
		ICompanyRepository CompanyRepository { get; }
		IGenericRepository<cust_products_range> CustProductRangeRepository { get; }
		IGenericRepository<process> ProcessRepository { get; }
		ICustProductRepository CustProductRepository { get; }
		IGenericRepository<cust_products_bundle> CustProductBundleRepository { get; }
		IGenericRepository<Sales_orders> SalesOrdersRepository { get; }
		IGenericRepository<Sales_data> SalesDataRepository { get; }
		IGenericRepository<Order_lines> OrderLinesRepository { get; }
		IGenericRepository<Order_header> OrderRepository { get; }
		IGenericRepository<Container_types> ContainerTypeRepository { get; }
		IGenericRepository<Containercalculation_order> ContainerCalculationRepository { get; }
		IInvoiceRepository InvoiceRepository { get; }
		IGenericRepository<Exchange_rates> ExchangeRateRepository { get; }
		IGenericRepository<MastProduct_Component> MastProdComponentsRepository { get; }
		IGenericRepository<Return_category> ReturnCategoryRepository { get; }
		IGenericRepository<change_notice_reasons> ChangeNoticeReasonsRepository { get; }
		IChangeNoticeRepository ChangeNoticeRepository { get; }
		IInspV2TemplateRepository InspectionV2TemplateRepository { get; }
		IGenericRepository<Inspv2point> InspectionV2PointRepository { get; }
		IGenericRepository<Inspv2_criteriacategory> InspectionV2CriteriaCategoryRepository { get; }
		IGenericRepository<Inspv2_customcriteria> InspectionV2CustomCriteriaRepository { get; }
		IGenericRepository<Inspection_v2_type> InspectionV2TypeRepository { get; }
		IGenericRepository<change_notice_product_table> ChangeNoticeProductTableRepository { get; }
		IInspectionV2Repository InspectionV2Repository { get; }
		IGenericRepository<Inspection_v2_line> InspectionV2LineRepository { get; }
		IGenericRepository<Inspection_v2_loading> InspectionV2LoadingRepository { get; }
		IGenericRepository<Inspection_v2_area> InspectionV2AreaRepository { get; }
		IGenericRepository<Inspection_v2_image_type> InspectionV2ImageTypeRepository { get; }
		IGenericRepository<Qc_comments> QcCommentsRepository { get; }
		IProductTrackNumberQcRepository ProductTrackNumberQcRepository { get; }
		INrHeaderRepository NrHeaderRepository { get; }
		IGenericRepository<Nr_image_type> NrImageTypeRepository { get; }
		IGenericRepository<nr_type> NrTypeRepository {get;}
		IGenericRepository<ConfigSetting> ConfigSettingRepository { get;}
		IMastProductRepository MastProductRepository { get;}
		IGenericRepository<Material> MaterialRepository { get;}
		IGenericRepository<Packaging> PackagingRepository { get;}
		IGenericRepository<Category1> Category1Repository { get;}
		IGenericRepository<File> FileRepository { get;}
		GenericRepository<ProductPricing_settings> ProductPricingSettingsRepository { get;}
		IProductPricingModelRepository ProductPricingModelRepository { get;}
		GenericRepository<Freightcost> FreightCostRepository { get;}
		GenericRepository<Market> MarketRepository { get;}
		IProductPricingProjectRepository ProductPricingProjectRepository { get;}
		GenericRepository<Technical_subcategory_template> TechnicalSubcatTemplatesRepository { get;}
		GenericRepository<Technical_data_type> TechnicalDataTypeRepository { get;}
		GenericRepository<Technical_product_data> TechnicalProductDataRepository { get;}
		IReturnRepository ReturnRepository { get;}
		IGenericRepository<Inspection_v2_si_subject> InspectionSubjectRepository { get;}
		IGenericRepository<Inspection_v2_image_type> InspectionImageTypeRepository { get;}
		IUserRepository UserRepository { get;}
		IGenericRepository<UserGroup> UserGroupRepository { get;}
		IGenericRepository<feedback_authorization> FeedbackAuthorizationRepository { get;}
		IGenericRepository<Returns_qcusers> ReturnsUserUserRepository { get;}
		IGenericRepository<Inspections> InspectionRepository { get;}
		IGenericRepository<feedback_issue_type> FeedbackIssueTypeRepository { get;}
		IGenericRepository<returns_events> ReturnsEventsRepository { get;}
		IGenericRepository<Ca_reasons> CaReasonsRepository { get;}
		IGenericRepository<ports> PortsRepository { get; }
		IGenericRepository<Sales_forecast> SalesForecastRepository { get; }
		IGenericRepository<sales_forecast_contract> SalesForecastContractRepository { get; }
		IGenericRepository<product_file_type> ProductFileTypesRepository { get; }
		IGenericRepository<product_file> ProductFileRepository { get; }
		IGenericRepository<Web_category> WebCategoryRepository { get; }
		IGenericRepository<forward_order_lines> ForwardOrderLinesRepository { get; }
		IGenericRepository<web_product_info_type> WebProductInfoTypeRepository { get;}
		IGenericRepository<Web_product_new> WebProductNewRepository { get;  }
		IGenericRepository<Factory_stock_order> FactoryStockOrderRepository { get; }
		IGenericRepository<BudgetActualData> BudgetActualDataRepository { get; }
		IGenericRepository<Distributor_sales> DistSalesRepository { get;  }

		IGenericRepository<Cust_product_details> CustProductDetailsRepository { get;  }
		IGenericRepository<Us_dealers> UsDealerRepository { get;  }

		IGenericRepository<Form> FormRepository { get;  }
		IGenericRepository<Form_question_type> FormQuestionTypeRepository { get;  }
		IGenericRepository<Form_question_rendermethod> FormQuestionRenderMethodRepository { get;  }
		IGenericRepository<Form_submission> FormSubmissionRepository { get; }
		IGenericRepository<instructions_new> InstructionsRepository { get; }
		IGenericRepository<manual> ManualV2Repository { get; }
		IGenericRepository<Porder_lines> POrderLineRepository { get; }
		IGenericRepository<Stock_order_allocation> StockOrderAllocationRepository { get; }
		IGenericRepository<Ip2> Ip2Repository { get; }
		IGenericRepository<Factory_client_settings> FactoryClientSettingsRepository { get; }
		IGenericRepository<Navigation_item> NavigationItemRepository { get; }
		IGenericRepository<Navigation_item_permission> NavigationItemPermissionRepository { get; }
		IGenericRepository<Role> RoleRepository { get; }
	}
}
