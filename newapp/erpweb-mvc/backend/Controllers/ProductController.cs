using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using erp.DAL.EF.New;
using erp.Model.Dal.New;
using backend.ApiServices;
using backend.Models;

namespace backend.Controllers
{
    public class ProductController : BaseController
    {
		
		private readonly ICustproductsDAL custproductsDAL;
		private readonly ICategory1DAL category1DAL;
		private readonly ICompanyDAL companyDAL;
		private readonly IBrandsDAL brandsDAL;
		private readonly IInspectionsDAL inspectionsDAL;
		private readonly IOrderLinesDAL orderLinesDAL;
		private readonly ISalesForecastDal salesForecastDal;
		private readonly IOrderHeaderDAL orderHeaderDAL;
		private readonly IBrandCategoriesDal brandCategoriesDal;
		private readonly IOrderMgmtDetailDAL orderMgmtDetailDAL;
		
		private readonly IStockCodeDal stockCodeDal;
		private readonly ICustProductDocTypeDal custProductDocTypeDal;
		private readonly IMastProductsDal mastProductsDal;
		private readonly IWebProductNewDal webProductNewDal;
		private readonly ITechnicalDataService technicalDataService;
		private readonly ITechnicalProductDataDal technicalProductDataDal;
		private readonly ICategory1SubDal category1SubDal;
		private readonly ITechnicalDataTypeDal technicalDataTypeDal;
		private readonly IWebSiteDal webSiteDal;
		private readonly IAnalyticsDAL analyticsDAL;
		private readonly IProductPricingService productPricingService;
		private readonly ICurrenciesDAL currenciesDAL;
		private readonly ITariffsDal tariffsDal;
		private readonly IWebCategoryDal webCategoryDal;
		private readonly IReturnsService returnsService;
		private readonly IAnalyticsSubcategoryDal analyticsSubcategoryDal;
		
		private readonly ISabcSortDal sabcSortDal;

		//
		// GET: /Product/

		public ProductController(IUnitOfWork unitOfWork, ICustproductsDAL custproductsDAL, ICategory1DAL category1DAL,
			ICompanyDAL companyDAL, IBrandsDAL brandsDAL, IInspectionsDAL inspectionsDAL, IOrderLinesDAL orderLinesDAL,
			ISalesForecastDal salesForecastDal, IOrderHeaderDAL orderHeaderDAL, IBrandCategoriesDal brandCategoriesDal,
			IOrderMgmtDetailDAL orderMgmtDetailDAL, IStockCodeDal stockCodeDal,
			ICustProductDocTypeDal custProductDocTypeDal, IMastProductsDal mastProductsDal, IWebProductNewDal webProductNewDal,
			ITechnicalDataService technicalDataService, ITechnicalProductDataDal technicalProductDataDal, ICategory1SubDal category1SubDal,
			ITechnicalDataTypeDal technicalDataTypeDal, IWebSiteDal webSiteDal, IAnalyticsDAL analyticsDAL,
			IProductPricingService productPricingService, ICurrenciesDAL currenciesDAL, ITariffsDal tariffsDal,
			IWebCategoryDal webCategoryDal, IReturnsService returnsService, IAnalyticsSubcategoryDal analyticsSubcategoryDal,
			ISabcSortDal sabcSortDal, ILoginHistoryDetailDAL loginHistoryDetailDAL, IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL,
			IClientPagesAllocatedDAL clientPagesAllocatedDAL, IAccountService accountService) 
			: base(unitOfWork, loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService )
		{
			
			this.custproductsDAL = custproductsDAL;
			this.category1DAL = category1DAL;
			this.companyDAL = companyDAL;
			this.brandsDAL = brandsDAL;
			this.inspectionsDAL = inspectionsDAL;
			this.orderLinesDAL = orderLinesDAL;
			this.salesForecastDal = salesForecastDal;
			this.orderHeaderDAL = orderHeaderDAL;
			this.brandCategoriesDal = brandCategoriesDal;
			this.orderMgmtDetailDAL = orderMgmtDetailDAL;
			
			this.stockCodeDal = stockCodeDal;
			this.custProductDocTypeDal = custProductDocTypeDal;
			this.mastProductsDal = mastProductsDal;
			this.webProductNewDal = webProductNewDal;
			this.technicalDataService = technicalDataService;
			this.technicalProductDataDal = technicalProductDataDal;
			this.category1SubDal = category1SubDal;
			this.technicalDataTypeDal = technicalDataTypeDal;
			this.webSiteDal = webSiteDal;
			this.analyticsDAL = analyticsDAL;
			this.productPricingService = productPricingService;
			this.currenciesDAL = currenciesDAL;
			this.tariffsDal = tariffsDal;
			this.webCategoryDal = webCategoryDal;
			this.returnsService = returnsService;
			this.analyticsSubcategoryDal = analyticsSubcategoryDal;			
			this.sabcSortDal = sabcSortDal;
		}

		public ActionResult ProductPricingReport(int projectId, int market_id, ProductPricingCalculation calculation)
		{
			var project = unitOfWork.ProductPricingProjectRepository.Get(p => p.id == projectId, includeProperties: "Products.MastProduct.Factory,Products.MarketData, ProjectSettings, PricingModel.Levels").FirstOrDefault();
			foreach (var p in project.Products)
			{
				if (p.MastProduct != null)
				{
					unitOfWork.MastProductRepository.LoadCollection(p.MastProduct, "Prices");
					unitOfWork.MastProductRepository.LoadReference(p.MastProduct, "ProductPricingData");
				}

				unitOfWork.CustProductRepository.LoadCollection(p, "SalesForecast");
			}
			project.Settings = productPricingService.MergeSettings(project);
			var market = unitOfWork.MarketRepository.GetByID(market_id);

			return View(
				new ProductPricingProjectReportModel(project, market, calculation, currenciesDAL.GetAll(),
				unitOfWork.FreightCostRepository.Get().ToList(), tariffsDal.GetAll())
			);
		}
	}
}