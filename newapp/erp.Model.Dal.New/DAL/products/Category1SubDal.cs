using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class Category1SubDal : GenericDal<Category1_sub>, ICategory1SubDal
	{
		public Category1SubDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM category1_sub";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM category1_sub WHERE cat2_code = @id";
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
