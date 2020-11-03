
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class ReturnResolutionDAL : GenericDal<Return_resolution>, IReturnResolutionDAL
	{
		public ReturnResolutionDAL(IDbConnection conn) : base(conn)
		{
		}
				

		protected override string IdField => "id";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM return_resolution";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM return_resolution WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO return_resolution (id,resolution) VALUES(@id,@resolution)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE return_resolution SET resolution = @resolution WHERE id = @id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM return_resolution WHERE id = @id";
		}
	}
}
			
			