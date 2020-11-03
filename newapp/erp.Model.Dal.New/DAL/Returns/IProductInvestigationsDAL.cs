using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IProductInvestigationsDAL : IGenericDal<Product_investigations>
	{
		List<Product_investigations> GetClaimInvestigationForProduct(int cprodId);
	}
}