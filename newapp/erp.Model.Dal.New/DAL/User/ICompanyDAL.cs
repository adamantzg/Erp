using System.Collections.Generic;
using System;

namespace erp.Model.Dal.New
{
	public interface ICompanyDAL : IGenericDal<Company>
	{
		
		List<Company> GetDistributors(string brand);
		List<Company> GetDistributors(bool includeHidden = false);
		List<Company> GetNonBrandClients();
		List<Company> GetClients(string prefixText = "", IList<Company_User_Type> companyUserType = null );
		List<Company> GetFactories(string prefixText);
		List<Company> GetClientsFromProducts();
		List<Company> GetFactoriesForProducts();
		List<Company> GetFactories(bool combined = false, int? location_id = null, bool? files = null);
		List<Company> GetFactoriesForLocation(int? location_id = null);
		List<Company> GetFactories(IList<int> ids);
		List<Company> GetByIds(List<int> ids);
		List<Company> GetCompaniesForUser(int userid, Company_User_Type type = Company_User_Type.Factory);
		
		Company GetByFactoryCode(string factory_code);
		Company GetFactoryForProduct(int cprod_id);
		List<Company> GetFactoriesForClients(IList<int> client_ids);
		CompanyType GetCompanyType(int company_id);
		List<Company> GetCompaniesByType(Company_User_Type user_type);
				
		List<int> GetMasterDistributors();
		List<int> GetHeadDistributors();
		List<Company> GetAllSiblings(int companyId);
		List<Company> GetFactoriesByCombinedCode(int combined_factory);
		List<Brand> GetBrands(int company_id);
		Company GetByCustomerCode(string customerCode);
		void RemoveFile(int companyId, int fileId);
		void UpdateFiles(Company c, int? fileType = null);
		List<Company> GetFactoriesWithOrders(DateTime? orderDateLimit = null);
		List<Company> GetClientsWithOrders(DateTime? orderDateLimit = null);
		List<Company> GetFactoriesForBrandAndCategory(int brand_id, int? category1 = null);
	}
}