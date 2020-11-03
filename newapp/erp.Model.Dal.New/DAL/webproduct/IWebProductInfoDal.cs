using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IWebProductInfoDal
	{
		List<Web_product_info> GetAll();
		List<Web_product_info> GetForProduct(int web_unique, IDbConnection conn = null,int? language_id = null);
		Web_product_info GetById(int id);
		void Create(Web_product_info o, IDbTransaction tr = null);
		void Update(Web_product_info o,IDbTransaction tr = null);
		void Delete(int id, IDbTransaction tr = null);
		void DeleteAll();
		void DeleteMissing(int web_unique, IList<int> ids = null, IDbTransaction tr = null);
	}
}