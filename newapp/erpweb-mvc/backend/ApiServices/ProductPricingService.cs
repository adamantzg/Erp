using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;
using erp.Model.Dal.New;
using erp.DAL.EF.New;

namespace backend.ApiServices
{
	public class ProductPricingService : IProductPricingService
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICustproductsDAL custproductsDAL;
		private readonly IMastProductsDal mastProductsDal;
		private readonly ILocationDAL locationDAL;

		public ProductPricingService(IUnitOfWork unitOfWork, ICustproductsDAL custproductsDAL, IMastProductsDal mastProductsDal,
			ILocationDAL locationDAL)
		{
			this.unitOfWork = unitOfWork;
			this.custproductsDAL = custproductsDAL;
			this.mastProductsDal = mastProductsDal;
			this.locationDAL = locationDAL;
		}

		public object GetProjects()
		{
			return unitOfWork.ProductPricingProjectRepository.Get(includeProperties: "Currency, PricingModel").
				Select(GetProjectUIObject);
		}

		internal static object GetProjectUIObject(ProductPricingProject p)
		{
			return new
			{
				p.id,
				p.name,
				p.pricing_model_id,
				currency = p.Currency,
				p.currency_id,
				pricingModel = p.PricingModel,
				products = p.Products?.Select(ProductService.GetUIObject),
				settings = p.Settings
			};
		}

		public object GetEmptyProject()
		{
			return GetProjectUIObject(new ProductPricingProject { Products = new List<Cust_products>(), Settings = unitOfWork.ProductPricingSettingsRepository.Get().ToList() });
		}

		public object GetProject(int id, bool includeProducts = true, bool loadFirstMastProduct = false)
		{
			var include = "ProjectSettings";
			if (includeProducts)
				include += ",Products";
			var project = unitOfWork.ProductPricingProjectRepository.Get(p => p.id == id, includeProperties: include).FirstOrDefault();
			if (project != null)
			{
				project.Settings = MergeSettings(project);
				if(loadFirstMastProduct && project.Products.Count > 0)
				{
					unitOfWork.CustProductRepository.LoadReference(project.Products[0], "MastProduct");
				}
				return GetProjectUIObject(project);
			}

			return null;
		}

		public List<ProductPricing_settings> MergeSettings(ProductPricingProject project)
		{
			var settings = unitOfWork.ProductPricingSettingsRepository.Get().ToList();
			var projSettings = project.ProjectSettings.ToDictionary(s => s.setting_id);
			foreach (var s in settings)
			{
				if (projSettings.ContainsKey(s.id))
					s.numValue = projSettings[s.id].numValue;
			}
			return settings;
		}

		public object CreateProject(ProductPricingProject project)
		{
			var settings = unitOfWork.ProductPricingSettingsRepository.Get().ToDictionary(s => s.id);
			project.ProjectSettings = new List<ProductPricingProjectSettings>();
			project.ProjectSettings.AddRange(project.Settings.Where(s => settings[s.id].numValue != s.numValue)
											.Select(s => new ProductPricingProjectSettings { setting_id = s.id, numValue = s.numValue }));
			unitOfWork.ProductPricingProjectRepository.Insert(project);
			unitOfWork.Save();
			return GetProjectUIObject(project);
		}

		public object UpdateProject(ProductPricingProject project)
		{
			var settings = unitOfWork.ProductPricingSettingsRepository.Get().ToDictionary(s => s.id);
			project.ProjectSettings = new List<ProductPricingProjectSettings>();
			project.ProjectSettings.AddRange(project.Settings.Where(s => settings[s.id].numValue != s.numValue)
											.Select(s => new ProductPricingProjectSettings { setting_id = s.id, numValue = s.numValue }));
			unitOfWork.ProductPricingProjectRepository.Update(project);
			unitOfWork.Save();
			return GetProjectUIObject(project);
		}

		public object GetModels(bool includeLevels = true)
		{
			return unitOfWork.ProductPricingModelRepository.Get(includeProperties: includeLevels ? "Levels" : "").ToList()
				.Select(GetPricingModelUIObject);
		}

		internal static object GetPricingModelUIObject(ProductPricing_model m)
		{
			return new
			{
				m.id,
				m.name,
				m.market_id,
				levels = m.Levels?.Select(l => new
				{
					l.id,
					l.level,
					l.value,
					l.model_id
				})
			};
		}

		public object UpdateModels(List<ProductPricing_model> models)
		{

			foreach (var m in models)
			{
				if (m.id > 0)
					unitOfWork.ProductPricingModelRepository.Update(m);
				else
					unitOfWork.ProductPricingModelRepository.Insert(m);
			}
			unitOfWork.Save();
			unitOfWork.ProductPricingModelRepository.DeleteByIds(models.Select(m => m.id), true);

			return models.Select(GetPricingModelUIObject);
		}

		public object GetSettings()
		{
			return unitOfWork.ProductPricingSettingsRepository.Get().ToList();
		}

		public object UpdateSettings(List<ProductPricing_settings> settings)
		{
			foreach (var s in settings)
			{
				unitOfWork.ProductPricingSettingsRepository.Update(s);
			}
			unitOfWork.Save();
			return null;
		}

		public object GetMarkets()
		{
			return unitOfWork.MarketRepository.Get();
		}

		public object GetLocations()
		{
			return locationDAL.GetAll();
		}

		public object GetContainers()
		{
			var types = new[] { Container_types.Gp40, Container_types.Gp20 };
			return unitOfWork.ContainerTypeRepository.Get(t => types.Contains(t.container_type_id));
		}

		public object GetFreightCosts()
		{
			return unitOfWork.FreightCostRepository.Get().Select(GetFreightCostUIObject);
		}

		internal static object GetFreightCostUIObject(Freightcost c)
		{
			return new
			{
				c.container_id,
				c.id,
				c.location_id,
				c.market_id,
				c.value
			};
		}

		public object UpdateFreightCosts(List<Freightcost> data)
		{
			foreach (var d in data)
			{
				if (d.id > 0)
					unitOfWork.FreightCostRepository.Update(d);
				else
					unitOfWork.FreightCostRepository.Insert(d);
			}
			unitOfWork.Save();
			unitOfWork.FreightCostRepository.DeleteByIds(data.Select(c => c.id), true);
			return data;
		}

		public object UpdateMarkets(List<Market> markets)
		{

			foreach (var m in markets)
			{
				if (m.id > 0)
					unitOfWork.MarketRepository.Update(m);
				else
					unitOfWork.MarketRepository.Insert(m);
			}
			unitOfWork.Save();
			unitOfWork.MarketRepository.DeleteByIds(markets.Select(m => m.id), true);

			return markets;
		}

	}
}