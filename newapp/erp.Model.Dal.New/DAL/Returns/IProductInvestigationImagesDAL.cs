using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IProductInvestigationImagesDAL : IGenericDal<Product_investigation_images>
	{
		List<Product_investigation_images> GetForInvestigation(int id);
		List<Product_investigation_images> GetForProduct(int cprodId);
	}
}