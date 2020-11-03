using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface ITechnicalProductDataDal : IGenericDal<Technical_product_data>
	{
		List<Technical_product_data> GetByProduct(int mast_id);
	}
}