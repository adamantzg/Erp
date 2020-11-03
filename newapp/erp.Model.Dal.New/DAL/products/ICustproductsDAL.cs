using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public interface ICustproductsDAL : IGenericDal<Cust_products>
	{
		List<Cust_products> GetAll(bool includeMastProducts = false);
		List<Cust_products> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null);
		Cust_products GetById(int id, bool loadParents = false);
		Cust_products GetByIdFull(int id);
		List<Cust_products> GetByUser(int cprod_user);
		List<Cust_products> GetByCode1(string cprod_code1, int? cprod_user = null);
		List<Cust_products> GetByCompany(int company_id, string prefixText = null);
		List<Cust_products> GetByDistributor(int company_id);
		List<Cust_products> GetForIds(List<int> ids);
		List<Cust_products> GetForFactories(IList<int> factory_ids, string text = null);
		List<Cust_products> GetForCodes(List<string> codes);
		List<Cust_products> GetForMastIds(List<int> ids, bool grouped = true);
		List<Cust_products> GetForCatIds(List<int> catids);
		List<Cust_products> GetByPatternAndFactory(string prefixText, int? factory_id);
		List<Cust_products> GetByCriteria(IList<int> clientIds, IList<int> factoryIds,bool? spares = null, bool? discontinued=null, int? category1_id = null, bool getSpecCount = false, string productName = null);
		List<ProductFit> GetProductFitByCriteria(List<int> clientIds, List<int> factoryIds, bool? spares = null, bool? discontinued = null, int? category1_id = null, bool getSpecCount = false, string productName = null);
		List<Cust_products> GetProductsOrdered(IList<int> company_ids, string searchCriteria);
		List<Cust_products> GetByNameOrCode(string criteria);
		List<Cust_products> GetProductData(IList<int> ids);
		List<Cust_products> GetSpareParents(int cprod_id);
		List<Spare> GetSpares(IList<int> cprodIds);
		List<Brand> GetXYBrandsFromProducts();
		List<User> GetFactoryControllerUsers(int cprod_id);
		List<Cust_products> GetProductsForAnalyticsCategories(int? brand_user_id = null, bool useSalesOrders = false);
		List<Range> GetRangesForBrand(int brand_id);
		int GetCountByCriteria(int brand_id, int? range);
		List<Cust_products> GetByBrandAndRange(int brand_id, int? range);
		List<Cust_products> GetDisplayedComponents(int? brand_id = null);
		List<Cust_products> GetCustProductsByCriteria(string searchText, IList<int> clientIds = null);
		List<Cust_products> GetCustProductsByCriteria2(string searchText, IList<int> clientIds = null);
		List<Company> GetDistributorsByFactory(int factory_id);
		List<Company> GetFactoriesForClientsDetails();
		IList<Cust_products> GetAllProductsWithSameCode(IList<int> prodIds);
		List<ProductDate> ProductFirstShipmentDates(IList<int> cprodIds);
		List<Cust_products> GetForAnalyticsSubcats(IList<int> subcat_ids = null);
		List<ProductStats> GetProductStats(int ageForExclusion = 6);
		List<ProductGroupClassReportRow> GetProductGroupClassData(DateTime? from, DateTime? to, bool distributorsOnly = true);
		void UpdateProductGroupId(int cprod_id, int? newValue);
		List<Cust_products> GetByCompanies(List<int?> companyIds, string prefixText);
		void UpdateOtherFiles(Cust_products cp, int? fileType = null);
		void RemoveFile(int cprodId, int fileId);
		//List<Cust_products> GetProductsForClients(IList<int> clientIds);
	}
}