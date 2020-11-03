using System.Collections.Generic;
using erp.Model;

namespace backend.ApiServices
{
	public interface IProductPricingService
	{
		object CreateProject(ProductPricingProject project);
		object GetContainers();
		object GetEmptyProject();
		object GetFreightCosts();
		object GetLocations();
		object GetMarkets();
		object GetModels(bool includeLevels = true);
		object GetProject(int id, bool includeProducts = true, bool loadFirstMastProduct = false);
		object GetProjects();
		object GetSettings();
		List<ProductPricing_settings> MergeSettings(ProductPricingProject project);
		object UpdateFreightCosts(List<Freightcost> data);
		object UpdateMarkets(List<Market> markets);
		object UpdateModels(List<ProductPricing_model> models);
		object UpdateProject(ProductPricingProject project);
		object UpdateSettings(List<ProductPricing_settings> settings);
	}
}