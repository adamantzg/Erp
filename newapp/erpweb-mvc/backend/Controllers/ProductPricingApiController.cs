using erp.DAL.EF.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.Model;
using erp.Model.Dal.New;
using backend.ApiServices;

namespace backend.Controllers
{
	public class ProductPricingApiController : ApiController
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICategory1DAL category1DAL;
		private readonly IProductService productService;
		private readonly IProductPricingService productPricingService;
		private readonly ICurrenciesDAL currenciesDAL;
		private readonly ICompanyDAL companyDAL;
		private readonly ITariffsDal tariffsDal;

		public ProductPricingApiController(IUnitOfWork unitOfWork, ICategory1DAL category1DAL, IProductService productService,
			IProductPricingService productPricingService, ICurrenciesDAL currenciesDAL, ICompanyDAL companyDAL,
			ITariffsDal tariffsDal)
		{
			this.unitOfWork = unitOfWork;
			this.category1DAL = category1DAL;
			this.productService = productService;
			this.productPricingService = productPricingService;
			this.currenciesDAL = currenciesDAL;
			this.companyDAL = companyDAL;
			this.tariffsDal = tariffsDal;
		}


		[Route("api/productpricing/models")]
		[HttpGet]
		public object GetModels()
		{
			return productPricingService.GetModels();
		}

		[Route("api/productpricing/updateModels")]
		[HttpPost]
		public object UpdateModels(List<ProductPricing_model> models)
		{
			return productPricingService.UpdateModels(models);
		}

		[Route("api/productpricing/getSettings")]
		[HttpGet]
		public object GetSettings()
		{
			return productPricingService.GetSettings();
		}

		[Route("api/productpricing/updateSettings")]
		[HttpPost]
		public object UpdateSettings(List<ProductPricing_settings> settings)
		{
			return productPricingService.UpdateSettings(settings);
		}

		[Route("api/productpricing/markets")]
		[HttpGet]
		public object GetMarkets()
		{
			return productPricingService.GetMarkets();
		}

		[Route("api/productpricing/locations")]
		[HttpGet]
		public object GetLocations()
		{
			return productPricingService.GetLocations();
		}

		[Route("api/productpricing/containers")]
		[HttpGet]
		public object GetContainers()
		{
			return productPricingService.GetContainers();
		}

		[Route("api/productpricing/freightCosts")]
		[HttpGet]
		public object GetFreightCosts()
		{
			return productPricingService.GetFreightCosts();
		}

		[Route("api/productpricing/currencies")]
		[HttpGet]
		public object GetCurrencies()
		{
			return currenciesDAL.GetAll().Where(c => new[] { "USD", "EUR", "GBP" }.Contains(c.curr_symbol));
		}

		[Route("api/productpricing/tariffs")]
		[HttpGet]
		public object GetTariffs()
		{
			return tariffsDal.GetAll();
		}

		[Route("api/productpricing/getfreightCostModel")]
		[HttpGet]
		public object GetFreightCostModel()
		{

			return new
			{
				locations = GetLocations(),
				containerTypes = GetContainers(),
				markets = GetMarkets(),
				costs = GetFreightCosts()
			};
		}

		[Route("api/productpricing/getProductEditModel")]
		[HttpGet]
		public object GetProductEditModel(int? id = null, int? projectId = null)
		{
			return new
			{
				product = id > 0 ? productService.Get(id.Value, true) :
				new Cust_products
				{
					cprod_status = "D",
					MastProduct = new Mast_products
					{
						Files = new List<MastProductFile>(),
						ProductPricingData = new ProductPricingMastProductData(),
						Prices = new List<mastproduct_price>()
					},
					MarketData = new List<Market_product>(),
					SalesForecast = new List<Sales_forecast>()
				},
				freightCosts = GetFreightCosts(),
				factories = companyDAL.GetFactories(),
				clients = companyDAL.GetClients(),
				categories = category1DAL.GetAll(),
				project = projectId != null ? productPricingService.GetProject(projectId.Value, true, true) : null,
				settings = GetSettings(),
				pricingModels = GetModels(),
				containerTypes = GetContainers(),
				currencies = GetCurrencies(),
				markets = GetMarkets(),
				locations = GetLocations(),
				tariffs = GetTariffs()
			};
		}



		[Route("api/productpricing/updateFreightCosts")]
		[HttpPost]
		public object UpdateFreightCosts(List<Freightcost> data)
		{
			return productPricingService.UpdateFreightCosts(data);
		}

		[Route("api/productpricing/updateMarkets")]
		[HttpPost]
		public object UpdateMarkets(List<Market> markets)
		{

			return productPricingService.UpdateMarkets(markets);
		}

		[Route("api/productpricing/projects")]
		[HttpGet]
		public object GetProjects()
		{
			return productPricingService.GetProjects();
		}

		[Route("api/productpricing/projectEditModel")]
		[HttpGet]
		public object GetProjectEditModel(int? id = null)
		{
			return new
			{
				project = id > 0 ? productPricingService.GetProject(id.Value) : productPricingService.GetEmptyProject(),
				currencies = GetCurrencies(),
				pricingModels = productPricingService.GetModels(false)
			};
		}

		[Route("api/productpricing/createproject")]
		[HttpPost]
		public object CreateProject(ProductPricingProject p)
		{
			return productPricingService.CreateProject(p);
		}


		[Route("api/productpricing/updateproject")]
		[HttpPost]
		public object UpdateProject(ProductPricingProject p)
		{
			return productPricingService.UpdateProject(p);
		}


	}
}