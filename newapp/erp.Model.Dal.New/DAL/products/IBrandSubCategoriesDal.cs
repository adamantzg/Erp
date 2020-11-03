using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IBrandSubCategoriesDal : IGenericDal<BrandSubCategory>
	{
		List<BrandSubCategory> GetAllBrandSubCategories(int brandid, string language_id = null);
		List<BrandSubCategory> GetBrandSubCategories(int catId, string language_id = null);
		BrandSubCategory GetSubCategory(int id, string language_id = null);
	}
}
