using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IAnalyticsSubcategoryDal : IGenericDal<Analytics_subcategory>
	{
		List<Analytics_subcategory> GetForBrand(int? brandId = null, bool nullBrandOnly = false);
	}
}
