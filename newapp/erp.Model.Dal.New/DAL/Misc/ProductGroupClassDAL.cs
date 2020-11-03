using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class ProductGroupClassDAL : GenericDal<product_group_class>, IProductGroupClassDal
	{
		public ProductGroupClassDAL(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM product_group_class";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}
	}
}
