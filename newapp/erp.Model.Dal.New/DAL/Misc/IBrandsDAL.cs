using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IBrandsDAL : IGenericDal<Brand>
	{
		List<Brand> GetAll(bool eb_brands_only = true);
		List<Brand> GetByIds(IList<int> ids );
		List<Brand> GetByCompanyIds(IList<int> ids);		
		Brand GetByCode(string brand_code);
		Brand GetByCompanyId(int company_id);
		List<Brand> GetBrandsByCompanyId(int company_id);
	}
}