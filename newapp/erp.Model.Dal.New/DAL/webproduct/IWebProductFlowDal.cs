using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IWebProductFlowDal
	{
		List<Web_product_flow> GetAll();
		List<Web_product_flow> GetForProduct(int web_unique, IDbConnection conn = null);
		Web_product_flow GetById(int id);
		void Create(Web_product_flow o, IDbTransaction tr = null);
		void Update(Web_product_flow o, IDbTransaction tr = null);
		void Delete(int id,IDbTransaction tr = null);
		void DeleteMissing(int web_unique, IList<int> ids = null, IDbTransaction tr = null);
	}
}