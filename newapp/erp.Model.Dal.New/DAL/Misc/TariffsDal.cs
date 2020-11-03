using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.Model.Dal.New
{
	public class TariffsDal : GenericDal<Tariffs>, ITariffsDal
	{
		public TariffsDal(IDbConnection conn) : base(conn)
		{
		}

		public void Test() { }

		protected override string GetAllSql()
		{
			return "SELECT * FROM Tariffs";
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
