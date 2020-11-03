using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IWebSiteDal
	{
		List<Web_site> GetAll();
		Web_site GetById(int id);
		Web_site GetByBrandId(int id);
		Web_site GetByCode(string code);
		void Create(Web_site o);
		void Update(Web_site o);
		void Delete(int id);
	}
}