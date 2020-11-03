using erp.Model;

namespace backend.ApiServices
{
	public interface IProductService
	{
		object Create(Cust_products prod);
		object Get(int id, bool includeProductPricingData = false);
		object Update(Cust_products prod);
		string GetCombinedCode(string cprod_code1, string factory_ref);
	}
}