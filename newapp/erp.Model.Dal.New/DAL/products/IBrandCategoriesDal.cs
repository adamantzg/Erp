using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IBrandCategoriesDal : IGenericDal<BrandCategory>
	{
		
		List<BrandCategory> GetBrandCategories(IList<int> brands = null);
		List<BrandCategory> GetBrandCategories(int brand, string language_id = null, bool filterByWebSeq = true);
		List<BrandCategory> GetBrandCategoriesSimple(int brand_id);
		BrandCategory GetCategory(int id, bool loadSubs = false, string language_id = null);
		
	}
}