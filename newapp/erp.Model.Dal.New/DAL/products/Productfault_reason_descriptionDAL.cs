
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class ProductfaultReasonDescriptionDAL : GenericDal<Productfault_reason_description>, IProductfaultReasonDescriptionDAL
	{
		public ProductfaultReasonDescriptionDAL(IDbConnection conn) : base(conn)
		{
		}
				

		protected override string IdField => "xpgcode";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM productfault_reason_description";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM productfault_reason_description WHERE xpgcode = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO productfault_reason_description (xpgcode,description) VALUES(@xpgcode,@description)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE productfault_reason_description SET description = @description WHERE xpgcode = @xpgcode";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM productfault_reason_description WHERE xpgcode = @id";
		}
	}
}
			
			