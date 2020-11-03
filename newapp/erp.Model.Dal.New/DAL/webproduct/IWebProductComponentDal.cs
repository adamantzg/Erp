using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public interface IWebProductComponentDal
	{
		List<Web_product_component> GetAll();
		Web_product_component GetById(int id);
		void Create(Web_product_component o, IDbTransaction tr = null);
		void Update(Web_product_component o);
		void Delete(int cprod_id,int web_unique);
		void DeleteForProduct(int web_unique, IDbTransaction tr = null);
		List<Web_product_component> GetForProduct(int web_unique, IDbConnection conn = null, int? language_id = null);
	}
}
