using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class AnalyticsOptionDal : GenericDal<Analytics_options>, IAnalyticsOptionDal
	{
		public AnalyticsOptionDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM Analytics_options";
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
