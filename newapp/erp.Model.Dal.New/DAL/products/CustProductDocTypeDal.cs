using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class CustProductDocTypeDal : GenericDal<Cust_product_doctype>, ICustProductDocTypeDal
	{
		public CustProductDocTypeDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM cust_product_doctype";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM cust_product_doctype WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO cust_product_doctype (id,name) VALUES(@id,@name)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM cust_product_doctype WHERE id = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE cust_product_doctype SET name = @name WHERE id = @id";
		}

		protected override bool IsAutoKey => false;
	}
}
