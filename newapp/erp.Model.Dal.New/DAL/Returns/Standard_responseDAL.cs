
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class StandardResponseDAL : GenericDal<Standard_response>, IStandardResponseDAL
	{
		public StandardResponseDAL(IDbConnection conn) : base(conn)
		{
		}

		protected override string IdField => "response_id";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM standard_response";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM standard_response WHERE response_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO standard_response (response_id,response_type,response_text) VALUES(@response_id,@response_type,@response_text)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE standard_response SET response_type = @response_type,response_text = @response_text WHERE response_id = @response_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM standard_response WHERE response_id = @id";
		}
	}
}
			
			