using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface ICategory1DAL : IGenericDal<Category1>
	{
		List<Category1> GetByBrand(int brand_id);
	}
}