using System.Collections.Generic;
using erp.Model;

namespace backend.ApiServices
{
	public interface ITechnicalDataService
	{
		List<Cust_products> GetCustProductsByCriteria(int subcat_id, List<int?> clientIds = null);
		List<Technical_product_data> GeTechnicalProductDataForMastProduct(int mast_id);
		List<Technical_data_type> GetTechnicalDataTypes();
		List<Technical_product_data> GetTechnicalProductDataForSubCat(int subcat_id);
		List<Technical_subcategory_template> GetTemplateForSubCat(int subcat_id);
		void UpdateTechnicalData(List<Technical_product_data> dataToUpdate, List<Technical_product_data> dataToDelete);
	}
}