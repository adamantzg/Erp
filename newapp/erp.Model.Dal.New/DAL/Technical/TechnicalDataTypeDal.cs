using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class TechnicalDataTypeDal : GenericDal<Technical_data_type>, ITechnicalDataTypeDal
	{
		public TechnicalDataTypeDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM technical_data_type";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO technical_data_type (data_type_desc) VALUES(@data_type_desc)";
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string IdField => "data_type_id";
	}
}
