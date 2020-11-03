using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IAnalyticsCategoryDal : IGenericDal<Analytics_categories>
	{
		List<Analytics_categories> GetForBrand(int? brandId = null);
	}
}
